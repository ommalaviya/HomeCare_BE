namespace Admin.Domain.HomeCare.DataModels.Response.ServicePartner
{
    public class PartnerServiceResponseModel
    {
        public int ServiceId { get; set; }

        public string ServiceName { get; set; } = string.Empty;

        public string SubCategoryName { get; set; } = string.Empty;

        public string Duration { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal Commission { get; set; }

        public bool IsAvailable { get; set; }
    }
}