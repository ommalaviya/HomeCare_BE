namespace Public.Domain.HomeCare.DataModels.Response.Home
{
    public class ServiceDetailResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
        public List<string> Inclusions { get; set; } = new();
        public List<string> Exclusions { get; set; } = new();
        public List<ServiceResponseModel> RelatedServices { get; set; } = new();
    }
}