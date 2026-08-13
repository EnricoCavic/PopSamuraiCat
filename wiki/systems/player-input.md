---
type: system
status: verified
confidence: high
sources: [Assets/Scripts/PlayerInput/InputProcessor.cs, Assets/Scripts/PlayerInput/InputBuffer.cs, Assets/Scripts/PlayerInput/InputData.cs, Assets/InputActions/Character Input Actions.inputactions, Assets/Scripts/Components/TimeScaleChanger.cs, Assets/Scenes/Cena1.unity]
updated: 2026-08-13
---

# Player input

Input System 1.7.0, one action map, one wrapper class. `InputProcessor` is a plain C# object built
in `PlayerStateMachineBusiness.Awake()` (`InputProcessor.cs:20-26`); it pulls `InputAction` handles
out of the `PlayerInput` component by name and hands them to the state responses in
[[player-state-machine]].

## The bindings

From `Assets/InputActions/Character Input Actions.inputactions`, map `Player`:

| Action | Bound to | Read by |
|---|---|---|
| `Jump` | Space, X | `InputProcessor` (buffer + held flag) and `jumpResponse` |
| `Dash` | Mouse left | `dashResponse`, on both `started` and `canceled` |
| `ChangeLane` | Z, mouse right | `changeLaneResponse` |
| `MousePosition` | Mouse position | dash aiming |
| `TimeScale` | P | `TimeScaleChanger` |
| `Move` | A / D, as a 1D axis | **nothing** |

**`Move` is bound and never fetched.** `SetAllInputActions()` retrieves five actions and `Move` is
not among them (`InputProcessor.cs:28-35`), so A and D do nothing — the game is an auto-runner
([[player-movement]]). `InputProcessor.changeLaneAxis` (`InputProcessor.cs:17`) is declared and
never assigned or read either; the lane change is a toggle, not an axis.

Only `Jump` is subscribed by the processor itself, to track held state and feed the buffer
(`InputProcessor.cs:37-47`, `:62-71`). The other three are subscribed by
`PlayerStateMachineBusiness` directly onto the responses, and unsubscribed in `OnDestroy`.

## Dash aiming is a mouse ray, not a direction key

`GetDashDirection()` converts the mouse position to a world point at the player's own z, then
normalises the vector from player to cursor (`InputProcessor.cs:49-60`). The dash therefore aims
**anywhere in the plane**, including backwards and downwards — the charge decides how hard, the
cursor decides where.

The z-flattening is done by hand: `mousePosition.z = targetZ - camera.transform.position.z` before
`ScreenToWorldPoint` (`InputProcessor.cs:52`).

## The jump buffer

`InputBuffer` is a FIFO of `InputData` (an action name plus the frame it arrived on), with a
per-entry expiry coroutine (`InputBuffer.cs:18-60`). Buffer window in `Assets/Scenes/Cena1.unity`:
**0.08 s** (`inputBufferTimeout`).

The mechanism is deliberately narrow — **only the head of the queue is ever inspected**, and only by
name (`InputBuffer.cs:33-39`). `idle` and `running` check for a buffered `"jump"` on `Enter` and act
on it immediately (`IdlePlayerState.cs:13-17`, `RunningPlayerState.cs:27-28`), which is what lets a
jump pressed just before landing still fire. Nothing else registers into the buffer: the only
`RegisterInput` call is for `"jump"` (`InputProcessor.cs:65`), so dash and lane change have no
buffering at all.

Expiry drops an entry only if it is still at the head (`InputBuffer.cs:53-60`) — an entry that has
been overtaken is left for whoever dequeues it. The file carries the author's own note that the
`Queue` type is the wrong data structure here (`InputBuffer.cs:7`, in Portuguese).

`InputData.frameCount` (`InputData.cs:8`) is recorded and never read.

## Time scale toggle

`TimeScaleChanger` grabs `timeScaleAction` off the player's `InputProcessor` in `Start` and toggles
`Time.timeScale` between `1` and `slowMotionMultiplier` on P (`TimeScaleChanger.cs:17-29`). It is
present in `Cena1` and reachable in a build — it is a debug affordance with no build guard around
it.

Its `Awake()` is empty (`TimeScaleChanger.cs:12-16`); the `Start`-not-`Awake` ordering is load-
bearing, because `InputProcessor` does not exist until `PlayerStateMachineBusiness.Awake()` has run.

## Not verified

Read from source and from the `.inputactions` asset. No input was exercised — nothing was played
during this pass.
