# ECMA-262 Top Missing Features Backlog (2026-02-21)

> Purpose: capture the highest-value unsupported/incomplete ECMA-262 features to drive issue creation and execution planning.
> Source basis: current `docs/ECMA262/**/Section*.json` + subsection markdown status, plus runtime/validator behavior checks.

## Ranking Criteria

- Real-world unblock impact (npm/tooling/runtime compatibility)
- Breadth of spec surface currently missing
- Dependency leverage (enables multiple later features)
- Testability and rollout feasibility

## Top 10 Backlog (Recommended Order)

| Rank | Backlog item | Primary spec areas | Current status signal |
|---:|---|---|---|
| 1 | [ES Modules: spec-accurate module records/linking/evaluation (beyond current `import`/`export` lowering)](https://github.com/tomacox74/js2il/issues/772) | 16.2.1.x, 16.2.1.5-16.2.1.7 | Supported with Limitations (static import/export rewritten to CommonJS `require`; no real module records / live bindings) |
| 2 | [ArrayBuffer/DataView-backed typed array semantics](https://github.com/tomacox74/js2il/issues/773) | 23.2.5.1.3 and related typed-array AOs | Not Yet Supported |
| 3 | [Full `%TypedArray%` surface (constructors + prototype methods)](https://github.com/tomacox74/js2il/issues/774) | 23.2.x | Incomplete |
| 4 | [RegExp parity for modern behavior (`u`/`y`/`d`/`v`, symbol methods)](https://github.com/tomacox74/js2il/issues/775) | 22.2.6+, 22.2.7+ | Incomplete |
| 5 | [Ordinary object internal method invariants (`[[DefineOwnProperty]]`, `[[GetOwnProperty]]`, keys/descriptor rules)](https://github.com/tomacox74/js2il/issues/776) | 10.1.x | Incomplete / Not Yet Supported |
| 6 | [Object integrity APIs (`preventExtensions`, `seal`, `freeze`, `is*`)](https://github.com/tomacox74/js2il/issues/777) | 20.1.2.6, 20.1.2.16-22 | Supported with Limitations (implemented; needs invariant enforcement + tests) |
| 7 | [Function constructor and dynamic function creation (`new Function`)](https://github.com/tomacox74/js2il/issues/778) | 20.2.1.x | Not Yet Supported |
| 8 | [Symbol ecosystem completeness (symbol-keyed property introspection + well-known-symbol parity)](https://github.com/tomacox74/js2il/issues/779) | 20.1.2.11, 20.4.x, symbol-driven APIs | Supported with Limitations (Symbol + `Object.getOwnPropertySymbols` exist; remaining ecosystem parity gaps) |
| 9 | [Array iterator method surface (`entries`, `keys`, `values`, `Array.prototype[Symbol.iterator]`)](https://github.com/tomacox74/js2il/issues/780) | 23.1.3.5, 23.1.3.19, 23.1.3.38, 23.1.3.40 | Partially supported (for..of works via runtime iterator protocol; prototype methods not exposed) |
| 10 | [WeakRef/FinalizationRegistry processing model and cleanup jobs](https://github.com/tomacox74/js2il/issues/781) | 9.9-9.13 | Not Yet Supported |

## Issue-ready Backlog Stubs

## Issue 1: Close ES Module Semantics Gaps (beyond current import/export lowering) ([#772](https://github.com/tomacox74/js2il/issues/772))
- Suggested labels: `enhancement`, `spec:ecma-262`, `modules`, `commonjs`, `priority:high`
- Minimum acceptance:
  - Keep supporting `import`/`export` syntax (currently lowered to CommonJS `require` + export getters)
  - Add an implementation strategy that enables live bindings and cyclic ESM graphs
  - Add execution + generator tests that cover: live binding behavior, circular dependencies, and export namespace objects

## Issue 2: Implement ArrayBuffer/DataView-backed TypedArray Semantics ([#773](https://github.com/tomacox74/js2il/issues/773))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:high`
- Minimum acceptance:
  - Support ArrayBuffer-backed typed array construction paths
  - Align byteOffset/byteLength semantics for supported typed arrays
  - Add conformance-style tests for construction and bounds behavior

## Issue 3: Complete `%TypedArray%` Constructors and Prototype Methods ([#774](https://github.com/tomacox74/js2il/issues/774))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:high`
- Minimum acceptance:
  - Implement static constructors (`from`, `of`) and core prototype operations
  - Preserve current Int32Array behavior while broadening type coverage
  - Add method-level execution/generator tests

## Issue 4: Close RegExp Semantics Gaps ([#775](https://github.com/tomacox74/js2il/issues/775))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:high`
- Minimum acceptance:
  - Add `u`/`y` behavior and symbol methods (`@@match`, `@@replace`, `@@search`, `@@split`)
  - Improve `lastIndex` and iteration semantics to spec-aligned behavior
  - Add targeted tests for modern RegExp usage

## Issue 5: Implement Ordinary Object Internal Invariants ([#776](https://github.com/tomacox74/js2il/issues/776))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Implement missing ordinary object internal methods and descriptor compatibility checks
  - Align own-key enumeration behavior for descriptor-driven objects
  - Add tests for descriptor edge cases and invariant enforcement

## Issue 6: Object integrity APIs semantics audit (freeze/seal/preventExtensions/is*) ([#777](https://github.com/tomacox74/js2il/issues/777))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Audit and close gaps in `Object.preventExtensions/seal/freeze/is*` semantics
  - Integrate with descriptor/invariant model from Issue 5
  - Add behavioral tests for mutable vs immutable transitions

## Issue 7: Implement Function Constructor (`new Function`) ([#778](https://github.com/tomacox74/js2il/issues/778))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Implement guarded function-constructor creation path (AOT-friendly subset is OK)
  - Define runtime constraints and diagnostics for unsupported forms
  - Add dedicated validation/runtime tests

## Issue 8: Symbol ecosystem completeness audit (well-known symbols + symbol-key introspection semantics) ([#779](https://github.com/tomacox74/js2il/issues/779))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Validate symbol-keyed reflection behavior (ordering, descriptors, enumeration)
  - Harden well-known-symbol-dependent behavior across iterables and built-ins
  - Add execution tests for symbol-keyed operations

## Issue 9: Implement Array Iterator Methods and Symbol Iterator Wiring ([#780](https://github.com/tomacox74/js2il/issues/780))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:medium`
- Minimum acceptance:
  - Expose `entries`, `keys`, `values`, and `Array.prototype[Symbol.iterator]`
  - Ensure compatibility with `for..of` and iterator protocol helpers
  - Add tests for direct iterator method usage and interoperability

## Issue 10: Implement WeakRef/FinalizationRegistry Processing Model ([#781](https://github.com/tomacox74/js2il/issues/781))
- Suggested labels: `enhancement`, `spec:ecma-262`, `priority:low`
- Minimum acceptance:
  - Add runtime model and host hooks for cleanup job scheduling
  - Implement `ClearKeptObjects` / related abstract operations
  - Add deterministic/host-safe tests for supported cleanup semantics

