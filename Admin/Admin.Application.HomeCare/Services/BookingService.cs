using Admin.Application.HomeCare.Interfaces;
using Admin.Domain.HomeCare.DataModels.Request.Booking;
using Admin.Domain.HomeCare.DataModels.Response.Booking;
using Admin.Domain.HomeCare.Interface;
using AutoMapper;
using Shared.HomeCare.DataModel.Response;
using Shared.HomeCare.Entities;
using Shared.HomeCare.Enums;
using Shared.HomeCare.Interfaces.Repositories;
using Shared.HomeCare.Resources;
using Shared.HomeCare.Services;
using Shared.Interfaces.Services;
using System.Security.Claims;

namespace Admin.Application.HomeCare.Services
{
    public class BookingService(
        IBookingRepository bookingRepository,
        IGenericService<Transaction> transactionService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ClaimsPrincipal principal)
        : GenericService<Booking>(bookingRepository, unitOfWork, mapper, principal),
          IBookingService
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IGenericService<Transaction> _transactionService = transactionService;

        // Parent grid
        public async Task<FilteredDataQueryResponseModel<CustomerBookingSummaryResponse>>
            GetCustomerBookingSummariesAsync(FilterBookingRequestModel filter)
        {
            var result = await _bookingRepository.GetCustomerBookingSummariesAsync(filter);

            return new FilteredDataQueryResponseModel<CustomerBookingSummaryResponse>
            {
                TotalRecords = result.TotalRecords,
                Records = result.Records,
                FilterMeta = result.FilterMeta
            };
        }

        // Child grid
        public Task<IEnumerable<BookingDetailResponse>> GetBookingDetailsByUserIdAsync(
             int userId, FilterBookingRequestModel filter)
             => _bookingRepository.GetBookingDetailsByUserIdAsync(userId, filter);

        public async Task<FilteredDataQueryResponseModel<CustomerBookingDetailResponse>> GetCustomerBookingsAsync(
            int customerId, FilterCustomerBookingsRequestModel filter)
        {
            var result = await _bookingRepository.GetCustomerBookingsAsync(customerId, filter);

            return new FilteredDataQueryResponseModel<CustomerBookingDetailResponse>
            {
                TotalRecords = result.TotalRecords,
                Records = result.Records,
                FilterMeta = result.FilterMeta
            };
        }

        // Available experts
        public Task<IEnumerable<AvailableExpertResponse>> GetAvailableExpertsAsync(
            int serviceTypeId, int? excludeBookingId = null)
            => _bookingRepository.GetAvailableExpertsAsync(serviceTypeId, excludeBookingId);

        // Change Expert
        public async Task<bool> ChangeExpertAsync(ChangeExpertRequestModel request)
        {
            var booking = await FindOrThrowAsync(
                x => x.Id == request.BookingId && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Booking));

            if (booking.Status == BookingStatus.Completed)
                throw new InvalidOperationException(
                    string.Format(Messages.ActionNotAllowed, Messages.Change, Messages.ServicePartner, Messages.Completed));

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException(
                    string.Format(Messages.ActionNotAllowed, Messages.Change, Messages.ServicePartner, Messages.Cancle));

            booking.AssignedPartnerId = request.NewPartnerId;
            await UpdateAsync(booking);
            return true;
        }

        // Status changes
        public async Task<bool> CompleteBookingAsync(int bookingId)
        {
            var booking = await FindOrThrowAsync(
                x => x.Id == bookingId && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Booking));

            if (booking.Status == BookingStatus.Completed)
                throw new InvalidOperationException(Messages.AlreadyCompleted);

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException(Messages.AlreadyCompleted);

            booking.Status = BookingStatus.Completed;
            booking.PaymentStatus = PaymentStatus.Success;
            await UpdateAsync(booking);

            if (booking.AssignedPartnerId.HasValue)
            {
                await _bookingRepository.IncrementPartnerTotalJobsCompletedAsync(booking.AssignedPartnerId.Value);
                await unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> CancelBookingAsync(CancelBookingRequestModel request)
        {
            var booking = await FindOrThrowAsync(
                x => x.Id == request.BookingId && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Booking));

            if (booking.Status == BookingStatus.Completed)
                throw new InvalidOperationException(Messages.AlreadyCompleted);

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException(string.Format(Messages.AlreadyCompleted));

            booking.AssignedPartnerId = null;
            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = request.Reason;
            await UpdateAsync(booking);
            return true;
        }

        // Child-grid DELETE
        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            var booking = await FindOrThrowAsync(
                x => x.Id == bookingId && !x.IsDeleted,
                string.Format(Messages.NotFound, Messages.Booking));

            await SoftDeleteAsync(booking);

            var result = await _transactionService.GetAllAsync(
                t => t.BookingId == bookingId && !t.IsDeleted);

            var transaction = result.Records.FirstOrDefault();

            if (transaction is not null)
                await _transactionService.SoftDeleteAsync(transaction);

            return true;
        }

        // Parent-grid DELETE
        public async Task<bool> DeleteBookingsByPaymentAsync(int userId, PaymentMethod paymentMethod)
        {
            var user = await _bookingRepository.GetUserByIdAsync(userId);
            if (user is null || user.IsDeleted)
                throw new KeyNotFoundException(
                    string.Format(Messages.NotFound, Messages.User));

            var bookings = (await _bookingRepository.GetBookingsByUserAndPaymentAsync(userId, paymentMethod))
                .ToList();

            foreach (var booking in bookings)
            {
                await SoftDeleteAsync(booking);

                if (booking.Transaction is not null && !booking.Transaction.IsDeleted)
                    await _transactionService.SoftDeleteAsync(booking.Transaction);
            }

            return true;
        }
    }
}