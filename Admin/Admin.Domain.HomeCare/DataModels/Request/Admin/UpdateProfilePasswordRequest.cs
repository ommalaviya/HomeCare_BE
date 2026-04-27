using Microsoft.AspNetCore.Http; 

namespace Admin.Domain.HomeCare.DataModels.Request.Admin{
    public class UpdateProfileImageRequest { 
    public IFormFile? Image { get; set; } 
}
}
