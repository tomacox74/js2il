# Module: buffer

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/buffer.html) |

## Implementation

- `src/JavaScriptRuntime/Node/BufferModule.cs`
- `src/JavaScriptRuntime/Node/Buffer.cs`

## Notes

Supports require('buffer') and require('node:buffer'). It exports the global Buffer constructor, isUtf8 for Buffer/ArrayBuffer/typed-array inputs, and resolveObjectURL. Blob object URL creation is not yet supported, so resolveObjectURL returns undefined for all inputs.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| Buffer | property | supported | [docs](https://nodejs.org/api/buffer.html#class-buffer) |
| isUtf8(input) | function | supported | [docs](https://nodejs.org/api/buffer.html#bufferisutf8input) |
| resolveObjectURL(id) | function | partial | [docs](https://nodejs.org/api/buffer.html#bufferresolveobjecturlid) |

## API Details

### Buffer

Exports the same constructor as globalThis.Buffer, including base64/utf8 decoding, allocation, concatenation, binary integer I/O, views, and array-like indexing.

**Tests:**
- `Jroc.Tests.Node.Buffer.ExecutionTests.Require_Buffer_Undici_Core` (`tests/Jroc.Tests/Node/Buffer/ExecutionTests.cs`)
- `Jroc.Tests.Node.Buffer.GeneratorTests.Require_Buffer_Undici_Core` (`tests/Jroc.Tests/Node/Buffer/GeneratorTests.cs`)

### isUtf8(input)

Returns whether a Buffer, ArrayBuffer, or typed-array input contains well-formed UTF-8.

**Tests:**
- `Jroc.Tests.Node.Buffer.ExecutionTests.Require_Buffer_Undici_Core` (`tests/Jroc.Tests/Node/Buffer/ExecutionTests.cs`)
- `Jroc.Tests.Node.Buffer.GeneratorTests.Require_Buffer_Undici_Core` (`tests/Jroc.Tests/Node/Buffer/GeneratorTests.cs`)

### resolveObjectURL(id)

Exported for Undici compatibility. Blob object URL creation is not yet supported, so this returns undefined.

**Tests:**
- `Jroc.Tests.Node.Buffer.ExecutionTests.Require_Buffer_Undici_Core` (`tests/Jroc.Tests/Node/Buffer/ExecutionTests.cs`)
- `Jroc.Tests.Node.Buffer.GeneratorTests.Require_Buffer_Undici_Core` (`tests/Jroc.Tests/Node/Buffer/GeneratorTests.cs`)
