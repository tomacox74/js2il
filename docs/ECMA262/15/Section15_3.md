<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.3: Arrow Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-03T15:15:03Z

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

Feature-level support tracking with repo test references and optional test262 evidence.

### 15.3 ([tc39.es](https://tc39.es/ecma262/#sec-arrow-function-definitions))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| arrow functions (=>) - basic syntax, closure, and invocation | Supported | [`ArrowFunction_SimpleExpression.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_SimpleExpression.js)<br>[`ArrowFunction_BlockBody_Return.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_BlockBody_Return.js)<br>[`ArrowFunction_CapturesOuterVariable.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_CapturesOuterVariable.js)<br>[`ArrowFunction_ClosureMutatesOuterVariable.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_ClosureMutatesOuterVariable.js)<br>[`ArrowFunction_GlobalFunctionWithMultipleParameters.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionWithMultipleParameters.js)<br>[`ArrowFunction_NestedFunctionAccessesMultipleScopes.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_NestedFunctionAccessesMultipleScopes.js)<br>[`ArrowFunction_GlobalFunctionCallsGlobalFunction.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionCallsGlobalFunction.js)<br>[`ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal.js)<br>[`ArrowFunction_DefaultParameterValue.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_DefaultParameterValue.js)<br>[`ArrowFunction_DefaultParameterExpression.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_DefaultParameterExpression.js)<br>[`ArrowFunction_ParameterDestructuring_Object.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_ParameterDestructuring_Object.js)<br>[`Function_Arguments_Basics.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Arguments_Basics.js) | suite `pr`<br>suite `nightly`<br>`test/language/expressions/arrow-function/arrow/concisebody-lookahead-assignmentexpression-1.js`<br>`test/language/expressions/arrow-function/arrow/concisebody-lookahead-assignmentexpression-2.js`<br>`test/language/expressions/arrow-function/ArrowFunction_restricted-properties.js` | Arrow functions compile via the IR pipeline and are emitted as callable methods (see JavaScriptArrowFunctionGenerator). They do not create their own arguments object: when arguments is referenced, it resolves through the nearest non-arrow function scope, while the current bounded test262 MVP suites also cover the restricted caller/arguments accessors required on arrow function objects. |

### 15.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-concisebodycontainsusestrict))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| directive prologue / strict mode ("use strict") semantics | Supported with Limitations |  | suite `pr`<br>suite `nightly`<br>`test/language/expressions/arrow-function/array-destructuring-param-strict-body.js` | JS2IL parses directive prologues but does not currently aim for full strict-mode semantics/early errors across the language. This clause is tracked as limited until a dedicated strict-mode test matrix exists. The current bounded test262 MVP suites include a representative parse-negative case for non-simple arrow parameters plus a strict directive. |

### 15.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiatearrowfunctionexpression))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| lexical this for arrow functions | Supported | [`ArrowFunction_LexicalThis_ConstructorAssigned.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_LexicalThis_ConstructorAssigned.js)<br>[`ArrowFunction_LexicalThis_CreatedInMethod.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_LexicalThis_CreatedInMethod.js)<br>[`ArrowFunction_LexicalThis_TopLevelCallback.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_LexicalThis_TopLevelCallback.js) | `test/language/expressions/arrow-function/cannot-override-this-with-thisArg.js` | Arrow functions capture lexical this at creation time and invoke with that bound this, including top-level callback cases that receive a thisArg (GitHub issue #219). |

