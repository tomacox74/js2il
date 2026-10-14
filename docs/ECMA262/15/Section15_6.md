<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.6: Async Generator Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-14T21:52:46Z

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
| async generator functions (async function*) | Supported with Limitations | [`AsyncGenerator_BasicNext.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_BasicNext.js)<br>[`AsyncGenerator_ForAwaitOf.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_ForAwaitOf.js)<br>[`AsyncGenerator_YieldAwait.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_YieldAwait.js)<br>[`AsyncGenerator_GeneratedFunctionObject_Semantics.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_GeneratedFunctionObject_Semantics.js)<br>[`named-dflt-obj-ptrn-prop-obj-value-undef.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-generator/JavaScript/dstr/named-dflt-obj-ptrn-prop-obj-value-undef.js)<br>`tests/Jroc.Test262.Tests/language/expressions/async-generator/AsyncGeneratorFunctionExpressionConformance11BatchTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/async-generator/AsyncGeneratorFunctionDeclarationConformance11BatchTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/async-generator/AsyncGeneratorDeclarationCompletionTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/async-generator/dstr/AsyncGeneratorDestructuringCompletionTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/async-generator/forbidden-ext/b1/AsyncGeneratorForbiddenExtensionB1CompletionTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/async-generator/forbidden-ext/b2/AsyncGeneratorForbiddenExtensionB2CompletionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/async-generator/AsyncGeneratorExpressionCompletionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/async-generator/dstr/AsyncGeneratorExpressionDestructuringCompletionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/async-generator/forbidden-ext/b1/AsyncGeneratorExpressionForbiddenExtensionB1CompletionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/async-generator/forbidden-ext/b2/AsyncGeneratorExpressionForbiddenExtensionB2CompletionTests.cs` | suite `language.expressions.async_generator`<br>suite `language.statements.async_generator`<br>`test/language/expressions/async-generator/default-proto.js`<br>`test/language/statements/async-generator/dstr/ary-init-iter-get-err.js`<br>`test/language/statements/async-generator/dstr/dflt-obj-ptrn-prop-id-init-throws.js` | Async generator declarations, expressions, object methods, and class methods materialize as generated non-constructable JsFunctionObject instances. Every call creates an independent AsyncGeneratorScope and AsyncGeneratorObject while retaining shared lexical captures and the call-time receiver. Parameter binding runs synchronously when called. Native Test262 coverage verifies 618 expression cases and 300 statement/declaration cases, including parameter destructuring, defaults, grammar, iterator delegation, restricted properties, with/unscopables, function identity, and return/throw completion behavior. Five expression cases and one declaration case remain unsupported because they require eval. Full cross-realm/brand conformance is not claimed. |
| await expression in async generators | Supported | [`AsyncGenerator_YieldAwait.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_YieldAwait.js) |  | Await expressions work within async generator functions, allowing promises to be awaited before yielding values. |
| for await...of with async generators | Supported | [`AsyncGenerator_ForAwaitOf.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_ForAwaitOf.js) |  | for await...of loops correctly consume async generators, awaiting each yielded value. |
| Statement syntax parse-negative conformance expansion | Supported with Limitations | `tests/Jroc.Test262.Tests/language/statements/StatementSyntaxConformance8BatchParseTests.cs` | `test/language/statements/async-generator/array-destructuring-param-strict-body.js`<br>`test/language/statements/async-generator/await-as-binding-identifier-escaped.js` | The native Test262 harness verifies 33 additional pinned parse-negative statement fixtures (33 `async-generator`). All 64 metadata-selected variants passed the catalog preflight under one current compiler provenance; no compiler, runtime, or harness changes were required. |
| yield expression in async generators | Supported | [`AsyncGenerator_BasicNext.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_BasicNext.js) |  | Ordinary yield expressions await their operands before suspending with a promise-wrapped iterator result. Delegated yield* awaits the iterator result but preserves its value without unwrapping promises supplied by a manually implemented async iterator; sync iterator fallback performs the required value unwrapping. |

### 15.6.1 ([tc39.es](https://tc39.es/ecma262/#sec-async-generator-function-definitions-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Async-generator expression early-error conformance expansion | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/ExpressionSyntaxConformance7BatchParseTests.cs` |  | The native Test262 harness verifies 48 additional pinned parse-negative async-generator expression fixtures. They cover parameter and binding restrictions, `await`/`yield` grammar, strict-mode restrictions, and invalid `super` use. |

### 15.6.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluateasyncgeneratorbody))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Awaited return expressions and delegated abrupt completions | Supported with Limitations | `tests/Jroc.Test262.Tests/language/statements/async-generator/AsyncGeneratorDeclarationCompletionTests.cs` | `test/language/statements/async-generator/return-undefined-implicit-and-explicit.js`<br>`test/language/statements/async-generator/yield-star-promise-not-unwrapped.js`<br>`test/language/statements/async-generator/yield-star-normal-notdone-iter-value-throws.js`<br>`test/language/statements/async-generator/yield-star-return-notdone-iter-value-throws.js`<br>`test/language/statements/async-generator/yield-star-throw-notdone-iter-value-throws.js` | An explicit return expression is awaited even when its value is undefined; a bare return or normal body fallthrough completes without that extra await. Await-result fields are planned consistently for declarations, expressions, dynamic functions, and methods. Synchronous delegated-protocol and value-getter exceptions route to the surrounding generator catch/finally state without placing suspension inside a CLR exception region. |

### 15.6.4 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncgeneratorfunctionexpression))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Named async-generator expression self-binding lifetime | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/async-generator/AsyncGeneratorExpressionCompletionTests.cs`<br>[`AsyncGenerator_GeneratedFunctionObject_Semantics.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_GeneratedFunctionObject_Semantics.js) | `test/language/expressions/async-generator/named-no-strict-reassign-fn-name-in-body.js`<br>`test/language/expressions/async-generator/named-no-strict-reassign-fn-name-in-body-in-arrow.js`<br>`test/language/expressions/async-generator/named-strict-error-reassign-fn-name-in-body.js`<br>`test/language/expressions/async-generator/named-strict-error-reassign-fn-name-in-body-in-arrow.js` | The private self-name is initialized to the created function once during the original call, before parameter defaults, and is retained by the suspended frame and nested closures rather than overwritten by a later resumption's callee. Existing assignment semantics ignore writes in non-strict code and throw TypeError in strict code. Eval-dependent variants remain unsupported. |

