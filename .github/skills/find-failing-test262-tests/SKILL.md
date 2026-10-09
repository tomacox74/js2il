---
name: find-failing-test262-tests
description: Discover unported failing test262 cases from scoped central Supabase reporting, distinguishing current product failures from historical, harness and infrastructure evidence.
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
   - Follow **Catalog-First Candidate Selection** in `test262-porting`:
     query authorized scoped Supabase reporting or use an approved coherent
     central export. Read **Local agent read access** in
     `docs/ECMA262/Test262SupabaseCatalog.md`. Reuse `SUPABASE_URL` and
     `SUPABASE_PUBLISHABLE_KEY` from performance analysis for bounded REST reads
     when catalogue API exposure and read permissions are enabled. The deployed
     `anon` role currently lacks those rights; use the read-only connector/export
     on access denial. Never substitute service-role or coordinator credentials.
   - Record pin, target/source revision, evidence kind, provenance,
     registration-snapshot identity and as-of time. Reconcile the central
     snapshot with working-tree and pending-PR registrations.
   - Use central product-failure clusters, with full upstream paths, to find
     demonstrated semantic failures. Exclude already registered cases using
     complete paths; distinguish central master from unmerged branch state.
   - Treat `runner-error`, metadata errors, policy exclusions, and timeouts
     separately from demonstrated runtime semantic mismatches. Historical
     failures need reproduction on the current build before diagnosis.
   - For **passing** unported tests, require every required variant under
     one compatible provenance and keep MVP/historical hints separate from
     trusted native acceptance. Do not present partial coverage as exhaustive.
   - If central read access/export is unavailable, state that limitation and
     use the explicit offline rules in `test262-porting`. Legacy
     `failures.csv`/`registered.txt` lists may guide diagnosis but cannot
     establish current central status.

2. **Probe missing or stale evidence by feature area**:
   - For shared evidence, use an authorized bounded central workflow run;
     its supervisor owns claims/budgets and isolates fixture execution.
   - For a feature-branch fix, reproduce locally with focused native tests
     or targeted MVP probes and report these as local validation. Central
     workflow runs check out master. Use `catalog.py scan` only for labeled
     offline diagnosis; it does not acknowledge central ingestion.
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
- Catalogue authority: scoped Supabase reporting/approved coherent central
  exports; see `docs/ECMA262/Test262SupabaseCatalog.md`.
- Local SQLite artifacts: offline/historical hints or caches only; never the
  shared authority or proof of trusted central native acceptance.
- Running test262 MVP: `node scripts/test262/runMvp.js` for targeted reproduction.
- Spec support docs: `docs/ECMA262/` (use clause status and support notes to spot missing functionality)

