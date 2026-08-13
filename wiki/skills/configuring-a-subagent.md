---
type: skill
status: partial
confidence: medium
sources: []
updated: 2026-08-12
---

# Configuring a subagent

*Vendored from the method — edit it upstream, not here.*

**When to use.** A task is closing and it repeated a worker shape that has no definition — same kind
of worker, same instructions, same tool needs. Seeing the shape more than once or twice means a
definition is overdue, not that one may now be considered. This page is the how. Also use it when
reviewing an existing definition after a slice misfired.

Nothing here is project knowledge — it is harness mechanics, and should be read alongside whatever
agent-configuration reference the deploying project's harness ships.

## Two things are called "skills" — they are unrelated

The word collides in this method: a **written procedure** — this page is one — and an **executable
command** the harness loads by path at session start. They have different owners, different readers
and different catalogs, and only the first belongs in the wiki layer. Say this once per deployment,
not on every skill page.

## Steps

1. **Name the shape in one sentence** — what it reads, what it returns, what it must not touch. If
   that sentence needs an "and", it is two agents.
2. **Create the agent definition.** `name` and `description` are the fields that decide it exists at
   all and gets selected; `description` is what the main thread matches against when choosing a
   worker, so write it for selection, not for documentation.
3. **Pin `model:`.** Omitting it means the definition inherits the main thread's model, so a planning
   session on the strongest tier silently staffs a fleet of workers on that same, most expensive
   tier. Which model: § *Choosing the model* below.
4. **List tools narrowly.** An omitted tool list inherits everything a full session can do. A
   read-only agent gets read and search tools and then *cannot* edit — a guardrail the system prompt
   can only ask for, not enforce, unless the tool list backs it. A tool that can write files or run
   shell commands re-opens writing even on an agent meant to be read-only; say so explicitly in the
   body if it's granted for a narrow reason such as counting.
5. **Write the body as the system prompt.** The agent gets this plus the project's own top-level
   instructions, repository state and working directory — not the main thread's system prompt, and
   not the conversation. Everything it needs in order not to invent must be in the body or the
   delegation message.
6. **Give it a return contract and an escalation clause.** Bound the shape and length of the report —
   the report is part of the cost of delegating — and say what to do when the slice turns out to need
   more judgement than the agent's tier can give: stop and say so, never guess.

## Choosing the model

**This is a procedure, not a table of assignments.** A table of *slice → tier* is the same fact in a
second place, and it drifts from the definitions it is meant to summarize within days of being
written. Re-run the questions below for each definition rather than copying a neighbour's pin:

1. **Does the slice need judgement at all?** A list, a count, or a set of quoted lines is mechanical
   and goes to the cheapest tier that extracts reliably. Applying a change the main thread has already
   decided doesn't need the deepest tier either — it is a step up from mechanical, not the top.
2. **How many of these run at once?** Tier cost multiplies by fan-out width, so an agent launched
   several at a time over disjoint slices should sit a tier below one that runs alone, reads broadly,
   and returns a judgement.
3. **Does the slice depend on current external knowledge, or only on reading this repository?** Model
   cutoffs differ from model capability. For an agent producing output against a live external
   toolchain, prefer the newer cutoff. For one that reads the repository and cites `file:line`, cutoff
   doesn't matter and capability wins.

Then set the effort/depth parameter where the harness offers one — pin the model to the *kind* of
judgement, set effort for its *depth*; this is often the better lever than moving to a different model
tier.

**A tier is not free to raise.** A mechanical agent promoted too far burns budget on every launch, and
a judgement agent pinned too low returns a confidently thin report every time. Both are worth one more
pass over this section before the definition ships.

**Re-pinning an existing agent may mean rewriting its body.** Instructions written for one model can
be too prescriptive for a more capable one and reduce its output quality. If a definition enumerates
steps and is moving up a tier, restate those steps as the goal and the constraints, and let the agent
choose the method — a pin change alone can make an agent worse than it was.

## What a definition does not authorize

**Defining an agent makes it available; it does not pre-authorize using it.** State the count, the
agent and the split, and wait for a go-ahead before every launch, however many times the shape has
run before.

## Gotchas

- **A definition you just wrote may not be usable this session.** Whether a new agent definition or a
  new executable command becomes available mid-session or only after a restart has not been consistent
  in practice. Plan `.claude/`-equivalent work to land next session and say so in the report, but
  check whether it is actually listed before assuming either way.
- **Model resolution has sources above the definition file.** An environment-level override and a
  per-invocation override both take precedence over frontmatter. A pin can be overridden on purpose;
  it should never be forgotten by accident.
- **A project-level definition overrides a built-in of the same name.** Treat that as a deliberate
  renaming tool, not an accident to guard against.
- **An effort/depth parameter may not be supported on every model a tier offers.** A rejected
  combination is a failed launch, not a thin report — check it on first use rather than assume it from
  a table, and record the exception on the definition itself if one is found.
- **Agents may not nest indefinitely.** Depth is bounded, and a delegated agent can find the
  delegation tool withheld, at which point it does the work itself instead of splitting further. Don't
  design a fan-out that depends on knowing exactly where the limit is.
- **No second register.** The harness lists the available agent definitions itself every session, so
  the wiki's own catalog deliberately does not duplicate them — a second register would drift.

## Verification

There is no automated check that a definition is *good*. What can be checked:

- The file loads — the agent appears in the session's available worker types. Check this before
  relying on it; a definition that never appears has failed silently as far as the conversation is
  concerned.
- The first real slice returns in the contracted shape and length. If the report needs reformatting in
  the main thread, the contract is wrong, not the agent.
- A delegated `file:line` behind a decision the main thread is about to build on is still read against
  source before it's built on, whatever tier produced it. A cheaper tier does not lower that bar.

## Related

[[parallel-coding-fan-out]] · [[handing-off-a-session]] · [[verifying-a-milestone-plan]]
