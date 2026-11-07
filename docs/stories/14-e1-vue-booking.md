# E1 Vue: pick a slot and book, on Static Web Apps

## Business Value
As a customer, I want a web page to pick a slot and book it, so that I can book from my phone.

## Description
**Context:**
TBD

**Scope:**
- Vue 3 + TypeScript + Vite app
- Pick staff and date, see free slots, book with name and email, see a confirmation (how many of the 4 screens this uses is decided when groomed)
- Static Web Apps Free in Terraform; CORS on the API; SPA deploy in CD

**Out of Scope, Known Limitations:**
- The waiting-room screen: E9
- Admin UI: out of v1

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
- Blocked by: E1 Build: list free slots and book one (naive); Infra: Terraform for the prod environment
- Blocks: none
