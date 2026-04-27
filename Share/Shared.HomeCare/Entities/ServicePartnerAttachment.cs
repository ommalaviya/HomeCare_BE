namespace Shared.HomeCare.Entities
{
    public class ServicePartnerAttachment : BaseEntity
    {
        public int Id { get; set; }

        public int ServicePartnerId { get; set; }

        public required string FileUrl { get; set; }

        public required string FileName { get; set; }

        public string? FileType { get; set; }

        public int? FileSizeKb { get; set; }

        public string? DocumentLabel { get; set; }
    }
}