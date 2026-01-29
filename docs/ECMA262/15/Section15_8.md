<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.8: Async Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

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

Feature-level support tracking with test script references.

### 15.8.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncfunctionobject))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| async function (no await) | Supported | [`Async_HelloWorld.js`](../../../Js2IL.Tests/Async/JavaScript/Async_HelloWorld.js)<br>[`Async_ReturnValue.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ReturnValue.js) | Async functions without await expressions compile successfully. Return values are wrapped in Promise.resolve(). |

### 15.8.4 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluateasyncfunctionbody))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| await expression | Supported | [`Async_SimpleAwait.js`](../../../Js2IL.Tests/Async/JavaScript/Async_SimpleAwait.js)<br>[`Async_TryCatch_AwaitReject.js`](../../../Js2IL.Tests/Async/JavaScript/Async_TryCatch_AwaitReject.js)<br>[`Async_TryFinally_AwaitInFinally_Normal.js`](../../../Js2IL.Tests/Async/JavaScript/Async_TryFinally_AwaitInFinally_Normal.js)<br>[`Async_TryCatchFinally_AwaitInFinally_OnReject.js`](../../../Js2IL.Tests/Async/JavaScript/Async_TryCatchFinally_AwaitInFinally_OnReject.js)<br>[`Async_TryFinally_PreservesExceptionThroughAwait.js`](../../../Js2IL.Tests/Async/JavaScript/Async_TryFinally_PreservesExceptionThroughAwait.js)<br>[`Async_TryFinally_FinallyThrowOverridesOriginal.js`](../../../Js2IL.Tests/Async/JavaScript/Async_TryFinally_FinallyThrowOverridesOriginal.js)<br>[`Async_TryFinally_ReturnPreservedThroughAwait.js`](../../../Js2IL.Tests/Async/JavaScript/Async_TryFinally_ReturnPreservedThroughAwait.js) | Full state machine implementation with suspension/resumption. Each await point stores _asyncState, schedules promise.then() continuation via SetupAwaitContinuation, and returns. On resume, the state switch dispatches to the appropriate label and loads the awaited result from a scope field. Scope persistence handled via PrependScopeToArray. Await rejection inside try/catch resumes into the catch block via pending-exception storage. Await in finally is supported, including correct completion semantics across suspension (preserve throw/return through awaited finally, and allow finally throws to override prior completion). |

