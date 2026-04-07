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
- `src/JavaScriptRuntime/Node/StreamPromises.cs`

## Notes

The stream baseline now includes lifecycle controls and helpers that are useful to higher-level Node modules: pause()/resume(), UTF-8 setEncoding(), destroy()/destroyed, constructor-level object mode flags, callback-style or Promise-backed pipeline(...)/finished(...), `node:stream/promises`, and AbortSignal-aware helper cancellation with deterministic writable drain/finish/error teardown. Important gaps remain: cork()/uncork(), richer write() callback semantics, array/iterable/web-stream pipeline forms, byte-accurate non-object-mode buffering, broader encoding breadth, and the more complex teardown/buffering edge cases that full Node parity requires.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| pipeline(...streams[, callback]) | function | partial | [docs](https://nodejs.org/api/stream.html#streampipelinesource-transforms-destination-callback) |
| finished(stream[, callback]) | function | partial | [docs](https://nodejs.org/api/stream.html#streamfinishedstream-options-callback) |
| node:stream/promises | module | partial | [docs](https://nodejs.org/api/stream.html#stream-promises-api) |
| Readable | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamreadable) |
| Writable | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamwritable) |
| Duplex | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamduplex) |
| Transform | class | partial | [docs](https://nodejs.org/api/stream.html#class-streamtransform) |
| PassThrough | class | partial | [docs](https://nodejs.org/api/stream.html#class-streampassthrough) |

## API Details

### pipeline(...streams[, callback])

Supports callback-oriented pipeline chaining across the current EventEmitter-backed Readable/Duplex -> Writable/Duplex stream classes. The final non-stream argument may be an options object with `signal`, allowing AbortSignal-driven cancellation that destroys the participating streams and surfaces `AbortError`. Omitting the callback returns the same Promise-backed completion path used by `node:stream/promises`.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipeline_Error` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipeline_Error` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Promises_Pipeline_AbortSignal` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Promises_Pipeline_AbortSignal` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### finished(stream[, callback])

Supports callback-oriented completion/error observation for the current Readable, Writable, Duplex, Transform, and PassThrough implementations by monitoring end, finish, error, and premature close signals. An optional options object with `signal` cancels the observer with `AbortError`. Omitting the callback returns the same Promise-backed observer used by `node:stream/promises`.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Finished_Callback_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Finished_Callback_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Finished_Callback_DestroyError` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Finished_Callback_DestroyError` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Promises_Finished_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Promises_Finished_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### node:stream/promises

Provides Promise-oriented `pipeline(...)` and `finished(...)` wrappers over the stream helper runtime. The current slice supports EventEmitter-backed stream instances plus `options.signal` cancellation; advanced iterable/web-stream forms remain out of scope.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Promises_Finished_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Promises_Finished_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Promises_Pipeline_ObjectMode` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Promises_Pipeline_ObjectMode` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Promises_Pipeline_AbortSignal` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Promises_Pipeline_AbortSignal` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Readable

Readable streams now support push(), read(), pipe(), pause(), resume(), destroy(), destroyed, readable, readableEncoding, readableObjectMode, and setEncoding('utf8'/'utf-8'). Constructor options may enable `objectMode` / `readableObjectMode` so arbitrary JS values flow without requiring Buffer/string chunks. The current setEncoding() slice is UTF-8-only and covers buffered and flowing string decoding for Buffer/byte[] backed chunks. pipe() keeps the minimal backpressure integration: when destination.write(...) returns false, forwarding pauses until the destination emits drain.

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
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Promises_Pipeline_ObjectMode` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Promises_Pipeline_ObjectMode` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Writable

Writable streams support write(), end(), destroy(), destroyed, writable, writableObjectMode, and the current highWaterMark field/constructor option. Constructor options may enable `objectMode` / `writableObjectMode`, and the current highWaterMark logic counts queued chunks rather than byte length. write() still treats encoding/callback parameters as baseline no-ops, but drain/finish ordering is now queued deterministically and _write exceptions propagate as stream error/close teardown instead of being swallowed silently.

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
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Promises_Pipeline_ObjectMode` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Promises_Pipeline_ObjectMode` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Duplex

Duplex combines the new readable-side lifecycle controls (pause/resume, UTF-8 setEncoding, destroy/destroyed, pipe backpressure, readableObjectMode) with the Writable behavior above (including writableObjectMode/highWaterMark options). In the current runtime it primarily acts as the base for Transform, PassThrough, and NetSocket-style consumers.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Promises_Pipeline_ObjectMode` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Promises_Pipeline_ObjectMode` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### Transform

Transform streams support a user-provided _transform function for chunk mapping. Constructor object-mode options flow through both the readable and writable sides, making object-to-object transforms practical for higher-level adapters. Throwing from _transform now destroys the stream and surfaces the error through pipeline()/finished()/EventEmitter listeners instead of swallowing it silently.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Transform_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Transform_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipeline_Error` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipeline_Error` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Promises_Pipeline_ObjectMode` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Promises_Pipeline_ObjectMode` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)

### PassThrough

PassThrough remains the identity Transform, now participating in the same lifecycle/helper surface as Duplex/Transform.

**Tests:**
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_PassThrough_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Stream.ExecutionTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Stream.GeneratorTests.Stream_Pipeline_Basic` (`Js2IL.Tests/Node/Stream/GeneratorTests.cs`)
