---
name: ingest
description: Ingest a new source into the wiki — read it, discuss it, write pages, update index and log
argument-hint: <path, URL, or topic>
---

# /brain-os:ingest

Read `wiki/SCHEMA.md` first if you haven't this session.

Ingest: **$ARGUMENTS**

The source may be a file in `wiki/raw/`, a path in the codebase, a URL, or a topic to investigate.
If it's a URL or external material, save a copy into `wiki/raw/` first — raw sources are kept
permanently.

## Steps

1. **Read it properly.** Not skim. If it's a code system, read the actual source files, not just
   filenames. Whatever you don't read, you don't get to assert.

2. **Discuss the takeaways before writing.** Tell the user what you found — especially anything
   surprising, anything that contradicts an existing page, and anything that changes what they
   should do next. Let them redirect emphasis before you commit it to pages.

3. **Write or update pages — each fact in the one folder that owns it.** `systems/` = what the code
   is · `design/` = what the product is meant to be · `planning/` = how we intend to get there ·
   `decisions/` = why it changed · `tasks/` = what we did.
   One topic, one page: check `wiki/index.md` before creating. Frontmatter is mandatory:
   `type`, `status`, `confidence`, `sources`, `updated`. Cite code as `path/to/file.ext:LINE`.

4. **Follow the links.** Revisit every page that links to what you changed. Stale cross-references
   are the main way this wiki rots. Fix them in the same pass — and where a page *restates* what it
   links to, cut the restatement rather than updating it twice.

5. **Update `wiki/index.md`** — add new pages, update summaries and confidence markers for changed
   ones, and revise the "Biggest gaps" section if this pass closed or opened one.

6. **Open or update the task file**, then append **one line** to `wiki/log.md`:
   `| YYYY-MM-DD | ingest | <title> | [[TASK-NNNN-slug]] |`. No body. What you read versus inferred
   goes on the pages themselves; what shipped goes in the task file.

7. **Close on the repetition checkpoint.** Before reporting done, ask the two questions in
   `wiki/SCHEMA.md` Hard Rule 6: did this task repeat a *procedure* that has no `wiki/skills/`
   page, or a *worker shape* that has no agent definition? If so, write it now — in this project's
   own `.claude/agents/`, not inside the vendored method subtree. Say so
   either way — "nothing repeated" is a valid answer and a checked one.

## Rules

- **Never edit `wiki/raw/`.** If a raw source is wrong, say so in the wiki layer.
- **Link, don't restate.** One clause of gloss at a link, never the reasoning behind it.
  `wiki/SCHEMA.md` § *The linking rule*.
- Mark honestly: `status: verified` only if you read the code. `confidence: low` is fine and
  useful; a confident wrong page is not.
- If the source contradicts an existing page, **surface it to the user** rather than silently
  picking a winner.
- **A claim attributed to a raw source is as checkable as a `file:line`** — re-read the source
  before writing "the human said X". An inference presented as a ruling is Hard Rule 2's failure.
- Don't claim anything was tested unless it actually ran — `wiki/SCHEMA.md` Hard Rule 3.
- **Subagents report back; you synthesize.** You decide what each page says; agents may write the
  agreed result. State count, model and split, then wait for a go-ahead before launching any.
  `wiki/SCHEMA.md` § *Delegation*.
