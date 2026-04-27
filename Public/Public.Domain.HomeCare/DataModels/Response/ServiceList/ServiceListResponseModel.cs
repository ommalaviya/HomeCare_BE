namespace Public.Domain.HomeCare.DataModels.Response.ServiceList
{
    public class ServiceListResponseModel
    {
        public string SubCategoryName { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public List<ServiceListItemResponseModel> Services { get; set; } = [];
    }

    public class ServiceListItemResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string? Image { get; set; }
    }
}