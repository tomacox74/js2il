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

Schedules a callback to be executed after a specified delay in milliseconds. Returns a refreshable Timeout handle that can be passed to clearTimeout or restarted with timeout.refresh().

## Tests

- `Jroc.Tests.Node.Timers.ExecutionTests.SetTimeout_ZeroDelay` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)
- `Jroc.Tests.Node.Timers.ExecutionTests.SetTimeout_MultipleZeroDelay_ExecutedInOrder` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)
- `Jroc.Tests.Node.Timers.ExecutionTests.SetTimeout_OneSecondDelay` (`tests/Jroc.Tests/Node/Timers/ExecutionTests.cs`)
