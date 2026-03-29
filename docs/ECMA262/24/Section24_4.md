<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 24.4: WeakSet Objects

[Back to Section24](Section24.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-29T04:29:49Z

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

Feature-level support tracking with test script references.

### 24.4.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-weakset-iterable))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| new WeakSet() | Supported | [`WeakSet_Constructor_Empty.js`](../../../Js2IL.Tests/WeakSet/JavaScript/WeakSet_Constructor_Empty.js) | Parameterless construction succeeds and allocates a ConditionalWeakTable-backed collection. |
| new WeakSet(iterable) | Not Yet Supported |  | JavaScriptRuntime.WeakSet exposes only a parameterless constructor, so iterable initialization is not available. |

### 24.4.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| WeakSet constructor value and WeakSet.prototype surface | Supported with Limitations | [`WeakSet_Constructor_Prototype_Surface.js`](../../../Js2IL.Tests/WeakSet/JavaScript/WeakSet_Constructor_Prototype_Surface.js) | JS2IL now exposes globalThis.WeakSet as a constructor value, wires WeakSet.prototype and WeakSet.prototype.constructor, and attaches the public prototype to new WeakSet instances so Object.getPrototypeOf(weakSet) and weakSet instanceof WeakSet observe the JS-visible surface. Iterable construction and the remaining WeakSet prototype gaps are still incomplete. |

### 24.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weakset-prototype-object))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| WeakSet.prototype.add / has / delete | Supported with Limitations | [`WeakSet_Add_Has_Basic.js`](../../../Js2IL.Tests/WeakSet/JavaScript/WeakSet_Add_Has_Basic.js)<br>[`WeakSet_Delete_Basic.js`](../../../Js2IL.Tests/WeakSet/JavaScript/WeakSet_Delete_Basic.js)<br>[`WeakSet_Object_Values.js`](../../../Js2IL.Tests/WeakSet/JavaScript/WeakSet_Object_Values.js) | Core WeakSet flows work for the object values used in tests. The runtime only rejects null, so it does not fully enforce ECMAScript's object-only weak-value restrictions for every non-null primitive or boxed value. |

### 24.4.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype-%symbol.tostringtag%))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| WeakSet.prototype[@@toStringTag] | Not Yet Supported |  | WeakSet instances do not currently expose a symbol-keyed toStringTag property. |

