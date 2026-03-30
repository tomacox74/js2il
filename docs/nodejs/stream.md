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

The stream baseline now includes lifecycle controls and helpers that are useful to higher-level Node modules: pause()/resume(), UTF-8 setEncoding(), destroy()/destroyed, callback-style pipeline(...), callback-style finished(...), and deterministic writable drain/finish/error teardown. Important gaps remain: objectMode, cork()/uncork(), richer write() callback semantics, AbortSignal support, node:stream/promises, binary/socket-oriented encoding breadth, and the more complex teardown/buffering edge cases that full Node parity requires.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| pipeline(...streams[, callback]) | function | partial | [docs](https://nodejs.org/api/stream.html#streampipelinesource-transforms-destination-callback) |
| finished(stream[, callback]) | function | partial | [docs](https://nodejs.org/api/stream.html#streamfinishedstream-options-callback) |
| Readable | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamreadable) |
| Writable | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamwritable) |
| Duplex | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamduplex) |
| Transform | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamtransform) |
| PassThrough | class | partial | [docs](https://nodejs.org/api/stream.html#class-streampassthrough) |

## API Details

### pipeline(...streams[, callback])

Supports callback-oriented pipeline chaining across the current EventEmitter-backed Readable/Duplex -> Writable/Duplex stream classes. On the first observed stream error or premature close, the helper tears down the remaining streams and invokes the callback with that error. The official node:stream/promises surface and AbortSignal-aware variants are not implemented yet.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipeline_Error` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipeline_Error` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### finished(stream[, callback])

Supports callback-oriented completion/error observation for the current Readable, Writable, Duplex, Transform, and PassThrough implementations by monitoring end, finish, error, and premature close signals. Omitting the callback returns a Promise-backed observer from the same runtime path, but node:stream/promises is still out of scope.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Finished_Callback_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Finished_Callback_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Finished_Callback_DestroyError` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Finished_Callback_DestroyError` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Readable

Readable streams now support push(), read(), pipe(), pause(), resume(), destroy(), destroyed, readable, readableEncoding, and setEncoding('utf8'/'utf-8'). The current setEncoding() slice is UTF-8-only and covers buffered and flowing string decoding for Buffer/byte[] backed chunks. pipe() keeps the minimal backpressure integration: when destination.write(...) returns false, forwarding pauses until the destination emits drain.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Readable_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Readable_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipe_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipe_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipe_Backpressure_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipe_Backpressure_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Readable_Pause_Resume` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Readable_Pause_Resume` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Readable_SetEncoding_Utf8` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Readable_SetEncoding_Utf8` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Writable

Writable streams support write(), end(), destroy(), destroyed, writable, and the current highWaterMark field. write() still treats encoding/callback parameters as baseline no-ops, but drain/finish ordering is now queued deterministically and _write exceptions propagate as stream error/close teardown instead of being swallowed silently.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Writable_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Writable_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Writable_CustomWrite` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Writable_CustomWrite` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipe_Backpressure_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipe_Backpressure_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Writable_Drain_Finish_Order` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Writable_Drain_Finish_Order` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Writable_Destroy_Error` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Writable_Destroy_Error` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Duplex

Duplex combines the new readable-side lifecycle controls (pause/resume, UTF-8 setEncoding, destroy/destroyed, pipe backpressure) with the Writable behavior above. In the current runtime it primarily acts as the base for Transform, PassThrough, and NetSocket-style consumers.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Transform

Transform streams support a user-provided _transform function for chunk mapping. Throwing from _transform now destroys the stream and surfaces the error through pipeline()/finished()/EventEmitter listeners instead of swallowing it silently.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Transform_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Transform_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipeline_Error` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipeline_Error` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### PassThrough

PassThrough remains the identity Transform, now participating in the same lifecycle/helper surface as Duplex/Transform.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
