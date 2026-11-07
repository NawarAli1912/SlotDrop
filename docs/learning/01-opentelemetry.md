# OpenTelemetry in practice

Three parts: what each piece is and where it sits in the code, how to build with observability in mind, and how to read the result in a dashboard. The examples come from SlotDrop's API (PR #3).

---

## 1. The pieces, mapped to packages and code

It works the same way as `ILogger`. Your code calls an interface, a provider you register decides where the output goes, and a viewer shows it.

| Piece | What it is | In SlotDrop | Package |
|---|---|---|---|
| **API** | The calls that *create* telemetry: start a span, record a number, write a log | `Activity`, `Meter`, `ILogger`. These are built into .NET, and ASP.NET Core and HttpClient already call them. | none (part of .NET) |
| **SDK** | The engine that *collects* it: listens to those calls, adds `service.name`, batches | `builder.Services.AddOpenTelemetry()` | `OpenTelemetry.Extensions.Hosting` |
| **Instrumentation** | Plug-ins that tell the SDK *which* sources to listen to and add the standard details | `.AddAspNetCoreInstrumentation()`, `.AddHttpClientInstrumentation()`, `.AddRuntimeInstrumentation()` | `OpenTelemetry.Instrumentation.*` |
| **Exporter** | *Sends* the collected data somewhere, in a format called **OTLP** | `.UseOtlpExporter()` | `OpenTelemetry.Exporter.OpenTelemetryProtocol` |
| **Backend** | *Stores and shows* the data. Not part of your app. | Aspire dashboard locally, App Insights on Azure | none (a separate program) |

Without an SDK, the API calls do nothing, and that costs almost nothing. That's why libraries can call the API unconditionally and leave it to the app to decide whether to collect.

Request flow in `TelemetryExtensions.cs`:

```
request ─▶ ASP.NET Core creates an Activity (API)
           └─▶ AspNetCore instrumentation: "SDK, listen to this one"
               └─▶ SDK adds service.name=slotdrop-api, batches
                   └─▶ OTLP exporter sends to localhost:4317
                       └─▶ Aspire dashboard stores and shows it
```

Changing backends means changing only the exporter. The rest of the code stays the same.

## 2. The three signals

| Signal | What it is | The question it answers |
|---|---|---|
| **Trace** | The steps of **one** request, as a tree of spans | "Why was *this* request slow, or why did it fail?" |
| **Metric** | A number aggregated over time | "Is the system healthy *in general*? What's the p95 latency?" |
| **Log** | A message from the code at a moment in time | "What did the code say during that request?" |

A **span** is one timed step, such as "handle `GET /health`" or "run a SQL query". A **trace** is all the spans of one request, and they share one **trace ID**. Each span records its **parent**. The first span in a trace has no parent and is called the **root span**.

```
GET /tenants/{slug}          [██████████████] 12 ms   root span (Server)
└─ SELECT tenants            [   █████      ]  5 ms   child span (Client)
```

The same trace ID also appears on the logs written during the request. That's how you jump from a slow span to its logs. Across services, the trace ID travels in the `traceparent` HTTP header, which the instrumentation reads and writes for you.

## 3. Building with observability in mind

What to do, in the order you'll need it:

1. **Name the service.** Set it with `AddService("slotdrop-api")`. Every backend groups data by this name.
2. **Keep names generic.** Use the route *template* (`/tenants/{slug}`), never the raw URL. Every distinct name becomes its own row or time series. With a million tenants, raw URLs would give you a million operations to sort through. ASP.NET Core already does this for you; for example, an unmatched 404 is named just `GET`.
3. **Log with templates inside requests.** Write `logger.LogInformation("Booked {SlotId}", id)`, not `$"Booked {id}"`. The template keeps `SlotId` as a field you can search on. A log written during a request carries that request's trace ID automatically.
4. **Add your own span for work worth timing.** Use `ActivitySource.StartActivity("ReserveSlot")` around a meaningful step, not around every method.
5. **Add your own metric for things you'll want to count.** For example `Counter<long> bookings_created`. Traces are about single requests; metrics are cheap totals.
6. **Keep secrets and personal data out.** Attributes and log fields are stored and shown in plain text.
7. **Prove it with a test.** SlotDrop's integration test asserts a span exists, so a broken setup fails CI.

Which one to reach for:

- a single request is slow or failing → **trace**
- a trend or alert ("errors went up") → **metric**
- you need the code's own explanation → **log**, found through the trace

## 4. Reading the Aspire dashboard (`http://localhost:18888`)

**Traces**
- Each row is one trace. The row shows the root span's name (`GET /health`), the service, its duration and how many spans it has.
- Click a row to open the **waterfall**. Each bar is a span; its width is its duration, and its indent shows its parent. Look for the widest bar: that's where the time went.
- Click a bar to see the span's **attributes**. Check `http.route` (the matched route), `http.response.status_code`, and the error status.
- From a span, **View logs** shows the logs that share its trace ID.

**Structured logs**
- One row per log. Filter by level, or by a field such as `SlotId`.
- The **Trace** column links back to the request the log came from.

**Metrics**
- Pick the service, then a **meter** (a group of metrics, such as `Microsoft.AspNetCore.Hosting`), then an **instrument**.
- `http.server.request.duration` is the main one. It shows **P50 / P90 / P99**; for example, P99 = 99% of requests were faster than this value.
- Nothing shows up the instant it happens: the SDK buffers telemetry and exports it in batches, off the request path. Traces arrive within about 5 s (the batch interval), metrics within about 60 s (the export interval). If a process dies before a flush, that last batch is lost.
- The dashboard keeps everything in memory only. Restarting its container wipes it, and a browser tab opened before the restart keeps showing the old data until you reload it.

A first diagnosis usually goes: metrics ("p99 jumped") → traces filtered to slow ones → open the waterfall → widest bar → logs for that trace.

## 5. Other dashboards: same data, different names

| Concept | Aspire | Azure App Insights (Sprint 2) | Grafana stack |
|---|---|---|---|
| Trace waterfall | Traces → trace | Transaction search → end-to-end transaction | Tempo |
| Trace ID | Trace ID | **Operation ID** | Trace ID |
| Server span | span, kind Server | `requests` table | span |
| Client span (DB, HTTP out) | span, kind Client | `dependencies` table | span |
| Logs | Structured logs | **`traces` table** (careful: in App Insights "traces" means logs) | Loki |
| Metrics | Metrics | Metrics explorer, `customMetrics` | Prometheus / Mimir |

OTel gives every backend the same data, so learning one dashboard teaches you most of the others.

## 6. Words to know

| Term | Meaning |
|---|---|
| OTLP | OpenTelemetry's wire format; gRPC on port 4317, HTTP on 4318 |
| Span kind | Server (incoming), Client (outgoing call), Internal, Producer/Consumer (queues) |
| Attribute | A key–value detail on a span, metric or log, e.g. `http.route=/health` |
| Status | Unset, Ok or Error. Only 5xx marks a server span Error; a 404 is the caller's mistake. |
| Resource | Who sent the data: `service.name`, instance ID |
| Semantic conventions | The standard attribute names (`http.request.method`, …) that every tool understands |
| Cardinality | How many distinct values something has. Keep it low in names and metric labels. |
| Histogram | A metric that keeps a distribution of values, which is where percentiles come from |
| p95 / p99 | 95% / 99% of measurements were below this value |
| Sampling | Recording only some traces, to save cost at high volume |
| `Activity` / `ActivitySource` | .NET's names for a span and a tracer |

## Sources

- OpenTelemetry concepts: https://opentelemetry.io/docs/concepts/
- .NET observability with OTel: https://learn.microsoft.com/dotnet/core/diagnostics/observability-with-otel
- Standalone Aspire dashboard: https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard/standalone
- App Insights with OpenTelemetry: https://learn.microsoft.com/azure/azure-monitor/app/opentelemetry-enable
