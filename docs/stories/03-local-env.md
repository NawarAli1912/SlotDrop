# Foundations: local environment with traces in the Aspire dashboard

## Business Value
As the developer, I want one command to start the API, built from the same image Azure will run, together with a telemetry dashboard, so that I can see what every request does.

## Description
**Context:**
The API serves only `GET /health` (`src/SlotDrop.Api/Program.cs:3-7`). It has no telemetry, no container image and no Compose file. Story 04 (the walking skeleton) and every later episode rely on traces as evidence, and Sprint 2 deploys a container image. Facts: [Facts for the local environment and CI on .NET 10](../tickets/02-local-env-and-ci.md) §1. Tooling, not a problem episode ([LEARNING-PATH.md](../LEARNING-PATH.md)).

**Scope:**
- `src/SlotDrop.Api/Dockerfile`, hand-written multi-stage: SDK build stage → runtime `mcr.microsoft.com/dotnet/aspnet:10.0.14-noble-chiseled` (pinned patch, non-root, port 8080); `.dockerignore`
- `compose.yaml` at the repo root:
  - `aspire-dashboard`: `mcr.microsoft.com/aspire/dashboard:13.6.2`, `ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true`, ports bound to `127.0.0.1` only: UI 18888, OTLP gRPC host 4317 → 18889; OTLP unsecured inside the Compose network
  - `api`: built from the Dockerfile, env `OTEL_EXPORTER_OTLP_ENDPOINT=http://aspire-dashboard:18889`, `OTEL_SERVICE_NAME=slotdrop-api`, published on `127.0.0.1:8080`
- Telemetry: an `AddTelemetry()` extension in `SlotDrop.Api`, called from `Program.cs`. `UseOtlpExporter()` (one call, all three signals) with ASP.NET Core traces and metrics, HttpClient traces and metrics, .NET runtime metrics, and OTel logs. Packages pinned in `Directory.Packages.props`.
- Running from Rider or `dotnet run` with no extra config: the exporter's default `localhost:4317` reaches the dashboard in Compose
- Config: `appsettings.json` keeps only shared, non-secret defaults (log levels). Anything that differs per environment comes from environment variables. No `appsettings.Production.json`.
- `tests/SlotDrop.IntegrationTests` (new): a `WebApplicationFactory` plus the OTel in-memory exporter. Plain xUnit `Assert`.
- CI: a `docker build` step inside the existing `build-test` job (no push, required check name unchanged)
- README: a "Run locally" section (`docker compose up --build`, dashboard URL, Rider alternative)

**Out of Scope, Known Limitations:**
- Postgres: story 04, with the first query that reads it
- Redis: E8, when the cache arrives
- Azure telemetry (App Insights): Sprint 2 CD story
- Filtering `/health` out of traces: kept on purpose, so the probe noise shows up in Sprint 2 and gets decided with evidence
- Custom `ActivitySource`/`Meter`: when there's domain code to measure
- An assertion library: decided in story 04 (ticket 11)
- Running Compose in CI: the in-memory exporter test covers the regression for far less cost
- Image scanning, base-image update automation: "CI: dependency and image scanning" (Sprint 2)

**Test Scenarios:**
- `docker compose up --build`: dashboard at `http://localhost:18888` with no login; `curl localhost:8080/health` → 200; the trace appears under `slotdrop-api` with its log entries; runtime metrics are listed
- API from Rider with Compose running only the dashboard: the same trace appears
- Integration test: `GET /health` produces one server span with `service.name=slotdrop-api`
- `docker run` the image: process runs as a non-root user and listens on 8080
- CI: a PR that breaks the Dockerfile fails `build-test`

## Functional Acceptance Criteria
**Nominal Use Cases:**
When the developer runs `docker compose up --build` on a clean clone, the API and the dashboard must start. Every request to the API must show up in the dashboard as a trace, with its logs and metrics, under service `slotdrop-api`.

**Corner Cases:**
- Dashboard not running when the API runs from Rider: the API still serves requests; export failures don't crash it
- From another machine on the network: the dashboard and the API are unreachable (bound to `127.0.0.1`)
- `OTEL_EXPORTER_OTLP_ENDPOINT` unset: the API exports to `localhost:4317`

## Technical Acceptance Criteria
- Code Review, including the Secure Code Review Checklist
- External Contract Change (API, exports): No
- Proposed solution:
  - Request: OTLP over gRPC from the API to the dashboard; endpoint and service name from `OTEL_*` environment variables
  - Response: the integration test reads exported spans from the in-memory exporter and asserts the server span and `service.name`

## Testing
- Unit Tests: always
- Component Tests: No
- Integration Tests: Yes (`SlotDrop.IntegrationTests`, in-memory exporter)
- System Tests: No (manual Compose check, with a screenshot in the PR)
- UI Tests (Cypress, ID / Selectors): N/A

## Deployment
- Backward Compatibility Requirement: No
- Monitoring, Alerting Metrics: Yes (local only; Azure in Sprint 2)
  - Logs: Yes (OTel logs to the Aspire dashboard)
  - OpenSearch field mapping: No (locally the Aspire dashboard; `service.name=slotdrop-api`)
  - Alerting: No
  - OpenSearch dashboard / visualization: No (the Aspire dashboard is the local view)
  - Healthcheck: Yes (the existing `GET /health`; no Compose healthcheck because the chiseled image has no curl)

## Required Documentation
- ADR-006 container image (Dockerfile, chiseled noble, non-root, the CI build step) and ADR-007 local environment and telemetry (Compose, anonymous dashboard on localhost, API in Compose plus Rider, `UseOtlpExporter` and its instrumentation, config rule): `docs/adr/`, mirrored to the ClickUp Doc "SlotDrop ADRs" on merge; never in the repo
- README: "Run locally" section


## Dependencies
- Blocked by: Foundations: CI on every pull request (done)
- Blocks: Foundations: walking skeleton reads a tenant through every layer
