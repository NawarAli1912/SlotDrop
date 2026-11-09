using Microsoft.EntityFrameworkCore;

using SlotDrop.Domain.Tenants;

namespace SlotDrop.Application.Abstractions;

// Application owns the shape of the data it reads; Infrastructure decides it is Postgres.
public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
}
