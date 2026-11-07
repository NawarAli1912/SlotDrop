---
title: "CI: what the workflow runs and how test results show up"
type: groom
for: 02-ci
status: closed
blocked_by: [13-ci-facts]
---

## Question
Which triggers and steps does the workflow have, and where does a developer see which test failed?

Hard requirements already settled:
- `dotnet restore --locked-mode`, `dotnet format --verify-no-changes`, build, `dotnet test --solution SlotDrop.slnx` ([Skeleton conventions](11-skeleton-conventions.md))
- `--logger trx` does not work under MTP ([Skeleton tool facts](09-skeleton-tool-facts.md) §3)
- one uniquely named job, because that name becomes the required check ([Local env and CI facts](02-local-env-and-ci.md) §3)

## Options
Facts from [CI facts](13-ci-facts.md): checkout v7.0.1, setup-dotnet v6.1.0 (`cache: true` needs `cache-dependency-path: '**/packages.lock.json'`, our lock files are in subfolders), upload-artifact v7.0.2.

**Test reporting**
- **a. Logs only** (naive): the failing test and ArchUnitNET's message are in the step log. Nothing to add; with 2 tests it is easy to read. Add a summary when finding a failure gets painful.
- **b. `dorny/test-reporter@v3` (3.2.0), summary mode** (default `use-actions-summary: true`): job summary from the TRX of `--report-xunit-trx`; no extra package, no `checks: write`. Parser compatibility checked by reading its code, not by running it.
- **c. MTP `Microsoft.Testing.Extensions.GitHubActionsReport` + `--report-gh`**: first-party; job summary verified locally; annotations unverified. One package in the test project.
- **d. `--report-xunit-markdown` appended to `$GITHUB_STEP_SUMMARY`**: built into xunit.v3; one shell line; no third-party action.
- **e. Upload the TRX as an artifact**: downloadable, not readable in the browser.

**Triggers**
- **a. `pull_request` + `push` to `main`**: PRs gate the merge; the push run checks the squash commit that actually landed
- **b. `pull_request` only**: half the runs; `main` itself never gets a run
- `concurrency` with `cancel-in-progress` for PR runs: a new push cancels the stale run

**Job shape**
- One job `build-test` on `ubuntu-latest`: checkout → setup-dotnet (global.json, cache) → `dotnet restore --locked-mode` → `dotnet format --verify-no-changes --no-restore` → `dotnet build --no-restore -c Release` → `dotnet test --solution SlotDrop.slnx --no-build -c Release`. `build-test` is the required check's name.
- `permissions: contents: read` only (no reporter needs more in options a, b-summary, c, d).

## Resolution
Groomed with Nawar, 2026-11-06.
- Reporting: logs only (naive). Next step when failures get hard to find: MTP `Microsoft.Testing.Extensions.GitHubActionsReport` with `--report-gh`.
- Triggers: `pull_request` + `push` to `main`; `concurrency` with `cancel-in-progress` for PR runs.
- One job `build-test` (the required check's name) on `ubuntu-latest`: `actions/checkout@v7` → `actions/setup-dotnet@v6` (`global-json-file: global.json`, `cache: true`, `cache-dependency-path: '**/packages.lock.json'`) → `dotnet restore --locked-mode` → `dotnet format --verify-no-changes --no-restore` → `dotnet build --no-restore -c Release` → `dotnet test --solution SlotDrop.slnx --no-build -c Release`.
- `permissions: contents: read` only.
- Actions pinned by major tag; SHA pinning with Dependabot moves to "CI: dependency and image scanning" (Sprint 2).
- Unverified until the first real run: the cache glob.
