<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.16: The debugger Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.16 | The debugger Statement | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-debugger-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.16.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-debugger-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 14.16.1 ([tc39.es](https://tc39.es/ecma262/#sec-debugger-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| throw statement | Supported | [`TryCatch_NoBinding.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding.js)<br>[`TryCatchFinally_ThrowValue.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatchFinally_ThrowValue.js) | Supports throwing any JS value. Non-Exception values are wrapped in JavaScriptRuntime.JsThrownValueException; catch unwrapping binds the original value. JavaScriptRuntime.Error is thrown directly. |
| try/catch (no binding) | Supported | [`TryCatch_NoBinding.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding.js)<br>[`TryCatch_NoBinding_NoThrow.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding_NoThrow.js) | Catch blocks handle values thrown within the try region (including non-Exception JS values via JsThrownValueException) and bind the caught value only when a binding is present. |
| try/catch (with binding; block-scoped catch parameter) | Supported | [`TryCatch_ScopedParam.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_ScopedParam.js) | Catch parameter binding is block-scoped to the catch clause and does not leak outside the catch block. |
| try/catch/finally | Supported | [`TryCatchFinally_ThrowValue.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatchFinally_ThrowValue.js) | Supports catch + finally with correct finally execution and catch binding when throwing arbitrary JS values. |
| try/finally (no catch) | Partially Supported | [`TryFinally_NoCatch.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryFinally_NoCatch.js)<br>[`TryFinally_NoCatch_Throw.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryFinally_NoCatch_Throw.js)<br>[`TryFinally_Return.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryFinally_Return.js) | Finally emission is in place and executes on normal and return exits. Execution test for unhandled throw is skipped pending top-level unhandled Error semantics; generator snapshot verifies structure. |

