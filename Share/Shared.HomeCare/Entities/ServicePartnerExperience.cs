namespace Shared.HomeCare.Entities
{
    public class ServicePartnerExperience : BaseEntity
    {
        public int Id { get; set; }

        public int ServicePartnerId { get; set; }

        public required string CompanyName { get; set; }

        public required string Role { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}