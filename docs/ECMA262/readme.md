# ECMA-262 Docs Workflow (docs/ECMA262)

This directory contains JS2IL’s **ECMA-262 coverage index** and per-section “hub” documents.

Important conventions
- These markdown files are **indexes only**: clause numbers, titles, status, and links.
- They intentionally **do not copy spec text** into this repo.
- Status values come from the project’s feature coverage tracking (see docs/ECMA262/Index.md).

Exception
- If explicitly requested, a section file may include an **appendix** containing converted/extracted spec text as a derived artifact.

## How section docs are generated and maintained

There are two related workflows:

### 1) Split/rollup generation (automated)

Scripts
- scripts/splitEcma262SectionsIntoSubsections.js (primary)
- scripts/splitEcma262SectionsIntoSubsections.ps1 (wrapper)

What it does
- Generates/updates subsection documents (e.g. Section27_3.md) from a parent section’s clause table when applicable.
- Rebuilds hub pages and updates Index.md rollups.

When to use it
- After updating the top-level section tables / coverage metadata and you want the subsection docs and rollups refreshed.

### 2) “Structural sync” of a subsection file (manual, links-only)

Sometimes a subsection document exists only as a **stub** (single row) and needs its **subclause structure** filled in so it mirrors the current ECMA-262 clause layout (still links-only).

We do that by extracting the authoritative clause ids/titles from tc39.es and then updating the corresponding markdown table.

Tooling used
- scripts/extractEcma262SectionHtml.js

Steps
1. Extract the HTML for the section you want to sync (auto-discovery mode):
   - node scripts/extractEcma262SectionHtml.js --section 27.6 --auto --no-wrap --out test_output/ecma262-27.6.html
2. From the extracted HTML, identify each subclause’s:
   - Section number (from <span class="secnum">…)
   - Title (from the <h1> text)
   - Anchor id (from <emu-clause id="…">)
3. Update the corresponding docs/ECMA262/SectionXX_Y.md file:
   - Keep it **links-only**.
   - Add a “## Subclauses” table with columns: Clause | Title | Status | Spec.
   - For the Spec link, use the extracted id:
     - https://tc39.es/ecma262/#<id>

Notes
- Prefer copying the title text as rendered in the heading (including %…% and %Symbol.toStringTag% forms).
- The extracted HTML is a scratch artifact; it does not need to be committed.

## Repo-local linking

Other docs in this repo may link to these indexes using repo-relative paths under docs/ECMA262/ (instead of linking directly to tc39.es). This keeps cross-references stable and reviewable.
