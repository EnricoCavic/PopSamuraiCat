---
type: system
status: verified
confidence: high
sources: [Assets/Scripts/LevelManager.cs, Assets/Scripts/ScriptableObjects/LevelPartData/LevelObjects/LevelObject.cs, Assets/Scripts/ScriptableObjects/LevelPartData/LevelParts/LevelPartObject.cs, Assets/Scripts/ScriptableObjects/LevelPartData/, Assets/Scenes/Cena1.unity]
updated: 2026-08-13
---

# Level assembly

The track is endless and built one part ahead of the player. Forty-six lines of `LevelManager` and
two `ScriptableObject` types are the whole system.

## How it works

`LevelManager` sits on its own object in `Assets/Scenes/Cena1.unity` and keeps a cursor — its own
`transform.position` — at the end of the last part it placed. `Start()` moves the cursor to
`startingPosition` and spawns one part; `Update()` spawns another whenever the player comes within
`spawnDistance` of the last part's end (`LevelManager.cs:14-26`).

`SpawnPart()` picks uniformly at random from `level.parts`, instantiates the part's prefab at the
cursor, and advances the cursor to that instance's `endPosition` (`LevelManager.cs:28-44`).

Scene values: `startingPosition (68, 0, 0)`, `spawnDistance 50`, level asset `FullLevel`.

A part's extent is authored, not measured: `LevelPartObject.endPosition` is
`prefabInstance.transform.position + (width, 0, 0)` (`LevelPartObject.cs:13`). **`width` is a number
someone typed**, so a prefab whose geometry disagrees with it leaves a gap or an overlap and nothing
detects that.

## The two asset types

- **`LevelObject`** — a list of parts plus `allowRepeatingParts` (`LevelObject.cs:8-9`). Two exist:
  `FullLevel` (6 parts, repeats **not** allowed) and `Test` (1 part, repeats allowed).
- **`LevelPartObject`** — prefab, `width`, `partDifficulty`, and a `[NonSerialized] prefabInstance`
  (`LevelPartObject.cs:9-13`).

`FullLevel` — the one the scene uses — contains `Part1-1` (width 132), `Part2-1` (70), `Part3-1`
(28), `PartGate` (28), `Part4-1` (32), `Part5-1` (68). `Test` is referenced by nothing.

**Seven further part assets are referenced by no `LevelObject` and no scene**: `Big1`, `Big2`
(width 135), `Medium1`–`Medium3` (65–68), `Small1`, `Small2` (22–28). Their prefabs live under
`Assets/Prefabs/Lanes/`, a different folder from the `Assets/Prefabs/Parts/` prefabs `FullLevel`
uses — which suggests two generations of level authoring, the older one abandoned. **That reading is
an inference from the folder split, not something the code says.**

## What this system does not do

Four absences, all checked rather than assumed:

1. **Nothing is ever destroyed.** There is no `Destroy` call anywhere in `Assets/Scripts/`
   (`GameObject.Destroy`, not the `OnDestroy` callback). Every part instantiated in a run stays
   in the scene for the whole run.
2. **`partDifficulty` is never read.** The field is declared (`LevelPartObject.cs:12`) and is `0` on
   all thirteen assets. Part selection is uniform random with no weighting, ordering or ramp — see
   [[run-loop-and-scoring]] for the wider point that this game has no progression at all.
3. **No seeding.** `Random.Range` with Unity's shared generator (`LevelManager.cs:33`); runs are not
   reproducible.
4. **No lane awareness.** The manager places parts along x only. Lanes are entirely the player's
   business ([[player-movement]]).

## Two traps

**`prefabInstance` is scene state stored on a shared asset** (`LevelPartObject.cs:10`). Each spawn
overwrites it, so `endPosition` always describes the *most recent* instance of that part. It works
today only because `LevelManager` reads it immediately after spawning, through `lastLevelPart`.
Being `[NonSerialized]`, it is also null until the first spawn — `Update()` would throw on
`lastLevelPart.endPosition` if it ran before `Start()`, which the Unity callback order prevents.

**The repeat-avoidance retry is unbounded recursion.** When the pick matches the previous part and
repeats are disallowed, `SpawnPart()` calls itself (`LevelManager.cs:35-39`). A `LevelObject` with
one part and `allowRepeatingParts: false` recurses until the stack overflows, and a two-part level
can retry an unbounded number of times. `FullLevel`'s six parts make it fine today and `Test` sets
`allowRepeatingParts: true`, so **neither existing asset triggers it** — it is a trap for whoever
authors the next `LevelObject`.

## Not verified

Read from source and from the asset YAML. No level was spawned or observed; nothing was compiled or
played during this pass.
