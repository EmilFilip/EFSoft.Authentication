namespace EFSoft.Authentication.Api.Models;

// Request model for initiating password reset
public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
