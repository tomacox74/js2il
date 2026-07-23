<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 24.1: Map Objects

[Back to Section24](Section24.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-23T02:06:52Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 24.1 | Map Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-map-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 24.1.1 | The Map Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-map-constructor) |
| 24.1.1.1 | Map ( [ iterable ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-map-iterable) |
| 24.1.1.2 | AddEntriesFromIterable ( target , iterable , adder ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-add-entries-from-iterable) |
| 24.1.2 | Properties of the Map Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-map-constructor) |
| 24.1.2.1 | Map.groupBy ( items , callback ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.groupby) |
| 24.1.2.2 | Map.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype) |
| 24.1.2.3 | get Map [ %Symbol.species% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-map-%symbol.species%) |
| 24.1.3 | Properties of the Map Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-map-prototype-object) |
| 24.1.3.1 | Map.prototype.clear ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.clear) |
| 24.1.3.2 | Map.prototype.constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.constructor) |
| 24.1.3.3 | Map.prototype.delete ( key ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.delete) |
| 24.1.3.4 | Map.prototype.entries ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.entries) |
| 24.1.3.5 | Map.prototype.forEach ( callback [ , thisArg ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.foreach) |
| 24.1.3.6 | Map.prototype.get ( key ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.get) |
| 24.1.3.7 | Map.prototype.getOrInsert ( key , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.getorinsert) |
| 24.1.3.8 | Map.prototype.getOrInsertComputed ( key , callback ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.getorinsertcomputed) |
| 24.1.3.9 | Map.prototype.has ( key ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.has) |
| 24.1.3.10 | Map.prototype.keys ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.keys) |
| 24.1.3.11 | Map.prototype.set ( key , value ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.set) |
| 24.1.3.12 | get Map.prototype.size | Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-map.prototype.size) |
| 24.1.3.13 | Map.prototype.values ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.values) |
| 24.1.3.14 | Map.prototype [ %Symbol.iterator% ] ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype-%symbol.iterator%) |
| 24.1.3.15 | Map.prototype [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype-%symbol.tostringtag%) |
| 24.1.4 | Properties of Map Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-map-instances) |
| 24.1.5 | Map Iterator Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-map-iterator-objects) |
| 24.1.5.1 | CreateMapIterator ( map , kind ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createmapiterator) |
| 24.1.5.2 | The %MapIteratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%mapiteratorprototype%-object) |
| 24.1.5.2.1 | %MapIteratorPrototype%.next ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%mapiteratorprototype%.next) |
| 24.1.5.2.2 | %MapIteratorPrototype% [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-%mapiteratorprototype%-%symbol.tostringtag%) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 24.1.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-map-iterable))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| new Map() | Supported | [`Map_Constructor_Empty.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Constructor_Empty.js) | `test/built-ins/Map/undefined-newtarget.js` | Zero-argument construction lowers to JavaScriptRuntime.Map() and produces an insertion-ordered keyed collection. |
| new Map(iterable) | Supported | [`Map_Constructor_Iterable.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Constructor_Iterable.js) |  | JavaScriptRuntime.Map accepts a single iterable argument, consumes it via the runtime iterator protocol, observes constructor adder lookup/call-order semantics, treats an own/inherited undefined @@iterator as a TypeError, and inserts each [key, value] pair in order for the covered test262 constructor cases. |

### 24.1.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-map.groupby))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Map.groupBy | Supported | `tests/Jroc.Test262.Tests/built-ins/Map/groupBy/ExecutionTests.cs` | `test/built-ins/Map/groupBy/callback-arg.js`<br>`test/built-ins/Map/groupBy/callback-throws.js`<br>`test/built-ins/Map/groupBy/emptyList.js`<br>`test/built-ins/Map/groupBy/evenOdd.js`<br>`test/built-ins/Map/groupBy/groupLength.js`<br>`test/built-ins/Map/groupBy/invalid-callback.js`<br>`test/built-ins/Map/groupBy/invalid-iterable.js`<br>`test/built-ins/Map/groupBy/iterator-next-throws.js`<br>`test/built-ins/Map/groupBy/length.js`<br>`test/built-ins/Map/groupBy/map-instance.js`<br>`test/built-ins/Map/groupBy/name.js`<br>`test/built-ins/Map/groupBy/negativeZero.js`<br>`test/built-ins/Map/groupBy/toPropertyKey.js` | Map.groupBy is exposed as a non-constructible static built-in with standard name and length metadata. It groups iterable values into insertion-ordered Map buckets, passes each callback (value, index), preserves object keys without property-key coercion, normalizes negative zero, and closes iterators after abrupt completion. |

### 24.1.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Map constructor value and Map.prototype surface | Supported with Limitations | [`Map_Constructor_Prototype_Surface.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Constructor_Prototype_Surface.js) | `test/built-ins/Map/prototype-of-map.js` | JROC exposes globalThis.Map as a constructor value with test262-covered name/length/constructibility/global descriptor metadata, wires Map.prototype and Map.prototype.constructor, stamps new Map instances with that prototype, and supports reflective checks such as Object.getPrototypeOf(map) === Map.prototype and map instanceof Map. Iterable construction, forEach, and @@iterator are implemented. |

### 24.1.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-get-map-%symbol.species%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Map[Symbol.species] accessor | Supported | `tests/Jroc.Test262.Tests/built-ins/Map/Symbol.species/ExecutionTests.cs` | `test/built-ins/Map/Symbol.species/length.js`<br>`test/built-ins/Map/Symbol.species/return-value.js`<br>`test/built-ins/Map/Symbol.species/symbol-species-name.js`<br>`test/built-ins/Map/Symbol.species/symbol-species.js` | Map now exposes @@species as a configurable, non-enumerable accessor with no setter. The getter returns the this value and has covered function metadata (name "get [Symbol.species]", length 0). |

### 24.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-map-prototype-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Core Map mutators/accessors (set, get, has, delete, clear, size) | Supported | [`Map_Set_Get_Basic.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Set_Get_Basic.js)<br>[`Map_Has_Basic.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Has_Basic.js)<br>[`Map_Delete_Basic.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Delete_Basic.js)<br>[`Map_Clear_Basic.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Clear_Basic.js)<br>[`Map_Size_Property.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Size_Property.js)<br>[`Map_Set_Returns_This.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Set_Returns_This.js)<br>[`Map_Update_Existing_Key.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Update_Existing_Key.js)<br>[`Map_Multiple_Keys.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Multiple_Keys.js)<br>[`Map_Null_Key.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Null_Key.js)<br>`tests/Jroc.Test262.Tests/built-ins/Map/prototype/get/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Map/prototype/has/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Map/prototype/set/ExecutionTests.cs` | `test/built-ins/Map/prototype/get/get.js`<br>`test/built-ins/Map/prototype/get/returns-undefined.js`<br>`test/built-ins/Map/prototype/get/returns-value-different-key-types.js`<br>`test/built-ins/Map/prototype/get/returns-value-normalized-zero-key.js`<br>`test/built-ins/Map/prototype/get/this-not-object-throw.js`<br>`test/built-ins/Map/prototype/has/has.js`<br>`test/built-ins/Map/prototype/has/normalizes-zero-key.js`<br>`test/built-ins/Map/prototype/has/return-false-different-key-types.js`<br>`test/built-ins/Map/prototype/has/return-true-different-key-types.js`<br>`test/built-ins/Map/prototype/has/this-not-object-throw.js`<br>`test/built-ins/Map/prototype/set/append-new-values-normalizes-zero-key.js`<br>`test/built-ins/Map/prototype/set/append-new-values-return-map.js`<br>`test/built-ins/Map/prototype/set/replaces-a-value.js`<br>`test/built-ins/Map/prototype/set/set.js`<br>`test/built-ins/Map/prototype/set/this-not-object-throw.js` | These operations are implemented by JavaScriptRuntime.Map with insertion-order storage, SameValueZero-style key comparison for common numeric cases, TypeError receiver checks, chainable set() behavior, and replacement of existing key records. |

### 24.1.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype.entries))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Map.prototype.keys / values / entries | Supported with Limitations | [`Map_Keys_Values_Entries.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Keys_Values_Entries.js)<br>[`Map_Symbol_Iterator.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Symbol_Iterator.js) |  | The methods are exposed on the public Map.prototype surface and return native iterator objects with .next(). |

### 24.1.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype.foreach))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Map.prototype.forEach | Supported | [`Map_ForEach_Basic.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_ForEach_Basic.js) |  | forEach invokes the callback in insertion order with (value, key, map) arguments and honors the optional thisArg. |

### 24.1.3.7 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype.getorinsert))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Map.prototype.getOrInsert / getOrInsertComputed | Not Yet Supported |  |  | The newer getOrInsert APIs are still not implemented on JavaScriptRuntime.Map. |

### 24.1.3.14 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype-%symbol.iterator%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Map.prototype[@@iterator] | Supported | [`Map_Symbol_Iterator.js`](../../../tests/Jroc.Tests/Map/JavaScript/Map_Symbol_Iterator.js) |  | Map instances now expose a Symbol.iterator method that returns the same entry iterator shape as entries(). |

### 24.1.3.15 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Map.prototype[@@toStringTag] | Supported | `tests/Jroc.Test262.Tests/built-ins/Map/prototype/Symbol.toStringTag/ExecutionTests.cs` | `test/built-ins/Map/prototype/Symbol.toStringTag.js` | Map.prototype exposes the configurable, non-enumerable, non-writable "Map" @@toStringTag data property. |

### 24.1.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-createmapiterator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Map iteration in for-of and other runtime iterator consumers | Supported with Limitations |  |  | Map instances expose Symbol.iterator and the keys/values/entries methods return native iterator objects with a dedicated %MapIteratorPrototype%. |

### 24.1.5.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-%mapiteratorprototype%-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| %MapIteratorPrototype%[@@toStringTag] | Supported | `tests/Jroc.Test262.Tests/built-ins/MapIteratorPrototype/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Object/prototype/toString/ExecutionTests.cs` | `test/built-ins/MapIteratorPrototype/Symbol.toStringTag.js`<br>`test/built-ins/Object/prototype/toString/symbol-tag-map-builtin.js` | Map iterator instances inherit from a dedicated %MapIteratorPrototype% with the configurable, non-enumerable, non-writable "Map Iterator" @@toStringTag data property. |

