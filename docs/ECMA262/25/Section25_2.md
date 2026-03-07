<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 25.2: SharedArrayBuffer Objects

[Back to Section25](Section25.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-07T01:50:59Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 25.2 | SharedArrayBuffer Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 25.2.1 | Fixed-length and Growable SharedArrayBuffer Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-fixed-length-and-growable-sharedarraybuffer-objects) |
| 25.2.2 | Abstract Operations for SharedArrayBuffer Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-sharedarraybuffer-objects) |
| 25.2.2.1 | AllocateSharedArrayBuffer ( constructor , byteLength [ , maxByteLength ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-allocatesharedarraybuffer) |
| 25.2.2.2 | IsSharedArrayBuffer ( obj ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-issharedarraybuffer) |
| 25.2.2.3 | IsGrowableSharedArrayBuffer ( obj ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isgrowablesharedarraybuffer) |
| 25.2.2.4 | HostGrowSharedArrayBuffer ( buffer , newByteLength ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-hostgrowsharedarraybuffer) |
| 25.2.3 | The SharedArrayBuffer Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer-constructor) |
| 25.2.3.1 | SharedArrayBuffer ( length [ , options ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer-length) |
| 25.2.4 | Properties of the SharedArrayBuffer Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-sharedarraybuffer-constructor) |
| 25.2.4.1 | SharedArrayBuffer.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer.prototype) |
| 25.2.4.2 | get SharedArrayBuffer [ %Symbol.species% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer-%symbol.species%) |
| 25.2.5 | Properties of the SharedArrayBuffer Prototype Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-sharedarraybuffer-prototype-object) |
| 25.2.5.1 | get SharedArrayBuffer.prototype.byteLength | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-sharedarraybuffer.prototype.bytelength) |
| 25.2.5.2 | SharedArrayBuffer.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer.prototype.constructor) |
| 25.2.5.3 | SharedArrayBuffer.prototype.grow ( newLength ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer.prototype.grow) |
| 25.2.5.4 | get SharedArrayBuffer.prototype.growable | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-sharedarraybuffer.prototype.growable) |
| 25.2.5.5 | get SharedArrayBuffer.prototype.maxByteLength | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-sharedarraybuffer.prototype.maxbytelength) |
| 25.2.5.6 | SharedArrayBuffer.prototype.slice ( start , end ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer.prototype.slice) |
| 25.2.5.7 | SharedArrayBuffer.prototype [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer.prototype-%symbol.tostringtag%) |
| 25.2.6 | Properties of SharedArrayBuffer Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-sharedarraybuffer-instances) |
| 25.2.7 | Growable SharedArrayBuffer Guidelines | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-growable-sharedarraybuffer-guidelines) |

