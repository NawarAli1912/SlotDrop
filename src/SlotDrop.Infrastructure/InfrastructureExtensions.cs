using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SlotDrop.Application.Abstractions;
using SlotDrop.Infrastructure.Persistence;

namespace SlotDrop.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSlotDropPostgres(connectionString));
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    // Runs at startup. Safe with one replica; several replicas racing to migrate is a Sprint 5 drill.
    public static async Task MigrateDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var scope = services.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    // One place for provider options, shared by the app and the design-time factory.
    // snake_case names keep raw SQL readable: tenants.id, not "Tenants"."Id".
    internal static DbContextOptionsBuilder UseSlotDropPostgres(this DbContextOptionsBuilder options, string connectionString) =>
        options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
}
