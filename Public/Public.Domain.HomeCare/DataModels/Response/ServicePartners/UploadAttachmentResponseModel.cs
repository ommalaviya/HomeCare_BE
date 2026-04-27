namespace Public.Domain.HomeCare.DataModels.Response.ServicePartners
{
    public class UploadAttachmentResponseModel
    {
        public string FileUrl { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string? FileType { get; set; }
        public int? FileSizeKb { get; set; }
    }
}