# Global: clearTimeout

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#cleartimeouttimeout) |

## Implementation

- `src/JavaScriptRuntime/GlobalThis.cs, src/JavaScriptRuntime/Timers.cs`

## Notes

Cancels a timer that was previously created with setTimeout, including a refreshable Timeout handle. Returns undefined (null in .NET).

## Tests

- `Jroc.Tests.Node.ExecutionTests.ClearTimeout_ZeroDelay` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
- `Jroc.Tests.Node.ExecutionTests.ClearTimeout_MultipleZeroDelay_ClearSecondTimer` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
