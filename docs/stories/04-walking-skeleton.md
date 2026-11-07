# Foundations: walking skeleton reads a tenant through every layer

## Business Value
As the developer, I want one real request to travel from endpoint to Postgres and back under integration tests, so that every later story starts from a proven path.

## Description
**Context:**
TBD

**Scope:**
- Tenant (id, slug, name), the only domain concept in Sprint 1
- EF Core DbContext and the first migration as a reviewed SQL script; one seeded tenant
- `GET /tenants/{slug}` through a handler returning Result; unknown slug → ProblemDetails 404
- First Result/Error type, handler shape and endpoint registration (the skeleton ships none, see [Skeleton: what the solution contains on day one](../tickets/08-skeleton-day-one-contents.md))
- IntegrationTests project; choose the assertion library (xUnit `Assert` · Shouldly · AwesomeAssertions; not FluentAssertions 8, see [Skeleton: names, solution file, build strictness and test conventions](../tickets/11-skeleton-conventions.md))
- Decide data access for the first query: DbContext in handlers · Application-owned interface · repositories with EF Core only in Infrastructure; add the matching architecture rule and Milan's handler rules (naming, `internal`). See [Skeleton: architecture-test library and first rules](../tickets/10-skeleton-architecture-tests.md)
- Testcontainers Postgres fixture; the integration test runs in CI
- The request's trace is visible in the Aspire dashboard

**Out of Scope, Known Limitations:**
- Booking concepts: E1
- Redis in tests: until an episode needs it

**Possible resolution paths to investigate:**
TBD

**Test Scenarios:**
- TBD

## Functional Acceptance Criteria
**Nominal Use Cases:**
TBD

**Corner Cases:**
- TBD

## Technical Acceptance Criteria
- Code Review, including the Secure Code Review Checklist
- External Contract Change (API, exports): TBD
- Proposed solution:
  - Request: TBD
  - Response: TBD

## Testing
- Unit Tests: always
- Component Tests: TBD
- Integration Tests: TBD
- System Tests: TBD
- UI Tests (Cypress, ID / Selectors): TBD

## Deployment
- Backward Compatibility Requirement: TBD
- Monitoring, Alerting Metrics: TBD
  - Logs: TBD
  - OpenSearch field mapping: TBD
  - Alerting: TBD
  - OpenSearch dashboard / visualization: TBD
  - Healthcheck: TBD

## Required Documentation
TBD

## Dependencies
- Blocked by: Foundations: CI on every pull request; Foundations: local environment with traces in the Aspire dashboard
- Blocks: Foundations: deploy by hand with az, and list what hurts
