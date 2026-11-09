using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SlotDrop.Domain.Tenants;

namespace SlotDrop.Infrastructure.Persistence.Configurations;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(tenant => tenant.Id);

        // The domain sets the id; without this EF would replace an empty Guid with its own, non-v7 one.
        builder.Property(tenant => tenant.Id).ValueGeneratedNever();

        builder.HasData(new Tenant(SeedData.FadeStudioId, SeedData.FadeStudioName));
    }
}
