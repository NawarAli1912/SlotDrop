---
title: Split v1 into epics, stories and sprints
type: groom
for: epic
status: closed
blocked_by: [01-milan-structure, 02-local-env-and-ci, 03-azure-cost-and-iac, 07-problem-and-domain-model]
---

## Question
How does v1 break into epics (Foundations plus episodes E1–E9 from [LEARNING-PATH.md](../LEARNING-PATH.md); each episode has naive, notice, react and write-up stories)? Which stories make up each epic, and in what order? What is each sprint's goal? Sprint 1 = walking skeleton locally + CI on every PR. Sprint 2 = CD to Azure with Terraform.

Sprint capacity is unknown until Sprint 1 ends. Assumption: commit about 13–16 points in Sprint 1, then adjust to measured velocity.

## Options
Draft v0 (2026-11-03), superseded: it was grouped by slice. Regroup it by episode per LEARNING-PATH.md (confirmed 2026-11-04). Q1–Q3 below still stand.
- Foundations, S1: solution skeleton · CI on every PR · local env (Compose + Aspire + OTel) · walking-skeleton slice + Testcontainers
- Foundations, S2: Terraform bootstrap · Terraform prod · CD pipeline · Azure telemetry
- Slice 1: booking domain + seed · read slots (naive) · book a slot (naive, racy) · Vue booking flow + SWA · k6 baseline (ticket 06 moves here)
- Slice 2: exclusion constraint · holds with TTL · idempotency keys · correctness rerun
- Slice 3: free-interval algorithm · multi-resource intersection · first-available · computed availability API
- Slice 4: Redis read cache · rate limiting · waiting room + screen · k6 rerun + before/after report

Round 1 questions asked, unanswered: Q1 skeleton thin path (rec: `GET /tenants/{slug}`), Q2 CI as second story (rec: yes), Q3 algorithm task as subtask of its story (rec: yes).

## Resolution
Groomed with Nawar, 2026-11-05. 51 story files in `stories/`, listed in MAP.md with a goal for each sprint.
- Epics: Foundations (Sprints 1–2) and episodes E1–E9 (Sprints 3–10), per [LEARNING-PATH.md](../LEARNING-PATH.md).
- Each episode splits into Build naive → Prove → Fix and re-measure. There is no Build story when the naive code already exists (E2, E4, E5, E8, E9).
- Prove stories merge a characterisation test that asserts today's broken behaviour, so CI stays green; the Fix story turns it around and includes the write-up and ADR.
- Fix stories are named by the problem, not the solution.
- Sprint 1 order: solution skeleton → CI → local env → walking skeleton (`GET /tenants/{slug}`).
- Sprint 2: a manual `az` deploy story covers Build and Prove; Terraform bootstrap, Terraform prod and CD are the fix. App Insights is part of the CD story.
- E1's Vue page is its own story. E2 Prove is two stories: race test and k6 baseline; the SLO ticket moved under the k6 baseline story.
- Nawar's algorithm tasks are subtasks of their Fix stories (E6, E7, and E8 if a rate limiter is chosen).
- Every sprint ends with a story to review, re-split, re-plan and re-estimate (Nawar's addition); Sprint 10 ends with the v1 retro. Notes go to `sprints/sprint-NN.md` in this map folder.
- Review/re-plan stories are unpointed and timeboxed to 1.5 h, so velocity counts only product and ops work.
- A production drill story (1–2 points) each sprint from Sprint 2: break prod on purpose, notice it from monitoring, recover, write a blameless postmortem in `docs/postmortems/`. Sprint 5's drill (breaking migration) comes before E3 Fix, so its lesson shapes the real migration. Sprint 10's drill is restoring Postgres from a backup.
- Each sprint has a deploy lens and a monitoring lens (in LEARNING-PATH.md), added as scope bullets to that sprint's existing stories. Security and cost are folded into the lenses; incident response comes through the postmortems.
- A 1-point story "CI: dependency and image scanning" in Sprint 2.
- Point guesses: Sprint 1 about 14, Sprint 2 about 18 (over the 13–16 assumption; the Sprint 1 review will likely move the scanning story or the drill to Sprint 3). Real estimates come when the stories are groomed.
