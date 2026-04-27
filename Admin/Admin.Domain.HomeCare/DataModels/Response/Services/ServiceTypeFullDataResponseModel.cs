namespace Admin.Domain.HomeCare.DataModels.Response.Services
{
    public class ServiceTypeFullDataResponseModel
    {
        public List<CategoryWithServicesResponseModel> Categories { get; set; } = [];
    }

    public class CategoryWithServicesResponseModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<SubCategoryWithServicesResponseModel> SubCategories { get; set; } = [];
    }

    public class SubCategoryWithServicesResponseModel
    {
        public int Id { get; set; }
        public string SubCategoryName { get; set; } = string.Empty;
        public List<GetServicesListResponseModel> Services { get; set; } = [];
    }
}