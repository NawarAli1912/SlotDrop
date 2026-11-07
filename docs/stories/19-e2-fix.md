# E2 Fix: one slot, one booking

## Business Value
As a customer, I want my booking to be mine alone, so that I never arrive to find someone else in my chair.

## Description
**Context:**
TBD

**Scope:**
- Trade-off brief comparing the options
- Build the chosen fix
- Turn the race test around: exactly one booking succeeds, the rest get a clear conflict
- Rerun the k6 baseline and compare
- E2 write-up (Nawar writes the problem and "What I learned") and ADR
- Monitoring lens: business metrics for bookings and conflicts

**Out of Scope, Known Limitations:**
- Bookings that span several slots: E3

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
- Blocked by: E2 Prove: a race test shows double booking; E2 Prove: k6 slot-drop baseline on Azure
- Blocks: Drill: Postgres restarts during load; E3 Build: services with a duration that spans several slots
