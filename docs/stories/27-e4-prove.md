# E4 Prove: a customer loses the slot while filling in the form

## Business Value
As the developer, I want a test that shows a customer losing a slot mid-form, so that the checkout problem is shown with evidence.

## Description
**Context:**
TBD

**Scope:**
- Scenario: two customers pick the same slot, and the slower one is rejected only after filling in the form
- Measure how often it happens under the k6 drop profile
- Findings in the E4 write-up draft

**Out of Scope, Known Limitations:**
- Fixing it: E4 Fix

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
- Blocked by: E3 Fix: bookings of any length
- Blocks: E4 Fix: keep the slot while the customer fills in the form
