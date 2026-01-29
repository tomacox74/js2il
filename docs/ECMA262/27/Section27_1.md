<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 27.1: Iteration

[Back to Section27](Section27.md) | [Back to Index](../Index.md)

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
| 27.1.2 | Iterator Helper Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator-helper-objects) |
| 27.1.2.1 | The %IteratorHelperPrototype% Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%-object) |
| 27.1.2.1.1 | %IteratorHelperPrototype%.next ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%.next) |
| 27.1.2.1.2 | %IteratorHelperPrototype%.return ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%.return) |
| 27.1.2.1.3 | %IteratorHelperPrototype% [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%-%symbol.tostringtag%) |
| 27.1.3 | Iterator Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator-objects) |
| 27.1.3.1 | The Iterator Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator-constructor) |
| 27.1.3.1.1 | Iterator ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator) |
| 27.1.3.2 | Properties of the Iterator Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-iterator-constructor) |
| 27.1.3.2.1 | Iterator.concat ( ... items ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.concat) |
| 27.1.3.2.2 | Iterator.from ( O ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.from) |
| 27.1.3.2.2.1 | The %WrapForValidIteratorPrototype% Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%wrapforvaliditeratorprototype%-object) |
| 27.1.3.2.2.1.1 | %WrapForValidIteratorPrototype%.next ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%wrapforvaliditeratorprototype%.next) |
| 27.1.3.2.2.1.2 | %WrapForValidIteratorPrototype%.return ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%wrapforvaliditeratorprototype%.return) |
| 27.1.3.2.3 | Iterator.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype) |
| 27.1.3.3 | Properties of the Iterator Prototype Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%iterator.prototype%-object) |
| 27.1.3.3.1 | Iterator.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.constructor) |
| 27.1.3.3.1.1 | get Iterator.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-iterator.prototype.constructor) |
| 27.1.3.3.1.2 | set Iterator.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-set-iterator.prototype.constructor) |
| 27.1.3.3.2 | Iterator.prototype.drop ( limit ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.drop) |
| 27.1.3.3.3 | Iterator.prototype.every ( predicate ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.every) |
| 27.1.3.3.4 | Iterator.prototype.filter ( predicate ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.filter) |
| 27.1.3.3.5 | Iterator.prototype.find ( predicate ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.find) |
| 27.1.3.3.6 | Iterator.prototype.flatMap ( mapper ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.flatmap) |
| 27.1.3.3.7 | Iterator.prototype.forEach ( procedure ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.foreach) |
| 27.1.3.3.8 | Iterator.prototype.map ( mapper ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.map) |
| 27.1.3.3.9 | Iterator.prototype.reduce ( reducer [ , initialValue ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.reduce) |
| 27.1.3.3.10 | Iterator.prototype.some ( predicate ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.some) |
| 27.1.3.3.11 | Iterator.prototype.take ( limit ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.take) |
| 27.1.3.3.12 | Iterator.prototype.toArray ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.toarray) |
| 27.1.3.3.13 | Iterator.prototype [ %Symbol.iterator% ] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype-%symbol.iterator%) |
| 27.1.3.3.14 | Iterator.prototype [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype-%symbol.tostringtag%) |
| 27.1.3.3.14.1 | get Iterator.prototype [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-iterator.prototype-%symbol.tostringtag%) |
| 27.1.3.3.14.2 | set Iterator.prototype [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-set-iterator.prototype-%symbol.tostringtag%) |
| 27.1.4 | The %AsyncIteratorPrototype% Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asynciteratorprototype) |
| 27.1.4.1 | %AsyncIteratorPrototype% [ %Symbol.asyncIterator% ] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%asynciteratorprototype%-%symbol.asynciterator%) |
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
| for..of consumes iterables via Symbol.iterator and performs IteratorClose on break/throw | Supported with Limitations | [`ControlFlow_ForOf_Array_Basic.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Array_Basic.js)<br>[`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js)<br>[`ControlFlow_ForOf_Let_PerIterationBinding.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Let_PerIterationBinding.js)<br>[`ControlFlow_ForOf_Let_Destructuring_PerIterationBinding.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Let_Destructuring_PerIterationBinding.js)<br>[`ControlFlow_ForOf_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledBreak.js)<br>[`ControlFlow_ForOf_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledContinue.js) | Runtime implements iterator protocol consumption for `for..of` over arrays and user-defined iterables via `obj[Symbol.iterator]()`. IteratorClose is implemented: if an iterator has a callable `return`, it is invoked on break/throw paths. |

### 27.1.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-asynciterable-interface))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| for await..of consumes async iterables via Symbol.asyncIterator and falls back to Symbol.iterator (async-from-sync wrapper) | Supported with Limitations | [`Async_ForAwaitOf_Array.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_Array.js)<br>[`Async_ForAwaitOf_AsyncIterator_BreakCloses.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_AsyncIterator_BreakCloses.js)<br>[`Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js) | Runtime implements async iterator protocol consumption for `for await..of`. When `Symbol.asyncIterator` is missing, it wraps the sync iterator (CreateAsyncFromSyncIterator semantics) and ensures `return()` is invoked on early-exit paths. |

