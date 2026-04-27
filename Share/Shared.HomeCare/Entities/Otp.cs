namespace Shared.HomeCare.Entities
{
    public class Otp
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Code { get; set; } = null!;

        public DateTime ExpiryAt { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? RefreshTokenHash { get; set; }

        public DateTime? RefreshTokenExpiryAt { get; set; }
    }
}