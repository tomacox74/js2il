# ECMA-262 Top Missing Features Backlog

> **Last Updated**: 2026-03-08
> Purpose: capture the highest-value unsupported or still-incomplete ECMA-262 features to drive issue execution planning.
> Source basis: current branch/runtime/compiler state, current `docs\ECMA262\**\Section*.json` status tracking, and targeted behavior checks.

## Current state changes since the previous draft

- **Issue #773 is no longer backlog work.** ArrayBuffer and DataView are implemented and documented, so the old "typed arrays need ArrayBuffer/DataView foundation" item is stale.
- **Issue #774's stated acceptance bar is now covered on the current branch.** The current branch now has ArrayBuffer-backed `Int32Array`, `Uint8Array`, and `Float64Array`, shared view properties (`buffer`, `byteOffset`, `byteLength`, `length`, `BYTES_PER_ELEMENT`), static `from`/`of`, shared search/callback/mutation helpers, and iterator/metadata basics (`values`, `keys`, `entries`, `%Symbol.iterator%`, `%Symbol.toStringTag%`) with focused tests/docs.
- **Issue #772 no longer reads like the top missing feature.** The codebase already supports static `import`/`export` lowering plus live-binding and cyclic coverage for common cases, so the remaining gap looks more like re-scope / hygiene unless broader module-record semantics are still explicitly desired.

## Ranking Criteria

- Real-world unblock impact (npm/tooling/runtime compatibility)
- Breadth of spec surface still missing
- Dependency leverage (enables multiple later features)
- Testability and rollout feasibility

## Current ranked backlog (recommended order)

| Rank | Backlog item | Primary spec areas | Current status signal |
|---:|---|---|---|
| 1 | [RegExp parity for modern behavior (`u` / `y` / `d` / `v`, symbol methods)](https://github.com/tomacox74/js2il/issues/775) | 22.2.6+, 22.2.7+ | Incomplete |
| 2 | [Ordinary object internal method invariants (`[[DefineOwnProperty]]`, `[[GetOwnProperty]]`, key ordering, descriptor rules)](https://github.com/tomacox74/js2il/issues/776) | 10.1.x | Incomplete / Not Yet Supported |
| 3 | [Object integrity APIs semantics audit (`preventExtensions`, `seal`, `freeze`, `is*`)](https://github.com/tomacox74/js2il/issues/777) | 20.1.2.6, 20.1.2.16-22 | Supported with Limitations (APIs exist; invariant enforcement still needs audit/closure) |
| 4 | [Function constructor and dynamic function creation (`new Function`)](https://github.com/tomacox74/js2il/issues/778) | 20.2.1.x | Not Yet Supported |
| 5 | [Symbol ecosystem completeness (symbol-keyed reflection + well-known-symbol parity)](https://github.com/tomacox74/js2il/issues/779) | 20.1.2.11, 20.4.x, symbol-driven APIs | Supported with Limitations |
| 6 | [Array iterator method surface (`entries`, `keys`, `values`, `Array.prototype[Symbol.iterator]`)](https://github.com/tomacox74/js2il/issues/780) | 23.1.3.5, 23.1.3.19, 23.1.3.38, 23.1.3.40 | Partially supported (`for..of` works; direct prototype methods remain incomplete) |
| 7 | [WeakRef / FinalizationRegistry processing model and cleanup jobs](https://github.com/tomacox74/js2il/issues/781) | 9.9-9.13 | Not Yet Supported |
| 8 | [Re-scope or close the remaining ES module semantics issue](https://github.com/tomacox74/js2il/issues/772) | 16.2.x | Supported with Limitations / likely issue hygiene unless broader module-record semantics are still required |

## TypedArray follow-up note

- The current branch + draft PR #818 appear to satisfy issue #774's explicit acceptance criteria.
- Remaining typed-array gaps are now follow-up polish items rather than one of the top missing features on this branch:
  - constructor/prototype metadata fidelity (`%TypedArray%.prototype`, `.constructor`, static `BYTES_PER_ELEMENT`)
  - detached-buffer semantics and species hooks
  - broader family parity beyond the currently targeted `Int32Array`, `Uint8Array`, and `Float64Array`

## Issue-ready backlog stubs

## Issue 1: Close RegExp Semantics Gaps ([#775](https://github.com/tomacox74/js2il/issues/775))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:high`
- Minimum acceptance:
  - Add `u` / `y` behavior and symbol methods (`@@match`, `@@replace`, `@@search`, `@@split`)
  - Improve `lastIndex` and iteration semantics toward spec-aligned behavior
  - Add targeted tests for modern RegExp usage

## Issue 2: Implement Ordinary Object Internal Invariants ([#776](https://github.com/tomacox74/js2il/issues/776))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Implement missing ordinary object internal methods and descriptor compatibility checks
  - Align own-key enumeration behavior for descriptor-driven objects
  - Add tests for descriptor edge cases and invariant enforcement

## TypedArray follow-up note ([#774](https://github.com/tomacox74/js2il/issues/774) after draft PR #818)
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:high`
- Current branch / draft PR #818 now provides:
  - ArrayBuffer-backed `Int32Array`, `Uint8Array`, and `Float64Array`
  - Shared `buffer` / `byteOffset` / `byteLength` / `length` / `BYTES_PER_ELEMENT`
  - Shared `set`, `at`, `includes`, `indexOf`, `lastIndexOf`, `slice`, `subarray`, `values()`, `keys()`, `entries()`
  - Shared callback/mutation helpers (`every`, `some`, `find`, `findIndex`, `forEach`, `map`, `filter`, `reduce`, `fill`, `reverse`, `join`)
  - `%Symbol.iterator%` / `%Symbol.toStringTag%` basics, preserved Int32Array compiler fast paths, and broader execution/generator coverage
- Follow-up only if the issue is intentionally expanded beyond its current acceptance criteria:
  - constructor/prototype metadata fidelity (`%TypedArray%.prototype`, `.constructor`, static `BYTES_PER_ELEMENT`)
  - detached-buffer semantics and species hooks
  - broader family parity beyond the currently targeted three families

## Issue 4: Object Integrity APIs Semantics Audit ([#777](https://github.com/tomacox74/js2il/issues/777))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Audit and close gaps in `Object.preventExtensions`, `seal`, `freeze`, and `is*` semantics
  - Integrate with the descriptor/invariant model from Issue 2
  - Add behavioral tests for mutable vs immutable transitions

## Issue 5: Implement Function Constructor (`new Function`) ([#778](https://github.com/tomacox74/js2il/issues/778))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Implement a guarded function-constructor creation path (an AOT-friendly subset is acceptable)
  - Define runtime constraints and diagnostics for unsupported forms
  - Add dedicated validation/runtime tests

## Issue 6: Symbol Ecosystem Completeness Audit ([#779](https://github.com/tomacox74/js2il/issues/779))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Validate symbol-keyed reflection behavior (ordering, descriptors, enumeration)
  - Harden well-known-symbol-dependent behavior across iterables and built-ins
  - Add execution tests for symbol-keyed operations

## Issue 7: Implement Array Iterator Methods and Symbol Iterator Wiring ([#780](https://github.com/tomacox74/js2il/issues/780))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Expose `entries`, `keys`, `values`, and `Array.prototype[Symbol.iterator]`
  - Ensure compatibility with `for..of` and iterator protocol helpers
  - Add tests for direct iterator method usage and interoperability

## Issue 8: Implement WeakRef / FinalizationRegistry Processing Model ([#781](https://github.com/tomacox74/js2il/issues/781))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:low`
- Minimum acceptance:
  - Add a runtime model and host hooks for cleanup-job scheduling
  - Implement `ClearKeptObjects` and related abstract operations
  - Add deterministic or host-safe tests for supported cleanup semantics

## Issue 9: Re-scope ES Module Semantics Gaps ([#772](https://github.com/tomacox74/js2il/issues/772))
- Suggested labels: `enhancement`, `spec:ecma-262`, `modules`, `commonjs`, `priority:medium`
- Current reality:
  - Static `import` / `export` lowering already exists
  - Live import bindings and cyclic coverage exist for common cases
  - The original issue wording now looks broader than the remaining codebase gap
- Minimum acceptance:
  - Either close the issue as effectively fulfilled, or re-scope it to the specific module-record / linking / evaluation semantics still missing
  - If kept open, document the exact unsupported behavior still intended (rather than the already-landed lowering work)
  - Add or refresh targeted tests only for the remaining unsupported semantics
