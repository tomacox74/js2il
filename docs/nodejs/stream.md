# Module: stream

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/stream.html) |

## Implementation

- `JavaScriptRuntime/Node/Stream.cs`
- `JavaScriptRuntime/Node/Readable.cs`
- `JavaScriptRuntime/Node/Writable.cs`

## Notes

Initial stream module baseline focused on basic Readable and Writable stream classes. Both extend EventEmitter for event handling. The implementation supports basic data flow patterns and pipe() functionality. Advanced stream features (Duplex, Transform, pipeline utilities, backpressure handling, encoding, objectMode, highWaterMark) are not implemented yet. This baseline enables simple stream-based workflows required by common Node.js libraries.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| Readable | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamreadable) |
| Writable | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamwritable) |

## API Details

### Readable

Basic Readable stream implementation. Supports push(), read(), pipe(), and the readable property. Emits 'data', 'end', and 'error' events via EventEmitter inheritance. Advanced features like flowing mode, pausing, and highWaterMark are not yet implemented.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Readable_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Readable_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipe_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipe_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Writable

Basic Writable stream implementation. Supports write(), end(), and the writable property. Emits 'drain', 'finish', and 'error' events via EventEmitter inheritance. The _write property can be set to customize write behavior. Advanced features like highWaterMark and encoding handling are not yet implemented.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Writable_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Writable_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Writable_CustomWrite` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Writable_CustomWrite` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
