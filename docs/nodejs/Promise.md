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

- `Jroc.Tests.Promise.ExecutionTests.Promise_Executor_Resolved` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Executor_Rejected` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Resolve_Then` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Reject_Then` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Resolve_ThenFinally` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Reject_FinallyCatch` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Resolve_FinallyThen` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Resolve_FinallyThrows` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Then_ReturnsResolvedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Then_ReturnsRejectedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Catch_ReturnsResolvedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Catch_ReturnsRejectedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Finally_ReturnsResolvedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Finally_ReturnsRejectedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Scheduling_StarvationTest` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
