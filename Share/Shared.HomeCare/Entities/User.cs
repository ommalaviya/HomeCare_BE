namespace Shared.HomeCare.Entities
{
    public class User : BaseEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public required string Email { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public string MobileNumber { get; set; } = "0000000000";

        public string Status { get; set; } = "Active";
    }
}