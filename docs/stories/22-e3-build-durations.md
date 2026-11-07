# E3 Build: services with a duration that spans several slots

## Business Value
As a clinic owner, I want to offer 45- and 90-minute services, so that customers can book the treatment they actually need.

## Description
**Context:**
TBD

**Scope:**
- Service with a duration, introduced here
- Seed a clinic tenant
- Booking a service takes the consecutive slots it needs (naive)
- The slot list shows only start times where the service fits

**Out of Scope, Known Limitations:**
- Fixing the problems this causes: E3 Fix

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
- Blocked by: E2 Fix: one slot, one booking
- Blocks: E3 Prove: tests show partial bookings and deadlocks
