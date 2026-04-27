using Microsoft.AspNetCore.Http;

namespace Admin.Domain.HomeCare.DataModels.Request.ServiceTypes
{
    public class UpdateServiceTypeRequestModel
    {
        public int Id { get; set; }
        public required string ServiceName { get; set; }
        public IFormFile? Image { get; set; }
    }
}