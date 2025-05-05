using EFSoft.Authentication.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace EFSoft.Authentication.Api.Extentsions;

public static class MigrationExtentions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using ApplicationDbContext context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        context.Database.Migrate();
    }
}
