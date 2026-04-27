namespace Shared.HomeCare.Entities
{
    public class ServicePartnerSkill : BaseEntity
    {
        public int Id { get; set; }

        public int ServicePartnerId { get; set; }

        public int CategoryId { get; set; }
    }
}