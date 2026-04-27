namespace Public.Domain.HomeCare.DataModels.Response.ServiceList
{
    public class ServiceSearchResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
}