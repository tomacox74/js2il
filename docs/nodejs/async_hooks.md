# Module: async_hooks

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 24.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/async_hooks.html) |

## Implementation

- `src/JavaScriptRuntime/Node/AsyncHooks.cs`
- `src/JavaScriptRuntime/Node/Contracts/IAsyncHooksModule.Generated.cs`

## Notes

Supports the AsyncResource behavior required by Undici, AsyncLocalStorage propagation through Promise, nextTick, timer, immediate, and fs callback boundaries, explicit async hook lifecycle events, and promiseResolve events. State is isolated per engine and context capture allocates only while a store is active. The deprecated asyncWrapProviders export remains explicit but unavailable.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| AsyncResource | property | supported | [docs](https://nodejs.org/api/async_context.html#class-asyncresource) |
| AsyncLocalStorage | property | supported | [docs](https://nodejs.org/api/async_context.html#class-asynclocalstorage) |
| createHook / AsyncHook | function | partial | [docs](https://nodejs.org/api/async_hooks.html#async_hookscreatehookcallbacks) |
| executionAsyncId / triggerAsyncId / executionAsyncResource | function | supported | [docs](https://nodejs.org/api/async_hooks.html#async_hooksexecutionasyncid) |
| asyncWrapProviders | property | not-supported | [docs](https://nodejs.org/api/async_hooks.html#async_hooksasyncwrapproviders) |

## API Details

### AsyncResource

Supports direct construction, JavaScript subclassing, runInAsyncScope, bind, emitDestroy, asyncId, triggerAsyncId, receiver and argument forwarding, return values, exceptions, and nested restoration.

**Tests:**
- `Jroc.Tests.Node.AsyncHooks.ExecutionTests.Require_AsyncHooks_AsyncResource_Subclass` (`tests/Jroc.Tests/Node/AsyncHooks/ExecutionTests.cs`)

### AsyncLocalStorage

Supports run, enterWith, getStore, exit, disable, bind, snapshot, name, defaultValue, nested stores, sibling isolation, and propagation across supported scheduling and I/O boundaries.

**Tests:**
- `Jroc.Tests.Node.AsyncHooks.ExecutionTests.Require_AsyncHooks_AsyncLocalStorage_Propagation` (`tests/Jroc.Tests/Node/AsyncHooks/ExecutionTests.cs`)

### createHook / AsyncHook

Supports enable, disable, and init/before/after/destroy lifecycle events for explicit AsyncResource instances plus init/promiseResolve for instrumented Promises. Other built-in resource types are not yet instrumented.

**Tests:**
- `Jroc.Tests.Node.AsyncHooks.ExecutionTests.Require_AsyncHooks_CreateHook` (`tests/Jroc.Tests/Node/AsyncHooks/ExecutionTests.cs`)

### asyncWrapProviders

The deprecated provider table remains in the generated Node 24 contract and throws NotImplementedException explicitly.
