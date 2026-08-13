---
type: skill
status: partial
confidence: medium
sources: []
updated: 2026-08-12
---

# Mapping what you built

*Vendored from the method — edit it upstream, not here.*

**The closing step of every implementation task.** The code exists, it runs — and the project map
still doesn't know it is there, or still describes an earlier version of it. This is how that gets
closed, in the same task.

## When it fires

At task close, before reporting done. The question is **not** *"is this file mapped?"* but **"what
does the wiki now say that is wrong or incomplete?"**

- **New system** → new page.
- **Changed system** → update the page that owns it. Already having a page is not coverage. A
  structural change to mapped code leaves a page that is *confidently false*, which is worse than an
  unmapped system — a future session has no reason to doubt it.
- **A change no page makes a claim about** → nothing.

## The five moves

### 1. Audit mechanically before reading anything

Do not ask an agent whether the map is complete — the question is a version-control diff and a
search, so it gets an exact answer instead of an impression.

For each commit the task shipped: list the files it touched, keep the ones under the project's source
directories, and search the systems pages for each filename.

This produces two work lists, and the second is the one that gets forgotten:

| Result | What it means | What you do |
|---|---|---|
| **Uncited** | Nothing describes this code | Write the page |
| **Cited** | A page makes claims about code you just changed | **Re-read that page against the change.** It is now either current or confidently wrong |

The cited list is not a pass mark. "Zero uncited" only ever means someone had written about these
files, not that what they wrote is still true. A page can describe a component that a later task gave
a materially different second responsibility, and the page will say nothing about it until someone
re-reads it against the change.

**Mention is not coverage**, in both directions. A filename search tests whether a name appears
anywhere, so a file named once in a passing note scores as cited. Read the hit before believing it
either way.

### 2. Decide new page or extension — this is the part that does not delegate

A new subsystem gets its own page. An extension updates the page that owns the area. Getting this
wrong forks a topic across two pages, which the wiki's single-source-of-truth rule exists to prevent.

Watch for name collisions where the wiki's link syntax resolves by filename rather than by path — a
`systems/` page and a `design/` page should not share a name, or one becomes unreachable. Convention:
the design page owns the plain noun, the systems page takes the mechanism.

### 3. Split the read on files, not on features

Same slicing rule as [[parallel-coding-fan-out]], and for the same reason: disjoint file sets, no two
agents in one file. The natural cut is by layer — data or model logic versus presentation is a common
one — and a judgement-reading agent is the right worker: it judges what the code *means*, which a
mechanical extraction agent deliberately does not.

Bound the return: what sections, roughly how long. A short brief costing far less than the code it
replaces is the whole justification for the fan-out.

### 4. Verify the load-bearing lines here, before writing them down

An agent's `file:line` is a claim the page is about to make permanent. Re-read the two or three that
would change what the page says — the surprising ones, the ones that contradict what the build task
recorded, and any defect about to be published. This costs one read and it is the only real defence
against a confidently wrong page.

### 5. Write the page, then close the loop around it

The page owes: a **file inventory** so the next session knows where the system lives, the
**contract** between its parts, a **defects table** with `file:line`, and an explicit **"not
confirmed"** section. It does not restate the design — it links.

Then, in the same pass: add the row to the project's index, delete the gap entry the page closes, add
any new defects found to the index's defects list, log the write-up, and list the page in the task
file's deliverables.

## What this catches that reading the plan does not

The read is done with the code fresh, which is the cheapest moment it will ever be — but it is also
the first time anyone reads the built code **as a system**, rather than as separate slices reviewed
against their own launch briefs. That reading routinely turns up things no slice review had: a check
written for the new feature that doesn't assert what an equivalent older check does, a function
documented as read-only that in fact mutates state, an edge case that only one direction of a two-way
sync handles. None of these are visible from the plan, the commits, or a single clean run.

## Related

[[parallel-coding-fan-out]] · [[verifying-a-milestone-plan]] · [[configuring-a-subagent]]
