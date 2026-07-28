<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 23.3: Uint8Array Objects

[Back to Section23](Section23.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-28T00:09:30Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 23.3 | Uint8Array Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-uint8array) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 23.3.1 | Additional Properties of the Uint8Array Constructor | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-additional-properties-of-the-uint8array-constructor) |
| 23.3.1.1 | Uint8Array.fromBase64 ( string [ , options ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.frombase64) |
| 23.3.1.2 | Uint8Array.fromHex ( string ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.fromhex) |
| 23.3.2 | Additional Properties of the Uint8Array Prototype Object | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-additional-properties-of-the-uint8array-prototype-object) |
| 23.3.2.1 | Uint8Array.prototype.setFromBase64 ( string [ , options ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.setfrombase64) |
| 23.3.2.2 | Uint8Array.prototype.setFromHex ( string ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.setfromhex) |
| 23.3.2.3 | Uint8Array.prototype.toBase64 ( [ options ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.tobase64) |
| 23.3.2.4 | Uint8Array.prototype.toHex ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.tohex) |
| 23.3.3 | Abstract Operations for Uint8Array Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-uint8array-objects) |
| 23.3.3.1 | ValidateUint8Array ( ta ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-validateuint8array) |
| 23.3.3.2 | GetUint8ArrayBytes ( ta ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-getuint8arraybytes) |
| 23.3.3.3 | SetUint8ArrayBytes ( into , bytes ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-setuint8arraybytes) |
| 23.3.3.4 | SkipAsciiWhitespace ( string , index ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-skipasciiwhitespace) |
| 23.3.3.5 | DecodeFinalBase64Chunk ( chunk , throwOnExtraBits ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-decodefinalbase64chunk) |
| 23.3.3.6 | DecodeFullLengthBase64Chunk ( chunk ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-decodefulllengthbase64chunk) |
| 23.3.3.7 | FromBase64 ( string , alphabet , lastChunkHandling [ , maxLength ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-frombase64) |
| 23.3.3.8 | FromHex ( string [ , maxLength ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-fromhex) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 23.3 ([tc39.es](https://tc39.es/ecma262/#sec-uint8array))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Uint8Array base64/hex extensions | Incomplete |  |  | Uint8Array.fromBase64 supports default base64 decoding/result semantics. Uint8Array.fromHex, Uint8Array.prototype.setFromHex, and Uint8Array.prototype.toHex support hexadecimal conversion with the specified descriptors, metadata, and receiver behavior. Base64 options, setFromBase64, and toBase64 remain unsupported. |

### 23.3.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-uint8array.prototype.setfromhex))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Uint8Array.prototype.setFromHex | Supported with Limitations | [`descriptor.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/descriptor.js)<br>[`illegal-characters.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/illegal-characters.js)<br>[`length.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/length.js)<br>[`name.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/name.js)<br>[`nonconstructor.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/nonconstructor.js)<br>[`results.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/results.js)<br>[`subarray.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/subarray.js)<br>[`target-size.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/target-size.js)<br>[`throws-when-string-length-is-odd.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/throws-when-string-length-is-odd.js)<br>[`writes-up-to-error.js`](../../../tests/Jroc.Test262.Tests/built-ins/Uint8Array/prototype/setFromHex/JavaScript/writes-up-to-error.js) | `test/built-ins/Uint8Array/prototype/setFromHex/descriptor.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/illegal-characters.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/length.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/name.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/nonconstructor.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/results.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/subarray.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/target-size.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/throws-when-string-length-is-odd.js`<br>`test/built-ins/Uint8Array/prototype/setFromHex/writes-up-to-error.js` | Supports strict string input, bounded writes into Uint8Array views, read/written result records, odd-length and invalid-character SyntaxErrors, writes completed byte pairs before later invalid input, and standard non-constructible built-in metadata. Detached-buffer behavior remains unsupported. |
