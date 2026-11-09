using Microsoft.EntityFrameworkCore;

using SlotDrop.Application.Abstractions;

namespace SlotDrop.Application.Tenants;

// A concrete class with no interface or dispatcher: one caller doesn't need either yet (ADR-008).
public sealed class GetTenantByIdHandler(IApplicationDbContext db)
{
    // null means "no such tenant"; the controller turns it into a 404.
    public Task<TenantResponse?> Handle(Guid id, CancellationToken cancellationToken) =>
        db.Tenants
            .AsNoTracking()
            .Where(tenant => tenant.Id == id)
            .Select(tenant => new TenantResponse(tenant.Id, tenant.Name))
            .SingleOrDefaultAsync(cancellationToken);
}
