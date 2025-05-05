namespace EFSoft.Authentication.Api.Models;

// Request model for confirming password reset
public class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    // This token comes from the email sent during the forgot-password step
    public string Token { get; set; } = string.Empty;
}
