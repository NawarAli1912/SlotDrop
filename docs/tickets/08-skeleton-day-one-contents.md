---
title: "Skeleton: what the solution contains on day one"
type: groom
for: 01-solution-skeleton
status: closed
blocked_by: []
---

## Question
What does the skeleton PR contain before any feature exists? Which projects, which shared abstractions (Result, handlers, endpoint registration), which test projects, and does the Api serve anything?

## Options

**a. Bare layers: projects, build files and architecture tests only**
- Contents: Domain, Application, Infrastructure, Api; build files; one ArchitectureTests project. No Result, no handler interfaces, no IEndpoint, no SharedKernel yet. The walking-skeleton story adds each abstraction when `GET /tenants/{slug}` first needs it.
- Benefits:
  - every abstraction arrives with a caller and a reason, matching the naive-first rule in [LEARNING-PATH.md](../LEARNING-PATH.md) ("each piece of the model is earned")
  - smallest PR; Sprint 1's learning target is the dependency rule and architecture tests, which need only the layers ([LEARNING-PATH.md](../LEARNING-PATH.md), Sprint 1 row)
- Costs:
  - the walking-skeleton story grows by the Result type and one handler shape
  - architecture tests start with layer direction only; naming and handler rules wait for handlers to exist
- Limitation: architecture tests over near-empty projects pass trivially, so the story must prove they can fail (a deliberate forbidden reference turns them red).

**b. Milan's template up front**
- Contents: five src projects incl. SharedKernel with `Result`/`Error`; `ICommand`/`IQuery` + handlers; Scrutor logging and validation decorators; FluentValidation; `IEndpoint` auto-registration; failed Result → ProblemDetails ([Milan structure ticket](01-milan-structure.md), sections 1–3).
- Benefits:
  - walking skeleton becomes a thin feature on a ready frame
  - all five of Milan's architecture rules can be written on day one ([Milan structure ticket](01-milan-structure.md), section 4)
- Costs:
  - code with no caller: decorators and validation do nothing for a single GET
  - the reasons behind each piece are told, not earned, which the naive-first rule rejects
  - biggest PR in Sprint 1, at about 14 points total ([Split ticket](05-split-v1.md), line 40)

**c. Bare layers plus SharedKernel with Result/Error**
- Contents: option a + a SharedKernel project holding only `Result`, `Result<T>`, `Error`.
- Benefits: the one abstraction every later handler returns exists; the walking skeleton stays small.
- Costs: Result has no caller until the walking skeleton; deciding "errors as values vs exceptions" before any error happens.

## Resolution
Groomed with Nawar, 2026-11-05.
- Contents: option a. Four src projects (Domain, Application, Infrastructure, Api), build files, architecture tests. No SharedKernel, Result, handler interfaces or `IEndpoint`; the walking-skeleton story adds what `GET /tenants/{slug}` needs.
- Project references follow the dependency rule already set in [MAP.md](../MAP.md) Notes ("Code structure"): Api → Application + Infrastructure, Infrastructure → Application, Application → Domain.
- Api serves only `GET /health` via `MapHealthChecks`, liveness only. Reason: the local-env story runs before the walking skeleton ([MAP.md](../MAP.md) Sprint 1 order) and needs a request to trace. Liveness vs readiness is Sprint 2's health-probe work.
- Test projects: ArchitectureTests only. IntegrationTests arrives with the walking skeleton (its scope already holds the Testcontainers fixture); UnitTests with the first logic worth testing.
- Must show the architecture tests can fail; how is decided in "Skeleton: architecture-test library and first rules".
- Assumed: the local-env story uses `/health` as the Compose healthcheck target (confirm when grooming it).
- Moved to fog: where Result/Error live when the walking skeleton adds them.
