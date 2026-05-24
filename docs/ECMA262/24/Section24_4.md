<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 24.4: WeakSet Objects

[Back to Section24](Section24.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T19:56:41Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 24.4 | WeakSet Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakset-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 24.4.1 | The WeakSet Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakset-constructor) |
| 24.4.1.1 | WeakSet ( [ iterable ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakset-iterable) |
| 24.4.2 | Properties of the WeakSet Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weakset-constructor) |
| 24.4.2.1 | WeakSet.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype) |
| 24.4.3 | Properties of the WeakSet Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weakset-prototype-object) |
| 24.4.3.1 | WeakSet.prototype.add ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype.add) |
| 24.4.3.2 | WeakSet.prototype.constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype.constructor) |
| 24.4.3.3 | WeakSet.prototype.delete ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype.delete) |
| 24.4.3.4 | WeakSet.prototype.has ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype.has) |
| 24.4.3.5 | WeakSet.prototype [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype-%symbol.tostringtag%) |
| 24.4.4 | Properties of WeakSet Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-weakset-instances) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 24.4.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-weakset-iterable))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| new WeakSet() | Supported | [`WeakSet_Constructor_Empty.js`](../../../tests/Js2IL.Tests/WeakSet/JavaScript/WeakSet_Constructor_Empty.js) | `test/built-ins/WeakSet/undefined-newtarget.js` | Parameterless construction succeeds and allocates a ConditionalWeakTable-backed collection. |
| new WeakSet(iterable) | Supported with Limitations | `tests/Js2IL.Test262.Tests/built-ins/WeakSet/ExecutionTests.cs` |  | WeakSet(iterable) consumes iterable values through the constructor's adder path, including the covered test262 adder lookup failure semantics. Weak-value validation and broader exotic iterator edge cases remain partial. |

### 24.4.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| WeakSet constructor value and WeakSet.prototype surface | Supported with Limitations | [`WeakSet_Constructor_Prototype_Surface.js`](../../../tests/Js2IL.Tests/WeakSet/JavaScript/WeakSet_Constructor_Prototype_Surface.js) | `test/built-ins/WeakSet/prototype-of-weakset.js` | JS2IL exposes globalThis.WeakSet as a constructor value with test262-covered name/length/constructibility/global descriptor metadata, wires WeakSet.prototype and WeakSet.prototype.constructor, and attaches the public prototype to new WeakSet instances so Object.getPrototypeOf(weakSet) and weakSet instanceof WeakSet observe the JS-visible surface. Iterable construction is supported; the remaining WeakSet prototype gaps are still incomplete. |

### 24.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weakset-prototype-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| WeakSet.prototype.add / has / delete | Supported with Limitations | [`WeakSet_Add_Has_Basic.js`](../../../tests/Js2IL.Tests/WeakSet/JavaScript/WeakSet_Add_Has_Basic.js)<br>[`WeakSet_Delete_Basic.js`](../../../tests/Js2IL.Tests/WeakSet/JavaScript/WeakSet_Delete_Basic.js)<br>[`WeakSet_Object_Values.js`](../../../tests/Js2IL.Tests/WeakSet/JavaScript/WeakSet_Object_Values.js) |  | Core WeakSet flows work for the object values used in tests. The runtime only rejects null, so it does not fully enforce ECMAScript's object-only weak-value restrictions for every non-null primitive or boxed value. |

### 24.4.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| WeakSet.prototype[@@toStringTag] | Not Yet Supported |  |  | WeakSet instances do not currently expose a symbol-keyed toStringTag property. |

