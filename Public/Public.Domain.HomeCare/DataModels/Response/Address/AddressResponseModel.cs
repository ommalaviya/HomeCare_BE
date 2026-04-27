namespace Public.Domain.HomeCare.DataModels.Response.Address
{
    public class AddressResponseModel
    {
        public string AddressId { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string HouseFlatNumber { get; set; } = string.Empty;
        public string? Landmark { get; set; }
        public string? FullAddress { get; set; }
        public string? SaveAs { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}