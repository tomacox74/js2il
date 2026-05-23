<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 23.1: Array Objects

[Back to Section23](Section23.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-23T13:57:48Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 23.1 | Array Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 23.1.1 | The Array Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array-constructor) |
| 23.1.1.1 | Array ( ... values ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array) |
| 23.1.2 | Properties of the Array Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-array-constructor) |
| 23.1.2.1 | Array.from ( items [ , mapper [ , thisArg ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.from) |
| 23.1.2.2 | Array.fromAsync ( items [ , mapper [ , thisArg ] ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.fromasync) |
| 23.1.2.3 | Array.isArray ( arg ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.isarray) |
| 23.1.2.4 | Array.of ( ... items ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.of) |
| 23.1.2.5 | Array.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype) |
| 23.1.2.6 | get Array [ %Symbol.species% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-array-%symbol.species%) |
| 23.1.3 | Properties of the Array Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-array-prototype-object) |
| 23.1.3.1 | Array.prototype.at ( index ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.at) |
| 23.1.3.2 | Array.prototype.concat ( ... items ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.concat) |
| 23.1.3.2.1 | IsConcatSpreadable ( O ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isconcatspreadable) |
| 23.1.3.3 | Array.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.constructor) |
| 23.1.3.4 | Array.prototype.copyWithin ( target , start [ , end ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.copywithin) |
| 23.1.3.5 | Array.prototype.entries ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.entries) |
| 23.1.3.6 | Array.prototype.every ( callback [ , thisArg ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.every) |
| 23.1.3.7 | Array.prototype.fill ( value [ , start [ , end ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.fill) |
| 23.1.3.8 | Array.prototype.filter ( callback [ , thisArg ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.filter) |
| 23.1.3.9 | Array.prototype.find ( predicate [ , thisArg ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.find) |
| 23.1.3.10 | Array.prototype.findIndex ( predicate [ , thisArg ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.findindex) |
| 23.1.3.11 | Array.prototype.findLast ( predicate [ , thisArg ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.findlast) |
| 23.1.3.12 | Array.prototype.findLastIndex ( predicate [ , thisArg ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.findlastindex) |
| 23.1.3.12.1 | FindViaPredicate ( O , len , direction , predicate , thisArg ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-findviapredicate) |
| 23.1.3.13 | Array.prototype.flat ( [ depth ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.flat) |
| 23.1.3.13.1 | FlattenIntoArray ( target , source , sourceLen , start , depth [ , mapperFunction [ , thisArg ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-flattenintoarray) |
| 23.1.3.14 | Array.prototype.flatMap ( mapperFunction [ , thisArg ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.flatmap) |
| 23.1.3.15 | Array.prototype.forEach ( callback [ , thisArg ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.foreach) |
| 23.1.3.16 | Array.prototype.includes ( searchElement [ , fromIndex ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.includes) |
| 23.1.3.17 | Array.prototype.indexOf ( searchElement [ , fromIndex ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.indexof) |
| 23.1.3.18 | Array.prototype.join ( separator ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.join) |
| 23.1.3.19 | Array.prototype.keys ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.keys) |
| 23.1.3.20 | Array.prototype.lastIndexOf ( searchElement [ , fromIndex ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.lastindexof) |
| 23.1.3.21 | Array.prototype.map ( callback [ , thisArg ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.map) |
| 23.1.3.22 | Array.prototype.pop ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.pop) |
| 23.1.3.23 | Array.prototype.push ( ... items ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.push) |
| 23.1.3.24 | Array.prototype.reduce ( callback [ , initialValue ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.reduce) |
| 23.1.3.25 | Array.prototype.reduceRight ( callback [ , initialValue ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.reduceright) |
| 23.1.3.26 | Array.prototype.reverse ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.reverse) |
| 23.1.3.27 | Array.prototype.shift ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.shift) |
| 23.1.3.28 | Array.prototype.slice ( start , end ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.slice) |
| 23.1.3.29 | Array.prototype.some ( callback [ , thisArg ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.some) |
| 23.1.3.30 | Array.prototype.sort ( comparator ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.sort) |
| 23.1.3.30.1 | SortIndexedProperties ( obj , len , SortCompare , holes ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-sortindexedproperties) |
| 23.1.3.30.2 | CompareArrayElements ( x , y , comparator ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-comparearrayelements) |
| 23.1.3.31 | Array.prototype.splice ( start , deleteCount , ... items ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.splice) |
| 23.1.3.32 | Array.prototype.toLocaleString ( [ reserved1 [ , reserved2 ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.tolocalestring) |
| 23.1.3.33 | Array.prototype.toReversed ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.toreversed) |
| 23.1.3.34 | Array.prototype.toSorted ( comparator ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.tosorted) |
| 23.1.3.35 | Array.prototype.toSpliced ( start , skipCount , ... items ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.tospliced) |
| 23.1.3.36 | Array.prototype.toString ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.tostring) |
| 23.1.3.37 | Array.prototype.unshift ( ... items ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.unshift) |
| 23.1.3.38 | Array.prototype.values ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.values) |
| 23.1.3.39 | Array.prototype.with ( index , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.with) |
| 23.1.3.40 | Array.prototype [ %Symbol.iterator% ] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype-%symbol.iterator%) |
| 23.1.3.41 | Array.prototype [ %Symbol.unscopables% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype-%symbol.unscopables%) |
| 23.1.4 | Properties of Array Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-array-instances) |
| 23.1.4.1 | length | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-array-instances-length) |
| 23.1.5 | Array Iterator Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array-iterator-objects) |
| 23.1.5.1 | CreateArrayIterator ( array , kind ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createarrayiterator) |
| 23.1.5.2 | The %ArrayIteratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%arrayiteratorprototype%-object) |
| 23.1.5.2.1 | %ArrayIteratorPrototype%.next ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%arrayiteratorprototype%.next) |
| 23.1.5.2.2 | %ArrayIteratorPrototype% [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%arrayiteratorprototype%-%symbol.tostringtag%) |
| 23.1.5.3 | Properties of Array Iterator Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-array-iterator-instances) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 23.1.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-array))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array constructor/call forms (empty, length, variadic) | Supported with Limitations | [`Array_Callable_Construct.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Callable_Construct.js)<br>[`Array_New_Empty.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_New_Empty.js)<br>[`Array_New_Length.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_New_Length.js)<br>[`Array_New_MultipleArgs.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_New_MultipleArgs.js) |  | Implements Array() and new Array(...) including numeric length mode; range and integer checks are implemented with runtime-backed limits. |

### 23.1.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-array.from))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.from (items) | Supported with Limitations | [`Array_Static_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Static_Basic.js)<br>[`Array_Iterator_Methods.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Iterator_Methods.js)<br>[`calling-from-valid-1-noStrict.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/from/JavaScript/calling-from-valid-1-noStrict.js)<br>[`Array.from-name.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/from/JavaScript/Array.from-name.js)<br>[`Array.from_arity.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/from/JavaScript/Array.from_arity.js) | `test/built-ins/Array/from/calling-from-valid-1-noStrict.js`<br>`test/built-ins/Array/from/Array.from-name.js`<br>`test/built-ins/Array/from/Array.from_arity.js` | Supports array, enumerable, and iterator-producing sources (including Array iterator methods) and exposes the standard Array.from name/length metadata; mapper and thisArg semantics are not fully modeled. |

### 23.1.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-array.fromasync))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.fromAsync | Not Yet Supported |  |  | Async iterator collection and async mapping pipeline are not implemented. |

### 23.1.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-array.isarray))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.isArray | Supported | [`Array_IsArray_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_IsArray_Basic.js)<br>[`15.4.3.2-0-1.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/isArray/JavaScript/15.4.3.2-0-1.js)<br>[`15.4.3.2-1-1.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/isArray/JavaScript/15.4.3.2-1-1.js)<br>[`15.4.3.2-0-5.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/isArray/JavaScript/15.4.3.2-0-5.js) | `test/built-ins/Array/isArray/15.4.3.2-0-1.js`<br>`test/built-ins/Array/isArray/15.4.3.2-1-1.js`<br>`test/built-ins/Array/isArray/15.4.3.2-0-5.js` | Returns true for JavaScriptRuntime.Array instances and the intrinsic Array.prototype object, and false otherwise. |

### 23.1.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-array.of))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.of | Supported | [`Array_Static_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Static_Basic.js) |  | Creates arrays from argument lists with expected element ordering. |

### 23.1.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.concat))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.concat with Symbol.isConcatSpreadable | Supported with Limitations | [`Array_NonMutatingOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_NonMutatingOps_Basic.js)<br>[`Array_Concat_SymbolIsConcatSpreadable.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Concat_SymbolIsConcatSpreadable.js) |  | Supports concatenating arrays, primitive values, and array-like objects that opt in or out via Symbol.isConcatSpreadable. Species, sparse-hole fidelity, and broader exotic receiver behavior remain limited. |

### 23.1.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.entries))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.entries / keys / values / [Symbol.iterator] | Supported with Limitations | [`Array_Iterator_Methods.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Iterator_Methods.js)<br>[`Array.prototype.entries.js`](../../../tests/Js2IL.Test262.Tests/language/statements/for-of/JavaScript/Array.prototype.entries.js)<br>[`Array.prototype.keys.js`](../../../tests/Js2IL.Test262.Tests/language/statements/for-of/JavaScript/Array.prototype.keys.js)<br>[`Array.prototype.Symbol.iterator.js`](../../../tests/Js2IL.Test262.Tests/language/statements/for-of/JavaScript/Array.prototype.Symbol.iterator.js) | `test/language/statements/for-of/Array.prototype.entries.js`<br>`test/language/statements/for-of/Array.prototype.keys.js`<br>`test/language/statements/for-of/Array.prototype.Symbol.iterator.js` | Exposes Array iterator methods plus Array.prototype[Symbol.iterator] as the values alias, supports borrowed array-like calls, and routes for..of / spread through the same iterator surface. %ArrayIteratorPrototype% metadata (for example %Symbol.toStringTag%) and broader exotic edge behavior remain limited. |

### 23.1.3.6 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.every))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.every ( callback [ , thisArg ] ) | Supported with Limitations | [`Array_CallbackOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_CallbackOps_Basic.js)<br>[`15.4.4.16-0-1.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/prototype/every/JavaScript/15.4.4.16-0-1.js)<br>[`15.4.4.16-1-10.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/prototype/every/JavaScript/15.4.4.16-1-10.js) | `test/built-ins/Array/prototype/every/15.4.4.16-0-1.js`<br>`test/built-ins/Array/prototype/every/15.4.4.16-1-10.js`<br>`test/built-ins/Array/prototype/every/15.4.4.16-1-1.js`<br>`test/built-ins/Array/prototype/every/15.4.4.16-1-2.js`<br>`test/built-ins/Array/prototype/every/15.4.4.16-1-5.js`<br>`test/built-ins/Array/prototype/every/15.4.4.16-1-15.js` | Supports core predicate iteration for ordinary arrays. Current bounded test262 coverage also exercises sparse arrays, array-like receivers, and representative callback/length edge cases; species and broader exotic receiver behavior remain limited. |

### 23.1.3.8 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.filter))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.filter ( callback [ , thisArg ] ) | Supported with Limitations | [`Array_CallbackOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_CallbackOps_Basic.js)<br>[`15.4.4.20-1-1.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/prototype/filter/JavaScript/15.4.4.20-1-1.js)<br>[`15.4.4.20-10-1.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/prototype/filter/JavaScript/15.4.4.20-10-1.js) | `test/built-ins/Array/prototype/filter/15.4.4.20-1-1.js`<br>`test/built-ins/Array/prototype/filter/15.4.4.20-10-1.js`<br>`test/built-ins/Array/prototype/filter/15.4.4.20-1-3.js`<br>`test/built-ins/Array/prototype/filter/15.4.4.20-1-6.js`<br>`test/built-ins/Array/prototype/filter/15.4.4.20-1-10.js`<br>`test/built-ins/Array/prototype/filter/15.4.4.20-1-12.js` | Supports callback filtering for ordinary arrays. Current bounded test262 coverage also exercises sparse arrays, array-like receivers, and representative callback/length edge cases; species and broader exotic receiver behavior remain limited. |

### 23.1.3.14 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.flatmap))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.flat / flatMap | Supported with Limitations | [`Array_NonMutatingOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_NonMutatingOps_Basic.js)<br>[`Array_CallbackOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_CallbackOps_Basic.js) |  | Supports core flattening and map+flatten workflows for dense arrays; full species/holes/exotic edge behavior is not exhaustive. |

### 23.1.3.16 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.includes))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.includes | Supported with Limitations | [`Array_SearchOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_SearchOps_Basic.js) |  | Implements SameValueZero-style comparisons for common runtime values; sparse-array edge behavior remains limited. |

### 23.1.3.17 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.indexof))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.indexOf / lastIndexOf | Supported with Limitations | [`Array_SearchOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_SearchOps_Basic.js)<br>[`Array_PrototypeMethods_ArrayLike_Call.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_PrototypeMethods_ArrayLike_Call.js)<br>[`Array_PrototypeMethods_ArrayLike_EdgeCases.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_PrototypeMethods_ArrayLike_EdgeCases.js) |  | Supports standard array usage and array-like call patterns, including fromIndex edge cases; full sparse/exotic behavior is limited. |

### 23.1.3.18 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.join))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.join / toString / toLocaleString | Supported with Limitations | [`Array_Join_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Join_Basic.js)<br>[`Array_Stringification_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Stringification_Basic.js) |  | join and stringification are implemented; toLocaleString currently aliases basic stringification behavior. |

### 23.1.3.21 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.map))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.map ( callback [ , thisArg ] ) | Supported with Limitations | [`Array_CallbackOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_CallbackOps_Basic.js)<br>[`15.4.4.19-1-1.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/prototype/map/JavaScript/15.4.4.19-1-1.js)<br>[`15.4.4.19-2-1.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/prototype/map/JavaScript/15.4.4.19-2-1.js) | `test/built-ins/Array/prototype/map/15.4.4.19-1-1.js`<br>`test/built-ins/Array/prototype/map/15.4.4.19-2-1.js`<br>`test/built-ins/Array/prototype/map/15.4.4.19-1-3.js`<br>`test/built-ins/Array/prototype/map/15.4.4.19-1-6.js`<br>`test/built-ins/Array/prototype/map/15.4.4.19-1-10.js`<br>`test/built-ins/Array/prototype/map/15.4.4.19-1-12.js` | Supports callback mapping for ordinary arrays. Current bounded test262 coverage also exercises sparse arrays, array-like receivers, and representative callback/length edge cases; species and broader exotic receiver behavior remain limited. |

### 23.1.3.24 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.reduce))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.reduce / reduceRight | Supported with Limitations | [`Array_CallbackOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_CallbackOps_Basic.js)<br>[`Array_PrototypeMethods_ArrayLike_Call.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_PrototypeMethods_ArrayLike_Call.js)<br>[`Array_PrototypeMethods_ArrayLike_EdgeCases.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_PrototypeMethods_ArrayLike_EdgeCases.js)<br>[`15.4.4.21-1-1.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/prototype/reduce/JavaScript/15.4.4.21-1-1.js)<br>[`15.4.4.21-10-1.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/prototype/reduce/JavaScript/15.4.4.21-10-1.js)<br>[`15.4.4.21-10-3.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/prototype/reduce/JavaScript/15.4.4.21-10-3.js)<br>[`15.4.4.21-10-4.js`](../../../tests/Js2IL.Test262.Tests/built-ins/Array/prototype/reduce/JavaScript/15.4.4.21-10-4.js) | `test/built-ins/Array/prototype/reduce/15.4.4.21-1-1.js`<br>`test/built-ins/Array/prototype/reduce/15.4.4.21-10-1.js`<br>`test/built-ins/Array/prototype/reduce/15.4.4.21-10-3.js`<br>`test/built-ins/Array/prototype/reduce/15.4.4.21-10-4.js`<br>`test/built-ins/Array/prototype/reduce/15.4.4.21-1-3.js`<br>`test/built-ins/Array/prototype/reduce/15.4.4.21-1-6.js`<br>`test/built-ins/Array/prototype/reduce/15.4.4.21-1-10.js`<br>`test/built-ins/Array/prototype/reduce/15.4.4.21-1-12.js` | Supports core reduce flows and array-like .call(...) usage. Current bounded test262 coverage also exercises sparse arrays, array-like receivers, inherited array indexed elements, and representative accumulator-initialization edge cases; broader exotic behavior remains limited. |

### 23.1.3.30 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.sort))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.sort / toSorted | Supported with Limitations | [`Array_Sort_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Sort_Basic.js)<br>[`Array_Sort_WithComparatorArrow.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Sort_WithComparatorArrow.js)<br>[`Array_NonMutatingOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_NonMutatingOps_Basic.js) |  | Supports default and callback comparator paths; full spec stability/exotic-object details are not fully modeled. |

### 23.1.3.31 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.splice))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.splice / toSpliced | Supported with Limitations | [`Array_Splice_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Splice_Basic.js)<br>[`Array_Splice_InsertAndDelete.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Splice_InsertAndDelete.js)<br>[`Array_NonMutatingOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_NonMutatingOps_Basic.js) |  | Core insertion/deletion semantics are implemented; complete exotic length/property descriptor interactions are not exhaustive. |

### 23.1.3.33 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.toreversed))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.toReversed | Supported | [`Array_NonMutatingOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_NonMutatingOps_Basic.js) |  | Returns a reversed copy while preserving original array. |

### 23.1.3.39 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.with))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array.prototype.with | Supported with Limitations | [`Array_NonMutatingOps_Basic.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_NonMutatingOps_Basic.js) |  | Returns a copied array with one replaced index; error and coercion behavior is a pragmatic subset. |

### 23.1.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-array-instances-length))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array length property (read/write) | Supported with Limitations | [`Array_LengthProperty_ReturnsCount.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_LengthProperty_ReturnsCount.js)<br>[`Array_EmptyLength_IsZero.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_EmptyLength_IsZero.js)<br>[`Array_Length_Set_Fractional_ThrowsRangeError.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Length_Set_Fractional_ThrowsRangeError.js) |  | Length reads and integer-validated writes are implemented; full array exotic descriptor semantics are not complete. |

### 23.1.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-createarrayiterator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| CreateArrayIterator used by for-of | Supported with Limitations | [`ControlFlow_ForOf_Array_Basic.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Array_Basic.js)<br>[`array.js`](../../../tests/Js2IL.Test262.Tests/language/statements/for-of/JavaScript/array.js)<br>[`array-expand.js`](../../../tests/Js2IL.Test262.Tests/language/statements/for-of/JavaScript/array-expand.js)<br>[`array-contract.js`](../../../tests/Js2IL.Test262.Tests/language/statements/for-of/JavaScript/array-contract.js) | `test/language/statements/for-of/array.js`<br>`test/language/statements/for-of/array-expand.js`<br>`test/language/statements/for-of/array-contract.js` | for-of over arrays uses the same runtime iterator objects exposed by Array.prototype[Symbol.iterator], values(), entries(), and keys(); %ArrayIteratorPrototype% metadata and broader exotic edge behavior remain limited. |

