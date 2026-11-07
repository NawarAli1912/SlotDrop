# E3 Fix: bookings of any length

## Business Value
As a customer, I want to book a service of any length without clashes, so that the time I book is really mine.

## Description
**Context:**
TBD

**Scope:**
- Trade-off brief; the new booking model is chosen in this story
- Data migration with a reviewed SQL script
- Turn the E3 tests around; rerun the race test and the k6 baseline
- E3 write-up and ADR
- Deploy lens: the migration must work while the old and new revisions run side by side
- Monitoring lens: watch errors per revision during the migration

**Out of Scope, Known Limitations:**
- Keeping a slot during checkout: E4
- Working hours: E6

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
- Blocked by: E3 Prove: tests show partial bookings and deadlocks; Drill: a migration breaks the old revision
- Blocks: E4 Prove: a customer loses the slot while filling in the form
