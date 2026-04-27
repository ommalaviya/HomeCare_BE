namespace Public.Application.HomeCare.Interfaces
{
    public interface IReceiptService
    {
        Task<byte[]> GenerateBookingInvoiceAsync(int bookingId);
    }
}