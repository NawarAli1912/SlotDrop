# CI: dependency and image scanning

## Business Value
As the developer, I want vulnerable dependencies and images flagged automatically, so that I don't ship known holes.

## Description
**Context:**
TBD

**Scope:**
- Pin GitHub Actions to commit SHAs, kept current by Dependabot (moved here from the CI story, see [CI: what the workflow runs and how test results show up](../tickets/14-ci-workflow-and-reporting.md))
- Automated update PRs for NuGet packages, GitHub Actions and the base image
- CI fails on known-vulnerable packages; the API image is scanned before deploy
- Tools chosen when groomed

**Out of Scope, Known Limitations:**
- Fixing every finding at once: triage rules are set when groomed

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
- Blocked by: Foundations: CI on every pull request; Infra: deploy the API on every merge to main
- Blocks: none
