# Global: setImmediate

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#setimmediatecallback-args) |

## Implementation

- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs, JavaScriptRuntime/Engine/NodeSchedulerState.cs, JavaScriptRuntime/Engine/NodeEventLoopPump.cs`

## Notes

Schedules a callback to run on the next event loop iteration. Callbacks execute in FIFO order. Nested setImmediate calls run on the next iteration. Returns a handle that can be used with clearImmediate.

## Tests

- `Js2IL.Tests.Node.Timers.ExecutionTests.SetImmediate_ExecutesCallback` (`Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Timers.ExecutionTests.SetImmediate_WithArgs_PassesCorrectly` (`Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Timers.ExecutionTests.SetImmediate_Multiple_ExecuteInOrder` (`Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Timers.ExecutionTests.SetImmediate_ExecutesBeforeSetTimeout` (`Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Timers.ExecutionTests.SetImmediate_Nested_ExecutesInNextIteration` (`Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
