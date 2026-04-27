namespace Admin.Domain.HomeCare.DataModels.Response.ServicePartner
{
    public class ServicePartnerActionResponse
    {
        public int Id { get; set; }
        public string VerificationStatus { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
