---
title: Define the problem and the v1 domain model
type: groom
for: epic
status: closed
blocked_by: []
---

## Question
What exactly is SlotDrop solving, and how is the booking domain modelled (what is bookable, how time and availability are represented, how holds and bookings relate, how tenants define opening hours), and why is that model right? The explanation lives in [PROBLEM.md](../PROBLEM.md). The split of slices 1–4 depends on it.

## Resolution
Settled with Nawar, 2026-11-03/04.
- The problem is defined in [PROBLEM.md](../PROBLEM.md): who it's for, the two hard problems (the check-then-insert race, the slot-drop spike), what success means for v1, and what v1 is not.
- **No up-front domain model.** Nawar wants every problem met naive first, noticed with evidence, then fixed (see [LEARNING-PATH.md](../LEARNING-PATH.md)). An up-front model would give away each episode's answer.
- Each episode introduces only the concepts its problem needs and decides how to model them there; see "Domain model by episode" in LEARNING-PATH.md. Nothing about later episodes' models is settled now.
- Write-ups: one per episode in `docs/journey/`. Claude drafts the evidence, options and numbers; Nawar writes the problem and what he learned; each decision gets an ADR.
- Sprint 2 starts with a manual `az` deploy before Terraform.
