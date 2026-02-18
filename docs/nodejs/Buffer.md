# Global: Buffer

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/buffer.html#class-buffer) |

## Implementation

- `JavaScriptRuntime/Node/Buffer.cs`

## Notes

Initial Buffer foundation with Buffer.from(string|array|buffer), Buffer.isBuffer(value), instance length, and toString('utf8'). Additional Buffer APIs are not implemented yet.

## Tests

- `Js2IL.Tests.Node.Buffer.ExecutionTests.Buffer_From_And_IsBuffer` (`Js2IL.Tests/Node/Buffer/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Buffer.GeneratorTests.Buffer_From_And_IsBuffer` (`Js2IL.Tests/Node/Buffer/GeneratorTests.cs`)
