<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.9: Async Arrow Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

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

Feature-level support tracking with test script references.

### 15.9 ([tc39.es](https://tc39.es/ecma262/#sec-async-arrow-function-definitions))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| async arrow functions (async () => ...) with await | Supported | [`Async_ArrowFunction_SimpleAwait.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ArrowFunction_SimpleAwait.js)<br>[`Async_ArrowFunction_LexicalThis.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ArrowFunction_LexicalThis.js) | Covered by Async test fixture. Async arrow functions compile and run, including await and Promise chaining. |

### 15.9.4 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncarrowfunctionexpression))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| lexical this for async arrow functions | Supported | [`Async_ArrowFunction_LexicalThis.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ArrowFunction_LexicalThis.js) | Async arrow functions preserve lexical this across await (GitHub issue #219). |

