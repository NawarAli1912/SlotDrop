using SlotDrop.Api;
using SlotDrop.Api.Tenants;
using SlotDrop.Application.Tenants;
using SlotDrop.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddTelemetry();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
// Minimal APIs throw on a bad parameter (/tenants/abc) in Development and answer 400 elsewhere; the exception
// handler would turn that throw into a 500. One behaviour everywhere, so tests (Development) see what production does.
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);

// Fail at startup, not on the first request, when the environment forgot the database.
var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not set.");
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddScoped<GetTenantByIdHandler>();

var app = builder.Build();

// Every error leaves as application/problem+json: exceptions here, empty 4xx bodies (an unknown route) below.
app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapHealthChecks("/health");
app.MapTenantEndpoints();

await app.Services.MigrateDatabaseAsync();

await app.RunAsync();
