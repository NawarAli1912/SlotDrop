# ADR-004: CI workflow

Status: accepted, 2026-11-06 (PR #2)

## Context
PR #1 merged into `main` with no checks. Every later story assumes `main` builds, is formatted and passes its tests. ADR-003 already makes the build strict (all analyzers as errors, lock files, MTP runner); nothing enforces it yet. Facts ([CI facts](../tickets/13-ci-facts.md)): current majors are `actions/checkout@v7` and `actions/setup-dotnet@v6`; setup-dotnet caches NuGet keyed on `packages.lock.json`; xunit.v3 on MTP needs `dotnet test --solution`, and `--logger trx` no longer works.

## Options
- Reporting: step logs only · MTP `--report-gh` (GitHub Actions report extension) · TRX + `dorny/test-reporter` (needs `checks: write`) · TRX as an artifact.
- Shape: one job running every step · separate jobs for format, build, test.
- Triggers: `pull_request` only · `pull_request` + `push` to `main`.
- Action pinning: major tags · commit SHAs with Dependabot.

## Decision
- `.github/workflows/ci.yml`, triggers `pull_request` and `push` to `main`.
- `concurrency` grouped by workflow and PR branch; `cancel-in-progress` only for `pull_request` runs, so every commit on `main` gets a result.
- `permissions: contents: read`, nothing else.
- One job `build-test` on `ubuntu-latest`: checkout@v7 → setup-dotnet@v6 (`global-json-file`, `cache: true`, `cache-dependency-path: '**/packages.lock.json'`) → `dotnet restore --locked-mode` → `dotnet format --verify-no-changes --no-restore` → `dotnet build --no-restore -c Release` → `dotnet test --solution SlotDrop.slnx --no-build -c Release`.
- Reporting: logs only. Actions pinned by major tag.

## Consequences
- `build-test` is the name the `main` ruleset requires (ADR-005); renaming the job blocks every merge until the ruleset is updated.
- One job means one VM boot and one restore; the price is that a format error hides whether the build and tests would pass.
- `--locked-mode` turns a package change without an updated lock file into a red build (verified locally: NU1004).
- Failures are read in the step log. When that gets slow, add `--report-gh` first.
- Major tags can move under us; SHA pinning and Dependabot come with "CI: dependency and image scanning" (Sprint 2).
- Unverified until the first real run: the cache glob actually restores the cache on the second run.
