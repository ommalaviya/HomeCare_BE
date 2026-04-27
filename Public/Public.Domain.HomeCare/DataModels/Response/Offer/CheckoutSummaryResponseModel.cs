namespace Public.Domain.HomeCare.DataModels.Response.Offer
{
    public class CheckoutSummaryResponseModel
    {
        public decimal ItemsTotal { get; set; }
        public decimal TaxPercentage { get; set; } = 5;
        public decimal TaxAmount { get; set; }
        public int? AppliedOfferId { get; set; }
        public string? AppliedCouponCode { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}