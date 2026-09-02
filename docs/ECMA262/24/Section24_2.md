<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 24.2: Set Objects

[Back to Section24](Section24.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-09-02T20:07:55Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 24.2 | Set Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 24.2.1 | Abstract Operations For Set Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-set-objects) |
| 24.2.1.1 | Set Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set-records) |
| 24.2.1.2 | GetSetRecord ( obj ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getsetrecord) |
| 24.2.1.3 | SetDataHas ( setData , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setdatahas) |
| 24.2.1.4 | SetDataIndex ( setData , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setdataindex) |
| 24.2.1.5 | SetDataSize ( setData ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setdatasize) |
| 24.2.2 | The Set Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set-constructor) |
| 24.2.2.1 | Set ( [ iterable ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set-iterable) |
| 24.2.3 | Properties of the Set Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-set-constructor) |
| 24.2.3.1 | Set.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype) |
| 24.2.3.2 | get Set [ %Symbol.species% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-set-%symbol.species%) |
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
| 24.2.4.19 | Set.prototype [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-set.prototype-%symbol.tostringtag%) |
| 24.2.5 | Properties of Set Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-set-instances) |
| 24.2.6 | Set Iterator Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set-iterator-objects) |
| 24.2.6.1 | CreateSetIterator ( set , kind ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createsetiterator) |
| 24.2.6.2 | The %SetIteratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%setiteratorprototype%-object) |
| 24.2.6.2.1 | %SetIteratorPrototype%.next ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%setiteratorprototype%.next) |
| 24.2.6.2.2 | %SetIteratorPrototype% [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-%setiteratorprototype%-%symbol.tostringtag%) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 24.2 ([tc39.es](https://tc39.es/ecma262/#sec-set-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Set conformance coverage | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Set/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/Symbol.species/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/Symbol.iterator/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/add/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/clear/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/constructor/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/delete/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/difference/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/entries/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/forEach/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/has/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/intersection/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/isDisjointFrom/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/isSubsetOf/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/isSupersetOf/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/size/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/symmetricDifference/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/union/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/values/SetConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/SetRuntimeSemanticsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/forEach/SetRuntimeSemanticsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/size/SetRuntimeSemanticsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/values/SetRuntimeSemanticsBatchExecutionTests.cs` |  | 279 additional pinned Test262 cases verify Set construction, species and iterator metadata, add, clear, delete, entries, forEach, has, size, values, and the Set composition and relation methods. Verified coverage includes insertion order, SameValueZero matching, live traversal after mutation, coercion, callback behavior, set-like operands, iterator protocol and closing behavior, descriptors, and supported error paths. Proxy, cross-realm, and remaining exotic cases remain limited. |

### 24.2.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-getsetrecord))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| GetSetRecord ( obj ) | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Set/prototype/union/ExecutionTests.cs` | `test/built-ins/Set/prototype/union/array-throws.js`<br>`test/built-ins/Set/prototype/union/size-is-a-number.js`<br>`test/built-ins/Set/prototype/union/has-is-callable.js`<br>`test/built-ins/Set/prototype/union/keys-is-callable.js` | Non-objects are rejected, size is read once and coerced with ToNumber so a NaN result (including an absent size) throws a TypeError and a BigInt or Symbol size throws from the coercion itself, a negative size throws a RangeError, and has and keys are each read once and must be callable. Abrupt completions from a size valueOf hook propagate and the hook is observed exactly once. |

### 24.2.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-setdatahas))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SetDataHas / SetDataIndex / SetDataSize | Supported | `tests/Jroc.Test262.Tests/built-ins/Set/prototype/intersection/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/isSupersetOf/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/forEach/SetRuntimeSemanticsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/values/SetRuntimeSemanticsBatchExecutionTests.cs` | `test/built-ins/Set/prototype/intersection/allows-set-like-object.js`<br>`test/built-ins/Set/prototype/isSupersetOf/allows-set-like-object.js`<br>`test/built-ins/Set/prototype/forEach/iterates-values-revisits-after-delete-re-add.js`<br>`test/built-ins/Set/prototype/values/values-iteration-mutable.js` | Membership, index lookup, and size are served by an insertion-ordered SetData list plus a live-value hash set. Deletion and clear preserve EMPTY slots, re-added values append at the end, active callbacks and iterators observe later additions in order, and completed iterators remain exhausted. Keys canonicalize -0 to +0 and treat NaN as a single value. |

### 24.2.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-set-iterable))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| new Set() | Supported | [`Set_Constructor_Prototype_Surface.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_Constructor_Prototype_Surface.js)<br>[`Require_Util_Types_Expanded.js`](../../../tests/Jroc.Tests/Node/Util/JavaScript/Require_Util_Types_Expanded.js) | `test/built-ins/Set/set-does-not-throw-when-add-is-not-callable.js`<br>`test/built-ins/Set/set-undefined-newtarget.js` | Parameterless construction succeeds and produces a JavaScriptRuntime.Set instance that other runtime services can recognize. |
| new Set(iterable) | Supported | [`Set_Constructor_Iterable.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_Constructor_Iterable.js)<br>`tests/Jroc.Test262.Tests/built-ins/Set/SetRuntimeSemanticsBatchExecutionTests.cs` | `test/built-ins/Set/set-iterator-close-after-add-failure.js` | JavaScriptRuntime.Set accepts a single iterable argument, consumes it with the runtime iterator protocol, observes constructor adder lookup/call-order semantics, preserves insertion order, ignores duplicates, and closes the iterator while preserving the original abrupt completion when the adder throws. |

### 24.2.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-set.prototype))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Set constructor value and Set.prototype surface | Supported with Limitations | [`Set_Constructor_Prototype_Surface.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_Constructor_Prototype_Surface.js) | `test/built-ins/Set/prototype-of-set.js` | JROC exposes globalThis.Set as a constructor value with test262-covered name/length/constructibility/global descriptor metadata, wires Set.prototype and Set.prototype.constructor, attaches the public prototype to new Set instances, and supports reflective checks such as Object.getPrototypeOf(set) === Set.prototype and set instanceof Set. Iterable construction and the core prototype surface are implemented; full SetIteratorPrototype metadata and the spec's broader set-like-object protocol remain incomplete. |

### 24.2.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-get-set-%symbol.species%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Set[Symbol.species] accessor | Supported | `tests/Jroc.Test262.Tests/built-ins/Set/Symbol.species/ExecutionTests.cs` | `test/built-ins/Set/Symbol.species/return-value.js` | Set now exposes @@species as a configurable, non-enumerable accessor with no setter. The getter returns the this value and shares the same covered metadata surface as the Map/Promise species getter. |

### 24.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-set-prototype-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Implemented Set members: add, has, size, clear, delete, entries, forEach, keys, values, @@iterator | Supported | [`Set_Core_Methods.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_Core_Methods.js)<br>[`Set_Entries_Keys_Values.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_Entries_Keys_Values.js)<br>[`Set_ForEach_Basic.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_ForEach_Basic.js)<br>[`Set_Symbol_Iterator.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_Symbol_Iterator.js)<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/add/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/delete/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/forEach/SetRuntimeSemanticsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/size/SetRuntimeSemanticsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/values/SetRuntimeSemanticsBatchExecutionTests.cs` | `test/built-ins/Set/prototype/add/add.js`<br>`test/built-ins/Set/prototype/add/this-not-object-throw-undefined.js`<br>`test/built-ins/Set/prototype/add/will-not-add-duplicate-entry.js`<br>`test/built-ins/Set/prototype/add/will-not-add-duplicate-entry-normalizes-zero.js`<br>`test/built-ins/Set/prototype/delete/delete.js`<br>`test/built-ins/Set/prototype/delete/delete-entry-initial-iterable.js`<br>`test/built-ins/Set/prototype/delete/delete-entry-normalizes-zero.js`<br>`test/built-ins/Set/prototype/delete/returns-true-when-delete-operation-occurs.js`<br>`test/built-ins/Set/prototype/delete/this-not-object-throw-undefined.js`<br>`test/built-ins/Set/prototype/add/name.js`<br>`test/built-ins/Set/prototype/clear/name.js`<br>`test/built-ins/Set/prototype/delete/name.js`<br>`test/built-ins/Set/prototype/entries/name.js`<br>`test/built-ins/Set/prototype/forEach/name.js`<br>`test/built-ins/Set/prototype/has/name.js`<br>`test/built-ins/Set/prototype/values/name.js`<br>`test/built-ins/Set/prototype/intersection/name.js`<br>`test/built-ins/Set/prototype/difference/name.js`<br>`test/built-ins/Set/prototype/union/name.js`<br>`test/built-ins/Set/prototype/isSubsetOf/name.js`<br>`test/built-ins/Set/prototype/isDisjointFrom/name.js`<br>`test/built-ins/Set/prototype/isSupersetOf/name.js`<br>`test/built-ins/Set/prototype/forEach/iterates-values-revisits-after-delete-re-add.js`<br>`test/built-ins/Set/prototype/forEach/length.js`<br>`test/built-ins/Set/prototype/size/name.js`<br>`test/built-ins/Set/prototype/values/values-iteration-mutable.js` | JavaScriptRuntime.Set exposes the core and ES2025 algebra methods on Set.prototype with standard JavaScript function name and length metadata, including the named size getter. Coverage exercises receiver checks, duplicate handling, insertion-order deletion and re-addition, live callback and iterator traversal, SameValueZero normalization, and set-like protocol behavior. |

### 24.2.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-set.prototype.clear))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Core Set prototype members (clear, delete, entries, forEach, keys, values, @@iterator) | Supported | [`Set_Core_Methods.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_Core_Methods.js)<br>[`Set_Entries_Keys_Values.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_Entries_Keys_Values.js)<br>[`Set_ForEach_Basic.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_ForEach_Basic.js)<br>[`Set_Symbol_Iterator.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_Symbol_Iterator.js)<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/forEach/SetRuntimeSemanticsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/size/SetRuntimeSemanticsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/values/SetRuntimeSemanticsBatchExecutionTests.cs` |  | These members are available on Set instances and use native iterator objects for keys/values/entries. SetData EMPTY slots preserve live traversal semantics across deletion, clear, and re-addition, while completed iterators remain exhausted. |

### 24.2.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-set.prototype.difference))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| New Set methods (difference, intersection, isDisjointFrom, isSubsetOf, isSupersetOf, symmetricDifference, union) | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Set/prototype/union/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/intersection/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/difference/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/symmetricDifference/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/isSubsetOf/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/isSupersetOf/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Set/prototype/isDisjointFrom/ExecutionTests.cs`<br>[`Set_Algebra_Methods.js`](../../../tests/Jroc.Tests/Set/JavaScript/Set_Algebra_Methods.js) | `test/built-ins/Set/prototype/union/allows-set-like-object.js`<br>`test/built-ins/Set/prototype/union/allows-set-like-class.js`<br>`test/built-ins/Set/prototype/union/array-throws.js`<br>`test/built-ins/Set/prototype/intersection/allows-set-like-object.js`<br>`test/built-ins/Set/prototype/difference/allows-set-like-object.js`<br>`test/built-ins/Set/prototype/symmetricDifference/allows-set-like-object.js`<br>`test/built-ins/Set/prototype/isSubsetOf/allows-set-like-object.js`<br>`test/built-ins/Set/prototype/isSupersetOf/allows-set-like-object.js`<br>`test/built-ins/Set/prototype/isDisjointFrom/allows-set-like-object.js` | The ES2025 Set algebra methods take the set-like protocol path: the argument is validated with GetSetRecord and each method chooses between calling the argument's has method and draining its keys iterator based on the relative sizes, so has and keys are only observed where the spec requires it. Arrays and other plain iterables are rejected with a TypeError because they are not set-like. Results are built by copying the receiver's live set data directly rather than through Set.prototype.add. Proxy and remaining exotic set-like behavior are still limited. |

### 24.2.4.19 ([tc39.es](https://tc39.es/ecma262/#sec-set.prototype-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Set.prototype[@@toStringTag] | Supported | `tests/Jroc.Test262.Tests/built-ins/Set/prototype/Symbol.toStringTag/ExecutionTests.cs` | `test/built-ins/Set/prototype/Symbol.toStringTag.js`<br>`test/built-ins/Set/prototype/Symbol.toStringTag/property-descriptor.js` | Set.prototype exposes the configurable, non-enumerable, non-writable "Set" @@toStringTag data property. |

### 24.2.6.1 ([tc39.es](https://tc39.es/ecma262/#sec-createsetiterator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Set iteration in for-of and other runtime iterator consumers | Supported with Limitations |  |  | Set instances expose Symbol.iterator and the keys/values/entries methods return native iterator objects with a dedicated %SetIteratorPrototype%. |

### 24.2.6.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-%setiteratorprototype%-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| %SetIteratorPrototype%[@@toStringTag] | Supported | `tests/Jroc.Test262.Tests/built-ins/SetIteratorPrototype/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Object/prototype/toString/ExecutionTests.cs` | `test/built-ins/SetIteratorPrototype/Symbol.toStringTag.js`<br>`test/built-ins/Object/prototype/toString/symbol-tag-set-builtin.js` | Set iterator instances inherit from a dedicated %SetIteratorPrototype% with the configurable, non-enumerable, non-writable "Set Iterator" @@toStringTag data property. |

