---
title: "Skeleton: architecture-test library and first rules"
type: groom
for: 01-solution-skeleton
status: closed
blocked_by: [08-skeleton-day-one-contents, 09-skeleton-tool-facts]
---

## Question
Which library checks the layers, which rules exist in this PR, and how does the PR prove the rules can fail?

Known limit to weigh: project references already stop most wrong dependencies at compile time (Domain has no reference to Application, so it can't use its types). Architecture tests add value where the compiler doesn't look: someone adding a new `ProjectReference` or `PackageReference` (e.g. EF Core in Application) in a PR. The rule set should target those.

## Options
Fact that reshapes the choice: with the reference chain from [Skeleton: what the solution contains on day one](08-skeleton-day-one-contents.md), every wrong reference between the four projects is a cycle, and the build already rejects it. Measured: `dotnet add Domain reference Application` (Application already references Domain), then `dotnet build` → `error MSB4006: There is a circular dependency in the target dependency graph` (scratchpad `cycle/`, SDK 10.0.202, 2026-11-05). So a "layer direction" rule between projects can never fail while the chain holds. What the build does not stop: a forbidden **package** (EF Core, Npgsql, ASP.NET Core) referenced from Domain or Application, and later type-level rules (naming, visibility, sealed).

**Library**
- **a. ArchUnitNET 0.13.5 + `TngTech.ArchUnitNET.xUnitV3`** ([research ticket](09-skeleton-tool-facts.md) §1)
  - Benefits: xUnit v3 package exists; `rule.Check(arch)` names the offending type on failure; Milan's current pick; covers type-level rules later
  - Costs: a fluent API to learn; 0.13.5 started detecting async dependencies, so rules may tighten on upgrades
- **b. NetArchTest.Rules 1.3.2**
  - Limitation: last release 2021-05-23, last commit 2023-07 ([research ticket](09-skeleton-tool-facts.md) §2). Unmaintained; not recommended.
- **c. Hand-rolled reflection tests** (`Assembly.GetReferencedAssemblies()`)
  - Benefits: no dependency; the test shows exactly what is checked, good for understanding
  - Costs: assembly-level only, so naming/visibility rules later mean switching library or writing much more code

**First rule set**
- **a. Layer direction only**: can't fail while the reference chain holds (see fact above); teaches little
- **b. Forbidden packages**: Domain references no other package; Application doesn't reference EF Core, Npgsql or ASP.NET Core. Guards what the build doesn't
- **c. Milan's five rules** ([Milan structure ticket](01-milan-structure.md) §4): needs handlers, which the skeleton doesn't have

**Proving a rule can fail**
- **a. Red-then-revert commit in the PR**: one commit adds a forbidden package, the PR description pastes the red test output, the next commit reverts. Cheap; proof lives in the PR history, not in the code
- **b. Permanent "rule catches a violation" test**: a fixture assembly with a deliberate violation, and a test asserting the rule reports it. Proof runs on every build; costs an extra project
- **c. Local demo only**: show it on screen; no lasting evidence

## Resolution
Groomed with Nawar, 2026-11-05.
- Library: ArchUnitNET 0.13.5 + `TngTech.ArchUnitNET.xUnitV3`; `rule.Check(arch)` names the offending type ([research ticket](09-skeleton-tool-facts.md) §1).
- Rules in this PR:
  - Domain types depend on nothing outside Domain and `System.*`.
  - Application types don't use ASP.NET Core types (HTTP stays in Api).
- No layer-direction rule: every wrong reference between the four projects is a cycle the build rejects (`MSB4006`, measured in scratchpad `cycle/`, 2026-11-05).
- Application is deliberately **not** barred from EF Core, Npgsql or Dapper. Nawar asked whether handlers may use DbContext; that choice (DbContext in handlers · Application-owned interface · repositories with EF only in Infrastructure) is earned by the first query, so it moves to the walking-skeleton story, which adds any matching rule. Milan's sources disagree here: his architecture-test post bars EF Core from Application ([Milan structure ticket](01-milan-structure.md) §4), while his template puts EF Core behind an Application-owned interface (§7; assumed `IApplicationDbContext` with `DbSet<T>`, template not public).
- Milan's handler rules (naming, `internal`, colocation) are added by the story that adds the first handler.
- Proof the rules can fail: a red-then-revert commit pair in the PR; the red commit uses a forbidden type (e.g. an ASP.NET Core type in Application), and the PR description pastes the failing local `dotnet test` output (CI doesn't exist yet).
- Limit: ArchUnitNET sees types the code uses, not package references; an unused forbidden package passes.
- Assumed: each project gets `public sealed class AssemblyMarker;` to load its assembly. Clean under `AnalysisMode=All`; Sonar flags it (S2094). Settled in "Skeleton: names, solution file, build strictness and test conventions".
