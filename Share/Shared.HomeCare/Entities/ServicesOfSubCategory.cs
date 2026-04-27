namespace Shared.HomeCare.Entities
{
    public class ServicesOfSubCategory : BaseEntity
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int SubCategoryId { get; set; }

        public SubCategory SubCategory { get; set; } = null!;

        public required string Duration { get; set; } =null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal Commission { get; set; }

        public bool IsAvailable { get; set; }

        public ICollection<ServicesImages> Images { get; set; } = new List<ServicesImages>();

        public ICollection<ServiceInclusionExclusion> ServiceFilters { get; set; } = new List<ServiceInclusionExclusion>();
    }
}