namespace Shared.HomeCare.Entities
{
    public class ServicePartnerEducation : BaseEntity
    {
        public int Id { get; set; }

        public int ServicePartnerId { get; set; }

        public required string SchoolCollege { get; set; }

        public int PassingYear { get; set; }

        public decimal? Marks { get; set; }
    }
}