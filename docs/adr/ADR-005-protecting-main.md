# ADR-005: Protecting `main`

Status: accepted, 2026-11-06 (PR #2)

## Context
A CI workflow only reports; it doesn't stop a merge. Without protection, a red PR can still be merged, and a push can go straight to `main`. The repo `NawarAli1912/SlotDrop` is public on a free personal account, where both classic branch protection and repository rulesets are available. Nawar is the only developer, so required approvals can't be met by anyone else. GitHub may reject a required check that has never reported.

## Options
- Mechanism: repository ruleset · classic branch protection · none (discipline only).
- Bypass: owner may bypass · nobody bypasses.
- Definition: one `gh api` call recorded in this ADR · a JSON file in the repo · Terraform (Sprint 2).

## Decision
- A repository ruleset on `refs/heads/main`, created while the CI PR is open, after `build-test` has reported once. The CI PR is the first merge under protection.
- Rules: PR required with 0 approvals and squash only; `build-test` required from GitHub Actions (integration 15368), branch must be up to date (strict); no force push; no deletion.
- Bypass list empty, owner included. If CI is broken, the ruleset is disabled on purpose, which leaves a visible trace.
- Repo merge settings: squash only, delete head branches on merge.

```bash
gh repo edit NawarAli1912/SlotDrop --enable-squash-merge --enable-merge-commit=false --enable-rebase-merge=false --delete-branch-on-merge
```

```bash
gh api -X POST repos/NawarAli1912/SlotDrop/rulesets --input - <<'JSON'
{
  "name": "protect-main",
  "target": "branch",
  "enforcement": "active",
  "bypass_actors": [],
  "conditions": { "ref_name": { "include": ["refs/heads/main"], "exclude": [] } },
  "rules": [
    { "type": "deletion" },
    { "type": "non_fast_forward" },
    {
      "type": "pull_request",
      "parameters": {
        "required_approving_review_count": 0,
        "dismiss_stale_reviews_on_push": false,
        "require_code_owner_review": false,
        "require_last_push_approval": false,
        "required_review_thread_resolution": false,
        "allowed_merge_methods": ["squash"]
      }
    },
    {
      "type": "required_status_checks",
      "parameters": {
        "strict_required_status_checks_policy": true,
        "do_not_enforce_on_create": false,
        "required_status_checks": [{ "context": "build-test", "integration_id": 15368 }]
      }
    }
  ]
}
JSON
```

## Consequences
- Every change reaches `main` through a green, up-to-date, squash-merged PR, including the owner's.
- Strict mode means a PR behind `main` must be updated and re-run before merging; costs a CI run per update, buys "tested against what it merges into".
- Pinning the check to integration 15368 stops another app or a commit status from faking `build-test`.
- No red PR proves the ruleset works; a misconfiguration (e.g. a check name mismatch) shows only when a red PR merges. Nawar accepted this risk.
- The ruleset lives only in GitHub settings until Terraform takes over settings as code in Sprint 2.
- As applied (2026-11-06): the ruleset (id 26696645) and merge settings were created after PR #2 merged, not while it was open, so PR #2 merged unprotected. Read back, the ruleset matches this ADR, plus a GitHub default `require_extra_approval_for_unattributed_changes: true` that the POST didn't set.
