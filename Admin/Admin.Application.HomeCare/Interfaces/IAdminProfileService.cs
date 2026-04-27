    using Admin.Domain.HomeCare.DataModels.Request.Admin;
    using Admin.Domain.HomeCare.DataModels.Response.Admin;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.HttpResults;
    namespace Application.HomeCare.Interfaces
    {
        public interface IAdminProfileService
        {
            Task<AdminProfileResponse> GetProfileAsync();

            Task UpdateContactAsync(UpdateAdminContactRequest model);

            Task UpdatePasswordAsync(UpdateAdminPasswordRequest model);

            Task<FileContentHttpResult> UpdateProfileImageAsync(IFormFile file);
        }
    }