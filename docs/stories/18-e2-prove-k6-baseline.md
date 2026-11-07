# E2 Prove: k6 slot-drop baseline on Azure

## Business Value
As the developer, I want a repeatable slot-drop load test with a defined SLO, so that every later fix is measured against the same baseline.

## Description
**Context:**
TBD

**Scope:**
- Owns ticket "What load and SLO define 'survives a slot drop'" (traffic profile and pass criteria)
- k6 script in the repo
- A release (slots opening at one moment), only if the drop test needs one; decided when groomed
- Run against Azure prod with telemetry sampled; a query that counts double bookings
- Baseline recorded: p95, error rate, double bookings
- Monitoring lens: telemetry sampling during k6 (cost); the first SLI and SLO; a dashboard

**Out of Scope, Known Limitations:**
- Fixing anything
- k6 in CI

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
- Blocked by: E1 Build: list free slots and book one (naive); Infra: deploy the API on every merge to main
- Blocks: E2 Fix: one slot, one booking
