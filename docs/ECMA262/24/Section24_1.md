<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 24.1: Map Objects

[Back to Section24](Section24.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-29T06:55:11Z

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
| 24.1.2.1 | Map.groupBy ( items , callback ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-map.groupby) |
| 24.1.2.2 | Map.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype) |
| 24.1.2.3 | get Map [ %Symbol.species% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-map-%symbol.species%) |
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
| 24.1.3.15 | Map.prototype [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype-%symbol.tostringtag%) |
| 24.1.4 | Properties of Map Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-map-instances) |
| 24.1.5 | Map Iterator Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-map-iterator-objects) |
| 24.1.5.1 | CreateMapIterator ( map , kind ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createmapiterator) |
| 24.1.5.2 | The %MapIteratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%mapiteratorprototype%-object) |
| 24.1.5.2.1 | %MapIteratorPrototype%.next ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%mapiteratorprototype%.next) |
| 24.1.5.2.2 | %MapIteratorPrototype% [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%mapiteratorprototype%-%symbol.tostringtag%) |

## Support

Feature-level support tracking with test script references.

### 24.1.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-map-iterable))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| new Map() | Supported | [`Map_Constructor_Empty.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Constructor_Empty.js) | Zero-argument construction lowers to JavaScriptRuntime.Map() and produces an insertion-ordered keyed collection. |
| new Map(iterable) | Supported | [`Map_Constructor_Iterable.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Constructor_Iterable.js) | JavaScriptRuntime.Map now accepts a single iterable argument, consumes it via the runtime iterator protocol, and inserts each [key, value] pair in order. |

### 24.1.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Map constructor value and Map.prototype surface | Supported with Limitations | [`Map_Constructor_Prototype_Surface.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Constructor_Prototype_Surface.js) | JS2IL now exposes globalThis.Map as a constructor value, wires Map.prototype and Map.prototype.constructor, stamps new Map instances with that prototype, and supports reflective checks such as Object.getPrototypeOf(map) === Map.prototype and map instanceof Map. Iterable construction, forEach, and @@iterator are now implemented; Symbol.species and full MapIteratorPrototype metadata remain incomplete. |

### 24.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-map-prototype-object))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Core Map mutators/accessors (set, get, has, delete, clear, size) | Supported | [`Map_Set_Get_Basic.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Set_Get_Basic.js)<br>[`Map_Has_Basic.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Has_Basic.js)<br>[`Map_Delete_Basic.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Delete_Basic.js)<br>[`Map_Clear_Basic.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Clear_Basic.js)<br>[`Map_Size_Property.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Size_Property.js)<br>[`Map_Set_Returns_This.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Set_Returns_This.js)<br>[`Map_Update_Existing_Key.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Update_Existing_Key.js)<br>[`Map_Multiple_Keys.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Multiple_Keys.js)<br>[`Map_Null_Key.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Null_Key.js) | These operations are implemented by JavaScriptRuntime.Map with insertion-order storage and SameValueZero-style key comparison for common numeric cases. |

### 24.1.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype.entries))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Map.prototype.keys / values / entries | Supported with Limitations | [`Map_Keys_Values_Entries.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Keys_Values_Entries.js)<br>[`Map_Symbol_Iterator.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Symbol_Iterator.js) | The methods are exposed on the public Map.prototype surface and return native iterator objects with .next(). Iterator prototype metadata remains incomplete. |

### 24.1.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype.foreach))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Map.prototype.forEach | Supported | [`Map_ForEach_Basic.js`](../../../Js2IL.Tests/Map/JavaScript/Map_ForEach_Basic.js) | forEach invokes the callback in insertion order with (value, key, map) arguments and honors the optional thisArg. |

### 24.1.3.7 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype.getorinsert))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Map.prototype.getOrInsert / getOrInsertComputed | Not Yet Supported |  | The newer getOrInsert APIs are still not implemented on JavaScriptRuntime.Map. |

### 24.1.3.14 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype-%symbol.iterator%))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Map.prototype[@@iterator] | Supported | [`Map_Symbol_Iterator.js`](../../../Js2IL.Tests/Map/JavaScript/Map_Symbol_Iterator.js) | Map instances now expose a Symbol.iterator method that returns the same entry iterator shape as entries(). |

### 24.1.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-createmapiterator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Map iteration in for-of and other runtime iterator consumers | Supported with Limitations |  | Map instances now expose Symbol.iterator and the keys/values/entries methods return native iterator objects. Iterator prototype metadata such as %MapIteratorPrototype%[@@toStringTag] remains incomplete. |

