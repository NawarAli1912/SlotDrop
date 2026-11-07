---
title: "Skeleton: names, solution file, build strictness and test conventions"
type: groom
for: 01-solution-skeleton
status: closed
blocked_by: [09-skeleton-tool-facts]
---

## Question
The conventions every later PR inherits:
- project and namespace names (`SlotDrop.Domain` …; `Api` vs Milan's `Web.Api`) and folder layout (`src/`, `tests/`)
- solution file: `.slnx` or `.sln`
- build strictness: how many analyzer warnings become errors on day one
- test conventions: test framework is xUnit v3 ([Local env and CI ticket](02-local-env-and-ci.md) section 2); which assertion library; test naming

Hard requirements already settled elsewhere:
- `global.json` pins a fully qualified SDK version ([Local env and CI ticket](02-local-env-and-ci.md) section 3)
- `RestorePackagesWithLockFile=true`, because `setup-dotnet` `cache: true` errors without `packages.lock.json` (same section)
- Central Package Management via Directory.Packages.props ([Milan structure ticket](01-milan-structure.md) section 5)

## Options
Facts from [research ticket](09-skeleton-tool-facts.md):
- xunit.v3 4.x runs only on Microsoft Testing Platform on .NET 10; `global.json` must contain `"test": {"runner": "Microsoft.Testing.Platform"}` or `dotnet test` errors (§3). This is now a hard requirement.
- SDK: latest is 10.0.403; local is 10.0.202 (§7). Pinning 10.0.403 means upgrading the Mac SDK first.

**Build strictness** (measured on `dotnet new web` + `classlib`, §5)
- **a. Milan's full set** (`AnalysisMode=All` + TWAE + `EnforceCodeStyleInBuild` + SonarAnalyzer 10.35): 2 errors on day one (S2094 empty class, S6966 `app.Run()`); Sonar is source-available, not open source
- **b. `AnalysisMode=All` + TWAE + `EnforceCodeStyleInBuild`, no Sonar**: 0 errors on the templates; an empty marker interface trips CA1040, a sealed marker class doesn't
- **c. Default analysis level + TWAE**: 0 errors; least checking
- `EnforceCodeStyleInBuild` does nothing until an .editorconfig raises IDE rule severities.

**Assertion library** (§6)
- xUnit `Assert`: built in; plainer failure messages
- Shouldly 4.3.0 (BSD-3): active; 5.0 in preview
- AwesomeAssertions 9.6.0 (Apache-2.0): FluentAssertions-style API, active
- FluentAssertions 8.x: paid for commercial use; ruled out for a public portfolio repo with unclear licence status

**Solution file** (§4)
- **`.slnx`**: SDK 10 default; build, test, format all measured working; Rider supports it (2026.2 docs). Unverified: a reported Rider NuGet-update bug on .slnx
- **`.sln`**: works everywhere; noisier merges

**SDK pin**: 10.0.403 (latest, upgrade Mac) · 10.0.202 (what's installed). `rollForward: latestFeature` either way.

## Resolution
Groomed with Nawar, 2026-11-05.
- SDK: `global.json` pins `10.0.403`, `rollForward: latestFeature`. Nawar upgrades the Mac SDK first (local is 10.0.202; [research ticket](09-skeleton-tool-facts.md) §7).
- Test runner: `global.json` has `"test": {"runner": "Microsoft.Testing.Platform"}`, required by xunit.v3 4.x on .NET 10 (§3).
- Build strictness (option b): `AnalysisMode=All`, `AnalysisLevel=latest`, `TreatWarningsAsErrors`, `EnforceCodeStyleInBuild`; no SonarAnalyzer (0 errors on the templates vs 2 with Sonar, §5). Add Sonar only if a real bug slips through.
- .editorconfig: from `dotnet new editorconfig`; only IDE0161 (file-scoped namespaces) and `var` when the type is apparent raised to warning. Raise more when review shows inconsistency.
- CPM (Directory.Packages.props) and `RestorePackagesWithLockFile=true`: hard requirements, unchanged.
- Names: `src/SlotDrop.Domain`, `src/SlotDrop.Application`, `src/SlotDrop.Infrastructure`, `src/SlotDrop.Api`, `tests/SlotDrop.ArchitectureTests`; namespaces match. `Api`, not `Web.Api`: no second host yet.
- Solution file: `SlotDrop.slnx` (SDK 10 default; build/test/format measured working, §4). Fallback to `.sln` if the unverified Rider NuGet-update bug bites.
- Assertion library: deferred to the walking skeleton (this PR's only assertions are ArchUnitNET's `Check`). FluentAssertions 8 ruled out (paid for commercial use, §6).
- Test naming: sentence-style with underscores, one class per layer, e.g. `DomainTests.Domain_should_not_depend_on_other_libraries`.
- Marker type: `public sealed class AssemblyMarker;` per project; clean under these settings (§5). Closes the assumption in [Skeleton: architecture-test library and first rules](10-skeleton-architecture-tests.md).
