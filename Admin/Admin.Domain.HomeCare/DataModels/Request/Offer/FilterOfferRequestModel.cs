using Shared.HomeCare.DataModel.Request;

namespace Admin.Domain.HomeCare.DataModels.Request.Offer
{
    public class FilterOfferRequestModel : PageRequest
    {
        public decimal? DiscountPercentage { get; set; }
        public int? AppliedCountMin { get; set; }
        public int? AppliedCountMax { get; set; }
        public bool? Availability { get; set; }

    }
}