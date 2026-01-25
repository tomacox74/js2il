<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 27.1: Iteration

[Back to Section27](Section27.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 27.1 | Iteration | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iteration) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 27.1.1 | Common Iteration Interfaces | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-common-iteration-interfaces) |
| 27.1.1.1 | The Iterable Interface | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterable-interface) |
| 27.1.1.2 | The Iterator Interface | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator-interface) |
| 27.1.1.3 | The Async Iterable Interface | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-asynciterable-interface) |
| 27.1.1.4 | The Async Iterator Interface | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-asynciterator-interface) |
| 27.1.1.5 | The IteratorResult Interface | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iteratorresult-interface) |
| 27.1.2 | Iterator Helper Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator-helper-objects) |
| 27.1.2.1 | The %IteratorHelperPrototype% Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%-object) |
| 27.1.2.1.1 | %IteratorHelperPrototype%.next ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%.next) |
| 27.1.2.1.2 | %IteratorHelperPrototype%.return ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%.return) |
| 27.1.2.1.3 | %IteratorHelperPrototype% [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%iteratorhelperprototype%-%symbol.tostringtag%) |
| 27.1.3 | Iterator Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator-objects) |
| 27.1.3.1 | The Iterator Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator-constructor) |
| 27.1.3.1.1 | Iterator ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator) |
| 27.1.3.2 | Properties of the Iterator Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-iterator-constructor) |
| 27.1.3.2.1 | Iterator.concat ( ... items ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.concat) |
| 27.1.3.2.2 | Iterator.from ( O ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.from) |
| 27.1.3.2.2.1 | The %WrapForValidIteratorPrototype% Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%wrapforvaliditeratorprototype%-object) |
| 27.1.3.2.2.1.1 | %WrapForValidIteratorPrototype%.next ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%wrapforvaliditeratorprototype%.next) |
| 27.1.3.2.2.1.2 | %WrapForValidIteratorPrototype%.return ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%wrapforvaliditeratorprototype%.return) |
| 27.1.3.2.3 | Iterator.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype) |
| 27.1.3.3 | Properties of the Iterator Prototype Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%iterator.prototype%-object) |
| 27.1.3.3.1 | Iterator.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.constructor) |
| 27.1.3.3.1.1 | get Iterator.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-iterator.prototype.constructor) |
| 27.1.3.3.1.2 | set Iterator.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-set-iterator.prototype.constructor) |
| 27.1.3.3.2 | Iterator.prototype.drop ( limit ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.drop) |
| 27.1.3.3.3 | Iterator.prototype.every ( predicate ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.every) |
| 27.1.3.3.4 | Iterator.prototype.filter ( predicate ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.filter) |
| 27.1.3.3.5 | Iterator.prototype.find ( predicate ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.find) |
| 27.1.3.3.6 | Iterator.prototype.flatMap ( mapper ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.flatmap) |
| 27.1.3.3.7 | Iterator.prototype.forEach ( procedure ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.foreach) |
| 27.1.3.3.8 | Iterator.prototype.map ( mapper ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.map) |
| 27.1.3.3.9 | Iterator.prototype.reduce ( reducer [ , initialValue ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.reduce) |
| 27.1.3.3.10 | Iterator.prototype.some ( predicate ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.some) |
| 27.1.3.3.11 | Iterator.prototype.take ( limit ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.take) |
| 27.1.3.3.12 | Iterator.prototype.toArray ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype.toarray) |
| 27.1.3.3.13 | Iterator.prototype [ %Symbol.iterator% ] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype-%symbol.iterator%) |
| 27.1.3.3.14 | Iterator.prototype [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iterator.prototype-%symbol.tostringtag%) |
| 27.1.3.3.14.1 | get Iterator.prototype [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-iterator.prototype-%symbol.tostringtag%) |
| 27.1.3.3.14.2 | set Iterator.prototype [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-set-iterator.prototype-%symbol.tostringtag%) |
| 27.1.4 | The %AsyncIteratorPrototype% Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-asynciteratorprototype) |
| 27.1.4.1 | %AsyncIteratorPrototype% [ %Symbol.asyncIterator% ] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%asynciteratorprototype%-%symbol.asynciterator%) |
| 27.1.5 | Async-from-Sync Iterator Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-async-from-sync-iterator-objects) |
| 27.1.5.1 | CreateAsyncFromSyncIterator ( syncIteratorRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-createasyncfromsynciterator) |
| 27.1.5.2 | The %AsyncFromSyncIteratorPrototype% Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%asyncfromsynciteratorprototype%-object) |
| 27.1.5.2.1 | %AsyncFromSyncIteratorPrototype%.next ( [ value ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%asyncfromsynciteratorprototype%.next) |
| 27.1.5.2.2 | %AsyncFromSyncIteratorPrototype%.return ( [ value ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%asyncfromsynciteratorprototype%.return) |
| 27.1.5.2.3 | %AsyncFromSyncIteratorPrototype%.throw ( [ value ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%asyncfromsynciteratorprototype%.throw) |
| 27.1.5.3 | Properties of Async-from-Sync Iterator Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-async-from-sync-iterator-instances) |
| 27.1.5.4 | AsyncFromSyncIteratorContinuation ( result , promiseCapability , syncIteratorRecord , closeOnRejection ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-asyncfromsynciteratorcontinuation) |

## Support

Feature-level support tracking with test script references.

### 27.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-common-iteration-interfaces))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Promise constructor (executor), Promise.resolve, Promise.reject | Supported | [`Promise_Executor_Resolved.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Executor_Resolved.js)<br>[`Promise_Executor_Rejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Executor_Rejected.js)<br>[`Promise_Thenable_Resolve_Immediate.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_Resolve_Immediate.js)<br>[`Promise_Thenable_Resolve_Delayed.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_Resolve_Delayed.js)<br>[`Promise_Thenable_Reject.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_Reject.js)<br>[`Promise_Thenable_NonFunctionThen.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_NonFunctionThen.js)<br>[`Promise_Thenable_Nested.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_Nested.js) | Constructor accepts an executor delegate and supports the basic resolve/reject fast-paths and dynamic delegate invocation used in tests. Promise.resolve/reject create already-settled Promise instances, including thenable assimilation (Promise.resolve adopts thenables, handles non-function then properties, and supports nested thenables). |
| Promise.withResolvers() | Supported | [`Promise_WithResolvers_Resolve.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_WithResolvers_Resolve.js)<br>[`Promise_WithResolvers_Reject.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_WithResolvers_Reject.js)<br>[`Promise_WithResolvers_Idempotent.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_WithResolvers_Idempotent.js) | Implements `Promise.withResolvers()` returning `{ promise, resolve, reject }` (resolve/reject are functions that settle the associated promise). |

### 27.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-iterator-helper-objects))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Promise.prototype.then / catch / finally | Supported | [`Promise_Resolve_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_Then.js)<br>[`Promise_Reject_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Reject_Then.js)<br>[`Promise_Resolve_ThenFinally.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_ThenFinally.js)<br>[`Promise_Reject_FinallyCatch.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Reject_FinallyCatch.js)<br>[`Promise_Resolve_FinallyThen.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_FinallyThen.js)<br>[`Promise_Resolve_FinallyThrows.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_FinallyThrows.js)<br>[`Promise_Then_ReturnsResolvedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Then_ReturnsResolvedPromise.js)<br>[`Promise_Then_ReturnsRejectedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Then_ReturnsRejectedPromise.js)<br>[`Promise_Thenable_Returned_FromHandler.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Thenable_Returned_FromHandler.js)<br>[`Promise_Catch_ReturnsResolvedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Catch_ReturnsResolvedPromise.js)<br>[`Promise_Catch_ReturnsRejectedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Catch_ReturnsRejectedPromise.js)<br>[`Promise_Finally_ReturnsResolvedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Finally_ReturnsResolvedPromise.js)<br>[`Promise_Finally_ReturnsRejectedPromise.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Finally_ReturnsRejectedPromise.js)<br>[`Promise_Finally_ReturnsThenable_PassThrough_Fulfilled.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Finally_ReturnsThenable_PassThrough_Fulfilled.js)<br>[`Promise_Finally_ReturnsThenable_PassThrough_Rejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Finally_ReturnsThenable_PassThrough_Rejected.js)<br>[`Promise_Scheduling_StarvationTest.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Scheduling_StarvationTest.js) | Implements `then`, `catch`, and `finally` with microtask scheduling support. Handlers support Promise/thenable return assimilation. `finally` handlers are treated as observers: non-Promise return values do not alter the settled result, while returned Promises/thenables are awaited and propagated (fixed earlier bug where Promise returns from finally were masked). Tests include chaining, thenable returns, and then/catch/finally interactions. |

### 27.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-iterator-objects))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Promise.all | Supported | [`Promise_All_AllResolved.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_All_AllResolved.js)<br>[`Promise_All_OneRejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_All_OneRejected.js)<br>[`Promise_All_EmptyArray.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_All_EmptyArray.js) | Returns a Promise that resolves when all input promises resolve (with an array of results), or rejects when any input promise rejects (with the first rejection reason). |
| Promise.allSettled | Supported | [`Promise_AllSettled_MixedResults.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_AllSettled_MixedResults.js)<br>[`Promise_AllSettled_AllResolved.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_AllSettled_AllResolved.js)<br>[`Promise_AllSettled_AllRejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_AllSettled_AllRejected.js) | Returns a Promise that resolves when all input promises have settled (fulfilled or rejected), with an array of outcome objects containing status and value/reason. |
| Promise.any | Supported | [`Promise_Any_FirstResolved.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Any_FirstResolved.js)<br>[`Promise_Any_AllRejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Any_AllRejected.js) | Returns a Promise that resolves as soon as any input promise resolves, or rejects with an AggregateError if all input promises reject. |
| Promise.race | Supported | [`Promise_Race_FirstResolved.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Race_FirstResolved.js)<br>[`Promise_Race_FirstRejected.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Race_FirstRejected.js) | Returns a Promise that settles as soon as any input promise settles (resolves or rejects), with the same value or reason. |

