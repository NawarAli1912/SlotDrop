# Infra: Terraform for the prod environment

## Business Value
As the developer, I want the prod environment defined in code, so that it can be reviewed, recreated and torn down safely.

## Description
**Context:**
TBD

**Scope:**
- azurerm `~> 5.12` with the azurerm backend (OIDC + Entra auth)
- Consumption-only Container Apps environment; Log Analytics with a daily cap; App Insights
- Postgres Flexible B1ms, 32 GB, no HA; Redis container (min = max = 1, internal TCP)
- API app at min 0, Terraform ignoring the image; budget alert at about $10/month
- Plan on PR with a read-only identity; apply on `main` in the `production` environment

**Out of Scope, Known Limitations:**
- SPA hosting: E1 Vue story
- Service Bus and a worker: when an episode needs them

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
- Blocked by: Infra: Terraform bootstrap for state and the deploy identity
- Blocks: Infra: deploy the API on every merge to main; E1 Vue: pick a slot and book, on Static Web Apps
