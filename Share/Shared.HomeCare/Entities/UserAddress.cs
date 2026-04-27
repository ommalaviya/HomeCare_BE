namespace Shared.HomeCare.Entities
{
    public class UserAddress : BaseEntity
    {
        public int AddressId { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public string HouseFlatNumber { get; set; } = string.Empty;

        public string? Landmark { get; set; }

        public string? FullAddress { get; set; }

        public string? SaveAs { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}