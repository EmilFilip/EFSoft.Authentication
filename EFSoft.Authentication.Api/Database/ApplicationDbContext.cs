namespace EFSoft.Authentication.Api.Database;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
    base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Identity");
    }
}
