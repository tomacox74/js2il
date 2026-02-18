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

Buffer foundation plus core phase-2 APIs: Buffer.from(string|array|buffer) now supports utf8/base64/hex string decoding, Buffer.isBuffer(value), Buffer.alloc(size[, fill[, encoding]]), Buffer.byteLength(value[, encoding]), Buffer.concat(list[, totalLength]), instance length, and toString(encoding).

## Tests

- `Js2IL.Tests.Node.Buffer.ExecutionTests.Buffer_From_And_IsBuffer` (`Js2IL.Tests/Node/Buffer/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Buffer.ExecutionTests.Buffer_Alloc_ByteLength_Concat` (`Js2IL.Tests/Node/Buffer/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Buffer.GeneratorTests.Buffer_From_And_IsBuffer` (`Js2IL.Tests/Node/Buffer/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Buffer.GeneratorTests.Buffer_Alloc_ByteLength_Concat` (`Js2IL.Tests/Node/Buffer/GeneratorTests.cs`)
