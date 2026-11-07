# E9 Fix: fair access when demand exceeds capacity

## Business Value
As a customer, I want a fair chance at a slot during a drop, so that refreshing fastest isn't what wins.

## Description
**Context:**
TBD

**Scope:**
- Trade-off brief; the mechanism must work across several API replicas
- A Vue screen for the waiting experience
- Re-measure; E9 write-up and ADR
- Deploy lens: the state must be shared across replicas
- Monitoring lens: queue length and admission rate; an alert on a stuck queue

**Out of Scope, Known Limitations:**
- Tenant isolation: v2

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
- Blocked by: E9 Prove: under drop load, access is unfair
- Blocks: Drill: restore Postgres from a backup; Docs: v1 before/after report in the README
