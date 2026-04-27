namespace Public.Domain.HomeCare.DataModels.Response.Address
{
    public class ReverseGeocodeResponseModel
    {
        public string? DisplayTitle { get; set; }
        public string? DisplaySubtitle { get; set; }
        public string? HouseNumber { get; set; }
        public string? Road { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public string? PostCode { get; set; }
        public string? FullAddress { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}