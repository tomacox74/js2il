# Global: Promise

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/globals.html#promise) |

## Implementation

- `src/JavaScriptRuntime/Promise.cs, src/JavaScriptRuntime/Engine/EngineCore.cs`

## Notes

Promise/A+ compliant implementation with microtask scheduling via IMicrotaskScheduler. Supports constructor, Promise.resolve(), Promise.reject(), then(), catch(), and finally(). Includes proper handling of returned Promises in handlers and chaining semantics.

## Tests

- `Js2IL.Tests.Promise.ExecutionTests.Promise_Executor_Resolved` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Executor_Rejected` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_Then` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Reject_Then` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_ThenFinally` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Reject_FinallyCatch` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_FinallyThen` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_FinallyThrows` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Then_ReturnsResolvedPromise` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Then_ReturnsRejectedPromise` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Catch_ReturnsResolvedPromise` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Catch_ReturnsRejectedPromise` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Finally_ReturnsResolvedPromise` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Finally_ReturnsRejectedPromise` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Scheduling_StarvationTest` (`tests/Js2IL.Tests/Promise/ExecutionTests.cs`)
