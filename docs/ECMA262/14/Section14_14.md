<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.14: The throw Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T00:29:07Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.14 | The throw Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-throw-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.14.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-throw-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.14.1 ([tc39.es](https://tc39.es/ecma262/#sec-throw-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| throw statement (throw arbitrary JS value) | Supported | [`TryCatchFinally_ThrowValue.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryCatchFinally_ThrowValue.js)<br>[`ControlFlow_TryCatchFinally_ThrowValue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryCatchFinally_ThrowValue.js)<br>[`S12.13_A3_T1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/throw/JavaScript/S12.13_A3_T1.js) |  | Supports throwing any JS value, including expression results. Non-Exception values are wrapped in JavaScriptRuntime.JsThrownValueException so they can be transported as .NET exceptions and unwrapped in catch. |
| throw statement (throw Error/TypeError, catch by name/message) | Supported | [`TryCatch_NewExpression_BuiltInErrors.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryCatch_NewExpression_BuiltInErrors.js)<br>[`TryCatch_CallMember_MissingMethod_IsTypeError.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryCatch_CallMember_MissingMethod_IsTypeError.js) |  |  |

