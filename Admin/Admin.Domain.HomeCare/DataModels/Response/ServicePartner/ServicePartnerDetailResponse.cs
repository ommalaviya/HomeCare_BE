namespace Admin.Domain.HomeCare.DataModels.Response.ServicePartner
{
    public class ServicePartnerDetailResponse
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string ResidentialAddress { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public decimal TotalWorkExperienceYears { get; set; }

        public string VerificationStatus { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }

        public IEnumerable<ServicePartnerSkillResponse> Skills { get; set; } = [];

        public IEnumerable<ServicePartnerServiceOfferedResponse> ServicesOffered { get; set; } = [];

        public IEnumerable<ServicePartnerLanguageResponse> LanguagesSpoken { get; set; } = [];

        public IEnumerable<ServicePartnerExperienceResponse> PreviousExperiences { get; set; } = [];

        public IEnumerable<ServicePartnerAttachmentResponse> Attachments { get; set; } = [];
    }

    public class ServicePartnerSkillResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class ServicePartnerServiceOfferedResponse
    {
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = string.Empty;
    }

    public class ServicePartnerLanguageResponse
    {
        public int LanguageId { get; set; }
        public string LanguageName { get; set; } = string.Empty;
        public string Proficiency { get; set; } = string.Empty;
    }

    public class ServicePartnerExperienceResponse
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public decimal DurationYears { get; set; }
    }

    public class ServicePartnerAttachmentResponse
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public int? FileSizeKb { get; set; }
        public string? DocumentLabel { get; set; }
    }
}