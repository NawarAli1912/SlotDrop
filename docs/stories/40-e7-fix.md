# E7 Fix: fast first-available across resources

## Business Value
As a customer, I want the earliest time with anyone, so that I get the soonest appointment without trying every person.

## Description
**Context:**
TBD

**Scope:**
- Trade-off brief
- Nawar's algorithm subtasks, LeetCode-style
- Re-run the benchmark and k6; E7 write-up and ADR
- Monitoring lens: a per-endpoint p95 dashboard; compare the benchmark with prod

**Out of Scope, Known Limitations:**
- Drop-scale load: E8

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
- Blocked by: E7 Prove: a benchmark shows the combinations explode
- Blocks: Drill: rotate the admin API key; E8 Prove: k6 at target load breaks the SLO
