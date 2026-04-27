namespace Shared.HomeCare.Entities
{
    public class AdminPasswordResetToken
    {
        public int Id { get; set; }

        public int AdminId { get; set; }

        public required string Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AdminUser Admin { get; set; } = null!;
    }
}