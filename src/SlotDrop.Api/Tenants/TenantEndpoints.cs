using Microsoft.AspNetCore.Http.HttpResults;

using SlotDrop.Application.Tenants;

namespace SlotDrop.Api.Tenants;

internal static class TenantEndpoints
{
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        var tenants = app.MapGroup("/tenants");

        // No {id:guid} route constraint: with one, /tenants/abc matches no route and returns 404.
        // Without it, binding "abc" to a Guid fails and the request ends in 400 ProblemDetails.
        tenants.MapGet("/{id}", GetById);

        return app;
    }

    private static async Task<Results<Ok<TenantResponse>, NotFound>> GetById(
        Guid id, GetTenantByIdHandler getTenantById, CancellationToken cancellationToken)
    {
        var tenant = await getTenantById.Handle(id, cancellationToken);

        return tenant is null ? TypedResults.NotFound() : TypedResults.Ok(tenant);
    }
}
