namespace Admin.Domain.HomeCare.DataModels.Response.SubCategory
{
    public class GetSubCategoryResponseModel
    {
        public int Id { get; set; }
        public string? SubCategoryName { get; set; }     
        public int CategoryId { get; set; } 
    }
}