<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 24.2: Set Objects

[Back to Section24](Section24.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-29T06:55:11Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 24.2 | Set Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 24.2.1 | Abstract Operations For Set Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-set-objects) |
| 24.2.1.1 | Set Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-set-records) |
| 24.2.1.2 | GetSetRecord ( obj ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-getsetrecord) |
| 24.2.1.3 | SetDataHas ( setData , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-setdatahas) |
| 24.2.1.4 | SetDataIndex ( setData , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-setdataindex) |
| 24.2.1.5 | SetDataSize ( setData ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-setdatasize) |
| 24.2.2 | The Set Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set-constructor) |
| 24.2.2.1 | Set ( [ iterable ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set-iterable) |
| 24.2.3 | Properties of the Set Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-set-constructor) |
| 24.2.3.1 | Set.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype) |
| 24.2.3.2 | get Set [ %Symbol.species% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-set-%symbol.species%) |
| 24.2.4 | Properties of the Set Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-set-prototype-object) |
| 24.2.4.1 | Set.prototype.add ( value ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.add) |
| 24.2.4.2 | Set.prototype.clear ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.clear) |
| 24.2.4.3 | Set.prototype.constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.constructor) |
| 24.2.4.4 | Set.prototype.delete ( value ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.delete) |
| 24.2.4.5 | Set.prototype.difference ( other ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.difference) |
| 24.2.4.6 | Set.prototype.entries ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.entries) |
| 24.2.4.7 | Set.prototype.forEach ( callback [ , thisArg ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.foreach) |
| 24.2.4.8 | Set.prototype.has ( value ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.has) |
| 24.2.4.9 | Set.prototype.intersection ( other ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.intersection) |
| 24.2.4.10 | Set.prototype.isDisjointFrom ( other ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.isdisjointfrom) |
| 24.2.4.11 | Set.prototype.isSubsetOf ( other ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.issubsetof) |
| 24.2.4.12 | Set.prototype.isSupersetOf ( other ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.issupersetof) |
| 24.2.4.13 | Set.prototype.keys ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.keys) |
| 24.2.4.14 | get Set.prototype.size | Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-set.prototype.size) |
| 24.2.4.15 | Set.prototype.symmetricDifference ( other ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.symmetricdifference) |
| 24.2.4.16 | Set.prototype.union ( other ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.union) |
| 24.2.4.17 | Set.prototype.values ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype.values) |
| 24.2.4.18 | Set.prototype [ %Symbol.iterator% ] ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype-%symbol.iterator%) |
| 24.2.4.19 | Set.prototype [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype-%symbol.tostringtag%) |
| 24.2.5 | Properties of Set Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-set-instances) |
| 24.2.6 | Set Iterator Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set-iterator-objects) |
| 24.2.6.1 | CreateSetIterator ( set , kind ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createsetiterator) |
| 24.2.6.2 | The %SetIteratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%setiteratorprototype%-object) |
| 24.2.6.2.1 | %SetIteratorPrototype%.next ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%setiteratorprototype%.next) |
| 24.2.6.2.2 | %SetIteratorPrototype% [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%setiteratorprototype%-%symbol.tostringtag%) |

## Support

Feature-level support tracking with test script references.

### 24.2.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-set-iterable))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| new Set() | Supported | [`Set_Constructor_Prototype_Surface.js`](../../../Js2IL.Tests/Set/JavaScript/Set_Constructor_Prototype_Surface.js)<br>[`Require_Util_Types_Expanded.js`](../../../Js2IL.Tests/Node/Util/JavaScript/Require_Util_Types_Expanded.js) | Parameterless construction succeeds and produces a JavaScriptRuntime.Set instance that other runtime services can recognize. |
| new Set(iterable) | Supported | [`Set_Constructor_Iterable.js`](../../../Js2IL.Tests/Set/JavaScript/Set_Constructor_Iterable.js) | JavaScriptRuntime.Set now accepts a single iterable argument and consumes it with the runtime iterator protocol, preserving insertion order and ignoring duplicates. |

### 24.2.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-set.prototype))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Set constructor value and Set.prototype surface | Supported with Limitations | [`Set_Constructor_Prototype_Surface.js`](../../../Js2IL.Tests/Set/JavaScript/Set_Constructor_Prototype_Surface.js) | JS2IL now exposes globalThis.Set as a constructor value, wires Set.prototype and Set.prototype.constructor, attaches the public prototype to new Set instances, and supports reflective checks such as Object.getPrototypeOf(set) === Set.prototype and set instanceof Set. Iterable construction and the core prototype surface are implemented; Symbol.species, full SetIteratorPrototype metadata, and the spec's broader set-like-object protocol remain incomplete. |

### 24.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-set-prototype-object))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Implemented Set members: add, has, size, clear, delete, entries, forEach, keys, values, @@iterator | Supported | [`Set_Core_Methods.js`](../../../Js2IL.Tests/Set/JavaScript/Set_Core_Methods.js)<br>[`Set_Entries_Keys_Values.js`](../../../Js2IL.Tests/Set/JavaScript/Set_Entries_Keys_Values.js)<br>[`Set_ForEach_Basic.js`](../../../Js2IL.Tests/Set/JavaScript/Set_ForEach_Basic.js)<br>[`Set_Symbol_Iterator.js`](../../../Js2IL.Tests/Set/JavaScript/Set_Symbol_Iterator.js) | JavaScriptRuntime.Set now exposes the core prototype method family on the public Set.prototype surface and returns native iterator objects for keys/values/entries. |

### 24.2.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-set.prototype.clear))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Core Set prototype members (clear, delete, entries, forEach, keys, values, @@iterator) | Supported | [`Set_Core_Methods.js`](../../../Js2IL.Tests/Set/JavaScript/Set_Core_Methods.js)<br>[`Set_Entries_Keys_Values.js`](../../../Js2IL.Tests/Set/JavaScript/Set_Entries_Keys_Values.js)<br>[`Set_ForEach_Basic.js`](../../../Js2IL.Tests/Set/JavaScript/Set_ForEach_Basic.js)<br>[`Set_Symbol_Iterator.js`](../../../Js2IL.Tests/Set/JavaScript/Set_Symbol_Iterator.js) | These members are available on Set instances and use native iterator objects for keys/values/entries. |

### 24.2.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-set.prototype.difference))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| New Set methods (difference, intersection, isDisjointFrom, isSubsetOf, isSupersetOf, symmetricDifference, union) | Supported with Limitations | [`Set_Algebra_Methods.js`](../../../Js2IL.Tests/Set/JavaScript/Set_Algebra_Methods.js) | The ES2025 Set operation methods are implemented, but non-Set operands are normalized through new Set(iterable) rather than the full spec set-like-object protocol. |

### 24.2.6.1 ([tc39.es](https://tc39.es/ecma262/#sec-createsetiterator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Set iteration in for-of and other runtime iterator consumers | Supported with Limitations |  | Set instances now expose Symbol.iterator and the keys/values/entries methods return native iterator objects. Iterator prototype metadata such as %SetIteratorPrototype%[@@toStringTag] remains incomplete. |

