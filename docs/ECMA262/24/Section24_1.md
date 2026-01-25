<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 24.1: Map Objects

[Back to Section24](Section24.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 24.1 | Map Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 24.1.1 | The Map Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map-constructor) |
| 24.1.1.1 | Map ( [ iterable ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map-iterable) |
| 24.1.1.2 | AddEntriesFromIterable ( target , iterable , adder ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-add-entries-from-iterable) |
| 24.1.2 | Properties of the Map Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-map-constructor) |
| 24.1.2.1 | Map.groupBy ( items , callback ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.groupby) |
| 24.1.2.2 | Map.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype) |
| 24.1.2.3 | get Map [ %Symbol.species% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-map-%symbol.species%) |
| 24.1.3 | Properties of the Map Prototype Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-map-prototype-object) |
| 24.1.3.1 | Map.prototype.clear ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.clear) |
| 24.1.3.2 | Map.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.constructor) |
| 24.1.3.3 | Map.prototype.delete ( key ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.delete) |
| 24.1.3.4 | Map.prototype.entries ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.entries) |
| 24.1.3.5 | Map.prototype.forEach ( callback [ , thisArg ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.foreach) |
| 24.1.3.6 | Map.prototype.get ( key ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.get) |
| 24.1.3.7 | Map.prototype.getOrInsert ( key , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.getorinsert) |
| 24.1.3.8 | Map.prototype.getOrInsertComputed ( key , callback ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.getorinsertcomputed) |
| 24.1.3.9 | Map.prototype.has ( key ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.has) |
| 24.1.3.10 | Map.prototype.keys ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.keys) |
| 24.1.3.11 | Map.prototype.set ( key , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.set) |
| 24.1.3.12 | get Map.prototype.size | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-map.prototype.size) |
| 24.1.3.13 | Map.prototype.values ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype.values) |
| 24.1.3.14 | Map.prototype [ %Symbol.iterator% ] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype-%symbol.iterator%) |
| 24.1.3.15 | Map.prototype [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map.prototype-%symbol.tostringtag%) |
| 24.1.4 | Properties of Map Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-map-instances) |
| 24.1.5 | Map Iterator Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-map-iterator-objects) |
| 24.1.5.1 | CreateMapIterator ( map , kind ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-createmapiterator) |
| 24.1.5.2 | The %MapIteratorPrototype% Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%mapiteratorprototype%-object) |
| 24.1.5.2.1 | %MapIteratorPrototype%.next ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%mapiteratorprototype%.next) |
| 24.1.5.2.2 | %MapIteratorPrototype% [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%mapiteratorprototype%-%symbol.tostringtag%) |

## Support

Feature-level support tracking with test script references.

### 24.1.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype.clear))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.startsWith | Supported | [`String_StartsWith_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_StartsWith_Basic.js) | Reflection-based string dispatch routes CLR string receivers to JavaScriptRuntime.String.StartsWith with optional position argument. Returns a boolean value (boxed). |

### 24.1.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype.constructor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.includes | Supported |  | Reflection-based dispatch recognizes definite string receivers and routes to JavaScriptRuntime.String.Includes; supports optional position argument. Returns a boolean value. (No dedicated JS fixture currently referenced in this doc.) |

### 24.1.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype.delete))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.endsWith | Supported |  | Implemented in JavaScriptRuntime.String and wired via IL generator for definite string receivers. Supports optional end position. Returns a boolean value. (No dedicated JS fixture currently referenced in this doc.) |

### 24.1.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype.entries))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.split | Supported | [`String_Split_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_Split_Basic.js) | Supports string and regular-expression separators and optional limit. Implemented via JavaScriptRuntime.String.Split and returned as JavaScriptRuntime.Array. Separator omitted or undefined returns [input]. Empty string separator splits into individual UTF-16 code units. |

### 24.1.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-map.prototype.foreach))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.replace (regex literal, string replacement) | Partially Supported | [`String_Replace_Regex_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_Replace_Regex_Global.js) | Supported when the receiver is String(x), the pattern is a regular expression literal, and the replacement is a string. Global (g) and ignoreCase (i) flags are honored. Function replacement, non-regex patterns, and other flags are not yet implemented. Implemented via host intrinsic JavaScriptRuntime.String.Replace and dynamic resolution in IL generator. |

### 24.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-map-instances))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.localeCompare (numeric compare) | Supported | [`String_LocaleCompare_Numeric.js`](../../../Js2IL.Tests/String/JavaScript/String_LocaleCompare_Numeric.js) | Returns a number (boxed double) consistent with ECMAScript compare semantics; numeric option supported. |

