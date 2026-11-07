# Drill: a log flood hits the Log Analytics cap

## Business Value
As the operator, I want to practise a burst of logs that hits the daily ingestion cap on prod on purpose, so that when it happens for real I notice it from monitoring and recover quickly.

## Description
**Context:**
TBD

**Scope:**
- Cause it on purpose on Azure prod: a burst of logs that hits the daily ingestion cap
- Notice it: which signal showed it, how fast, and what was missing
- Recover, and time the recovery
- Blameless postmortem as a page in the ClickUp Doc "SlotDrop Postmortems" (timeline, detection, impact, cause, actions)

**Out of Scope, Known Limitations:**
- Automating the recovery, unless the postmortem shows it's needed

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
- Blocked by: E6 Fix: availability that is correct and fast
- Blocks: none
