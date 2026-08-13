---
name: lint
description: Wiki health check — contradictions, stale claims, orphans, broken links, verifiable low-confidence claims
context: fork
# Namespaced, because the bare name appears not to resolve. Whether `agent:`
# resolves an unqualified in-plugin name is undocumented; the one observation
# available says it does not — a run with `agent: wiki-linter` forked into a
# thread holding `Write` and `Edit`, which `wiki-linter` does not declare.
# The symptom to watch for is exactly that: a fork whose tools are wider than
# `Read, Glob, Grep, Bash`. If this form fails to fork at all, the bare name is
# what to try instead.
agent: brain-os:wiki-linter
background: false
---

# /brain-os:lint

A health pass over `wiki/`. **This skill runs as the `wiki-linter` agent** — detection happens in a
forked context so that a large batch of files stays out of the main thread, and only the findings come
back. You detect and report. You do not fix, and you have no `Write` or `Edit` tool to do it with:
every resolution is the main thread's call, with the human in the loop.

Read `wiki/SCHEMA.md` first — § *Single source of truth* is what checks 7 and 8 are measured
against.

## Check for

1. **Contradictions** — two pages making incompatible claims. Report both with quotes; don't
   silently pick a winner.

2. **Stale claims** — pages whose `sources` files have changed since the page's `updated` date,
   or claims that no longer match the code. `git log --since=<updated> -- <source paths>` is the
   cheap way to find candidates.

3. **Orphans** — pages not listed in `wiki/index.md`, or that nothing links to. Either link them
   in or ask whether they should exist.

4. **Broken links — run the checker, don't read for them.** From the repository root:

   ```
   python .claude/skills/brain-os/tools/check-links.py
   ```

   It reports bare wikilinks that don't resolve, markdown links to missing files, anchors naming a
   heading that isn't there, `../` links inside `wiki/`, and wikilink names matching more than one
   file. Exit code 1 means something is broken.

   **Reading the output takes one distinction**, the same one `wiki/SCHEMA.md` § *The linking rule*
   draws. A bare `[[page]]` renders as a link; a backticked one renders as text. So:

   | Finding | What it is |
   |---|---|
   | A bare link to a page that could exist later | **A planned page** — intentional, it marks something worth writing. Report as a prompt, not an error |
   | A bare link to a worked example, format specifier or placeholder | **A defect.** It will never resolve, and it dangles in every vault. The fix is backticks, not a new page |
   | A markdown link, anchor or `../` finding | **A defect.** These have no intentional-dangle case |

   **Do not classify by name.** An allowlist of things that *look* like placeholders is how this
   check misses real defects — the names are the part you cannot trust, and a live link inside a
   worked example is exactly the shape that survives it.

   **Ambiguous names are reported but don't fail the run.** A repo that ships templates alongside
   their deployed copies has them by construction; what matters is whether a *reader* is pointed at
   the right half.

5. **Frontmatter** — missing or malformed `type` / `status` / `confidence` / `sources` /
   `updated`. **Scope this to `systems/`, `design/`, `planning/`, `decisions/` and `skills/`.** A naive
   repo-wide check reports a pile of false positives: `wiki/raw/` is human-owned and uses
   `type`/`source`/`captured`, `SCHEMA.md` uses `type: schema`, and `wiki/tasks/` uses
   `type`/`status`/`started`/`shipped`/`deliverables` per [[tasks/README]]. All three shapes are
   deliberate.

6. **Promotable claims** — `confidence: low` or `status: stub` pages where the underlying code
   could now be read cheaply. These are the highest-value lint findings: they're the wiki telling
   you what to ingest next.

7. **Overreach** — anything asserted as fact that the `sources` don't support. This is the most
   important check. Verified-sounding invention is the one failure mode that compounds, because
   every future session inherits it. **Attributions to a raw source count**: "the human ruled X"
   is as checkable as a `file:line`, and re-reading the numbered answer is the check.

8. **Duplication — facts living outside the folder that owns them.** The second most important
   check, and the reason lint passes get expensive. For each finding: name the **owner**
   (`wiki/SCHEMA.md` § *Single source of truth*), and cut the copies down to a link plus one
   clause. Common shapes:
   - a `decisions/` page restating the design it produced, instead of linking the `design/` page
   - a `design/` page rehearsing the alternatives that were rejected
   - a `design/` page carrying build order or cost, or a `planning/` page restating the design it
     sequences or absorbing the account of work already shipped (that is the task file)
   - a task file's `## Notes` explaining *why*, which is a decision page's job
   - a `log.md` entry with a body
   - a folder `README.md` listing its own contents — `index.md` is the only catalog
   - the same count or status quoted on three pages

   **Find these mechanically, don't read for them.** Normalise every page under `wiki/` (strip
   frontmatter, code fences, markdown emphasis; resolve `[[a|b]]` to its text), split into words,
   hash every 9-word window, and report windows appearing in two or more files — then merge
   overlapping windows into runs and sort by length. The long runs are the real findings; 9-word
   hits are often shared boilerplate. This is minutes of compute against hours of reading. Exclude
   `wiki/raw/` — the quotes there are the originals every page is copying *from*.

9. **Index accuracy** — does `wiki/index.md` reflect reality, including the "Biggest gaps"
   section?

10. **Unpromoted repetition** — repeated work that nothing has captured.
    `wiki/SCHEMA.md` Hard Rule 6 has two halves and both are checkable from `wiki/tasks/`:
    - **Procedures** — the same multi-step sequence described in two or more task files with no
      `wiki/skills/` page. Write the skill.
    - **Worker shapes** — the same kind of agent specified in prose in two or more task files with
      no definition anywhere in `.claude/`. Write the definition ([[configuring-a-subagent]]) — in
      **this project's own `.claude/agents/`**, never inside the vendored method subtree, which is
      the distinction check 11 exists to hold.

    Hard Rule 6 is a checkpoint at task close, not a count, so also flag **a task file that
    doesn't record its answer** — that is the rule failing silently.
    Also flag the reverse, across both agent locations: a definition no task file has used in
    months, and a definition with no `model:` — an unpinned agent inherits the main thread's model,
    which is the default this rule exists to prevent.

11. **Vendored drift — run the manifest, don't read for it.** The method wrote
    `.claude/skills/brain-os/.manifest.sha256` at deploy: one `sha256sum`-format line per vendored
    file, hashed **as deployed**, so a mismatch means *edited since the method landed* rather than
    *differs from some template you cannot see*. From the repository root:

    ```
    sha256sum -c .claude/skills/brain-os/.manifest.sha256
    ```

    Read the result in three parts, because the command only catches the first two:

    | Symptom | What it means | Report it as |
    |---|---|---|
    | `FAILED` | a vendored file was edited here | **drift** — the highest-severity finding this check produces |
    | `No such file` / `FAILED open or read` | a vendored file was deleted or moved | **drift** |
    | A file under a vendored path with **no line in the manifest** | something was *added* to the method's territory | **drift**, and the one `-c` cannot see |

    That third case needs its own pass: list every file under `.claude/skills/brain-os/` (excluding
    the manifest itself), compare against the paths the manifest names, and report the extras. An
    added agent is the common shape, and it belongs in the project's own `.claude/agents/`.

    **Every drift finding is a proposal to move the change upstream, never to revert it.** Someone
    edited that file for a reason, and a method rule that is wrong here is wrong everywhere. Quote
    what changed (`git log --follow -p -- <path>` usually has it) and say so; do not restore the
    file, and do not re-hash the manifest to make the finding disappear — that destroys the evidence
    and the check in one move.

    **If the manifest is missing entirely, that is itself the finding.** Say so and stop this check:
    without it nothing distinguishes a vendored file from a project one, and the table below is the
    only remaining guide.

    **If `sha256sum` is unavailable** (it ships with coreutils and is absent on a bare Windows
    shell), say the check did not run rather than eyeballing the files. `certutil -hashfile <path>
    SHA256` and PowerShell's `Get-FileHash` both work per-file if you need a specific answer, but a
    partial check reported as a complete one is worse than an honest skip.

    The boundary the manifest encodes, for reading findings against:

    | Location | What it is | Editing it is |
    |---|---|---|
    | `.claude/skills/brain-os/**`, `.githooks/pre-commit`, `wiki/SCHEMA.md`, and the five method pages in `wiki/skills/` | the vendored method | **drift**, and reported as such |
    | `.claude/agents/`, `.claude/skills/<other>/`, `.claude/settings.json`, every other page in `wiki/` | this project's own | normal |

    Then, outside the manifest's reach: flag a skill or agent file in either location that
    `git check-ignore` says is untracked, and any `SKILL.md` missing `name` or `description`.

    **Also reconcile `CLAUDE.md`'s agent list against the frontmatter of every agent definition in
    both locations** — every agent listed, no agent missing, and each stated tier matching its
    actual `model:`. This is the one deliberate second copy of the roster, so it is worth checking
    mechanically rather than by eye. Flag too any `effort:` on a Haiku-pinned agent — that
    combination errors at launch ([[configuring-a-subagent]] § *Gotchas*).

## Output

Group findings by severity. Lead with overreach and contradictions, then duplication; leave
formatting nits last. For each: the page, the problem, and a concrete fix **stated as a proposal**.

Close with a **Clean** list (checks that found nothing, so the caller knows the pass was complete)
and a **Not checked** list (what you ran out of budget for, or what needs a judgement you weren't
asked to make).

## Hand back

You cannot write files, so end your report with this block verbatim, for the main thread:

> **Next, in the main thread:** resolve each finding with the human — decide what it means and what
> the page should say instead, and **ask before** anything judgement-heavy (deleting a page, merging
> two, or picking a winner in a contradiction where both sides are plausible). **Deciding is the
> half that does not delegate.**
>
> **Then hand the agreed writes to `brain-os:wiki-writer`** rather than typing them here. A pass
> that returns a dozen findings is a dozen edits, and making them in the main thread spends on
> writing the context this fork just saved on reading. Slice **by file** — one agent per group of
> pages, no page in two slices — and give each agent the *resolved text*, not the finding. State the
> count, the agent and the split, and wait for a go-ahead: agreeing the fixes does not pre-authorize
> the fleet that applies them.
>
> **`wiki/index.md` and `wiki/log.md` stay in the main thread**, edited serially. Every pass appends
> to both, so they are the two files a fan-out cannot safely hold.
>
> Then open a task file for the pass and append **one line** to `wiki/log.md`:
> `| YYYY-MM-DD | lint | <summary> | [[TASK-NNNN-slug]] |`. What was checked and fixed goes in the
> task file's deliverables, not in the log.
