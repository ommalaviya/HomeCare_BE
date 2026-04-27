namespace Public.Domain.HomeCare.DataModels.Response.SupportTicket
{
    public class GetSupportTicketResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}