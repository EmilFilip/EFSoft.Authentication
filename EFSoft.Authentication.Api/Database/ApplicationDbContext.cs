using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EFSoft.Authentication.Api.Database;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
    base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().Property(u => u.Initials).HasMaxLength(5);
        modelBuilder.HasDefaultSchema("Identity");
    }
}
