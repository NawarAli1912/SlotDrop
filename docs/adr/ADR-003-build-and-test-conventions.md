# ADR-003: Build and test conventions

Status: accepted, 2026-11-05

## Context
Every later PR inherits these settings, and CI (next story) will enforce them. Facts that constrain the choice, measured on SDK 10:

- xunit.v3 4.x runs only on Microsoft Testing Platform; without the runner setting in `global.json`, `dotnet test` stops with "Testing with VSTest target is no longer supported".
- On the empty templates, `AnalysisMode=All` with warnings as errors gives 0 errors; adding SonarAnalyzer gives 2, and Sonar is source-available, not open source.
- `actions/setup-dotnet` caching needs `packages.lock.json`.

## Options
Strictness: Milan's full set with Sonar · all built-in analyzers as errors, no Sonar · default analyzers.
Solution file: `.slnx` (SDK 10 default) · `.sln`.
Assertion library: xUnit `Assert` · Shouldly · AwesomeAssertions · FluentAssertions 8 (paid for commercial use).

## Decision
- `global.json`: SDK `10.0.403`, `rollForward: latestFeature`, test runner `Microsoft.Testing.Platform`.
- `Directory.Build.props`: `net10.0`, `AnalysisLevel=latest`, `AnalysisMode=All`, `TreatWarningsAsErrors`, `EnforceCodeStyleInBuild`, `RestorePackagesWithLockFile`. No Sonar.
- Central Package Management in `Directory.Packages.props`; lock files committed.
- `.editorconfig` from `dotnet new editorconfig`, with only file-scoped namespaces (IDE0161) and `var` when the type is apparent (IDE0007) raised to warning, and `insert_final_newline = true` (the template's `false` makes `dotnet format` strip the newline editors add).
- `SlotDrop.slnx`, projects under `src/` and `tests/`.
- Test names are sentences with underscores, one class per layer: `DomainTests.Domain_should_not_depend_on_other_libraries`. CA1707 is off for `tests/**` only, since it rejects underscores.
- Assertion library: chosen in the walking skeleton with the first test that asserts something; this PR's tests assert through ArchUnitNET's `Check`. FluentAssertions 8 is ruled out.

## Consequences
- Run tests with `dotnet test --solution SlotDrop.slnx`. CI reports need `--report-xunit-trx` or `--report-trx`; `--logger trx` no longer works.
- The analyzers already shaped the code: CA1515 rejected a public marker type in the Api (an executable), so only library projects have one.
- More style rules are raised when review shows inconsistency, not up front. Sonar is added only if a real bug slips past the built-in analyzers.
