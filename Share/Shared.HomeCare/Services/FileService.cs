using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.HomeCare.Interfaces.Services;
using Shared.HomeCare.Resources;

namespace Shared.HomeCare.Services
{
    public class FileService : IFileService
    {
        private readonly string _baseImagePath;
        private readonly string _baseAttachmentPath;
        private const long MaxImageSize = 5 * 1024 * 1024;
        public string BaseImagePath => _baseImagePath;

        private const long MaxAttachmentSize = 20 * 1024 * 1024;

        private static readonly string[] AllowedImageTypes =
            [".jpg", ".jpeg", ".png", ".svg", ".webp", ".bmp", ".avif"];

        private static readonly string[] AllowedAttachmentTypes =
            [".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx"];

        public FileService(IWebHostEnvironment env)
        {
            _baseImagePath = Path.Combine(
                Directory.GetParent(
                    Directory.GetParent(env.ContentRootPath)!.FullName
                )!.FullName,
                "Share", "Shared.HomeCare", "Resources", "Images"
            );

            _baseAttachmentPath = Path.Combine(
                Directory.GetParent(
                    Directory.GetParent(env.ContentRootPath)!.FullName
                )!.FullName,
                "Share", "Shared.HomeCare", "Resources", "Attachments"
            );
        }

        public async Task<string> SaveImageAsync(IFormFile file, string subFolder)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedImageTypes.Contains(extension))
                throw new InvalidOperationException(Messages.InvalidImageType);

            if (file.Length > MaxImageSize)
                throw new InvalidOperationException(Messages.FileSizeExceeds);

            var folderPath = Path.Combine(_baseImagePath, subFolder);
            Directory.CreateDirectory(folderPath);

            var diskName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, diskName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return diskName;
        }

        public async Task<string> SaveAttachmentAsync(IFormFile file, string subFolder)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedAttachmentTypes.Contains(extension))
                throw new InvalidOperationException(Messages.InvalidRequest);

            if (file.Length > MaxAttachmentSize)
                throw new InvalidOperationException(Messages.FileSizeExceeds);

            var folderPath = Path.Combine(_baseAttachmentPath, subFolder);
            Directory.CreateDirectory(folderPath);

            var diskName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, diskName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return diskName;
        }

        public async Task<FileContentHttpResult> GetImageResultAsync(
            string imageName, string subFolder)
        {
            var filePath = Path.Combine(_baseImagePath, subFolder, imageName);
            var bytes = await File.ReadAllBytesAsync(filePath);
            var contentType = ResolveContentType(imageName);
            return TypedResults.File(bytes, contentType, imageName);
        }

        public async Task<FileContentHttpResult> GetFileResultAsync(
            string fileName, string subFolder)
        {
            var filePath = Path.Combine(_baseAttachmentPath, subFolder, fileName);
            var bytes = await File.ReadAllBytesAsync(filePath);
            var contentType = ResolveContentType(fileName);
            return TypedResults.File(bytes, contentType, fileName);
        }

        public async Task<string> UpdateImageAsync(
            IFormFile newFile, string oldImageName, string subFolder)
        {
            // Never delete default image
            if (!IsDefaultImage(oldImageName))
                DeleteImage(oldImageName, subFolder);

            return await SaveImageAsync(newFile, subFolder);
        }

        public void DeleteImage(string imageName, string subFolder)
        {
            if (string.IsNullOrWhiteSpace(imageName)) return;
            if (IsDefaultImage(imageName))
                return;

            var filePath = Path.Combine(_baseImagePath, subFolder, imageName);
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        private static bool IsDefaultImage(string? imageName) =>
            !string.IsNullOrWhiteSpace(imageName) &&
            Path.GetFileName(imageName).StartsWith("defaultimage", StringComparison.OrdinalIgnoreCase);

        public string? GetDefaultImageName(string subFolder)
        {
            var folderPath = Path.Combine(_baseImagePath, subFolder);
            if (!Directory.Exists(folderPath)) return null;

            var defaultFile = Directory
                .EnumerateFiles(folderPath)
                .FirstOrDefault(f =>
                    Path.GetFileName(f).StartsWith("defaultimage", StringComparison.OrdinalIgnoreCase));

            return defaultFile is not null ? Path.GetFileName(defaultFile) : null;
        }

        private static string ResolveContentType(string fileName) =>
            Path.GetExtension(fileName).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".svg" => "image/svg+xml",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
    }
}
