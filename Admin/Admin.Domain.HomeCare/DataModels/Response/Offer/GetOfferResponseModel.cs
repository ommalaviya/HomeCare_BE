namespace Admin.Domain.HomeCare.DataModels.Response.Offer
{
    public class GetOfferResponseModel
    {
        public int Id { get; set; }

        public string? CouponCode { get; set; }

        public string? CouponDescription { get; set; }

        public decimal DiscountPercentage { get; set; }

        public int AppliedCount { get; set; }

        public bool IsActive { get; set; }
    }
}