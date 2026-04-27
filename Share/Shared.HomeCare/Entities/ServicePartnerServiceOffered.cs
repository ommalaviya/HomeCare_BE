namespace Shared.HomeCare.Entities
{
    public class ServicePartnerServiceOffered : BaseEntity
    {
        public int Id { get; set; }

        public int ServicePartnerId { get; set; }

        public int SubCategoryId { get; set; }
    }
}