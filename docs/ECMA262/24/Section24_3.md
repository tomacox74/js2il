<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 24.3: WeakMap Objects

[Back to Section24](Section24.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-27T12:25:28Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 24.3 | WeakMap Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakmap-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 24.3.1 | The WeakMap Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakmap-constructor) |
| 24.3.1.1 | WeakMap ( [ iterable ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakmap-iterable) |
| 24.3.2 | Properties of the WeakMap Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weakmap-constructor) |
| 24.3.2.1 | WeakMap.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype) |
| 24.3.3 | Properties of the WeakMap Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weakmap-prototype-object) |
| 24.3.3.1 | WeakMap.prototype.constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype.constructor) |
| 24.3.3.2 | WeakMap.prototype.delete ( key ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype.delete) |
| 24.3.3.3 | WeakMap.prototype.get ( key ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype.get) |
| 24.3.3.4 | WeakMap.prototype.getOrInsert ( key , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype.getorinsert) |
| 24.3.3.5 | WeakMap.prototype.getOrInsertComputed ( key , callback ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype.getorinsertcomputed) |
| 24.3.3.6 | WeakMap.prototype.has ( key ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype.has) |
| 24.3.3.7 | WeakMap.prototype.set ( key , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype.set) |
| 24.3.3.8 | WeakMap.prototype [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype-%symbol.tostringtag%) |
| 24.3.4 | Properties of WeakMap Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-weakmap-instances) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 24.3.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-weakmap-iterable))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| new WeakMap() | Supported | [`WeakMap_Constructor_Empty.js`](../../../tests/Js2IL.Tests/WeakMap/JavaScript/WeakMap_Constructor_Empty.js) | `test/built-ins/WeakMap/undefined-newtarget.js` | Parameterless construction succeeds and allocates a ConditionalWeakTable-backed collection. |
| new WeakMap(iterable) | Supported with Limitations | `tests/Js2IL.Test262.Tests/built-ins/WeakMap/ExecutionTests.cs` |  | WeakMap(iterable) consumes iterable entry pairs through the constructor's adder path, including the covered test262 adder lookup failure semantics. Weak-key validation and broader exotic iterator edge cases remain partial. |

### 24.3.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| WeakMap constructor value and WeakMap.prototype surface | Supported with Limitations | [`WeakMap_Constructor_Prototype_Surface.js`](../../../tests/Js2IL.Tests/WeakMap/JavaScript/WeakMap_Constructor_Prototype_Surface.js) | `test/built-ins/WeakMap/prototype-of-weakmap.js` | JS2IL exposes globalThis.WeakMap as a constructor value with test262-covered name/length/constructibility/global descriptor metadata, wires WeakMap.prototype and WeakMap.prototype.constructor, and stamps new WeakMap instances with the public prototype so reflective prototype checks and instanceof use the JS-visible surface. Iterable construction is supported; the remaining WeakMap prototype gaps are still incomplete. |

### 24.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weakmap-prototype-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| WeakMap.prototype.set / get / has / delete | Supported with Limitations | [`WeakMap_Set_Get_Basic.js`](../../../tests/Js2IL.Tests/WeakMap/JavaScript/WeakMap_Set_Get_Basic.js)<br>[`WeakMap_Has_Basic.js`](../../../tests/Js2IL.Tests/WeakMap/JavaScript/WeakMap_Has_Basic.js)<br>[`WeakMap_Delete_Basic.js`](../../../tests/Js2IL.Tests/WeakMap/JavaScript/WeakMap_Delete_Basic.js)<br>[`WeakMap_Object_Keys.js`](../../../tests/Js2IL.Tests/WeakMap/JavaScript/WeakMap_Object_Keys.js)<br>`tests/Js2IL.Test262.Tests/built-ins/WeakMap/prototype/get/ExecutionTests.cs` | `test/built-ins/WeakMap/prototype/get/get.js`<br>`test/built-ins/WeakMap/prototype/get/returns-undefined-if-key-cannot-be-held-weakly.js`<br>`test/built-ins/WeakMap/prototype/get/returns-undefined-with-object-key.js`<br>`test/built-ins/WeakMap/prototype/get/this-not-object-throw.js` | Core WeakMap flows work for the object keys used in tests. Current checked-in coverage includes get() receiver checks, object-key lookup, and undefined results for missing or non-weakly-held keys. The runtime only rejects null, so it does not fully enforce ECMAScript's object-only weak-key restrictions for every non-null primitive or boxed value. |

### 24.3.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype.getorinsert))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| WeakMap.prototype.getOrInsert / getOrInsertComputed | Not Yet Supported |  |  | The newer WeakMap insertion helpers are not implemented. |

### 24.3.3.8 ([tc39.es](https://tc39.es/ecma262/#sec-weakmap.prototype-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| WeakMap.prototype[@@toStringTag] | Not Yet Supported |  |  | WeakMap instances do not currently expose a symbol-keyed toStringTag property. |

