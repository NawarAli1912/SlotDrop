# Foundations: solution skeleton with architecture tests

## Business Value
As the developer, I want a solution whose layer boundaries are checked by tests, so that the structure stays clean as features are added.

## Description
**Context:**
First PR of SlotDrop. Sprint 1's learning target is to explain Clean Architecture's dependency rule and prove it with architecture tests ([LEARNING-PATH.md](../LEARNING-PATH.md)). Following the naive-first rule, the skeleton holds only the layers and the tests that guard them; abstractions arrive with their first caller ([Skeleton: what the solution contains on day one](../tickets/08-skeleton-day-one-contents.md)).

**Scope:**
- 4 src projects (Domain, Application, Infrastructure, Api) with references Api → Application + Infrastructure, Infrastructure → Application, Application → Domain
- 1 test project: ArchitectureTests
- Api serves only `GET /health` (`MapHealthChecks`, liveness only)
- `SlotDrop.slnx`; projects `src/SlotDrop.{Domain,Application,Infrastructure,Api}`, `tests/SlotDrop.ArchitectureTests`; namespaces match project names; one `public sealed class AssemblyMarker;` per src project
- global.json: SDK `10.0.403`, `rollForward: latestFeature`, `"test": {"runner": "Microsoft.Testing.Platform"}`
- Directory.Build.props: net10.0, `AnalysisMode=All`, `AnalysisLevel=latest`, `TreatWarningsAsErrors`, `EnforceCodeStyleInBuild`, `RestorePackagesWithLockFile`; no Sonar
- Directory.Packages.props (CPM); NuGet lock files committed
- .editorconfig from `dotnet new editorconfig`, with IDE0161 (file-scoped namespaces) and `var` when the type is apparent raised to warning
- Test naming: sentence-style with underscores, one class per layer (`DomainTests.Domain_should_not_depend_on_other_libraries`)
- ArchUnitNET 0.13.5 (`TngTech.ArchUnitNET.xUnitV3`) rules: Domain depends on nothing outside Domain and `System.*`; Application uses no ASP.NET Core types
- A red-then-revert commit pair proves a rule fails; the failing `dotnet test` output goes in the PR description
- ADR-001 solution structure, ADR-002 architecture tests and rules, ADR-003 build and test conventions
- README stub: what SlotDrop is, prerequisite SDK 10.0.403, build/test/run commands

**Out of Scope, Known Limitations:**
- Endpoints beyond `/health`: walking-skeleton story
- SharedKernel, Result/Error, handler interfaces, `IEndpoint`, decorators, FluentValidation: added when a caller needs them (walking skeleton or later), per the naive-first rule
- IntegrationTests project: walking-skeleton story; UnitTests project: with the first logic worth testing
- Readiness checks and probe split: Sprint 2
- Layer-direction rule: the build already rejects every wrong reference between the projects as a cycle (MSB4006)
- Rules on EF Core / Npgsql / Dapper in Application, and Milan's handler rules: walking-skeleton story, with the first query and handler
- Docker and Compose: local-env story
- CI: next story
- Assertion library: walking-skeleton story, with the first unit or integration test; FluentAssertions 8 ruled out (paid licence)
- SonarAnalyzer: only if a real bug slips past the built-in analyzers

**Test Scenarios:**
- `GET /health` on a running Api: 200 Healthy
- An ASP.NET Core type used in Application (red commit): the Application rule fails and names the type
- A non-`System` dependency used in Domain: the Domain rule fails
- After the revert commit: all architecture tests pass
- `dotnet build SlotDrop.slnx` on a clean clone: 0 warnings, 0 errors
- `dotnet format SlotDrop.slnx --verify-no-changes`: exit 0
- `dotnet test --solution SlotDrop.slnx`: all architecture tests pass
- Build with SDK 10.0.202 only: fails fast with the global.json "SDK not found" error

## Functional Acceptance Criteria
**Nominal Use Cases:**
On a clean clone with SDK 10.0.403, the developer must be able to build (`dotnet build SlotDrop.slnx`: 0 warnings, 0 errors), check formatting (`dotnet format SlotDrop.slnx --verify-no-changes`: exit 0) and run the architecture tests (`dotnet test --solution SlotDrop.slnx`: all pass). When the Api runs (`dotnet run --project src/SlotDrop.Api`), `GET /health` must return 200 with `text/plain` body `Healthy`.

**Corner Cases:**
- An ASP.NET Core type used in Application (red commit): the Application rule fails and the message names the type
- A non-`System` dependency used in Domain: the Domain rule fails
- Older SDK only (e.g. 10.0.202): the build fails fast with the global.json "SDK not found" error

## Technical Acceptance Criteria
- Code Review, including the Secure Code Review Checklist
- External Contract Change (API, exports): No (`/health` is new but has no consumers)
- Proposed solution:
  - Request: `GET /health`, no parameters
  - Response: 200 `text/plain` `Healthy` from `MapHealthChecks` with no checks registered (liveness only)

## Testing
- Unit Tests: always
- Component Tests: No
- Integration Tests: No (the walking-skeleton story adds the first; `/health` is checked by hand here and by the local-env story's Compose healthcheck)
- System Tests: No
- UI Tests (Cypress, ID / Selectors): N/A
- Architecture Tests: Yes (ArchUnitNET)

## Deployment
- Backward Compatibility Requirement: No (first PR)
- Monitoring, Alerting Metrics: No
  - Logs: default ASP.NET Core console logs only; OpenTelemetry in the local-env story
  - OpenSearch field mapping: No
  - Alerting: No
  - OpenSearch dashboard / visualization: No
  - Healthcheck: Yes, liveness at `/health`

## Required Documentation
- ADR-001 solution structure, ADR-002 architecture tests and rules, ADR-003 build and test conventions (Context → Options → Decision → Consequences), kept in `docs/adr/` and mirrored to the ClickUp Doc "SlotDrop ADRs" on merge, not in the repo
- README stub: what SlotDrop is, prerequisite SDK 10.0.403, build/test/run commands
- No journey write-up: tooling, not an episode


## Dependencies
- Blocked by: none
- Blocks: Foundations: CI on every pull request
