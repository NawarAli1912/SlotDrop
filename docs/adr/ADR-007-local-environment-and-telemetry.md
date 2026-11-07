# ADR-007: Local environment and telemetry

Status: accepted, 2026-11-06 (PR #3)

## Context
Every later episode relies on evidence: a trace, a log, a metric. Before story 03 the API had none (`Program.cs` only mapped `/health`). The standalone Aspire dashboard takes OTLP and shows traces, logs and metrics. It's a developer tool, not production monitoring; Azure gets App Insights in Sprint 2. Facts: [Facts for the local environment and CI on .NET 10](../tickets/02-local-env-and-ci.md) §1.

## Options
- What Compose runs now: dashboard + Postgres + Redis · dashboard only, each dependency arriving with the code that first uses it.
- How the API runs locally: only in Compose · only from Rider · both.
- Dashboard login: anonymous on `127.0.0.1` · a fixed browser token in `.env` · the default random token from `docker logs`.
- Telemetry: ASP.NET Core only · + HttpClient + runtime metrics · + custom `ActivitySource`/`Meter`.
- Where the setup lives: inline in `Program.cs` · an extension in the Api · Infrastructure.
- Proof: a manual check · + an in-memory exporter test · a CI job that runs Compose.
- Config: no `appsettings*.json` · shared defaults in `appsettings.json`, environment-specific values from env vars · per-environment files.

## Decision
- `compose.yaml` at the repo root, with two services:
  - `aspire-dashboard`: `mcr.microsoft.com/aspire/dashboard:13.6.2`, anonymous login, ports bound to `127.0.0.1` (UI 18888, OTLP gRPC host 4317 → container 18889), OTLP unsecured inside the Compose network
  - `api`: built from the ADR-006 Dockerfile, `OTEL_EXPORTER_OTLP_ENDPOINT=http://aspire-dashboard:18889`, published on `127.0.0.1:8080`
- Postgres arrives with story 04 and Redis with E8.
- Rider and `dotnet run` work against the same dashboard with no config, because the exporter's default endpoint is `localhost:4317`.
- `AddTelemetry()` in `SlotDrop.Api` (`TelemetryExtensions.cs`): OTel logs (formatted message and scopes), traces from ASP.NET Core and HttpClient, metrics from ASP.NET Core, HttpClient and the runtime, all through one `UseOtlpExporter()` call.
- The service name `slotdrop-api` is set in code with `AddService`, not through `OTEL_SERVICE_NAME`: it's the same in every environment, so it isn't environment config. This differs from the story's Compose env list, and it makes Rider runs and tests named correctly too.
- Config rule: `appsettings.json` holds only shared, non-secret defaults (log levels). Anything that differs per environment comes from environment variables. There are no per-environment files baked into the image.
- `/health` stays in traces. The probe noise should show up in Sprint 2 and get decided with evidence.
- Proof: `tests/SlotDrop.IntegrationTests` runs the API through `WebApplicationFactory` with the in-memory exporter and asserts one server span `GET /health` and `service.name=slotdrop-api`.

## Consequences
- Verified 2026-11-06 with `docker compose up --build`: `/health` → 200; traces, structured logs and metrics (ASP.NET Core, Kestrel, HttpClient, `System.Runtime`) appear under `slotdrop-api` with no login. Metrics take up to 60 s, the exporter's default interval.
- Verified: the API keeps serving 200s when no dashboard is reachable. Export failures don't crash it.
- The dashboard is reachable without a login by anything on this machine. That's accepted for a local tool. Nothing on the network can reach it.
- The test can't just read the exported list after the response. TestServer returns the response before hosting stops the server activity, and the first version of the test failed every time. A small processor, registered after the exporter, signals when the server span has ended. The in-memory exporter adds to its list without a lock, so the test reads the list only after that signal. Proven red by removing ASP.NET Core tracing: the test times out.
- CA2007 (ConfigureAwait) is off for `tests/**`. Tests have no synchronization context, so it only adds noise.
- The OTLP endpoint is unauthenticated inside Compose. Azure uses App Insights through its own exporter (Sprint 2), not this endpoint.
