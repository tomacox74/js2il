# ECMA-262 Top Missing Features Backlog

> **Last Updated**: 2026-03-08
> Purpose: capture the highest-value unsupported or still-incomplete ECMA-262 features to drive issue execution planning.
> Source basis: current branch/runtime/compiler state, current `docs\ECMA262\**\Section*.json` status tracking, and targeted behavior checks.

## Current state changes since the previous draft

- **Issue #773 is no longer backlog work.** ArrayBuffer and DataView are implemented and documented, so the old "typed arrays need ArrayBuffer/DataView foundation" item is stale.
- **Issue #774 is no longer backlog work.** PR #818 is merged, issue #774 is closed, and typed arrays are no longer one of the top missing ECMA-262 features in the current codebase.
- **Issue #775 is no longer backlog work.** PR #819 landed the modern RegExp parity pass, so this item should not remain in the ranked missing-features list.
- **Issue #776 is no longer backlog work.** PR #821 landed the ordinary object invariant work on `master`, closing the descriptor/key-ordering gap that this backlog previously ranked near the top.
- **Issue #777 is no longer backlog work.** PR #822 is merged, issue #777 is closed, and object integrity semantics are now documented on `master`.
- **Issue #778 is no longer backlog work.** PR #823 is merged, issue #778 is closed, and the staged Function-constructor support has landed on `master`.
- **Issue #779 now looks like active landing work rather than a top missing feature.** The current branch (`copilot/gh-779-symbol-ecosystem`) has PR #824 open with runtime/tests/docs updates for symbol-key reflection, `Symbol.isConcatSpreadable`, and `Symbol.toStringTag`.
- **Issue #772 still reads more like close-or-rescope hygiene than a top missing implementation gap.** Static `import` / `export` lowering plus live-binding and cycle coverage already exist for common cases, so the remaining work needs sharper scoping if the issue stays open.

## Ranking Criteria

- Real-world unblock impact (npm/tooling/runtime compatibility)
- Breadth of spec surface still missing
- Dependency leverage (enables multiple later features)
- Testability and rollout feasibility

## Current ranked backlog (recommended order)

| Rank | Backlog item | Primary spec areas | Current status signal |
|---:|---|---|---|
| 1 | [Array iterator method surface (`entries`, `keys`, `values`, `Array.prototype[Symbol.iterator]`)](https://github.com/tomacox74/js2il/issues/780) | 23.1.3.5, 23.1.3.19, 23.1.3.38, 23.1.3.40 | Partially supported (`for..of` works; direct prototype methods remain incomplete) |
| 2 | [WeakRef / FinalizationRegistry processing model and cleanup jobs](https://github.com/tomacox74/js2il/issues/781) | 9.9-9.13 | Not Yet Supported |
| 3 | [Re-scope or close the remaining ES module semantics issue](https://github.com/tomacox74/js2il/issues/772) | 16.2.x | Supported with Limitations / likely issue hygiene unless broader module-record semantics are still required |
| 4 | [Descriptor-backed `Function` metadata (`length` / `name`)](https://github.com/tomacox74/js2il/issues/727) | 20.2.4.1, 20.2.4.2 | Supported with Limitations |
| 5 | [Complete bound function constructor/new-target + metadata semantics](https://github.com/tomacox74/js2il/issues/728) | 20.2.3.2, 10.2.x | Supported with Limitations |

## TypedArray follow-up note

- PR #818 is merged and issue #774 is closed.
- Remaining typed-array gaps are follow-up polish items rather than one of the top missing features:
  - constructor/prototype metadata fidelity (`%TypedArray%.prototype`, `.constructor`, static `BYTES_PER_ELEMENT`)
  - detached-buffer semantics and species hooks
  - broader family parity beyond the currently targeted `Int32Array`, `Uint8Array`, and `Float64Array`

## Object integrity status note

- PR #822 is merged and issue #777 is closed, so object integrity work is no longer backlog planning material.

## Symbol ecosystem status note

- The current branch + open PR #824 appear to satisfy issue #779's explicit acceptance criteria.
- Remaining work is review/merge rather than a large missing implementation area:
  - `Symbol.isConcatSpreadable` in `Array.prototype.concat`
  - symbol-key descriptor / enumeration coverage and doc alignment
  - custom `Symbol.toStringTag` behavior coverage
- If review uncovers gaps, follow-up should likely narrow to the remaining well-known-symbol hooks (`Symbol.hasInstance`, `Symbol.species`, `Symbol.toPrimitive`) rather than re-ranking #779 as a top missing feature.

## Issue-ready backlog stubs

## Issue 1: Implement Array Iterator Methods and Symbol Iterator Wiring ([#780](https://github.com/tomacox74/js2il/issues/780))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Expose `entries`, `keys`, `values`, and `Array.prototype[Symbol.iterator]`
  - Ensure compatibility with `for..of` and iterator protocol helpers
  - Add tests for direct iterator method usage and interoperability

## Issue 2: Implement WeakRef / FinalizationRegistry Processing Model ([#781](https://github.com/tomacox74/js2il/issues/781))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:low`
- Minimum acceptance:
  - Add a runtime model and host hooks for cleanup-job scheduling
  - Implement `ClearKeptObjects` and related abstract operations
  - Add deterministic or host-safe tests for supported cleanup semantics

## Issue 3: Re-scope ES Module Semantics Gaps ([#772](https://github.com/tomacox74/js2il/issues/772))
- Suggested labels: `enhancement`, `spec:ecma-262`, `modules`, `commonjs`, `priority:medium`
- Current reality:
  - Static `import` / `export` lowering already exists
  - Live import bindings and cyclic coverage exist for common cases
  - The original issue wording now looks broader than the remaining codebase gap
- Minimum acceptance:
  - Either close the issue as effectively fulfilled, or re-scope it to the specific module-record / linking / evaluation semantics still missing
  - If kept open, document the exact unsupported behavior still intended (rather than the already-landed lowering work)
  - Add or refresh targeted tests only for the remaining unsupported semantics

## Issue 4: Descriptor-Backed `Function` Metadata ([#727](https://github.com/tomacox74/js2il/issues/727))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Move `Function` instance `.length` / `.name` to descriptor-backed own properties
  - Match ordinary property reflection behavior for those metadata properties
  - Add execution/generator coverage for descriptor interactions and reflection

## Issue 5: Complete Bound Function Semantics ([#728](https://github.com/tomacox74/js2il/issues/728))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Preserve bound constructor / `new.target` semantics
  - Align bound-function metadata and prototype behavior with the spec
  - Add focused coverage for constructor calls, metadata, and reflection
