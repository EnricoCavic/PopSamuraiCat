---
type: system
status: verified
confidence: high
sources: [Assets/Scripts/GameOver.cs, Assets/Scripts/Components/SceneChanger.cs, Assets/Scripts/Components/ScoreSystem.cs, Assets/Scripts/Components/UI/FloatReaderUI.cs, Assets/Scripts/Components/UI/HighscoreDisplay.cs, Assets/Scripts/ScriptableObjects/FloatObjects/FloatObject.cs, Assets/Scripts/MoveCam.cs, Assets/Scripts/TextActivationControl.cs, Assets/Scripts/Components/UI/QuitComponent.cs, Assets/Scripts/Components/ParallaxBackground.cs, Assets/Scenes/Cena1.unity, Assets/Scenes/Menu.unity]
updated: 2026-08-13
---

# Run loop and scoring

How a run starts, how it ends, and what it leaves behind. **There is no progression system** — this
page is partly a record of what is absent, checked rather than assumed.

## The loop is two scenes

`Menu` → `Cena1` → `Menu`, with `SceneManager.LoadScene` by name and no additive loading, no
persistence object and no `DontDestroyOnLoad` anywhere (`SceneChanger.cs:10-13`). Three
`SceneChanger` components carry the whole flow: one in `Menu` targets `Cena1`, and two in `Cena1`
target `Menu`.

`Menu` is static UI over a parallax backdrop: `MoveCam` drifts a camera at a constant
`velocidade` (`MoveCam.cs:14-17`) to keep the parallax below moving, `TextActivationControl`
toggles a controls panel, and `QuitComponent` exits.

## A run ends in one of two ways

1. **Falling out of the world.** `PlayerStateMachineBusiness.Update()` checks `position.y` against
   `deathZone` every frame and loads the menu (`PlayerStateMachineBusiness.cs:36-37`). The scene
   sets `deathZone: -10`; the field's C# default of `-5` is not what runs.
2. **Being caught by the trailing wall.** `GameOverTrigger` is a box trigger parented to the
   Cinemachine virtual camera in `Cena1`, so it tracks the camera rather than the player. On
   `OnTriggerEnter` with a `Player`-tagged collider it loads the menu (`GameOver.cs:40-45`).

`GameOver` does two non-obvious things. It re-centres its collider **every `LateUpdate`** so the
box sits at world z ≈ 0 regardless of the camera's own z (`GameOver.cs:34-38`) — the camera moves in
z as the player changes lane, and this cancels that out so the wall spans both lanes. And it holds a
**one-second grace period** after scene load before it can fire at all (`GameOver.cs:21-32`),
because the camera starts damped behind the player and would otherwise catch them at spawn.

There is no other fail state. **Obstacles do not kill.** Nothing in `Assets/Scripts/` handles a
collision with the obstacle prefabs; hitting one blocks forward movement via the `RightWallCheck`
probe ([[player-movement]]), which drops the player back to `idle`, and it is the trailing wall that
ends the run.

## Score is a physics-tick counter

`ScoreSystem` zeroes the score in `Awake` and adds `1` every `FixedUpdate` (`ScoreSystem.cs:9-17`).
So the score is **elapsed physics time**, not distance and not obstacles cleared — a player standing
still against a wall scores exactly as fast as one running. `FloatReaderUI` writes it to a `Text`
every `FixedUpdate` (`FloatReaderUI.cs:17-20`).

It is stored in a `FloatObject` — a `ScriptableObject` holding a single float
(`FloatObject.cs:8-12`) — which is how the value crosses the scene boundary at all. `Score.asset` is
the only one wired up. Its `unserializedValue` field has no readers anywhere.

## The "highscore" does not persist

`HighscoreDisplay` in `Menu` reads `Score.asset` as `previousScore`, compares it against a
**`private static float`**, and shows the larger (`HighscoreDisplay.cs:8-19`). A static survives
scene loads, so the best score of the current *session* is shown correctly — and it is gone when the
process exits. **There is no `PlayerPrefs` call, no save file and no serialisation of the high score
anywhere in `Assets/Scripts/`.**

`Highscore.asset` exists, is a `FloatObject`, holds `0`, and **is referenced by nothing** — not by a
scene, a prefab or another asset. Presumably the intended home for a persisted best; it was never
wired.

One editor-only wrinkle: writes to a `ScriptableObject` persist between Play sessions **in the
Editor** but reset to the serialised value on every launch of a build. `Score.asset` is committed
with `value: 0`, so this affects what a developer sees in the Editor, not a player.

## No progression, checked

The human asked for progression as one of the main systems. It does not exist. What is absent, each
verified by grep rather than assumed:

- **No difficulty ramp.** `partDifficulty` is authored on every level part and read by no code
  ([[level-assembly]]).
- **No speed ramp.** `movementSpeed` comes from the tuning asset and is never modified at runtime;
  `xAxis` is fixed at `1` ([[player-movement]]).
- **No unlocks, currency, upgrades or run modifiers.** No script mentions any.
- **No persistence of any kind.** No `PlayerPrefs`, no file I/O.

A run is therefore uniform from the first second to the last: the same random draw over the same six
level parts, at the same speed, scoring at the same rate. **Whether that is intended is a design
question, and nothing in this repository answers it** — `wiki/design/` is empty.

## Parallax

`ParallaxBackground` moves a sprite by a fraction of the camera's frame delta and wraps it once the
camera has travelled a full texture width (`ParallaxBackground.cs:21-32`), with the texture width
computed from the sprite's pixels-per-unit and lossy scale. Nine instances in `Cena1`, seven in
`Menu` — it is the only reason `Menu` needs a moving camera.

## Not verified

All of the above is read from source, scene YAML and asset YAML. **Nothing was compiled and no run
was played**, so the fail conditions and the grace period have not been observed working.
