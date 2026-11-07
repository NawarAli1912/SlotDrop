# SlotDrop: the problem

What SlotDrop is trying to solve and what success means. It holds no domain model on purpose: the model grows one episode at a time ([LEARNING-PATH.md](LEARNING-PATH.md)), and each step is explained in that episode's write-up in `docs/journey/`. This file moves to `docs/PROBLEM.md` in the repo once the repo exists.

## The problem

### Who and what
Small businesses that sell **time with a person or a room**: barbers, physiotherapists, dental clinics. Their customers book an appointment online, pay nothing up front, and show up.

SlotDrop is one platform that many such businesses share (**multi-tenant**). Each business is a **tenant** with its own staff, services and opening hours.

### Why it is hard
Booking looks like a CRUD app. Two things make it not one.

**Hard problem 1: double booking.** Two customers must never get the same barber at the same time.

The naive code is "check, then insert":

```
1. SELECT: is 10:00–10:30 with Sam free?   → yes
2. INSERT booking for 10:00–10:30 with Sam
```

When two requests run this at the same moment, both see "free" in step 1 and both insert in step 2. Postgres's default isolation level, Read Committed, does not stop this: each statement sees data committed before it started, and neither booking is committed yet when the other checks ([Postgres 17: transaction isolation](https://www.postgresql.org/docs/17/transaction-iso.html)). This is a **race condition** (a check-then-act race), and it only appears under concurrency, so unit tests never catch it.

**Hard problem 2: the slot drop.** Popular businesses release their calendar at a fixed moment ("December opens Sunday 20:00"). For a few minutes, thousands of people hit the same few hundred slots. That is a traffic spike of 100× normal load, aimed at the most contended rows in the database. It is the same shape as concert ticket sales.

Problem 2 makes problem 1 worse: the more concurrent requests, the more often the race fires.

### What success looks like for v1
- **Zero double bookings**, proven by a concurrency test and by a k6 load test that counts overlaps afterwards.
- **The system stays usable during a drop**: it meets a latency and error-rate target (ticket "What load and SLO define 'survives a slot drop'"), and people who can't get in right away get a fair queue instead of errors.
- **Before/after evidence**: each episode measures the naive version breaking, then measures the fix with the same test.

### What v1 is not
No accounts, no payments, no admin UI, no reminders or no-show handling. Customers book with name + email.

## Glossary
Terms are added when an episode introduces them. This is the vocabulary up to E1.

| Term | Meaning | Avoid |
|---|---|---|
| Tenant | A business using SlotDrop | shop, client, org |
| Staff | A person a customer books time with | employee, barber (in code) |
| Slot | A fixed time cell for one staff member, stored as a row in E1 | timeslot |
| Booking | A customer's claim on a slot, with name + email | appointment (in code) |
