<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 25.3: DataView Objects

[Back to Section25](Section25.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-07T01:50:59Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 25.3 | DataView Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 25.3.1 | Abstract Operations For DataView Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-dataview-objects) |
| 25.3.1.1 | DataView With Buffer Witness Records | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview-with-buffer-witness-records) |
| 25.3.1.2 | MakeDataViewWithBufferWitnessRecord ( obj , order ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-makedataviewwithbufferwitnessrecord) |
| 25.3.1.3 | GetViewByteLength ( viewRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getviewbytelength) |
| 25.3.1.4 | IsViewOutOfBounds ( viewRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isviewoutofbounds) |
| 25.3.1.5 | GetViewValue ( view , requestIndex , isLittleEndian , type ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getviewvalue) |
| 25.3.1.6 | SetViewValue ( view , requestIndex , isLittleEndian , type , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-setviewvalue) |
| 25.3.2 | The DataView Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview-constructor) |
| 25.3.2.1 | DataView ( buffer [ , byteOffset [ , byteLength ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview-buffer-byteoffset-bytelength) |
| 25.3.3 | Properties of the DataView Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-dataview-constructor) |
| 25.3.3.1 | DataView.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype) |
| 25.3.4 | Properties of the DataView Prototype Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-dataview-prototype-object) |
| 25.3.4.1 | get DataView.prototype.buffer | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-dataview.prototype.buffer) |
| 25.3.4.2 | get DataView.prototype.byteLength | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-dataview.prototype.bytelength) |
| 25.3.4.3 | get DataView.prototype.byteOffset | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-dataview.prototype.byteoffset) |
| 25.3.4.4 | DataView.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.constructor) |
| 25.3.4.5 | DataView.prototype.getBigInt64 ( byteOffset [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.getbigint64) |
| 25.3.4.6 | DataView.prototype.getBigUint64 ( byteOffset [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.getbiguint64) |
| 25.3.4.7 | DataView.prototype.getFloat16 ( byteOffset [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.getfloat16) |
| 25.3.4.8 | DataView.prototype.getFloat32 ( byteOffset [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.getfloat32) |
| 25.3.4.9 | DataView.prototype.getFloat64 ( byteOffset [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.getfloat64) |
| 25.3.4.10 | DataView.prototype.getInt8 ( byteOffset ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.getint8) |
| 25.3.4.11 | DataView.prototype.getInt16 ( byteOffset [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.getint16) |
| 25.3.4.12 | DataView.prototype.getInt32 ( byteOffset [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.getint32) |
| 25.3.4.13 | DataView.prototype.getUint8 ( byteOffset ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.getuint8) |
| 25.3.4.14 | DataView.prototype.getUint16 ( byteOffset [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.getuint16) |
| 25.3.4.15 | DataView.prototype.getUint32 ( byteOffset [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.getuint32) |
| 25.3.4.16 | DataView.prototype.setBigInt64 ( byteOffset , value [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.setbigint64) |
| 25.3.4.17 | DataView.prototype.setBigUint64 ( byteOffset , value [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.setbiguint64) |
| 25.3.4.18 | DataView.prototype.setFloat16 ( byteOffset , value [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.setfloat16) |
| 25.3.4.19 | DataView.prototype.setFloat32 ( byteOffset , value [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.setfloat32) |
| 25.3.4.20 | DataView.prototype.setFloat64 ( byteOffset , value [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.setfloat64) |
| 25.3.4.21 | DataView.prototype.setInt8 ( byteOffset , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.setint8) |
| 25.3.4.22 | DataView.prototype.setInt16 ( byteOffset , value [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.setint16) |
| 25.3.4.23 | DataView.prototype.setInt32 ( byteOffset , value [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.setint32) |
| 25.3.4.24 | DataView.prototype.setUint8 ( byteOffset , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.setuint8) |
| 25.3.4.25 | DataView.prototype.setUint16 ( byteOffset , value [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.setuint16) |
| 25.3.4.26 | DataView.prototype.setUint32 ( byteOffset , value [ , littleEndian ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype.setuint32) |
| 25.3.4.27 | DataView.prototype [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dataview.prototype-%symbol.tostringtag%) |
| 25.3.5 | Properties of DataView Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-dataview-instances) |

