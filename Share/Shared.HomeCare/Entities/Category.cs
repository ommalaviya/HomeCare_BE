namespace Shared.HomeCare.Entities
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }

        public required string CategoryName { get; set; }

        public bool IsActive { get; set; } = true;

        public int ServiceTypeId { get; set; }

        public ServiceTypes ServiceTypes { get; set; } = null!;

        public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    }
}