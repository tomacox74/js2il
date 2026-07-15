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
| Buffer.from(value[, encoding]) | function | supported | [docs](https://nodejs.org/api/buffer.html#static-method-bufferfromarray) |
| Buffer.alloc(size[, fill[, encoding]]) | function | supported | [docs](https://nodejs.org/api/buffer.html#static-method-bufferallocsize-fill-encoding) |
| Buffer.allocUnsafe(size) | function | supported | [docs](https://nodejs.org/api/buffer.html#static-method-bufferallocunsafesize) |
| Buffer.byteLength(value[, encoding]) | function | supported | [docs](https://nodejs.org/api/buffer.html#static-method-bufferbytelengthstring-encoding) |
| Buffer.concat(list[, totalLength]) | function | supported | [docs](https://nodejs.org/api/buffer.html#static-method-bufferconcatlist-totallength) |
| Buffer.isBuffer(value) | function | supported | [docs](https://nodejs.org/api/buffer.html#static-method-bufferisbufferobj) |
| Buffer.compare(buf1, buf2) | function | supported | [docs](https://nodejs.org/api/buffer.html#static-method-buffercomparebuf1-buf2) |
| buffer.length / buffer.byteLength / buffer.byteOffset / buffer.buffer | property | supported | [docs](https://nodejs.org/api/buffer.html#bufbuffer) |
| buffer[index] | property | supported | [docs](https://nodejs.org/api/buffer.html#bufindex) |
| buffer.toString([encoding[, start[, end]]]) | function | supported | [docs](https://nodejs.org/api/buffer.html#buftostringencoding-start-end) |
| buffer.slice([start[, end]]) / buffer.subarray([start[, end]]) | function | supported | [docs](https://nodejs.org/api/buffer.html#bufsubarraystart-end) |
| buffer.copy(target[, targetStart[, sourceStart[, sourceEnd]]]) | function | supported | [docs](https://nodejs.org/api/buffer.html#bufcopytarget-targetstart-sourcestart-sourceend) |
| buffer.equals(other) | function | supported | [docs](https://nodejs.org/api/buffer.html#bufequalsotherbuffer) |
| buffer.indexOf(value[, byteOffset[, encoding]]) / buffer.lastIndexOf(value[, byteOffset[, encoding]]) / buffer.includes(value[, byteOffset[, encoding]]) | function | supported | [docs](https://nodejs.org/api/buffer.html#bufindexofvalue-byteoffset-encoding) |
| buffer.fill(value[, offset[, end[, encoding]]]) | function | supported | [docs](https://nodejs.org/api/buffer.html#buffillvalue-offset-end-encoding) |
| buffer.write(string[, offset[, length[, encoding]]]) | function | supported | [docs](https://nodejs.org/api/buffer.html#bufwritestring-offset-length-encoding) |
| buffer.readInt8/readUInt8/readInt16BE/readInt16LE/readUInt16BE/readUInt16LE/readInt32BE/readInt32LE/readUInt32BE/readUInt32LE | function | supported | [docs](https://nodejs.org/api/buffer.html#buffer) |
| buffer.writeInt8/writeUInt8/writeInt16BE/writeInt16LE/writeUInt16BE/writeUInt16LE/writeInt32BE/writeInt32LE/writeUInt32BE/writeUInt32LE | function | supported | [docs](https://nodejs.org/api/buffer.html#buffer) |
| buffer.readFloatLE/readFloatBE/readDoubleLE/readDoubleBE | function | supported | [docs](https://nodejs.org/api/buffer.html#buffer) |
| buffer.writeFloatLE/writeFloatBE/writeDoubleLE/writeDoubleBE | function | supported | [docs](https://nodejs.org/api/buffer.html#buffer) |
| isUtf8(input) | function | supported | [docs](https://nodejs.org/api/buffer.html#bufferisutf8input) |
| resolveObjectURL(id) | function | partial | [docs](https://nodejs.org/api/buffer.html#bufferresolveobjecturlid) |

## API Details

### Buffer

Exports the same constructor as globalThis.Buffer, including base64/utf8 decoding, allocation, concatenation, binary integer I/O, views, and array-like indexing.

**Tests:**
- `Jroc.Tests.Node.Buffer.ExecutionTests.Require_Buffer_Undici_Core` (`tests/Jroc.Tests/Node/Buffer/ExecutionTests.cs`)
- `Jroc.Tests.Node.Buffer.GeneratorTests.Require_Buffer_Undici_Core` (`tests/Jroc.Tests/Node/Buffer/GeneratorTests.cs`)

### Buffer.from(value[, encoding])

Creates a Buffer from a Buffer, byte array, JavaScript array, enumerable value, or string with utf8, hex, or base64 decoding.

### Buffer.alloc(size[, fill[, encoding]])

Allocates a zeroed Buffer, optionally filled from a byte, string, or Buffer.

### Buffer.allocUnsafe(size)

Allocates an uninitialized Buffer.

### Buffer.byteLength(value[, encoding])

Returns the encoded byte length of a Buffer, byte array, or string.

### Buffer.concat(list[, totalLength])

Concatenates Buffer-compatible binary values, optionally truncating or padding to totalLength.

### Buffer.isBuffer(value)

Returns whether value is a JROC Buffer.

### Buffer.compare(buf1, buf2)

Lexicographically compares two Buffers.

### buffer.length / buffer.byteLength / buffer.byteOffset / buffer.buffer

Exposes view length, byte length, byte offset, and the backing ArrayBuffer.

### buffer[index]

Supports indexed byte reads and writes for in-bounds integer indexes.

### buffer.toString([encoding[, start[, end]]])

Decodes Buffer contents using utf8, hex, or base64 encoding.

### buffer.slice([start[, end]]) / buffer.subarray([start[, end]])

Returns Buffer views over the existing backing storage.

### buffer.copy(target[, targetStart[, sourceStart[, sourceEnd]]])

Copies bytes into a target Buffer and returns the copied byte count.

### buffer.equals(other)

Compares Buffer contents for equality.

### buffer.indexOf(value[, byteOffset[, encoding]]) / buffer.lastIndexOf(value[, byteOffset[, encoding]]) / buffer.includes(value[, byteOffset[, encoding]])

Searches for byte, string, or Buffer values.

### buffer.fill(value[, offset[, end[, encoding]]])

Fills a Buffer range from a byte, string, or Buffer value.

### buffer.write(string[, offset[, length[, encoding]]])

Writes an encoded string and returns the number of bytes written.

### buffer.readInt8/readUInt8/readInt16BE/readInt16LE/readUInt16BE/readUInt16LE/readInt32BE/readInt32LE/readUInt32BE/readUInt32LE

Reads signed and unsigned 8-, 16-, and 32-bit integer values in big- and little-endian formats.

### buffer.writeInt8/writeUInt8/writeInt16BE/writeInt16LE/writeUInt16BE/writeUInt16LE/writeInt32BE/writeInt32LE/writeUInt32BE/writeUInt32LE

Writes signed and unsigned 8-, 16-, and 32-bit integer values in big- and little-endian formats.

### buffer.readFloatLE/readFloatBE/readDoubleLE/readDoubleBE

Reads IEEE 754 single- and double-precision values in big- and little-endian formats.

### buffer.writeFloatLE/writeFloatBE/writeDoubleLE/writeDoubleBE

Writes IEEE 754 single- and double-precision values in big- and little-endian formats.

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
