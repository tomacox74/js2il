# ECMA-262 Docs Workflow (docs/ECMA262)

This directory contains JS2IL’s **ECMA-262 coverage index** and per-section “hub” documents.

Important conventions
- These markdown files are **indexes only**: clause numbers, titles, status, and links.
- They intentionally **do not copy spec text** into this repo.
- Status values come from the project’s feature coverage tracking (see docs/ECMA262/Index.md).

Exception
- If explicitly requested, a section file may include an **appendix** containing converted/extracted spec text as a derived artifact.

## Tooling location

ECMA-262-related scripts live under:
- scripts/ECMA262/

(Other scripts under scripts/ are unrelated to ECMA-262 docs.)

## How section docs are generated and maintained

There are two related workflows:

### 1) Split/rollup generation (automated)

Scripts
- scripts/ECMA262/splitEcma262SectionsIntoSubsections.js (primary)
- scripts/ECMA262/splitEcma262SectionsIntoSubsections.ps1 (wrapper)

What it does
- Generates/updates subsection documents (e.g. 27/Section27_3.md) from a parent section’s clause table when applicable.
- Rebuilds hub pages and updates Index.md rollups.

When to use it
- After updating the top-level section tables / coverage metadata and you want the subsection docs and rollups refreshed.

How to run
- npm: `npm run generate:ecma262-subsections`
- direct: `node scripts/ECMA262/splitEcma262SectionsIntoSubsections.js`

### 2) “Structural sync” of a subsection file (manual, links-only)

Sometimes a subsection document exists only as a **stub** (single row) and needs its **subclause structure** filled in so it mirrors the current ECMA-262 clause layout (still links-only).

We do that by extracting the authoritative clause ids/titles from tc39.es and then updating the corresponding markdown table.

Tooling used
- scripts/ECMA262/extractEcma262SectionHtml.js
- scripts/ECMA262/convertEcmaExtractHtmlToMarkdown.js (optional, for embedding converted text)

Steps
1. Extract the HTML for the section you want to sync (auto-discovery mode):
   - node scripts/ECMA262/extractEcma262SectionHtml.js --section 27.6 --auto --no-wrap --out test_output/ecma262-27.6.html
2. From the extracted HTML, identify each subclause’s:
   - Section number (from <span class="secnum">…)
   - Title (from the <h1> text)
   - Anchor id (from <emu-clause id="…">)
3. Update the corresponding docs/ECMA262/XX/SectionXX_Y.md file:
   - Keep it **links-only**.
   - Add a “## Subclauses” table with columns: Clause | Title | Status | Spec.
   - For the Spec link, use the extracted id:
     - https://tc39.es/ecma262/#<id>

Notes
- Prefer copying the title text as rendered in the heading (including %…% and %Symbol.toStringTag% forms).
- The extracted HTML is a scratch artifact; it does not need to be committed.

## JSON helpers (subsection metadata)

Subsection markdown files (e.g. docs/ECMA262/27/Section27_1.md) have sibling JSON files
(e.g. docs/ECMA262/27/Section27_1.json) which track clause/title/status/specUrl and (optionally)
`subclauses`.

### Backfill missing JSON files (safe)

If a markdown subsection doc exists but its JSON is missing, you can generate the missing JSON files
without overwriting anything:

- script: scripts/ECMA262/generateMissingEcma262SectionJson.js
- dry-run: `node scripts/ECMA262/generateMissingEcma262SectionJson.js`
- write: `node scripts/ECMA262/generateMissingEcma262SectionJson.js --write`

### Sync subclause structure from tc39.es (safe)

Some subsection JSON files are stubs (no `subclauses`) or missing newer subclauses.
This script pulls the authoritative clause list (ids + titles) from https://tc39.es/ecma262/
and updates JSON subclauses.

- script: scripts/ECMA262/syncEcma262SubclausesFromSpec.js
- default mode is **missing-only** (append-only; never rewrites or removes existing items):
   - dry-run: `node scripts/ECMA262/syncEcma262SubclausesFromSpec.js`
   - write: `node scripts/ECMA262/syncEcma262SubclausesFromSpec.js --write`
- optional: fill blank title/specUrl fields on existing items (does not overwrite non-empty values):
   - `node scripts/ECMA262/syncEcma262SubclausesFromSpec.js --fill-blanks --write`

## Regenerating markdown from JSON

If you want to (re)generate a subsection markdown file from its JSON description:

- script: scripts/ECMA262/generateEcma262SectionMarkdown.js
- example: `node scripts/ECMA262/generateEcma262SectionMarkdown.js --section 27.3`

Note: by convention these docs are links-only. The generator supports an embedded reference appendix,
but that should only be used when explicitly requested.

## Feature coverage markdown

Feature-level support tracking lives alongside the subsection docs.

- Add/update support metadata under `support.entries` in the relevant subsection JSON file(s) (e.g. `docs/ECMA262/27/Section27_1.json`).
- Regenerate the subsection markdown using `node scripts/ECMA262/generateEcma262SectionMarkdown.js --section <section.subsection>`.

## Repo-local linking

Other docs in this repo may link to these indexes using repo-relative paths under docs/ECMA262/ (instead of linking directly to tc39.es). This keeps cross-references stable and reviewable.
