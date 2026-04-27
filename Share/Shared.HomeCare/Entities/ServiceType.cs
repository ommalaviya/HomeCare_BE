namespace Shared.HomeCare.Entities
{
    public class ServiceTypes : BaseEntity
    {
        public int Id { get; set; }

        public required string ServiceName { get; set; }

        public string? ImageName { get; set; }

        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}