# Module: util/types

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/util.html#utiltypes) |

## Implementation

- `src/JavaScriptRuntime/Node/Util.cs`
- `src/JavaScriptRuntime/Node/UtilTypesModule.cs`
- `src/JavaScriptRuntime/CommonJS/Require.cs`

## Notes

Both `util/types` and `node:util/types` resolve to the same object as `require('util').types`. This module exposes the existing partial util.types predicate surface; it adds the subpath required by Undici's Fetch and WebSocket paths.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| isUint8Array(value) | function | supported | [docs](https://nodejs.org/api/util.html#utiltypesisuint8arrayvalue) |
| isArrayBuffer(value) | function | supported | [docs](https://nodejs.org/api/util.html#utiltypesisarraybufferview) |

## API Details

### isUint8Array(value)

Returns true for Uint8Array and Buffer instances and false for other typed-array views, DataView, ArrayBuffer, and ordinary values.

**Tests:**
- `Jroc.Tests.Node.Util.ExecutionTests.Require_UtilTypes_Undici_Predicates` (`tests/Jroc.Tests/Node/Util/ExecutionTests.cs`)
- `Jroc.Tests.Node.Util.GeneratorTests.Require_UtilTypes_Undici_Predicates` (`tests/Jroc.Tests/Node/Util/GeneratorTests.cs`)

### isArrayBuffer(value)

Returns true for ArrayBuffer instances and false for views, Buffer, and ordinary values.
