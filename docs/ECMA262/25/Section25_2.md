<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 25.2: SharedArrayBuffer Objects

[Back to Section25](Section25.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-25T20:32:47Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 25.2 | SharedArrayBuffer Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 25.2.1 | Fixed-length and Growable SharedArrayBuffer Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-fixed-length-and-growable-sharedarraybuffer-objects) |
| 25.2.2 | Abstract Operations for SharedArrayBuffer Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-sharedarraybuffer-objects) |
| 25.2.2.1 | AllocateSharedArrayBuffer ( constructor , byteLength [ , maxByteLength ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-allocatesharedarraybuffer) |
| 25.2.2.2 | IsSharedArrayBuffer ( obj ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-issharedarraybuffer) |
| 25.2.2.3 | IsGrowableSharedArrayBuffer ( obj ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-isgrowablesharedarraybuffer) |
| 25.2.2.4 | HostGrowSharedArrayBuffer ( buffer , newByteLength ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-hostgrowsharedarraybuffer) |
| 25.2.3 | The SharedArrayBuffer Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer-constructor) |
| 25.2.3.1 | SharedArrayBuffer ( length [ , options ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer-length) |
| 25.2.4 | Properties of the SharedArrayBuffer Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-sharedarraybuffer-constructor) |
| 25.2.4.1 | SharedArrayBuffer.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer.prototype) |
| 25.2.4.2 | get SharedArrayBuffer [ %Symbol.species% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer-%symbol.species%) |
| 25.2.5 | Properties of the SharedArrayBuffer Prototype Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-sharedarraybuffer-prototype-object) |
| 25.2.5.1 | get SharedArrayBuffer.prototype.byteLength | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-sharedarraybuffer.prototype.bytelength) |
| 25.2.5.2 | SharedArrayBuffer.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer.prototype.constructor) |
| 25.2.5.3 | SharedArrayBuffer.prototype.grow ( newLength ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer.prototype.grow) |
| 25.2.5.4 | get SharedArrayBuffer.prototype.growable | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-sharedarraybuffer.prototype.growable) |
| 25.2.5.5 | get SharedArrayBuffer.prototype.maxByteLength | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-sharedarraybuffer.prototype.maxbytelength) |
| 25.2.5.6 | SharedArrayBuffer.prototype.slice ( start , end ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer.prototype.slice) |
| 25.2.5.7 | SharedArrayBuffer.prototype [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer.prototype-%symbol.tostringtag%) |
| 25.2.6 | Properties of SharedArrayBuffer Instances | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-sharedarraybuffer-instances) |
| 25.2.7 | Growable SharedArrayBuffer Guidelines | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-growable-sharedarraybuffer-guidelines) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 25.2 ([tc39.es](https://tc39.es/ecma262/#sec-sharedarraybuffer-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SharedArrayBuffer objects | Not Yet Supported | [`allocation-limit.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/JavaScript/allocation-limit.js)<br>[`init-zero.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/JavaScript/init-zero.js)<br>[`is-a-constructor.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/JavaScript/is-a-constructor.js)<br>[`length-is-absent.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/JavaScript/length-is-absent.js)<br>[`length-is-too-large-throws.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/JavaScript/length-is-too-large-throws.js)<br>[`negative-length-throws.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/JavaScript/negative-length-throws.js)<br>[`return-abrupt-from-length.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/JavaScript/return-abrupt-from-length.js)<br>[`zero-length.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/JavaScript/zero-length.js)<br>[`constructor.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/prototype/JavaScript/constructor.js)<br>[`return-bytelength.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/prototype/byteLength/JavaScript/return-bytelength.js)<br>[`this-is-sharedarraybuffer.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/prototype/grow/JavaScript/this-is-sharedarraybuffer.js)<br>[`context-is-not-arraybuffer-object.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/prototype/slice/JavaScript/context-is-not-arraybuffer-object.js)<br>[`context-is-not-object.js`](../../../tests/Jroc.Test262.Tests/built-ins/SharedArrayBuffer/prototype/slice/JavaScript/context-is-not-object.js) | `test/built-ins/SharedArrayBuffer/allocation-limit.js`<br>`test/built-ins/SharedArrayBuffer/init-zero.js`<br>`test/built-ins/SharedArrayBuffer/is-a-constructor.js`<br>`test/built-ins/SharedArrayBuffer/length-is-absent.js`<br>`test/built-ins/SharedArrayBuffer/length-is-too-large-throws.js`<br>`test/built-ins/SharedArrayBuffer/negative-length-throws.js`<br>`test/built-ins/SharedArrayBuffer/return-abrupt-from-length.js`<br>`test/built-ins/SharedArrayBuffer/zero-length.js`<br>`test/built-ins/SharedArrayBuffer/prototype/constructor.js`<br>`test/built-ins/SharedArrayBuffer/prototype/byteLength/return-bytelength.js`<br>`test/built-ins/SharedArrayBuffer/prototype/grow/this-is-sharedarraybuffer.js`<br>`test/built-ins/SharedArrayBuffer/prototype/slice/context-is-not-arraybuffer-object.js`<br>`test/built-ins/SharedArrayBuffer/prototype/slice/context-is-not-object.js` | Ported coverage verifies fixed-length construction, length validation/coercion, zero initialization, basic prototype members, and selected receiver validation. Growable buffers and multi-agent shared-memory semantics remain unsupported. |

