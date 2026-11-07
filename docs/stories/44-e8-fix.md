# E8 Fix: stay within the SLO at drop load

## Business Value
As a customer, I want the site to stay fast during a drop, so that I can actually book.

## Description
**Context:**
TBD

**Scope:**
- Trade-off brief per bottleneck
- Fix one variable at a time, measuring after each
- A Nawar algorithm subtask if a rate limiter is chosen
- E8 write-up and ADR; likely re-split when groomed
- Deploy lens: Redis is a cache only; replicas × pool size stays below Postgres's connection limit; autoscaling rules
- Monitoring lens: an SLO burn-rate alert

**Out of Scope, Known Limitations:**
- Fairness between users: E9

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
- Blocked by: E8 Prove: k6 at target load breaks the SLO
- Blocks: Drill: Redis restarts under load; E9 Prove: under drop load, access is unfair
