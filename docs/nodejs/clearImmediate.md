# Global: clearImmediate

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#clearimmediateimmediate) |

## Implementation

- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs, JavaScriptRuntime/Engine/NodeSchedulerState.cs, JavaScriptRuntime/Engine/NodeEventLoopPump.cs`

## Notes

Cancels an immediate that was previously created with setImmediate. Returns undefined (null in .NET).

## Tests

- `Js2IL.Tests.Node.Timers.ExecutionTests.ClearImmediate_CancelsCallback` (`Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
