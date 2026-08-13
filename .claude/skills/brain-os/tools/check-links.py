#!/usr/bin/env python3
"""Mechanical link check over a brain-os wiki.

Run from the repository root:

    python .claude/skills/brain-os/tools/check-links.py

Exit code is 0 when nothing is broken, 1 otherwise, so it can gate a commit.

WHAT IT CHECKS, and why each check exists:

  1. Bare wikilinks that do not resolve. A bare `[[page]]` renders as a link and
     dangles; a backticked one renders as text and is inert. Fenced blocks and
     inline code spans are stripped first, so worked examples and format
     specifiers are correctly ignored. Classifying by *name* instead -- an
     allowlist of things that look like placeholders -- is the version of this
     check that misses real defects.
  2. Relative markdown links whose target does not exist on disk.
  3. Markdown anchors (`file.md#section`) naming a heading that is not there.
  4. Markdown links inside wiki/ that start with `../`. Where the wiki is opened
     as a vault rooted at wiki/, these render as nonexistent files even when they
     are correct on disk.
  5. Wikilink names that match more than one file. Ambiguous in any vault; the
     resolution is whichever file the editor happens to pick.

A file under a directory named `templates/` is a special case: its relative
markdown links resolve against wherever it is *deployed*, not where it lives, so
checking them here would report every one as broken. They are listed separately
as unresolved-in-place rather than counted as failures. The only real check for
those is to build the deployed tree and run this against that.

Nothing here is fixed automatically. Every finding is reported for a human.
"""
import re
import sys
from collections import defaultdict
from pathlib import Path

# Directories never scanned: version control, dependencies, build output.
SKIP_DIRS = {".git", "node_modules", "__pycache__", ".venv", "venv",
             "dist", "build", "target", ".obsidian"}


def find_markdown(root: Path):
    for p in sorted(root.rglob("*.md")):
        if any(part in SKIP_DIRS for part in p.parts):
            continue
        yield p


def blank(m):
    """Replace a matched span with its newlines, so line numbers survive."""
    return "\n" * m.group(0).count("\n")


def strip_code(text: str) -> str:
    """Remove fenced blocks and inline spans; keep line numbering intact."""
    text = re.sub(r"```.*?```", blank, text, flags=re.S)
    text = re.sub(r"~~~.*?~~~", blank, text, flags=re.S)
    text = re.sub(r"`[^`\n]*`", "", text)
    return text


def slugify(heading: str) -> str:
    s = heading.strip().lower()
    s = re.sub(r"[^\w\s-]", "", s)
    return re.sub(r"\s+", "-", s)


def headings_of(path: Path) -> set:
    out = set()
    try:
        for line in path.read_text(encoding="utf-8", errors="replace").splitlines():
            m = re.match(r"^(#{1,6})\s+(.*)", line)
            if m:
                out.add(slugify(m.group(2)))
    except OSError:
        pass
    return out


WIKILINK = re.compile(r"\[\[([^\]]+?)\]\]")
MDLINK = re.compile(r"(?<!!)\[([^\]]*)\]\(([^)\s]+?)(?:\s+\"[^\"]*\")?\)")


def wikilink_target(raw: str) -> str:
    """[[a/b#anchor|Label]] and [[a/b\\|Label]] both reduce to 'a/b'."""
    return raw.split("|")[0].split("#")[0].rstrip("\\").strip()


def main() -> int:
    repo = Path(sys.argv[1] if len(sys.argv) > 1 else ".").resolve()
    wiki = repo / "wiki"
    if not wiki.is_dir():
        print(f"No wiki/ directory under {repo} -- nothing to check.")
        return 0

    # A wikilink resolves against any .md under wiki/, by basename or by
    # folder-relative path ([[tasks/README]]).
    by_stem = defaultdict(list)
    by_relpath = {}
    for p in find_markdown(wiki):
        rel = p.relative_to(wiki).as_posix()
        by_stem[p.stem].append(rel)
        by_relpath[rel[:-3]] = rel

    scan = list(find_markdown(repo))

    broken_wiki, broken_md, broken_anchor, dotdot = [], [], [], []
    template_md = []
    heading_cache = {}

    for f in scan:
        rel_f = f.relative_to(repo).as_posix()
        in_wiki = rel_f.startswith("wiki/")
        # See the module docstring: a template's relative links target its
        # deployed location, which is not where the file sits.
        is_template = "templates" in f.relative_to(repo).parts
        raw = f.read_text(encoding="utf-8", errors="replace")
        text = strip_code(raw)

        for m in WIKILINK.finditer(text):
            target = wikilink_target(m.group(1))
            line = text.count("\n", 0, m.start()) + 1
            if "/" in target:
                ok = target in by_relpath or target.split("/")[-1] in by_stem
            else:
                ok = target in by_stem
            if not ok:
                broken_wiki.append((rel_f, line, target))

        for m in MDLINK.finditer(text):
            href = m.group(2)
            line = text.count("\n", 0, m.start()) + 1
            if re.match(r"^(https?:|mailto:|#)", href):
                continue
            path_part, _, anchor = href.partition("#")
            if not path_part:
                continue
            if in_wiki and path_part.startswith("../"):
                dotdot.append((rel_f, line, href))
            target = f.parent / path_part
            if not target.exists():
                (template_md if is_template else broken_md).append((rel_f, line, href))
            elif anchor and target.suffix == ".md":
                if target not in heading_cache:
                    heading_cache[target] = headings_of(target)
                if anchor.lower() not in heading_cache[target]:
                    broken_anchor.append((rel_f, line, href))

    ambiguous = {k: v for k, v in by_stem.items() if len(v) > 1}

    def report(title, rows, fmt):
        print(f"\n=== {title}: {len(rows)} ===")
        for r in rows:
            print("  " + fmt(r))

    report("BARE WIKILINKS THAT DO NOT RESOLVE", broken_wiki,
           lambda r: f"{r[0]}:{r[1]}  [[{r[2]}]]")
    report("MARKDOWN LINKS TO MISSING FILES", broken_md,
           lambda r: f"{r[0]}:{r[1]}  ({r[2]})")
    report("MARKDOWN ANCHORS WITH NO SUCH HEADING", broken_anchor,
           lambda r: f"{r[0]}:{r[1]}  ({r[2]})")
    report("'../' LINKS INSIDE wiki/", dotdot,
           lambda r: f"{r[0]}:{r[1]}  ({r[2]})")

    if template_md:
        report("IN TEMPLATES - unresolved in place, not counted as failures",
               template_md, lambda r: f"{r[0]}:{r[1]}  ({r[2]})")
        print("  (these resolve where the template deploys to; build the"
              " deployed tree and check that instead)")

    print(f"\n=== AMBIGUOUS WIKILINK NAMES: {len(ambiguous)} ===")
    for name in sorted(ambiguous):
        print(f"  [[{name}]] matches {len(ambiguous[name])} files:")
        for rel in sorted(ambiguous[name]):
            print(f"       wiki/{rel}")

    total = len(broken_wiki) + len(broken_md) + len(broken_anchor) + len(dotdot)
    print(f"\nScanned {len(scan)} markdown files against {len(by_stem)} page names.")
    print(f"{total} broken link(s); {len(ambiguous)} ambiguous name(s).")

    # Ambiguity is a warning, not a failure -- a repo that ships templates
    # alongside their deployed copies has it by construction.
    return 1 if total else 0


if __name__ == "__main__":
    raise SystemExit(main())
