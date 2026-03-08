# ECMA-262 Top Missing Features Backlog

> **Last Updated**: 2026-03-08
> Purpose: capture the highest-value unsupported or still-incomplete ECMA-262 features to drive issue execution planning.
> Source basis: current branch/runtime/compiler state, current `docs\ECMA262\**\Section*.json` status tracking, and targeted behavior checks.

## Current state changes since the previous draft

- **Issue #773 is no longer backlog work.** ArrayBuffer and DataView are implemented and documented, so the old "typed arrays need ArrayBuffer/DataView foundation" item is stale.
- **Issue #774 is no longer backlog work.** PR #818 is merged, issue #774 is closed, and typed arrays are no longer one of the top missing ECMA-262 features in the current codebase.
- **Issue #775 is no longer backlog work.** PR #819 landed the modern RegExp parity pass, so this item should not remain in the ranked missing-features list.
- **Issue #776 is no longer backlog work.** PR #821 landed the ordinary object invariant work on `master`, closing the descriptor/key-ordering gap that this backlog previously ranked near the top.
- **Issue #777 now looks like active landing work rather than a top missing feature.** The current branch (`copilot/gh-777-object-integrity`) has ready-for-review PR #822, and its runtime/tests/docs updates appear to satisfy the issue's explicit acceptance criteria.
- **Issue #772 still reads more like close-or-rescope hygiene than a top missing implementation gap.** Static `import` / `export` lowering plus live-binding and cycle coverage already exist for common cases, so the remaining work needs sharper scoping if the issue stays open.

## Ranking Criteria

- Real-world unblock impact (npm/tooling/runtime compatibility)
- Breadth of spec surface still missing
- Dependency leverage (enables multiple later features)
- Testability and rollout feasibility

## Current ranked backlog (recommended order)

| Rank | Backlog item | Primary spec areas | Current status signal |
|---:|---|---|---|
| 1 | [Function constructor and dynamic function creation (`new Function`)](https://github.com/tomacox74/js2il/issues/778) | 20.2.1.x | Not Yet Supported |
| 2 | [Symbol ecosystem completeness (symbol-keyed reflection + well-known-symbol parity)](https://github.com/tomacox74/js2il/issues/779) | 20.1.2.11, 20.4.x, symbol-driven APIs | Supported with Limitations |
| 3 | [Array iterator method surface (`entries`, `keys`, `values`, `Array.prototype[Symbol.iterator]`)](https://github.com/tomacox74/js2il/issues/780) | 23.1.3.5, 23.1.3.19, 23.1.3.38, 23.1.3.40 | Partially supported (`for..of` works; direct prototype methods remain incomplete) |
| 4 | [WeakRef / FinalizationRegistry processing model and cleanup jobs](https://github.com/tomacox74/js2il/issues/781) | 9.9-9.13 | Not Yet Supported |
| 5 | [Re-scope or close the remaining ES module semantics issue](https://github.com/tomacox74/js2il/issues/772) | 16.2.x | Supported with Limitations / likely issue hygiene unless broader module-record semantics are still required |

## TypedArray follow-up note

- PR #818 is merged and issue #774 is closed.
- Remaining typed-array gaps are follow-up polish items rather than one of the top missing features:
  - constructor/prototype metadata fidelity (`%TypedArray%.prototype`, `.constructor`, static `BYTES_PER_ELEMENT`)
  - detached-buffer semantics and species hooks
  - broader family parity beyond the currently targeted `Int32Array`, `Uint8Array`, and `Float64Array`

## Object integrity status note

- The current branch + ready-for-review PR #822 appear to satisfy issue #777's explicit acceptance criteria.
- Remaining work is review/merge rather than a large missing implementation area:
  - strict-mode `TypeError` behavior for add/write/delete failures under non-extensible, sealed, frozen, read-only, and getter-only conditions
  - focused integrity / `Object.defineProperty` coverage and updated `docs\ECMA262\20\Section20_1.*` tracking
- If review uncovers gaps, follow-up should likely be narrowed to exotic-object or remaining invariant edge cases rather than re-ranking #777 as a top missing feature.

## Issue-ready backlog stubs

## Issue 1: Implement Function Constructor (`new Function`) ([#778](https://github.com/tomacox74/js2il/issues/778))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Implement a guarded function-constructor creation path (an AOT-friendly subset is acceptable)
  - Define runtime constraints and diagnostics for unsupported forms
  - Add dedicated validation/runtime tests

## Issue 2: Symbol Ecosystem Completeness Audit ([#779](https://github.com/tomacox74/js2il/issues/779))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Validate symbol-keyed reflection behavior (ordering, descriptors, enumeration)
  - Harden well-known-symbol-dependent behavior across iterables and built-ins
  - Add execution tests for symbol-keyed operations

## Issue 3: Implement Array Iterator Methods and Symbol Iterator Wiring ([#780](https://github.com/tomacox74/js2il/issues/780))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Expose `entries`, `keys`, `values`, and `Array.prototype[Symbol.iterator]`
  - Ensure compatibility with `for..of` and iterator protocol helpers
  - Add tests for direct iterator method usage and interoperability

## Issue 4: Implement WeakRef / FinalizationRegistry Processing Model ([#781](https://github.com/tomacox74/js2il/issues/781))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:low`
- Minimum acceptance:
  - Add a runtime model and host hooks for cleanup-job scheduling
  - Implement `ClearKeptObjects` and related abstract operations
  - Add deterministic or host-safe tests for supported cleanup semantics

## Issue 5: Re-scope ES Module Semantics Gaps ([#772](https://github.com/tomacox74/js2il/issues/772))
- Suggested labels: `enhancement`, `spec:ecma-262`, `modules`, `commonjs`, `priority:medium`
- Current reality:
  - Static `import` / `export` lowering already exists
  - Live import bindings and cyclic coverage exist for common cases
  - The original issue wording now looks broader than the remaining codebase gap
- Minimum acceptance:
  - Either close the issue as effectively fulfilled, or re-scope it to the specific module-record / linking / evaluation semantics still missing
  - If kept open, document the exact unsupported behavior still intended (rather than the already-landed lowering work)
  - Add or refresh targeted tests only for the remaining unsupported semantics
