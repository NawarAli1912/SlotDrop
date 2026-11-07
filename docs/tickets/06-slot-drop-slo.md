---
title: What load and SLO define "survives a slot drop"
type: groom
for: 18-e2-prove-k6-baseline
status: open
blocked_by: [05-split-v1]
---

## Question
Which traffic profile and pass criteria will the k6 baseline (E2 · Double booking) and the E8/E9 reruns be measured against? Candidate parameters:
- concurrent virtual users when a calendar opens
- ramp shape
- p95 latency
- error rate
- hard rule: zero double bookings

This ticket belongs to [E2 Prove: k6 slot-drop baseline on Azure](../stories/18-e2-prove-k6-baseline.md).

## Resolution
