---
name: query
description: Answer a question against the wiki, then file the answer back as a page if it was worth the work
argument-hint: <question>
---

# /brain-os:query

Read `wiki/SCHEMA.md` first if you haven't this session.

Question: **$ARGUMENTS**

## Steps

1. **Search the wiki first.** Start at `wiki/index.md`, then read the relevant pages. This is the
   whole reason the wiki exists — don't re-derive from the codebase what's already written down.

2. **Check confidence.** If the pages that answer the question are marked `confidence: low` or
   `status: stub`, say so. Then go to the code and verify before answering, rather than passing
   along an inference as fact.

3. **Fall back to the code** for anything the wiki doesn't cover. The codebase is ground truth.

4. **Answer with citations.** Link pages as `[[page-name]]` and code as `path/to/file.ext:LINE`.
   Distinguish what you verified now from what you're relying on a page for.

5. **File it back — into the folder that owns it.** If the answer took real work (reading several
   files, tracing a system, resolving a contradiction), write it into the wiki: a fact about the
   code goes in `systems/`, a fact about the design goes in `design/`. Extend an existing page
   before creating one. Then update `wiki/index.md` and append **one line** to `wiki/log.md`:
   `| YYYY-MM-DD | query | <question> | [[TASK-NNNN-slug]] |`.

   If it was a one-line lookup, don't. Not every question deserves a page, and a wiki full of
   trivia is worse than a small dense one — in that case there is no task file and no log line.

## Rules

- **A good answer that only lives in chat is a wasted answer.** That's the failure this system
  exists to prevent.
- **Write it once.** If the answer belongs on a page that already exists, extend that page — don't
  add a second account of it elsewhere and link between them.
- If the wiki and the code disagree, **the code wins** — and fix the page in the same turn.
- If you can't answer confidently, say so and name what you'd need to read.
- **Close on the repetition checkpoint** — did answering repeat a procedure with no skill page, or a
  worker shape with no agent definition? Write it before reporting (`wiki/SCHEMA.md` Hard Rule 6).
  A query that files no page can still owe a skill.
- **Subagents report back; you synthesize.** You decide what the answer is; agents may write it up.
  State count, model and split, then wait for a go-ahead before launching any.
  `wiki/SCHEMA.md` § *Delegation*.
