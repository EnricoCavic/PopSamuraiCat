---
type: system
status: verified
confidence: high
sources: [Assets/Scripts/ScriptableObjects/EventSystem/GameEventObject.cs, Assets/Scripts/Components/GameEventListener.cs, Assets/Scripts/Components/AnimatorController.cs, Assets/Scripts/AudioManager/AudioManager.cs, Assets/Scripts/AudioManager/SoundClass.cs, Assets/Scripts/ScriptableObjects/EventSystem/, Assets/Scenes/Cena1.unity]
updated: 2026-08-13
---

# Game events, animation and audio

The player states never call the animator or the audio manager directly. They raise a
`ScriptableObject` event and something in the scene answers — the standard Unity SO-event pattern,
implemented in two files totalling 45 lines.

## The bus

`GameEventObject` is a `ScriptableObject` with a `tag` string and a runtime list of listeners.
`Raise()` walks that list **backwards** so a listener unregistering during dispatch cannot skip its
neighbour (`GameEventObject.cs:11-15`). `GameEventListener` registers on `OnEnable`, unregisters on
`OnDisable`, and invokes a `UnityEvent` when raised (`GameEventListener.cs:11-13`) — so the wiring
from event to effect is done in the Inspector, not in code.

**A state finds its event by scanning a list and matching `tag` at construction time.** Every player
state that raises a cue loops over `business.eventList` in its constructor and keeps the matching
reference — for example `JumpingPlayerState.cs:15-27`. The list is serialised on the player object
in `Assets/Scenes/Cena1.unity` and holds all nine event assets.

Two costs come with that. **A missing or misspelled tag yields a null field, and the raise sites
mostly do not null-check** — `JumpingPlayerState.cs:34` and `DashingPlayerState.cs:34` call
`eventObject.Raise()` unguarded, so an event dropped from the scene list is a
`NullReferenceException` at the first jump rather than a silent no-op. `ChargingDashPlayerState`
uses `?.` on entry (`:41-42`) but not in `StateTimeout` (`:81`). And **the tag is a magic string
duplicated** between the asset and the state that looks for it.

## The nine events and what answers them

All nine assets live under `Assets/Scripts/ScriptableObjects/EventSystem/`. Listener wiring read
from `Assets/Scenes/Cena1.unity`:

| Event asset | `tag` | Raised by | Listener response |
|---|---|---|---|
| `RunAnimation` | `RunAnimation` | `running` on enter | `AnimatorController.PlayAnimation("Running")` |
| `JumpAnimation` | `JumpAnimation` | `jumping` and `airborne` on enter | `PlayAnimation("Jumping")` |
| `ChargeAnimation` | `ChargeAnimation` | `chargingDash` on enter | `PlayAnimation("Charging")` |
| `DashAnimation` | `DashAnimation` | `dashing` on enter | `PlayAnimation("Dashing")` |
| `JumpEvent` | `Jump` | `jumping` on enter | `AudioManager.Play("Jump")` |
| `ChargeEvent` | `Charge` | `chargingDash` on enter | `Play("Charge")` |
| `DashEvent` | `Dash` | `dashing` on enter | `Play("Dash")` **and** `Stop("Charge")` |
| `DashFailEvent` | `DashFail` | `chargingDash` timeout | `Play("DashFail")` **and** `Stop("Charge")` |
| `CooldownEvent` | `Cooldown` | dash cooldown elapsed (`PlayerDashResponse.cs:71`) | `Play("Cooldown")` |

The charge loop is stopped by whichever event ends the charge, which is why `DashEvent` and
`DashFailEvent` each carry two calls. `airborne` reuses the jump animation rather than having its
own.

Note that **the animation cues are a superset of the state list only for the states that have art**:
`idle` and `changingLane` raise nothing, so the animator keeps playing whatever the previous state
started.

## Animator and audio

`AnimatorController` is a one-method wrapper calling `Animator.Play(name)` by string
(`AnimatorController.cs:14-17`) — direct play, no parameters and no transition graph, so cue order
is the only thing sequencing the animation. This is what makes defect 4 in [[player-state-machine]]
matter: two `Play` calls in the same frame, and the last one wins.

`AudioManager` builds one `AudioSource` per `SoundClass` entry on `Awake`, copying clip, volume and
pitch, then plays or stops by name with `Array.Find` (`AudioManager.cs:10-32`). A name that matches
nothing returns null and throws on the next line — there is no guard. **Only those three fields are
copied**: `loop` and `playOnAwake` are left at the defaults of a runtime-added `AudioSource`, so
nothing here loops. The charge cue is `Stop()`ped by both `DashEvent` and `DashFailEvent` as though
it were a sustained sound, but the source that plays it is one-shot.

One `AudioManager` instance, in `Cena1`, carrying five cues: `Jump`, `Dash`, `Charge`, `Cooldown`
and `DashFail`. **Music does not go through it** — both tracks in `Assets/Audio/` are placed as
ordinary `AudioSource` components directly in the scenes, `PopSamuraiCat_loop 1.mp3` in `Cena1` and
`PopSamuraiCat_introloop 1.mp3` in `Menu`.

## Not verified

Read from source, from the event assets' YAML and from the listener wiring in the scene. **Nothing
was compiled or played**, so no cue has been heard or seen firing.
