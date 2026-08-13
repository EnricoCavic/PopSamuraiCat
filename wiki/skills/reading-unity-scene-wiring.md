---
type: skill
status: verified
confidence: high
sources: [Assets/Scenes/Cena1.unity, Assets/Scenes/Menu.unity]
updated: 2026-08-13
---

# Reading Unity scene wiring without opening the Editor

**This project's own procedure**, not part of the vendored method.

Half of what a Unity system does is not in its `.cs` file. Serialized field values, which asset a
component points at, what a `UnityEvent` calls, and the object hierarchy all live in scene and
prefab YAML. Reading a script alone and asserting behaviour from it is how a confident wrong page
gets written ([[SCHEMA]] Hard Rule 2) — the tuned numbers in [[player-movement]] differ from the C#
field initialisers, and [[player-input]]'s dead `Move` binding is invisible from either file alone.

**Read-only.** Scenes, prefabs and `.meta` files are single-writer ([[SCHEMA]] § *Delegation*) —
grep them, never edit them.

## The indirection to know

Unity references assets by **GUID**, and the GUID lives in the asset's `.meta` file, not in the
asset. So `m_Script: {guid: c26479ef...}` in a scene means nothing until you map it back:

```bash
# GUID -> file
grep -rl "guid: <GUID>" Assets/ --include="*.meta"

# file -> GUID
grep '^guid:' Assets/Scripts/Foo.cs.meta
```

Build the whole script table once at the start of a pass and reuse it:

```bash
for f in $(find Assets/Scripts -name "*.cs.meta"); do
  echo "$(grep '^guid:' $f | cut -d' ' -f2) $(basename $f .cs.meta)"
done | sort > /tmp/guids.txt
```

## Four recipes

**Which scripts a scene actually uses** — the fastest way to tell live code from dead code, and the
first thing to run:

```bash
grep -o "guid: [a-f0-9]\{32\}" Assets/Scenes/Cena1.unity | cut -d' ' -f2 | sort | uniq -c \
  | while read c g; do n=$(grep "^$g " /tmp/guids.txt | cut -d' ' -f2); [ -n "$n" ] && echo "$c x $n"; done
```

**A component's serialized values** — the numbers that really run, which are *not* the C# field
initialisers once the component has been touched in the Inspector:

```bash
grep -n -A 25 "guid: $(grep '^guid:' Assets/Scripts/LevelManager.cs.meta | cut -d' ' -f2)" \
  Assets/Scenes/Cena1.unity
```

**The object hierarchy** — parse `!u!1` (GameObject) for names and `!u!4` (Transform) for
`m_GameObject` / `m_Father`, then walk from the roots. Worth a throwaway Python script; it is how
[[run-loop-and-scoring]] establishes that the kill trigger is a child of the camera rather than a
static object, which no `.cs` file states.

**`UnityEvent` targets** — `m_MethodName` and `m_StringArgument` in the listener's block give the
call and its argument, which is the only place the event-to-effect mapping in [[game-events]]
exists.

## Checks worth running once per pass

- **Is this asset referenced by anything?** `grep -rl "guid: <GUID>" Assets/ --include="*.unity"
  --include="*.prefab" --include="*.asset"`. This is what found `Highscore.asset` and seven level
  parts orphaned.
- **Is this field ever read?** `grep -rn "<fieldName>" Assets/Scripts/` — a hit only on the
  declaration and the asset YAML means the field is authored and dead, as `partDifficulty` is.

## The trap

A `.meta` file is the asset's identity. Deleting or hand-writing one changes the GUID and silently
breaks **every** reference to that asset across every scene and prefab. Read them; never write them,
and never move an asset outside the Editor.
