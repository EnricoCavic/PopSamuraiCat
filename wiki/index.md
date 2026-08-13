---
type: index
updated: 2026-08-13
---

# Index

**The only catalog.** One row per page, every folder. Updated on **every** operation; a page not
listed here is an orphan and `/brain-os:lint` will flag it. Folder READMEs explain their folder and
do not list its contents.

Read [[SCHEMA]] first if you're an LLM working in this repo. Chronology is in [[log]] — a ledger of
one row per operation, pointing at task files.

**Confidence legend**: 🟢 verified · 🟡 partial · 🔴 stub — **taken from each page's own
frontmatter.** If this column and a page's frontmatter disagree, the frontmatter wins and this page
is stale.

## Systems — what the code is

| Page | Summary | Status |
|---|---|---|
| [[systems/README\|Systems]] | What the folder owns, and why it is deliberately incomplete | — |
| [[player-state-machine]] | The generic machine, the seven player states and their transitions, the three input responses, and four defects found by reading | 🟢 |
| [[player-movement]] | Forces, per-state gravity and drag, the two hardcoded lanes, the box-cast probes, and the tuned values that actually run | 🟢 |
| [[player-input]] | The action map, the jump-only buffer, mouse-aimed dashing, and the movement binding nothing reads | 🟢 |
| [[level-assembly]] | How the endless track spawns one part ahead, its part inventory, and the four things it does not do | 🟢 |
| [[run-loop-and-scoring]] | Two scenes, two fail conditions, a tick-counter score — and the absence of any progression | 🟢 |
| [[game-events]] | The ScriptableObject event bus, the nine cues, and how animation and audio hang off it | 🟢 |

## Design — what the product is meant to be

| Page | Summary | Status |
|---|---|---|
| [[design/README\|Design]] | What the folder owns, and the three outcomes of settling a question | — |

## Planning — how we intend to get there

| Page | Summary | Status |
|---|---|---|
| [[planning/README\|Planning]] | What the folder owns, and why a plan's code claims go stale fastest | — |

## Decisions — why it is that way

| Page | Summary | Status |
|---|---|---|
| [[decisions/README\|Decisions]] | How decision locking works, and why this register's numbers are this project's own | — |

## Tasks — what we did, and when

| Page | Summary | Status |
|---|---|---|
| [[tasks/README\|Tasks]] | Format, lifecycle, and the link-don't-restate rule | — |
| [[TASK-0001-deploy-brain-os]] | Deploying the brain-os method into this repository | — |
| [[TASK-0002-first-ingest-pass]] | First read of the game's own code — the character, the level, the run loop | — |

## Skills — how to do it again

The five pages below arrived with the method and are **vendored** — they describe how to work, not
how this project works, and editing one is drift. Procedures this project writes for itself go in
the same folder and are its own.

| Page | Summary | Status |
|---|---|---|
| [[skills/README\|Skills]] | When a procedure gets written down, and the two unrelated things called "skills" | — |
| [[configuring-a-subagent]] | Writing an agent definition: the description, the tool list, and choosing the model pin | 🟡 |
| [[parallel-coding-fan-out]] | Splitting a specified change into disjoint slices and running coders over them | 🟡 |
| [[mapping-what-you-built]] | Hard Rule 10 in practice — keeping `systems/` true as the code changes | 🟡 |
| [[handing-off-a-session]] | Landing the deliverable and writing a `## State` a cold session can resume from | 🟡 |
| [[verifying-a-milestone-plan]] | Checking a plan's claims before building on it | 🟡 |

This project's own, written here and not vendored:

| Page | Summary | Status |
|---|---|---|
| [[reading-unity-scene-wiring]] | Recovering serialized values, asset references and hierarchy from scene YAML without the Editor | 🟢 |

## Raw sources

Human-owned. **Never edited by the LLM.**

| Page | Summary |
|---|---|
| [[raw/README\|Raw Sources]] | What goes here, how transcription works, and the folder's own index |

## Wiki meta

| Page | What it is |
|---|---|
| [[SCHEMA]] | **The operating manual** — folder ownership, the three operations, hard rules, working practices. **Vendored**: it was deployed from a method repository and a local edit to it is drift. A rule that is wrong here is a finding about the method, raised upstream |
| [[log]] | The chronological **ledger** — one row per operation, pointing at task files |

## Biggest gaps

**The code is mapped; the intent is not.** All 42 files of `Assets/Scripts/` were read on
2026-08-13 ([[TASK-0002-first-ingest-pass]]) and the six `systems/` pages above cover the character,
the level, the run loop and the event bus. `design/`, `planning/`, `decisions/` and `raw/` remain
empty.

1. **Nothing in `design/` — the game's intent is entirely unrecorded, and this is now the largest
   gap by a wide margin.** Reading the code answered *what it does* and left *what it is for*
   completely open. The questions the code raised and cannot settle:
   - **There is no progression of any kind** ([[run-loop-and-scoring]]) — no ramp, no unlocks, no
     persistence, and a score that counts physics ticks rather than distance. Is that a gap or the
     design?
   - `partDifficulty` is authored on every level part and read by nothing
     ([[level-assembly]]) — evidence someone intended a ramp, but not a spec for one.
   - The dash charge curve is a fifth root that never reaches full power
     ([[player-state-machine]]), and the jump falls roughly ten times faster than it rises
     ([[player-movement]]). Both are deliberate-looking numbers with no recorded reasoning.

   Only the human can close this — a file in `wiki/raw/`, or settling the questions in conversation.
2. **Nothing has been observed running.** Every `systems/` page is read-not-run: no compile was
   triggered and no Play Mode session was watched, so all four defects in [[player-state-machine]]
   § *Defects found by reading* and every claim about feel are unconfirmed. Reproducing them is the
   cheapest next verification and needs a human at the Editor ([[SCHEMA]] Hard Rule 3).
3. **Read but not written up**: the unity-bridge package under `Packages/`, the prefab contents
   under `Assets/Prefabs/` (obstacle and part geometry), the Cinemachine camera rig, and the
   `Assets/Editor/` tooling. None of it is mapped, and the run-loop page's account of the trailing
   kill-wall depends on camera behaviour nobody has read.
4. **Keep this section current.** It is the honest edge of the map, and it is the first thing a new
   session reads to know what *not* to trust.
