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
| 1 | ES Modules: `import`/`export`, module linking/evaluation | 16.x, 9.1.1.5, 9.1.2.6 | Not Yet Supported / rejected in validator |
| 2 | ArrayBuffer/DataView-backed typed array semantics | 23.2.5.1.3 and related typed-array AOs | Not Yet Supported |
| 3 | Full `%TypedArray%` surface (constructors + prototype methods) | 23.2.x | Incomplete |
| 4 | RegExp parity for modern behavior (`u`/`y`/`d`/`v`, symbol methods) | 22.2.6+, 22.2.7+ | Incomplete |
| 5 | Ordinary object internal method invariants (`[[DefineOwnProperty]]`, `[[GetOwnProperty]]`, keys/descriptor rules) | 10.1.x | Incomplete / Not Yet Supported |
| 6 | Object integrity APIs (`preventExtensions`, `seal`, `freeze`, `is*`) | 20.1.2.6, 20.1.2.16-22 | Not Yet Supported |
| 7 | Function constructor and dynamic function creation (`new Function`) | 20.2.1.x | Not Yet Supported |
| 8 | Symbol ecosystem completeness (symbol-keyed property introspection + well-known-symbol parity) | 20.1.2.11, 20.4.x, symbol-driven APIs | Incomplete / Not Yet Supported |
| 9 | Array iterator method surface (`entries`, `keys`, `values`, `Array.prototype[Symbol.iterator]`) | 23.1.3.5, 23.1.3.19, 23.1.3.38, 23.1.3.40 | Not Yet Supported |
| 10 | WeakRef/FinalizationRegistry processing model and cleanup jobs | 9.9-9.13 | Not Yet Supported |

## Issue-ready Backlog Stubs

## Issue 1: Implement ES Module Pipeline (import/export)
- Suggested labels: `enhancement`, `ecma262`, `modules`, `priority-p0`, `tracking-issues`
- Minimum acceptance:
  - Parse and validate `import`/`export` syntax
  - Implement module environment records and module evaluation order
  - Add execution + generator tests for import/export flows

## Issue 2: Implement ArrayBuffer/DataView-backed TypedArray Semantics
- Suggested labels: `enhancement`, `ecma262`, `typedarray`, `priority-p0`, `tracking-issues`
- Minimum acceptance:
  - Support ArrayBuffer-backed typed array construction paths
  - Align byteOffset/byteLength semantics for supported typed arrays
  - Add conformance-style tests for construction and bounds behavior

## Issue 3: Complete `%TypedArray%` Constructors and Prototype Methods
- Suggested labels: `enhancement`, `ecma262`, `typedarray`, `priority-p0`, `tracking-issues`
- Minimum acceptance:
  - Implement static constructors (`from`, `of`) and core prototype operations
  - Preserve current Int32Array behavior while broadening type coverage
  - Add method-level execution/generator tests

## Issue 4: Close RegExp Semantics Gaps
- Suggested labels: `enhancement`, `ecma262`, `regexp`, `priority-p0`, `tracking-issues`
- Minimum acceptance:
  - Add `u`/`y` behavior and symbol methods (`@@match`, `@@replace`, `@@search`, `@@split`)
  - Improve `lastIndex` and iteration semantics to spec-aligned behavior
  - Add targeted tests for modern RegExp usage

## Issue 5: Implement Ordinary Object Internal Invariants
- Suggested labels: `enhancement`, `ecma262`, `object-model`, `priority-p1`, `tracking-issues`
- Minimum acceptance:
  - Implement missing ordinary object internal methods and descriptor compatibility checks
  - Align own-key enumeration behavior for descriptor-driven objects
  - Add tests for descriptor edge cases and invariant enforcement

## Issue 6: Implement Object Integrity API Family
- Suggested labels: `enhancement`, `ecma262`, `object-model`, `priority-p1`, `tracking-issues`
- Minimum acceptance:
  - Implement `Object.preventExtensions`, `seal`, `freeze`, `isExtensible`, `isSealed`, `isFrozen`
  - Integrate with descriptor/invariant model from Issue 5
  - Add behavioral tests for mutable vs immutable transitions

## Issue 7: Implement Function Constructor (`new Function`)
- Suggested labels: `enhancement`, `ecma262`, `function`, `priority-p1`, `tracking-issues`
- Minimum acceptance:
  - Implement guarded dynamic function creation path
  - Define runtime constraints and diagnostics for unsupported forms
  - Add dedicated validation/runtime tests

## Issue 8: Complete Symbol and Symbol-keyed Property Surface
- Suggested labels: `enhancement`, `ecma262`, `symbol`, `priority-p1`, `tracking-issues`
- Minimum acceptance:
  - Implement symbol-key introspection (`Object.getOwnPropertySymbols`) and key ordering integration
  - Harden well-known-symbol dependent behavior across iterables and built-ins
  - Add execution tests for symbol-keyed operations

## Issue 9: Implement Array Iterator Methods and Symbol Iterator Wiring
- Suggested labels: `enhancement`, `ecma262`, `array`, `iterators`, `priority-p1`, `tracking-issues`
- Minimum acceptance:
  - Expose `entries`, `keys`, `values`, and `Array.prototype[Symbol.iterator]`
  - Ensure compatibility with `for..of` and iterator protocol helpers
  - Add tests for direct iterator method usage and interoperability

## Issue 10: Implement WeakRef/FinalizationRegistry Processing Model
- Suggested labels: `enhancement`, `ecma262`, `memory`, `priority-p2`, `tracking-issues`
- Minimum acceptance:
  - Add runtime model and host hooks for cleanup job scheduling
  - Implement `ClearKeptObjects` / related abstract operations
  - Add deterministic/host-safe tests for supported cleanup semantics

