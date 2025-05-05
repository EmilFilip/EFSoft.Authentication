// EFSoft.Authentication.Api/Services/CustomUserClaimsPrincipalFactory.cs
using EFSoft.Authentication.Api.Enitites;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using System.Security.Claims;
using System.Threading.Tasks;

namespace EFSoft.Authentication.Api.Services;

// Inherit from UserClaimsPrincipalFactory<TUser, TRole> if you use Roles,
// otherwise UserClaimsPrincipalFactory<TUser> is fine.
public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    public CustomUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        // Generate the base claims first (like sub, username, email etc.)
        var identity = await base.GenerateClaimsAsync(user);

        // Add the User's Id as a claim.
        // ClaimTypes.NameIdentifier is the standard type for user ID.
        if (!identity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
        {
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
        }
        if (!identity.HasClaim(c => c.Type == "sub"))
        {
            identity.AddClaim(new Claim("sub", user.Id));
        }

        // You could add other custom claims here if needed
        // if (!string.IsNullOrEmpty(user.Initials))
        // {
        //     identity.AddClaim(new Claim("initials", user.Initials));
        // }

        return identity;
    }
}