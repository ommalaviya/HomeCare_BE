using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Public.Domain.HomeCare.DataModels.Request.ServicePartners;
using Public.Domain.HomeCare.DataModels.Response.ServicePartners;
using Shared.HomeCare.Entities;
using Shared.Interfaces.Services;

namespace Public.Application.HomeCare.Interfaces
{
    public interface IServicePartnerService : IGenericService<ServicePartner>
    {
        Task<ApplyServicePartnerResponseModel> ApplyAsync(ApplyServicePartnerRequestModel request);
        Task<UploadAttachmentResponseModel> UploadAttachmentAsync(IFormFile file, string? documentLabel);
        Task<string> UploadProfileImageAsync(IFormFile file);
        Task<FileContentHttpResult> GetProfileImageAsync(string id);
    }
}
