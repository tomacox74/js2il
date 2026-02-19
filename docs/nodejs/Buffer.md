# Global: Buffer

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/buffer.html#class-buffer) |

## Implementation

- `JavaScriptRuntime/Node/Buffer.cs`

## Notes

Core Buffer APIs implemented: Buffer.from/isBuffer/alloc/allocUnsafe/byteLength/concat/compare, instance methods (slice/copy/write/toString/length), array-like indexing (buffer[i]), and binary read/write methods (readInt8/16/32BE/LE, writeInt8/16/32BE/LE, readUInt8/16/32BE/LE, writeUInt8/16/32BE/LE) with utf8/base64/hex encoding support. Covers common use cases for binary data manipulation and I/O operations.

## Tests

- `Js2IL.Tests.Node.Buffer.ExecutionTests.Buffer_From_And_IsBuffer` (`Js2IL.Tests/Node/Buffer/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Buffer.ExecutionTests.Buffer_Alloc_ByteLength_Concat` (`Js2IL.Tests/Node/Buffer/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Buffer.ExecutionTests.Buffer_Slice_Copy_IndexAccess` (`Js2IL.Tests/Node/Buffer/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Buffer.ExecutionTests.Buffer_AllocUnsafe_Compare` (`Js2IL.Tests/Node/Buffer/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Buffer.ExecutionTests.Buffer_ReadWrite_Methods` (`Js2IL.Tests/Node/Buffer/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Buffer.GeneratorTests.Buffer_From_And_IsBuffer` (`Js2IL.Tests/Node/Buffer/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Buffer.GeneratorTests.Buffer_Alloc_ByteLength_Concat` (`Js2IL.Tests/Node/Buffer/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Buffer.GeneratorTests.Buffer_Slice_Copy_IndexAccess` (`Js2IL.Tests/Node/Buffer/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Buffer.GeneratorTests.Buffer_AllocUnsafe_Compare` (`Js2IL.Tests/Node/Buffer/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Buffer.GeneratorTests.Buffer_ReadWrite_Methods` (`Js2IL.Tests/Node/Buffer/GeneratorTests.cs`)
