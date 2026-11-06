using System.Diagnostics;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SlotDrop.IntegrationTests;

public class TelemetryTests
{
    [Fact]
    public async Task A_request_is_traced_as_a_server_span_of_slotdrop_api()
    {
        List<Activity> exported = [];
        using var serverSpanEnded = new ServerSpanEnded();
        await using var baseFactory = new WebApplicationFactory<Program>();
        await using var factory = baseFactory.WithWebHostBuilder(host =>
            host.ConfigureServices(services =>
                services.ConfigureOpenTelemetryTracerProvider(tracing => tracing
                    .AddInMemoryExporter(exported)
                    .AddProcessor(serverSpanEnded))));
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/health", UriKind.Relative), TestContext.Current.CancellationToken);
        await serverSpanEnded.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.True(response.IsSuccessStatusCode);
        var span = Assert.Single(exported, activity => activity.Kind == ActivityKind.Server);
        Assert.Equal("GET /health", span.DisplayName);
        Assert.Contains(
            new KeyValuePair<string, object>("service.name", "slotdrop-api"),
            factory.Services.GetRequiredService<TracerProvider>().GetResource().Attributes);
    }

    // TestServer returns the response before hosting stops the server activity, so the test waits for it to end.
    // Processors run in registration order, so the exporter has stored the span when this one fires.
    private sealed class ServerSpanEnded : BaseProcessor<Activity>
    {
        private readonly TaskCompletionSource _ended = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task Task => _ended.Task;

        public override void OnEnd(Activity data)
        {
            if (data.Kind == ActivityKind.Server)
            {
                _ended.TrySetResult();
            }
        }
    }
}
