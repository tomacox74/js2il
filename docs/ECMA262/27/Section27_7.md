<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 27.7: AsyncFunction Objects

[Back to Section27](Section27.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T19:49:59Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 27.7 | AsyncFunction Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-function-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 27.7.1 | The AsyncFunction Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-constructor) |
| 27.7.1.1 | AsyncFunction ( ... parameterArgs , bodyArg ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-constructor-arguments) |
| 27.7.2 | Properties of the AsyncFunction Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-function-constructor-properties) |
| 27.7.2.1 | AsyncFunction.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-function-constructor-prototype) |
| 27.7.3 | Properties of the AsyncFunction Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-function-prototype-properties) |
| 27.7.3.1 | AsyncFunction.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-function-prototype-properties-constructor) |
| 27.7.3.2 | AsyncFunction.prototype [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-function-prototype-%symbol.tostringtag%) |
| 27.7.4 | AsyncFunction Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-function-instances) |
| 27.7.4.1 | length | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-instances-length) |
| 27.7.4.2 | name | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-function-instances-name) |
| 27.7.5 | Async Functions Abstract Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-functions-abstract-operations) |
| 27.7.5.1 | AsyncFunctionStart ( promiseCapability , asyncFunctionBody ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-functions-abstract-operations-async-function-start) |
| 27.7.5.2 | AsyncBlockStart ( promiseCapability , asyncBody , asyncContext ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncblockstart) |
| 27.7.5.3 | Await ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#await) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 27.7.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-async-function-constructor-prototype))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| async function instances expose a dedicated AsyncFunction constructor/prototype surface | Supported with Limitations | [`AsyncFunction_intrinsic.js`](../../../tests/Js2IL.Test262.Tests/built-ins/AsyncFunction/JavaScript/AsyncFunction_intrinsic.js) |  | Async function values now inherit from a dedicated `AsyncFunction.prototype`, and expose `instance.constructor`, `AsyncFunction.prototype.constructor`, and `%Symbol.toStringTag%`. Calling the `AsyncFunction` constructor itself is still not supported. |

### 27.7.5.3 ([tc39.es](https://tc39.es/ecma262/#await))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| async/await lowers to Promise-based state machine and supports await of pending Promises | Supported | [`Async_SimpleAwait.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_SimpleAwait.js)<br>[`Async_ReturnValue.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_ReturnValue.js)<br>[`Async_PendingPromiseAwait.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_PendingPromiseAwait.js)<br>[`Async_RealSuspension_SetTimeout.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_RealSuspension_SetTimeout.js)<br>[`Async_TryCatch_AwaitReject.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_TryCatch_AwaitReject.js)<br>[`Async_TryCatchFinally_AwaitInFinally_OnReject.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_TryCatchFinally_AwaitInFinally_OnReject.js)<br>[`Async_TryFinally_ReturnPreservedThroughAwait.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_TryFinally_ReturnPreservedThroughAwait.js)<br>[`Async_TryFinally_PreservesExceptionThroughAwait.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_TryFinally_PreservesExceptionThroughAwait.js)<br>[`declaration-returns-promise.js`](../../../tests/Js2IL.Test262.Tests/language/statements/async-function/JavaScript/declaration-returns-promise.js)<br>[`dflt-params-ref-prior.js`](../../../tests/Js2IL.Test262.Tests/language/statements/async-function/JavaScript/dflt-params-ref-prior.js)<br>[`returns-async-arrow-returns-arguments-from-parent-function.js`](../../../tests/Js2IL.Test262.Tests/language/statements/async-function/JavaScript/returns-async-arrow-returns-arguments-from-parent-function.js)<br>[`returns-async-arrow.js`](../../../tests/Js2IL.Test262.Tests/language/statements/async-function/JavaScript/returns-async-arrow.js)<br>[`returns-async-function.js`](../../../tests/Js2IL.Test262.Tests/language/statements/async-function/JavaScript/returns-async-function.js)<br>[`try-reject-finally-return.js`](../../../tests/Js2IL.Test262.Tests/language/statements/async-function/JavaScript/try-reject-finally-return.js)<br>[`try-reject-finally-throw.js`](../../../tests/Js2IL.Test262.Tests/language/statements/async-function/JavaScript/try-reject-finally-throw.js)<br>[`try-throw-finally-reject.js`](../../../tests/Js2IL.Test262.Tests/language/statements/async-function/JavaScript/try-throw-finally-reject.js)<br>[`arrow-returns-promise.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/async-arrow-function/JavaScript/arrow-returns-promise.js)<br>[`dflt-params-arg-val-not-undefined.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/async-arrow-function/JavaScript/dflt-params-arg-val-not-undefined.js)<br>[`dflt-params-arg-val-undefined.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/async-arrow-function/JavaScript/dflt-params-arg-val-undefined.js)<br>[`dflt-params-ref-prior.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/async-arrow-function/JavaScript/dflt-params-ref-prior.js)<br>[`try-throw-finally-return.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/async-arrow-function/JavaScript/try-throw-finally-return.js) |  | Async functions are supported via syntax (`async function`, `await`) and runtime Promise integration. JS2IL now exposes the observable `AsyncFunction` constructor/prototype surface for compiled async functions, and the checked-in test262 ports cover returned async callables, async default-parameter binding, no-await Promise completion/rejection, and await/finally override ordering for both async functions and async arrows. |

