---
name: test262-porting
description: Port or fix pinned upstream test262 cases using the central Supabase catalogue for shared evidence, and extend the native C# harness when required.
tier: standard
applyTo: 'tests/Jroc.Test262.Tests/**,tests/Jroc.Testing/Test262/**,tests/test262/**,.github/copilot-instructions.md'
---

# Test262 Porting

Use this skill when you need to port one or more upstream `test262` tests into `tests\Jroc.Test262.Tests`.

## Goal

Keep the upstream `test262` case as the source of truth by copying the JavaScript fixture exactly as-is whenever it is brought into this repo.

## Catalog-First Candidate Selection

Use the **central Supabase catalogue** as the shared source of evidence,
registration snapshots, reporting targets, leases and budgets. Read
[the central catalogue runbook](../../../docs/ECMA262/Test262SupabaseCatalog.md)
before selection or screening. Repository variable
`TEST262_CATALOGUE_AUTHORITY=supabase` selects this backend; legacy SQLite
artifacts do not become authoritative when copied into a checkout.

1. Update the working branch from current `master` and read its instructions.
   Scope central reporting to repository
   `faf01df8-aa65-4375-a708-6c1e555f957b`, the pinned revision in
   `tests/test262/test262.pin.json`, evidence kind and selected channel.
   Use an authorized read-only reporting connection/connector or an
   operator-provided coherent central export. Start with
   `test262_reporting.v_current_fixture_status`,
   `v_historical_candidates`, `v_native_acceptance` and
   `v_failure_clusters`; inspect their actual column definitions before
   querying. Record source/as-of time, source revision, provenance, target
   and registration-snapshot identities, required variants and limitations.
   Never request supervisor/publication credentials for candidate discovery.

2. Compare central registration evidence with the actual working-tree C#
   registrations and fixture hashes, including pending PRs and legacy paths.
   Use complete upstream paths, never basenames. A master snapshot cannot
   know about this checkout's unmerged registrations; subtract those locally
   without deleting or replacing central registrations. Resolve unmapped or
   ambiguous entries before counting a candidate as unported.

3. Prefer compatible all-required-variant evidence for coverage-only ports.
   Treat historical passes and MVP composite-runner passes as discovery
   hints, not fresh native acceptance. For broken-test work, group central
   product failures by root cause and reproduce them on the branch build.
   Keep harness gaps, policy exclusions, metadata errors, infrastructure
   errors and incomplete work separate. Do not combine variants across
   provenance, pins, dependencies or evidence kinds.

4. Use the reviewed `.github/workflows/test262-central.yml` supervisor for
   authorized shared screening, queue claims and durable uploads. Inspect
   existing runs before proposing additional work:

   ```sh
   gh run list --workflow test262-central.yml --branch master --limit 10
   gh run view RUN_ID --json headSha,status,conclusion,jobs
   gh api repos/tomacox74/js2il/actions/runs/RUN_ID/artifacts
   ```

   The workflow checks out `master`; selecting a feature branch in the
   dispatch UI does not screen that branch's compiler fix. Request a bounded
   `area` with `kind=native` for native screening or `mvp-composite` for
   discovery; default to `publish=false`. Dispatch/publication requires
   existing task authorization. `export_snapshot` is optional and defaults
   to false: ordinary recovery artifacts contain context/outboxes, not
   necessarily a complete catalogue. A central NDJSON export needs a matching
   repository header, snapshot token/as-of time and verified footer counts
   and SHA-256; reject `.partial` files or a missing completion footer.
   Do not restart disabled `test262-catalog.yml` or legacy native workflows.

5. Run focused native tests on the working-tree compiler to diagnose and
   validate a fix. These local results are valid PR validation but do not
   automatically enter Supabase or qualify as trusted central evidence.
   Report that distinction explicitly. Never run fixtures in a process with
   database/GitHub write credentials. Shared execution uses the scoped
   supervisor and isolated fixture containers from the runbook; preserve
   outboxes until server receipts and completion acknowledgements are
   verified. Do not directly update tables, reporting targets, trust classes,
   leases or authority, or manufacture observations from TRX/SQLite counts.

6. After porting, recheck complete-path registrations locally and document
   the candidate/evidence source and focused validation in the PR. After
   merge, verify required master CI at the exact merged SHA, then verify a
   central preparation/registration refresh selects that SHA and maps the
   new fixtures. Ordinary PR/master test CI alone does not refresh Supabase
   registrations. Core catalogue acceptance additionally requires fresh
   trusted required-variant bindings, acknowledged ingestion/completions and
   settled reservations; a green workflow or registration count is not proof.

### Offline diagnosis and SQLite caches

If read-only central access or a usable central export is unavailable, state
the access limitation. Continue scoped compiler/harness diagnosis and local
PR validation when authorized, but mark selection/evidence as offline,
untrusted with respect to central acceptance, and potentially stale. Do not
claim a central refresh, current global completeness or trusted native
acceptance. Do not block a concrete compiler fix on a whole-corpus scan.

Use `catalog.py`, `nativePorting.py` and legacy SQLite lists only as explicit
offline diagnostics, historical discovery or disposable generation caches.
Their local `export --refresh-registrations` changes only local state; it
does not update Supabase. Follow the legacy/offline command reference in
`docs/ECMA262/Test262Catalog.md` only with that scope stated. Never import an
agent's local database as a replacement for active central state or reuse
another producer's IDs/epoch. A source registration, local pass, legacy import
or accepted batch each has a different meaning.

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
Disk-backed test classes in `language` and `intl402` inherit the shared
`tests/Jroc.Test262.Tests/DiskExecutionTestsBase.cs`. Built-ins inherit
`InMemoryExecutionTestsBase`, a collection-marked wrapper over the same base.
Execution and compilation-failure fixtures resolve relative to the calling
test source file, including nested fixture paths.

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

Every accepted, passing `test262` case changes JROC's published conformance
evidence. Update both customer-facing conformance documents in the same PR:

1. Update `docs/ECMA262/Index.md`:
   - refresh the Test262 `Verified passing` and `Not yet verified` counts;
   - recalculate their percentages against the applicable pinned ECMA-262
     corpus;
   - keep the link to the detailed conformance report.
2. Update `docs/ECMA262/Test262Conformance.md`:
   - refresh the overall totals;
   - refresh every affected area and feature row;
   - ensure each row's passing, known-unsupported, and no-published-result
     counts add up to its applicable total;
   - recalculate each affected verified percentage.
3. Keep these documents client-facing. Use conformance terms such as
   `Verified passing`, `Known unsupported`, and `No published result`; do not
   expose fixture-porting or repository-registration details.
4. Count unique standalone upstream tests at the pinned Test262 revision.
   Do not count `_FIXTURE.js` support files, strict/non-strict variants, test
   infrastructure, or duplicate local fixtures as additional conformance.
5. Keep the documented JROC version accurate. Do not attribute conformance
   added only on the development branch to an older released version.

When the cases also change the feature support story, update the relevant
`docs/ECMA262/**/Section*.json` entries, regenerate their Markdown, and update
`CHANGELOG.md` in the same PR.
