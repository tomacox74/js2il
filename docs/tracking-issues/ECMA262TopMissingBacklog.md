# ECMA-262 Top Missing Features Backlog

> **Last Updated**: 2026-03-09
> Purpose: capture the highest-value unsupported or still-incomplete ECMA-262 features to drive issue execution planning.
> Source basis: current branch/runtime/compiler state (including open PR #831), current `docs\ECMA262\**\Section*.json` status tracking, and targeted behavior checks.

## Current state changes since the previous draft

- **Issue #773 is no longer backlog work.** ArrayBuffer and DataView are implemented and documented, so the old "typed arrays need ArrayBuffer/DataView foundation" item is stale.
- **Issue #774 is no longer backlog work.** PR #818 is merged, issue #774 is closed, and typed arrays are no longer one of the top missing ECMA-262 features in the current codebase.
- **Issue #775 is no longer backlog work.** PR #819 landed the modern RegExp parity pass, so this item should not remain in the ranked missing-features list.
- **Issue #776 is no longer backlog work.** PR #821 landed the ordinary object invariant work on `master`, closing the descriptor/key-ordering gap that this backlog previously ranked near the top.
- **Issue #777 is no longer backlog work.** PR #822 is merged, issue #777 is closed, and object integrity semantics are now documented on `master`.
- **Issue #778 is no longer backlog work.** PR #823 is merged, issue #778 is closed, and the staged Function-constructor support has landed on `master`.
- **Issue #779 is no longer backlog work.** PR #824 is merged on `master`, issue #779 is closed, and the symbol ecosystem audit landed with runtime/tests/docs updates.
- **Issue #780 is no longer backlog work.** PR #825 is merged, issue #780 is closed, and Array iterator methods / `Array.prototype[Symbol.iterator]` are now on `master`.
- **Issue #781 is no longer one of the top missing features on the current branch.** PR #831 adds a host-safe WeakRef / FinalizationRegistry baseline, updates the related 9.x / 19.3 / 26.x docs to `Supported with Limitations`, and leaves review/merge as the remaining step.
- **Issue #772 still reads more like close-or-rescope hygiene than a top missing implementation gap.** Static `import` / `export` lowering plus live-binding and cycle coverage already exist for common cases, so the remaining work needs sharper scoping if the issue stays open.

## Ranking Criteria

- Real-world unblock impact (npm/tooling/runtime compatibility)
- Breadth of spec surface still missing
- Dependency leverage (enables multiple later features)
- Testability and rollout feasibility

## Current ranked backlog (recommended order)

| Rank | Backlog item | Primary spec areas | Current status signal |
|---:|---|---|---|
| 1 | [Re-scope or close the remaining ES module semantics issue](https://github.com/tomacox74/js2il/issues/772) | 16.2.x | Supported with Limitations / likely issue hygiene unless broader module-record semantics are still required |
| 2 | [Descriptor-backed `Function` metadata (`length` / `name`)](https://github.com/tomacox74/js2il/issues/727) | 20.2.4.1, 20.2.4.2 | Supported with Limitations |
| 3 | [Complete bound function constructor/new-target + metadata semantics](https://github.com/tomacox74/js2il/issues/728) | 20.2.3.2, 10.2.x | Supported with Limitations |

No fourth tracked ECMA-262 issue currently stands above these three; additional spec backlog work likely needs fresh issue scoping rather than carrying forward now-landed #780 or active-landing #781 work.

## TypedArray follow-up note

- PR #818 is merged and issue #774 is closed.
- Remaining typed-array gaps are follow-up polish items rather than one of the top missing features:
  - constructor/prototype metadata fidelity (`%TypedArray%.prototype`, `.constructor`, static `BYTES_PER_ELEMENT`)
  - detached-buffer semantics and species hooks
  - broader family parity beyond the currently targeted `Int32Array`, `Uint8Array`, and `Float64Array`

## Object integrity status note

- PR #822 is merged and issue #777 is closed, so object integrity work is no longer backlog planning material.

## Symbol ecosystem status note

- PR #824 is merged and issue #779 is closed, so symbol ecosystem work is no longer backlog planning material.
- If additional symbol follow-up is needed, it should likely narrow to the remaining well-known-symbol hooks (`Symbol.hasInstance`, `Symbol.species`, `Symbol.toPrimitive`) rather than reopen the completed #779 landing scope.

## Array iterator status note

- PR #825 is merged and issue #780 is closed, so array iterator work is no longer backlog planning material.
- If any follow-up is needed, it should narrow to iterator metadata/fidelity (for example `%ArrayIteratorPrototype%[%Symbol.toStringTag%]` and more exotic edge cases) rather than re-ranking #780 as a top missing feature.

## WeakRef / FinalizationRegistry status note

- PR #831 is open and implements a host-safe baseline for issue #781 on the current branch.
- The branch docs now mark the relevant host-cleanup and object sections as `Supported with Limitations` rather than `Not Yet Supported`.
- Remaining work is review/merge and any fidelity follow-up, not top-backlog scoping.

## Issue-ready backlog stubs

## Issue 1: Re-scope ES Module Semantics Gaps ([#772](https://github.com/tomacox74/js2il/issues/772))
- Suggested labels: `enhancement`, `spec:ecma-262`, `modules`, `commonjs`, `priority:medium`
- Current reality:
  - Static `import` / `export` lowering already exists
  - Live import bindings and cyclic coverage exist for common cases
  - The original issue wording now looks broader than the remaining codebase gap
- Minimum acceptance:
  - Either close the issue as effectively fulfilled, or re-scope it to the specific module-record / linking / evaluation semantics still missing
  - If kept open, document the exact unsupported behavior still intended (rather than the already-landed lowering work)
  - Add or refresh targeted tests only for the remaining unsupported semantics

## Issue 2: Descriptor-Backed `Function` Metadata ([#727](https://github.com/tomacox74/js2il/issues/727))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Move `Function` instance `.length` / `.name` to descriptor-backed own properties
  - Match ordinary property reflection behavior for those metadata properties
  - Add execution/generator coverage for descriptor interactions and reflection

## Issue 3: Complete Bound Function Semantics ([#728](https://github.com/tomacox74/js2il/issues/728))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Preserve bound constructor / `new.target` semantics
  - Align bound-function metadata and prototype behavior with the spec
  - Add focused coverage for constructor calls, metadata, and reflection
