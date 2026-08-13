---
type: raw
updated: 2026-08-13
---

# Raw Sources

**Human-owned. The LLM never edits or deletes anything in this folder** — with two exceptions, both
narrow, both in [[SCHEMA]] Hard Rule 1: it may **append** a faithful transcription of something the
human said, and it maintains **this file**, which is the index of the folder's contents.

This is where context for the models gets dropped: meeting notes, transcribed conversation, design
docs, exports from other tools, external references. **It does not need to be tidy** — tidying is
the LLM's job, and the tidied version lives in the wiki layer, not here.

**Transcription is a normal working tool, not a last resort.** When the human states substantive new
material in conversation rather than settling a specific question, that is a raw source arriving by
voice: transcribe it here first, dated and clearly marked, then synthesize from it. Most of this
folder should be expected to arrive that way.

**Verbatim quotes belong here.** A wiki page may quote the single clause its own claim turns on; the
full block stays in this folder and every other page links to it. Otherwise the human withdraws his
own wording and every copy becomes a page that now misquotes him.

**Layout:** `wiki/raw/<category>/<YYYY-MM>-<slug>.md` — date-stamped, because provenance matters.
`meetings/`, `feedback/` and `references/` are the usual categories; add one when a source does not
fit, and record it below.

## Current contents

| File | What it is |
|---|---|
| *(nothing yet)* | |

## Not in this folder

**The codebase itself is also a raw source** — the project's own source tree is ground truth, and it
is not copied here. Wiki pages describe it and cite it as `path/to/file.ext:LINE`.
