# Global: SharedArrayBuffer

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/globals.html#class-sharedarraybuffer) |

## Implementation

- `src/JavaScriptRuntime/SharedArrayBuffer.cs`
- `src/JavaScriptRuntime/ArrayBuffer.cs`
- `src/JavaScriptRuntime/GlobalThis.cs`

## Notes

Minimal global support added for the compiled scripts/release.js canary. Construction and byteLength are supported well enough for Int32Array-backed Atomics.wait polling, but the broader SharedArrayBuffer prototype surface and standards coverage remain incomplete.

## Tests

- `Jroc.Tests.TypedArray.ExecutionTests.SharedArrayBuffer_Int32Array_AtomicsWait` (`tests/Jroc.Tests/TypedArray/ExecutionTests.cs`)
- `Jroc.Tests.TypedArray.GeneratorTests.SharedArrayBuffer_Int32Array_AtomicsWait` (`tests/Jroc.Tests/TypedArray/GeneratorTests.cs`)
