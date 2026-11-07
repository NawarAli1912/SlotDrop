# E5 Fix: a retried request books once

## Business Value
As a customer, I want a retried booking request to give me the same booking, so that a flaky connection never double-books me or shows a false error.

## Description
**Context:**
TBD

**Scope:**
- Trade-off brief; an idempotency record is introduced here
- Turn the E5 test around; E5 write-up and ADR
- Monitoring lens: a metric for replayed requests

**Out of Scope, Known Limitations:**
- Retries between internal services: none exist in v1

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
- Blocked by: E5 Prove: retries create duplicate bookings
- Blocks: Drill: a replica dies mid-request; E6 Build: working hours, time off and a naive availability loop
