<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 27.1: Iteration

[Back to Section27](Section27.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-29T04:51:56Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 27.1 | Iteration | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteration) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 27.1.1 | Common Iteration Interfaces | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-common-iteration-interfaces) |
| 27.1.1.1 | The Iterable Interface | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterable-interface) |
| 27.1.1.2 | The Iterator Interface | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator-interface) |
| 27.1.1.3 | The Async Iterable Interface | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asynciterable-interface) |
| 27.1.1.4 | The Async Iterator Interface | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asynciterator-interface) |
| 27.1.1.5 | The IteratorResult Interface | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratorresult-interface) |
| 27.1.2 | Iterator Helper Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator-helper-objects) |
| 27.1.2.1 | The %IteratorHelperPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%-object) |
| 27.1.2.1.1 | %IteratorHelperPrototype%.next ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%.next) |
| 27.1.2.1.2 | %IteratorHelperPrototype%.return ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%.return) |
| 27.1.2.1.3 | %IteratorHelperPrototype% [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%-%symbol.tostringtag%) |
| 27.1.3 | Iterator Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator-objects) |
| 27.1.3.1 | The Iterator Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator-constructor) |
| 27.1.3.1.1 | Iterator ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator) |
| 27.1.3.2 | Properties of the Iterator Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-iterator-constructor) |
| 27.1.3.2.1 | Iterator.concat ( ... items ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.concat) |
| 27.1.3.2.2 | Iterator.from ( O ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.from) |
| 27.1.3.2.2.1 | The %WrapForValidIteratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%wrapforvaliditeratorprototype%-object) |
| 27.1.3.2.2.1.1 | %WrapForValidIteratorPrototype%.next ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%wrapforvaliditeratorprototype%.next) |
| 27.1.3.2.2.1.2 | %WrapForValidIteratorPrototype%.return ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%wrapforvaliditeratorprototype%.return) |
| 27.1.3.2.3 | Iterator.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype) |
| 27.1.3.3 | Properties of the Iterator Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%iterator.prototype%-object) |
| 27.1.3.3.1 | Iterator.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.constructor) |
| 27.1.3.3.1.1 | get Iterator.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-iterator.prototype.constructor) |
| 27.1.3.3.1.2 | set Iterator.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set-iterator.prototype.constructor) |
| 27.1.3.3.2 | Iterator.prototype.drop ( limit ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.drop) |
| 27.1.3.3.3 | Iterator.prototype.every ( predicate ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.every) |
| 27.1.3.3.4 | Iterator.prototype.filter ( predicate ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.filter) |
| 27.1.3.3.5 | Iterator.prototype.find ( predicate ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.find) |
| 27.1.3.3.6 | Iterator.prototype.flatMap ( mapper ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.flatmap) |
| 27.1.3.3.7 | Iterator.prototype.forEach ( procedure ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.foreach) |
| 27.1.3.3.8 | Iterator.prototype.map ( mapper ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.map) |
| 27.1.3.3.9 | Iterator.prototype.reduce ( reducer [ , initialValue ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.reduce) |
| 27.1.3.3.10 | Iterator.prototype.some ( predicate ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.some) |
| 27.1.3.3.11 | Iterator.prototype.take ( limit ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.take) |
| 27.1.3.3.12 | Iterator.prototype.toArray ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.toarray) |
| 27.1.3.3.13 | Iterator.prototype [ %Symbol.iterator% ] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype-%symbol.iterator%) |
| 27.1.3.3.14 | Iterator.prototype [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype-%symbol.tostringtag%) |
| 27.1.3.3.14.1 | get Iterator.prototype [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-iterator.prototype-%symbol.tostringtag%) |
| 27.1.3.3.14.2 | set Iterator.prototype [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set-iterator.prototype-%symbol.tostringtag%) |
| 27.1.4 | The %AsyncIteratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asynciteratorprototype) |
| 27.1.4.1 | %AsyncIteratorPrototype% [ %Symbol.asyncIterator% ] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%asynciteratorprototype%-%symbol.asynciterator%) |
| 27.1.5 | Async-from-Sync Iterator Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-from-sync-iterator-objects) |
| 27.1.5.1 | CreateAsyncFromSyncIterator ( syncIteratorRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createasyncfromsynciterator) |
| 27.1.5.2 | The %AsyncFromSyncIteratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%asyncfromsynciteratorprototype%-object) |
| 27.1.5.2.1 | %AsyncFromSyncIteratorPrototype%.next ( [ value ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%asyncfromsynciteratorprototype%.next) |
| 27.1.5.2.2 | %AsyncFromSyncIteratorPrototype%.return ( [ value ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%asyncfromsynciteratorprototype%.return) |
| 27.1.5.2.3 | %AsyncFromSyncIteratorPrototype%.throw ( [ value ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%asyncfromsynciteratorprototype%.throw) |
| 27.1.5.3 | Properties of Async-from-Sync Iterator Instances | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-async-from-sync-iterator-instances) |
| 27.1.5.4 | AsyncFromSyncIteratorContinuation ( result , promiseCapability , syncIteratorRecord , closeOnRejection ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncfromsynciteratorcontinuation) |

## Support

Feature-level support tracking with test script references.

### 27.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-common-iteration-interfaces))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| for..of consumes iterables via Symbol.iterator and performs IteratorClose on break/throw | Supported with Limitations | [`ControlFlow_ForOf_Array_Basic.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Array_Basic.js)<br>[`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js)<br>[`ControlFlow_ForOf_Let_PerIterationBinding.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Let_PerIterationBinding.js)<br>[`ControlFlow_ForOf_Let_Destructuring_PerIterationBinding.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Let_Destructuring_PerIterationBinding.js)<br>[`ControlFlow_ForOf_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledBreak.js)<br>[`ControlFlow_ForOf_LabeledContinue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledContinue.js) | Runtime implements iterator protocol consumption for `for..of` over arrays and user-defined iterables via `obj[Symbol.iterator]()`. IteratorClose is implemented: if an iterator has a callable `return`, it is invoked on break/throw paths. |

### 27.1.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-asynciterable-interface))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| for await..of consumes async iterables via Symbol.asyncIterator and falls back to Symbol.iterator (async-from-sync wrapper) | Supported with Limitations | [`Async_ForAwaitOf_Array.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_Array.js)<br>[`Async_ForAwaitOf_AsyncIterator_BreakCloses.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_AsyncIterator_BreakCloses.js)<br>[`Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js) | Runtime implements async iterator protocol consumption for `for await..of`. When `Symbol.asyncIterator` is missing, it wraps the sync iterator (CreateAsyncFromSyncIterator semantics) and ensures `return()` is invoked on early-exit paths. |

### 27.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-iterator-helper-objects))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Iterator helper objects support lazy map/filter/drop/take/flatMap pipelines plus helper next/return behavior | Supported with Limitations | [`Iterator_From_HelperChain.js`](../../../tests/Js2IL.Tests/Iterator/JavaScript/Iterator_From_HelperChain.js)<br>[`Iterator_Helper_Next_Return.js`](../../../tests/Js2IL.Tests/Iterator/JavaScript/Iterator_Helper_Next_Return.js) | Helper objects are lazy, iterable, and expose `next()` / `return()` through the public iterator-helper surface. Limit/count coercion and constructor metadata follow the new helper API shape, but only the synchronous helper family is implemented here. |

### 27.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-iterator-objects))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Public Iterator surface exposes Iterator.from and the helper methods for ordinary synchronous iterables | Supported with Limitations | [`Iterator_From_HelperChain.js`](../../../tests/Js2IL.Tests/Iterator/JavaScript/Iterator_From_HelperChain.js)<br>[`Iterator_Helper_Next_Return.js`](../../../tests/Js2IL.Tests/Iterator/JavaScript/Iterator_Helper_Next_Return.js) | JS2IL now exposes global `Iterator` and `Iterator.prototype` surfaces, including `Iterator.from(...)`, helper chaining, terminal helpers, and iterable array-iterator objects. `Iterator.concat` and full abstract-constructor semantics are still not implemented. |

### 27.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-asynciteratorprototype))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Public AsyncIterator surface is exposed with %Symbol.asyncIterator% for runtime async iterators | Supported with Limitations | [`Iterator_Helper_Next_Return.js`](../../../tests/Js2IL.Tests/Iterator/JavaScript/Iterator_Helper_Next_Return.js)<br>[`Async_ForAwaitOf_AsyncIterator_BreakCloses.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_AsyncIterator_BreakCloses.js)<br>[`Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js) | Global `AsyncIterator` and `%AsyncIteratorPrototype%[@@asyncIterator]` are exposed, and runtime async iterators inherit that public surface. Async iterator helper methods beyond the prototype exposure are still not implemented. |

