using Microsoft.AspNetCore.Http;

namespace Admin.Domain.HomeCare.DataModels.Request.ServiceTypes
{
    public class CreateServiceTypeRequestModel
    {
        public IFormFile? Image { get; set; }
        public string? ServiceName { get; set; }
    }
}