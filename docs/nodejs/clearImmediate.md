# Global: clearImmediate

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#clearimmediateimmediate) |

## Implementation

- `src/JavaScriptRuntime/GlobalThis.cs, src/JavaScriptRuntime/Timers.cs, src/JavaScriptRuntime/Engine/NodeSchedulerState.cs, src/JavaScriptRuntime/Engine/NodeEventLoopPump.cs`

## Notes

Cancels an immediate that was previously created with setImmediate. Returns undefined (null in .NET).

## Tests

- `Jroc.Tests.Node.ExecutionTests.ClearImmediate_CancelsCallback` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
