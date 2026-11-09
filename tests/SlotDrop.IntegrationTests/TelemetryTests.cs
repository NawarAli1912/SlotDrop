using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using SlotDrop.Infrastructure.Persistence;

namespace SlotDrop.IntegrationTests;

public class TelemetryTests(ApiFixture api)
{
    [Fact]
    public async Task A_request_is_traced_as_a_server_span_of_slotdrop_api()
    {
        using var trace = new TracedRequest(api.Factory, "/health");

        await trace.SendAsync();

        Assert.True(trace.Response.IsSuccessStatusCode);
        var span = Assert.Single(trace.Spans, activity => activity.Kind == ActivityKind.Server);
        Assert.Equal("GET /health", span.DisplayName);
        Assert.Contains(
            new KeyValuePair<string, object>("service.name", "slotdrop-api"),
            trace.Factory.Services.GetRequiredService<TracerProvider>().GetResource().Attributes);
    }

    [Fact]
    public async Task A_tenant_request_traces_its_sql_query_as_a_child_span()
    {
        using var trace = new TracedRequest(api.Factory, $"/tenants/{SeedData.FadeStudioId}");

        await trace.SendAsync();

        Assert.True(trace.Response.IsSuccessStatusCode);
        var server = Assert.Single(trace.Spans, activity => activity.Kind == ActivityKind.Server);
        Assert.Equal("GET /tenants/{id}", server.DisplayName);
        var query = Assert.Single(trace.Spans, activity => activity.Source.Name == "Npgsql");
        Assert.Equal(ActivityKind.Client, query.Kind);
        Assert.Equal(server.SpanId, query.ParentSpanId);
    }

    // Sends one request with its own trace id and collects only that trace's spans. Test classes run in parallel
    // in one process, and every API host listens to the same ActivitySources, so other tests' spans arrive too.
    private sealed class TracedRequest : IDisposable
    {
        private readonly ActivityTraceId _traceId = ActivityTraceId.CreateRandom();
        private readonly List<Activity> _exported = [];
        private readonly ServerSpanEnded _serverSpanEnded;
        private readonly Uri _uri;

        public TracedRequest(SlotDropApiFactory api, string path)
        {
            _uri = new Uri(path, UriKind.Relative);
            _serverSpanEnded = new ServerSpanEnded(_traceId);
            Factory = api.WithWebHostBuilder(host =>
                host.ConfigureServices(services =>
                    services.ConfigureOpenTelemetryTracerProvider(tracing => tracing
                        .AddInMemoryExporter(_exported)
                        .AddProcessor(_serverSpanEnded))));
        }

        public Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> Factory { get; }

        public HttpResponseMessage Response { get; private set; } = null!;

        // Read only after SendAsync: the in-memory exporter adds to the list without a lock.
        public IEnumerable<Activity> Spans => _exported.Where(activity => activity.TraceId == _traceId);

        public async Task SendAsync()
        {
            using var client = Factory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, _uri);
            // A W3C traceparent header makes ASP.NET Core continue this trace instead of starting a new one.
            request.Headers.Add("traceparent", $"00-{_traceId}-{ActivitySpanId.CreateRandom()}-01");

            Response = await client.SendAsync(request, TestContext.Current.CancellationToken);
            await _serverSpanEnded.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        }

        public void Dispose()
        {
            Response?.Dispose();
            Factory.Dispose();
            _serverSpanEnded.Dispose();
        }
    }

    // TestServer returns the response before hosting stops the server activity, so the test waits for it to end.
    // Processors run in registration order, so the exporter has stored the span when this one fires.
    private sealed class ServerSpanEnded(ActivityTraceId traceId) : BaseProcessor<Activity>
    {
        private readonly TaskCompletionSource _ended = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task Task => _ended.Task;

        public override void OnEnd(Activity data)
        {
            if (data.Kind == ActivityKind.Server && data.TraceId == traceId)
            {
                _ended.TrySetResult();
            }
        }
    }
}
