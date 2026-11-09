using SlotDrop.Api;
using SlotDrop.Application.Tenants;
using SlotDrop.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddTelemetry();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

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
app.MapControllers();

await app.Services.MigrateDatabaseAsync();

await app.RunAsync();
