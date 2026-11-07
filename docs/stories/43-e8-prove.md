# E8 Prove: k6 at target load breaks the SLO

## Business Value
As the developer, I want to know exactly where the system breaks at drop load, so that each fix targets a measured bottleneck.

## Description
**Context:**
TBD

**Scope:**
- Rerun k6 at the target profile from the SLO ticket
- Collect pool usage, DB CPU, p95 and errors; read the traces
- Rank the bottlenecks in the E8 write-up draft
- Monitoring lens: saturation metrics (connection pool, CPU, DB connections)

**Out of Scope, Known Limitations:**
- Fixing it: E8 Fix

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
- Blocked by: E7 Fix: fast first-available across resources
- Blocks: E8 Fix: stay within the SLO at drop load
