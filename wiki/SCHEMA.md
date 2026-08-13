---
type: schema
updated: 2026-08-13
---

# Wiki Schema

The operating manual for this wiki. If you are an LLM working in this repo, this file governs how
you read and write everything under `wiki/`. Read it once per session.

Based on Andrej Karpathy's LLM wiki proposal, adapted for a **codebase** rather than a reading list.

**This file states rules and what breaking them costs. It does not carry the history of how a rule
got its shape** — that lives elsewhere in this method's own records, not in what gets deployed here.

## This file is vendored

This SCHEMA was deployed into this project from a central method repository, not written for this
project specifically. Local edits to it will be reported as drift the next time the method is
synced. If a rule here is wrong or incomplete, that is a finding about the method, not about this
project — raise it and let the change go back upstream rather than editing this file in place. The
deploying repository owns the return path; this copy does not.

**Three other things in this project are vendored on the same terms**: the method plugin at
`.claude/skills/brain-os/`, the credential hook at `.githooks/pre-commit`, and the five procedure
pages in `wiki/skills/` that this file links — [[configuring-a-subagent]],
[[parallel-coding-fan-out]], [[mapping-what-you-built]], [[handing-off-a-session]] and
[[verifying-a-milestone-plan]]. All of it is listed in `.claude/skills/brain-os/.manifest.sha256`,
and `/brain-os:lint` reports anything edited since the deploy. **Everything else under `wiki/` and
`.claude/` is this project's own**, including any further procedure page it writes for itself.

## The layers

| Layer | Path | Who writes it | Rule |
|---|---|---|---|
| **Raw sources** | `wiki/raw/` | Human only | The LLM **never** edits or deletes anything here. Read-only, append-by-human. |
| **Wiki** | `wiki/systems/`, `wiki/design/`, `wiki/planning/`, `wiki/decisions/`, `wiki/skills/` | LLM only | The human **never** hand-writes these. Ask the LLM to change them. |
| **Task record** | `wiki/tasks/` | LLM only | The project's history — **not knowledge**. One file per task with a deliverable, created when work starts and kept after it ships. See § *Task record*. |
| **Schema** | this file | Both, deliberately | Co-evolves as we learn what works — in the deploying project, not by local edit here. Changing it changes how every future session behaves. |

The wiki layer says what we *know*; the task record says what we *did*. They are separate because a
fact and the work that produced it go stale on different schedules.

**`.claude/` is not a layer — it is configuration.** Agent definitions, skills and settings; the
harness reads them from fixed paths, which are the only discovery paths its documentation gives, so
they cannot be moved into `wiki/` and are not pages. They are tracked in the repo and ship with it;
the wiki documents them ([[configuring-a-subagent]]) rather than containing them.

**It holds two kinds of thing and they are not interchangeable.** `.claude/skills/brain-os/` is the
**vendored method** — a plugin, loaded as `brain-os@skills-dir`, carrying the three operations, the
agents that ship with them, the context hook and the tools. Everything else under `.claude/` is
**this project's own**. Editing the first is drift and gets reported as such; editing the second is
ordinary work. A new agent this project needs goes in `.claude/agents/`, outside the subtree.

**The codebase itself is also a raw source** — the project's own source tree is ground truth. Wiki
pages describe it; they never replace it. When a wiki page and the code disagree, **the code is
right and the page is stale** — fix the page.

The division of labour is the point: the human curates sources and asks good questions; the LLM does
all the bookkeeping — cross-references, keeping summaries current, flagging contradictions.

### Inside the wiki layer — what each folder is derived from

The folders are not four flavours of the same thing. They differ by **what they're built from**,
which is also what makes them grow.

| Folder            | Derived from                                    | Grows when                                                  |
| ----------------- | ------------------------------------------------ | ------------------------------------------------------------ |
| `wiki/systems/`   | the **codebase**                                | an unmapped area gets read                                  |
| `wiki/design/`    | `wiki/raw/`, **and** design work with the human  | a source lands in `raw/`, or a design question gets settled |
| `wiki/planning/`  | `design/` **and** `decisions/` together          | a route to build something gets costed, or a milestone lands and changes what follows |
| `wiki/decisions/` | a choice actually made, in conversation          | a real decision is locked                                   |
| `wiki/skills/`    | repetition — a procedure worth not rediscovering | a task closes having repeated one (Hard Rule 6)              |

Two consequences worth stating outright:

- **Human material enters in exactly two ways: a file in `wiki/raw/`, or a statement in
  conversation.** There is no third path, and no separate `wiki/synthesized/` folder — that would
  contain *everything that isn't `raw/`*, a distinction the human/LLM split above already draws.
  When the human states substantive new material in conversation rather than settling a specific
  question, that is a raw source arriving by voice: **transcribe it into `wiki/raw/` per Hard Rule 1
  before synthesizing from it.**
- **`systems/` is deliberately incomplete and expected to stay that way for a while.** It is a map
  under construction, not a specification. `index.md` § *Biggest gaps* is its edge — the honest list
  of what hasn't been read yet.

#### Design work is not ingest

`design/` grows from **settling questions** as much as from reading sources. Settling one produces
three different outputs, and telling them apart is the whole discipline:

| What was actually settled | Where it lands |
|---|---|
| **A real fork** — an alternative was rejected and the reason is worth remembering | The design page states the resulting design; a **`decisions/` page** records the fork, the rejected option, and what it would have cost |
| **Unstated detail** — a small specific the design didn't pin down yet | The **design page** only. No decision page: nothing was rejected, and a register full of one-line non-choices stops being readable |
| **A question about what the code already does** | Not a design question at all. `/brain-os:query` it and update **`systems/`** |

Then, in the same pass: record the resolution on the design page itself, with a one-line note of
what was decided and why it landed there, and append one line to `log.md`. **The note paraphrases
and links; it never quotes** — a resolution has no claim of its own beyond what the design page now
says. Verbatim stays in `raw/`.

## Single source of truth — what each folder owns

Every fact has **exactly one home**. Other pages reach it with a wikilink and, at most, **one
clause** of gloss so the sentence reads. Restating the reasoning at the link is the failure this
section exists to prevent: one correction then has to be chased across five pages instead of made
once.

| Folder | Answers | Owns — the only place this lives | Never contains |
|---|---|---|---|
| `wiki/systems/` | **What the code is** | Code facts with `file:line`; defects found by reading; what is still unread | Design intent · why anything was chosen · project history |
| `wiki/design/` | **What the product is meant to be** | The design as it currently stands, stated as settled fact | Rejected alternatives · the argument that produced it · when it was decided |
| `wiki/planning/` | **How we intend to get there** | The route: order, blocking structure, cost, and what each step must prove | The design itself · why a fork was chosen · an account of work already done |
| `wiki/decisions/` | **Why it is that way** | The fork: what was rejected, what it would have cost, what would reopen it | The resulting design restated · the code it lands on · the work that shipped it |
| `wiki/tasks/` | **What we did, and when** | The project's history — one file per task, pointing at deliverables | Any fact a future session needs; that belongs in the wiki layer |
| `wiki/skills/` | **How to do it again** | Repeatable procedures | Anything done only once |
| `wiki/raw/` | **What the human said** | Verbatim sources | Interpretation of any kind |

**`tasks/` is the project's history and the register that matters most.** It answers *"what has been
done here"*, and it is the only folder ordered by time. **`decisions/` is secondary to it and
usually attached to it** — a decision records *intent*, and it can start a task, land in the middle
of one, or fall out of the end of one. Link the task from the decision and the decision from the
task; write the substance once, in whichever of the two owns it.

### Where a given fact goes

| The fact | Its one home | Everyone else |
|---|---|---|
| The retry queue caps at three attempts (`worker/queue.py:42`) | the `systems/` page for that module | links |
| The dashboard *is* the default landing page after login | the `design/` page for that screen | links |
| The retry cap was chosen over four other backoff strategies, and what each would have cost | the `decisions/` page | links |
| That the retry-queue rewrite ships in phase two, costs roughly three days, and what shipping it must prove | the `planning/` page | links |
| That it was decided on a given date, and which pages changed | the task file | links |
| That an operation happened on a date | `log.md`, **one line** | — |

### The linking rule

When you link, you get one clause of context — enough that the sentence stands alone — and no more:

- ✅ The retry queue caps at three attempts (see the decision page for the cap).
- ❌ The retry queue caps at three attempts, chosen after a fourth retry was found to double the
  average request latency under load while barely improving the success rate (see the decision page
  for the cap).

The second reads better, and that is the trap. When the load test that produced "doubles the
latency" gets rerun and the number changes, the first sentence costs one edit — on the decision page
— and the second costs however many pages restated it.

**Derived numbers have one home too.** Counts, tallies, statuses and *"N resolved across M rounds"*
lines belong on the page that owns the thing being counted. Everywhere else links rather than
quoting a number that will go stale.

**A markdown link in `wiki/` may never start `../`.** Where the wiki is opened as a vault rooted at
`wiki/` (an Obsidian vault is the common case), a relative path that climbs above it renders as a
nonexistent file *even when it is correct on disk* — and dot-folders such as `.claude/` are hidden
there regardless. Wiki-internal targets use `[[wikilinks]]`; a path outside the wiki gets backticks
and **no link**.

**A bare wikilink must resolve. An illustrative one goes in backticks.** These are two different
things that look identical in a plain-text editor and behave differently everywhere else: a bare
`[[page]]` renders as a link and dangles if the page does not exist, while a backticked one renders
as text and is inert. So a link to a page not yet written is fine — it marks something worth writing
— but a **worked example, a format specifier or a placeholder name is not a link at all** and must
be backticked, including inside a table cell.

This is mechanically checkable and worth checking, because the two cases are indistinguishable by
eye and the failure is silent: strip fenced blocks and inline code spans, then every remaining
`[[target]]` must name a real page. Classifying by *name* instead — treating anything that looks
like a placeholder as intentional — is the version of this check that misses real defects, because
the names are the part you cannot trust.

**Verbatim human quotes belong to `wiki/raw/`.** A page may quote the **single clause its own claim
turns on** — a `decisions/` page quoting the sentence that settles its fork is the normal case. The
full block stays in the raw file and every other page links to it by section and number. The cost of
ignoring this: the human withdraws his own wording, and every copy becomes a page that now misquotes
him.

### Registers: `index.md` lists, READMEs explain

[[index]] is the **only** catalog — one row per page, for every folder. A folder's `README.md`
states what that folder owns, what it must never contain, and how to write a page for it. **It does
not list its own contents.** Two copies of a register drift.

## The three operations

### Ingest — `/brain-os:ingest <path or topic>`
A new source arrives (meeting notes, a design doc, an export from another tool, or a system of the
codebase we haven't mapped).

**Code this project writes is one of those sources, and it is ingested by the task that wrote it** —
not when someone later notices the gap (Hard Rule 10). *Unmapped* code and *unwritten-up* code are
the same debt, and the second is cheaper to pay immediately.

Read it, discuss the takeaways with the human, then in **one pass**:
1. Write or update the relevant page(s) — **in the folder that owns each fact**, once.
2. Update `index.md`.
3. Open or update the task file; append **one line** to `log.md`.
4. Revisit every page that links to what changed — stale cross-references are the main failure mode.

### Query — `/brain-os:query <question>`
Answer from the wiki first, the code second. Cite with `[[page]]` links and `file:line` for code.
**If the answer took real work to produce, file it back as a page** — that is the whole point. A
good answer that only exists in chat history is a wasted answer.

### Lint — `/brain-os:lint`
Periodic health check. Look for: contradictions between pages, claims now stale against the code,
orphan pages nothing links to, missing cross-references, wiki pages no task file claims (§ *Task
record* — parentage is inbound), `confidence: low` claims that could now be verified, **facts
living outside the folder that owns them**, and gaps worth investigating. Report findings; fix the
mechanical ones.

## `log.md` — a ledger, not a record

**One table row per operation**, newest at the bottom:

| Date | Op | What | Record |
|---|---|---|---|
| 2026-03-14 | decision | Retry-queue backoff strategy | `[[TASK-0021-retry-queue-rewrite]]` |

That is the entire entry. What happened lives in the **task file**; what it taught us lives in the
**wiki layer**; why it was chosen lives in the **decision page**. **If an entry wants a paragraph,
the paragraph belongs somewhere else and a link is missing.** `<op>` is `ingest`, `query`, `lint`,
`decision` or **`build`** — the last for work that changes the product rather than the wiki.

Still append-only: never edit or delete a past row. **Every row points at a task file**, including
any rows backfilled for work that predates the registry.

## Page format

Every wiki page starts with frontmatter:

```yaml
---
type: system | design | planning | decision | skill
status: verified | partial | stub | locked
confidence: high | medium | low
sources: [src/worker/queue.py, raw/meetings/2026-01-planning-notes.md]
updated: YYYY-MM-DD
---
```

- **`status`** — `verified` = read the code and confirmed. `partial` = some of it confirmed. `stub`
  = placeholder, no real content yet. **`locked` is the decision-page value** and the only one
  that is not a statement about reading: it means Hard Rule 7 applies, so the page is superseded
  rather than edited.
- **`sources:` may be empty, and an empty list is a claim.** It says *this page rests on nothing
  citable in this repository* — true of a procedure that was reasoned out rather than read off
  something. It is not the same as a missing field, and it is not a placeholder to be filled in
  later.
- **`confidence`** — how much weight to put on the claims. Be honest; `low` is useful, a confident
  wrong page is not.
- **`sources`** — what the page is derived from. Every non-obvious claim should be traceable.

Link between pages with wikilinks: `[[retry-queue]]`. Link to code with paths and line
numbers: `src/worker/queue.py:42`.

## Hard rules

1. **Never edit `wiki/raw/`** — with one exception. If a raw source is wrong, note the correction in
   the wiki layer and say so; do not rewrite history.

   *Exception — transcription.* When the human says something in conversation that belongs in a raw
   source, the LLM may **append** it to the relevant raw file as a dated, clearly-marked
   transcription block, or create a new raw file. The LLM is acting as a typist, not an author:
   transcribe faithfully, add no interpretation, and say in chat that you did it so the human can
   correct it. Never revise or delete existing raw content this way. **This is a normal working
   tool, not a last resort** — most of `wiki/raw/` should be expected to arrive this way.

   *Exception — `raw/README.md`.* The folder's own README is an **index of its contents and the LLM
   maintains it**, like every other `README.md` in the project. Keeping it current is a normal part
   of transcribing a source into a new category, not an ask for the human.
2. **Never assert what you have not read.** Most of any codebase is unread at first, and a lot of it
   stays that way for a while. Mark inferences `confidence: low` or leave them out. Plausible-
   sounding invention is the one failure this system cannot recover from, because future sessions
   will trust it. **This is the most important rule in this file.**
3. **No verification theatre.** Never write "tests pass" or "verified working" unless something
   actually ran. Say what was actually done, not what would ideally be true.

   *What this project can actually prove:*

   - **Compilation — only while the Unity Editor is open on this project.**
     `.\unity-cmd.ps1 '{"type": "refresh"}' -Timeout 120` triggers a script recompile through the
     unity-bridge package and returns the compile errors. **A timeout is not a pass**: the bridge is
     a polling file channel (`Assets/LLM/Bridge/request.json` → `response.md`), so if the Editor is
     not running, the command times out and nothing was checked. Say which of the two happened.
   - **Nothing is automatically tested.** `com.unity.test-framework` 1.1.33 is listed in
     `Packages/manifest.json`, but there are **no test assemblies and no test files** anywhere under
     `Assets/` — there is no suite to run and no coverage of any kind.
   - **No CI, no build server, no linter, no formatter.** There is no `.github/` and no workflow in
     this repository; nothing runs on push. A headless or player build has never been demonstrated
     here.
   - **Runtime behaviour cannot currently be read back.** The bridge's `describe` / `interact` /
     `game-step` commands report widgets implementing `IDescribable`, and **nothing in this project
     implements it**, so they have nothing to report until something does. Play Mode observation is
     a human at the Editor.
   - **The one mechanical check in the repo is the credential pre-commit hook**
     (`.githooks/pre-commit`), and only for a developer who has run
     `git config core.hooksPath .githooks` in their own clone. It scans staged changes for secrets;
     it checks nothing else.

   So: "it compiles" is provable when the Editor is open, and **it is the only claim about
   correctness this project can currently make mechanically.** Everything about whether the game
   plays right is a human looking at it.
4. **One topic, one page.** Before creating a page, check `index.md`. If a page covers the topic,
   extend it; don't fork it.
5. **Every operation appends one line to `log.md`.** No silent edits, and no paragraphs — see
   § *`log.md` — a ledger, not a record*.
6. **Repeated work gets written down — as a skill, or as an agent — and it is checked at the end of
   every task.** The same multi-step *procedure* goes in `wiki/skills/`. The same *worker shape* —
   same kind of agent, same instructions — gets a definition in this project's own
   `.claude/agents/`, never inside the vendored subtree. Both stop the
   thing being rediscovered; the second also stops it being re-specified at a default model. See
   § *Delegation*.

   **The trigger is a checkpoint, not a count.** Before reporting a task done, answer both questions
   in writing: *did this repeat a procedure that has no skill page?* and *did it repeat a worker
   shape that has no agent definition?* If yes, write it **in the same task** — the deliverable is
   incomplete without it. "Nothing repeated" is a valid answer; not having asked is not. How much
   evidence you need: a shape that is obviously going to recur gets written the first time it is
   specified, and one seen three times is already overdue.
7. **Decisions get locked.** When a real choice is made (architecture, tooling, design direction),
   record it in `wiki/decisions/` with the reasoning and the alternatives rejected. A locked decision
   is not reopened without a new decision page superseding it.
8. **One fact, one home.** Before writing a sentence, ask which folder owns it (§ *Single source of
   truth*). If another page owns it, **link** — one clause of gloss, never the reasoning.
   Duplication is not redundancy insurance; it is what makes every future correction cost five edits
   instead of one.
9. **No unagreed subagents.** State the count, the model and the split, then wait for a go-ahead —
   every time, for any number of agents. See § *Delegation*.
10. **Code we write is mapped in the same task that writes it.** A task that adds a new system is not
    done until `systems/` describes it; a task that **changes** an existing one updates that page.
    **The unit is the claim, not the file** — ask *what does the wiki now say that is wrong or
    incomplete?* and fix that. A structural change to already-mapped code is the **worst** case, not
    an exempt one: it leaves a confident page that is now false, which Hard Rule 2 names as the one
    failure this system cannot recover from. Only a change no page makes a claim about needs
    nothing. Checked at task close beside Hard Rule 6. The cost of skipping it is paid by every later
    session that re-reads the same code from scratch — or worse, trusts a stale page — which is the
    expense this whole wiki exists to avoid ([[mapping-what-you-built]]).

## Working practices

The wiki is the *what*. This section is the *how* — a deliberately small set of habits, kept
intentionally minimal rather than expanded into a full governance framework.

### Plan before implementing, check scope after

For anything beyond a trivial edit, **write the plan first and get agreement on it** before changing
code: the files you'll touch, the approach, what could go wrong, and how to undo it. Disagreement is
far cheaper at the plan stage than after an implementation. Trivial edits — a typo, a tuning
constant, a one-line fix in code you just read — don't need this; judge honestly, because scaling
ceremony to risk is the point and over-applying it is how process becomes theatre.

**When done, re-read the plan and ask what changed that wasn't in it.** Unrequested refactors,
opportunistic cleanups and drive-by "improvements" are the most common way an AI change becomes hard
to review. Either flag them explicitly or revert them.

### Register work as it lands — the context is fleeting, the wiki is not

Anything that exists only in the conversation is lost at the next context clear, so nothing is
banked until the end. The task file is created when work **starts**; a page is written the moment
its content is settled, not after the last one is; each landed piece is committed. The test for a
task file's `## State` is whether a session that has never seen this conversation could continue from
it alone. Procedure: [[handing-off-a-session]].

**Context ceiling: 150,000 tokens.** Checked mechanically by
`.claude/skills/brain-os/tools/context-report.ps1`, which the method plugin runs as a
`UserPromptSubmit` hook on every prompt. It prints nothing until the session reaches the ceiling, so
it costs no tokens until it matters; at or past it, it prints one line telling you to land the
deliverable, update the task file's `## State`, commit, and `/clear`. The number is the script's
`-WarnAt` default, which `hooks/hooks.json` does not override.

**It warns; it does not stop.** Two things it depends on: the plugin being loaded this session (see
`CLAUDE.md` if the operations are missing), and PowerShell — the hook command is Windows-only, which
is fine for this project and would need fixing upstream if that ever changes. For the full picture
on demand, including the main-thread versus subagent output split:
`powershell -NoProfile -File .claude\skills\brain-os\tools\context-report.ps1`.

### Delegation — plan on the strongest model, execute on cheaper ones, the fan-out is agreed first

Four rules, ordered by how often they get broken.

**1. Planning and synthesis stay in the main thread, on the strongest model available.** The main
thread reads the question, decides how the work splits, and writes the result — plan on the
strongest model, implement on cheaper ones. That shape is the point, not a specific roster: execution
spreads across however many tiers the harness offers, and the model for a slice is whatever its
agent pins. The part that gets missed: **subagents inherit the session model unless told
otherwise**, so an unconfigured launch from a session on the strongest model silently spawns workers
on that same, most expensive tier. Override an agent's pin only for a subtask that is itself a
judgement call, and name which one and why.

**2. No subagent launches without an explicit go-ahead.** State the **count**, the **agent** and
**how the work is split**, then stop and wait — one agent or eight, never a courtesy reserved for
large fleets. A "more than a few" threshold is the loophole that lets unannounced agents consume half
a session. One go-ahead covers the whole stated fleet; changing the count, the agent or the split
needs a new one. **This is also where the size of a fan-out is set** — there is no session-wide mode
that pre-authorizes one, so *"use two, not six"* or *"none, do it here"* is a thing the human says at
the go-ahead and the session then holds to.

Before launching at all, check the fan-out earns its cost. **The report is part of the cost** — an
agent's findings land in this thread's context, so a fan-out only pays when **the reading it replaces
is larger than the prose it returns**. So: **bound the return format** when you launch (what shape,
roughly how long), and **check first whether the question is mechanical** — a script that greps,
counts or diffs beats both a fan-out and a manual read, and returns a precise answer instead of a
summary (the `/brain-os:lint` duplicate-detection check is a worked example). Signs the fleet is too big:
overlapping slices, agents that finish with nothing to report, or a synthesis pass longer than the
work would have been. A cheaper model is not a licence to spawn more — many cheap agents is not the
improvement on many expensive ones; fewer, better-sliced agents usually is.

**3. Subagents report back; the main thread synthesizes.** An agent returns findings — paths,
`file:line`, quotes, and what it could not confirm — and the main thread decides what the wiki should
say. **Synthesis is what does not delegate; writing does.** Once a page's shape is decided here,
handing the edits to agents is fine and often right. What must not happen is N agents each
independently deciding what a fact means and where it goes: that is § *Single source of truth*'s
problem arriving through a second door. **A delegated finding behind a locked decision gets read
against source** at the point the decision is made, not as a lint afterthought — an agent's
`file:line` is a claim this thread is about to build on, Hard Rule 2's failure is exactly a plausible
one being trusted, and the check costs one read.

**Agreement between parallel agents is not corroboration.** They are the same model on the same
training data with the same blind spots, so N agents concurring is one opinion reported N times.
**Never count agents as votes.** What actually raises confidence is a different *kind* of evidence:
reading the source, running something, or a human. Give agents deliberately different slices rather
than the same question, and when a finding matters, verify it — do not re-ask it.

**4. A repeated shape becomes a definition in `.claude/agents/`.** Hard Rule 6's executor half: a
`description` the main thread selects on, a narrow `tools:` list, a **pinned `model:`**, and a
bounded return contract. How: [[configuring-a-subagent]]. The pin is the mandatory part — a
definition that omits `model:` inherits the main thread's, so a planning session on the strongest
model silently staffs a fleet of workers on that same tier. Ask at the **end of every task**, not
when a counter trips; write it in the task that revealed the shape, and expect it to be usable
**next** session rather than this one. Definitions are cheap, reversible and deletable — a stale,
unused definition is a smaller failure than a later session re-specifying the same worker at the
inherited model. **A definition is availability, not authorization:** rule 2 still binds on every
launch, and a pinned tier does not lower Hard Rule 2's bar. **The roster is not in the wiki** — the
harness lists the available agent types every session, so [[index]] deliberately does not catalog
them; the definitions are their own register — the vendored roster under
`.claude/skills/brain-os/agents/`, this project's own under `.claude/agents/`.

### What a fan-out costs, and what a session costs

**Prefer the defined agent over an unconfigured, general-purpose launch.** It carries the model pin
and the tool limits with it, so the choice is structural instead of something each launch has to
remember; a shape with no agent yet is the trigger for rule 4. **The model each one runs on is its
own frontmatter and is not restated here** — a table of *slice → tier* is the same fact in a second
place, and it will drift from the definitions within days of being written. How to choose a pin, and
what each tier is actually good at, is [[configuring-a-subagent]] § *Choosing the model*.

Two slices never go to an agent at all: **a design or architecture call** (rule 3 — that is the
synthesis that does not delegate), and **a slice that can be rewritten as a mechanical extraction
plus a judgement here** — cheaper than any agent, and the judgement lands where the context already
is.

**Session length is the other half of the cost.** Every turn at high context re-reads a large
prefix, so a long session is expensive before a single agent launches. One wiki operation per
session; clear context when switching tasks; use a longer-running compaction step when one operation
genuinely runs long. A fan-out inside an already-huge session pays twice.

**Implementation is the other half:** writing code in the main thread is what fills a context
fastest, so a specified change is delegated to a coding agent (procedure: [[parallel-coding-fan-out]])
— with edits to this project's single-writer files staying here, serially.

**Single-writer files.** In a Unity project the dangerous set is larger than the usual manifest and
lockfile, because the Editor and the asset database are writers too. Only the main thread touches
these, serially:

| What | Why |
|---|---|
| `Packages/manifest.json` | Package registration; Unity rewrites `Packages/packages-lock.json` from it |
| `ProjectSettings/*.asset` | Editor-owned YAML. `EditorBuildSettings.asset` (the scene list), `TagManager.asset`, `InputManager.asset` and friends are rewritten wholesale, not line-edited |
| Any `.meta` file | Unity mints and owns these. A hand-written or deleted `.meta` changes an asset's GUID and silently breaks every reference to it |
| `Assets/Scenes/*.unity`, `Assets/Prefabs/*.prefab` | Serialised YAML keyed by fileID. Edited through the Editor or a bridge command, never by hand and never concurrently |
| `Assets/LLM/Bridge/request.json`, `response.md`, `unity-cmd.ps1` | **One request/response channel, one caller.** Two agents calling the bridge at once overwrite each other's request and read each other's response |
| `Assets/Editor/BridgeScratch.cs` | One scratch pad, one writer — same reason |

**Creating, deleting, moving or renaming anything under `Assets/` is a single-writer act too**, even
though the file itself is new: it makes Unity mint a `.meta`. Agents edit the **bodies** of existing
`.cs` files under `Assets/Scripts/`; new assets and every registration step are done here.

### Commit to `main` — a default, not a universal

**Work directly on `main`** is the default this method assumes: pre-release and single-developer,
where `main` does not yet need to stay stable and a branch per task produces history harder to
follow than the work it protects. **That condition has to actually hold for this rule to apply** —
once a project is released, has more than one contributor committing concurrently, or otherwise needs
`main` to stay shippable at all times, this default no longer holds and branching resumes being the
right call. Branch only when a change is genuinely risky enough to want a merge request in front of
it, and **delete the branch after it merges** — long-lived merged branches are the mess this rule
exists to prevent.

### Report the result, not the process

Do the work, then report it **short and readable — a screen or less**: a few lines for ordinary work,
at most a short paragraph plus a list of links for a large one. Narrating each step as it happens,
echoing file contents back and listing every tool call buries the part the human actually needs —
what changed, what it means, and what needs a decision. The measure is not "did I mention
everything", it is "can this be read in one pass". Specifically: no preamble or restatement of the
request, no recap of files already listed in the task file, no per-agent summaries after a fan-out,
no closing offer of next steps that weren't asked for. Link the task file and the pages; they hold
the detail.

This is a reporting rule, not a rigour rule — the work itself stays thorough and Hard Rules #2 and #3
still bind. **Saying what you actually did is the practice that matters most in this repo**: *"read
the code path, did not run it"* is a fine and useful statement; *"tests pass"* is not, unless
something actually ran them. If a step genuinely needs sign-off before proceeding, ask for it — that
is a question, not a status update.

### What is deliberately NOT here

No phase state machine, no gate receipts, no per-task classification freeze, no validator over
documentation structure. Those were evaluated and dropped in favour of a small set of habits that
hold under deadline pressure. The one mechanical enforcement kept is the credential pre-commit hook,
because secrets are the one mistake that good intent doesn't prevent and that can't be undone after
the fact.

## Task record — `wiki/tasks/`

The fourth layer, introduced in § *The layers*. One file per task, `TASK-NNNN-short-name.md`, created
when work starts and kept after it ships. Full format and lifecycle in [[tasks/README|tasks]].

**This is the project's history and the primary record of it.** The wiki layer says what we *know*;
this says what we *did*, and it is the only register ordered by time — so *"what has been done on
this project"* is answered here, not in `log.md` and not by reading decision pages in sequence.

Rules:

- **Anything with a deliverable gets one** — a commit, a set of pages, a decision, a resolved design
  question. If nothing shipped, there is no file.
- **Link to deliverables; don't restate them.** The file points at the commit and the pages; it never
  summarises them. Longer than a screen means the content belongs in the wiki layer.
- **A task file links every wiki page its commits created or substantively rewrote.** Parentage is
  inbound-only — pages never cite task files — so *which task produced this page* can only be
  answered from the task side, and an unparented page is always a defect in the task file, never in
  the page.
- **`## Notes` is for what has no other home.** If a note explains *why* a choice was made, it
  belongs in a `decisions/` page; if it states a fact about the code or the design, it belongs in
  `systems/` or `design/`. Usually the section is empty, and that is the correct state.
- **While `in-progress` it is also the handoff note** — what's done, what's next. That section is
  **deleted on ship**; the deliverables speak for themselves.
- **Not a knowledge source.** Never cite a task file from a `systems/` or `design/` page. Task files
  point into the wiki, never the reverse.
- **Not a planner.** How work is intended to proceed belongs in [[planning/README|planning]];
  unsettled design questions belong wherever this project keeps its open-questions list, if it keeps
  one; unmapped code in [[index]] § *Biggest gaps*.

## Naming conventions

- Page files: `kebab-case.md`, named for the topic, not the date.
- Decision files: `DEC-NNNN-short-name.md`, numbered sequentially, never renumbered.
- Task files: `TASK-NNNN-short-name.md`, numbered sequentially, never renumbered.
- Log entries: one table row — `| YYYY-MM-DD | <op> | <title> | [[TASK-NNNN-slug]] |`. No body.
- Raw sources: `wiki/raw/<category>/<YYYY-MM>-<slug>.md` — date-stamped, because provenance matters.

## Project-specific vocabulary

The words already in this tree, each bound to one folder. **No new vocabulary is invented for the
wiki's sake**, and Unity's own words — *component*, *prefab*, *scene*, *asset*, *package*, *Play
Mode* — keep their Unity meanings and get no wiki meaning of their own.

| Term | What it is here | Owned by |
|---|---|---|
| **State** / **StateResponse** | The player state machine: `StateMachineBusiness<T>` with named states registered in `PlayerStateMachineBusiness.InitializeStates()` (`idle`, `running`, `jumping`, `airborne`, `changingLane`, `chargingDash`, `dashing`), and `*Response` classes bound to input events — `Assets/Scripts/BaseStateMachine/`, `Assets/Scripts/StateMachineImplementation/` | `systems/` |
| **…Object** (`GameEventObject`, `FloatObject`, `LevelObject`, `LevelPartObject`, `GameplayVariablesObject`) | This project's suffix for a `ScriptableObject` asset type — `Assets/Scripts/ScriptableObjects/`. Tuning values and events live in these assets rather than in scene components | `systems/` |
| **Level part** | A `LevelPartObject` the level is assembled from as the player advances (`Assets/Scripts/LevelManager.cs:20`) | `systems/` for how it works · `design/` for what a level should feel like |
| **Lane** | The lateral position the player changes between — `changeLaneAction`, `ChangingLanePlayerState`, `PlayerChangeLaneResponse` | `design/` for the rule · `systems/` for the implementation |
| **Bridge** / **bridge command** | The unity-bridge file channel and its JSON commands, driven by `unity-cmd.ps1` | `systems/` |
| **Scene** | A Unity scene asset — `Assets/Scenes/Menu.unity`, `Assets/Scenes/Cena1.unity`. *Cena* is Portuguese for *scene*; the tree mixes English and Portuguese names and **existing names are kept as they are**, not translated | `systems/` |

**A wiki page is never called a scene, and a Unity scene is never called a page.** That is the only
collision the two vocabularies have.

There is no architecture overview page yet — [[index]] § *Biggest gaps* is the honest state of the
map, and `systems/` is empty until something is actually read.
