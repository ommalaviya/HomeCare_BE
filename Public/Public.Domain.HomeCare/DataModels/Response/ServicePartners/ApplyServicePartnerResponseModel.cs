namespace Public.Domain.HomeCare.DataModels.Response.ServicePartners
{
    public class ApplyServicePartnerResponseModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string VerificationStatus { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}