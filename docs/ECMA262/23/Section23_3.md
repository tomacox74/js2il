<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 23.3: Uint8Array Objects

[Back to Section23](Section23.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-07T21:14:58Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 23.3 | Uint8Array Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-uint8array) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 23.3.1 | Additional Properties of the Uint8Array Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-additional-properties-of-the-uint8array-constructor) |
| 23.3.1.1 | Uint8Array.fromBase64 ( string [ , options ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.frombase64) |
| 23.3.1.2 | Uint8Array.fromHex ( string ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.fromhex) |
| 23.3.2 | Additional Properties of the Uint8Array Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-additional-properties-of-the-uint8array-prototype-object) |
| 23.3.2.1 | Uint8Array.prototype.setFromBase64 ( string [ , options ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.setfrombase64) |
| 23.3.2.2 | Uint8Array.prototype.setFromHex ( string ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.setfromhex) |
| 23.3.2.3 | Uint8Array.prototype.toBase64 ( [ options ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.tobase64) |
| 23.3.2.4 | Uint8Array.prototype.toHex ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.tohex) |
| 23.3.3 | Abstract Operations for Uint8Array Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-uint8array-objects) |
| 23.3.3.1 | ValidateUint8Array ( ta ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-validateuint8array) |
| 23.3.3.2 | GetUint8ArrayBytes ( ta ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getuint8arraybytes) |
| 23.3.3.3 | SetUint8ArrayBytes ( into , bytes ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setuint8arraybytes) |
| 23.3.3.4 | SkipAsciiWhitespace ( string , index ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-skipasciiwhitespace) |
| 23.3.3.5 | DecodeFinalBase64Chunk ( chunk , throwOnExtraBits ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-decodefinalbase64chunk) |
| 23.3.3.6 | DecodeFullLengthBase64Chunk ( chunk ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-decodefulllengthbase64chunk) |
| 23.3.3.7 | FromBase64 ( string , alphabet , lastChunkHandling [ , maxLength ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-frombase64) |
| 23.3.3.8 | FromHex ( string [ , maxLength ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-fromhex) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 23.3 ([tc39.es](https://tc39.es/ecma262/#sec-uint8array))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Uint8Array base64/hex extensions | Supported with Limitations |  |  | Base64 decoding supports base64/base64url alphabets, loose/strict/stop-before-partial final chunks, ASCII whitespace, padding and strict overflow-bit validation, whole-chunk capacity limits, precise read/written records, and writes completed before later syntax errors. Encoding supports both alphabets and optional padding. Base64/hex instance methods validate attachment and current bounds after their specified input/option side effects. Broader resizable-buffer and exotic-object coverage is not exhaustive. |

### 23.3.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-uint8array.frombase64))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Uint8Array.fromBase64 | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Uint8Array/fromBase64/TypedArrayConformanceExecutionTests.cs` | `test/built-ins/Uint8Array/fromBase64/alphabet.js`<br>`test/built-ins/Uint8Array/fromBase64/last-chunk-handling.js`<br>`test/built-ins/Uint8Array/fromBase64/last-chunk-invalid.js`<br>`test/built-ins/Uint8Array/fromBase64/option-coercion.js`<br>`test/built-ins/Uint8Array/fromBase64/whitespace.js` | Supports both alphabets and all three final-chunk modes through direct and receiver-aware intrinsic calls, retaining length 1. Options are objects with ordered, single getter reads and string-valued validation without ToString coercion. Only ASCII whitespace is ignored; malformed characters, padding, incomplete strict chunks, and strict overflow bits throw SyntaxError. |

### 23.3.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.setfrombase64))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Uint8Array.prototype.setFromBase64 | Supported with Limitations | [`descriptor.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromBase64/JavaScript/descriptor.js)<br>[`results.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromBase64/JavaScript/results.js)<br>`tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromBase64/TypedArrayConformanceExecutionTests.cs` | `test/built-ins/Uint8Array/prototype/setFromBase64/descriptor.js`<br>`test/built-ins/Uint8Array/prototype/setFromBase64/results.js`<br>`test/built-ins/Uint8Array/prototype/setFromBase64/alphabet.js`<br>`test/built-ins/Uint8Array/prototype/setFromBase64/detached-buffer.js`<br>`test/built-ins/Uint8Array/prototype/setFromBase64/last-chunk-handling.js`<br>`test/built-ins/Uint8Array/prototype/setFromBase64/option-coercion.js`<br>`test/built-ins/Uint8Array/prototype/setFromBase64/subarray.js`<br>`test/built-ins/Uint8Array/prototype/setFromBase64/target-size.js`<br>`test/built-ins/Uint8Array/prototype/setFromBase64/trailing-garbage-empty.js`<br>`test/built-ins/Uint8Array/prototype/setFromBase64/trailing-garbage.js`<br>`test/built-ins/Uint8Array/prototype/setFromBase64/whitespace.js`<br>`test/built-ins/Uint8Array/prototype/setFromBase64/writes-up-to-error.js` | Uses the shared option-aware Base64 decoder with view-relative, whole-chunk capacity limits and precise read/written records. Completed chunks remain written if a later chunk is malformed; exhausted targets ignore unconsumed trailing input, including all input for an empty target. Options are read and validated before current attachment/bounds checks. Broader resizable-buffer and exotic-object coverage is not exhaustive. |

### 23.3.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.setfromhex))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Uint8Array.prototype.setFromHex | Supported with Limitations | [`descriptor.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/descriptor.js)<br>[`illegal-characters.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/illegal-characters.js)<br>[`length.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/length.js)<br>[`name.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/name.js)<br>[`nonconstructor.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/nonconstructor.js)<br>[`results.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/results.js)<br>[`subarray.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/subarray.js)<br>[`target-size.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/target-size.js)<br>[`throws-when-string-length-is-odd.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/throws-when-string-length-is-odd.js)<br>[`writes-up-to-error.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/writes-up-to-error.js)<br>[`detached-buffer.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/detached-buffer.js) | `test/built-ins/Uint8Array/prototype/setFromHex/descriptor.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/illegal-characters.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/length.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/name.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/nonconstructor.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/results.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/subarray.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/target-size.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/throws-when-string-length-is-odd.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/writes-up-to-error.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/detached-buffer.js` | Supports strict string input, bounded writes into Uint8Array views, read/written result records, odd-length and invalid-character SyntaxErrors, writes completed byte pairs before later invalid input, and standard non-constructible built-in metadata. Detached and out-of-bounds receivers are rejected before decoding. |

### 23.3.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.tobase64))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Uint8Array.prototype.toBase64 | Supported with Limitations | [`alphabet.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/toBase64/JavaScript/alphabet.js)<br>[`descriptor.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/toBase64/JavaScript/descriptor.js)<br>[`length.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/toBase64/JavaScript/length.js)<br>[`name.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/toBase64/JavaScript/name.js)<br>[`nonconstructor.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/toBase64/JavaScript/nonconstructor.js)<br>[`omit-padding.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/toBase64/JavaScript/omit-padding.js)<br>[`option-coercion.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/toBase64/JavaScript/option-coercion.js)<br>[`results.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/toBase64/JavaScript/results.js)<br>[`detached-buffer.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/toBase64/JavaScript/detached-buffer.js) | `test/built-ins/Uint8Array/prototype/toBase64/alphabet.js`<br>`test/built-ins/Uint8Array/prototype/toBase64/descriptor.js`<br>`test/built-ins/Uint8Array/prototype/toBase64/length.js`<br>`test/built-ins/Uint8Array/prototype/toBase64/name.js`<br>`test/built-ins/Uint8Array/prototype/toBase64/nonconstructor.js`<br>`test/built-ins/Uint8Array/prototype/toBase64/omit-padding.js`<br>`test/built-ins/Uint8Array/prototype/toBase64/option-coercion.js`<br>`test/built-ins/Uint8Array/prototype/toBase64/results.js`<br>`test/built-ins/Uint8Array/prototype/toBase64/detached-buffer.js` | Supports RFC 4648 vectors, base64 and base64url alphabets, optional padding omission, option getter ordering and coercion, receiver validation, and standard non-constructible built-in metadata. Attachment and current bounds are checked after option getters, including getters that detach the buffer. |

### 23.3.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.tohex))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Uint8Array.prototype.toHex | Supported with Limitations | [`detached-buffer.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/toHex/JavaScript/detached-buffer.js) | `test/built-ins/Uint8Array/prototype/toHex/detached-buffer.js` | Encodes the current in-bounds view as lowercase hexadecimal and rejects detached or out-of-bounds receivers before reading bytes. |
