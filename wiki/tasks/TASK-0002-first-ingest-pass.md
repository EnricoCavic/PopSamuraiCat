---
type: task
status: shipped
started: 2026-08-13
shipped: 2026-08-13
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

Shipped in commit `fc4b271`, together with [[TASK-0001-deploy-brain-os]].

**No design page was written, deliberately** — everything found this pass is a code fact, and what
the game is *for* is unrecorded anywhere in the repo. That gap, the four unreproduced defects, and
the unread areas are carried in [[index]] § *Biggest gaps*, which is where a new session looks.

## Notes

**Hard Rule 6, second half: no agent definition was needed.** No subagent was launched — 42 files
and ~1,630 lines is smaller than the report a fan-out would have returned, so the read stayed in the
main thread and no worker shape repeated.
