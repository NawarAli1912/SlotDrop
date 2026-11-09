using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Mvc;

using SlotDrop.Application.Tenants;

namespace SlotDrop.Api.Tenants;

[SuppressMessage("Maintainability", "CA1515", Justification = "MVC only discovers public controllers.")]
[ApiController]
[Route("tenants")]
public sealed class TenantsController(GetTenantByIdHandler getTenantById) : ControllerBase
{
    // No {id:guid} route constraint: with one, /tenants/abc matches no route and returns 404.
    // Without it, model binding rejects "abc" and [ApiController] answers 400 ProblemDetails.
    [HttpGet("{id}")]
    [ProducesResponseType<TenantResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await getTenantById.Handle(id, cancellationToken);

        return tenant is null ? NotFound() : Ok(tenant);
    }
}
