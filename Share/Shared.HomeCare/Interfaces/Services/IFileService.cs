using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Shared.HomeCare.Interfaces.Services
{
    public interface IFileService
    {
         string BaseImagePath { get; }

        Task<string> SaveImageAsync(IFormFile file, string subFolder);

        Task<string> SaveAttachmentAsync(IFormFile file, string subFolder);

        Task<FileContentHttpResult> GetImageResultAsync(string imageName, string subFolder);

        Task<FileContentHttpResult> GetFileResultAsync(string fileName, string subFolder);

        Task<string> UpdateImageAsync(IFormFile newFile, string oldImageName, string subFolder);

        void DeleteImage(string imageName, string subFolder);

        string? GetDefaultImageName(string subFolder);
    }
}
