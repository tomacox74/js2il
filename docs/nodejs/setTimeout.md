# Global: setTimeout

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#settimeoutcallback-delay-args) |

## Implementation

- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs`

## Notes

Schedules a callback to be executed after a specified delay in milliseconds. Returns a timer handle that can be used with clearTimeout.

## Tests

- `Js2IL.Tests.Node.Timers.ExecutionTests.SetTimeout_ZeroDelay` (`Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Timers.ExecutionTests.SetTimeout_MultipleZeroDelay_ExecutedInOrder` (`Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Timers.ExecutionTests.SetTimeout_OneSecondDelay` (`Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
