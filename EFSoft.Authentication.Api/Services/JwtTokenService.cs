using EFSoft.Authentication.Api.Enitites;
using EFSoft.Authentication.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EFSoft.Authentication.Api.Services;

public class JwtTokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value; // Access settings via .Value
    }

    // Generates a JWT for a given user
    public async Task<string> GenerateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
            {
                // Standard claims
                new Claim(JwtRegisteredClaimNames.Sub, user.Id), // Subject (usually user ID)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // Issued At
                new Claim(JwtRegisteredClaimNames.Email, user.Email), // Email claim
            };

        // Optionally add user roles as claims
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role)); // Add role claim
        }

        // Get the security key and signing credentials
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Define token expiration
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifetimeMinutes);

        // Create the JWT token
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow, // Token is valid from now
            expires: expires, // Token expires at this time
            signingCredentials: creds // Sign the token
        );

        // Write the token as a string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Helper method to get the token expiration time
    public DateTime GetTokenExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifetimeMinutes);
    }
}
