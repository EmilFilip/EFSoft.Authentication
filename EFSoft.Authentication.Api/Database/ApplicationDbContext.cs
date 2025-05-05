using EFSoft.Authentication.Api.Enitites;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EFSoft.Authentication.Api.Database;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
    base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Identity");
    }
}
