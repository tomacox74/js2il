# Global: setInterval

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#setintervalcallback-delay-args) |

## Implementation

- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs, JavaScriptRuntime/Engine/NodeSchedulerState.cs, JavaScriptRuntime/Engine/NodeEventLoopPump.cs`

## Notes

Schedules a callback to run repeatedly with the specified delay in milliseconds. Returns a handle that can be used with clearInterval. Supports additional arguments passed to the callback.

## Tests

- `Js2IL.Tests.Node.ExecutionTests.SetInterval_ExecutesThreeTimes_ThenClears` (`Js2IL.Tests/Node/ExecutionTests.cs`)
