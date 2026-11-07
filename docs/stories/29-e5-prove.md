# E5 Prove: retries create duplicate bookings

## Business Value
As the developer, I want a test that shows what a retried request does, so that the retry problem is shown with evidence.

## Description
**Context:**
TBD

**Scope:**
- Inject latency so that the client times out and retries
- Show the result: a duplicate booking or a false error
- Characterisation test; findings in the E5 write-up draft

**Out of Scope, Known Limitations:**
- Fixing it: E5 Fix

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
- Blocked by: E4 Fix: keep the slot while the customer fills in the form
- Blocks: E5 Fix: a retried request books once
