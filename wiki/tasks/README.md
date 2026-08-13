---
type: task
status: stub
confidence: high
sources: []
updated: 2026-08-13
---

# Tasks

**This folder owns this project's history, and it is the register that matters most.** One file per
task with a deliverable, created when the work **starts** and kept long after it ships. *"What has
been done on this project"* is answered here — not in [[log]], which is a one-row-per-operation
ledger, and not by reading decision pages in sequence.

**Not knowledge.** What a task *taught us* belongs in the wiki layer; what a task *was* belongs
here. Never cite a task file from a `systems/` or `design/` page — pages point into `tasks/`, never
the reverse.

## Format

`TASK-NNNN-short-name.md`, numbered sequentially, **never renumbered**.

```yaml
---
type: task
status: in-progress | shipped
started: YYYY-MM-DD
shipped: YYYY-MM-DD        # once it ships
deliverables: [page-slug, DEC-NNNN-slug]
---
```

Four sections, in this order:

- **`## Goal`** — what this task is for, in a few lines. Enough that someone who was not there knows
  why it happened.
- **`## Deliverables`** — a list of links. Every wiki page the task created or substantively
  rewrote, every decision it locked, the commits.
- **`## State`** — **only while `in-progress`.** This is the handoff note: what is done, what is
  next, what is known to be broken or unverified. **Deleted on ship** — the deliverables speak for
  themselves by then.
- **`## Notes`** — for what genuinely has no other home. Usually empty, and that is the correct
  state: a note explaining *why* belongs in [[decisions/README|decisions]], a note stating a fact
  about the code belongs in [[systems/README|systems]].

## Rules

- **Anything with a deliverable gets one** — a commit, a set of pages, a decision, a resolved design
  question. If nothing shipped, there is no file.
- **Link to deliverables; don't restate them.** The file points at the commit and the pages; it
  never summarises them. Longer than a screen usually means content that belongs in the wiki layer.
- **Every page a task created or substantively rewrote is linked from it.** Parentage is
  inbound-only, so *which task produced this page* can be answered only from the task side — an
  unparented page is a defect in the task file, never in the page.
- **The `## State` test:** could a session that has never seen the conversation continue from it
  alone? Write it as though the context is about to be cleared, because eventually it is.
- **Not a planner.** How work is intended to proceed belongs in [[planning/README|planning]];
  unmapped code belongs in [[index]] § *Biggest gaps*.

**Two tasks recorded.** The register is in [[index]] § *Tasks*.

**Full register: [[index]] § *Tasks*.** It is the only catalog — this README explains the folder, it
does not list its contents.
