using Shared.HomeCare.Enums;

namespace Shared.HomeCare.Entities
{
    public class ServicePartner : BaseEntity
    {
        public int Id { get; set; }

        public required string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public required string MobileNumber { get; set; }

        public required string Email { get; set; }

        public int ApplyingForTypeId { get; set; }

        public required string PermanentAddress { get; set; }

        public required string ResidentialAddress { get; set; }
        
        public string? ProfileImageUrl { get; set; }

        public ServicePartnerStatus Status { get; set; } = ServicePartnerStatus.Pending;

        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Unverified;
        public DateTime? VerifiedAt { get; set; }

        public int? VerifiedBy { get; set; }

        public string? RejectionReason { get; set; }

        public int TotalJobsCompleted { get; set; } = 0;
        
        public ServiceTypes? ServiceTypes { get; set; }
    }
}