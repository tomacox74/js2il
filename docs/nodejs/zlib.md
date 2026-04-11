# Module: zlib

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/zlib.html) |

## Implementation

- `src/JavaScriptRuntime/Node/Zlib.cs`

## Notes

Provides a focused gzip baseline for common synchronous and Transform-stream-based workflows. The current createGzip()/createGunzip() slice composes with the supported stream pipeline helpers but buffers the full payload and emits a single output chunk on end(); deflate/inflate/brotli APIs and advanced zlib tuning flags remain out of scope and fail explicitly when passed to the delivered gzip/gunzip entry points.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| gzipSync(buffer[, options]) | function | supported | [docs](https://nodejs.org/api/zlib.html#zlibgzipsyncbuffer-options) |
| gunzipSync(buffer[, options]) | function | supported | [docs](https://nodejs.org/api/zlib.html#zlibgunzipsyncbuffer-options) |
| createGzip([options]) | function | supported | [docs](https://nodejs.org/api/zlib.html#zlibcreategzipoptions) |
| createGunzip([options]) | function | supported | [docs](https://nodejs.org/api/zlib.html#zlibcreategunzipoptions) |

## API Details

### gzipSync(buffer[, options])

Accepts Buffer, ArrayBuffer, typed-array, byte-array, and string inputs. Supports the `level` option in the `-1..9` range with Node-style numeric coercion/truncation and returns a Buffer containing gzip-compressed bytes.

**Tests:**
- `Js2IL.Tests.Node.Zlib.ExecutionTests.Require_Zlib_GzipSync_GunzipSync_RoundTrip` (`tests/Js2IL.Tests/Node/Zlib/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Zlib.GeneratorTests.Require_Zlib_GzipSync_GunzipSync_RoundTrip` (`tests/Js2IL.Tests/Node/Zlib/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Zlib.ExecutionTests.Require_Zlib_ErrorPaths` (`tests/Js2IL.Tests/Node/Zlib/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Zlib.GeneratorTests.Require_Zlib_ErrorPaths` (`tests/Js2IL.Tests/Node/Zlib/GeneratorTests.cs`)

### gunzipSync(buffer[, options])

Returns a Buffer containing the decompressed gzip payload. Deferred gunzip tuning flags such as `flush`, `finishFlush`, and `chunkSize` are not implemented and fail explicitly.

**Tests:**
- `Js2IL.Tests.Node.Zlib.ExecutionTests.Require_Zlib_GzipSync_GunzipSync_RoundTrip` (`tests/Js2IL.Tests/Node/Zlib/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Zlib.GeneratorTests.Require_Zlib_GzipSync_GunzipSync_RoundTrip` (`tests/Js2IL.Tests/Node/Zlib/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Zlib.ExecutionTests.Require_Zlib_ErrorPaths` (`tests/Js2IL.Tests/Node/Zlib/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Zlib.GeneratorTests.Require_Zlib_ErrorPaths` (`tests/Js2IL.Tests/Node/Zlib/GeneratorTests.cs`)

### createGzip([options])

Returns a Transform-compatible gzip stream that buffers the current payload until end() and then emits a single compressed Buffer chunk. Supports the `level` option in the `-1..9` range with Node-style numeric coercion/truncation.

**Tests:**
- `Js2IL.Tests.Node.Zlib.ExecutionTests.Require_Zlib_Stream_RoundTrip` (`tests/Js2IL.Tests/Node/Zlib/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Zlib.GeneratorTests.Require_Zlib_Stream_RoundTrip` (`tests/Js2IL.Tests/Node/Zlib/GeneratorTests.cs`)

### createGunzip([options])

Returns a Transform-compatible gunzip stream for the current stream baseline. Deferred zlib tuning flags remain unsupported and fail explicitly.

**Tests:**
- `Js2IL.Tests.Node.Zlib.ExecutionTests.Require_Zlib_Stream_RoundTrip` (`tests/Js2IL.Tests/Node/Zlib/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Zlib.GeneratorTests.Require_Zlib_Stream_RoundTrip` (`tests/Js2IL.Tests/Node/Zlib/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Zlib.ExecutionTests.Require_Zlib_ErrorPaths` (`tests/Js2IL.Tests/Node/Zlib/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Zlib.GeneratorTests.Require_Zlib_ErrorPaths` (`tests/Js2IL.Tests/Node/Zlib/GeneratorTests.cs`)
