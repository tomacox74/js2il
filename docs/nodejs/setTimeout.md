# Global: setTimeout

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#settimeoutcallback-delay-args) |

## Implementation

- `src/JavaScriptRuntime/GlobalThis.cs, src/JavaScriptRuntime/Timers.cs`

## Notes

Schedules a callback to be executed after a specified delay in milliseconds. Returns a timer handle that can be used with clearTimeout.

## Tests

- `Js2IL.Tests.Node.Timers.ExecutionTests.SetTimeout_ZeroDelay` (`tests/Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Timers.ExecutionTests.SetTimeout_MultipleZeroDelay_ExecutedInOrder` (`tests/Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Timers.ExecutionTests.SetTimeout_OneSecondDelay` (`tests/Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
