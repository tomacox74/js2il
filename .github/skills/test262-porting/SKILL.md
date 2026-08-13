---
name: test262-porting
description: Port upstream test262 cases and extend the native C# test262 harness when a fixture requires another helper.
tier: standard
applyTo: 'tests/Jroc.Test262.Tests/**,tests/Jroc.Testing/Test262/**,tests/test262/**,.github/copilot-instructions.md'
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
   - preserve frontmatter such as `includes`, `flags`, and `negative`; the shared C# harness uses it to select helpers and validate expected failures,
   - do not add or inline JavaScript harness files. Extend the native C# harness when a required helper is missing.
4. Add or update the folder's `ExecutionTests.cs` entry so:
   - the xUnit `DisplayName` is the original `test262` filename,
   - the C# method name is an identifier-safe version of that filename,
   - the execution test points at the preserved JavaScript fixture path.
5. Successful test262 fixtures must produce no output. Assertions fail by throwing, so do not create an execution snapshot.

## Native Harness Overview

All test262 harness support lives under `tests/Jroc.Testing/Test262`.

| File | Responsibility |
| --- | --- |
| `Test262SharedAssertHarness.cs` | Parses frontmatter, injects `onlyStrict` when needed, selects native helpers from `includes`, compiles and executes the fixture, checks runtime-negative exception types, and enforces no output. |
| `Test262HostRuntimeIntrinsics.cs` | Registers the core host globals and dispatches optional helper registration. |
| `Test262PropertyHelpers.cs` | Implements `propertyHelper.js` descriptor and attribute checks. |
| `Test262TypedArrayHelpers.cs` | Implements typed-array constructor lists, argument factories, and callback matrices. |
| `Test262AtomicsHelpers.cs` | Implements Atomics index and non-view value matrices. |
| `Test262EncodingHelpers.cs` | Implements hexadecimal encoding helpers. |
| `Test262PromiseHelpers.cs` | Implements promise sequence and settled-result checks. |

The harness does not read, concatenate, or compile helper JavaScript. The
`tests/Jroc.Test262.Tests/Harness` directory was removed.

`Test262SharedAssertHarness` reads the fixture's `includes` array and passes it
to `Test262HostRuntimeIntrinsics.Create`. Core globals are always available:

- `assert`, backed by the production `JavaScriptRuntime.Node.AssertModule`;
- `Test262Error`, `$ERROR`, `$DONE`, and `$262`;
- `compareArray`, `isConstructor`, `getWellKnownIntrinsicObject`,
  `assertRelativeDateMs`, and `asyncTest`;
- property helpers, which remain unconditional because some older hand-ported
  fixtures use them without retaining upstream frontmatter.

Other helper groups are registered only when their upstream filename appears
in `includes`, for example `testTypedArray.js`, `testAtomics.js`,
`decimalToHexString.js`, `promiseHelper.js`, `tcoHelper.js`, or `nans.js`.
This keeps runtime setup small while preserving the upstream metadata contract.

Assertions do not print success markers. A normal test passes by completing
with empty output. `assert` failures throw `AssertionError`. Runtime-negative
tests that intentionally leave an exception unhandled are validated against
the frontmatter `negative.type`; tests that catch an expected exception with
`assert.throws` execute normally.

## Adding a Missing Harness Helper

When a newly ported fixture names a helper that is not implemented:

1. Read the pinned upstream helper and inventory every global it defines that
   the ported fixtures use. Preserve its observable JavaScript semantics,
   callback order, constructor matrix, coercions, and assertion behavior.
2. Add a focused `Test262<Name>Helpers.cs` file under
   `tests/Jroc.Testing/Test262`. Use a `Register(...)` entry point when the
   helper exposes multiple globals.
3. Expose JavaScript-callable functions with
   `Test262HostRuntimeIntrinsics.CreateFunction`, including the upstream
   function `name` and `length`. Use `ObjectRuntime`, `TypeUtilities`,
   `Closure`, `JsNull`, and public runtime objects so behavior follows JROC's
   JavaScript semantics rather than CLR shortcuts.
4. Add conditional registration in `Test262HostRuntimeIntrinsics.Create` keyed
   by the exact upstream include filename. Register unconditionally only when
   existing hand-ported fixtures demonstrably rely on the helper without
   frontmatter, and document that reason beside the registration.
5. For helper data such as constructor lists or value tables, return actual
   JavaScript-visible arrays and objects. Preserve the distinction between CLR
   `null` (JavaScript `undefined`) and `JsNull.Null` (JavaScript `null`).
6. Extend
   `tests/Jroc.Test262.Tests/Integration/JavaScript/test262NativeHostHelpers.js`
   with the helper filename in `includes` and assertions covering its native
   globals. Also run representative real fixtures that exercise its edge cases.
7. Do not recreate `tests/Jroc.Test262.Tests/Harness`, prepend helper source to
   fixtures, or weaken copied assertions to make a port pass.

Typed-array constructors require particular care. Pass JavaScript-visible,
constructible adapters rather than raw CLR delegates, and preserve the
upstream constructor/argument-factory callback matrix so moving the helper to
C# does not silently reduce test coverage.

## Repo-Specific Rules

- Prefer execution coverage only. Do **not** automatically add a parallel `tests\Jroc.Tests\...` regression unless we specifically need generator/IL assertions or other project-specific coverage beyond what the `test262` port already proves.
- Keep the original `test262` layout recognizable. The path and filename are the main breadcrumb back to the upstream test.
- Do not edit copied `tests\Jroc.Test262.Tests\...\JavaScript\*.js` fixtures to fit the local harness. Fix missing support in the native C# harness or product runtime.
- PR #1011 is the reference example for this workflow: the arrow-function restricted `caller` / `arguments` scenario belongs under `tests\Jroc.Test262.Tests\language\expressions\arrow-function\`, and the parallel `tests\Jroc.Tests\ArrowFunction\ArrowFunction_RestrictedCallerArgumentsProperties` regression is redundant.

## Validation

- Run the focused `Jroc.Test262.Tests` suite for the affected area.
- If the case fails, first classify whether it is:
  1. a porting problem (wrong file placement, missing additional file, malformed frontmatter, or missing native harness helper), or
  2. a real product bug.
- Keep the port, fix the correct layer, and avoid masking product defects with ad-hoc test rewrites.
- When changing shared native helpers, run the harness integration tests plus
  representative fixtures for every affected helper. Let PR CI run the full
  test262 and normal solution suites.

## Documentation Follow-Through

When a new `test262` case changes the documented support story, update the relevant ECMA-262 docs and changelog entry in the same PR.
