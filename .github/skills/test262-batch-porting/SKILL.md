---
name: test262-batch-porting
description: Port large, efficient batches of pinned upstream test262 cases by screening up to 500 candidates and splitting implementation work when failures span multiple root causes.
tier: standard
applyTo: 'tests/Jroc.Test262.Tests/**,tests/Jroc.Testing/Test262/**,tests/test262/**,scripts/test262/**,src/Compiler/**,src/JavaScriptRuntime/**,docs/ECMA262/**,CHANGELOG.md'
---

# Test262 Batch Porting

Use this skill when asked to port the next batch, hundreds of tests, or a
feature-area batch from upstream `test262`.

Follow `test262-porting` as the authoritative workflow for fixture fidelity,
native harness behavior, test registration, validation, and documentation.
This skill adds selection, sizing, and splitting rules for efficient bulk work.

## Batch Sizing

Treat **500 tests as the candidate intake ceiling**, not an unconditional
implementation target.

| Batch condition | Implementation size |
| --- | ---: |
| Cases already pass and belong to one coherent feature area | 400-500 |
| Cases expose one or two related compiler/runtime gaps | 100-200 |
| Cases span several unrelated failures or subsystems | Split by root cause |
| Cases require major syntax, async/iterator, Proxy/Reflect, or cross-realm work | 50 or fewer |

Prefer one coherent 500-test batch over several small batches only when it
remains straightforward to diagnose, validate, and review. Do not preserve an
arbitrary numeric target at the cost of mixing unrelated product changes.

## Workflow

### 1. Restore the catalog and inventory the pinned corpus

Start with **Catalog-First Candidate Selection** in `test262-porting`, not a
new full-corpus preflight. Download the `test262-catalog` Actions artifact or
reuse a local database, inspect `summary.json` provenance/completeness, and run
`catalog.py export --refresh-registrations` against the working tree. Full
commands and artifact behavior are in `docs/ECMA262/Test262Catalog.md`.

Use the catalog's full upstream paths and refreshed registration inventory
as the starting point, verifying the mapping below. Keep SQLite databases,
exports, and batch lists out of source control.

1. Run `node scripts/test262/bootstrap.js --print-root`.
2. Use only the commit pinned by `tests/test262/test262.pin.json`.
3. Build a deterministic, path-sorted inventory under `test/language` and
   `test/built-ins`.
4. Compare complete relative paths, never basenames. For example:

   `test/built-ins/Array/prototype/at/returns-item.js`

   maps to:

   `tests/Jroc.Test262.Tests/built-ins/Array/prototype/at/JavaScript/returns-item.js`

5. Exclude cases already represented in the repository. Check legacy or
   non-canonical fixture locations before declaring a case unported so the
   batch does not add duplicate coverage.

Keep temporary inventories and classifications in session storage or the
test262 output directory; do not commit planning manifests.

### 2. Select up to 500 coherent candidates

Choose candidates from one built-in, language construct, or closely related
set of clauses. Prefer compatible all-variant passes in
`passing-unported.txt`, in deterministic path order within the chosen area.
Use `historical-passing-unported.txt` as a secondary discovery source requiring
fresh confirmation, not as current pass evidence. Favor an area with enough
known passes over repeatedly screening the same known failures. Honor an
explicitly requested feature area.

The catalog may contain only a partially scanned corpus. Select up to 500
coherent candidates from the known evidence; do not block a useful batch on an
exhaustive scan or describe the known subset as the complete passing list.
`failures.csv` is for focused shared-gap work, not coverage-only intake.

Before copying fixtures:

- honor `excludedFromMvp` and metadata-driven unsupported classifications;
- exclude known unsupported requirements such as direct `eval`;
- inspect `includes`, `flags`, `features`, `negative`, and sibling-file
  dependencies;
- prefer cases supported by the existing native C# harness;
- note nearby ECMA-262 documentation entries that may need updating.

Do not replace unsuitable cases with arbitrary tests from unrelated areas just
to reach 500.

### 3. Preflight and classify

Reuse compatible catalog results before launching new preflight work.
`passing-unported.txt` requires every metadata-selected variant to pass under
one provenance; never aggregate a fixture from just one passing variant or
combine strict/non-strict results from different builds.

For missing or stale evidence, use a bounded `catalog.py scan --filter ...`
after initializing against the current build, then export the results.
Its `--limit` counts variants and each result is checkpointed immediately.
Use `node scripts/test262/runMvp.js` with an individual `--file` or area filter
for diagnosis; do not restrict variants when claiming a full fixture pass.
MVP and native C# harness behavior can differ, so the focused native suite
remains the acceptance gate even for catalogued passes.

Classify each candidate as:

- **clean**: passes with current compiler/runtime and needs only faithful
  fixture registration;
- **shared-gap**: fails because of a root cause shared with other candidates;
- **harness-gap**: requires a missing native test262 helper or host API;
- **large-gap**: requires a substantial or currently unsupported language or
  runtime feature;
- **policy-excluded**: not runnable under the current MVP policy.

If most candidates are clean and the remainder has at most one tightly related
root cause, keep a 400-500-test implementation batch. Otherwise, partition the
work into 100-200-test groups by root cause. Move large gaps into focused
batches of 50 or fewer rather than blocking unrelated clean ports.

### 4. Port the selected implementation group

For every accepted case:

1. Preserve the upstream relative path and filename.
2. Copy the JavaScript source exactly as pinned, including frontmatter,
   assertions, directives, and copyright headers.
3. Copy required sibling modules or scripts and register them through the
   existing `additionalFiles` mechanism.
4. Add the matching `ExecutionTests.cs` entry with the original filename as
   `DisplayName` and an identifier-safe method name.
5. Keep successful fixtures silent; do not add execution snapshots.

Generate or update registrations consistently within each affected folder.
Check for duplicate xUnit display names and C# method names after bulk edits.

### 5. Fix the correct layer

- Fix product semantics in the compiler or runtime when a preserved upstream
  assertion exposes a product defect.
- Extend the native C# test262 harness when an upstream `includes` helper or
  host API is missing.
- Never rewrite, weaken, skip, or add output to an upstream fixture to make it
  pass.
- Keep fixes scoped to root causes exercised by the accepted implementation
  group.
- If investigation reveals multiple unrelated fixes, split the batch instead
  of accumulating a broad change set.

### 6. Validate incrementally

Validate after each coherent group rather than waiting for all 500 candidates:

1. Run focused `Jroc.Test262.Tests` filters for the affected namespaces.
2. When shared native helpers change, run harness integration tests plus
   representative fixtures for every affected helper.
3. Build the affected projects or solution using the repository's existing
   commands.
4. Do not run the full local test262 suite unless explicitly requested or the
   change truly requires it; use CI for the complete parallel suite.

All accepted tests must pass before the batch is complete. Report deferred
large-gap and policy-excluded cases separately; do not count them as ported.
Refresh catalog registration exclusions using
`catalog.py export --refresh-registrations` after the accepted ports are
registered. Record native failures separately for follow-up without relabeling
MVP results as native evidence.

### 7. Complete documentation

Every accepted batch must update the customer-facing conformance evidence:

- update the Test262 summary in `docs/ECMA262/Index.md`;
- update the overall totals and every affected area/feature row in
  `docs/ECMA262/Test262Conformance.md`;
- recalculate counts and percentages using unique standalone tests from the
  pinned applicable ECMA-262 corpus;
- preserve the client-facing status vocabulary and accurate JROC version
  attribution defined by `test262-porting`.

Do this even when the batch only increases verified coverage without changing
a feature's qualitative support status.

When the batch changes the broader support story:

- update the relevant `docs/ECMA262/**/Section*.json` entries;
- regenerate their Markdown;
- update `CHANGELOG.md` with the number and principal feature areas ported.

State the final candidate count, accepted/ported count, deferred count, root
causes fixed, and focused validation result.

## Efficiency Guardrails

- Optimize for **tests ported and passing per reviewable change**, not raw files
  copied.
- Batch file discovery and reads, but investigate one failure cluster at a
  time.
- Consult and resume the catalog rather than restarting exhaustive scans.
  Do not repeatedly preflight compatible known passes; spend execution time
  on native acceptance, missing evidence, and selected failure clusters.
- Reuse existing registration, harness, runtime, and compiler patterns.
- Stop growing the implementation group when it crosses more than three
  unrelated compiler/runtime subsystems.
- Prefer a completed 150-test shared-gap batch over an unstable 500-test mixed
  batch.
- Never claim the intake count as completed coverage; only accepted,
  registered, passing fixtures count as ported.
