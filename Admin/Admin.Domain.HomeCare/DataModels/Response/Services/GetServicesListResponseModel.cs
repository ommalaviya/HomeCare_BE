namespace Admin.Domain.HomeCare.DataModels.Response.Services
{
    public class GetServicesListResponseModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? SubCategoryName { get; set; }
        public decimal Price { get; set; }
        public decimal Commission { get; set; }
        public bool IsAvailable { get; set; }
    }
}