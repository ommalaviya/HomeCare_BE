using Microsoft.AspNetCore.Http;

namespace Admin.Domain.HomeCare.DataModels.Request.Services
{
    public class UpdateServiceRequestModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int SubCategoryId { get; set; }
        public string? Duration { get; set; } 
        public decimal Price { get; set; }
        public decimal Commission { get; set; }
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
        public List<IFormFile>? NewImages { get; set; }
        public List<int>? DeleteImageIds { get; set; }
        public List<string>? InclusionItems { get; set; }
        public List<string>? ExclusionItems { get; set; }
    }
}