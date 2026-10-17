<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.5: Generator Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-16T19:43:15Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.5 | Generator Function Definitions | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.5.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions-static-semantics-early-errors) |
| 15.5.2 | Runtime Semantics: EvaluateGeneratorBody | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluategeneratorbody) |
| 15.5.3 | Runtime Semantics: InstantiateGeneratorFunctionObject | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiategeneratorfunctionobject) |
| 15.5.4 | Runtime Semantics: InstantiateGeneratorFunctionExpression | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiategeneratorfunctionexpression) |
| 15.5.5 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 15.5 ([tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Statement syntax parse-negative conformance expansion | Supported with Limitations | `tests/Jroc.Test262.Tests/language/statements/StatementSyntaxConformance8BatchParseTests.cs` | `test/language/statements/generators/array-destructuring-param-strict-body.js`<br>`test/language/statements/generators/dflt-params-duplicates.js` | The native Test262 harness verifies 32 additional pinned parse-negative statement fixtures (32 `generators`). All 62 metadata-selected variants passed the catalog preflight under one current compiler provenance; no compiler, runtime, or harness changes were required. |

### 15.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Generator function declaration/expression (function*) | Supported with Limitations | [`Generator_BasicNext.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_BasicNext.js)<br>[`Generator_GeneratedFunctionObject_Semantics.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_GeneratedFunctionObject_Semantics.js)<br>[`arguments-with-arguments-lex.js`](../../../tests/Jroc.Test262.Tests/language/expressions/generators/JavaScript/arguments-with-arguments-lex.js)<br>`tests/Jroc.Test262.Tests/language/expressions/generators/dstr/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/generators/GeneratorFunctionExpressionConformance11BatchTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/generators/dstr/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/generators/GeneratorFunctionDeclarationConformance11BatchTests.cs` | suite `language.expressions.generators`<br>suite `language.statements.generators`<br>`test/language/expressions/generators/dstr/ary-ptrn-elem-id-iter-val.js`<br>`test/language/expressions/generators/dstr/ary-ptrn-elem-obj-val-null.js`<br>`test/language/expressions/generators/yield-star-before-newline.js`<br>`test/language/statements/generators/dstr/obj-init-null.js`<br>`test/language/statements/generators/dstr/dflt-obj-ptrn-rest-getter.js`<br>`test/language/statements/generators/restricted-properties.js` | Generator declarations, expressions, object methods, and class methods materialize as generated non-constructable JsFunctionObject instances. Every call creates a fresh iterator scope/state machine; captures are shared through environments while mutable execution state remains isolated per invocation. The native Test262 harness verifies 346 earlier generator declaration/expression fixtures and 100 additional pinned destructuring cases that failed before this fix. Default and destructured parameters now bind at call time, throwing synchronously while the generator body remains suspended; initialized locals survive until the first resume. Named generator-expression self-bindings initialize once before parameter defaults and persist across deferred body execution and subsequent steps. Throw/return propagation through try/finally remains limited. |
| yield expression | Supported | [`Generator_BasicNext.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_BasicNext.js)<br>[`captured-free-vars.js`](../../../tests/Jroc.Test262.Tests/language/expressions/yield/JavaScript/captured-free-vars.js)<br>`tests/Jroc.Test262.Tests/language/expressions/yield/RelatedLanguageConformance12BatchTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/yield/YieldCompletionExecutionTests.cs` | `test/language/expressions/yield/arguments-object-attributes.js`<br>`test/language/expressions/yield/from-with.js`<br>`test/language/expressions/yield/within-for.js`<br>`test/language/expressions/yield/rhs-unresolvable.js` | Supports yielding values, parent-scope captures, resumption via next(), and operand exceptions caught by surrounding generator handlers. All 63 pinned yield-expression cases pass all 123 metadata-required native variants, including three parse-negative grammar cases. This is development-branch validation, not central trusted acceptance; none of these cases requires eval. |
| yield* delegation | Supported | [`Generator_YieldStar_ArrayBasic.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_YieldStar_ArrayBasic.js)<br>[`Generator_YieldStar_NestedGenerator.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_YieldStar_NestedGenerator.js)<br>[`Generator_YieldStar_PassNextValue.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_YieldStar_PassNextValue.js)<br>[`Generator_YieldStar_ReturnForwards.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_YieldStar_ReturnForwards.js)<br>[`star-array.js`](../../../tests/Jroc.Test262.Tests/language/expressions/yield/JavaScript/star-array.js)<br>[`star-iterable.js`](../../../tests/Jroc.Test262.Tests/language/expressions/yield/JavaScript/star-iterable.js)<br>`tests/Jroc.Test262.Tests/language/expressions/yield/YieldCompletionExecutionTests.cs` | `test/language/expressions/yield/star-rhs-iter-nrml-next-invoke.js`<br>`test/language/expressions/yield/star-rhs-iter-nrml-res-value-final.js`<br>`test/language/expressions/yield/star-rhs-iter-rtrn-res-value-final.js`<br>`test/language/expressions/yield/star-rhs-iter-thrw-thrw-invoke.js`<br>`test/language/expressions/yield/star-rhs-iter-thrw-violation-rtrn-invoke.js`<br>`test/language/expressions/yield/star-return-is-null.js`<br>`test/language/expressions/yield/star-throw-is-null.js` | Synchronous delegation retains the original iterator and cached next method, forwards next/return/throw values with the original receiver and correct argument count, validates returned objects, and preserves raw non-final iterator results. Missing or null return methods complete the outer return; missing or null throw methods close the iterator before reporting a TypeError. Operand/protocol/getter exceptions reach surrounding catch/finally handlers, and delegated return completion runs finally without resuming ordinary execution. The 36 additional pinned cases pass every required native variant. |

