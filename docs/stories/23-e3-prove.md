# E3 Prove: tests show partial bookings and deadlocks

## Business Value
As the developer, I want tests that expose what goes wrong when bookings span several slots, so that the next model change is driven by evidence.

## Description
**Context:**
TBD

**Scope:**
- Concurrent tests for overlapping multi-slot bookings: partial bookings, deadlocks
- A service length that doesn't fit the grid
- Characterisation tests that merge green; findings in the E3 write-up draft

**Out of Scope, Known Limitations:**
- Fixing it: E3 Fix

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
- Blocked by: E3 Build: services with a duration that spans several slots
- Blocks: Drill: a migration breaks the old revision; E3 Fix: bookings of any length
