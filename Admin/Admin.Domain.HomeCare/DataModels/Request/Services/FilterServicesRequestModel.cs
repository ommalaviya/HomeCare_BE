using Shared.HomeCare.DataModel.Request;

namespace Admin.Domain.HomeCare.DataModels.Request.Services
{
    public class FilterServicesRequestModel : PageRequest
    {
        public int SubCategoryId { get; set; }
        public int? FilterSubCategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsAvailable { get; set; }
        public decimal? Commission { get; set; }
    }
}