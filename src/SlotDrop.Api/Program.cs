using SlotDrop.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddTelemetry();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();
