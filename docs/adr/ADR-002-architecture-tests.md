# ADR-002: Architecture tests

Status: accepted, 2026-11-05

## Context
The layers in ADR-001 should stay clean as features are added. Part of that is already enforced: every wrong reference between the four projects closes a cycle, and the build rejects it (`error MSB4006: There is a circular dependency in the target dependency graph`). What the build does not stop is code in Domain or Application using types from a framework or package it should not know about.

## Options
Library:
1. **ArchUnitNET** (`TngTech.ArchUnitNET.xUnitV3`): maintained, has an xUnit v3 package, failure messages name the offending type, and handles type-level rules (naming, visibility) later.
2. **NetArchTest.Rules**: last release 2021, unmaintained.
3. **Hand-rolled reflection over referenced assemblies**: no dependency, but assembly-level only.

Rules:
1. **Layer direction**: cannot fail while the build rejects cycles.
2. **Forbidden dependencies**: guards what the build does not.
3. **Milan's five rules**: need handlers, which do not exist yet.

## Decision
ArchUnitNET 0.13.5 with two rules:

- `Domain_should_not_depend_on_other_libraries`: Domain types depend only on `SlotDrop.Domain` and `System.*`.
- `Application_should_not_depend_on_aspnetcore`: HTTP stays in the Api.

Application is deliberately not barred from EF Core, Npgsql or Dapper yet; how handlers reach data is decided with the first query in the walking skeleton.

The PR proves the rules can fail with a red commit (ASP.NET Core in Application, logging in Domain) followed by a revert.

## Consequences
- The red commit found a rule that could not fail: `OnlyDependOnTypesThat()` only looks at types inside the loaded assemblies, so Domain's use of `ILogger` passed. The Domain rule now uses `NotDependOnAnyTypesThat().DoNotResideInNamespaceMatching(...)`, which sees every dependency. Any future "only depend on" rule needs the same check.
- ArchUnitNET checks types the code uses, not package references: an unused forbidden package passes.
- Each library project has a `public sealed class AssemblyMarker;` so the tests can load it.
- Data-access and handler rules are added by the story that adds the first query and handler.
