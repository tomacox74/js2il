# ECMA-262 Coverage Index

Clause index of ECMA-262 (tc39.es) cross-labeled using JS2IL current coverage tracking.

Important:
- Lists clause numbers/titles/links only (no spec text).
- Status comes from the per-section JSON docs (e.g. `docs/ECMA262/**/Section*.json`).
- `Untracked` means not represented in the coverage matrix yet, not necessarily unsupported.

## Status legend
- `Supported`: Implemented and expected to behave correctly for typical usage.
- `Supported with Limitations`: Safe for general/daily-driver use, but has known edge-case/spec-corner gaps (documented in subsection notes).
- `Incomplete`: Some implementation exists, but missing core semantics and not safe to rely on broadly.
- `Not Yet Supported`: Not implemented (or intentionally rejected by validator) for the documented scope.
- `N/A (informational)`: Spec clause is informational/organizational (not a JS runtime/compiler feature).
- `Untracked`: Not evaluated/documented yet; may work, but not claimed.

Notes:
- `Partially Supported` is deprecated legacy wording and is treated as `Supported with Limitations`.

## Summary
- Total clauses indexed: **2176**
- Clauses with tracked status: **104** (Supported: **98**, Supported with Limitations: **5**, Not Yet Supported: **1**)
- Untracked clauses: **2072**

## Sections

| Section | Title | Status | Spec | Document |
|---:|---|---|---|---|
| 1 | Scope | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-scope) | [Section1.md](1/Section1.md) |
| 2 | Conformance | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-conformance) | [Section2.md](2/Section2.md) |
| 3 | Normative References | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-normative-references) | [Section3.md](3/Section3.md) |
| 4 | Overview | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-overview) | [Section4.md](4/Section4.md) |
| 5 | Notational Conventions | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-notational-conventions) | [Section5.md](5/Section5.md) |
| 6 | ECMAScript Data Types and Values | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-data-types-and-values) | [Section6.md](6/Section6.md) |
| 7 | Abstract Operations | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations) | [Section7.md](7/Section7.md) |
| 8 | Syntax-Directed Operations | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-syntax-directed-operations) | [Section8.md](8/Section8.md) |
| 9 | Executable Code and Execution Contexts | Supported | [tc39.es](https://tc39.es/ecma262/#sec-executable-code-and-execution-contexts) | [Section9.md](9/Section9.md) |
| 10 | Ordinary and Exotic Objects Behaviours | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-and-exotic-objects-behaviours) | [Section10.md](10/Section10.md) |
| 11 | ECMAScript Language: Source Text | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-source-code) | [Section11.md](11/Section11.md) |
| 12 | ECMAScript Language: Lexical Grammar | Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-lexical-grammar) | [Section12.md](12/Section12.md) |
| 13 | ECMAScript Language: Expressions | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-expressions) | [Section13.md](13/Section13.md) |
| 14 | ECMAScript Language: Statements and Declarations | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-statements-and-declarations) | [Section14.md](14/Section14.md) |
| 15 | ECMAScript Language: Functions and Classes | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-functions-and-classes) | [Section15.md](15/Section15.md) |
| 16 | ECMAScript Language: Scripts and Modules | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-scripts-and-modules) | [Section16.md](16/Section16.md) |
| 17 | Error Handling and Language Extensions | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-error-handling-and-language-extensions) | [Section17.md](17/Section17.md) |
| 18 | ECMAScript Standard Built-in Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-standard-built-in-objects) | [Section18.md](18/Section18.md) |
| 19 | The Global Object | Supported | [tc39.es](https://tc39.es/ecma262/#sec-global-object) | [Section19.md](19/Section19.md) |
| 20 | Fundamental Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-fundamental-objects) | [Section20.md](20/Section20.md) |
| 21 | Numbers and Dates | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-numbers-and-dates) | [Section21.md](21/Section21.md) |
| 22 | Text Processing | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-text-processing) | [Section22.md](22/Section22.md) |
| 23 | Indexed Collections | Supported | [tc39.es](https://tc39.es/ecma262/#sec-indexed-collections) | [Section23.md](23/Section23.md) |
| 24 | Keyed Collections | Supported | [tc39.es](https://tc39.es/ecma262/#sec-keyed-collections) | [Section24.md](24/Section24.md) |
| 25 | Structured Data | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-structured-data) | [Section25.md](25/Section25.md) |
| 26 | Managing Memory | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-managing-memory) | [Section26.md](26/Section26.md) |
| 27 | Control Abstraction Objects | Supported | [tc39.es](https://tc39.es/ecma262/#sec-control-abstraction-objects) | [Section27.md](27/Section27.md) |
| 28 | Reflection | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-reflection) | [Section28.md](28/Section28.md) |
| 29 | Memory Model | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-memory-model) | [Section29.md](29/Section29.md) |

