---
title: How Milan Jovanović structures a .NET 10 Clean Architecture service today
type: research
for: epic
status: closed
blocked_by: []
---

## Question
What does Milan Jovanović currently recommend for a .NET 10 Clean Architecture solution? Specifically:
- project layout
- vertical slices vs. layers inside Application
- CQRS without MediatR?
- Result pattern
- validation
- architecture-test library
- Directory.Build.props / central package management
- OpenTelemetry setup
- EF Core configuration

Sprint 1's skeleton story and ADR-001 depend on this.

## Resolution
Researched 2026-11-03. T = his free template page https://www.milanjovanovic.tech/templates/clean-architecture (page says "updated 6 months ago" (~May 2026), targets .NET 10). The template has no public GitHub repo: you get it by email. Neither `m-jovanovic/clean-architecture` nor any equivalent repo exists.

**1. Layout**
- There are 5 src projects: Domain (no deps), Application (use cases), Infrastructure (persistence/auth/caching), Web.Api (Minimal APIs + composition root), and SharedKernel (Result, Error, primitives). Source: T (~May 2026)
- There are 3 test projects: ArchitectureTests, Application.UnitTests, and IntegrationTests (Testcontainers). Source: T (~May 2026)

**2. Application structure and CQRS**
- He uses a hybrid. Keep the Clean Architecture project boundaries, and put feature folders inside Application (`Orders/PlaceOrder/{Command,Handler,Validator}`). The API mirrors that structure, and Domain groups by aggregate. Sources: https://milanjovanovic.tech/blog/feature-folders-dotnet (2026-08-13) and https://milanjovanovic.tech/blog/vertical-slice-vs-clean-architecture (2026-08-13)
- He dropped MediatR. He hand-rolls `ICommand`/`ICommand<T>`/`IQuery<T>` with `ICommandHandler`/`IQueryHandler` that return `Result`/`Result<T>`. Scrutor scans and `Decorate`s them, with logging and validation decorators (none for validation on queries). He cites MediatR going commercial as a reason. Source: https://www.milanjovanovic.tech/blog/cqrs-pattern-the-way-it-should-have-been-from-the-start (2025-05-17)
- In the template, handlers are injected directly into endpoints with no dispatcher. Source: T (~May 2026)
- "CQRS ≠ MediatR". Source: https://www.milanjovanovic.tech/blog/stop-conflating-cqrs-and-mediatr (Feb 2025)

**3. Errors, validation, endpoints**
- He uses a custom `Result`/`Result<T>` + `Error` instead of exceptions for flow control, and `Match` maps a result to an HTTP response. Source: https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern (2023-10-28)
- In the template, a failed Result maps to ProblemDetails. Source: T (~May 2026)
- For ProblemDetails he uses `AddProblemDetails` + `UseExceptionHandler` + `IExceptionHandler`, plus `CustomizeProblemDetails` to add `requestId`/`traceId`. Source: https://milanjovanovic.tech/blog/problem-details-for-aspnetcore-apis (2024-10-19)
- Validation uses FluentValidation `IValidator<T>` inside a validation decorator, which returns a `ValidationError` Result. Source: CQRS post above (2025-05-17)
- He uses Minimal APIs, not controllers. Each endpoint is one `IEndpoint` class (`MapEndpoint(IEndpointRouteBuilder)`), found by assembly scanning and mapped through one `MapEndpoints()` extension. Sources: https://www.milanjovanovic.tech/blog/automatically-register-minimal-apis-in-aspnetcore (2024-02-24) and T

**4. Architecture tests**
- His current library is ArchUnitNET (`TngTech.ArchUnitNET.xUnit`). He gives 5 rules:
  - layer direction
  - naming (`*CommandHandler`, validators only in Application)
  - handler colocated with its command (done with reflection)
  - handlers are `internal`
  - Domain/Application must not touch `Microsoft.EntityFrameworkCore`
  - Source: https://www.milanjovanovic.tech/blog/5-architecture-tests-you-should-add-to-your-dotnet-projects (2026-03-07)
- His earlier library was NetArchTest ("I prefer"). Example rules: domain events are sealed, entities have no public constructors, handler naming. Source: https://www.milanjovanovic.tech/blog/shift-left-with-architecture-testing-in-dotnet (2024-05-25)

**5. Build hygiene**
- His Directory.Build.props sets:
  - `AnalysisLevel=latest`
  - `AnalysisMode=All`
  - `TreatWarningsAsErrors`, `CodeAnalysisTreatWarningsAsErrors` and `EnforceCodeStyleInBuild` all `true`
  - SonarAnalyzer.CSharp
  - .editorconfig silences noisy rules (S125, S1075, S2094, S3267)
  - Source: https://www.milanjovanovic.tech/blog/improving-code-quality-in-csharp-with-static-code-analysis (2024-08-03)
- He uses CPM by default with Directory.Packages.props. Use `GlobalPackageReference` sparingly (Sonar is his example) and add a comment to every `VersionOverride`. Source: https://milanjovanovic.tech/blog/central-package-management-in-net-simplify-nuget-dependencies (2024-12-07)
- Microsoft's docs cover `ManagePackageVersionsCentrally` and the opt-in `CentralPackageTransitivePinningEnabled`. Source: https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management (updated 2026-03-03)

**6. Observability**
- He uses OpenTelemetry for logs, traces and metrics:
  - logs via `builder.Logging.AddOpenTelemetry` (IncludeScopes, IncludeFormattedMessage)
  - traces: AspNetCore, Http and Npgsql instrumentation
  - metrics: AspNetCore and Http instrumentation
  - a single `UseOtlpExporter()` driven by `OTEL_EXPORTER_OTLP_ENDPOINT`
  - the standalone Aspire dashboard container (UI on 18888, OTLP on 18889, in-memory only)
  - no Serilog in this setup
  - Source: https://milanjovanovic.tech/blog/standalone-aspire-dashboard-setup-for-distributed-dotnet-applications (2025-08-30)
- The template exports OTLP to a bundled Grafana/Loki/Tempo stack via docker compose. Source: T (~May 2026)
- Microsoft calls the Aspire dashboard "a developer visualization tool, not a production monitoring tool". Source: https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-otlp-example (updated 2026-09-22)
- His older guidance is Serilog configured through appsettings, with enrichers and request logging. Source: https://www.milanjovanovic.tech/blog/5-serilog-best-practices-for-better-structured-logging (2023-12-16)

**7. EF Core and SQL**
- The template uses EF Core + Postgres with snake_case naming, per-entity configurations, migrations, and a domain-event dispatcher, all behind an Application-owned interface. It also uses HybridCache. Source: T (~May 2026)
- He uses EF Core for writes and Dapper/raw SQL for reads. Both can share a connection and transaction, chosen per use case. Source: https://milanjovanovic.tech/blog/ef-core-vs-dapper (2026-07-04)
- He prefers generated SQL scripts for applying migrations, and says startup `Migrate()` is only for local/test. Source: https://milanjovanovic.tech/blog/efcore-migrations-a-detailed-guide (2024-05-18)
- Gap: I found no primary Milan post on schemas for a single-module app. His per-module `HasDefaultSchema` advice is modular-monolith only, and I saw it only through a mirror, so it is unverified.

**Conflicts and evolution**
- MediatR (https://milanjovanovic.tech/blog/cqrs-pattern-with-mediatr, Oct 2023) was replaced by hand-rolled handlers + Scrutor decorators (May 2025 onward).
- NetArchTest (2023–May 2024) was replaced by ArchUnitNET (Mar 2026).
- On logging, there are three positions:
  - Serilog (Dec 2023)
  - the CQRS post's decorators still use Serilog `LogContext` (May 2025)
  - plain OTel logging provider (Aug 2025)
- For the observability backend, the template uses Grafana/Loki/Tempo while the blog uses the Aspire dashboard. Both use OTLP, so only the sink differs.

**Recommended for SlotDrop**
- Use 5 src projects (Domain, Application, Infrastructure, Api, SharedKernel) and 3 test projects (ArchitectureTests, Application.UnitTests, IntegrationTests with Testcontainers Postgres/Redis).
- Use feature folders inside Application, with the Api endpoint folders mirroring them.
- Use no MediatR. Hand-roll `ICommandHandler`/`IQueryHandler` that return `Result<T>`, with Scrutor logging and validation decorators and FluentValidation.
- Use Minimal APIs with one `IEndpoint` per class and auto-registration. Map failed Results to ProblemDetails, and add `traceId` via `CustomizeProblemDetails`.
- Use ArchUnitNET with his 5 rules, plus the "entities have no public constructors" rule.
- Add Directory.Build.props (net10.0, warnings as errors, AnalysisMode=All, EnforceCodeStyleInBuild, Sonar), Directory.Packages.props (CPM) and .editorconfig.
- Use OTel for logs, traces and metrics with `UseOtlpExporter()` plus Npgsql instrumentation (and Redis instrumentation, which is my addition and not from his sources). Use the Aspire dashboard locally and skip Serilog.
- Use EF Core with Fluent `IEntityTypeConfiguration` and snake_case naming for writes, and Dapper/raw SQL for reads. Apply migrations with reviewed SQL scripts, not startup `Migrate()`.
