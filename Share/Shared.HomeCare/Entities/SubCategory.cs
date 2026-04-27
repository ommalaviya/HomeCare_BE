namespace Shared.HomeCare.Entities
{
    public class SubCategory : BaseEntity
    {
        public int Id { get; set; }

        public required string SubCategoryName { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public ICollection<ServicesOfSubCategory> Services { get; set; } = new List<ServicesOfSubCategory>();
    }
}