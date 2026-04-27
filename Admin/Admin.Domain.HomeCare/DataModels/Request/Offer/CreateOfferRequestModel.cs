namespace Admin.Domain.HomeCare.DataModels.Request.Offer
{
    public class CreateOfferRequestModel
    {
        public string? CouponCode { get; set; }

        public string? CouponDescription { get; set; }

        public decimal DiscountPercentage { get; set; }

        public bool IsActive { get; set; }
    }
}