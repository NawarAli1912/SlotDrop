# E1 Build: list free slots and book one (naive)

## Business Value
As a customer, I want to see a barber's free slots and book one with my name and email, so that I can get a haircut without calling.

## Description
**Context:**
TBD

**Scope:**
- Staff, Slot (seeded 30-min cells) and Booking (slot, name, email), as named in PROBLEM.md
- Seed one barber shop with staff and a week of slots
- List free slots for a staff member and a date
- Book: check that the slot is free, then insert (naive on purpose)
- Logs and traces; deployed to Azure
- Monitoring lens: structured logs carry the booking id; find one booking's trace in App Insights

**Out of Scope, Known Limitations:**
- Protection against concurrent bookings: E2
- Services of different lengths: E3
- UI: next story

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
- Blocked by: Infra: deploy the API on every merge to main
- Blocks: E1 Vue: pick a slot and book, on Static Web Apps; Drill: measure the cold start; E2 Prove: a race test shows double booking; E2 Prove: k6 slot-drop baseline on Azure
