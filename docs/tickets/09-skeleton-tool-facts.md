---
title: "Skeleton: current versions and tool limits for the build and architecture tests"
type: research
for: 01-solution-skeleton
status: closed
blocked_by: []
---

## Question
Facts the build and architecture-test choices wait on, as of the research date:
- ArchUnitNET: current version; is there an xUnit v3 package (`TngTech.ArchUnitNET.xUnitV3` or similar) or only xUnit v2; does it load assemblies that hold a single marker type
- NetArchTest.Rules: last release date and whether it is maintained
- `xunit.v3` current version and runner packages for `dotnet test` on .NET 10 (Microsoft Testing Platform vs VSTest)
- `.slnx`: is it the .NET 10 `dotnet new sln` default; do Rider (current), `dotnet format` and `actions/setup-dotnet@v6` handle it
- `AnalysisMode=All` + `TreatWarningsAsErrors` on an empty .NET 10 web project: which rules fire (count), and SonarAnalyzer.CSharp current version
- Assertion libraries for xUnit v3: FluentAssertions v8 licence (commercial?), AwesomeAssertions, Shouldly; current versions and maintenance
- Local fact already known: `dotnet --list-sdks` on Nawar's Mac returned `10.0.202` (2026-11-05); is a newer 10.0 feature band out

## Resolution
_Researched 2026-11-05._ Scratch measurements live in `/private/tmp/claude-501/-Users-nawarali-Desktop/b99a884e-846b-45bb-9fb3-9852ef0bd965/scratchpad/research-09` (throwaway).

### 1. ArchUnitNET
- `TngTech.ArchUnitNET` current version is 0.13.5, published 2026-10-26 (Apache-2.0). Sources: https://www.nuget.org/packages/TngTech.ArchUnitNET (NuGet registration API, 2026-11-05) and https://github.com/TNG/ArchUnitNET/releases (tag 0.13.5, 2026-10-26).
- 0.13.5 now detects dependencies inside `async` methods (Release builds) and inside async lambdas. Rules that passed only because these were missed may start failing. Source: https://github.com/TNG/ArchUnitNET/releases (2026-11-05).
- An xUnit v3 integration package exists: `TngTech.ArchUnitNET.xUnitV3` 0.13.5 (2026-10-26). It has shipped alongside the core package since 0.11.3 (2025-02-28) and depends on `xunit.v3.assert >= 1.1.0`. Source: https://www.nuget.org/packages/TngTech.ArchUnitNET.xUnitV3 (NuGet registration API, 2026-11-05).
- `TngTech.ArchUnitNET.xUnit` (the xUnit v2 package) is also still published at 0.13.5. No `TngTech.ArchUnitNET.xUnit.v3` ID exists (the registration API returned not found on 2026-11-05).
- The `xUnitV3` package's `rule.Check(architecture)` works with `xunit.v3` 4.0.2 under MTP. A violating rule throws `ArchUnitNET.xUnitV3.FailedArchRuleException` with the rule text and the offending type ("Lib.AssemblyMarker is sealed"). Command: `dotnet test --solution Tiny.slnx` in the scratchpad returned 4 passed and 1 failed (the deliberate failure), as expected.
- The integration package is optional. With only `TngTech.ArchUnitNET` 0.13.5, `Assert.True(rule.HasNoViolations(architecture))` and `rule.Evaluate(architecture).Where(r => !r.Passed)` work with plain xunit.v3 asserts. Measured with `dotnet test --solution Tiny.slnx`: 3/3 passed, including a deliberately violating rule asserted `False`. Plain `Assert.True` gives no violation details in the failure message. `Check` from the integration package does.
- An assembly that holds only one type (`public sealed class AssemblyMarker;`) loads fine. `new ArchLoader().LoadAssemblies(typeof(AssemblyMarker).Assembly).Build()` returned an architecture containing `AssemblyMarker`, and rules over it evaluated correctly (same scratchpad run, 2026-11-05).

### 2. NetArchTest.Rules
- The latest version is 1.3.2, published 2021-05-23. Source: https://www.nuget.org/packages/NetArchTest.Rules (NuGet registration API, 2026-11-05) and https://github.com/BenMorris/NetArchTest/releases (v1.3.2, 2021-05-23).
- It looks effectively unmaintained. The last commit on the default branch is 2023-07-03 and the last push to any branch is 2024-07-29. The repo is not archived and has 25 open issues. Source: GitHub API for https://github.com/BenMorris/NetArchTest (queried 2026-11-05).

### 3. xunit.v3 and `dotnet test` on .NET 10
- `xunit.v3` current version is 4.0.2 (2026-10-09). 4.0.0 shipped on 2026-08-15. `xunit.runner.visualstudio` is at 4.0.1 (2026-10-09), and `Microsoft.NET.Test.Sdk` at 18.11.0 (2026-10-13). Source: NuGet registration API for https://www.nuget.org/packages/xunit.v3, https://www.nuget.org/packages/xunit.runner.visualstudio and https://www.nuget.org/packages/Microsoft.NET.Test.Sdk (2026-11-05).
- xunit.v3 4.x defaults to MTP v2 and dropped MTP v1. MTP support is built into xunit.v3. VSTest support comes only from `xunit.runner.visualstudio` (plus `Microsoft.NET.Test.Sdk`), and both packages can be removed for MTP-only use. Package variants are `xunit.v3`, `xunit.v3.mtp-v2` and `xunit.v3.mtp-off`. Source: https://xunit.net/docs/getting-started/v3/microsoft-testing-platform (page dated 2026-05-27, read 2026-11-05).
- On the .NET 10 SDK, `dotnet test` defaults to VSTest mode. MTP mode needs `global.json` containing `{ "test": { "runner": "Microsoft.Testing.Platform" } }`. Source: https://learn.microsoft.com/dotnet/core/testing/unit-testing-with-dotnet-test (ms.date 2026-09-12). `dotnet new globaljson --test-runner Microsoft.Testing.Platform` generates the file (checked locally with `dotnet new globaljson --help`, SDK 10.0.202).
- **On .NET 10, the global.json setting is required with xunit.v3 4.x, measured.** Without it, `dotnet test Tiny.slnx` fails with `error : Testing with VSTest target is no longer supported by Microsoft.Testing.Platform on .NET 10 SDK and later...`. The error is emitted by `Microsoft.Testing.Platform.MSBuild` 2.5.1 (`_MTPBeforeVSTest` target). Referencing `xunit.runner.visualstudio` 4.0.1 + `Microsoft.NET.Test.Sdk` 18.11.0 and passing `-p:TestingPlatformDotnetTestSupport=false` did not avoid it. The error condition only checks `IsTestingPlatformApplication` and SDK major >= 10. Untested way around it: the `xunit.v3.mtp-off` package.
- In MTP mode, `dotnet test <file>` positional syntax is rejected with "Specifying a solution for 'dotnet test' should be via '--solution'.". Use `dotnet test --solution X.slnx` or `--project`. Measured locally and documented in the learn.microsoft.com page above.
- **`--logger trx` does not work under MTP.** Measured: exit code 5, with "Unknown option '--logger' ... uses VSTest syntax, which is not supported by Microsoft.Testing.Platform. Use '--report-trx' instead.".
- `--report-trx` needs the `Microsoft.Testing.Extensions.TrxReport` package (current 2.5.2, 2026-10-27). Without it: exit code 5. With it, `dotnet test --solution Tiny.slnx --report-trx` wrote `TestResults/Lib.Tests_net10.0_arm64.trx` (measured). Docs: https://learn.microsoft.com/dotnet/core/testing/unit-testing-with-dotnet-test.
- xunit.v3's own `--report-xunit-trx` works with no extra package. Measured: it wrote `TestResults/<user>_<machine>_<timestamp>.trx`, and the filename can be set with `--report-xunit-trx-filename` (filename only, no path). Source: https://xunit.net/docs/getting-started/v3/microsoft-testing-platform.
- Both TRX files use the standard `http://microsoft.com/schemas/VisualStudio/TeamTest/2010` schema with a `<Counters>` element (inspected locally). Unverified: that dorny/test-reporter `dotnet-trx` parses these MTP/xunit TRX files. It was not run here.

### 4. `.slnx`
- `dotnet new sln` on SDK 10.0.202 creates `Tiny.slnx` containing `<Solution>\n</Solution>` (command run locally, 2026-11-05). This is documented as a .NET 10 breaking change, and `--format sln` restores the old format. Source: https://learn.microsoft.com/dotnet/core/compatibility/sdk/10.0/dotnet-new-sln-slnx-default (read 2026-11-05).
- The CLI accepts .slnx. `dotnet sln Tiny.slnx add ...`, `dotnet build Tiny.slnx` (0 warnings, 0 errors), `dotnet format Tiny.slnx --verify-no-changes` (exit 0) and `dotnet test --solution Tiny.slnx` (MTP mode, tests ran) all worked on SDK 10.0.202 (measured 2026-11-05).
- Rider supports .slnx. The current help (2026.2) has a "Use SLNX format" section covering Save as XML Solution (.slnx) and conversion back to .sln. Source: https://www.jetbrains.com/help/rider/2026.2/Extending_Your_Solution.html (last modified 2026-09-30). Support first shipped in Rider 2024.3 (https://blog.jetbrains.com/dotnet/2024/10/04/support-for-slnx-solution-files/; page returned 403 to fetch, so this comes from a search snippet).
- Unverified: whether the Rider 2026.2.2 release notes still list a known issue "Updating NuGet packages fails: NullReferenceException in .slnx solutions" (https://youtrack.jetbrains.com/articles/DOTNET-A-397). This came from a search snippet only, and the page did not render for fetching.
- `actions/setup-dotnet` latest major is v6. Its README does not mention .slnx or solution formats. It installs SDKs (optionally via `global-json-file`) and has opt-in NuGet caching. Source: https://github.com/actions/setup-dotnet (read 2026-11-05). .slnx handling is entirely in the installed SDK (>= 9.0.200), so setup-dotnet needs nothing beyond installing a 10.0 SDK. (Inference from the above. Not run in Actions.)

### 5. Build strictness (`dotnet new web` + `dotnet new classlib`, net10.0, SDK 10.0.202, clean `--no-incremental` builds, no .editorconfig)
- Baseline, with no Directory.Build.props: 0 warnings, 0 errors.
- a) `TreatWarningsAsErrors=true` + `EnforceCodeStyleInBuild=true` + `AnalysisLevel=latest`: 0 warnings, 0 errors.
- b) a) + `AnalysisMode=All`: 0 warnings, 0 errors. The csc command line confirmed `/warnaserror+` and `analysislevel_10_all.globalconfig` were active, so the template code is simply clean.
- c) b) + `SonarAnalyzer.CSharp` 10.35.0.4138: **2 errors**. `S2094` fired in Core/Class1.cs ("Remove this empty class, write its code or make it an 'interface'"), and `S6966` fired in Web/Program.cs(6,1) ("Await RunAsync instead" for `app.Run()`).
- These results matter for an assembly marker type (measured):
  - `public sealed class AssemblyMarker;` and `public static class AssemblyMarker;` each trigger S2094 under c).
  - `public interface IAssemblyMarker;` triggers `CA1040` (avoid empty interfaces) under b) and c).
  - `public sealed class AssemblyMarker;` builds clean under b).
- Without an .editorconfig, `EnforceCodeStyleInBuild` adds no build diagnostics on the templates (observed: 0 IDE* diagnostics). Unverified: which IDE rules would fire once an .editorconfig raises their severity.
- SonarAnalyzer.CSharp's current version is 10.35.0.4138, published 2026-09-28. Its licence is the **SONAR Source-Available License v1.0**, which is not an OSI open-source licence. Source: https://www.nuget.org/packages/SonarAnalyzer.CSharp/10.35.0.4138/License (read 2026-11-05).

### 6. Assertion libraries for xUnit v3
- FluentAssertions is at 8.11.0 (2026-09-14). v8+ is commercial: per Xceed, commercial use requires a paid per-seat licence granted for 12 months. It stays free under the "Xceed Community License for Fluent Assertions" only "so long as the intended use is not for commercial purpose". v7 stays Apache-2.0 and receives bugfixes. Sources: https://xceed.com/fluent-assertions-faq/ and https://www.nuget.org/packages/FluentAssertions (read 2026-11-05).
- The FAQ does not define commercial use or name open-source projects as a category. Unverified: whether a public portfolio repo counts as non-commercial (see the Xceed licence agreement).
- AwesomeAssertions (the Apache-2.0 fork of FluentAssertions 8) is at 9.6.0 (2026-08-20). The previous release was 9.5.0 (2026-07-19). It is active: last commit 2026-11-03 and 31 open issues. Sources: https://www.nuget.org/packages/AwesomeAssertions and GitHub API for https://github.com/AwesomeAssertions/AwesomeAssertions (2026-11-05).
- Shouldly's latest stable is 4.3.0 (2025-01-23, BSD-3-Clause). 5.0.0-preview.2 is on NuGet (published 2026-07-18 per NuGet, GitHub release dated 2026-10-01). It is active: last commit 2026-11-03 and 86 open issues. Sources: https://www.nuget.org/packages/Shouldly and https://github.com/shouldly/shouldly/releases (2026-11-05).

### 7. .NET 10 SDK
- The latest .NET 10 release is runtime 10.0.14 (2026-10-25), shipping SDKs **10.0.403** (latest feature band 4xx) and 10.0.114. Source: https://builds.dotnet.microsoft.com/dotnet/release-metadata/10.0/releases.json (`latest-sdk: 10.0.403`, fetched 2026-11-05).
- The local `dotnet --list-sdks` returned only `10.0.202 [/usr/local/share/dotnet/sdk]`. That is two feature bands behind (2xx vs 4xx) and missing later patches. The 3xx band shipped 10.0.303 with runtime 10.0.11 on 2026-08-11 (same releases.json).
