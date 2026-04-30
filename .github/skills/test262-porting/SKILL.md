---
name: test262-porting
description: Port a specific upstream test262 case into tests/Js2IL.Test262.Tests while preserving the original path/filename and adapting assert-based checks to this repo's execution-test model.
tier: standard
applyTo: 'tests/Js2IL.Test262.Tests/**,tests/test262/**,.github/copilot-instructions.md'
---

# Test262 Porting

Use this skill when you need to port one or more upstream `test262` tests into `tests\Js2IL.Test262.Tests`.

## Goal

Keep the upstream `test262` case as the source of truth, but adapt it into this repo's execution-test harness with minimal, explicit rewrites.

## Porting Workflow

1. Start from one concrete upstream `test262` file and preserve its relative spec path and base filename.
2. Add the repo fixture under the matching folder in `tests\Js2IL.Test262.Tests\...\JavaScript\`, using the same filename so the port still clearly maps back to the original source.
3. Keep the upstream scenario intact whenever practical, but rewrite it for this repo's execution-test model:
   - convert `assert.sameValue(...)`, `assert(...)`, and similar checks into `console.log(...)` output that captures the observable result,
   - preserve directive prologues such as `"use strict";`,
   - keep any additional local fixture files when the case depends on sibling modules or scripts, and pass them through the C# test using the existing `additionalFiles` pattern.
4. Add or update the folder's `ExecutionTests.cs` entry so:
   - the xUnit `DisplayName` is the original `test262` filename,
   - the C# method name is an identifier-safe version of that filename,
   - the execution test points at the preserved JavaScript fixture path.
5. Create the matching expected execution snapshot under the same folder's `Snapshots\` directory. The snapshot should reflect the rewritten `console.log(...)` behavior, not the original `assert` API.

## Repo-Specific Rules

- Prefer execution coverage only. Do **not** automatically add a parallel `tests\Js2IL.Tests\...` regression unless we specifically need generator/IL assertions or other project-specific coverage beyond what the `test262` port already proves.
- Keep the original `test262` layout recognizable. The path and filename are the main breadcrumb back to the upstream test.
- PR #1011 is the reference example for this workflow: the arrow-function restricted `caller` / `arguments` scenario belongs under `tests\Js2IL.Test262.Tests\language\expressions\arrow-function\`, and the parallel `tests\Js2IL.Tests\ArrowFunction\ArrowFunction_RestrictedCallerArgumentsProperties` regression is redundant.

## Validation

- Run the focused `Js2IL.Test262.Tests` suite for the affected area.
- If the case fails, first classify whether it is:
  1. a porting problem (bad rewrite, wrong snapshot, missing additional file), or
  2. a real product bug.
- Keep the port, fix the correct layer, and avoid masking product defects with ad-hoc test rewrites.

## Documentation Follow-Through

When a new `test262` case changes the documented support story, update the relevant ECMA-262 docs and changelog entry in the same PR.
