---
title: Facts for the local environment and CI on .NET 10
type: research
for: epic
status: closed
blocked_by: []
---

## Question
What are the current, exact facts for:
- running the standalone Aspire dashboard in Docker Compose (image, ports, OTLP endpoint/protocol, auth settings) and pointing a .NET 10 API at it
- Testcontainers for .NET with Postgres 17 and Redis, including on GitHub Actions `ubuntu-latest`
- a GitHub Actions CI workflow for .NET 10: setup-dotnet version, NuGet caching, `dotnet format --verify-no-changes`, test result reporting, branch protection with required checks

## Resolution
_Researched 2026-11-03. Versions are as of that date; re-check before pinning._

### 1. Standalone Aspire dashboard + OpenTelemetry
- Image is now `mcr.microsoft.com/aspire/dashboard:latest`. `mcr.microsoft.com/dotnet/aspire-dashboard` is legacy (Aspire 13 servicing only). Pin a version tag rather than relying on `latest`. https://aspire.dev/dashboard/standalone/
- Some Microsoft Learn pages still show the legacy image name, so treat aspire.dev as authoritative. https://learn.microsoft.com/dotnet/core/diagnostics/observability-otlp-example
- Container ports: UI `18888`, OTLP/gRPC `18889`, OTLP/HTTP `18890`. The docs map them to host ports 18888, 4317 and 4318. https://aspire.dev/dashboard/standalone/
- The current Aspire CLI is 13.6.0 (2026-09-29). The image version itself isn't documented beyond its tags. https://aspire.dev/dashboard/standalone/
- Login is token-based by default. The token is printed in the container logs (`docker logs aspire-dashboard | grep "login?t="`). https://aspire.dev/dashboard/standalone/
- Local, no auth: `ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true` (default `false`). https://aspire.dev/dashboard/configuration/
- Fixed browser token instead: `DASHBOARD__FRONTEND__AUTHMODE=BrowserToken` (default) and `DASHBOARD__FRONTEND__BROWSERTOKEN=<value>`. These names follow the documented `:`→`__` mapping. https://aspire.dev/dashboard/configuration/
- OTLP auth defaults to `Unsecured`. `DASHBOARD__OTLP__AUTHMODE=ApiKey` with `DASHBOARD__OTLP__PRIMARYAPIKEY` secures it. https://aspire.dev/dashboard/configuration/
- The dashboard is a developer tool, not production monitoring. https://learn.microsoft.com/dotnet/core/diagnostics/observability-otlp-example
- API container env:
  - `OTEL_EXPORTER_OTLP_ENDPOINT=http://aspire-dashboard:18889`. Inside the compose network, use the container port, not the host port.
  - `OTEL_EXPORTER_OTLP_PROTOCOL=grpc` (the default; the other option is `http/protobuf`).
  - `OTEL_SERVICE_NAME=slotdrop-api`.
  - Source: https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.OpenTelemetryProtocol/README.md
- Per-signal `OTEL_EXPORTER_OTLP_{LOGS,METRICS,TRACES}_ENDPOINT` work only with `UseOtlpExporter()`. That call registers OTLP for all three signals at once, may be called only once, and can't be mixed with `AddOtlpExporter`. Prefer it. (same README)
- Core packages, stable 1.20.1 (2026-10-20): `OpenTelemetry.Extensions.Hosting` and `OpenTelemetry.Exporter.OpenTelemetryProtocol`. https://www.nuget.org/packages/OpenTelemetry.Extensions.Hosting
- `OpenTelemetry.Instrumentation.AspNetCore` and `OpenTelemetry.Instrumentation.Http` are the documented set for an ASP.NET Core API. https://learn.microsoft.com/dotnet/core/diagnostics/observability-otlp-example
- `OpenTelemetry.Instrumentation.EntityFrameworkCore` (1.20.1-beta.1, prerelease) registers with `.AddEntityFrameworkCoreInstrumentation()`. https://www.nuget.org/packages/OpenTelemetry.Instrumentation.EntityFrameworkCore
- `Npgsql.OpenTelemetry` registers with `.AddNpgsql()` on tracing. Use it instead of, or alongside, EF instrumentation; both give DB spans. https://www.npgsql.org/doc/diagnostics/tracing.html
- `OpenTelemetry.Instrumentation.StackExchangeRedis` (1.20.1-beta.1, prerelease) registers with `.AddRedisInstrumentation()`. The multiplexer resolves from DI. https://www.nuget.org/packages/OpenTelemetry.Instrumentation.StackExchangeRedis
- Registration sketch, following the Learn sample but using `UseOtlpExporter`:
  - `builder.Logging.AddOpenTelemetry(o => { o.IncludeFormattedMessage = true; o.IncludeScopes = true; });`
  - `builder.Services.AddOpenTelemetry().WithMetrics(m => m.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation()).WithTracing(t => t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddNpgsql().AddRedisInstrumentation()).UseOtlpExporter();`

```yaml
services:
  aspire-dashboard:
    image: mcr.microsoft.com/aspire/dashboard:latest  # pin a version tag
    ports: ["18888:18888"]           # UI; OTLP stays on the internal network
    environment:
      ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS: "true"
  postgres:
    image: postgres:17
    environment: { POSTGRES_PASSWORD: dev, POSTGRES_DB: slotdrop }
    ports: ["5432:5432"]
  redis:
    image: redis:7
    ports: ["6379:6379"]
  api:
    build: .
    depends_on: [aspire-dashboard, postgres, redis]
    environment:
      OTEL_EXPORTER_OTLP_ENDPOINT: http://aspire-dashboard:18889
      OTEL_EXPORTER_OTLP_PROTOCOL: grpc
      OTEL_SERVICE_NAME: slotdrop-api
```
(The Redis tag is a placeholder I didn't research.)

### 2. Testcontainers for .NET
- Module packages: `Testcontainers.PostgreSql` (4.16.0, 2026-10-08) and `Testcontainers.Redis`. https://dotnet.testcontainers.org/modules/ and https://www.nuget.org/packages/Testcontainers.PostgreSql
- Builders take the image in the constructor: `new PostgreSqlBuilder("postgres:17").Build()`, then `StartAsync()` and `GetConnectionString()`. https://dotnet.testcontainers.org/modules/postgres/
- xUnit integration packages: `Testcontainers.XunitV3` for v3 and `Testcontainers.Xunit` for v2. https://dotnet.testcontainers.org/test_frameworks/xunit_net/
- Those packages provide `ContainerTest<TBuilder,TContainer>` (a container per test), `ContainerFixture<,>` (per class, via `IClassFixture`), `DbContainerFixture` / `DbContainerTest` (ADO.NET helpers), and v3 assembly-wide sharing with `[assembly: AssemblyFixture(typeof(...))]`. (same page)
- Plain xUnit pattern: the fixture implements `IAsyncLifetime`. Share it across classes with `[CollectionDefinition]` + `ICollectionFixture<T>` and `[Collection("name")]`. Collection definitions must be in the same assembly as the tests. https://xunit.net/docs/shared-context
- v3 has assembly fixtures (`[assembly: AssemblyFixture]`). They don't change parallelism, so the fixture must be thread-safe. (same page)
- v3 core package is `xunit.v3`. `IAsyncLifetime` now returns `ValueTask` from `InitializeAsync`/`DisposeAsync` and inherits `IAsyncDisposable`. https://xunit.net/docs/getting-started/v3/migration
- CI: GitHub-hosted runners match Microsoft-hosted agents, which ship with Docker pre-installed, so no extra setup is needed. Windows agents can't run Linux containers, so use `ubuntu-latest`. https://dotnet.testcontainers.org/cicd/
- Recommendation: one Postgres + one Redis container per test assembly through an assembly fixture (or a collection fixture), with per-test data reset. This is my inference, not a documented rule.

### 3. GitHub Actions CI for .NET 10
- `actions/setup-dotnet@v6` is current. v6 is an ESM migration with no input changes. v5 moved to node24 and needs runner ≥ 2.327.1. https://github.com/actions/setup-dotnet
- `actions/checkout@v7` is current. https://github.com/actions/checkout
- Pin the SDK with `global-json-file: global.json` and `"sdk": {"version": "10.0.202", "rollForward": "latestFeature"}`.
  - `sdk.version` must be fully qualified; wildcards aren't supported.
  - Setting both `dotnet-version` and `global-json-file` installs both versions.
  - https://github.com/actions/setup-dotnet
- Built-in NuGet cache: `cache: true` hashes `packages.lock.json`, and the action errors if the lock file is missing. https://github.com/actions/setup-dotnet
  - Enable lock files with `RestorePackagesWithLockFile=true` (in Directory.Build.props).
  - Set `NUGET_PACKAGES: ${{ github.workspace }}/.nuget/packages`, run `dotnet restore --locked-mode`, and use `cache-dependency-path: '**/packages.lock.json'` for multiple projects.
  - The only alternative is `actions/cache`. The built-in cache is the simpler choice.
- `dotnet format --verify-no-changes` exits non-zero if any file would change. It does an implicit restore, so add `--no-restore` after the restore step. https://learn.microsoft.com/dotnet/core/tools/dotnet-format
- Test reporting with `dorny/test-reporter@v3`:
  - Use `reporter: dotnet-trx` on `dotnet test --logger "trx;LogFileName=test-results.trx"`.
  - The job needs `checks: write`.
  - It doesn't work on PRs from forks; those need a separate `workflow_run` report workflow.
  - https://github.com/dorny/test-reporter
- Required checks: a ruleset or branch protection rule "Require status checks to pass" blocks merges until the selected checks pass. https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/available-rules-for-rulesets
  - The check must have reported at least once before you can select it, so run CI once first.
  - Optionally pin the source to the GitHub Actions app.
  - "Strict" mode requires the branch to be up to date with the base.
- Keep job names unique across all workflows. Duplicate names make required-check results ambiguous and can block merges. https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/about-protected-branches

```yaml
name: ci
on: { push: { branches: [main] }, pull_request: {} }
permissions: { contents: read, checks: write }
env: { NUGET_PACKAGES: "${{ github.workspace }}/.nuget/packages" }
jobs:
  build-test:            # this job name is what you mark as the required check
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v7
      - uses: actions/setup-dotnet@v6
        with: { global-json-file: global.json, cache: true, cache-dependency-path: "**/packages.lock.json" }
      - run: dotnet restore --locked-mode
      - run: dotnet format --verify-no-changes --no-restore
      - run: dotnet build --no-restore -c Release
      - run: dotnet test --no-build -c Release --logger "trx;LogFileName=test-results.trx"
      - uses: dorny/test-reporter@v3
        if: ${{ !cancelled() }}
        with: { name: Tests, path: "**/*.trx", reporter: dotnet-trx }
```
