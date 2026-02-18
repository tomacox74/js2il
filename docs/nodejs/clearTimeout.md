# Global: clearTimeout

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/timers.html#cleartimeouttimeout) |

## Implementation

- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs`

## Notes

Cancels a timer that was previously created with setTimeout. Returns undefined (null in .NET).

## Tests

- `Js2IL.Tests.Node.Timers.ExecutionTests.ClearTimeout_ZeroDelay` (`Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Timers.ExecutionTests.ClearTimeout_MultipleZeroDelay_ClearSecondTimer` (`Js2IL.Tests/Node/Timers/ExecutionTests.cs`)
