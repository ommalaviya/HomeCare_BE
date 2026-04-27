namespace Shared.HomeCare.Entities
{
    public class Language : BaseEntity
    {
        public int Id { get; set; }

        public required string Name { get; set; }
    }
}