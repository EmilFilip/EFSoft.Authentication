namespace EFSoft.Authentication.Api.Models;

// Request model for confirming email address
public class ConfirmEmailRequest
{
    [Required]
    // UserId is needed to find the user
    public string UserId { get; set; } = string.Empty;

    [Required]
    // This token comes from the confirmation email
    public string Token { get; set; } = string.Empty;
}
