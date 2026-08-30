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

### 1. Materialize and inventory the pinned corpus

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
set of clauses. Continue from the first unported path in deterministic sorted
order unless the user names a feature area.

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

Use `node scripts/test262/runMvp.js` with a feature-area filter or individual
`--file` selections to classify candidates before making a large edit.

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

### 7. Complete documentation

When the batch changes the support story:

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
- Reuse existing registration, harness, runtime, and compiler patterns.
- Stop growing the implementation group when it crosses more than three
  unrelated compiler/runtime subsystems.
- Prefer a completed 150-test shared-gap batch over an unstable 500-test mixed
  batch.
- Never claim the intake count as completed coverage; only accepted,
  registered, passing fixtures count as ported.
