# Module: stream

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/stream.html) |

## Implementation

- `src/JavaScriptRuntime/Node/Stream.cs`
- `src/JavaScriptRuntime/Node/Readable.cs`
- `src/JavaScriptRuntime/Node/Writable.cs`
- `src/JavaScriptRuntime/Node/Duplex.cs`
- `src/JavaScriptRuntime/Node/Transform.cs`
- `src/JavaScriptRuntime/Node/PassThrough.cs`

## Notes

Expanded the initial stream baseline to include minimal Duplex/Transform/PassThrough classes and basic pipe() backpressure behavior (pause on write()==false, resume on drain). Many Node.js stream features are still not implemented (objectMode, encoding/setEncoding, pause()/resume() APIs, pipeline/finished helpers, full Writable buffering semantics, cork/uncork, and complex error/teardown edge cases).

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| Readable | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamreadable) |
| Writable | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamwritable) |
| Duplex | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamduplex) |
| Transform | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamtransform) |
| PassThrough | class | partial | [docs](https://nodejs.org/api/stream.html#class-streampassthrough) |

## API Details

### Readable

Basic Readable stream implementation. Supports push(), read(), pipe(), and the readable property. Emits 'data', 'end', and 'error' events via EventEmitter inheritance. pipe() includes a minimal backpressure integration: when destination.write(...) returns false, pipe() pauses forwarding and resumes on the destination 'drain' event.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Readable_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Readable_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipe_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipe_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipe_Backpressure_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipe_Backpressure_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Writable

Basic Writable stream implementation. Supports write(), end(), and the writable property. Emits 'drain', 'finish', and 'error' events via EventEmitter inheritance. Includes a minimal highWaterMark field (default 16) used to influence the boolean return of write(...) for backpressure signaling; encoding and callback handling are still baseline/ignored.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Writable_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Writable_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Writable_CustomWrite` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Writable_CustomWrite` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipe_Backpressure_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipe_Backpressure_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Duplex

Minimal Duplex stream implementation (Readable + Writable). Intended primarily as a base class for Transform and PassThrough in the current baseline.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Transform

Minimal Transform stream implementation. Supports a user-provided _transform function for chunk mapping; when the writable side ends, the readable side is ended by pushing null.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Transform_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Transform_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### PassThrough

Minimal PassThrough implementation (identity Transform).

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
