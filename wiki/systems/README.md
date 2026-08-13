---
type: system
status: stub
confidence: high
sources: []
updated: 2026-08-13
---

# Systems

**This folder owns *what the code is*.** One page per system or subsystem: what the code actually
does, what it gets wrong, and what has not been read yet — every non-obvious claim carrying a
`path/to/file.ext:LINE` citation. It grows when an unmapped area gets read.

**Never contains** design intent, why anything was chosen, or an account of work done. Those belong
to [[design/README|design]], [[decisions/README|decisions]] and [[tasks/README|tasks]] — see
[[SCHEMA]] § *Single source of truth*.

**Deliberately incomplete, and expected to stay that way for a while.** This is a map under
construction, not a specification: most of any codebase is unread at first. [[index]] § *Biggest
gaps* is its honest edge. What is never acceptable is a page asserting something nobody read
([[SCHEMA]] Hard Rule 2) — the code is ground truth, and when a page and the code disagree, the page
is stale.

**Six pages, covering the player, the level and the run loop.** All of `Assets/Scripts/` has been
read once; none of it has been observed running.

**Full register: [[index]] § *Systems*.** It is the only catalog — this README explains the folder,
it does not list its contents.
