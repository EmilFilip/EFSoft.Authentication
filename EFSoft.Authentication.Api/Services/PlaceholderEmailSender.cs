
using System.Diagnostics;

namespace EFSoft.Authentication.Api.Services;

public class PlaceholderEmailSender : IEmailSender
{
    private readonly ILogger<PlaceholderEmailSender> _logger;

    public PlaceholderEmailSender(ILogger<PlaceholderEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        _logger.LogInformation($"Sending email to {email}");
        _logger.LogInformation($"Subject: {subject}");
        _logger.LogInformation($"Message: {htmlMessage}");

        // In development, you might log the email content or write it to a file/console.
        // In production, this would call an actual email sending service/API.

        // For password reset/email confirmation, the htmlMessage would contain a URL like:
        // https://yourapp.com/auth/reset-password?email={email}&token={token}
        // https://yourapp.com/auth/confirm-email?userId={userId}&token={token}
        // Make sure the token is URL-encoded if needed, though Identity tokens are usually URL-safe Base64.

        Debug.WriteLine($"--- EMAIL TO: {email} ---");
        Debug.WriteLine($"Subject: {subject}");
        Debug.WriteLine($"Body: {htmlMessage}");
        Debug.WriteLine("--------------------------");


        return Task.CompletedTask;
    }
}
