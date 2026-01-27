<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.14: The throw Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.14 | The throw Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-throw-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.14.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-throw-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 14.14.1 ([tc39.es](https://tc39.es/ecma262/#sec-throw-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| throw statement (throw arbitrary JS value) | Supported | [`TryCatchFinally_ThrowValue.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatchFinally_ThrowValue.js)<br>[`ControlFlow_TryCatchFinally_ThrowValue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryCatchFinally_ThrowValue.js) | Supports throwing any JS value. Non-Exception values are wrapped in JavaScriptRuntime.JsThrownValueException so they can be transported as .NET exceptions and unwrapped in catch. |
| throw statement (throw Error/TypeError, catch by name/message) | Supported | [`TryCatch_NewExpression_BuiltInErrors.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_NewExpression_BuiltInErrors.js)<br>[`TryCatch_CallMember_MissingMethod_IsTypeError.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_CallMember_MissingMethod_IsTypeError.js) |  |

