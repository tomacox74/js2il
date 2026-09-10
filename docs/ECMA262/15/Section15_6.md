<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.6: Async Generator Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-09-10T20:13:53Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.6 | Async Generator Function Definitions | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-generator-function-definitions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.6.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-generator-function-definitions-static-semantics-early-errors) |
| 15.6.2 | Runtime Semantics: EvaluateAsyncGeneratorBody | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluateasyncgeneratorbody) |
| 15.6.3 | Runtime Semantics: InstantiateAsyncGeneratorFunctionObject | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncgeneratorfunctionobject) |
| 15.6.4 | Runtime Semantics: InstantiateAsyncGeneratorFunctionExpression | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncgeneratorfunctionexpression) |
| 15.6.5 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgenerator-definitions-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 15.6 ([tc39.es](https://tc39.es/ecma262/#sec-async-generator-function-definitions))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| async generator functions (async function*) | Supported with Limitations | [`AsyncGenerator_BasicNext.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_BasicNext.js)<br>[`AsyncGenerator_ForAwaitOf.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_ForAwaitOf.js)<br>[`AsyncGenerator_YieldAwait.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_YieldAwait.js)<br>[`AsyncGenerator_GeneratedFunctionObject_Semantics.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_GeneratedFunctionObject_Semantics.js)<br>[`named-dflt-obj-ptrn-prop-obj-value-undef.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-generator/JavaScript/dstr/named-dflt-obj-ptrn-prop-obj-value-undef.js)<br>`tests/Jroc.Test262.Tests/language/expressions/async-generator/AsyncGeneratorFunctionExpressionConformance11BatchTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/async-generator/AsyncGeneratorFunctionDeclarationConformance11BatchTests.cs` | suite `language.expressions.async_generator`<br>suite `language.statements.async_generator`<br>`test/language/expressions/async-generator/default-proto.js`<br>`test/language/statements/async-generator/dstr/ary-init-iter-get-err.js`<br>`test/language/statements/async-generator/dstr/dflt-obj-ptrn-prop-id-init-throws.js` | Async generator declarations, expressions, object methods, and class methods materialize as generated non-constructable JsFunctionObject instances. Every call creates an independent AsyncGeneratorScope and AsyncGeneratorObject while retaining shared lexical captures and the call-time receiver. Parameter binding runs synchronously when called. The native Test262 harness now additionally verifies 61 current-provenance async-generator declaration/expression fixtures, predominantly parameter-destructuring abrupt-completion and static early-error cases. Known limitations: throw() and return() protocol methods have known issues; try/catch/finally with async generators may generate invalid IL in some cases. The native Test262 harness now verifies 143 additional async-generator expression cases, including parameter binding, destructuring, and static early errors. |
| await expression in async generators | Supported | [`AsyncGenerator_YieldAwait.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_YieldAwait.js) |  | Await expressions work within async generator functions, allowing promises to be awaited before yielding values. |
| for await...of with async generators | Supported | [`AsyncGenerator_ForAwaitOf.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_ForAwaitOf.js) |  | for await...of loops correctly consume async generators, awaiting each yielded value. |
| Statement syntax parse-negative conformance expansion | Supported with Limitations | `tests/Jroc.Test262.Tests/language/statements/StatementSyntaxConformance8BatchParseTests.cs` | `test/language/statements/async-generator/array-destructuring-param-strict-body.js`<br>`test/language/statements/async-generator/await-as-binding-identifier-escaped.js` | The native Test262 harness verifies 33 additional pinned parse-negative statement fixtures (33 `async-generator`). All 64 metadata-selected variants passed the catalog preflight under one current compiler provenance; no compiler, runtime, or harness changes were required. |
| yield expression in async generators | Supported | [`AsyncGenerator_BasicNext.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_BasicNext.js) |  | Yield expressions work within async generator functions, suspending execution and returning Promise-wrapped iterator results. |

### 15.6.1 ([tc39.es](https://tc39.es/ecma262/#sec-async-generator-function-definitions-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Async-generator expression early-error conformance expansion | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/ExpressionSyntaxConformance7BatchParseTests.cs` |  | The native Test262 harness verifies 48 additional pinned parse-negative async-generator expression fixtures. They cover parameter and binding restrictions, `await`/`yield` grammar, strict-mode restrictions, and invalid `super` use. |

