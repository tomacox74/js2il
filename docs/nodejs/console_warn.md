# Global: console.warn

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/console.html#consolewarndata-args) |

## Implementation

- `JavaScriptRuntime/Console.cs`

## Notes

Writes to stderr.

## Tests

- `JavaScriptRuntime.Tests.ConsoleTests.Warn_PrintsAllArgumentsWithSpaces` (`Js2IL.Tests/ConsoleTests.cs`)
