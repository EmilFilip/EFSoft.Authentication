using Microsoft.AspNetCore.Identity;

namespace EFSoft.Authentication.Api.Database;

public class User : IdentityUser
{
    public string? Initials { get; set; }
}
