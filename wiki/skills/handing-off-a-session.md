---
type: skill
status: partial
confidence: medium
sources: []
updated: 2026-08-12
---

# Handing off a session

*Vendored from the method — edit it upstream, not here.*

**When to use.** The context-ceiling check has fired, the work is bigger than the context left, or
the task is finished and the next one is different. Any of the three ends the session the same way.

**The premise.** Work is registered **as it lands**, not banked until the end. Anything that exists
only in the conversation is lost at the next context clear.

## Register as you go — the four points during a task

Not a closing ritual. These happen while the work is live:

1. **The task file is created when work starts**, not when it ships. It is the handoff note for the
   whole time it says in-progress.
2. **A page is written the moment its content is settled**, not after the last one is. A decided page
   held only in the conversation is an unregistered deliverable.
3. **Commit at each landed piece.** A commit is the only record that survives everything.
4. **The task's `## State` section is updated whenever "next" changes** — not reconstructed at the end
   from memory that is about to be discarded.

## The handoff itself

1. **Land the current piece or abandon it cleanly.** Do not start a new one. A half-written page is
   worse than an absent one, because the next session will trust it.
2. **Write `## State` so it starts cold.** The test: *could a session that has never seen this
   conversation continue from these lines alone?* Name files by path, decisions by number, and say
   what the next action is — not "continue the work".
3. **Update the project's index and append the log row** for what actually shipped. An operation that
   happened but was never logged is invisible to every future session.
4. **Commit**, with the task number in the message.
5. **Say in chat what was left undone** and tell the human to clear the session. The report is the
   last thing the context does.

## Checking where you are

The number that matters is the current conversation's size against the project's stated ceiling
([[SCHEMA]] § *Register work as it lands*), plus how much of this session's output came from the main
thread versus from subagents. Whatever mechanism a deployment wires for this should read the session
transcript directly, so it is a measurement, not an estimate. If it's wired as a hook that only prints
past the ceiling, treat silence as "below it" — check by hand if that's ever in doubt.

## What the numbers do not include

**Account-level usage is not readable from a session transcript.** Daily and weekly spend, and the
split across models, live wherever the model provider's own usage reporting lives. If they matter to
a decision, ask the human to paste them.

## The cheapest way to not need this

- **One wiki operation per session.**
- **Delegate reading and coding**, which are what actually fill a context — a subagent's context is
  discarded when it returns, and only its report is charged to this one.
- **Bound what an agent returns.** An unbounded fan-out spends the ceiling on prose.
- **Do not re-read what you have already read.** The transcript is the cache; re-reading a file to
  check a detail already established pays for it twice.

## Related

[[parallel-coding-fan-out]] · [[configuring-a-subagent]] · [[mapping-what-you-built]]
