# Global: setImmediate

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#setimmediatecallback-args) |

## Implementation

- `src/JavaScriptRuntime/GlobalThis.cs, src/JavaScriptRuntime/Timers.cs, src/JavaScriptRuntime/Engine/NodeSchedulerState.cs, src/JavaScriptRuntime/Engine/NodeEventLoopPump.cs`

## Notes

Schedules a callback to run on the next event loop iteration. Callbacks execute in FIFO order. Nested setImmediate calls run on the next iteration. Returns a handle that can be used with clearImmediate.

## Tests

- `Jroc.Tests.Node.Timers.ExecutionTests.SetImmediate_ExecutesCallback` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)
- `Jroc.Tests.Node.Timers.ExecutionTests.SetImmediate_WithArgs_PassesCorrectly` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)
- `Jroc.Tests.Node.Timers.ExecutionTests.SetImmediate_Multiple_ExecuteInOrder` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)
- `Jroc.Tests.Node.Timers.ExecutionTests.SetImmediate_ExecutesBeforeSetTimeout` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)
- `Jroc.Tests.Node.Timers.ExecutionTests.SetImmediate_Nested_ExecutesInNextIteration` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)
