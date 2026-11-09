using Microsoft.EntityFrameworkCore;

using SlotDrop.Application.Abstractions;

namespace SlotDrop.Application.Tenants;

// Red on purpose: a handler without the Handler suffix, using Npgsql's ILike from inside Application.
public sealed class FindTenantsByName(IApplicationDbContext db)
{
    public Task<TenantResponse[]> Handle(string pattern, CancellationToken cancellationToken) =>
        db.Tenants
            .Where(tenant => EF.Functions.ILike(tenant.Name, pattern))
            .Select(tenant => new TenantResponse(tenant.Id, tenant.Name))
            .ToArrayAsync(cancellationToken);
}
