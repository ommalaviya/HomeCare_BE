namespace Admin.Application.HomeCare.Interfaces
{
    public interface IEmailService
    {
        Task SendResetPasswordEmailAsync(
            string toEmail,
            string adminName,
            string resetLink,
            int expiryMinutes);

        Task SendWelcomeEmailAsync(string toEmail, string adminName, string plainPassword);

        Task SendAccountUpdatedEmailAsync(
            string toEmail,
            string adminName,
            Dictionary<string, string> changedFields);

        Task SendPasswordChangedEmailAsync(
            string toEmail,
            string adminName,
            string? newPlainPassword = null);

        Task SendAccountDeletedEmailAsync(string toEmail, string adminName);

        Task SendAccountReactivatedEmailAsync(
            string toEmail,
            string adminName,
            string newPassword);
    }
}