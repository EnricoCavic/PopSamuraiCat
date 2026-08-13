---
type: skill
status: partial
confidence: medium
sources: []
updated: 2026-08-12
---

# Verifying a milestone plan before building it

*Vendored from the method — edit it upstream, not here.*

**When to use.** A plan's cost table rests on claims about existing code made during planning, by
reading, rather than at build time. Any milestone whose costing leans on such claims needs this
before the build starts.

**Why it exists.** A plan built from a broad read early on carries load-bearing assumptions about
what already exists — persistence, an existing interface, a defect's severity — the exact kind of
claim a future session will otherwise trust without checking. Checking costs one fan-out; discovering
a wrong assumption mid-build costs the build.

## The five moves

1. **Pull the load-bearing claims out of the milestone's own table.** Not every row — the ones
   phrased as facts about existing code ("already handles X", "already built", "passes Y through
   untouched"). A row that only describes work to do has nothing to verify. A handful is the normal
   count; many more than that means the milestone is too big.
2. **One slice per claim, a claim-verification agent, disjoint.** State the count, agent and split and
   wait for a go-ahead. Give each agent the claim as a quoted sentence and ask it to confirm or
   refute — not "read this area". A verification slice has an answer; an exploration slice does not,
   and mixing them wastes the fan-out.

   If the agent normally reserved for this work is pinned to a tier priced for a single instance, and
   this is the one place it runs several at once, name an override at the go-ahead — tier cost
   multiplies by fan-out width, and that cost-shape rule applies here as it does anywhere else
   ([[configuring-a-subagent]] § *Choosing the model*).
3. **Bound the return.** A verdict line, `file:line` on every part checked, and a line for what could
   not be confirmed; add numbered points matching the sub-questions asked. That last line is the
   point — an agent that cannot say what it failed to establish sends the next session back to the
   same files.
4. **Re-read the load-bearing lines in the main thread.** A delegated `file:line` behind a decision
   the main thread is about to build on gets read against source at the point the decision is made.
   Costs one read per claim. This is where refinements surface that a confirming agent glossed over —
   an agent can report CONFIRMED and still miss the detail that changes how the work should be done.
5. **Route the results to the folder that owns each.** Code facts to `systems/`, cost changes to the
   plan itself, the task's own record to its task file. Correct the plan in place — a verified row
   that still reads as unverified invites the same fan-out next session.

## What a good outcome looks like

**Mostly confirmations, plus a couple of refinements.** That is not a wasted pass — the refinements
are the yield, and they are usually not refutations. A typical result: every claim holds, but *how* to
do the work changes (a required declaration site nobody had read closely), *how much it costs*
changes (a defect's surcharge drops once the actual mechanism is read), and *what to copy* changes (a
named working pattern replaces a vague "match the house style").

**Expect side findings and file them.** Agents reading a slice properly find things nobody asked
about. These belong on the owning `systems/` page, not in the milestone's task file.

## Gotchas

- **A confirming report is not corroboration.** One agent confirming is one opinion; a second agent
  asked the same question is the same opinion twice. What raises confidence is move 4 — reading the
  source — not another agent.
- **Do not let the pass become a rewrite of the plan** — but **do correct what it disproves.**
  Re-ordering the ladder or re-opening a settled decision needs a human. **A row the pass shows is
  understated is different: review it and write the corrected version in place, in the same task.**
  Flagging it and moving on leaves a number the next session will build against, knowing it is wrong.
  Say what changed and why in the row itself, and carry the delta to the plan total.
- **A refuted claim is a stop, not a patch.** If a persistence or reuse assumption fails, the
  milestone's cost table is wrong, and that is a finding to report before building, not to work around
  silently.
- **Nothing in this procedure runs anything.** It raises confidence in claims about code; it says
  nothing about behaviour. The milestone still ends in a human check of the running result.

## Related

[[configuring-a-subagent]] · [[parallel-coding-fan-out]] · [[mapping-what-you-built]]
