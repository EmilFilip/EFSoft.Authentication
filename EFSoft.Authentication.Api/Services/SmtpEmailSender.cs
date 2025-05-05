
using EFSoft.Authentication.Api.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace EFSoft.Authentication.Api.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<EmailSettings> emailSettings, ILogger<SmtpEmailSender> logger)
        {
            _emailSettings = emailSettings.Value; // Access settings via .Value
            _logger = logger;

            // Basic check if settings seem configured, although we check in Program.cs too
            if (string.IsNullOrEmpty(_emailSettings.SmtpHost) || string.IsNullOrEmpty(_emailSettings.SmtpUser) || string.IsNullOrEmpty(_emailSettings.SmtpPass) || string.IsNullOrEmpty(_emailSettings.FromAddress))
            {
                _logger.LogWarning("EmailSettings are not fully configured. SmtpEmailSender may fail.");
                // Note: The fallback to PlaceholderEmailSender happens in Program.cs based on this check.
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                // Create the MailMessage
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromAddress),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true, // Assuming HTML content for body
                };
                mailMessage.To.Add(email);

                // Create the SmtpClient
                using (var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort))
                {
                    client.Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                    client.EnableSsl = _emailSettings.EnableSsl;
                    // Optional: Add other SmtpClient settings like Timeout

                    _logger.LogInformation("Attempting to send email to {Email} via {SmtpHost}:{SmtpPort}", email, _emailSettings.SmtpHost, _emailSettings.SmtpPort);

                    // Send the email asynchronously
                    await client.SendMailAsync(mailMessage);

                    _logger.LogInformation("Email sent successfully to {Email}", email);
                }
            }
            catch (Exception ex)
            {
                // Log any errors during email sending
                _logger.LogError(ex, "Failed to send email to {Email}", email);
                // IMPORTANT: Decide how to handle sending failures.
                // For critical emails (like password reset), you might need retry logic
                // or integration with a robust queueing/email service API.
                // For V1, just logging is acceptable per the PRD scope,
                // but be aware this is a potential point of failure in production.

                // Re-throw the exception if you want the API call to fail loudly
                // throw;
                // Or, suppress the exception if you want the API call (e.g., forgot-password)
                // to still return a success status to the user (as per PRD for enumeration prevention).
                // For reset/confirm email *confirmation* failing, you might want to re-throw
                // so the user gets an error response indicating the token failed.
                // Let's re-throw here for general case in this service, but AuthController
                // should catch and handle appropriately (e.g., ForgotPassword returns success anyway).
                throw;
            }
        }
    }
}
