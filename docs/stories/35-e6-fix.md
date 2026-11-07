# E6 Fix: availability that is correct and fast

## Business Value
As a customer, I want accurate free times that load quickly, so that I can book without trial and error.

## Description
**Context:**
TBD

**Scope:**
- Trade-off brief
- Nawar's algorithm subtask: problem, C# signature, constraints, examples, failing xUnit tests
- Load once, compute in memory; turn the E6 tests around; re-measure p95
- E6 write-up and ADR
- Deploy lens: CPU cost per request against the replica size
- Monitoring lens: latency histograms

**Out of Scope, Known Limitations:**
- Several resources per booking: E7

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
- Blocked by: E6 Prove: wrong availability around days off, and N+1 queries
- Blocks: Drill: a log flood hits the Log Analytics cap; E7 Build: rooms and "anyone", with a naive combination loop
