---
type: task
status: in-progress
started: 2026-08-13
deliverables: [SCHEMA, index, log, configuring-a-subagent, parallel-coding-fan-out, mapping-what-you-built, handing-off-a-session, verifying-a-milestone-plan]
---

# TASK-0001 — Deploy the brain-os method

## Goal

Install the brain-os LLM-wiki method into this repository, so that every future session works to the
same rules: the wiki layers and what each owns, the three operations, the hard rules, and the
delegation practice. Deployed from the method repository at **version 0.1.0**
(`.claude/skills/brain-os/.claude-plugin/plugin.json`).

The method arrives as a **vendored** plugin rather than a per-project invention — the point is that a
rule which turns out to be wrong here is wrong everywhere, and goes back upstream instead of being
edited in place.

## Deliverables

**Vendored, byte-for-byte from the method repository** — listed in
`.claude/skills/brain-os/.manifest.sha256`, and a local edit to any of it is drift:

- The method plugin at `.claude/skills/brain-os/` — three operations, seven agents, the context hook
  and the tools.
- The credential pre-commit hook at `.githooks/pre-commit`.
- [[SCHEMA]] — the operating manual, with this project's eight slots filled at deploy.
- Five procedure pages: [[configuring-a-subagent]], [[parallel-coding-fan-out]],
  [[mapping-what-you-built]], [[handing-off-a-session]], [[verifying-a-milestone-plan]].

**This project's own**, a starting point it now owns and edits freely:

- The wiki shell — [[index]], [[log]] and the seven folder READMEs.
- `CLAUDE.md` at the repository root, the entry point that still loads when the plugin does not.

**Also changed:** `.gitattributes` gained an LF pin for the method's paths. The repo relies on
`core.autocrlf`, which is `true` here, so without the pin a fresh clone would get CRLF copies of
every vendored file and the manifest would report all of it as drift. The rule is scoped to
`.githooks/pre-commit`, `.claude/skills/brain-os/**` and `wiki/**/*.md`; Unity's assets, scripts and
settings are untouched by it.

**What was checked, and how:** the 20 unmodified vendored files were compared against the templates
by SHA-256 and are byte-identical; the two slot-filled ones differ only in their slots; every
deployed file is LF-only, UTF-8, no BOM; and `sha256sum -c .claude/skills/brain-os/.manifest.sha256`
was run from the repository root and passed. **Nothing here proves the plugin loads** — see
§ *State*.

## State

**The deploy wrote files; it did not install anything.** Two things remain, and neither can be done
from inside a session:

1. **`git config core.hooksPath .githooks`** — installs the credential scan. **Per-clone and
   per-developer**: it is not stored in the repository, so every clone needs it run once.
2. **Restart Claude Code at the repository root, then check that `/brain-os:ingest` and
   `brain-os:code-reader` are actually listed.** A project-scope plugin loads on the *next* session,
   only after the workspace trust dialog is accepted, and only when Claude Code starts at the
   repository root — it does not walk up from a subdirectory.

**If they are not listed, it is the trust gate far more often than it is this repository**, and a
closed gate shows no error at all — the commands are simply absent. `CLAUDE.md` § *If
`/brain-os:ingest` and `brain-os:code-reader` are not listed* is written for exactly that moment.
The two that cost the most time: the dialog **may never be presented** in some front-ends (the
VSCode extension among them — launching the CLI once from the repository root presents it), and on
Windows the `~/.claude.json` project key is **case-sensitive on the drive letter**, so `c:/Users/…`
and `C:/Users/…` can exist side by side with only one of them trusted. Both need
`hasTrustDialogAccepted: true`. That file is user-level configuration outside this repository and
Claude Code rewrites it on exit, so it is edited by hand between sessions, not from one.

**Then this task ships**: flip `status:` to `shipped`, add `shipped:`, and delete this section.

**Unverified beyond that:** nothing in the wiki has been checked against a running Editor. The
[[SCHEMA]] slots were written from `Packages/manifest.json`, `ProjectSettings/`, the shape of
`Assets/`, the unity-bridge package docs, and a partial read of `Assets/Scripts/` — not from
compiling the project or playing it.

## Notes

**The commit-to-`main` default holds here, and it was checked rather than assumed.** [[SCHEMA]]
§ *Commit to `main`* applies only while a project is pre-release and single-developer; `git shortlog`
shows one person under two identities (`Enrico Cavicchioli`, `EnricoCavic`), and `main` is the only
branch. If a second contributor starts committing concurrently, or the game is released, that
default stops applying and branching resumes being the right call.
