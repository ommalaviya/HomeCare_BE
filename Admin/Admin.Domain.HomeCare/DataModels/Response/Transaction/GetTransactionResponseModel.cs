namespace Admin.Domain.HomeCare.DataModels.Response.Transaction
{
    public class GetTransactionResponseModel
    {
        public int Id { get; set; }

        public Guid TransactionId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string ServiceName { get; set; } = string.Empty;

        public decimal TransactionAmount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; }
    }
}
