using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace SPMD.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IConfiguration configuration)
        {
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Initial check for unconfigured or placeholder settings
            if (string.IsNullOrEmpty(_emailSettings.SmtpServer) || 
                string.IsNullOrEmpty(_emailSettings.SmtpPassword) || 
                _emailSettings.SmtpPassword == "YOUR_GMAIL_APP_PASSWORD" ||
                _emailSettings.SmtpPassword.Length < 10) // Basic length check for App Password
            {
                LogSimulation(email, subject, htmlMessage, "INCOMPLETE_CONFIG");
                return;
            }

            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                    EnableSsl = _emailSettings.EnableSsl,
                    Timeout = 10000 // 10 second timeout
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                // If SMTP fails (invalid credentials, network issues), fall back to simulation instead of crashing
                LogSimulation(email, subject, htmlMessage, $"SMTP_FAILURE: {ex.Message}");
            }
            catch (Exception ex)
            {
                LogSimulation(email, subject, htmlMessage, $"GENERAL_FAILURE: {ex.Message}");
            }
        }

        private void LogSimulation(string email, string subject, string htmlMessage, string reason)
        {
            Console.WriteLine($"\n--- EMAIL SIMULATION ({reason}) ---");
            Console.WriteLine($"To: {email}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {htmlMessage}");
            Console.WriteLine("-----------------------------------\n");
        }
    }
}
