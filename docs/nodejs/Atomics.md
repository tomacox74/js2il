# Global: Atomics

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/globals.html#class-atomics) |

## Implementation

- `src/JavaScriptRuntime/Atomics.cs`
- `src/JavaScriptRuntime/GlobalThis.cs`
- `src/JavaScriptRuntime/Int32Array.cs`
- `src/JavaScriptRuntime/SharedArrayBuffer.cs`

## Notes

Minimal global support added to keep the compiled scripts/release.js canary runnable under JROC. Only Atomics.wait(...) is currently exposed, and it is implemented as a blocking timeout sleep suitable for the release-script polling path rather than full shared-memory synchronization semantics.

## Tests

- `Jroc.Tests.TypedArray.ExecutionTests.SharedArrayBuffer_Int32Array_AtomicsWait` (`tests/Jroc.Tests/TypedArray/ExecutionTests.cs`)
- `Jroc.Tests.TypedArray.GeneratorTests.SharedArrayBuffer_Int32Array_AtomicsWait` (`tests/Jroc.Tests/TypedArray/GeneratorTests.cs`)
