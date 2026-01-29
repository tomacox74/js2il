<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.2: Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

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

Feature-level support tracking with test script references.

### 15.2 ([tc39.es](https://tc39.es/ecma262/#sec-function-definitions))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| function declarations (global + nested) and calls | Supported | [`Function_HelloWorld.js`](../../../Js2IL.Tests/Function/JavaScript/Function_HelloWorld.js)<br>[`Function_GlobalFunctionWithParameter.js`](../../../Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionWithParameter.js)<br>[`Function_GlobalFunctionCallsGlobalFunction.js`](../../../Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionCallsGlobalFunction.js)<br>[`Function_GlobalFunctionDeclaresAndCallsNestedFunction.js`](../../../Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionDeclaresAndCallsNestedFunction.js)<br>[`Function_NestedFunctionAccessesMultipleScopes.js`](../../../Js2IL.Tests/Function/JavaScript/Function_NestedFunctionAccessesMultipleScopes.js) | Ordinary functions compile end-to-end, including nested functions and closure capture (scope-as-class). Parameter-list limitations apply (no rest params; max 6 params). |
| function expressions (assigned/returned) and closures | Supported | [`Function_NestedFunctionExpression_ReturnedAndCalledViaVariable.js`](../../../Js2IL.Tests/Function/JavaScript/Function_NestedFunctionExpression_ReturnedAndCalledViaVariable.js)<br>[`Function_CallViaVariable_Reassignment.js`](../../../Js2IL.Tests/Function/JavaScript/Function_CallViaVariable_Reassignment.js)<br>[`Function_Closure_MultiLevel_ReadWriteAcrossScopes.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Closure_MultiLevel_ReadWriteAcrossScopes.js)<br>[`Function_FunctionExpression_AsExpression_ArrayMapCapturesOuter.js`](../../../Js2IL.Tests/Function/JavaScript/Function_FunctionExpression_AsExpression_ArrayMapCapturesOuter.js) |  |
| IIFE (immediately-invoked function expressions) | Supported | [`Function_IIFE_Classic.js`](../../../Js2IL.Tests/Function/JavaScript/Function_IIFE_Classic.js)<br>[`Function_IIFE_Recursive.js`](../../../Js2IL.Tests/Function/JavaScript/Function_IIFE_Recursive.js) |  |

