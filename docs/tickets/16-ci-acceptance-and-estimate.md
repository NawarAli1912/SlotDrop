---
title: "CI: acceptance criteria, docs and estimate"
type: groom
for: 02-ci
status: closed
blocked_by: [14-ci-workflow-and-reporting, 15-ci-protect-main]
---

## Question
With the workflow and protection settled: the story's acceptance criteria and test scenarios, which story-format sections are N/A, what ADRs and README changes it needs (ADRs go to `docs/adr/` and the ClickUp Doc "SlotDrop ADRs", never the repo), and how many points it is against the 3-point skeleton story.

## Resolution
Groomed with Nawar, 2026-11-06.
- Nominal: a PR into `main` runs `build-test`, green on current code; a merge runs it on `main`; merge is blocked until `build-test` is green and the branch is up to date (verified by reading the ruleset back with `gh api`, no red PR); only squash merge is offered and head branches are deleted; from the second run the NuGet cache hits.
- Corner: a format violation fails the format step; a package change without an updated lock file fails `restore --locked-mode`; a new push cancels the PR's older run.
- External contract change, new tests, deployment, monitoring: No.
- Docs: ADR-004 CI workflow, ADR-005 protecting `main` (local `docs/adr/` + ClickUp Doc "SlotDrop ADRs" on merge); README gets a CI status badge.
- Estimate: **2 points** (against the 3-point skeleton).
