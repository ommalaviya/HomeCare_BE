namespace Shared.HomeCare.Entities
{
    public class CouponUsage
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int CouponId { get; set; }

        public Offer Coupon { get; set; } = null!;

        public int BookingId { get; set; }

        public Booking Booking { get; set; } = null!;

        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    }
}