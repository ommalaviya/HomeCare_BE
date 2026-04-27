using Shared.HomeCare.Enums;

namespace Shared.HomeCare.Entities
{
    public class Transaction : BaseEntity
    {
        public int Id { get; set; }

        public Guid TransactionId { get; set; } = Guid.NewGuid();

        public int BookingId { get; set; }

        public Booking Booking { get; set; } = null!;

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int ServiceId { get; set; }

        public ServicesOfSubCategory Service { get; set; } = null!;

        public decimal TransactionAmount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public PaymentStatus PaymentStatus { get; set; }

        public string? StripePaymentIntentId { get; set; }     
    }
}