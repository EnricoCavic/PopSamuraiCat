---
type: system
status: verified
confidence: high
sources: [Assets/Scripts/BaseStateMachine/State.cs, Assets/Scripts/BaseStateMachine/StateMachine.cs, Assets/Scripts/BaseStateMachine/StateMachineBusiness.cs, Assets/Scripts/BaseStateMachine/StateResponse.cs, Assets/Scripts/StateMachineImplementation/PlayerStateMachineBusiness.cs, Assets/Scripts/StateMachineImplementation/States/PlayerState.cs, Assets/Scripts/StateMachineImplementation/States/PlayerStatesImplementation/, Assets/Scripts/StateMachineImplementation/StateResponses/]
updated: 2026-08-13
---

# Player state machine

The player is the only thing in the game driven by a state machine. Seven states, one at a time,
each deciding its own successor. Physics and probes are [[player-movement]]; the input side is
[[player-input]]; the animation and audio cues each state raises are [[game-events]].

## The generic machine

Four small classes under `Assets/Scripts/BaseStateMachine/`, none of them player-specific:

- `State` — `Enter()` / `Tick()` / `FixedTick()` / `Exit()`, all virtual no-ops.
  **`Tick` and `FixedTick` return the next state**, and returning `this` means "stay"
  (`State.cs:9-11`).
- `StateMachine<T>` — holds `currentState`, and `NewState()` runs `Exit()` → swap → `Enter()`,
  short-circuiting when the next state is the current one (`StateMachine.cs:15-23`).
- `StateMachineBusiness<T>` — the `MonoBehaviour` half: a `string`-keyed dictionary of states, and
  `TickStateMachine()` / `FixedTickStateMachine()` for the owner to call
  (`StateMachineBusiness.cs:11-37`).
- `StateResponse` — base class for the input-driven actions, holding a reference back to the
  business object (`StateResponse.cs:5-21`).

`StateResponse` is generic in name only: it is typed to `PlayerStateMachineBusiness` and
`StateMachine<PlayerState>` directly (`StateResponse.cs:7-8`), so a second state machine in this
project would reuse the other three classes but not this one.

## The player's machine

`PlayerStateMachineBusiness` is a `MonoBehaviour` on the `Jogador` object in
`Assets/Scenes/Cena1.unity`, and it is where everything on the player is assembled. `Awake()`
constructs the [[player-movement|GameplayManager]] and the [[player-input|InputProcessor]] by hand,
then registers the states and the input responses
(`PlayerStateMachineBusiness.cs:23-32`).

The tick order matters and is not the obvious one:

| Callback | What runs |
|---|---|
| `Update` | death-plane check, then `TickStateMachine()` (`PlayerStateMachineBusiness.cs:34-40`) |
| `FixedUpdate` | `currentState.MoveInput()` **first**, then `FixedTickStateMachine()` (`PlayerStateMachineBusiness.cs:42-46`) |

`MoveInput()` is overridden only by `idle`, where it is the idle→running transition
(`IdlePlayerState.cs:30-34`) — it is not a movement input in the ordinary sense, and no input
feeds it.

Seven states are registered by string tag, and `idle` is the initial one
(`PlayerStateMachineBusiness.cs:55-67`).

## The seven states

| State | Enters from | Leaves for | Notes |
|---|---|---|---|
| `idle` | initial; `running` when blocked ahead | `running` (`MoveInput`, path clear) · `airborne` (not grounded) · `jumping` | Consumes a buffered jump on `Enter` (`IdlePlayerState.cs:13-17`) |
| `running` | `idle` · `airborne` (landed) · `changingLane` | `idle` (blocked ahead) · `jumping` · `airborne` (via coyote timeout) | Applies forward force each `FixedTick` (`RunningPlayerState.cs:45-52`) |
| `jumping` | `idle` / `running` `Jump()` | `airborne` | Ends on falling **or** on jump released — a variable-height jump (`JumpingPlayerState.cs:39-49`) |
| `airborne` | `jumping` · `running` (coyote) · `changingLane` · `chargingDash` and `dashing` timeouts | `running` on landing | Landing needs grounded **and** falling (`AirbornePlayerState.cs:30-31`) |
| `changingLane` | any state's `ChangeLane` | `running` or `airborne` when the move completes | Blocks dash and further lane changes while it runs (`ChangingLanePlayerState.cs:32-34`) |
| `chargingDash` | any state's `DashInputStart` | `dashing` on release · `airborne` on timeout | Accumulates `dashPower` per physics tick (`ChargingDashPlayerState.cs:50-56`) |
| `dashing` | `chargingDash` release | `airborne` on timeout | Re-applies the dash velocity every `FixedTick` (`DashingPlayerState.cs:43-48`) |

**Coyote time is a coroutine, not a timer field.** `running` starts it the first frame it is not
grounded (`RunningPlayerState.cs:35-39`); the coroutine waits `coyoteTime` and then calls
`StateTimeout()` on whatever state is current, passing `running` as the identity token
(`PlayerStateMachineBusiness.cs:109-113`). Each state's `StateTimeout` compares that token against
itself and ignores timeouts that are not its own — that is the whole mechanism for "a timer fired
but we already left".

## The three responses

`StateResponse` subclasses are constructed and subscribed to Input System callbacks in
`InitializeResponses()` / `SubscribeResponses()`, and unsubscribed in `OnDestroy`
(`PlayerStateMachineBusiness.cs:69-92`). Each one is a bridge: the input event arrives at the
response, the response asks the *current state* what to do.

- **`PlayerJumpResponse`** — `PerformAction()` sets the jump velocity, drops the buffered input, and
  starts a one-fixed-frame coroutine that unlocks `jumping`'s exit condition
  (`PlayerJumpResponse.cs:18-29`).
- **`PlayerChangeLaneResponse`** — flips `currentLane`, then re-lerps the player toward it from a
  self-restarting per-frame coroutine until the move is within tolerance
  (`PlayerChangeLaneResponse.cs:25-44`).
- **`PlayerDashResponse`** — gates on `canStartCharge`, holds the dash direction and the cooldown,
  and owns both the charge timeout and the dash timeout coroutines
  (`PlayerDashResponse.cs:30-83`).

Dash strength is `Mathf.Pow(dashPower, 1f/5f) / chargingDashTimeout` — a **fifth root**, unclamped
(`ChargingDashPlayerState.cs:84`). With the tuned `chargingDashTimeout` of 1.18
([[player-movement]] for the values) the curve reaches ≈0.39 after 20 ms of holding and tops out at
≈0.876, never 1. Charging is therefore heavily front-loaded and a full-length hold is worth barely
more than a fifth of one. **Read off the code and the asset, not observed in play** — whether it is
intended is a design question nothing in the repo answers.

## Defects found by reading

All four were found by reading the source. **None has been reproduced in Play Mode** — nothing was
run during this pass.

1. **A lane-change press during a lane change desyncs the tracked lane.**
   `PlayerChangeLaneResponse.RecieveInput` flips `gameplayManager.currentLane` *before* asking the
   state (`PlayerChangeLaneResponse.cs:20-22`), but `changingLane` overrides `ChangeLane` to a
   no-op (`ChangingLanePlayerState.cs:34`). The flip sticks with no movement, so the next press
   targets the lane the player is already on — and because `GetDistanceToLane()` measures distance
   to *either* boundary rather than to the target ([[player-movement]]), that press exits
   `changingLane` on its first `Tick` and is silently eaten.
2. **`DashFail` fires on unrelated timeouts.** `ChargingDashPlayerState.StateTimeout` raises
   `chargeEndEvent` outside the `if(_currentState == this)` guard
   (`ChargingDashPlayerState.cs:76-82`), so a coyote-time timeout landing mid-charge plays the
   dash-fail cue without ending the charge.
3. **Charge timeouts are started even when the charge is refused.**
   `PlayerDashResponse.RecieveInput` starts `ChargingStateTimeout` unconditionally once
   `canStartCharge` passes (`PlayerDashResponse.cs:30-38`), but `changingLane`, `chargingDash` and
   `dashing` all ignore `DashInputStart`. Presses during those states queue coroutines that later
   fire `StateTimeout` at an unrelated state — which is what makes defect 2 reachable.
4. **The run animation overwrites a buffered jump's animation.** `RunningPlayerState.Enter` consumes
   the buffered jump *before* raising `RunAnimation` (`RunningPlayerState.cs:24-31`), so the
   jump cue is raised inside `Jump()` and then immediately overridden on the way back out.

One more, harmless today: `StateMachineBusiness.AddState` null-checks `stateDictionary` *after*
dereferencing it (`StateMachineBusiness.cs:30`), so the guard cannot fire. `InitializeStates()`
always assigns the dictionary first, so nothing reaches it.

## Not read

Nothing in `Assets/Scripts/` is unread. What is **unverified** is all of the above at runtime: no
compile was triggered and no Play Mode session was observed ([[SCHEMA]] Hard Rule 3).
