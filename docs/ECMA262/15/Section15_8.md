<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.8: Async Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-08T06:45:15Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.8 | Async Function Definitions | Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-definitions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.8.1 | Static Semantics: Early Errors | Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-definitions-static-semantics-early-errors) |
| 15.8.2 | Runtime Semantics: InstantiateAsyncFunctionObject | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncfunctionobject) |
| 15.8.3 | Runtime Semantics: InstantiateAsyncFunctionExpression | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncfunctionexpression) |
| 15.8.4 | Runtime Semantics: EvaluateAsyncFunctionBody | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluateasyncfunctionbody) |
| 15.8.5 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-definitions-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 15.8.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncfunctionobject))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| async function (no await) | Supported | [`Async_HelloWorld.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_HelloWorld.js)<br>[`Async_ReturnValue.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ReturnValue.js)<br>[`Async_GeneratedFunctionObject_Semantics.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_GeneratedFunctionObject_Semantics.js) |  | Async declarations and expressions materialize as generated non-constructable JsFunctionObject instances and use the common callable ABI. Calls return Promises, including no-await bodies; synchronous parameter/body setup failures are converted into rejected Promises by the generated adapter. |

### 15.8.4 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluateasyncfunctionbody))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| await expression | Supported | [`Async_SimpleAwait.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_SimpleAwait.js)<br>[`Async_TryCatch_AwaitReject.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_TryCatch_AwaitReject.js)<br>[`Async_TryFinally_AwaitInFinally_Normal.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_TryFinally_AwaitInFinally_Normal.js)<br>[`Async_TryCatchFinally_AwaitInFinally_OnReject.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_TryCatchFinally_AwaitInFinally_OnReject.js)<br>[`Async_TryFinally_PreservesExceptionThroughAwait.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_TryFinally_PreservesExceptionThroughAwait.js)<br>[`Async_TryFinally_FinallyThrowOverridesOriginal.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_TryFinally_FinallyThrowOverridesOriginal.js)<br>[`Async_TryFinally_ReturnPreservedThroughAwait.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_TryFinally_ReturnPreservedThroughAwait.js)<br>[`Async_GeneratedFunctionObject_Semantics.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_GeneratedFunctionObject_Semantics.js) |  | Full state machine implementation with suspension/resumption. Generated async function objects share immutable captures while each invocation creates independent Promise/state-machine state, so overlapping calls remain isolated. Each await point stores _asyncState, schedules a continuation, and resumes through the state switch. Await rejection and finally completion semantics are preserved. |

