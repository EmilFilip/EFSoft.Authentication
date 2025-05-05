namespace EFSoft.Authentication.Api.Models;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    // Consider adding a property for token expiration time (e.g., int ExpiresInMinutes)
    public int TokenLifetimeMinutes { get; set; } = 720; 
}
