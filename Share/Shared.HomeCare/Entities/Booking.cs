using Shared.HomeCare.Enums;

namespace Shared.HomeCare.Entities
{
    public class Booking : BaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int ServiceId { get; set; }

        public ServicesOfSubCategory Service { get; set; } = null!;

        public int ServiceTypeId { get; set; }

        public ServiceTypes ServiceType { get; set; } = null!;

        public int? AssignedPartnerId { get; set; }

        public ServicePartner? AssignedPartner { get; set; }

        public int AddressId { get; set; }

        public UserAddress Address { get; set; } = null!;

        public DateOnly BookingDate { get; set; }

        public string BookingTime { get; set; } = string.Empty;

        public int DurationInMinutes { get; set; }

        public int? OfferId { get; set; }

        public Offer? Offer { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public decimal BookingAmount { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public PaymentStatus PaymentStatus { get; set; }

        public string? CancellationReason { get; set; }

        public Transaction? Transaction { get; set; }

        public CouponUsage? CouponUsage { get; set; }
    }
}