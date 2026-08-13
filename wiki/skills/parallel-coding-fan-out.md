---
type: skill
status: partial
confidence: medium
sources: []
updated: 2026-08-12
---

# Parallel coding fan-out

*Vendored from the method — edit it upstream, not here.*

**When to use.** A piece of work is specified and ready to build, and it divides into slices that do
not share files. The workers are defined coder agents — writing code in the main thread is what fills
a context fastest, which is why a specified change leaves it.

## Slice on files, never on features

A feature is a **set of files**, and two features routinely touch the same one — a shared entry
point, a shared configuration list. So the split is decided by listing files first and grouping
second.

1. **List every file the work touches**, per piece of the plan.
2. **Any file appearing in two pieces collapses them into one slice.** Do not hand the same file to
   two agents and reconcile afterwards; there is no merge step, and the later write wins silently.
3. **Pull every edit to the project's single-writer files out into a main-thread list** — the files a
   project has designated as touchable by only one agent at a time ([[SCHEMA]] § *Delegation*). These
   are done here, serially.
4. **What is left is the fan-out.** If it is one slice, do it in the main thread — the spec costs more
   than the edit.

## The serial-then-parallel-then-serial shape

```
main thread          fan-out                 main thread
────────────────    ───────────────────     ─────────────────────────
register/prepare →  coder agents × N     →   wire up, build, verify
shared state          (bodies only)          (verification is not delegated)
```

Whatever the fan-out depends on must exist **before** the coders run — an agent cannot fill in a file
whose containing structure hasn't been created yet. And nothing an agent returns is evidence the code
works: building and verifying happen here, in the main thread, afterward.

## What a launch message must carry

An agent cannot see this conversation. Each launch states:

- **The exact file list** — its slice, and the instruction that everything else is another agent's.
- **The pattern to copy, with `file:line`.** Not "follow the house style" — the specific working
  example. This is the single biggest quality lever.
- **The behaviour**, at the level of detail the plan settled it. Anything left open comes back as a
  stated assumption, which is cheap to overrule — as long as the agent was told to state it.
- **What it must not touch**, restated: the single-writer files, anything outside the slice.
- **The cross-slice API, fixed here before anyone launches** — every function one slice calls from
  another, with its exact signature and return shape. Agents cannot negotiate; an unfixed seam becomes
  two incompatible guesses.
- **The shared-namespace split, per slice.** Learned the hard way, below.

Then: **state the count, the agent and the split in chat, and wait.** How many run is settled at that
go-ahead and nowhere else.

## On return

- **Read the reports before building anything on them.** A `file:line` from an agent is a claim, and
  it is read against source when a decision rests on it.
- **Do the "needs the main thread" list** each report ends with — that is where the shared-state work
  the slice implied comes back.
- **Then build and run.** A clean run is the first evidence in the whole loop.
- **Collapse the reports.** They are context; once acted on, what survives is the task file and the
  systems page, not the agent prose.

## Two lessons a first real run is likely to teach

**1. Slicing on files is not enough — the namespace is shared too.** Two agents writing constants or
identifiers in separate files cannot see each other, and nothing warns them. Two slices minting names
in the same shared namespace without colliding is luck, not the procedure working. Fix the naming
split per slice in the launch message, the same way the cross-slice API is fixed.

**2. Reading the returned slices against source finds a different class of defect than running them
does.** The read is still owed wherever a decision rests on an agent's claim, but it does not catch
integration defects — a scoping error that only throws on first execution, a slice with no execution
path exercised at all, a rendering bug only a live frame shows. **A fan-out that ends at the read has
not been checked.** Budget the build-and-run as part of the fan-out, not as a follow-up — and ask
which slices whatever verification exists actually exercises, since a slice only the presentation
layer touches can pass a model-level check while never having executed at all.

## Related

[[configuring-a-subagent]] · [[handing-off-a-session]] · [[mapping-what-you-built]]
