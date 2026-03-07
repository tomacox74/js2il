<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 24.4: WeakSet Objects

[Back to Section24](Section24.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-07T01:50:59Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 24.4 | WeakSet Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakset-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 24.4.1 | The WeakSet Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakset-constructor) |
| 24.4.1.1 | WeakSet ( [ iterable ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakset-iterable) |
| 24.4.2 | Properties of the WeakSet Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weakset-constructor) |
| 24.4.2.1 | WeakSet.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype) |
| 24.4.3 | Properties of the WeakSet Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weakset-prototype-object) |
| 24.4.3.1 | WeakSet.prototype.add ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype.add) |
| 24.4.3.2 | WeakSet.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype.constructor) |
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
| WeakSet constructor value and WeakSet.prototype surface | Not Yet Supported |  | WeakSet is compiler-recognized in new expressions but is not exposed as a first-class global constructor value with readable prototype properties. |

### 24.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weakset-prototype-object))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| WeakSet.prototype.add / has / delete | Supported with Limitations | [`WeakSet_Add_Has_Basic.js`](../../../Js2IL.Tests/WeakSet/JavaScript/WeakSet_Add_Has_Basic.js)<br>[`WeakSet_Delete_Basic.js`](../../../Js2IL.Tests/WeakSet/JavaScript/WeakSet_Delete_Basic.js)<br>[`WeakSet_Object_Values.js`](../../../Js2IL.Tests/WeakSet/JavaScript/WeakSet_Object_Values.js) | Core WeakSet flows work for the object values used in tests. The runtime only rejects null, so it does not fully enforce ECMAScript's object-only weak-value restrictions for every non-null primitive or boxed value. |

### 24.4.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-weakset.prototype-%symbol.tostringtag%))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| WeakSet.prototype[@@toStringTag] | Not Yet Supported |  | WeakSet instances do not currently expose a symbol-keyed toStringTag property. |

