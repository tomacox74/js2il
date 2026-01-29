<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.3: Arrow Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.3 | Arrow Function Definitions | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arrow-function-definitions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.3.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arrow-function-definitions-static-semantics-early-errors) |
| 15.3.2 | Static Semantics: ConciseBodyContainsUseStrict | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-concisebodycontainsusestrict) |
| 15.3.3 | Runtime Semantics: EvaluateConciseBody | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluateconcisebody) |
| 15.3.4 | Runtime Semantics: InstantiateArrowFunctionExpression | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiatearrowfunctionexpression) |
| 15.3.5 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arrow-function-definitions-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 15.3 ([tc39.es](https://tc39.es/ecma262/#sec-arrow-function-definitions))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| arrow functions (=>) - basic syntax, closure, and invocation | Supported | [`ArrowFunction_SimpleExpression.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_SimpleExpression.js)<br>[`ArrowFunction_BlockBody_Return.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_BlockBody_Return.js)<br>[`ArrowFunction_CapturesOuterVariable.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_CapturesOuterVariable.js)<br>[`ArrowFunction_ClosureMutatesOuterVariable.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_ClosureMutatesOuterVariable.js)<br>[`ArrowFunction_GlobalFunctionWithMultipleParameters.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionWithMultipleParameters.js)<br>[`ArrowFunction_NestedFunctionAccessesMultipleScopes.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_NestedFunctionAccessesMultipleScopes.js)<br>[`ArrowFunction_GlobalFunctionCallsGlobalFunction.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionCallsGlobalFunction.js)<br>[`ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal.js)<br>[`ArrowFunction_DefaultParameterValue.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_DefaultParameterValue.js)<br>[`ArrowFunction_DefaultParameterExpression.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_DefaultParameterExpression.js)<br>[`ArrowFunction_ParameterDestructuring_Object.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_ParameterDestructuring_Object.js) | Arrow functions compile via the IR pipeline and are emitted as callable methods (see JavaScriptArrowFunctionGenerator). Most real-world uses (callbacks/closures) work, but lexical 'this' is not yet supported (tracked separately). |

### 15.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiatearrowfunctionexpression))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| lexical this for arrow functions | Not Yet Supported |  | JS2IL currently rejects ThisExpression inside ArrowFunctionExpression during validation (see JavaScriptAstValidator: AllowsThis = node is not ArrowFunctionExpression). This prevents correct lexical-this capture/inheritance (see GitHub issue #219). |

