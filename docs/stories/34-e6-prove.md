# E6 Prove: wrong availability around days off, and N+1 queries

## Business Value
As the developer, I want tests and traces that show where naive availability goes wrong, so that the algorithm is driven by evidence.

## Description
**Context:**
TBD

**Scope:**
- Tests around days off and other edge cases
- A trace showing one query per candidate time
- k6 p95 for the availability endpoint

**Out of Scope, Known Limitations:**
- Fixing it: E6 Fix

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
- Blocked by: E6 Build: working hours, time off and a naive availability loop
- Blocks: E6 Fix: availability that is correct and fast
