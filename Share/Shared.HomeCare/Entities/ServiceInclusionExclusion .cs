using Shared.HomeCare.Enums;
 
namespace Shared.HomeCare.Entities
{
    public class ServiceInclusionExclusion : BaseEntity
    {
        public int Id { get; set; }
 
        public int ServiceId { get; set; }
 
        public ServicesOfSubCategory Service { get; set; } = null!;
 
        public string Item { get; set; } = string.Empty;
 
        public ServiceInclusionExclusionType Type { get; set; }
    }
}