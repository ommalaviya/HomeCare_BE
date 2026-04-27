using Shared.HomeCare.DataModel.Request;

namespace Admin.Domain.HomeCare.DataModels.Request.Transaction
{
    public class FilterTransactionRequestModel : PageRequest
    {
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
