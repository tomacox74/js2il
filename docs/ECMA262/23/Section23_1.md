<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 23.1: Array Objects

[Back to Section23](Section23.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 23.1 | Array Objects | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 23.1.1 | The Array Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array-constructor) |
| 23.1.1.1 | Array ( ... values ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array) |
| 23.1.2 | Properties of the Array Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-array-constructor) |
| 23.1.2.1 | Array.from ( items [ , mapper [ , thisArg ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.from) |
| 23.1.2.2 | Array.fromAsync ( items [ , mapper [ , thisArg ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.fromasync) |
| 23.1.2.3 | Array.isArray ( arg ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.isarray) |
| 23.1.2.4 | Array.of ( ... items ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.of) |
| 23.1.2.5 | Array.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype) |
| 23.1.2.6 | get Array [ %Symbol.species% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-array-%symbol.species%) |
| 23.1.3 | Properties of the Array Prototype Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-array-prototype-object) |
| 23.1.3.1 | Array.prototype.at ( index ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.at) |
| 23.1.3.2 | Array.prototype.concat ( ... items ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.concat) |
| 23.1.3.2.1 | IsConcatSpreadable ( O ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isconcatspreadable) |
| 23.1.3.3 | Array.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.constructor) |
| 23.1.3.4 | Array.prototype.copyWithin ( target , start [ , end ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.copywithin) |
| 23.1.3.5 | Array.prototype.entries ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.entries) |
| 23.1.3.6 | Array.prototype.every ( callback [ , thisArg ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.every) |
| 23.1.3.7 | Array.prototype.fill ( value [ , start [ , end ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.fill) |
| 23.1.3.8 | Array.prototype.filter ( callback [ , thisArg ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.filter) |
| 23.1.3.9 | Array.prototype.find ( predicate [ , thisArg ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.find) |
| 23.1.3.10 | Array.prototype.findIndex ( predicate [ , thisArg ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.findindex) |
| 23.1.3.11 | Array.prototype.findLast ( predicate [ , thisArg ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.findlast) |
| 23.1.3.12 | Array.prototype.findLastIndex ( predicate [ , thisArg ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.findlastindex) |
| 23.1.3.12.1 | FindViaPredicate ( O , len , direction , predicate , thisArg ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-findviapredicate) |
| 23.1.3.13 | Array.prototype.flat ( [ depth ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.flat) |
| 23.1.3.13.1 | FlattenIntoArray ( target , source , sourceLen , start , depth [ , mapperFunction [ , thisArg ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-flattenintoarray) |
| 23.1.3.14 | Array.prototype.flatMap ( mapperFunction [ , thisArg ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.flatmap) |
| 23.1.3.15 | Array.prototype.forEach ( callback [ , thisArg ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.foreach) |
| 23.1.3.16 | Array.prototype.includes ( searchElement [ , fromIndex ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.includes) |
| 23.1.3.17 | Array.prototype.indexOf ( searchElement [ , fromIndex ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.indexof) |
| 23.1.3.18 | Array.prototype.join ( separator ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.join) |
| 23.1.3.19 | Array.prototype.keys ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.keys) |
| 23.1.3.20 | Array.prototype.lastIndexOf ( searchElement [ , fromIndex ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.lastindexof) |
| 23.1.3.21 | Array.prototype.map ( callback [ , thisArg ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.map) |
| 23.1.3.22 | Array.prototype.pop ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.pop) |
| 23.1.3.23 | Array.prototype.push ( ... items ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.push) |
| 23.1.3.24 | Array.prototype.reduce ( callback [ , initialValue ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.reduce) |
| 23.1.3.25 | Array.prototype.reduceRight ( callback [ , initialValue ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.reduceright) |
| 23.1.3.26 | Array.prototype.reverse ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.reverse) |
| 23.1.3.27 | Array.prototype.shift ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.shift) |
| 23.1.3.28 | Array.prototype.slice ( start , end ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.slice) |
| 23.1.3.29 | Array.prototype.some ( callback [ , thisArg ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.some) |
| 23.1.3.30 | Array.prototype.sort ( comparator ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.sort) |
| 23.1.3.30.1 | SortIndexedProperties ( obj , len , SortCompare , holes ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-sortindexedproperties) |
| 23.1.3.30.2 | CompareArrayElements ( x , y , comparator ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-comparearrayelements) |
| 23.1.3.31 | Array.prototype.splice ( start , deleteCount , ... items ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.splice) |
| 23.1.3.32 | Array.prototype.toLocaleString ( [ reserved1 [ , reserved2 ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.tolocalestring) |
| 23.1.3.33 | Array.prototype.toReversed ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.toreversed) |
| 23.1.3.34 | Array.prototype.toSorted ( comparator ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.tosorted) |
| 23.1.3.35 | Array.prototype.toSpliced ( start , skipCount , ... items ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.tospliced) |
| 23.1.3.36 | Array.prototype.toString ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.tostring) |
| 23.1.3.37 | Array.prototype.unshift ( ... items ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.unshift) |
| 23.1.3.38 | Array.prototype.values ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.values) |
| 23.1.3.39 | Array.prototype.with ( index , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype.with) |
| 23.1.3.40 | Array.prototype [ %Symbol.iterator% ] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype-%symbol.iterator%) |
| 23.1.3.41 | Array.prototype [ %Symbol.unscopables% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array.prototype-%symbol.unscopables%) |
| 23.1.4 | Properties of Array Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-array-instances) |
| 23.1.4.1 | length | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-array-instances-length) |
| 23.1.5 | Array Iterator Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array-iterator-objects) |
| 23.1.5.1 | CreateArrayIterator ( array , kind ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-createarrayiterator) |
| 23.1.5.2 | The %ArrayIteratorPrototype% Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%arrayiteratorprototype%-object) |
| 23.1.5.2.1 | %ArrayIteratorPrototype%.next ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%arrayiteratorprototype%.next) |
| 23.1.5.2.2 | %ArrayIteratorPrototype% [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%arrayiteratorprototype%-%symbol.tostringtag%) |
| 23.1.5.3 | Properties of Array Iterator Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-array-iterator-instances) |

## Support

Feature-level support tracking with test script references.

### 23.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-array-constructor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array.isArray | Supported | [`Array_IsArray_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_IsArray_Basic.js) | Returns true for JavaScriptRuntime.Array instances; false otherwise. |

### 23.1.3.8 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.filter))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array.prototype.find | Supported | [`Array_Find_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Find_Basic.js) | Invokes the callback with (value, index, array) depending on the compiled delegate signature and returns the first matching element; returns undefined when no element matches. thisArg is currently ignored. |

### 23.1.3.13 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.flat))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array.prototype.join | Supported | [`Array_Join_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Join_Basic.js) | Elements are stringified via DotNet2JSConversions.ToString and joined with a separator (default ','). Codegen dispatches to JavaScriptRuntime.Array.join(object[]). |

### 23.1.3.20 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.lastindexof))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array.prototype.pop | Supported | [`Array_Pop_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Pop_Basic.js) | Removes and returns the last element; when empty returns undefined (represented as null in this runtime). |

### 23.1.3.22 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.pop))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array.prototype.push | Supported | [`Array_Push_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Push_Basic.js) | Appends items to the end of the array and returns the new length (as a JS number). |

### 23.1.3.25 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.reduceright))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array.prototype.map | Supported | [`Array_Map_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Map_Basic.js)<br>[`Array_Map_NestedParam.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Map_NestedParam.js) | Supports callback mapping including nested callback closures. Callback receives (value, index, array) depending on the compiled delegate signature. thisArg is currently ignored. Returns a new array. |

### 23.1.3.27 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.shift))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array.prototype.sort (default comparator and comparator function) | Supported | [`Array_Sort_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Sort_Basic.js)<br>[`Array_Sort_WithComparatorArrow.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Sort_WithComparatorArrow.js) | Default lexicographic sort implemented in JavaScriptRuntime.Array.sort(). Also supports an optional comparator callback (common delegate shapes produced by the compiler). Returns the array instance. |

### 23.1.3.28 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.slice))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array.prototype.slice | Supported | [`Array_Slice_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Slice_Basic.js)<br>[`Array_Slice_FromCharCode_Apply.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Slice_FromCharCode_Apply.js) | Returns a shallow copy; handles negative indices and undefined end per spec. |

### 23.1.3.29 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.some))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array.prototype.some | Supported | [`Array_Some_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Some_Basic.js) | Calls the predicate callback for each element until it returns a truthy value (then returns true); otherwise returns false. thisArg is accepted but currently ignored; implementation assumes dense arrays. |

### 23.1.3.31 ([tc39.es](https://tc39.es/ecma262/#sec-array.prototype.splice))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array.prototype.splice | Supported | [`Array_Splice_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Splice_Basic.js)<br>[`Array_Splice_InsertAndDelete.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Splice_InsertAndDelete.js) | Mutates the array by removing and/or inserting elements; returns an array of removed elements. |

### 23.1.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-array-instances-length))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array.length property (read) | Supported | [`Array_LengthProperty_ReturnsCount.js`](../../../Js2IL.Tests/Array/JavaScript/Array_LengthProperty_ReturnsCount.js)<br>[`Array_EmptyLength_IsZero.js`](../../../Js2IL.Tests/Array/JavaScript/Array_EmptyLength_IsZero.js) | length getter returns number of elements. Lowered via JavaScriptRuntime.Object.GetLength(object) and specialized to direct Array/Int32Array length intrinsics when the receiver type is proven. Used by for-of implementation. |
| Array.length property (write) | Supported with Limitations |  | Implemented for JavaScriptRuntime.Array: setting length truncates/extends the backing List (extending fills with undefined). This is a minimal subset intended to support common library patterns (e.g., buf.length = 0). Full spec behavior for non-writable length, sparse arrays, and array exotic objects is not implemented. |

