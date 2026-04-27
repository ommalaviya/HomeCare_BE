using Shared.HomeCare.Enums;

namespace Shared.HomeCare.Entities
{
    public class ServicePartnerLanguage : BaseEntity
    {
        public int Id { get; set; }

        public int ServicePartnerId { get; set; }

        public int LanguageId { get; set; }

        public Proficiency Proficiency { get; set; }
    }
}