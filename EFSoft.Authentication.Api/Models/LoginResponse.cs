namespace EFSoft.Authentication.Api.Models;

// Response model for successful login
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
