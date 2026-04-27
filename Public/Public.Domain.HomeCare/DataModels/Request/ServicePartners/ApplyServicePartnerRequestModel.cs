using Shared.HomeCare.Enums;

namespace Public.Domain.HomeCare.DataModels.Request.ServicePartners
{
    public class ApplyServicePartnerRequestModel
    {
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string MobileNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int ApplyingForTypeId { get; set; }
        public string PermanentAddress { get; set; } = null!;
        public string ResidentialAddress { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }

        public List<EducationRequestModel> Educations { get; set; } = new();
        public List<ExperienceRequestModel> Experiences { get; set; } = new();
        public List<int> SkillCategoryIds { get; set; } = new();
        public List<int> ServiceSubCategoryIds { get; set; } = new();
        public List<LanguageRequestModel> Languages { get; set; } = new();
        public List<AttachmentRequestModel> Attachments { get; set; } = new();
    }

    public class EducationRequestModel
    {
        public string SchoolCollege { get; set; } = null!;
        public int PassingYear { get; set; }
        public decimal? Marks { get; set; }
    }

    public class ExperienceRequestModel
    {
        public string CompanyName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class LanguageRequestModel
    {
        public int LanguageId { get; set; }
        public Proficiency Proficiency { get; set; }
    }

    public class AttachmentRequestModel
    {
        public string FileUrl { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string? FileType { get; set; }
        public int? FileSizeKb { get; set; }
        public string? DocumentLabel { get; set; }
    }
}