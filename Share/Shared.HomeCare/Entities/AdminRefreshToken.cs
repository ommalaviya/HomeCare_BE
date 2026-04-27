namespace Shared.HomeCare.Entities
{
    public class AdminRefreshToken
    {
        public int Id { get; set; }

        public int AdminId { get; set; }

        public required string TokenHash { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ReplacedByTokenHash { get; set; }

        public AdminUser Admin { get; set; } = null!;
    }
}