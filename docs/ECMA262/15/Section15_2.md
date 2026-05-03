<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.2: Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-03T15:15:03Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.2 | Function Definitions | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-definitions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.2.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-definitions-static-semantics-early-errors) |
| 15.2.2 | Static Semantics: FunctionBodyContainsUseStrict | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-functionbodycontainsusestrict) |
| 15.2.3 | Runtime Semantics: EvaluateFunctionBody | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluatefunctionbody) |
| 15.2.4 | Runtime Semantics: InstantiateOrdinaryFunctionObject | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateordinaryfunctionobject) |
| 15.2.5 | Runtime Semantics: InstantiateOrdinaryFunctionExpression | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateordinaryfunctionexpression) |
| 15.2.6 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-definitions-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 15.2 ([tc39.es](https://tc39.es/ecma262/#sec-function-definitions))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| function declarations (global + nested) and calls | Supported | [`Function_HelloWorld.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_HelloWorld.js)<br>[`Function_GlobalFunctionWithParameter.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionWithParameter.js)<br>[`Function_GlobalFunctionCallsGlobalFunction.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionCallsGlobalFunction.js)<br>[`Function_GlobalFunctionDeclaresAndCallsNestedFunction.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionDeclaresAndCallsNestedFunction.js)<br>[`Function_NestedFunctionAccessesMultipleScopes.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_NestedFunctionAccessesMultipleScopes.js)<br>[`Function_RestParameters_WithNamedParams.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_RestParameters_WithNamedParams.js)<br>[`Function_RestParameters_MultipleNamed.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_RestParameters_MultipleNamed.js)<br>[`Function_MaxParameters_32_CallViaVariable.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_MaxParameters_32_CallViaVariable.js) |  | Ordinary functions compile end-to-end, including nested functions, closure capture, and rest parameters. Current validator/runtime coverage supports ordinary functions with up to 32 formal parameters; functions with more than 32 parameters are rejected during validation. |
| function expressions (assigned/returned) and closures | Supported | [`Function_NestedFunctionExpression_ReturnedAndCalledViaVariable.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_NestedFunctionExpression_ReturnedAndCalledViaVariable.js)<br>[`Function_CallViaVariable_Reassignment.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_CallViaVariable_Reassignment.js)<br>[`Function_Closure_MultiLevel_ReadWriteAcrossScopes.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Closure_MultiLevel_ReadWriteAcrossScopes.js)<br>[`Function_FunctionExpression_AsExpression_ArrayMapCapturesOuter.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_FunctionExpression_AsExpression_ArrayMapCapturesOuter.js) |  |  |
| IIFE (immediately-invoked function expressions) | Supported | [`Function_IIFE_Classic.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_IIFE_Classic.js)<br>[`Function_IIFE_Recursive.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_IIFE_Recursive.js) |  |  |

### 15.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-functionbodycontainsusestrict))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| directive prologue / strict mode ("use strict") semantics | Supported with Limitations |  |  | JS2IL parses directive prologues but does not currently aim for full strict-mode semantics/early errors across the language. This clause is tracked as limited until a dedicated strict-mode test matrix exists. |

