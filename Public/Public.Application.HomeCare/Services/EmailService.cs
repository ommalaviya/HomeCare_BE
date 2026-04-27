using Microsoft.Extensions.Configuration;
using Public.Application.HomeCare.Interfaces;
using System.Net;
using System.Net.Mail;
using Shared.HomeCare.Resources;

namespace Public.Application.HomeCare.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtp = _config.GetSection("Smtp");

            var host = smtp["Host"];
            var port = int.Parse(smtp["Port"] ?? "587");
            var email = smtp["Email"];
            var password = smtp["Password"];

            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(email, password),
                EnableSsl = true
            };

            var template = $@"
<html>
<body style='font-family:Arial;background:#f3f4f6;padding:20px;'>
<div style='max-width:600px;margin:auto;background:#ffffff;padding:30px;border-radius:8px;'>
<h2 style='background:#3730A3;color:white;padding:15px;text-align:center;'>HomeCare</h2>
<div style='color:#111827;font-size:15px;line-height:1.6;'>{body}</div>
<hr>
<p style='font-size:12px;color:#777;text-align:center;'>
© {DateTime.UtcNow.Year} HomeCare. All rights reserved.<br/>HomeCare
</p>
</div>
</body>
</html>";

            var message = new MailMessage(
                email ?? throw new InvalidOperationException(
                    string.Format(Messages.NotConfigured, Messages.SmtpEmail)),
                to,
                subject,
                template
            );

            message.IsBodyHtml = true;

            await client.SendMailAsync(message);
        }
    }
}
