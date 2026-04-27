using Shared.HomeCare.Entities;
using ServicePartnerEntity = Shared.HomeCare.Entities.ServicePartner;

namespace Admin.Domain.HomeCare.DataModels.Response.ServicePartner
{
    public class ServicePartnerDetailProjection
    {
        public ServicePartnerEntity ServicePartner { get; set; } = null!;

        public IReadOnlyList<ServicePartnerSkill>          Skills          { get; set; } = [];
        public IReadOnlyList<ServicePartnerServiceOffered> ServicesOffered { get; set; } = [];
        public IReadOnlyList<ServicePartnerLanguage>       Languages       { get; set; } = [];
        public IReadOnlyList<ServicePartnerExperience>     Experiences     { get; set; } = [];
        public IReadOnlyList<ServicePartnerAttachment>     Attachments     { get; set; } = [];
        public IReadOnlyDictionary<int, string> CategoryNames    { get; set; } = new Dictionary<int, string>();
        public IReadOnlyDictionary<int, string> SubCategoryNames { get; set; } = new Dictionary<int, string>();
        public IReadOnlyDictionary<int, string> LanguageNames    { get; set; } = new Dictionary<int, string>();

        public string ServiceTypeName { get; set; } = string.Empty;
    }

    public class AssignedServiceRow
    {
        public int    BookingId      { get; set; }
        public int    ServiceId      { get; set; }
        public string ServiceName    { get; set; } = string.Empty;
        public string CustomerName   { get; set; } = string.Empty;
        public string DateAndTime    { get; set; } = string.Empty;
        public string ServiceAddress { get; set; } = string.Empty;
        public string ServiceStatus  { get; set; } = string.Empty;
    }
}