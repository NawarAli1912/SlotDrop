---
title: Accounts and tools checklist before Sprint 1
type: task
for: epic
status: closed
blocked_by: []
---

## Question
Get Nawar's machine and accounts ready. This checklist is for Nawar:
- [x] Install Terraform: `brew tap hashicorp/tap && brew install hashicorp/tap/terraform` (only Sprint 2 needs it)
- [x] Start Docker Desktop: Compose and Testcontainers need it in Sprint 1
- [x] Install k6: `brew install k6` (Sprint 4)
- [x] Azure: find when the Free Trial ends (portal → Subscriptions → the free-services grid tooltip) and upgrade to pay-as-you-go before then
- [x] Run `az login` then `az account show --query "{name:name, offer:offerId?, state:state}"` and paste the output. This tells us whether it's a free account and how much credit remains.
- [x] ClickUp: turn on the Sprints and Sprint Points ClickApps for the Default space (Settings → ClickApps)
- [x] GitHub: confirm the repo name `SlotDrop` is free on your account

## Resolution
Checked by Claude on 2026-11-05 with read-only commands:
- .NET SDK 10.0.202 is installed (`dotnet --list-sdks`); it matches the `global.json` pin in ticket 02.
- `gh` is logged in as `NawarAli1912`. `gh api repos/NawarAli1912/SlotDrop` returns 404, so the name is free.
- `az` is logged in. The subscription is "Azure subscription 1", Enabled, with `quotaId` `FreeTrial_2014-09-01` and spending limit On (from `az rest` GET on the subscription).
  - Free Trial facts: the $200 credit lasts 30 days. When it expires, the subscription and its services are disabled unless upgraded to pay-as-you-go. After upgrading, the 12-month free services continue. Source: https://learn.microsoft.com/azure/cost-management-billing/manage/avoid-charges-free-account
  - So Sprint 2 needs the upgrade done if the trial ends before or during it. Upgrading needs a payment method, so Nawar does it himself.
- Resource providers `Microsoft.App`, `Microsoft.DBforPostgreSQL` and `Microsoft.OperationalInsights` are NotRegistered. This is added to the Terraform bootstrap story.
- Terraform 1.17.1 installed by Nawar on 2026-11-05 (`terraform version`). k6 2.3.0 installed by Nawar on 2026-11-05. Docker daemon running, server 29.5.3 (rechecked 2026-11-05).
- Not checkable from here: the ClickUp ClickApps.

Nawar's update, 2026-11-05: ClickUp Sprints and Sprint Points ClickApps are on. The Free Trial ends in 28 days, about **2026-12-03**.
- If Sprint 1 starts about 2026-11-05 (an assumption), Sprint 2 runs to about 2026-12-03, so the trial ends on the last day of the Azure sprint.
- Upgrading early keeps the remaining credit until day 30 (same Microsoft Learn source), so there is no reason to wait.
- **Upgrading to pay-as-you-go is not done yet.** It is now a scope item of "Process: Sprint 1 review, re-split, re-plan and re-estimate": Sprint 2 doesn't start until it's done.
