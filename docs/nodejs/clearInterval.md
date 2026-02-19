# Global: clearInterval

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#clearintervaltimeout) |

## Implementation

- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs, JavaScriptRuntime/Engine/NodeSchedulerState.cs, JavaScriptRuntime/Engine/NodeEventLoopPump.cs`

## Notes

Cancels a repeating timer that was previously created with setInterval. Returns undefined (null in .NET).

## Tests

- `Js2IL.Tests.Node.ExecutionTests.SetInterval_ExecutesThreeTimes_ThenClears` (`Js2IL.Tests/Node/ExecutionTests.cs`)
