using System.Security.Claims;
using AutoMapper;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.Booking;
using Public.Domain.HomeCare.DataModels.Response.Booking;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Interfaces.Services;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;

namespace Public.Application.HomeCare.Services
{
    public class BookingService(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal,
        IAddressRepository addressRepository,
        IServiceRepository serviceRepository,
        IOfferService offerService,
        ITransactionRepository transactionRepository,
        IOfferRepository offerRepository,
        IBookingNotificationService bookingNotificationService,
         IUserRepository userRepository)
        : GenericService<Booking>(bookingRepository, unitOfWork, mapper, principal), IBookingService
    {
        private async Task<UserAddress> GetAddressOrThrowAsync(int addressId)
            => await addressRepository.GetByIdAsync(addressId)
               ?? throw new KeyNotFoundException(string.Format(Messages.NotFound, Messages.Address));

        private SlotAvailabilityRequestModel ToSlotRequest(
            CreateBookingRequestModel request, int durationMinutes)
        {
            var slotRequest = mapper.Map<SlotAvailabilityRequestModel>(request);
            slotRequest.KnownDurationMinutes = durationMinutes;
            return slotRequest;
        }

        public async Task<SlotAvailabilityResponseModel> CheckSlotAvailabilityAsync(
            SlotAvailabilityRequestModel request)
        {
            var partner = await bookingRepository.GetAvailablePartnerAsync(request);

            return new SlotAvailabilityResponseModel
            {
                IsAvailable = partner is not null,
                Message = partner is not null ? Messages.SlotAvailable : Messages.SlotUnavailable,
                Partner = partner is not null ? Map<AssignedPartnerModel, ServicePartner>(partner) : null
            };
        }

        public async Task<BookingResponseModel> CreateBookingAsync(CreateBookingRequestModel request)
        {
            var address = await GetAddressOrThrowAsync(request.AddressId);
            if (address.UserId != CurrentUserId)
                throw new UnauthorizedAccessException(Messages.AddressNotBelongsToUser);
                
            var user = await userRepository.GetFreshByIdAsync(CurrentUserId)
                    ?? throw new UnauthorizedAccessException(Messages.Unauthorized);

            if (string.Equals(user.Status, "Block", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(Messages.AccountBlocked);

            var isDuplicate = await bookingRepository.HasUserBookedSameSlotAsync(
                CurrentUserId, request);

            if (isDuplicate)
                throw new InvalidOperationException(Messages.DuplicateBookingSlot);

            var service = await serviceRepository.GetByIdAsync(request.ServiceId)
                ?? throw new KeyNotFoundException(string.Format(Messages.NotFound, Messages.Services));

            if (!service.IsAvailable)
                throw new InvalidOperationException(Messages.ServiceUnavailable);

            if (!int.TryParse(service.Duration, out var durationMinutes) || durationMinutes <= 0)
                throw new InvalidOperationException(Messages.ServiceDurationNotConfigured);

            var slotRequest = ToSlotRequest(request, durationMinutes);
            var partner = await bookingRepository.GetAvailablePartnerAsync(slotRequest)
                ?? throw new InvalidOperationException(Messages.SlotUnavailable);

            var paymentMethod = (PaymentMethod)request.PaymentMethod;
            if (paymentMethod == PaymentMethod.Card)
                throw new InvalidOperationException(Messages.CardPaymentUseIntent);

            var finalAmount = await offerService.CalculateFinalAmountAsync(
                service.Price, request.OfferId);

            var booking = ToEntity(request);
            booking.UserId = CurrentUserId;
            booking.AssignedPartnerId = partner.Id;
            booking.DurationInMinutes = durationMinutes;
            booking.BookingAmount = finalAmount;
            booking.Status = BookingStatus.Pending;
            booking.PaymentStatus = PaymentStatus.Pending;

            await AddAsync(booking);

            var transaction = mapper.Map<Transaction>(booking);
            transaction.PaymentStatus = PaymentStatus.Pending;
            transaction.TransactionDate = DateTime.UtcNow;
            transaction.StripePaymentIntentId = null;

            await transactionRepository.AddAsync(transaction);
            await unitOfWork.SaveChangesAsync();

            if (booking.OfferId.HasValue)
            {
                var couponUsage = new CouponUsage
                {
                    UserId = booking.UserId,
                    CouponId = booking.OfferId.Value,
                    BookingId = booking.Id,
                    UsedAt = DateTime.UtcNow
                };
                await offerRepository.AddCouponUsageAsync(couponUsage);
                await offerRepository.IncrementAppliedCountAsync(booking.OfferId.Value);
                await unitOfWork.SaveChangesAsync();
            }

            var created = await bookingRepository.GetByIdWithDetailsAsync(booking.Id) ?? booking;
            var response = ToResponseModel<BookingResponseModel>(created);

            // SignalR
            _ = bookingNotificationService.NotifyNewBookingAsync(new BookingNotifyRequest
            {
                BookingId = booking.Id,
                UserId = booking.UserId,
                PaymentMethod = paymentMethod.ToString()
            });

            return response;
        }

        public async Task<List<BookingResponseModel>> GetMyBookingsAsync()
        {
            var bookings = await bookingRepository.GetMyBookingsByUserAsync(CurrentUserId);
            return MapToList<BookingResponseModel>(bookings) ?? [];
        }

        public async Task<List<MyBookingResponseModel>> GetMyBookingsByTabAsync(
            MyBookingsRequestModel request)
        {
            var bookings = await bookingRepository.GetMyBookingsAsync(CurrentUserId, request.Tab);
            return mapper.Map<List<MyBookingResponseModel>>(bookings);
        }

        public async Task<BookingResponseModel> GetBookingByIdAsync(int bookingId)
        {
            var booking = await bookingRepository.GetByIdWithDetailsAsync(bookingId)
                ?? throw new KeyNotFoundException(string.Format(Messages.NotFound, Messages.Booking));

            if (booking.UserId != CurrentUserId)
                throw new UnauthorizedAccessException(Messages.Unauthorized);

            return ToResponseModel<BookingResponseModel>(booking);
        }
    }
}