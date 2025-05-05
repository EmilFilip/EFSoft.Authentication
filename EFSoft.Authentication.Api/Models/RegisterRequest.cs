namespace EFSoft.Authentication.Api.Models;

// Request model for user registration
public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    // Password complexity will be handled by IdentityOptions configured in Program.cs
    public string Password { get; set; } = string.Empty;
}
