---
name: find-failing-test262-tests
description: Discover which test262 tests are failing and not yet ported into tests/Jroc.Test262.Tests, identifying good candidates for future porting cycles.
tier: standard
applyTo: 'tests/Jroc.Test262.Tests/**,scripts/test262/**,docs/ECMA262/**'
---

# Find Failing Test262 Tests

Use this skill when you need to discover which test262 tests are failing but haven't yet been ported into this repo's test suite.

## Goal

Identify failing test262 test cases that:
1. Are not yet ported to `tests/Jroc.Test262.Tests/`
2. Don't require features marked as permanently unsupported (e.g., `eval`)
3. Could be good candidates for porting in future cycles

## Workflow

1. **Consult the catalog before probing**:
   - Follow **Catalog-First Candidate Selection** in `test262-porting` to
     download the `test262-catalog` Actions artifact and refresh registrations.
     Commands and provenance rules are in `docs/ECMA262/Test262Catalog.md`.
   - Read `summary.json` for the pinned revision, evaluation fingerprint,
     completeness, and registration warnings.
   - Use `failures.csv` to find recorded `runtime-mismatch` clusters. Subtract
     `registered.txt` using complete upstream paths: the failures export is
     not restricted to unported tests.
   - Treat `runner-error`, metadata errors, policy exclusions, and timeouts
     separately from demonstrated runtime semantic mismatches. Historical
     failures need reproduction on the current build before diagnosis.
   - If the user wants **passing** unported tests, use
     `passing-unported.txt` instead; it requires all selected variants to pass.
     Do not present a partially scanned catalog as an exhaustive result.

2. **Probe missing or stale evidence by feature area**:
   - Prefer bounded `catalog.py scan --filter ... --limit 100 --seconds 120`
     after `init` against the current build; export afterward to retain
     checkpointed results. The limit counts variants, not fixtures.
   - Use `node scripts/test262/runMvp.js` for targeted reproduction:
   - Filter by built-in category (e.g., `built-ins/Array`, `built-ins/String`)
   - Limit test count to avoid timeout (start with `--limit 50` to `--limit 100`)
   - Capture `RUNTIME-MISMATCH` failures (not `PASS` or `COMPILE-ERROR`)

3. **Identify non-ported cases**:
   - Check if test file is already ported by looking at `tests/Jroc.Test262.Tests/<area>/JavaScript/`
   - Use the test262 relative path as the lookup key (e.g., `test/built-ins/Array/prototype/at/returns-item.js`)
   - Exclude tests that require fully unsupported features (check comments in test file)
   - Cross-check `docs/ECMA262/` for nearby clauses still marked unsupported or partial support; those gaps often point to missing runtime/compiler behavior behind failing test262 cases

4. **Filter for scope**:
   - Prefer tests that require small fixes: missing methods, property descriptors, small implementations
   - Avoid tests requiring large features like:
     - New iterator types or async constructs
     - Cross-realm functionality (`$262` global)
     - Advanced `Proxy` or `Reflect` semantics
   - Look at actual test file content to understand what's needed

5. **Report findings**:
   - List 3-10 candidate test files with paths
   - For each, note:
     - What's failing (error message snippet)
     - What appears to be missing (method? descriptor? property?)
     - Estimated complexity (simple, moderate, complex)

## Example Query

```powershell
node scripts/test262/runMvp.js --filter "built-ins/Array/prototype" --limit 60
```

## Tips

- Focus on `built-ins/*` rather than `language/*` for simpler fixes
- Method metadata tests (name, length, property-desc) are often quick wins
- Avoid tests with `Symbol` in the name unless a Symbol feature was just added
- Read the test file header (after `/*---`) to see what features it requires
- Check the error message in RUNTIME-MISMATCH to understand the gap
- Use `docs/ECMA262/` as a second discovery source: unsupported or partially supported spec entries can help you predict which test262 areas are likely still broken

## Repo Context

- Test262 cache: `artifacts/test262/cache/<sha>/test/`
- Ported tests: `tests/Jroc.Test262.Tests/<category>/<feature>/JavaScript/`
- Execution tests: `tests/Jroc.Test262.Tests/<category>/<feature>/ExecutionTests.cs`
- Catalog: `artifacts/test262/catalog.sqlite`, or a downloaded `test262-catalog`
  Actions artifact; reuse recorded evidence before probing.
- Running test262 MVP: `node scripts/test262/runMvp.js` for targeted reproduction.
- Spec support docs: `docs/ECMA262/` (use clause status and support notes to spot missing functionality)
