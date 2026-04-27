namespace Admin.Domain.HomeCare.DataModels.Request.SubCategory
{
    public class CreateSubCategoryRequestModel{
        public string? SubCategoryName { get; set; } 
        public int CategoryId { get; set; }   
    }
}