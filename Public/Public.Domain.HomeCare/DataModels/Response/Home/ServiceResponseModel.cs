namespace Public.Domain.HomeCare.DataModels.Response.Home
{
    public class ServiceResponseModel
    {
        public int Id { get; set; }
        public int ServiceTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string SelectedCategoryName { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public Boolean IsAvailable { get; set; }
    }
}