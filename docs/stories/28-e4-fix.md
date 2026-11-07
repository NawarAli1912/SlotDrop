# E4 Fix: keep the slot while the customer fills in the form

## Business Value
As a customer, I want the slot kept for me while I type my details, so that I don't lose it at the last step.

## Description
**Context:**
TBD

**Scope:**
- Trade-off brief; a booking lifecycle is introduced here
- How long the slot is kept is a PO decision, made when groomed
- The Vue form shows how long the slot is kept
- Turn the E4 test around; E4 write-up and ADR
- Deploy lens: replicas die and scale to zero, so nothing lives in memory and no background job runs inside the API
- Monitoring lens: a metric for expired holds

**Out of Scope, Known Limitations:**
- Payments: out of v1

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
- Blocked by: E4 Prove: a customer loses the slot while filling in the form
- Blocks: E5 Prove: retries create duplicate bookings
