# SlotDrop

A learning project: a multi-tenant booking platform whose hard problems are double booking under concurrency and slot-drop traffic spikes. The goal is the lesson, not just the feature.

## Where we are and where we're heading
Read before planning or grooming any work:
- `docs/MAP.md`: sprint order, each sprint's goal, every story with its state (split / grooming / ready / done)
- `docs/LEARNING-PATH.md`: the learning target per sprint and the episode for each problem
- `docs/PROBLEM.md`: the problem statement
- `docs/stories/NN-*.md`: one file per story; `docs/tickets/`: the facts gathered while grooming
- `docs/adr/`: decisions with their context and trade-offs; check here before reopening a decision

## How the work runs
- **Naive first.** Each problem is an episode: build the naive version, make the problem visible with evidence (a failing test, a k6 number, a trace), compare options, fix, measure again. Keep end-state designs out of a story until its episode earns them.
- Nawar writes the algorithmic code from a LeetCode-style task (problem, C# signature, constraints, examples, failing xUnit tests). Agents write the rest, open with a trade-off brief, and explain the *why* behind each choice.
- One story = one branch = one PR, squash-merged to a protected `main` after Nawar approves.
- A decision a story makes gets an ADR in `docs/adr/` (Context → Options → Decision → Consequences) in the same PR.
