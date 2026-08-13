---
type: task
status: in-progress
started: 2026-08-13
deliverables: [player-state-machine, player-movement, player-input, level-assembly, run-loop-and-scoring, game-events, reading-unity-scene-wiring]
---

# TASK-0002 — First ingest pass over the game's main systems

## Goal

Map the game itself. [[index]] § *Biggest gaps* opened with *"nothing about the game has been mapped
yet"*, and every session since the deploy would have started by re-reading the same code cold. This
pass reads the whole of `Assets/Scripts/` — 42 files, ~1,630 lines — plus both scenes, the
ScriptableObject assets and the input bindings, and writes `systems/` pages for the character, the
level, the run loop and the event bus.

Scope was the human's: *the main systems like the character, level, progression*. **Progression is
the one that came back empty** — see [[run-loop-and-scoring]].

## Deliverables

Six `systems/` pages, all written from reading the source:

- [[player-state-machine]] — the generic machine and the seven player states
- [[player-movement]] — forces, gravity, drag, ground/wall probes, and the tuned values
- [[player-input]] — the action map, the jump buffer, and the two unread actions
- [[level-assembly]] — how the endless track is spawned, and its part inventory
- [[run-loop-and-scoring]] — scene flow, the two death conditions, score and "highscore"
- [[game-events]] — the ScriptableObject event bus and what listens on it

One `skills/` page, from the [[SCHEMA]] Hard Rule 6 checkpoint:

- [[reading-unity-scene-wiring]] — recovering serialized values, asset references, hierarchy and
  `UnityEvent` targets from scene YAML. Written because this pass did it repeatedly and every
  future ingest of a scene-wired system will need it again.

Also updated: [[index]] (seven new rows, rewritten *Biggest gaps*), [[log]], and the "empty" lines
in the `systems/` and `tasks/` READMEs.

## State

**The pages are written and the registers updated; nothing is committed yet.**

**No design page was written, deliberately.** Everything found this pass is a code fact. What the
runner is *meant* to feel like, whether score should be distance rather than time, and whether
difficulty is supposed to ramp are unrecorded anywhere in the repo, and inventing them would be
[[SCHEMA]] Hard Rule 2's failure. `design/` stays empty until the human puts something in
`wiki/raw/` or settles it in conversation.

**Nothing was compiled or run.** The Unity Editor was not driven during this pass, so no claim here
rests on observed behaviour — the pages say so where it matters. The defects listed in them were
found by reading and **none has been reproduced in Play Mode**; that is the obvious next step and it
needs a human at the Editor.

**Open for the human**, carried in the pages rather than here: the four read-only defects in
[[player-state-machine]] § *Defects found by reading*, and whether the seven unreferenced level-part
assets in [[level-assembly]] are abandoned or staged for later.

## Notes

**Hard Rule 6, second half: no agent definition was needed.** No subagent was launched — 42 files
and ~1,630 lines is smaller than the report a fan-out would have returned, so the read stayed in the
main thread and no worker shape repeated.
