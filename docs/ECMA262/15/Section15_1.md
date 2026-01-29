<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.1: Parameter Lists

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.1 | Parameter Lists | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-parameter-lists) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.1.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-parameter-lists-static-semantics-early-errors) |
| 15.1.2 | Static Semantics: ContainsExpression | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-containsexpression) |
| 15.1.3 | Static Semantics: IsSimpleParameterList | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-issimpleparameterlist) |
| 15.1.4 | Static Semantics: HasInitializer | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-hasinitializer) |
| 15.1.5 | Static Semantics: ExpectedArgumentCount | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-expectedargumentcount) |

## Support

Feature-level support tracking with test script references.

### 15.1 ([tc39.es](https://tc39.es/ecma262/#sec-parameter-lists))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| object destructuring in parameters | Supported | [`Function_ParameterDestructuring_Object.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ParameterDestructuring_Object.js)<br>[`ArrowFunction_ParameterDestructuring_Object.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_ParameterDestructuring_Object.js) |  |
| rest parameters (...args) | Not Yet Supported |  | Explicitly rejected in validator (ValidateFunctionParameters). |
| simple parameter lists (<= 6 params) | Supported with Limitations | [`Function_GlobalFunctionWithParameter.js`](../../../Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionWithParameter.js)<br>[`Function_GlobalFunctionWithMultipleParameters.js`](../../../Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionWithMultipleParameters.js)<br>[`ArrowFunction_GlobalFunctionWithMultipleParameters.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionWithMultipleParameters.js) | JS2IL supports simple parameter lists but enforces a current limit of 6 parameters (validator) and does not support rest parameters (...args). |

### 15.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-containsexpression))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| default parameter expressions (can reference earlier params) | Supported | [`Function_DefaultParameterExpression.js`](../../../Js2IL.Tests/Function/JavaScript/Function_DefaultParameterExpression.js)<br>[`ArrowFunction_DefaultParameterExpression.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_DefaultParameterExpression.js) |  |

### 15.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-hasinitializer))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| default parameter values | Supported | [`Function_DefaultParameterValue.js`](../../../Js2IL.Tests/Function/JavaScript/Function_DefaultParameterValue.js)<br>[`ArrowFunction_DefaultParameterValue.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_DefaultParameterValue.js) |  |

