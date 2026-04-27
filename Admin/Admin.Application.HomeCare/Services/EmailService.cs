using System.Net;
using System.Net.Mail;
using Admin.Application.HomeCare.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Admin.Application.HomeCare.Services
{
  public class EmailService : IEmailService
  {
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    private SmtpClient BuildClient(out string username)
    {
      var smtp = _configuration.GetSection("EmailSettings");
      var host = smtp["Host"]!;
      var port = int.Parse(smtp["Port"] ?? "587");
      username = smtp["Username"]!;
      var password = smtp["Password"]!;

      return new SmtpClient(host, port)
      {
        Credentials = new NetworkCredential(username, password),
        EnableSsl = true
      };
    }

    private async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
      var client = BuildClient(out var username);
      var message = new MailMessage
      {
        From = new MailAddress(username, "HomeCare Admin"),
        Subject = subject,
        Body = htmlBody,
        IsBodyHtml = true
      };
      message.To.Add(toEmail);
      await client.SendMailAsync(message);
    }

    private static string WrapHtml(string adminName, string contentHtml) => $@"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8' />
  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
  <meta http-equiv='X-UA-Compatible' content='IE=edge' />
  <title>HomeCare Admin</title>
  <style>
    body {{ margin: 0; padding: 0; background-color: #f3f4f6; font-family: Arial, sans-serif; -webkit-text-size-adjust: 100%; }}
    table {{ border-spacing: 0; mso-table-lspace: 0pt; mso-table-rspace: 0pt; }}
    td {{ padding: 0; }}
    img {{ border: 0; display: block; }}
    .email-wrapper {{ width: 100%; background-color: #f3f4f6; padding: 20px 0; }}
    .email-container {{ max-width: 600px; width: 100%; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.08); }}
    .email-header {{ background-color: #3730A3; padding: 20px 30px; text-align: center; }}
    .email-header h2 {{ color: #ffffff; margin: 0; font-size: 20px; letter-spacing: 0.5px; }}
    .email-body {{ padding: 30px 24px; }}
    .email-footer {{ background-color: #f9fafb; padding: 12px; text-align: center; border-top: 1px solid #e5e7eb; }}
    .email-footer p {{ font-size: 12px; color: #9ca3af; margin: 0; }}
    p {{ color: #374151; line-height: 1.6; margin: 0 0 12px 0; font-size: 14px; word-break: break-word; }}
    .btn-wrap {{ text-align: center; margin: 28px 0; }}
    .btn {{ background-color: #3730A3; color: #ffffff !important; padding: 12px 28px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block; }}
    .info-table {{ width: 100%; border-collapse: collapse; margin: 16px 0; border: 1px solid #e5e7eb; border-radius: 6px; overflow: hidden; font-size: 14px; }}
    .info-table td {{ padding: 10px 14px; word-break: break-word; border-bottom: 1px solid #e5e7eb; vertical-align: top; }}
    .info-table td:first-child {{ font-weight: 600; color: #374151; white-space: nowrap; width: 40%; }}
    .info-table td:last-child {{ color: #111827; }}
    .info-table tr:last-child td {{ border-bottom: none; }}
    .info-table tr:nth-child(odd) {{ background-color: #f9fafb; }}
    .text-red {{ color: #dc2626; font-size: 13px; }}
    .text-gray {{ color: #6b7280; font-size: 13px; }}
    .text-green {{ color: #15803d; }}
    .mono {{ font-family: monospace; letter-spacing: 1px; }}
    @media only screen and (max-width: 600px) {{
      .email-wrapper {{ padding: 10px 0 !important; }}
      .email-container {{ border-radius: 0 !important; box-shadow: none !important; }}
      .email-header {{ padding: 16px 16px !important; }}
      .email-body {{ padding: 20px 16px !important; }}
      .info-table td:first-child {{ width: auto !important; white-space: normal !important; }}
      .btn {{ display: block !important; text-align: center !important; }}
    }}
  </style>
</head>
<body>
  <div class='email-wrapper'>
    <div class='email-container'>
      <div class='email-header'>
        <h2>HomeCare Admin</h2>
      </div>
      <div class='email-body'>
        <p>Hello <strong>{adminName}</strong>,</p>
        {contentHtml}
        <p style='margin-top:24px;'>Regards,<br><strong>HomeCare Team</strong></p>
      </div>
      <div class='email-footer'>
        <p>© {DateTime.UtcNow.Year} HomeCare. All rights reserved.</p>
      </div>
    </div>
  </div>
</body>
</html>";

    public async Task SendResetPasswordEmailAsync(
        string toEmail, string adminName, string resetLink, int expiryMinutes)
    {
      var content = $@"
<p>We received a request to reset your password.</p>
<div class='btn-wrap'>
  <a href='{resetLink}' class='btn'>Reset Password</a>
</div>
<p>This link expires in <strong>{expiryMinutes} minutes</strong>.</p>
<p class='text-gray'>If you did not request this, you can safely ignore this email.</p>";

      await SendAsync(toEmail, "Reset Your HomeCare Password", WrapHtml(adminName, content));
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string adminName, string plainPassword)
    {
      var content = $@"
<p>Your admin account has been created. Here are your login credentials:</p>
<table class='info-table'>
  <tr>
    <td>Email</td>
    <td>{toEmail}</td>
  </tr>
  <tr>
    <td>Password</td>
    <td class='mono'>{plainPassword}</td>
  </tr>
</table>
<p class='text-red'>Please change your password after your first login.</p>";

      await SendAsync(toEmail, "Your HomeCare Admin Account", WrapHtml(adminName, content));
    }

    public async Task SendAccountUpdatedEmailAsync(
        string toEmail,
        string adminName,
        Dictionary<string, string> changedFields)
    {
      var rows = string.Join("", changedFields.Select(kv => $@"
  <tr>
    <td>{kv.Key}</td>
    <td>{kv.Value}</td>
  </tr>"));

      var tableHtml = changedFields.Count > 0
          ? $"<table class='info-table'>{rows}</table>"
          : "<p class='text-gray'>No field details available.</p>";

      var content = $@"
<p>Your admin account details have been updated by a super administrator. Below are the changes:</p>
{tableHtml}
<p class='text-red'>If you did not expect these changes, please contact your system administrator immediately.</p>";

      await SendAsync(toEmail, "Your HomeCare Admin Account Has Been Updated", WrapHtml(adminName, content));
    }

    public async Task SendPasswordChangedEmailAsync(
        string toEmail,
        string adminName,
        string? newPlainPassword = null)
    {
      var passwordRow = newPlainPassword != null
          ? $@"
<table class='info-table'>
  <tr>
    <td>New Password</td>
    <td class='mono'>{newPlainPassword}</td>
  </tr>
</table>
<p class='text-red'>Please change this password immediately after logging in.</p>"
          : "<p class='text-gray'>Your password was changed. If you made this change yourself, no action is needed.</p>";

      var content = $@"
<p>Your HomeCare admin account password has been <strong>changed</strong>.</p>
{passwordRow}
<p class='text-red'>If you did not request this change, please contact your system administrator immediately.</p>";

      await SendAsync(toEmail, "Your HomeCare Admin Password Has Been Changed", WrapHtml(adminName, content));
    }

    public async Task SendAccountDeletedEmailAsync(string toEmail, string adminName)
    {
      var content = $@"
<p>Your HomeCare admin account has been <strong class='text-red'>removed</strong> by a super administrator.</p>
<p class='text-gray'>If you believe this was done in error, please contact your system administrator.</p>";

      await SendAsync(toEmail, "Your HomeCare Admin Account Has Been Removed", WrapHtml(adminName, content));
    }

    public async Task SendAccountReactivatedEmailAsync(
        string toEmail, string adminName, string newPassword)
    {
      var content = $@"
<p>Your HomeCare admin account has been <strong class='text-green'>reactivated</strong> by a super administrator.</p>
<p>Your new login credentials are:</p>
<table class='info-table'>
  <tr>
    <td>Email</td>
    <td>{toEmail}</td>
  </tr>
  <tr>
    <td>New Password</td>
    <td class='mono'>{newPassword}</td>
  </tr>
</table>
<p class='text-red'>Please change your password after your first login.</p>";

      await SendAsync(toEmail, "Your HomeCare Admin Account Has Been Reactivated", WrapHtml(adminName, content));
    }
  }
}