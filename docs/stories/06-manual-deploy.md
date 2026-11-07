# Foundations: deploy by hand with az, and list what hurts

## Business Value
As the developer, I want to deploy once by hand, so that I feel the problems infrastructure as code and CD solve before I use them.

## Description
**Context:**
TBD

**Scope:**
- Create the resource group, a Consumption-only Container Apps environment, Postgres and the API app with `az`, recording every command
- Smoke-test `GET /tenants/{slug}`
- Write down what hurts: unrecorded steps, secrets in shell history, what a teardown misses, drift
- Tear everything down

**Out of Scope, Known Limitations:**
- Terraform and automation: the next stories

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
- Blocked by: Foundations: walking skeleton reads a tenant through every layer
- Blocks: Infra: Terraform bootstrap for state and the deploy identity
