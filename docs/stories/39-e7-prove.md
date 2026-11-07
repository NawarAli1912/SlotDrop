# E7 Prove: a benchmark shows the combinations explode

## Business Value
As the developer, I want a benchmark of the naive combination loop, so that I can see how it grows with staff and rooms.

## Description
**Context:**
TBD

**Scope:**
- Benchmark across growing numbers of staff, rooms and bookings
- Trace and p95 for the availability endpoint; findings in the E7 write-up draft

**Out of Scope, Known Limitations:**
- Fixing it: E7 Fix

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
- Blocked by: E7 Build: rooms and "anyone", with a naive combination loop
- Blocks: E7 Fix: fast first-available across resources
