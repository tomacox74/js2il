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

1. **Probe by feature area** using `node scripts/test262/runMvp.js`:
   - Filter by built-in category (e.g., `built-ins/Array`, `built-ins/String`)
   - Limit test count to avoid timeout (start with `--limit 50` to `--limit 100`)
   - Capture `RUNTIME-MISMATCH` failures (not `PASS` or `COMPILE-ERROR`)

2. **Identify non-ported cases**:
   - Check if test file is already ported by looking at `tests/Jroc.Test262.Tests/<area>/JavaScript/`
   - Use the test262 relative path as the lookup key (e.g., `test/built-ins/Array/prototype/at/returns-item.js`)
   - Exclude tests that require fully unsupported features (check comments in test file)
   - Cross-check `docs/ECMA262/` for nearby clauses still marked unsupported or partial support; those gaps often point to missing runtime/compiler behavior behind failing test262 cases

3. **Filter for scope**:
   - Prefer tests that require small fixes: missing methods, property descriptors, small implementations
   - Avoid tests requiring large features like:
     - New iterator types or async constructs
     - Cross-realm functionality (`$262` global)
     - Advanced `Proxy` or `Reflect` semantics
   - Look at actual test file content to understand what's needed

4. **Report findings**:
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
- Running test262 MVP: `node scripts/test262/runMvp.js` (fastest way to discover failing tests)
- Spec support docs: `docs/ECMA262/` (use clause status and support notes to spot missing functionality)
