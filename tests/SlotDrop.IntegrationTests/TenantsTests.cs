using System.Net;
using System.Net.Http.Json;

using SlotDrop.Application.Tenants;
using SlotDrop.Infrastructure.Persistence;

namespace SlotDrop.IntegrationTests;

// One API and one Postgres for the whole run (ApiFixture). These tests only read the seeded tenant.
public class TenantsTests(ApiFixture api)
{
    [Fact]
    public async Task Getting_the_seeded_tenant_returns_200_with_its_id_and_name()
    {
        using var client = api.Factory.CreateClient();

        using var response = await client.GetAsync(
            new Uri($"/tenants/{SeedData.FadeStudioId}", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        var tenant = await response.Content.ReadFromJsonAsync<TenantResponse>(TestContext.Current.CancellationToken);
        Assert.Equal(new TenantResponse(SeedData.FadeStudioId, SeedData.FadeStudioName), tenant);
    }

    [Fact]
    public async Task Getting_an_unknown_tenant_returns_404_problem_details()
    {
        using var client = api.Factory.CreateClient();
        var unknownId = Guid.CreateVersion7();

        using var response = await client.GetAsync(
            new Uri($"/tenants/{unknownId}", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Getting_a_tenant_with_a_malformed_id_returns_400_problem_details()
    {
        using var client = api.Factory.CreateClient();

        using var response = await client.GetAsync(
            new Uri("/tenants/abc", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
