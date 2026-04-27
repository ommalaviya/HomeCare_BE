namespace Public.Domain.HomeCare.DataModels.Response.Payment
{
    public class TransactionIntentResponse
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
    }
}