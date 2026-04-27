using Shared.HomeCare.DataModel.Request;

namespace Admin.Domain.HomeCare.DataModels.Request.Customer{
    public class FilterCustomerRequestModel : PageRequest
    {
        public int? BookingMin { get; set; }

        public int? BookingMax { get; set; }

        public string? Status { get; set; }
    }
}