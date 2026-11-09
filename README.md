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
`GET /health` returns `200 Healthy` while the API runs. `dotnet run` and the integration tests need Docker: the API reads Postgres at startup, and the tests start their own Postgres with Testcontainers.

## Run locally
Needs Docker. One command starts the API (built from the same image Azure runs), Postgres 17 and the Aspire dashboard:
```bash
docker compose up --build
```
- API: http://localhost:8080/health, and the seeded tenant at http://localhost:8080/tenants/01a1c14b-fcfd-780d-9a99-eba805d438c7
- Dashboard (traces, logs, metrics; no login): http://localhost:18888. Metrics appear about a minute after start.
- Postgres: `localhost:5432`, database and user `slotdrop`, password `dev`. No volume, so `docker compose down` empties it; the API migrates and reseeds on start.

To debug from Rider or `dotnet run` instead, start only the dependencies with `docker compose up aspire-dashboard postgres`. The API sends telemetry to `localhost:4317` by default, and `launchSettings.json` points it at the Compose Postgres, so no config is needed.

Everything is bound to `127.0.0.1`, so nothing is reachable from other machines.

## Migrations
The API applies pending migrations at startup. To add one:
```bash
dotnet ef migrations add <Name> --project src/SlotDrop.Infrastructure --startup-project src/SlotDrop.Infrastructure --output-dir Persistence/Migrations
dotnet ef migrations script --project src/SlotDrop.Infrastructure --startup-project src/SlotDrop.Infrastructure
```
The second command prints the SQL, which goes in the PR for review.

## Layout
- `src/SlotDrop.Domain`, `src/SlotDrop.Application`, `src/SlotDrop.Infrastructure`, `src/SlotDrop.Api`: Clean Architecture layers; references point inward only
- `tests/SlotDrop.ArchitectureTests`: rules that keep Domain and Application free of framework types and Npgsql, and name handlers `*Handler`
- `tests/SlotDrop.IntegrationTests`: the API in memory through `WebApplicationFactory`, against one Testcontainers Postgres shared by the run (`ApiFixture.cs`)
- `src/SlotDrop.Api/Dockerfile`, `compose.yaml`: the API image and the local environment
- `.github/workflows/ci.yml`: the `build-test` check on every PR and every push to `main`
- Build conventions: `global.json`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`
