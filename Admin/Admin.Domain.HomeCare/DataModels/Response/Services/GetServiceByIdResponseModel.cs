namespace Admin.Domain.HomeCare.DataModels.Response.Services
{
    public class GetServiceByIdResponseModel
    {
        public int CategoryId { get; set; }   
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int SubCategoryId { get; set; }
        public string? SubCategoryName { get; set; }
        public string? CategoryName { get; set; }
        public string? ServiceTypeName { get; set; }
        public string? Duration { get; set; }
        public decimal Price { get; set; }
        public decimal Commission { get; set; }
        public bool IsAvailable { get; set; }

        public List<ServiceImageResponseModel> Images { get; set; } = [];
        public List<ServiceFilterItemResponseModel> InclusionItems { get; set; } = [];
        public List<ServiceFilterItemResponseModel> ExclusionItems { get; set; } = [];
    }

    public class ServiceImageResponseModel
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class ServiceFilterItemResponseModel
    {
        public int Id { get; set; }
        public string? Item { get; set; }
    }
}