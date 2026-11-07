# E2 Prove: a race test shows double booking

## Business Value
As the developer, I want a test that reproduces double booking under concurrency, so that the problem is shown with evidence, not assumed.

## Description
**Context:**
TBD

**Scope:**
- Integration test (Testcontainers) that fires parallel bookings at one slot
- It asserts today's behaviour (more than one booking) and records the count, so it merges green
- Findings go into the E2 write-up draft

**Out of Scope, Known Limitations:**
- Fixing it: E2 Fix
- Load on Azure: the k6 story

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
- Blocked by: E1 Build: list free slots and book one (naive)
- Blocks: E2 Fix: one slot, one booking
