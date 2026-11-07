# SlotDrop v1

## Destination
v1 (Foundations + episodes E1–E9, covering slices 1–4) split into epics and stories in sprint order, each sprint with a goal. Sprint 1 and Sprint 2 stories groomed to `ready`. Stories for later sprints stay `split` and get groomed just in time, two weeks before their sprint.

## Notes
Repos: /Users/nawarali/Desktop/SlotDrop (not created yet)
- SlotDrop is a multi-tenant booking platform for barbers and clinics. Its two hard problems are double booking under concurrency and surviving slot-drop traffic spikes.
- v1 = four slices:
  1. naive booking + baseline load test
  2. correctness: holds, idempotency, exclusion constraints
  3. availability engine
  4. slot-drop survival: waiting room, rate limiting, caching
- Stack:
  - .NET 10, EF Core plus raw SQL where concurrency matters
  - Postgres 17, Redis (container), Azure Service Bus Basic
  - Vue 3 + TS + Vite: 4 screens, no admin UI
  - k6, OpenTelemetry, standalone Aspire dashboard in Docker Compose
- Deploy: Azure Container Apps, Terraform, GitHub Actions with OIDC. Environments are local Compose plus one Azure `prod`.
- Code structure: Clean Architecture (Domain / Application / Infrastructure / Api), enforced by architecture tests.
- Learning path: [LEARNING-PATH.md](LEARNING-PATH.md), with a learning target per sprint. Every problem is an episode: naive, notice with evidence, options, react, write-up. Epics are Foundations + episodes E1–E9; they cover the same content as slices 1–4, regrouped by problem.
- Problem statement: [PROBLEM.md](PROBLEM.md). No up-front domain model; it grows per episode.
- Process:
  - sprints are an ordered sequence with a goal each; no dates, no story points or time estimates (Nawar, 2026-11-06: a learning project, timing and estimation add nothing)
  - one story = one branch = one PR, squash-merged to a protected `main`
  - tracked in a ClickUp folder `SlotDrop`: Roadmap, Backlog, one list per sprint
  - ClickUp (created 2026-11-05, Default space): folder 901215882833; lists Roadmap 901223574306, Backlog 901223574307, Sprint 1 901223574308. Sprint 1 stories are tasks; a story's tickets are its subtasks. Tags carry the grooming state (split / grooming / ready). Later sprint lists get created when that sprint is groomed. Backlog holds every not-yet-groomed story, tagged `sprint-N` + epic tag + grooming state; Roadmap holds one task per sprint (goal, learning target, stories). Doc "SlotDrop ADRs": 8cjtxr1-592 (ADR-001 page 8cjtxr1-472, ADR-002 8cjtxr1-492, ADR-003 8cjtxr1-512, ADR-004 8cjtxr1-532, ADR-005 8cjtxr1-552); "SlotDrop Journey" and "SlotDrop Postmortems" get created with their first page.
- Work split:
  - Nawar writes the algorithm code. Each task comes LeetCode-style: problem, signature, constraints, examples, failing xUnit tests.
  - Claude writes the rest, with a trade-off brief before each story, and opens the PR. Nawar reviews and merges.
- Learning guides for topics Nawar wants to catch up on live in `learning/` and get mirrored to the ClickUp Doc "SlotDrop Learning" (created with its first page). First: [OpenTelemetry, spans and the words around them](learning/01-opentelemetry.md), 2026-11-06, waiting for the ClickUp limit to reset.
- Each episode ends with a write-up as a page in the ClickUp Doc "SlotDrop Journey". Claude drafts the evidence, options and numbers; Nawar writes the problem and "What I learned". At sprint review Nawar explains the episode without notes.
- ADRs follow Context → Options → Decision → Consequences. They live in `docs/adr/` and are mirrored to the ClickUp Doc "SlotDrop ADRs" when the PR merges. The roadmap, stories, tickets and ADRs live in the repo under `docs/` so any agent opening the repo has the context (Nawar, 2026-11-07, reversing the 2026-11-05 rule). Journey write-ups and postmortems stay in ClickUp Docs.
- Definition of Done:
  - PR merged with green CI: build, format, unit tests, Testcontainers integration tests
  - deployed and smoke-tested on Azure (from Sprint 2)
  - logs and traces on new endpoints
  - an ADR for each decision; docs updated
- Story format: `~/.my-skills/shared/STORY-FORMAT.md`. Its Deployment section's "OpenSearch" fields map to the Aspire dashboard locally and to Azure Monitor / App Insights on Azure.

## Stories
<!-- in grooming order; status is split | grooming | ready -->

**Sprint 1** · goal: The walking skeleton runs locally with traces, and every PR must pass CI

- [Foundations: solution skeleton with architecture tests](stories/01-solution-skeleton.md) · ready · done: [PR #1](https://github.com/NawarAli1912/SlotDrop/pull/1) merged 2026-11-05
- [Foundations: CI on every pull request](stories/02-ci.md) · ready · done: [PR #2](https://github.com/NawarAli1912/SlotDrop/pull/2) merged 2026-11-06; ruleset protect-main (26696645) applied after the merge
- [Foundations: local environment with traces in the Aspire dashboard](stories/03-local-env.md) · ready · done: [PR #3](https://github.com/NawarAli1912/SlotDrop/pull/3) merged 2026-11-06; ClickUp pending: ADR-006/007 pages, task status (MCP daily limit)
- [Foundations: walking skeleton reads a tenant through every layer](stories/04-walking-skeleton.md) · split
- [Process: Sprint 1 review, re-split and re-plan](stories/05-sprint-01-replan.md) · split

**Sprint 2** · goal: Every merge to `main` deploys to Azure through Terraform + OIDC, with no manual steps, and a bad deploy can be rolled back

- [Foundations: deploy by hand with az, and list what hurts](stories/06-manual-deploy.md) · split
- [Infra: Terraform bootstrap for state and the deploy identity](stories/07-terraform-bootstrap.md) · split
- [Infra: Terraform for the prod environment](stories/08-terraform-prod.md) · split
- [Infra: deploy the API on every merge to main](stories/09-cd.md) · split
- [CI: dependency and image scanning](stories/10-ci-scanning.md) · split
- [Drill: roll back a broken deploy](stories/11-drill-rollback.md) · split
- [Process: Sprint 2 review, re-split and re-plan](stories/12-sprint-02-replan.md) · split

**Sprint 3** · goal: A customer can book a slot end to end on Azure (naive)

- [E1 Build: list free slots and book one (naive)](stories/13-e1-build-booking.md) · split
- [E1 Vue: pick a slot and book, on Static Web Apps](stories/14-e1-vue-booking.md) · split
- [Drill: measure the cold start](stories/15-drill-cold-start.md) · split
- [Process: Sprint 3 review, re-split and re-plan](stories/16-sprint-03-replan.md) · split

**Sprint 4** · goal: Double booking is proven, measured and fixed

- [E2 Prove: a race test shows double booking](stories/17-e2-prove-race.md) · split
- [E2 Prove: k6 slot-drop baseline on Azure](stories/18-e2-prove-k6-baseline.md) · split
- [E2 Fix: one slot, one booking](stories/19-e2-fix.md) · split
- [Drill: Postgres restarts during load](stories/20-drill-postgres-restart.md) · split
- [Process: Sprint 4 review, re-split and re-plan](stories/21-sprint-04-replan.md) · split

**Sprint 5** · goal: Bookings of any length work without double booking

- [E3 Build: services with a duration that spans several slots](stories/22-e3-build-durations.md) · split
- [E3 Prove: tests show partial bookings and deadlocks](stories/23-e3-prove.md) · split
- [Drill: a migration breaks the old revision](stories/24-drill-breaking-migration.md) · split
- [E3 Fix: bookings of any length](stories/25-e3-fix.md) · split
- [Process: Sprint 5 review, re-split and re-plan](stories/26-sprint-05-replan.md) · split

**Sprint 6** · goal: Abandoned forms and retries no longer lose or duplicate bookings

- [E4 Prove: a customer loses the slot while filling in the form](stories/27-e4-prove.md) · split
- [E4 Fix: keep the slot while the customer fills in the form](stories/28-e4-fix.md) · split
- [E5 Prove: retries create duplicate bookings](stories/29-e5-prove.md) · split
- [E5 Fix: a retried request books once](stories/30-e5-fix.md) · split
- [Drill: a replica dies mid-request](stories/31-drill-replica-killed.md) · split
- [Process: Sprint 6 review, re-split and re-plan](stories/32-sprint-06-replan.md) · split

**Sprint 7** · goal: Availability follows real schedules, correctly and fast

- [E6 Build: working hours, time off and a naive availability loop](stories/33-e6-build-schedules.md) · split
- [E6 Prove: wrong availability around days off, and N+1 queries](stories/34-e6-prove.md) · split
- [E6 Fix: availability that is correct and fast](stories/35-e6-fix.md) · split
- [Drill: a log flood hits the Log Analytics cap](stories/36-drill-log-flood.md) · split
- [Process: Sprint 7 review, re-split and re-plan](stories/37-sprint-07-replan.md) · split

**Sprint 8** · goal: First-available works across staff and rooms

- [E7 Build: rooms and "anyone", with a naive combination loop](stories/38-e7-build-multi-resource.md) · split
- [E7 Prove: a benchmark shows the combinations explode](stories/39-e7-prove.md) · split
- [E7 Fix: fast first-available across resources](stories/40-e7-fix.md) · split
- [Drill: rotate the admin API key](stories/41-drill-key-rotation.md) · split
- [Process: Sprint 8 review, re-split and re-plan](stories/42-sprint-08-replan.md) · split

**Sprint 9** · goal: A drop at target load stays within the SLO

- [E8 Prove: k6 at target load breaks the SLO](stories/43-e8-prove.md) · split
- [E8 Fix: stay within the SLO at drop load](stories/44-e8-fix.md) · split
- [Drill: Redis restarts under load](stories/45-drill-redis-restart.md) · split
- [Process: Sprint 9 review, re-split and re-plan](stories/46-sprint-09-replan.md) · split

**Sprint 10** · goal: Access during a drop is fair, and the v1 before/after report is published

- [E9 Prove: under drop load, access is unfair](stories/47-e9-prove.md) · split
- [E9 Fix: fair access when demand exceeds capacity](stories/48-e9-fix.md) · split
- [Drill: restore Postgres from a backup](stories/49-drill-restore-backup.md) · split
- [Docs: v1 before/after report in the README](stories/50-v1-report.md) · split
- [Process: Sprint 10 review and v1 retrospective](stories/51-sprint-10-retro.md) · split

## Waiting on PO
None. Nawar is PO.

## Split history
- 2026-11-05: first split, 41 stories: Foundations + episodes E1–E9 (Build → Prove → Fix) plus a review/re-plan story at the end of every sprint, see [Split v1 into epics, stories and sprints](tickets/05-split-v1.md)
- 2026-11-05: 51 stories: added a production drill per sprint from Sprint 2, a CI scanning story, and deploy/monitoring lenses in existing stories, because Nawar wants to learn deploying, monitoring and real production issues, see [Split v1 into epics, stories and sprints](tickets/05-split-v1.md)

## Decisions so far
- Scope, stack, process and DoD were settled in the grooming session on 2026-11-03 (see Notes).
- [How Milan Jovanović structures a .NET 10 Clean Architecture service today](tickets/01-milan-structure.md): Domain/Application/Infrastructure/Web.Api/SharedKernel; feature folders; hand-rolled CQRS handlers + Result<T> (no MediatR); FluentValidation; Minimal API IEndpoint; ArchUnitNET; CPM + warnings-as-errors; OTel via UseOtlpExporter; EF Core writes, Dapper reads, SQL-script migrations.
- [Azure monthly cost of v1 and the Terraform + OIDC setup facts](tickets/03-azure-cost-and-iac.md): ~$6/mo idle in year 1, ~$25 after (Postgres); Redis as container (cache only), SWA Free for the SPA, ghcr.io; watch Log Analytics >5 GB and Dedicated-plan fees; azurerm ~> 5.12; bootstrap root for state + identity; azure/login@v3 with user-assigned identity; deploy via `az containerapp update --image`, Terraform ignores image.
- [Define the problem and the v1 domain model](tickets/07-problem-and-domain-model.md): problem in PROBLEM.md; naive-first episodes instead of an up-front model; Sprint 2 starts with a manual deploy; split write-ups + ADRs.
- [Accounts and tools checklist before Sprint 1](tickets/04-setup-checklist.md): tools and accounts ready; the Azure Free Trial ends about 2026-12-03, so upgrade before Sprint 2 (tracked in the Sprint 1 review story); resource providers get registered in the Terraform bootstrap.
- [Split v1 into epics, stories and sprints](tickets/05-split-v1.md): Foundations + E1–E9 as epics; Build → Prove → Fix stories (no Build when naive code exists); CI second in Sprint 1; algorithm tasks are subtasks of Fix stories; a review/re-plan story (unpointed, 1.5 h) ends every sprint; a production drill with a postmortem each sprint from Sprint 2; deploy and monitoring lenses per sprint; a goal per sprint.
- [Facts for the local environment and CI on .NET 10](tickets/02-local-env-and-ci.md): dashboard image `mcr.microsoft.com/aspire/dashboard` (OTLP gRPC 18889); Testcontainers 4.x with xUnit v3 fixtures; setup-dotnet@v6 + global.json + lock files + `dotnet format --verify-no-changes`.
- [Skeleton: what the solution contains on day one](tickets/08-skeleton-day-one-contents.md): bare layers (4 src projects, ArchitectureTests only, `GET /health`); Result, handlers and `IEndpoint` arrive with their first caller.
- [Skeleton: current versions and tool limits for the build and architecture tests](tickets/09-skeleton-tool-facts.md): ArchUnitNET 0.13.5 has an xUnit v3 package; NetArchTest dead since 2021; xunit.v3 on .NET 10 needs the MTP runner in global.json and `--report-xunit-trx`/`--report-trx` instead of `--logger trx`; `.slnx` works with CLI and Rider; Sonar adds 2 errors to the templates; FluentAssertions 8 is paid; latest SDK 10.0.403.
- [Skeleton: architecture-test library and first rules](tickets/10-skeleton-architecture-tests.md): ArchUnitNET (xUnitV3); rules = Domain depends only on itself + `System.*`, Application uses no ASP.NET Core; no layer-direction rule (build rejects cycles); data-access rule deferred to the walking skeleton; red-then-revert commit proves failure.
- [Skeleton: names, solution file, build strictness and test conventions](tickets/11-skeleton-conventions.md): SDK 10.0.403 + MTP runner in global.json; `AnalysisMode=All` + TWAE, no Sonar; minimal .editorconfig (IDE0161, var); `SlotDrop.slnx` with `src/` + `tests/`, `Api` not `Web.Api`; assertion library deferred to the walking skeleton; sentence-style test names.
- [Skeleton: acceptance criteria, docs and estimate](tickets/12-skeleton-acceptance-and-estimate.md): build/format/test green on a clean clone with SDK 10.0.403, `/health` 200; three ADRs (structure, architecture tests, conventions) + README stub; 3 points, the reference estimate.
- [CI: current facts for the workflow, test reporting and rulesets](tickets/13-ci-facts.md): checkout v7.0.1, setup-dotnet v6.1.0 (cache needs `**/packages.lock.json`), dorny v3.2.0 summary mode should parse our TRX (code-read, not run); MTP GitHubActionsReport `--report-gh` exists; ruleset JSON with Actions integration 15368; empty bypass list by default.
- [CI: what protects main, and how we prove it](tickets/15-ci-protect-main.md): ruleset on main (PR, 0 approvals, required `build-test` from Actions, strict, squash only, no force push/deletion); no bypass; turned on while the CI PR is open; created by `gh api`; no red-PR proof (trust the settings).
- [CI: what the workflow runs and how test results show up](tickets/14-ci-workflow-and-reporting.md): one `build-test` job (locked restore, format, Release build, MTP test) on PRs + pushes to main; logs-only reporting; major-tag actions, SHA pinning moved to the Sprint 2 scanning story.
- [CI: acceptance criteria, docs and estimate](tickets/16-ci-acceptance-and-estimate.md): PR and main runs green, merge blocked until green and up to date (checked by reading the ruleset back), squash only; ADR-004 workflow, ADR-005 main protection, README badge; 2 points.
- [Local environment: groomed 2026-11-06](stories/03-local-env.md): dashboard only (Postgres in 04, Redis in E8); API in Compose + Rider; hand-written Dockerfile on `aspnet:10.0.14-noble-chiseled`; anonymous dashboard `13.6.2` on 127.0.0.1; `UseOtlpExporter` with ASP.NET Core, HttpClient, runtime + logs via `AddTelemetry()` in Api; in-memory exporter test in new `SlotDrop.IntegrationTests`; `docker build` in `build-test`; `/health` kept in traces for Sprint 2; env vars for per-environment config; ADR-006 image, ADR-007 local env + telemetry.

## Not yet specified
- Where Result/Error live when the walking skeleton adds them (Application or a new SharedKernel). Sharpens when grooming the walking-skeleton story.
- How the waiting room behaves across several Container Apps replicas (shared state in Redis? token issuance?). Sharpens during slice 4 grooming.
- Where the 4 Vue screens land: so far one booking page in E1 and a waiting-room screen in E9; the other two are unplaced.

## Out of scope
- Outbox pattern, tenant isolation / noisy neighbours: v2.
- Kubernetes: rejected; Azure Container Apps covers the deployment learning goal.
- Customer or tenant accounts, admin UI: v1 uses name+email booking and a key-protected admin API.
- A second Azure environment (staging): cost and effort; one `prod` is enough for v1.
