# PopSamuraiCat — Agent Entry Point

This project uses a **Karpathy-style LLM wiki** as its knowledge system, deployed from a central
method repository.

**Read [`wiki/SCHEMA.md`](wiki/SCHEMA.md) at the start of every session.** It is the authority on
how to read and write everything under `wiki/`. This file only points at it.

Then read [`wiki/index.md`](wiki/index.md) for the catalog of what's already known.

## First-time setup in a fresh clone

```
git config core.hooksPath .githooks
```

Installs the credential pre-commit scan. **Per-clone and per-developer** — a fresh clone has no
hook until this is run.

The method itself needs no install: it is a plugin checked into this repo at
`.claude/skills/brain-os/`, and Claude Code loads it as `brain-os@skills-dir`. Three things about
that are worth knowing before you wonder why a command is missing — it loads on the **next**
session after it changes, only after you accept the **workspace trust dialog**, and only if you
started Claude Code at the **repository root** (a project-scope plugin does not walk up from a
subdirectory).

### If `/brain-os:ingest` and `brain-os:code-reader` are not listed

**It is the trust gate far more often than it is this repository.** A closed gate presents as an
absence — no error, no warning, just missing commands — and the layout under
`.claude/skills/brain-os/` looks unusual enough beside an ordinary `.claude/skills/*/SKILL.md` that
the structure is the natural thing to suspect. It is almost never the structure. Work through these
before changing any file:

1. **The dialog may never have been presented.** Some front-ends, the VSCode extension among them,
   can leave a project at `hasTrustDialogAccepted: false` without ever prompting. Launching the CLI
   once from the repository root presents it; accepting there clears the gate.
2. **Your editor's own "workspace trust" is a different system.** Marking the folder trusted in
   VSCode's *Manage Workspace Trust* has no effect on the flag Claude Code reads. Two unrelated
   mechanisms that share a word.
3. **On Windows, check the project key twice.** The path key in `~/.claude.json` is case-sensitive on
   the drive letter, and different front-ends can write different entries for the same folder —
   `c:/Users/…` and `C:/Users/…` side by side, one trusted and one not. The plugin then loads in one
   place and stays invisible in the other. **Both** need `hasTrustDialogAccepted: true`.
4. **A broken marketplace can bury `/plugin`'s output.** If `/plugin` reports a marketplace failing
   to load, that error can crowd out everything else it would have shown. It is unrelated: a
   skills-directory plugin needs no marketplace at all.

This section lives here rather than in the wiki because **`CLAUDE.md` still loads when the plugin
does not** — when the operations are missing, this file is one of the few things a session can still
read.

## The short version

- `wiki/raw/` — human-owned sources, and the **only** place human material enters the wiki.
  **Never edit or delete anything here.**
- `wiki/systems/`, `wiki/design/`, `wiki/planning/`, `wiki/decisions/`, `wiki/skills/` —
  LLM-maintained. The human doesn't hand-write these; ask for changes.
- **Each folder owns one kind of fact, and nothing else repeats it** —
  `systems/` = *what the code is* · `design/` = *what the product is meant to be* ·
  `tasks/` = *what we did and when, the project's history* · `decisions/` = *why it changed*.
  See `wiki/SCHEMA.md` § Single source of truth.
- The **codebase is ground truth**. When a wiki page and the code disagree, the page is stale.
- Operations: `/brain-os:ingest` (new source), `/brain-os:query` (ask the wiki), `/brain-os:lint`
  (health check). They are vendored under
  `.claude/skills/brain-os/` — **that folder cannot move into `wiki/`**;
  the harness reads it by fixed path. `wiki/skills/` is a different thing that shares the word:
  written procedures, not commands.
- Every operation updates `wiki/index.md` and appends **one line** to `wiki/log.md`.
- **Plan before implementing** anything non-trivial; **check scope after**. See
  `wiki/SCHEMA.md` § Working practices.
- Every task with a deliverable gets a `wiki/tasks/TASK-NNNN-*.md` file — created when work starts,
  kept after it ships. **Link to deliverables; don't restate them.**
- **Code we write is ingested by the task that writes it** — the project map grows with the project,
  so no future session re-reads the same code cold.
- **Report the result, not the process.** Do the work, then say what changed and what needs a
  decision — **a screen or less**. Detail belongs in the wiki and the task file, not in
  step-by-step narration.

## This wiki was deployed, and improvements travel back

`.claude/skills/brain-os/`, `.githooks/pre-commit`, `wiki/SCHEMA.md` and **five procedure pages in
`wiki/skills/`** came from a central method repository and are **vendored copies** —
`.claude/skills/brain-os/.manifest.sha256` lists exactly which files, and
`sha256sum -c .claude/skills/brain-os/.manifest.sha256` from the repository root is the check.
Everything else under `.claude/` and `wiki/` — including `.claude/agents/`,
`.claude/settings.json`, and any procedure page you write yourselves — is **yours**, and that
boundary is the one to hold: a new agent this project needs goes in `.claude/agents/`, never inside
the vendored subtree.

A rule that turns out to be wrong here is probably wrong everywhere, so:

- **Method changes are drafted, not made in place.** Draft in `wiki/method-drafts/`, numbered
  locally from 0, and push upstream with the task and decision that justify the change. Drafts stay
  out of `wiki/index.md` and `wiki/log.md` — this project's registers never take a number that
  belongs to the method repository.
- **A locally-edited vendored file is drift**, and `/lint` reports it. If the edit is genuinely
  project-specific, it belongs somewhere that isn't a vendored file.

## Subagents: plan on Opus, ask before launching, synthesize in the main thread

1. **Planning and synthesis stay here, on Opus.** Subagents don't plan and don't decide.
2. **Never launch a subagent without an explicit go-ahead** — one agent or eight. State the
   **count**, the **agent** and the **split**, then wait. Every time.
3. **Agents report back; the main thread synthesizes.** Writing an agreed result can be delegated.
   Deciding what it says cannot.
4. **Use the defined agents, not `general-purpose`.** Seven ship with the method as
   `brain-os:<name>` (vendored, in
   `.claude/skills/brain-os/agents/`); any this project adds are
   its own and live in `.claude/agents/`. Each pins its own model; a
   `general-purpose` launch inherits Opus. Why each pin is what it is:
   [`wiki/skills/configuring-a-subagent.md`](wiki/skills/configuring-a-subagent.md) § *Choosing the
   model*.
5. **Close every task on the two checkpoints.** *Repetition* (`wiki/SCHEMA.md` Hard Rule 6): did
   this repeat a **procedure** with no `wiki/skills/` page, or a **worker shape** with no agent
   definition? *Project map* (Hard Rule 10): **what does the wiki now say that is wrong or
   incomplete?** Write whatever is missing **in the same task**. "Nothing repeated" is a fine
   answer; not having asked is not.
6. **Assume `.claude/` edits land next session.** An agent or skill written mid-task may not be
   usable until Claude Code restarts. Plan for the pessimistic case, but **check whether it's
   listed** rather than assuming either way.

## Two rules that matter most

1. **Never assert what you haven't read.** Mark inferences `confidence: low` or leave them out. A
   confident wrong page is the one failure this system can't recover from — future sessions will
   trust it.

2. **No verification theatre.** Never write "tests pass" or "verified working" unless something
   actually ran.

   *What this project can actually prove* — the same list as `wiki/SCHEMA.md` Hard Rule 3, repeated
   here because **this file still loads when the plugin does not**:

   - **Compilation, and only while the Unity Editor is open on this project.**
     `.\unity-cmd.ps1 '{"type": "refresh"}' -Timeout 120` recompiles and returns the errors. **A
     timeout is not a pass** — it means the Editor was not running and nothing was checked. Say which
     of the two happened.
   - **Nothing is automatically tested.** `com.unity.test-framework` is in `Packages/manifest.json`,
     but there are no test assemblies and no test files under `Assets/`. There is no suite to run.
   - **No CI, no build server, no linter, no formatter.** Nothing runs on push. A headless or player
     build has never been demonstrated here.
   - **Runtime behaviour cannot be read back.** The bridge's `describe` / `interact` / `game-step`
     report `IDescribable` widgets, and nothing in this project implements it yet.
   - **The only mechanical check in the repo** is the credential pre-commit hook, for developers who
     have run `git config core.hooksPath .githooks`.

   "It compiles" is the only correctness claim this project can make mechanically. Everything about
   whether the game *plays* right is a human looking at it.

## Project quick facts

**A 2D-styled Unity game** — a lane-based runner: the player state machine registers `idle`,
`running`, `jumping`, `airborne`, `changingLane`, `chargingDash` and `dashing`
(`Assets/Scripts/StateMachineImplementation/`), and `LevelManager` spawns level parts ahead of the
player as they advance (`Assets/Scripts/LevelManager.cs`).

- **Unity 2021.3.45f2** (`ProjectSettings/ProjectVersion.txt`), so **C# 9** — no newer language
  features. Input System 1.7.0, Cinemachine 2.10.1, TextMeshPro 3.0.6. No `.asmdef` under `Assets/`,
  so everything compiles into `Assembly-CSharp`.
- **Windows and PowerShell.** The context hook and `unity-cmd.ps1` are both PowerShell; `.csproj`
  and `.sln` files are Unity-generated and git-ignored.
- **Two scenes**, both registered in `ProjectSettings/EditorBuildSettings.asset`:
  `Assets/Scenes/Menu.unity` and `Assets/Scenes/Cena1.unity`. Names mix English and Portuguese —
  keep existing ones as they are.
- **The Unity Editor is a live writer.** If it is open it owns the asset database; if it is closed
  nothing can be compiled or inspected. Both states are normal — know which one you are in before
  claiming anything.
- **The bridge is a single channel.** `unity-cmd.ps1` → `Assets/LLM/Bridge/request.json` →
  `response.md`, by polling. One caller at a time, from the main thread only. `{"type": "help"}`
  returns the full command reference — ask it rather than guessing a command.
- **Single-writer files** — `Packages/manifest.json`, `ProjectSettings/*.asset`, every `.meta`,
  `*.unity`, `*.prefab`, the bridge files and `Assets/Editor/BridgeScratch.cs`. Creating, deleting
  or renaming anything under `Assets/` is single-writer too, because Unity mints the `.meta`. Full
  table and reasoning: `wiki/SCHEMA.md` § *Delegation*.
- **Solo developer on `main`, pre-release.** Commit directly to `main` — see `wiki/SCHEMA.md`
  § *Commit to `main`*.
