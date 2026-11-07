# SlotDrop v1: learning path

Confirmed by Nawar, 2026-11-04. The epics are Foundations + episodes E1–E9.

## The rule
Every problem is an **episode** with five steps:

1. **Naive.** Build the simplest thing that could work.
2. **Notice.** Make the problem visible with evidence: a failing test, a k6 number or a trace. Gut feeling doesn't count.
3. **Options.** Compare 2–3 reactions and their trade-offs.
4. **React.** Build the chosen fix, then measure again with the same test.
5. **Write it up.** "We did X because of problem Y; the numbers went from A to B."

There is no up-front domain model. [PROBLEM.md](PROBLEM.md) states only the problem; each piece of the model is earned by an episode and explained in its write-up.

## Sprints

Sprints are an ordered sequence, each ending when its goal is met; no dates, points or time estimates (dropped 2026-11-06). Sprints 3–10 stay provisional and get re-planned at each review.

| Sprint | Episode | Learning target: after this sprint Nawar can… |
|---|---|---|
| 1 | Foundations: structure + CI | explain Clean Architecture's dependency rule and prove it with architecture tests; read and change a GitHub Actions workflow; explain what a required check protects |
| 2 | Foundations: shipping to Azure | explain why infrastructure as code and OIDC beat clicking and stored secrets; run Terraform plan/apply; trace a request from Azure in App Insights |
| 3 | E1 · The first booking | model a first domain (staff, slot, booking); write EF Core commands and Dapper queries; explain Read Committed |
| 4 | E2 · Double booking | reproduce a race condition with a test; compare a unique constraint, row locks and SERIALIZABLE; write a k6 test and define an SLO |
| 5 | E3 · Services of different lengths | explain why the slots table breaks; compare models for bookings of different lengths; migrate a schema with data safely |
| 6 | E4 · Abandoned checkouts + E5 · Retries | model a booking state machine with expiry; compare lazy expiry with a sweeper (Service Bus); explain idempotency and at-least-once delivery |
| 7 | E6 · Availability is wrong and slow | write and analyse interval algorithms; find an N+1 query with a trace |
| 8 | E7 · Two resources, "anyone" | intersect free intervals across resources; use a heap / k-way merge for first-available |
| 9 | E8 · The drop melts the database | read a load-test failure (pool exhaustion, p95); add cache-aside with invalidation and handle a stampede; implement a token bucket |
| 10 | E9 · Fair access during a drop | design a waiting room across replicas; explain backpressure and fairness; present before/after evidence |

## Domain model by episode
Each episode adds only the concepts its problem needs. How a concept is modelled is decided inside that episode, not here.

| When | New in the model | Why it's needed then |
|---|---|---|
| Sprint 1 | Tenant (id, slug, name) | the walking skeleton needs one real row to read through every layer |
| E1 | Staff, Slot (seeded 30-min cells), Booking (slot + name + email) | the smallest thing a customer can book |
| E2 | Release, only if the drop test needs one | slots must open at one moment for k6 to simulate a drop |
| E3 | Service with a duration | the problem is bookings of different lengths |
| E4 | a booking lifecycle (held → confirmed …) | the problem is losing the slot while filling in the form |
| E5 | an idempotency record | the problem is retries creating duplicates |
| E6 | Working hours, Time off | the problem is that seeded slots can't follow real schedules |
| E7 | a second kind of resource (room); "anyone" | the problem is appointments that need two resources |
| E8 | nothing | infrastructure only |
| E9 | a waiting-room entry | the problem is fair access |

Every sprint ends with a story to review, re-split and re-plan: run the learning check, check that the sprint taught something new, write notes to `sprints/sprint-NN.md`, and groom the next sprint.

## Threads through every sprint
Beside each episode, every sprint teaches two lenses and, from Sprint 2, one production drill. Lens work is added as scope bullets to that sprint's existing stories. Each drill is its own small story: break prod on purpose, notice it from monitoring, recover, and write a blameless postmortem as a page in the ClickUp Doc "SlotDrop Postmortems".

| Sprint | Deploy lens: how deploying shapes the code | Monitoring lens | Drill |
|---|---|---|---|
| 1 | config only from the environment; one image for local and Azure | logs, traces and metrics in the Aspire dashboard | — |
| 2 | immutable SHA images, revisions, graceful shutdown; OIDC instead of secrets; cost guards | App Insights, KQL, health probes, budget alert | roll back a broken deploy |
| 3 | scale to zero means cold starts; SPA and API on different origins means CORS | structured logs; one booking's trace end to end | measure the cold start |
| 4 | a k6 run on prod costs telemetry, so sample | business metrics, first SLI and SLO, a dashboard | Postgres restarts during load |
| 5 | old and new revisions run side by side, so migrations must suit both | errors per revision during a migration | a migration breaks the old revision (before E3 Fix) |
| 6 | replicas die and scale to zero: no in-memory state, no background job inside the API | expired-hold and replayed-request metrics | a replica dies mid-request |
| 7 | CPU per request against the replica size | latency histograms, N+1 in traces | a log flood hits the Log Analytics cap |
| 8 | CPU-bound work and replica sizing | per-endpoint p95 dashboard; benchmark vs prod | rotate the admin API key |
| 9 | Redis container is a cache only; replicas × pool size under Postgres's connection limit; autoscaling | saturation metrics, SLO burn-rate alert | Redis restarts under load |
| 10 | waiting-room state shared across replicas | queue length, admission rate, stuck-queue alert | restore Postgres from a backup |

Security (OIDC, secrets, the admin key, abuse) and cost run through the deploy lens. Incident response is learned through the postmortems. The deploy facts come from ticket "Azure monthly cost of v1 and the Terraform + OIDC setup facts"; Postgres B1ms's connection limit is not yet verified.

## Episodes in detail

**Sprint 1 · Foundations: structure + CI**
- Build: solution skeleton, architecture tests, CI on every PR, Compose + Aspire dashboard + OTel, a thin `GET /tenants/{slug}` through every layer, Testcontainers.
- This is tooling, not a problem episode. Forcing a fake "main broke" problem costs time and teaches little.

**Sprint 2 · Foundations: shipping to Azure**
- Naive: deploy once by hand with `az` commands.
- Notice: list what's unreproducible: which commands ran, which secrets sit in shell history, what a teardown would miss.
- React: Terraform bootstrap + prod, then CD with OIDC, smoke test, App Insights.

**Sprint 3 · E1 · The first booking**
- Naive: a pre-filled `slots` table (one row per 30-min cell per barber); book = "is the slot free? then insert", name + email; seed one barber shop; a minimal Vue page to pick a slot and book.
- Notice: nothing yet. It works for one user, and that is the point.

**Sprint 4 · E2 · Double booking**
- Notice:
  - an integration test fires 50 parallel bookings at one slot and gets more than one booking
  - k6 on Azure runs the slot-drop profile (ticket 06) and counts double bookings, p95 and errors; this is the baseline
- Options: unique constraint on the slot row · `SELECT … FOR UPDATE` · SERIALIZABLE + retry
- React: likely a unique constraint (cheapest, correct for fixed cells). Rerun both tests.

**Sprint 5 · E3 · Services of different lengths**
- Naive: a clinic tenant has 45- and 90-min services, so a booking takes 2–3 slot rows.
- Notice: tests show partial bookings and lock-ordering deadlocks, and a 20-min service doesn't fit the grid.
- Options: smaller cells · multi-row locking · time ranges + exclusion constraint
- React: the new booking model is chosen in the episode, from the options; migrate the data.

**Sprint 6 · E4 · Abandoned checkouts + E5 · Retries**
- E4:
  - Naive: no hold; the slot is only taken on submit.
  - Notice: a k6 or e2e scenario where a user fills in the form and gets 409 at the end.
  - React: holds with a TTL; lazy expiry vs a sweeper.
- E5:
  - Naive: the client retries on timeout.
  - Notice: inject latency, then one customer ends up with two bookings.
  - React: an `Idempotency-Key` header.

**Sprint 7 · E6 · Availability is wrong and slow**
- Problem: seeding slots by hand stops working once staff have working hours and days off.
- Naive: availability = loop over every candidate time and query whether it overlaps.
- Notice: wrong answers around days off (tests), plus N+1 queries and a high p95 (trace).
- React: Nawar's interval algorithm over the schedule: one query, then compute in memory.

**Sprint 8 · E7 · Two resources, "anyone"**
- Naive: loop over every barber and every room combination.
- Notice: the combinations explode (benchmark).
- React: Nawar's algorithms: interval intersection, then first-available with a heap.

**Sprint 9 · E8 · The drop melts the database**
- Notice: rerun k6 at target load: connection pool exhausted, p95 over the SLO, DB CPU at 100%.
- React, one variable at a time, measuring after each:
  1. Redis cache-aside for availability
  2. stampede protection
  3. rate limiting (Nawar's token bucket, Redis-backed)

**Sprint 10 · E9 · Fair access during a drop**
- Notice: under the rate limit, users get 429s at random; refresh-spammers win.
- React: waiting room with admission tokens, plus a Vue waiting-room screen. Final k6 run and before/after report.

## Write-up per episode
One page per episode in the ClickUp Doc "SlotDrop Journey". Claude drafts the evidence, options and numbers. Nawar writes "The problem" and "What I learned" in his own words. Each decision also gets a short ADR, kept in `docs/adr/` and mirrored to the ClickUp Doc "SlotDrop ADRs", linked from the write-up.

```markdown
# E2 · Double booking
## The problem          what broke, in one paragraph
## How we noticed       the test / k6 run / trace, with numbers
## The naive code       link to the commit
## Options              2–3, with trade-offs
## What we chose, why   links to the ADR
## After                same test, new numbers
## What I learned       written by Nawar
```

## Learning check per sprint
At sprint review, Nawar explains the episode without notes in five minutes: problem, evidence, options, choice. If he can't, the sprint's learning target isn't met, even if the code is merged.
