using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SlotDrop.Infrastructure.Persistence;

// Lets `dotnet ef migrations add` build the model without starting the API.
// Adding a migration never connects, so the connection string is a placeholder.
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>();
        options.UseSlotDropPostgres("Host=localhost");

        return new ApplicationDbContext(options.Options);
    }
}
