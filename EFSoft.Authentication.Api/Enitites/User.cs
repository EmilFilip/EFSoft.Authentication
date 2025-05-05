using Microsoft.AspNetCore.Identity;

namespace EFSoft.Authentication.Api.Enitites;

// Extend IdentityUser to add custom properties
public class ApplicationUser : IdentityUser
{
    // You can add other custom properties here if needed later
    // public DateTime RegistrationDate { get; set; }
}
