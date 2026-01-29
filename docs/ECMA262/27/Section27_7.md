<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 27.7: AsyncFunction Objects

[Back to Section27](Section27.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 27.7 | AsyncFunction Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-function-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 27.7.1 | The AsyncFunction Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-constructor) |
| 27.7.1.1 | AsyncFunction ( ... parameterArgs , bodyArg ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-constructor-arguments) |
| 27.7.2 | Properties of the AsyncFunction Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-constructor-properties) |
| 27.7.2.1 | AsyncFunction.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-constructor-prototype) |
| 27.7.3 | Properties of the AsyncFunction Prototype Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-prototype-properties) |
| 27.7.3.1 | AsyncFunction.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-prototype-properties-constructor) |
| 27.7.3.2 | AsyncFunction.prototype [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-prototype-%symbol.tostringtag%) |
| 27.7.4 | AsyncFunction Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-function-instances) |
| 27.7.4.1 | length | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-instances-length) |
| 27.7.4.2 | name | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-instances-name) |
| 27.7.5 | Async Functions Abstract Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-functions-abstract-operations) |
| 27.7.5.1 | AsyncFunctionStart ( promiseCapability , asyncFunctionBody ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-functions-abstract-operations-async-function-start) |
| 27.7.5.2 | AsyncBlockStart ( promiseCapability , asyncBody , asyncContext ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncblockstart) |
| 27.7.5.3 | Await ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#await) |

## Support

Feature-level support tracking with test script references.

### 27.7.5.3 ([tc39.es](https://tc39.es/ecma262/#await))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| async/await lowers to Promise-based state machine and supports await of pending Promises | Supported | [`Async_SimpleAwait.js`](../../../Js2IL.Tests/Async/JavaScript/Async_SimpleAwait.js)<br>[`Async_ReturnValue.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ReturnValue.js)<br>[`Async_PendingPromiseAwait.js`](../../../Js2IL.Tests/Async/JavaScript/Async_PendingPromiseAwait.js)<br>[`Async_RealSuspension_SetTimeout.js`](../../../Js2IL.Tests/Async/JavaScript/Async_RealSuspension_SetTimeout.js)<br>[`Async_TryCatch_AwaitReject.js`](../../../Js2IL.Tests/Async/JavaScript/Async_TryCatch_AwaitReject.js)<br>[`Async_TryCatchFinally_AwaitInFinally_OnReject.js`](../../../Js2IL.Tests/Async/JavaScript/Async_TryCatchFinally_AwaitInFinally_OnReject.js)<br>[`Async_TryFinally_ReturnPreservedThroughAwait.js`](../../../Js2IL.Tests/Async/JavaScript/Async_TryFinally_ReturnPreservedThroughAwait.js)<br>[`Async_TryFinally_PreservesExceptionThroughAwait.js`](../../../Js2IL.Tests/Async/JavaScript/Async_TryFinally_PreservesExceptionThroughAwait.js) | Async functions are supported via syntax (`async function`, `await`) and runtime Promise integration. The spec-level `AsyncFunction` constructor/prototype intrinsics are not currently exposed. |

