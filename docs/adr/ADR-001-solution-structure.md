# ADR-001: Solution structure

Status: accepted, 2026-11-05

## Context
SlotDrop starts empty. Sprint 1's learning target is to explain Clean Architecture's dependency rule and prove it with tests. The project is planned naive-first: each piece of design arrives when a real problem or caller needs it, not up front.

Milan Jovanović's current .NET 10 template ships five projects (Domain, Application, Infrastructure, Web.Api, SharedKernel) with `Result`/`Error`, hand-rolled CQRS handlers, Scrutor decorators, FluentValidation and `IEndpoint` registration already in place.

## Options
1. **Bare layers.** Four projects and the tests that guard them; abstractions arrive with their first caller.
2. **Milan's template up front.** Everything above on day one. The walking skeleton becomes a thin feature, but decorators and validation have no caller for a single GET, and their reasons are told rather than earned.
3. **Bare layers plus a SharedKernel with `Result`/`Error`.** Decides "errors as values vs exceptions" before any error exists.

## Decision
Option 1.

- `src/SlotDrop.Domain`, `src/SlotDrop.Application`, `src/SlotDrop.Infrastructure`, `src/SlotDrop.Api`
- References follow the dependency rule: Api → Application + Infrastructure, Infrastructure → Application, Application → Domain
- The Api serves only `GET /health` (liveness, no checks registered), so the local environment has a request to trace before any feature exists
- One test project: `tests/SlotDrop.ArchitectureTests`
- `Api`, not `Web.Api`: there is no second host yet

## Consequences
- The walking-skeleton story adds the first `Result`/`Error`, handler shape and endpoint registration, and decides where they live.
- Integration and unit test projects arrive with the first test that needs them.
- Liveness vs readiness is decided in Sprint 2 with the Azure health probes.
