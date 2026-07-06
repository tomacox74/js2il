---
name: test262-porting
description: Port a specific upstream test262 case into tests/Jroc.Test262.Tests while preserving the original path/filename and copying the JavaScript fixture verbatim.
tier: standard
applyTo: 'tests/Jroc.Test262.Tests/**,tests/test262/**,.github/copilot-instructions.md'
---

# Test262 Porting

Use this skill when you need to port one or more upstream `test262` tests into `tests\Jroc.Test262.Tests`.

## Goal

Keep the upstream `test262` case as the source of truth by copying the JavaScript fixture exactly as-is whenever it is brought into this repo.

## Porting Workflow

1. Start from one concrete upstream `test262` file and preserve its relative spec path and base filename.
2. Add the repo fixture under the matching folder in `tests\Jroc.Test262.Tests\...\JavaScript\`, using the same filename so the port still clearly maps back to the original source.
3. Copy the upstream JavaScript fixture exactly as-is:
   - do **not** rewrite `assert.sameValue(...)`, `assert(...)`, or other upstream checks into `console.log(...)`,
   - preserve directive prologues such as `"use strict";`,
   - keep any additional local fixture files when the case depends on sibling modules or scripts, and pass them through the C# test using the existing `additionalFiles` pattern,
   - only touch shared harness helpers such as `tests\Jroc.Test262.Tests\Harness\assert.js` when the upstream fixture genuinely depends on helper behavior that is not implemented yet; these additions should be rare and should preserve upstream intent rather than changing the fixture.
4. Add or update the folder's `ExecutionTests.cs` entry so:
   - the xUnit `DisplayName` is the original `test262` filename,
   - the C# method name is an identifier-safe version of that filename,
   - the execution test points at the preserved JavaScript fixture path.
5. Create the matching expected execution snapshot under the same folder's `Snapshots\` directory. The snapshot should reflect the fixture running unchanged against the shared test262 harness helpers.

## Repo-Specific Rules

- Prefer execution coverage only. Do **not** automatically add a parallel `tests\Jroc.Tests\...` regression unless we specifically need generator/IL assertions or other project-specific coverage beyond what the `test262` port already proves.
- Keep the original `test262` layout recognizable. The path and filename are the main breadcrumb back to the upstream test.
- Do not edit copied `tests\Jroc.Test262.Tests\...\JavaScript\*.js` fixtures to fit the local harness. If a case needs support glue, add it in shared harness helpers such as `assert.js`, and keep that kind of change rare.
- PR #1011 is the reference example for this workflow: the arrow-function restricted `caller` / `arguments` scenario belongs under `tests\Jroc.Test262.Tests\language\expressions\arrow-function\`, and the parallel `tests\Jroc.Tests\ArrowFunction\ArrowFunction_RestrictedCallerArgumentsProperties` regression is redundant.

## Validation

- Run the focused `Jroc.Test262.Tests` suite for the affected area.
- If the case fails, first classify whether it is:
  1. a porting problem (wrong file placement, wrong snapshot, missing additional file, or missing shared harness helper), or
  2. a real product bug.
- Keep the port, fix the correct layer, and avoid masking product defects with ad-hoc test rewrites.

## Documentation Follow-Through

When a new `test262` case changes the documented support story, update the relevant ECMA-262 docs and changelog entry in the same PR.
