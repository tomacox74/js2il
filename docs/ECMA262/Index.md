# ECMA-262 Coverage Index

Clause index of ECMA-262 (tc39.es) cross-labeled using JROC current coverage tracking.

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
- Prototype-chain design/strategy: see [PrototypeChainSupport.md](../compiler/PrototypeChainSupport.md).
- Coverage maintenance workflow: see the [ECMA-262 documentation guide](readme.md).

> Last generated (UTC): 2026-07-25T20:59:24Z

## Summary
- Total top-level sections indexed: **29**
- Top-level sections with tracked status: **28**
- Status breakdown: Supported with Limitations: **13**, Incomplete: **8**, Not Yet Supported: **1**, N/A (informational): **6**, Untracked: **1**
- Untracked top-level sections: **1**

## Test262 Conformance Status

For the current development branch following [JROC v0.12.15](https://github.com/tomacox74/js2il/releases/tag/v0.12.15), Test262 provides the following conformance evidence:

| Conformance status | Tests | Percentage of applicable Test262 tests |
|---|---:|---:|
| Verified passing | 10,232 | **21.21%** |
| Explicitly excluded due to known unsupported behavior | 58 | 0.12% |
| Not yet verified | 37,951 | 78.69% |
| **Total applicable ECMA-262 tests** | **48,241** | **100.00%** |

See the [detailed Test262 conformance breakdown](Test262Conformance.md) for results by language area, expression and statement feature, built-in API, and Annex B feature.

`Verified passing` means the current development branch successfully executes the corresponding Test262 test. `Not yet verified` means no conformance result is currently published for that test; it does not imply either support or failure. The explicitly excluded tests exercise known unsupported behavior, primarily `eval`.

The applicable corpus is taken from the [pinned Test262 revision](https://github.com/tc39/test262/tree/2b2ecead6e828dd9af13a9ec72065e645724a50f/test) and includes the ECMA-262 `annexB`, `built-ins`, and `language` test areas. ECMA-402 internationalization tests, staging proposals, Test262 harness self-tests, and `_FIXTURE.js` support files are outside this ECMA-262 conformance measure. Percentages are file-based; strict and non-strict execution variants are not counted separately.

## Sections

| Section | Title | Status | Spec | Document |
|---:|---|---|---|---|
| 1 | Scope | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-scope) | [Section 1 index](1/Index.md) |
| 2 | Conformance | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-conformance) | [Section 2 index](2/Index.md) |
| 3 | Normative References | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-normative-references) | [Section 3 index](3/Index.md) |
| 4 | Overview | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-overview) | [Section 4 index](4/Index.md) |
| 5 | Notational Conventions | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-notational-conventions) | [Section 5 index](5/Index.md) |
| 6 | ECMAScript Data Types and Values | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-data-types-and-values) | [Section 6 index](6/Index.md) |
| 7 | Abstract Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations) | [Section 7 index](7/Index.md) |
| 8 | Syntax-Directed Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-syntax-directed-operations) | [Section 8 index](8/Index.md) |
| 9 | Executable Code and Execution Contexts | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-executable-code-and-execution-contexts) | [Section 9 index](9/Index.md) |
| 10 | Ordinary and Exotic Objects Behaviours | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-and-exotic-objects-behaviours) | [Section 10 index](10/Index.md) |
| 11 | ECMAScript Language: Source Text | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-source-code) | [Section 11 index](11/Index.md) |
| 12 | ECMAScript Language: Lexical Grammar | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-lexical-grammar) | [Section 12 index](12/Index.md) |
| 13 | ECMAScript Language: Expressions | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-expressions) | [Section 13 index](13/Index.md) |
| 14 | ECMAScript Language: Statements and Declarations | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-statements-and-declarations) | [Section 14 index](14/Index.md) |
| 15 | ECMAScript Language: Functions and Classes | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-functions-and-classes) | [Section 15 index](15/Index.md) |
| 16 | ECMAScript Language: Scripts and Modules | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-scripts-and-modules) | [Section 16 index](16/Index.md) |
| 17 | Error Handling and Language Extensions | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-error-handling-and-language-extensions) | [Section 17 index](17/Index.md) |
| 18 | ECMAScript Standard Built-in Objects | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-standard-built-in-objects) | [Section 18 index](18/Index.md) |
| 19 | The Global Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-object) | [Section 19 index](19/Index.md) |
| 20 | Fundamental Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-fundamental-objects) | [Section 20 index](20/Index.md) |
| 21 | Numbers and Dates | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numbers-and-dates) | [Section 21 index](21/Index.md) |
| 22 | Text Processing | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-text-processing) | [Section 22 index](22/Index.md) |
| 23 | Indexed Collections | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-indexed-collections) | [Section 23 index](23/Index.md) |
| 24 | Keyed Collections | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-keyed-collections) | [Section 24 index](24/Index.md) |
| 25 | Structured Data | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-structured-data) | [Section 25 index](25/Index.md) |
| 26 | Managing Memory | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-managing-memory) | [Section 26 index](26/Index.md) |
| 27 | Control Abstraction Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-control-abstraction-objects) | [Section 27 index](27/Index.md) |
| 28 | Reflection | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-reflection) | [Section 28 index](28/Index.md) |
| 29 | Memory Model | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-memory-model) | [Section 29 index](29/Index.md) |
