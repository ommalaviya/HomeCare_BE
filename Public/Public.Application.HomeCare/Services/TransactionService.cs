using System.Security.Claims;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Public.Application.HomeCare.Interfaces;
using Public.Domain.HomeCare.DataModels.Request.Booking;
using Public.Domain.HomeCare.DataModels.Request.Payment;
using Public.Domain.HomeCare.DataModels.Response.Booking;
using Public.Domain.HomeCare.DataModels.Response.Payment;
using Public.Domain.HomeCare.Interface;
using Shared.HomeCare.DataModel.Request;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Interfaces.Services;
using Shared.HomeCare.Resources;
using Stripe;

namespace Public.Application.HomeCare.Services
{
    public class TransactionService(
        IConfiguration configuration,
        IBookingRepository bookingRepository,
        ITransactionRepository transactionRepository,
        IAddressRepository addressRepository,
        IServiceRepository serviceRepository,
        IOfferService offerService,
        IOfferRepository offerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal,
        PaymentIntentService paymentIntentService,
        IBookingNotificationService bookingNotificationService,
         IUserRepository userRepository) : ITransactionService
    {
        private int CurrentUserId =>
            int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException());
        private readonly IUserRepository userRepository = userRepository;

        public async Task<TransactionIntentResponse> CreateTransactionIntentAsync(CreateTransactionIntentRequest request)
        {
            var address = await addressRepository.GetByIdAsync(request.AddressId)
                ?? throw new KeyNotFoundException(string.Format(Messages.NotFound, Messages.Address));
            if (address.UserId != CurrentUserId)
                throw new UnauthorizedAccessException(Messages.AddressNotBelongsToUser);

            var user = await userRepository.GetFreshByIdAsync(CurrentUserId)
               ?? throw new UnauthorizedAccessException(Messages.Unauthorized);

            if (string.Equals(user.Status, "Block", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(Messages.AccountBlocked);

            var service = await serviceRepository.GetByIdAsync(request.ServiceId)
                ?? throw new KeyNotFoundException(string.Format(Messages.NotFound, Messages.Services));
            if (!int.TryParse(service.Duration, out var durationMinutes) || durationMinutes <= 0)
                throw new InvalidOperationException(Messages.ServiceDurationNotConfigured);

            var slotRequest = mapper.Map<SlotAvailabilityRequestModel>(request);
            slotRequest.KnownDurationMinutes = durationMinutes;

            var partner = await bookingRepository.GetAvailablePartnerAsync(slotRequest)
                ?? throw new InvalidOperationException(Messages.SlotUnavailable);

            var isDuplicate = await bookingRepository.HasUserBookedSameSlotAsync(
                CurrentUserId, request);
            if (isDuplicate)
                throw new InvalidOperationException(Messages.DuplicateBookingSlot);

            var finalAmount = await offerService.CalculateFinalAmountAsync(
                service.Price, request.OfferId);

            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)Math.Round(finalAmount * 100),
                Currency = configuration["Stripe:Currency"] ?? "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    ["userId"] = CurrentUserId.ToString(),
                    ["serviceId"] = request.ServiceId.ToString(),
                    ["serviceTypeId"] = request.ServiceTypeId.ToString(),
                    ["addressId"] = request.AddressId.ToString(),
                    ["bookingDate"] = request.BookingDate.ToString("yyyy-MM-dd"),
                    ["bookingTime"] = request.BookingTime,
                    ["offerId"] = request.OfferId?.ToString() ?? "",
                    ["partnerId"] = partner.Id.ToString(),
                    ["durationMinutes"] = durationMinutes.ToString(),
                    ["bookingAmount"] = finalAmount.ToString("F2")
                }
            };

            var intent = await paymentIntentService.CreateAsync(options);

            var response = mapper.Map<TransactionIntentResponse>(intent);
            response.Amount = finalAmount;

            return response;
        }

        public async Task<BookingResponseModel> ConfirmAndBookAsync(ConfirmTransactionRequest request)
        {
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            var intent = await paymentIntentService.GetAsync(request.PaymentIntentId);

            if (intent.Status != "succeeded")
                throw new InvalidOperationException(Messages.PaymentNotCompleted);

            var existing = await transactionRepository.GetByStripeIntentIdAsync(intent.Id);
            if (existing != null)
                throw new InvalidOperationException(Messages.PaymentAlreadyUsed);

            var meta = intent.Metadata;
            var userId = int.Parse(meta["userId"]);
            if (userId != CurrentUserId)
                throw new UnauthorizedAccessException();

            var booking = new Booking
            {
                UserId = userId,
                ServiceId = int.Parse(meta["serviceId"]),
                ServiceTypeId = int.Parse(meta["serviceTypeId"]),
                AddressId = int.Parse(meta["addressId"]),
                BookingDate = DateOnly.Parse(meta["bookingDate"]),
                BookingTime = meta["bookingTime"],
                PaymentMethod = Shared.HomeCare.Enums.PaymentMethod.Card,
                AssignedPartnerId = int.Parse(meta["partnerId"]),
                DurationInMinutes = int.Parse(meta["durationMinutes"]),
                BookingAmount = decimal.Parse(meta["bookingAmount"]),
                OfferId = string.IsNullOrEmpty(meta["offerId"]) ? null : int.Parse(meta["offerId"]),
                Status = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Success
            };

            await bookingRepository.AddAsync(booking);
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

            var transaction = mapper.Map<Transaction>(booking);
            transaction.PaymentStatus = PaymentStatus.Success;
            transaction.TransactionDate = DateTime.UtcNow;
            transaction.StripePaymentIntentId = intent.Id;

            await transactionRepository.AddAsync(transaction);
            await unitOfWork.SaveChangesAsync();

            var created = await bookingRepository.GetByIdWithDetailsAsync(booking.Id) ?? booking;
            var bookingResponse = mapper.Map<BookingResponseModel>(created);

            // SignalR: notify all connected admin clients
            _ = bookingNotificationService.NotifyNewBookingAsync(new BookingNotifyRequest
            {
                BookingId = booking.Id,
                UserId = booking.UserId,
                PaymentMethod = nameof(Shared.HomeCare.Enums.PaymentMethod.Card)
            });

            return bookingResponse;
        }

        public async Task RecordFailedTransactionAsync(FailedTransactionRequest request)
        {
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
            var intent = await paymentIntentService.GetAsync(request.PaymentIntentId);
            var meta = intent.Metadata;

            var existing = await transactionRepository.GetByStripeIntentIdAsync(intent.Id);
            if (existing != null) return;

            var booking = new Booking
            {
                UserId = int.Parse(meta["userId"]),
                ServiceId = int.Parse(meta["serviceId"]),
                ServiceTypeId = int.Parse(meta["serviceTypeId"]),
                AddressId = int.Parse(meta["addressId"]),
                BookingDate = DateOnly.Parse(meta["bookingDate"]),
                BookingTime = meta["bookingTime"],
                PaymentMethod = Shared.HomeCare.Enums.PaymentMethod.Card,
                AssignedPartnerId = int.Parse(meta["partnerId"]),
                DurationInMinutes = int.Parse(meta["durationMinutes"]),
                BookingAmount = decimal.Parse(meta["bookingAmount"]),
                OfferId = string.IsNullOrEmpty(meta["offerId"]) ? null : int.Parse(meta["offerId"]),
                Status = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Failed
            };

            await bookingRepository.AddAsync(booking);
            await unitOfWork.SaveChangesAsync();

            var transaction = mapper.Map<Transaction>(booking);
            transaction.PaymentStatus = PaymentStatus.Failed;
            transaction.TransactionDate = DateTime.UtcNow;
            transaction.StripePaymentIntentId = intent.Id;
            await transactionRepository.AddAsync(transaction);
            await unitOfWork.SaveChangesAsync();
        }
    }
}