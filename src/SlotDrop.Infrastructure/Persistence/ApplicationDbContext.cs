using Microsoft.EntityFrameworkCore;

using SlotDrop.Application.Abstractions;
using SlotDrop.Domain.Tenants;
using SlotDrop.Infrastructure.Persistence.Configurations;

namespace SlotDrop.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfiguration(new TenantConfiguration());
    }
}
