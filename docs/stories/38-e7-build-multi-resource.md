# E7 Build: rooms and "anyone", with a naive combination loop

## Business Value
As a clinic owner, I want treatments that need a therapist and a room, and customers to be able to pick "anyone", so that bookings match how my clinic works.

## Description
**Context:**
TBD

**Scope:**
- A second resource kind (room), introduced here; a service can need staff and a room
- The customer can choose "anyone"
- Naive: loop over every staff and room combination

**Out of Scope, Known Limitations:**
- The algorithm: E7 Fix

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
- Blocked by: E6 Fix: availability that is correct and fast
- Blocks: E7 Prove: a benchmark shows the combinations explode
