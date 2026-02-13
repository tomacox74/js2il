# Global: Promise

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/globals.html#promise) |

## Implementation

- `JavaScriptRuntime/Promise.cs, JavaScriptRuntime/Engine/EngineCore.cs`

## Notes

Promise/A+ compliant implementation with microtask scheduling via IMicrotaskScheduler. Supports constructor, Promise.resolve(), Promise.reject(), then(), catch(), and finally(). Includes proper handling of returned Promises in handlers and chaining semantics.

## Tests

- `Js2IL.Tests.Promise.ExecutionTests.Promise_Executor_Resolved` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Executor_Rejected` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_Then` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Reject_Then` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_ThenFinally` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Reject_FinallyCatch` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_FinallyThen` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_FinallyThrows` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Then_ReturnsResolvedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Then_ReturnsRejectedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Catch_ReturnsResolvedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Catch_ReturnsRejectedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Finally_ReturnsResolvedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Finally_ReturnsRejectedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Scheduling_StarvationTest` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
