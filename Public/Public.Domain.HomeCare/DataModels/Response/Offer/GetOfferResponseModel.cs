namespace Public.Domain.HomeCare.DataModels.Response.Offer
{
    public class GetOfferResponseModel
    {
        public int Id { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public string? CouponDescription { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int AppliedCount { get; set; }
    }
}