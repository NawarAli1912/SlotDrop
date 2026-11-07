# E6 Build: working hours, time off and a naive availability loop

## Business Value
As a business owner, I want slots to follow my staff's working hours and days off, so that I don't seed slots by hand.

## Description
**Context:**
TBD

**Scope:**
- Working hours and Time off, introduced here; how a tenant sets them (seed or the key-protected admin API) is decided when groomed
- Availability = loop over every candidate time and query for overlaps (naive)

**Out of Scope, Known Limitations:**
- The algorithm: E6 Fix

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
- Blocked by: E5 Fix: a retried request books once
- Blocks: E6 Prove: wrong availability around days off, and N+1 queries
