namespace Shared.HomeCare.Entities
{
    public class AdminUser : BaseEntity
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string MobileNumber { get; set; }

        public string? Address { get; set; }

        public required string PasswordHash { get; set; }

        public string? ProfileImageName { get; set; }

        public bool IsSuperAdmin { get; set; } = false;

        public ICollection<AdminPasswordResetToken> PasswordResetTokens { get; set; } = new List<AdminPasswordResetToken>();

        public ICollection<AdminRefreshToken> RefreshTokens { get; set; } = new List<AdminRefreshToken>();
    }
}