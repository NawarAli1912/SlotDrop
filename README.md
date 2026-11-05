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
- `src/SlotDrop.Domain`, `src/SlotDrop.Application`, `src/SlotDrop.Infrastructure`, `src/SlotDrop.Api`: Clean Architecture layers; references point inward only
- `tests/SlotDrop.ArchitectureTests`: rules that keep Domain and Application free of framework types
- Build conventions: `global.json`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`
