namespace Public.Domain.HomeCare.DataModels.Response.ServiceList
{
    public class ServiceTypeWithCategoriesResponseModel
    {
        public string ServiceName { get; set; } = string.Empty;
        public int TotalServiceCount { get; set; }
        public List<CategoryWithSubCategoriesResponseModel> Categories { get; set; } = [];
    }
    public class CategoryWithSubCategoriesResponseModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<SubCategorySlimResponseModel> SubCategories { get; set; } = [];
    }

    public class SubCategorySlimResponseModel
    {
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = string.Empty;
    }
}