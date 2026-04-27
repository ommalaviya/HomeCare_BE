using Shared.HomeCare.DataModel.Request;

namespace Admin.Domain.HomeCare.DataModels.Response.Transaction
{
    public class TransactionDetailResponseModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public Guid TransactionId { get; set; }

        public string MobileNumber { get; set; } = string.Empty;

        public string ServiceName { get; set; } = string.Empty;

        public int ServiceId { get; set; }

        public decimal TransactionAmount { get; set; }

        public string PaymentType { get; set; } = "Service Payment";

        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; }

    }
}