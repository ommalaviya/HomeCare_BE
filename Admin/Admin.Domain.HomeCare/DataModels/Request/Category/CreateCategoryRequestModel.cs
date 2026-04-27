namespace Admin.Domain.HomeCare.DataModels.Request.Category
{
    public class CreateCategoryRequestModel{
        public string? CategoryName { get; set; }
        public int ServiceTypeId { get; set; }
    }
}