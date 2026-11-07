# Infra: Terraform bootstrap for state and the deploy identity

## Business Value
As the developer, I want remote state and a GitHub-trusted deploy identity created once, so that pipelines can run Terraform without stored secrets.

## Description
**Context:**
TBD

**Scope:**
- `infra/bootstrap` root with local state, run once by Nawar (with a wizard for his steps)
- State resource group, storage account and container, with `prevent_destroy`
- User-assigned identity with federated credentials for the `production` environment and for pull requests
- Role assignments; outputs committed
- Register the resource providers the subscription lacks (`Microsoft.App`, `Microsoft.DBforPostgreSQL`, `Microsoft.OperationalInsights` were NotRegistered on 2026-11-05)

**Out of Scope, Known Limitations:**
- App resources: Terraform prod story

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
- Blocked by: Foundations: deploy by hand with az, and list what hurts
- Blocks: Infra: Terraform for the prod environment
