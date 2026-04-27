namespace Admin.Domain.HomeCare.DataModels.Response.Transaction
{
    public class GetUserTransactionResponseModel 
    {
        public int Id { get; set; }

        public Guid TransactionId { get; set; }

        public string ServiceName { get; set; } = string.Empty;

        public decimal TransactionAmount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;
    }
}