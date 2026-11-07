---
title: "CI: what protects main, and how we prove it"
type: groom
for: 02-ci
status: closed
blocked_by: []
---

## Question
Which rules guard `main`, who may bypass them, and what evidence shows a red PR can't merge?

Facts:
- `main` is unprotected today: PR #1 was squash-merged with no checks (`gh pr view 1` → MERGED 2026-11-05T20:50:51Z, merge commit `23e6c1b`).
- Sprint 1's learning target includes "explain what a required check protects" ([LEARNING-PATH.md](../LEARNING-PATH.md), Sprint 1 row).
- Nawar is the only developer and can't approve his own PR.
- A required check can be selected only after it has reported once ([Local env and CI facts](02-local-env-and-ci.md) §3).

## Options
**Mechanism**
- **a. Repository ruleset** on `main`: the current GitHub feature; several rulesets can stack; can be exported as JSON
- **b. Classic branch protection rule**: older UI; same core rules
- **c. Nothing, discipline only** (naive): nothing stops a red merge

**Rules** (combinable)
- Require a pull request, 0 approvals (solo)
- Required status check = the CI job, source pinned to GitHub Actions
- Strict: branch must be up to date with `main` before merging
- Squash merge only
- Block force pushes and deletion of `main`

**Bypass**
- **a. No bypass, owner included**: the rules mean what they say; an emergency means editing the ruleset, which is visible
- **b. Owner may bypass**: faster when CI is broken; an easy habit to slide into

**Proof**
- **a. Red PR**: open a PR with a failing test after the ruleset is on; screenshot or `gh pr view` shows the merge blocked; close it
- **b. Trust the settings**: no evidence

## Resolution
Groomed with Nawar, 2026-11-05/06.
- Mechanism: a repository ruleset on `refs/heads/main` (option a).
- Rules: `pull_request` with 0 approvals and `"allowed_merge_methods": ["squash"]`; `required_status_checks` with `strict_required_status_checks_policy: true` and `{"context": "build-test", "integration_id": 15368}` (GitHub Actions, from `gh api apps/github-actions`, [CI facts](13-ci-facts.md)); `non_fast_forward`; `deletion`.
- Bypass: none, owner included. The bypass list starts empty by default ([CI facts](13-ci-facts.md); implied by the docs, not stated). If CI is broken, the ruleset is disabled on purpose, which leaves a visible trace.
- When: while the CI PR is open, after its `build-test` check has reported once (GitHub may reject a required check that hasn't run). The CI PR is the first merge under protection.
- Definition: created with one `POST repos/NawarAli1912/SlotDrop/rulesets` call; the JSON is recorded in the ADR (local + ClickUp), not in the repo. Repo merge settings: `gh repo edit NawarAli1912/SlotDrop --enable-squash-merge --enable-merge-commit=false --enable-rebase-merge=false --delete-branch-on-merge`.
- Proof: none; Nawar chose to trust the settings. Risk accepted: a misconfiguration (e.g. a check name that doesn't match) shows only when a red PR merges.
- Claude asks before running the `gh api` / `gh repo edit` calls, since they change repo settings.
