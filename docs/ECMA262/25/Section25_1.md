<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 25.1: ArrayBuffer Objects

[Back to Section25](Section25.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-24T01:16:21Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 25.1 | ArrayBuffer Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 25.1.1 | Notation | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer-notation) |
| 25.1.2 | Fixed-length and Resizable ArrayBuffer Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-fixed-length-and-resizable-arraybuffer-objects) |
| 25.1.3 | Abstract Operations For ArrayBuffer Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-arraybuffer-objects) |
| 25.1.3.1 | AllocateArrayBuffer ( constructor , byteLength [ , maxByteLength ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-allocatearraybuffer) |
| 25.1.3.2 | ArrayBufferByteLength ( arrayBuffer , order ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybufferbytelength) |
| 25.1.3.3 | ArrayBufferCopyAndDetach ( arrayBuffer , newLength , preserveResizability ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffercopyanddetach) |
| 25.1.3.4 | IsDetachedBuffer ( arrayBuffer ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-isdetachedbuffer) |
| 25.1.3.5 | DetachArrayBuffer ( arrayBuffer [ , key ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-detacharraybuffer) |
| 25.1.3.6 | CloneArrayBuffer ( srcBuffer , srcByteOffset , srcLength ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-clonearraybuffer) |
| 25.1.3.7 | GetArrayBufferMaxByteLengthOption ( options ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getarraybuffermaxbytelengthoption) |
| 25.1.3.8 | HostResizeArrayBuffer ( buffer , newByteLength ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-hostresizearraybuffer) |
| 25.1.3.9 | IsFixedLengthArrayBuffer ( arrayBuffer ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isfixedlengtharraybuffer) |
| 25.1.3.10 | IsUnsignedElementType ( type ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isunsignedelementtype) |
| 25.1.3.11 | IsUnclampedIntegerElementType ( type ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isunclampedintegerelementtype) |
| 25.1.3.12 | IsBigIntElementType ( type ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-isbigintelementtype) |
| 25.1.3.13 | IsNoTearConfiguration ( type , order ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-isnotearconfiguration) |
| 25.1.3.14 | RawBytesToNumeric ( type , rawBytes , isLittleEndian ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-rawbytestonumeric) |
| 25.1.3.15 | GetRawBytesFromSharedBlock ( block , byteIndex , type , isTypedArray , order ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getrawbytesfromsharedblock) |
| 25.1.3.16 | GetValueFromBuffer ( arrayBuffer , byteIndex , type , isTypedArray , order [ , isLittleEndian ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getvaluefrombuffer) |
| 25.1.3.17 | NumericToRawBytes ( type , value , isLittleEndian ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numerictorawbytes) |
| 25.1.3.18 | SetValueInBuffer ( arrayBuffer , byteIndex , type , value , isTypedArray , order [ , isLittleEndian ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setvalueinbuffer) |
| 25.1.3.19 | GetModifySetValueInBuffer ( arrayBuffer , byteIndex , type , value , op ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-getmodifysetvalueinbuffer) |
| 25.1.4 | The ArrayBuffer Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer-constructor) |
| 25.1.4.1 | ArrayBuffer ( length [ , options ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer-length) |
| 25.1.5 | Properties of the ArrayBuffer Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-arraybuffer-constructor) |
| 25.1.5.1 | ArrayBuffer.isView ( arg ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.isview) |
| 25.1.5.2 | ArrayBuffer.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype) |
| 25.1.5.3 | get ArrayBuffer [ %Symbol.species% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-arraybuffer-%symbol.species%) |
| 25.1.6 | Properties of the ArrayBuffer Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-arraybuffer-prototype-object) |
| 25.1.6.1 | get ArrayBuffer.prototype.byteLength | Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-arraybuffer.prototype.bytelength) |
| 25.1.6.2 | ArrayBuffer.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype.constructor) |
| 25.1.6.3 | get ArrayBuffer.prototype.detached | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-arraybuffer.prototype.detached) |
| 25.1.6.4 | get ArrayBuffer.prototype.maxByteLength | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-arraybuffer.prototype.maxbytelength) |
| 25.1.6.5 | get ArrayBuffer.prototype.resizable | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-arraybuffer.prototype.resizable) |
| 25.1.6.6 | ArrayBuffer.prototype.resize ( newLength ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype.resize) |
| 25.1.6.7 | ArrayBuffer.prototype.slice ( start , end ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype.slice) |
| 25.1.6.8 | ArrayBuffer.prototype.transfer ( [ newLength ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype.transfer) |
| 25.1.6.9 | ArrayBuffer.prototype.transferToFixedLength ( [ newLength ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype.transfertofixedlength) |
| 25.1.6.10 | ArrayBuffer.prototype [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.prototype-%symbol.tostringtag%) |
| 25.1.7 | Properties of ArrayBuffer Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-arraybuffer-instances) |
| 25.1.8 | Resizable ArrayBuffer Guidelines | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-resizable-arraybuffer-guidelines) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 25.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-arraybuffer-constructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ArrayBuffer first-class constructor and prototype metadata | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/ArrayBuffer/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/ArrayBuffer/prototype/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/ArrayBuffer/prototype/byteLength/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/ArrayBuffer/prototype/maxByteLength/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/ArrayBuffer/prototype/resizable/ExecutionTests.cs` | `test/built-ins/ArrayBuffer/length.js`<br>`test/built-ins/ArrayBuffer/name.js`<br>`test/built-ins/ArrayBuffer/prop-desc.js`<br>`test/built-ins/ArrayBuffer/newtarget-prototype-is-not-object.js`<br>`test/built-ins/ArrayBuffer/prototype/constructor.js`<br>`test/built-ins/ArrayBuffer/prototype/Symbol.toStringTag.js` | Exposes globalThis.ArrayBuffer as a constructible function with standard name, length, global-property, prototype descriptors, and the standard @@species accessor. ArrayBuffer.prototype has receiver-checked byteLength, maxByteLength, resizable, and @@toStringTag properties. Custom newTarget prototypes, detachment, transfer, and resizable-buffer operations remain limited. |

### 25.1.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-arraybuffer-length))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ArrayBuffer(length) | Supported with Limitations | [`ArrayBuffer_Construct_ByteLength.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/ArrayBuffer_Construct_ByteLength.js) |  | Implemented as a fixed-length JavaScriptRuntime.ArrayBuffer backed by byte[]. Length uses ToIndex-like truncation for finite non-negative numbers. Resizable buffers, detached buffers, and transfer APIs are not implemented. |

### 25.1.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-arraybuffer.isview))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ArrayBuffer.isView | Supported with Limitations | [`ArrayBuffer_IsView_DataView.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/ArrayBuffer_IsView_DataView.js)<br>`tests/Jroc.Test262.Tests/built-ins/ArrayBuffer/PortNext200ExecutionTests.cs` | `test/built-ins/ArrayBuffer/isView/arg-is-typedarray.js` | Recognizes DataView and the supported TypedArray implementations as ArrayBuffer views. Broader TypedArray families and shared ArrayBuffer-backed view infrastructure remain follow-up work for issue #774. |

### 25.1.5.3 ([tc39.es](https://tc39.es/ecma262/#sec-get-arraybuffer-%symbol.species%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| get ArrayBuffer [ @@species ] | Supported | `tests/Jroc.Test262.Tests/built-ins/ArrayBuffer/Symbol.species/ExecutionTests.cs` |  | The ArrayBuffer constructor exposes the standard configurable, non-enumerable @@species getter with the covered name, length, and receiver-return semantics. |

### 25.1.6 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-arraybuffer-prototype-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ArrayBuffer.prototype.byteLength and ArrayBuffer.prototype.slice | Supported with Limitations | [`ArrayBuffer_Construct_ByteLength.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/ArrayBuffer_Construct_ByteLength.js)<br>`tests/Jroc.Test262.Tests/built-ins/ArrayBuffer/prototype/slice/ExecutionTests.cs` |  | byteLength reflects the backing byte[] length, and slice returns a copied fixed-length ArrayBuffer. The slice function now exposes the standard descriptor, extensibility, name, and length metadata. Species construction beyond the intrinsic getter, detach, resize, and transfer semantics remain limited. |

