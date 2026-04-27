namespace Admin.Domain.HomeCare.DataModels.Response.ServicePartner
{
    public class FileDownloadResult
    {
        public byte[] FileBytes { get; set; } = [];

        public string ContentType { get; set; } = "application/octet-stream";

        public string FileName { get; set; } = "file";
    }
}