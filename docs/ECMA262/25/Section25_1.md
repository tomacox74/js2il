<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 25.1: ArrayBuffer Objects

[Back to Section25](Section25.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-07T06:48:54Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 25.1 | ArrayBuffer Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 25.1.1 | Notation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer-notation) |
| 25.1.2 | Fixed-length and Resizable ArrayBuffer Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-fixed-length-and-resizable-arraybuffer-objects) |
| 25.1.3 | Abstract Operations For ArrayBuffer Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-arraybuffer-objects) |
| 25.1.3.1 | AllocateArrayBuffer ( constructor , byteLength [ , maxByteLength ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-allocatearraybuffer) |
| 25.1.3.2 | ArrayBufferByteLength ( arrayBuffer , order ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybufferbytelength) |
| 25.1.3.3 | ArrayBufferCopyAndDetach ( arrayBuffer , newLength , preserveResizability ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffercopyanddetach) |
| 25.1.3.4 | IsDetachedBuffer ( arrayBuffer ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isdetachedbuffer) |
| 25.1.3.5 | DetachArrayBuffer ( arrayBuffer [ , key ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-detacharraybuffer) |
| 25.1.3.6 | CloneArrayBuffer ( srcBuffer , srcByteOffset , srcLength ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-clonearraybuffer) |
| 25.1.3.7 | GetArrayBufferMaxByteLengthOption ( options ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getarraybuffermaxbytelengthoption) |
| 25.1.3.8 | HostResizeArrayBuffer ( buffer , newByteLength ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-hostresizearraybuffer) |
| 25.1.3.9 | IsFixedLengthArrayBuffer ( arrayBuffer ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isfixedlengtharraybuffer) |
| 25.1.3.10 | IsUnsignedElementType ( type ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isunsignedelementtype) |
| 25.1.3.11 | IsUnclampedIntegerElementType ( type ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isunclampedintegerelementtype) |
| 25.1.3.12 | IsBigIntElementType ( type ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isbigintelementtype) |
| 25.1.3.13 | IsNoTearConfiguration ( type , order ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isnotearconfiguration) |
| 25.1.3.14 | RawBytesToNumeric ( type , rawBytes , isLittleEndian ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-rawbytestonumeric) |
| 25.1.3.15 | GetRawBytesFromSharedBlock ( block , byteIndex , type , isTypedArray , order ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getrawbytesfromsharedblock) |
| 25.1.3.16 | GetValueFromBuffer ( arrayBuffer , byteIndex , type , isTypedArray , order [ , isLittleEndian ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getvaluefrombuffer) |
| 25.1.3.17 | NumericToRawBytes ( type , value , isLittleEndian ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numerictorawbytes) |
| 25.1.3.18 | SetValueInBuffer ( arrayBuffer , byteIndex , type , value , isTypedArray , order [ , isLittleEndian ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setvalueinbuffer) |
| 25.1.3.19 | GetModifySetValueInBuffer ( arrayBuffer , byteIndex , type , value , op ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getmodifysetvalueinbuffer) |
| 25.1.4 | The ArrayBuffer Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer-constructor) |
| 25.1.4.1 | ArrayBuffer ( length [ , options ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer-length) |
| 25.1.5 | Properties of the ArrayBuffer Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-arraybuffer-constructor) |
| 25.1.5.1 | ArrayBuffer.isView ( arg ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.isview) |
| 25.1.5.2 | ArrayBuffer.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype) |
| 25.1.5.3 | get ArrayBuffer [ %Symbol.species% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-arraybuffer-%symbol.species%) |
| 25.1.6 | Properties of the ArrayBuffer Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-arraybuffer-prototype-object) |
| 25.1.6.1 | get ArrayBuffer.prototype.byteLength | Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-arraybuffer.prototype.bytelength) |
| 25.1.6.2 | ArrayBuffer.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype.constructor) |
| 25.1.6.3 | get ArrayBuffer.prototype.detached | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-arraybuffer.prototype.detached) |
| 25.1.6.4 | get ArrayBuffer.prototype.maxByteLength | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-arraybuffer.prototype.maxbytelength) |
| 25.1.6.5 | get ArrayBuffer.prototype.resizable | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-arraybuffer.prototype.resizable) |
| 25.1.6.6 | ArrayBuffer.prototype.resize ( newLength ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype.resize) |
| 25.1.6.7 | ArrayBuffer.prototype.slice ( start , end ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype.slice) |
| 25.1.6.8 | ArrayBuffer.prototype.transfer ( [ newLength ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype.transfer) |
| 25.1.6.9 | ArrayBuffer.prototype.transferToFixedLength ( [ newLength ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype.transfertofixedlength) |
| 25.1.6.10 | ArrayBuffer.prototype [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype-%symbol.tostringtag%) |
| 25.1.7 | Properties of ArrayBuffer Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-arraybuffer-instances) |
| 25.1.8 | Resizable ArrayBuffer Guidelines | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-resizable-arraybuffer-guidelines) |

## Support

Feature-level support tracking with test script references.

### 25.1.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-arraybuffer-length))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ArrayBuffer(length) | Supported with Limitations | [`ArrayBuffer_Construct_ByteLength.js`](../../../Js2IL.Tests/TypedArray/JavaScript/ArrayBuffer_Construct_ByteLength.js) | Implemented as a fixed-length JavaScriptRuntime.ArrayBuffer backed by byte[]. Length uses ToIndex-like truncation for finite non-negative numbers. Resizable buffers, detached buffers, and transfer APIs are not implemented. |

### 25.1.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.isview))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ArrayBuffer.isView | Supported with Limitations | [`ArrayBuffer_IsView_DataView.js`](../../../Js2IL.Tests/TypedArray/JavaScript/ArrayBuffer_IsView_DataView.js) | Recognizes DataView and the current Int32Array typed-array surface as ArrayBuffer views. Broader TypedArray families and shared ArrayBuffer-backed view infrastructure remain follow-up work for issue #774. |

### 25.1.6 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-arraybuffer-prototype-object))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ArrayBuffer.prototype.byteLength and ArrayBuffer.prototype.slice | Supported with Limitations | [`ArrayBuffer_Construct_ByteLength.js`](../../../Js2IL.Tests/TypedArray/JavaScript/ArrayBuffer_Construct_ByteLength.js) | byteLength reflects the backing byte[] length, and slice returns a copied fixed-length ArrayBuffer. Species, detach, resize, and transfer semantics are not implemented. |

