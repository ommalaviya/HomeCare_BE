using Shared.HomeCare.Enums;

namespace Shared.HomeCare.Entities
{
    public class Offer : BaseEntity
    {
        public int Id { get; set; }

        public required string CouponCode { get; set; }

        public string? CouponDescription { get; set; }

        public decimal DiscountPercentage { get; set; }

        public int AppliedCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}