namespace Public.Domain.HomeCare.DataModels.Request.Payment
{
    public class ConfirmTransactionRequest
    {
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}