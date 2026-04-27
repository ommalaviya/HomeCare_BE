namespace Shared.HomeCare.Entities
{
    public class ServicesImages : BaseEntity
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        public ServicesOfSubCategory Service { get; set; } = null!;

        public string ImageName { get; set; } = string.Empty;
    }
}