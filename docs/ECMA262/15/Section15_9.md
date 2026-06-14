<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.9: Async Arrow Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T19:49:58Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.9 | Async Arrow Function Definitions | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-arrow-function-definitions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.9.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-arrow-function-definitions-static-semantics-early-errors) |
| 15.9.2 | Static Semantics: AsyncConciseBodyContainsUseStrict | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-asyncconcisebodycontainsusestrict) |
| 15.9.3 | Runtime Semantics: EvaluateAsyncConciseBody | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluateasyncconcisebody) |
| 15.9.4 | Runtime Semantics: InstantiateAsyncArrowFunctionExpression | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncarrowfunctionexpression) |
| 15.9.5 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-arrow-function-definitions-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 15.9 ([tc39.es](https://tc39.es/ecma262/#sec-async-arrow-function-definitions))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| async arrow functions (async () => ...) with await | Supported | [`Async_ArrowFunction_SimpleAwait.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ArrowFunction_SimpleAwait.js)<br>[`Async_ArrowFunction_LexicalThis.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ArrowFunction_LexicalThis.js)<br>[`arrow-returns-promise.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-arrow-function/JavaScript/arrow-returns-promise.js)<br>[`dflt-params-arg-val-not-undefined.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-arrow-function/JavaScript/dflt-params-arg-val-not-undefined.js)<br>[`dflt-params-arg-val-undefined.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-arrow-function/JavaScript/dflt-params-arg-val-undefined.js)<br>[`dflt-params-ref-prior.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-arrow-function/JavaScript/dflt-params-ref-prior.js)<br>[`try-return-finally-throw.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-arrow-function/JavaScript/try-return-finally-throw.js)<br>[`try-throw-finally-return.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-arrow-function/JavaScript/try-throw-finally-return.js) |  | Covered by Async and test262 fixtures. Async arrow functions compile and run, including await, Promise chaining, default-parameter binding, no-await Promise completion/rejection, and finally completion overrides. |

### 15.9.2 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-asyncconcisebodycontainsusestrict))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| directive prologue / strict mode ("use strict") semantics | Supported with Limitations |  |  | JROC parses directive prologues but does not currently aim for full strict-mode semantics/early errors across the language. This clause is tracked as limited until a dedicated strict-mode test matrix exists. |

### 15.9.4 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncarrowfunctionexpression))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| lexical this for async arrow functions | Supported | [`Async_ArrowFunction_LexicalThis.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ArrowFunction_LexicalThis.js) |  | Async arrow functions preserve lexical this across await (GitHub issue #219). |

