---
title: "Skeleton: acceptance criteria, docs and estimate"
type: groom
for: 01-solution-skeleton
status: closed
blocked_by: [08-skeleton-day-one-contents, 10-skeleton-architecture-tests, 11-skeleton-conventions]
---

## Question
With contents, rules and conventions settled: what are the story's acceptance criteria and test scenarios, what goes into ADR-001 and the README stub, which story-format sections are N/A for a skeleton (deployment, monitoring), and how many points is it?

Fills: Functional and Technical Acceptance Criteria, Test Scenarios, Testing, Deployment, Required Documentation, and the estimate (1/2/3/5/8).

## Resolution
Groomed with Nawar, 2026-11-05.
- Nominal: on a clean clone with SDK 10.0.403, `dotnet build SlotDrop.slnx` gives 0 warnings / 0 errors, `dotnet format SlotDrop.slnx --verify-no-changes` exits 0, `dotnet test --solution SlotDrop.slnx` passes all architecture tests; `dotnet run --project src/SlotDrop.Api` then `GET /health` → 200, `text/plain` `Healthy`.
- Corner: the red commit fails the Application rule and names the offending type; an older SDK (e.g. 10.0.202) fails fast with the global.json "SDK not found" error.
- External contract change: No (`/health` is new but has no consumers).
- Testing: unit/architecture tests only; component, integration, system: No; UI: N/A. `/health` is checked by hand here; the local-env story's Compose healthcheck exercises it.
- Deployment: backward compatibility No (first PR); logs = default ASP.NET console logs (OTel in the local-env story); field mapping, alerting, dashboards No; healthcheck Yes (liveness `/health`).
- Docs: three ADRs (Context → Options → Decision → Consequences) in `docs/adr/`: ADR-001 solution structure ([day one](08-skeleton-day-one-contents.md)), ADR-002 architecture tests and rules ([rules](10-skeleton-architecture-tests.md)), ADR-003 build and test conventions ([conventions](11-skeleton-conventions.md)); mirrored to the ClickUp Doc "SlotDrop ADRs" on merge. README stub: what SlotDrop is, prerequisite SDK 10.0.403, build/test/run commands. No `docs/journey/` write-up (tooling, not an episode).
- Estimate: **3 points**. First estimated story, so it is the reference for later estimates.
