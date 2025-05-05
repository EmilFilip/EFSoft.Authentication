namespace EFSoft.Authentication.Api.Models;

// Response model for successful registration
public class RegistrationResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
}
