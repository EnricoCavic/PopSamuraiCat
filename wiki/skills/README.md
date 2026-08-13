---
type: skill
status: stub
confidence: high
sources: []
updated: 2026-08-13
---

# Skills

**This folder owns *how to do it again*.** Repeatable procedures, written down once so they stop
being rediscovered. A skill page is a **how-to**, not knowledge — what a procedure taught us about
the code or the design belongs in [[systems/README|systems]] or [[design/README|design]].

**Never contains** anything done only once. A register of single events is a diary, and it makes the
real procedures harder to find.

**Written at a checkpoint, not on a counter.** [[SCHEMA]] Hard Rule 6: before reporting a task done,
ask whether it repeated a multi-step procedure that has no page here — and if so, write it **in that
same task**, while the details are still recoverable. "Nothing repeated" is a valid answer; not
having asked is not.

**Two unrelated things are called "skills" in a project running this method:**

- **`wiki/skills/`** — this folder. Written procedures, for a human or an LLM to follow, catalogued
  in [[index]].
- **`.claude/skills/`** — the harness's executable commands, including the vendored method at
  `.claude/skills/brain-os/skills/`. Configuration, not knowledge: not pages, not catalogued here,
  and not movable into this folder.

**Five pages in this folder arrived with the method and are vendored.**
[[configuring-a-subagent]], [[parallel-coding-fan-out]], [[handing-off-a-session]],
[[mapping-what-you-built]] and [[verifying-a-milestone-plan]] are method-tier procedures — true in
any project — and [[SCHEMA]] links four of them. **Do not edit them here.** A local edit is reported
as drift the next time the method is synced, and a rule that is wrong in one of them is wrong
everywhere: raise it upstream instead. They are the only pages here anyone but this project writes.

**Everything else in this folder is this project's own**, and starts empty. No procedure has been
repeated here yet.

**Full register: [[index]] § *Skills*.** It is the only catalog — this README explains the folder,
it does not list its contents.
