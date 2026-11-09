using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using Testcontainers.PostgreSql;

// One fixture for the whole test assembly: xUnit creates it before the first test and disposes it after the last.
[assembly: AssemblyFixture(typeof(SlotDrop.IntegrationTests.ApiFixture))]

namespace SlotDrop.IntegrationTests;

// One Postgres container and one API per test run. Tests only read the seed, so sharing them is safe for now;
// E1's tests write data, and isolating them is that sprint's testing lesson.
public sealed class ApiFixture : IAsyncLifetime
{
    // The same image as compose.yaml, so tests run against the engine the app runs against.
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17").Build();

    public SlotDropApiFactory Factory { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        // Starts the container on a random host port and waits until Postgres accepts connections.
        await _postgres.StartAsync();

        Factory = new SlotDropApiFactory(_postgres.GetConnectionString());
    }

    public async ValueTask DisposeAsync()
    {
        await Factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }
}

// The real Program, in memory; the only thing a test changes is which database it talks to.
// The API runs its migrations when the host starts, on the first CreateClient() call.
public sealed class SlotDropApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseSetting("ConnectionStrings:Postgres", connectionString);
    }
}
