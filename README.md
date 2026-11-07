# SlotDrop

[![CI](https://github.com/NawarAli1912/SlotDrop/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/NawarAli1912/SlotDrop/actions/workflows/ci.yml)

A multi-tenant booking platform for barbers and clinics, built to learn double booking under concurrency and slot-drop traffic spikes.

## Prerequisites
- .NET SDK 10.0.403 or a later 10.0 feature band (pinned in `global.json`). On Apple Silicon, use the Arm64 installer.

## Build, test, run
```bash
dotnet build SlotDrop.slnx
dotnet format SlotDrop.slnx --verify-no-changes
dotnet test --solution SlotDrop.slnx
dotnet run --project src/SlotDrop.Api
```
`GET /health` returns `200 Healthy` while the API runs.

## Run locally
Needs Docker. One command starts the API (built from the same image Azure runs) and the Aspire dashboard:
```bash
docker compose up --build
```
- API: http://localhost:8080/health
- Dashboard (traces, logs, metrics; no login): http://localhost:18888. Metrics appear about a minute after start.

To debug from Rider or `dotnet run` instead, start only the dashboard with `docker compose up aspire-dashboard`. The API sends telemetry to `localhost:4317` by default, so no config is needed.

Both are bound to `127.0.0.1`, so they're not reachable from other machines.

## Layout
- `src/SlotDrop.Domain`, `src/SlotDrop.Application`, `src/SlotDrop.Infrastructure`, `src/SlotDrop.Api`: Clean Architecture layers; references point inward only
- `tests/SlotDrop.ArchitectureTests`: rules that keep Domain and Application free of framework types
- `tests/SlotDrop.IntegrationTests`: the API in memory through `WebApplicationFactory`, e.g. that requests are traced
- `src/SlotDrop.Api/Dockerfile`, `compose.yaml`: the API image and the local environment
- `.github/workflows/ci.yml`: the `build-test` check on every PR and every push to `main`
- Build conventions: `global.json`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`
- `docs/`: roadmap, stories and ADRs (start at `docs/MAP.md`)
