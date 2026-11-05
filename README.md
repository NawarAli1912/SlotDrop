# SlotDrop

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

## Layout
- `src/SlotDrop.Domain`, `src/SlotDrop.Application`, `src/SlotDrop.Infrastructure`, `src/SlotDrop.Api`: Clean Architecture layers ([ADR-001](docs/adr/ADR-001-solution-structure.md))
- `tests/SlotDrop.ArchitectureTests`: rules that keep the layers clean ([ADR-002](docs/adr/ADR-002-architecture-tests.md))
- Build and test conventions: [ADR-003](docs/adr/ADR-003-build-and-test-conventions.md)
