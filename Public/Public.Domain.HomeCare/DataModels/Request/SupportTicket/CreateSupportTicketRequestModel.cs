namespace Public.Domain.HomeCare.DataModels.Request.SupportTicket
{
    public class CreateSupportTicketRequestModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Description { get; set; }
    }
}