namespace Admin.Domain.HomeCare.DataModels.Response.Category
{
    public class GetCategoryResponseModel
    {
        public int Id { get; set; }
        public string? CategoryName { get; set; } 
        public int ServiceTypeId { get; set; }   
        public string? ServiceTypeName{get;set;}
    }
}