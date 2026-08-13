---
type: system
status: verified
confidence: high
sources: [Assets/Scripts/GameplayManager.cs, Assets/Scripts/ScriptableObjects/GameplayVariables/GameplayVariablesObject.cs, Assets/Scripts/ScriptableObjects/GameplayVariables/Player.asset, Assets/Scripts/BoxCast/BoxCastManager.cs, Assets/Scripts/BoxCast/BoxCastCollision.cs, Assets/Scenes/Cena1.unity]
updated: 2026-08-13
---

# Player movement, physics and tuning

Every force the player feels goes through one plain C# class, `GameplayManager` — not a
`MonoBehaviour`, constructed by hand in `PlayerStateMachineBusiness.Awake()`
(`GameplayManager.cs:16-24`). The states decide *when*; this decides *what*. Which state calls what
is [[player-state-machine]].

## The three verbs

| Call | Effect |
|---|---|
| `Jump()` | **Sets** vertical velocity to `jumpForce`, preserving x and z (`GameplayManager.cs:33-36`) |
| `Move(axis)` | **Adds** a continuous forward force of `axis * movementSpeed` (`GameplayManager.cs:38-41`) |
| `Dash(dir)` | **Sets** the whole velocity vector to `dir * dashForce` (`GameplayManager.cs:43-46`) |

The mix of set-velocity and add-force is deliberate enough to be worth naming: jump and dash
overwrite whatever the physics engine was doing, while running accumulates against drag. Terminal
running speed is therefore a drag/force balance, not a configured number.

**The player never moves itself sideways.** `xAxis` is a public field on the business object fixed
at `1` in the scene, and forward force is applied by the states, not by an input
(`PlayerStateMachineBusiness.cs:16`, `:94-102`). This is an auto-runner — see [[player-input]] for
the movement binding that exists in the action map and is never read.

## Gravity and drag are per-state

`ApplyGravityMultiplier(m)` adds `GRAVITY * m - GRAVITY` as an acceleration
(`GameplayManager.cs:52-56`), so it applies the *difference* on top of Unity's own gravity: `m = 1`
is a no-op and `m = 0` cancels gravity outright. Four overloads pick the multiplier by state type
(`GameplayManager.cs:48-51`), and four more do the same for drag (`GameplayManager.cs:70-74`).
Overload-on-state-type is this codebase's idiom for "look up the constant for the caller"; the
argument is never used.

**Tuned values**, from `Assets/Scripts/ScriptableObjects/GameplayVariables/Player.asset` — the
`GameplayVariablesObject` asset the scene actually references. **The C# field initialisers in
`GameplayVariablesObject.cs` are not these numbers** and are dead once the asset exists; read the
asset, not the class.

| | Gravity × | Drag |
|---|---|---|
| grounded (default) | 1 | 9 |
| `jumping` | 1.2 | 6 |
| `airborne` | **12** | 6 |
| `chargingDash` | 0.6 | 10 |
| `dashing` | **0** | **0** |

Two consequences fall straight out of the table. `airborne` at 12× gravity against `jumping` at
1.2× means the fall is roughly ten times as sharp as the rise, so the jump arc is strongly
asymmetric. And `dashing` at zero gravity, zero drag, with the velocity re-set every physics tick
(`DashingPlayerState.cs:43-48`) is a perfectly straight line for the dash's duration.

Movement constants: `jumpForce: 21`, `movementSpeed: 90`, `coyoteTime: 0.2`. Dash constants:
`dashForce: 34`, `dashChargeForceWeight: 1.27`, `dashTimeout: 0.15`, `dashChargeTimeoutWeight: 1.82`,
`dashCooldown: 1`, `chargingDashSpeedMultiplier: 0.2`, `chargingDashTimeout: 1.18`.

The dash's launch speed is `powerPercentage × 1.27 × 34` and its duration is
`0.15 × powerPercentage × 1.82` (`PlayerDashResponse.cs:62-64`, `:79-83`) — both scaled by the
charge curve in [[player-state-machine]], which caps at ≈0.876.

## Lanes are two, hardcoded at z = ±1

There is no lane list and no lane count. `currentLane` is a single float that gets multiplied by
`-1` to switch (`PlayerChangeLaneResponse.cs:20`), and the arrival test is
`GetDistanceToLane() => 1 - Mathf.Abs(z)` (`GameplayManager.cs:68`) — the `1` is the lane offset,
written as a literal. The player starts at `z: -1` in `Assets/Scenes/Cena1.unity`.

`ChangeLane()` is a `Vector3.Lerp` at a fixed `0.6` per call (`GameplayManager.cs:26-31`), driven
from a coroutine that re-invokes itself every `WaitForEndOfFrame` until within tolerance
(`PlayerChangeLaneResponse.cs:34-44`). Two things follow: the lane change is **frame-rate
dependent** — faster machines complete it in less real time — and it converges geometrically, so
the player settles at |z| between 0.99 and 1 and never exactly on the lane.

**`GetDistanceToLane()` cannot tell which lane is the target.** It measures distance to whichever
boundary is nearer, which is what makes defect 1 in [[player-state-machine]] reachable. A third lane
would need this function, the `*= -1` toggle and the literal `1` all replaced.

## Ground and wall probes

`BoxCastManager` is a `MonoBehaviour` holding a list of named `BoxCastCollision` structs; a probe is
a `Physics.BoxCast` from a child transform against a layer mask, looked up by string
(`BoxCastManager.cs:9-19`, `BoxCastCollision.cs:28-33`). A name that does not match returns `false`
— a typo reads as "nothing there", silently.

`GameplayManager` exposes four of them (`GameplayManager.cs:58-66`):

| Query | Probe(s) | Used for |
|---|---|---|
| `IsGrounded()` | `GroundCheck` | landing, coyote start, idle/airborne |
| `CanMoveForward()` | `RightWallCheck` | running↔idle, and gating forward force while airborne |
| `CanChangeLane()` | `FrontWallCheck` **and** `BackWallCheck` | lane change gate |
| `IsFalling()` | — | `velocity.y < 0.5f`, not `< 0` |

`IsFalling()`'s threshold is `0.5`, not zero, so the jump is treated as over slightly before apex.

Five probes are configured on the player in `Assets/Scenes/Cena1.unity`; **`LeftWallCheck` is
configured but never queried by any code**. `GameplayManager.IsMoving()` (`GameplayManager.cs:64`)
has no callers either.

The Portuguese naming is local to this subsystem — `ChecarBoxCast`, `indentificador` (sic),
`distanciaOrigem`, `escalaFinal`. Existing names stay as they are ([[SCHEMA]] § *Project-specific
vocabulary*).

## Not verified

The numbers above are read from the asset and the source. **Nothing was compiled or played during
this pass**, so no claim here about how the movement *feels* — the asymmetric jump arc, the
front-loaded charge — has been observed.
