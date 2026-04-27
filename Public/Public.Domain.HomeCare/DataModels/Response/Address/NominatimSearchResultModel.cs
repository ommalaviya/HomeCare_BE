namespace Public.Domain.HomeCare.DataModels.Response.Address
{
    public class NominatimSearchResultModel
    {
        public long PlaceId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Lat { get; set; } = string.Empty;
        public string Lon { get; set; } = string.Empty;
        public NominatimAddressDetail Address { get; set; } = new();
    }

    public class NominatimAddressDetail
    {
        public string? Road { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? Town { get; set; }
        public string? Village { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Postcode { get; set; }
    }
}