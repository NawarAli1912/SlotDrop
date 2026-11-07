# ADR-006: Container image for the API

Status: accepted, 2026-11-06 (PR #3)

## Context
Sprint 2 deploys the API to Azure Container Apps, which runs a container image. Story 03 wants one command to start the API locally from the same image Azure will run. Understanding the deploy, and how the deployment target constrains the code, is a stated learning goal ([LEARNING-PATH.md](../LEARNING-PATH.md)). The build is strict (ADR-003: lock files, analyzers as errors, SDK pinned in `global.json` to 10.0.403).

## Options
- How the image is built: a hand-written multi-stage Dockerfile · `dotnet publish /t:PublishContainer` from the SDK (no Dockerfile).
- Runtime base: `aspnet:10.0.x` (Debian, has a shell, runs as root unless changed) · `aspnet:10.0.x-noble-chiseled` (Ubuntu 24.04, no shell or package manager, non-root by default) · `resolute-chiseled` (Ubuntu 26.04).
- Tag: floating (`10.0`) · pinned patch.
- CI: don't build the image · `docker build` in `build-test` · a separate, non-required job.

## Decision
- `src/SlotDrop.Api/Dockerfile`, multi-stage, build context = repo root:
  - build stage `mcr.microsoft.com/dotnet/sdk:10.0.403-noble` (matches `global.json`)
  - copy the solution-wide props and each project's `.csproj` + `packages.lock.json` first, then `dotnet restore --locked-mode`, so the restore layer stays cached until a dependency changes
  - copy `src/`, `dotnet publish --no-restore -c Release`
  - runtime stage `mcr.microsoft.com/dotnet/aspnet:10.0.14-noble-chiseled`
- `.dockerignore` keeps `bin/`, `obj/`, `.git/`, `tests/` and IDE files out of the context.
- CI: a `docker build` step at the end of `build-test` (not pushed). The ruleset (ADR-005) is unchanged.

## Consequences
- Verified 2026-11-06: the image runs as UID 1654 (`app`), listens on 8080 (`ASPNETCORE_HTTP_PORTS=8080`), and is 186 MB uncompressed.
- No shell in the image: no `docker exec sh`, no `curl` for a Compose `healthcheck`. Debugging happens through telemetry and logs, which is how it works on Container Apps anyway. Container Apps probes `/health` over HTTP, so it doesn't need curl.
- A broken Dockerfile fails the PR. The step costs roughly a minute per run, more with a cold Docker layer cache.
- Base-image patches don't arrive by themselves: `10.0.14` stays until someone bumps it. Automated updates and image scanning come with "CI: dependency and image scanning" (Sprint 2).
- The Dockerfile names every project, so a new `src/` project means a new `COPY` line for its project file and lock file. This was accepted as the price of a cached restore layer.
- Grooming picked `10.0.9`. MCR's tag list is sorted as text, so `10.0.14` showed up before `10.0.9`; the newest patch is `10.0.14`.
