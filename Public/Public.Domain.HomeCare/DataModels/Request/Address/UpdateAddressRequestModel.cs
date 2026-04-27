namespace Public.Domain.HomeCare.DataModels.Request.Address
{
    public class UpdateAddressRequestModel
    {
        public string HouseFlatNumber { get; set; } = string.Empty;

        public string? Landmark { get; set; }

        public string? FullAddress { get; set; }

        public string? SaveAs { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}