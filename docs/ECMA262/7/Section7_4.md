<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 7.4: Operations on Iterator Objects

[Back to Section7](Section7.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-07T20:40:52Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 7.4 | Operations on Iterator Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-operations-on-iterator-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 7.4.1 | Iterator Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator-records) |
| 7.4.2 | GetIteratorDirect ( obj ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getiteratordirect) |
| 7.4.3 | GetIteratorFromMethod ( obj , method ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getiteratorfrommethod) |
| 7.4.4 | GetIterator ( obj , kind ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getiterator) |
| 7.4.5 | GetIteratorFlattenable ( obj , primitiveHandling ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getiteratorflattenable) |
| 7.4.6 | IteratorNext ( iteratorRecord [ , value ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratornext) |
| 7.4.7 | IteratorComplete ( iteratorResult ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratorcomplete) |
| 7.4.8 | IteratorValue ( iteratorResult ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratorvalue) |
| 7.4.9 | IteratorStep ( iteratorRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratorstep) |
| 7.4.10 | IteratorStepValue ( iteratorRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratorstepvalue) |
| 7.4.11 | IteratorClose ( iteratorRecord , completion ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratorclose) |
| 7.4.12 | IteratorCloseAll ( iters , completion ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratorcloseall) |
| 7.4.13 | IfAbruptCloseIterator ( value , iteratorRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ifabruptcloseiterator) |
| 7.4.14 | AsyncIteratorClose ( iteratorRecord , completion ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asynciteratorclose) |
| 7.4.15 | IfAbruptCloseAsyncIterator ( value , iteratorRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ifabruptcloseasynciterator) |
| 7.4.16 | CreateIteratorResultObject ( value , done ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createiterresultobject) |
| 7.4.17 | CreateListIteratorRecord ( list ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createlistiteratorRecord) |
| 7.4.18 | IteratorToList ( iteratorRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratortolist) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 7.4 ([tc39.es](https://tc39.es/ecma262/#sec-operations-on-iterator-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Iterator operations across sync iteration, async iteration fallback, helper adapters, abrupt-close behavior, and list materialization | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js)<br>[`Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js)<br>[`Iterator_Helper_Cleanup.js`](../../../tests/Jroc.Tests/Iterator/JavaScript/Iterator_Helper_Cleanup.js)<br>[`Iterator_From_HelperChain.js`](../../../tests/Jroc.Tests/Iterator/JavaScript/Iterator_From_HelperChain.js)<br>[`Object_FromEntries_Basic.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_FromEntries_Basic.js) |  | JROC implements the iterator abstract-operation surface used by current language features and shipped iterator helpers, including sync/async acquisition, step/value/done extraction, close-on-abrupt-completion paths, and iterator-to-list style materialization. Remaining gaps are mostly proposal-edge semantics and the fact that some helpers are modeled inline through concrete iterator adapters rather than as separately named general-purpose abstract-operation entry points. |

### 7.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-iterator-records))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Iterator record tracking through sync and async iterator adapters | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js)<br>[`Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js) |  | Iterator state is represented by concrete `IJavaScriptIterator` and `IJavaScriptAsyncIterator` adapters that preserve next/return capability and done/value flow for the covered sync and async lowering paths. The runtime does not expose a distinct public iterator-record data structure beyond those adapters. |

### 7.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-getiteratordirect))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| GetIteratorDirect for native iterator objects and iterator helpers | Supported with Limitations | [`Iterator_Helper_Next_Return.js`](../../../tests/Jroc.Tests/Iterator/JavaScript/Iterator_Helper_Next_Return.js)<br>[`Iterator_From_HelperChain.js`](../../../tests/Jroc.Tests/Iterator/JavaScript/Iterator_From_HelperChain.js) |  | Native iterator objects and iterator-helper receivers can be consumed directly without re-entering `@@iterator`, matching the runtime's direct `IJavaScriptIterator` fast path. General coverage is anchored to the shipped iterator-helper surface rather than a standalone public helper. |

### 7.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-getiteratorfrommethod))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| GetIteratorFromMethod for Symbol.iterator-based sources | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js)<br>[`Function_Call_Spread_StringIterable.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_StringIterable.js) | `test/language/expressions/yield/star-iterable.js` | The runtime calls user-provided `@@iterator` methods with the correct receiver across for-of, spread, and `yield*` paths for the covered iterable shapes. Unsupported or still-partial exotic iterator method scenarios remain documented as general iterator limitations rather than as a missing core path. |

### 7.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-getiterator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| GetIterator for for..of sources | Supported with Limitations | [`ControlFlow_ForOf_Array_Basic.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Array_Basic.js)<br>[`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js) |  | Supports built-in iterables, user-defined Symbol.iterator sources, and IEnumerable fallback paths. |

### 7.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-getiteratorflattenable))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| GetIteratorFlattenable for Iterator.from and flatMap-style iterable normalization | Supported with Limitations | [`Iterator_From_HelperChain.js`](../../../tests/Jroc.Tests/Iterator/JavaScript/Iterator_From_HelperChain.js)<br>[`Iterator_Helper_Cleanup.js`](../../../tests/Jroc.Tests/Iterator/JavaScript/Iterator_Helper_Cleanup.js) |  | Iterator helpers normalize iterable and iterator-like mapper results through `Iterator.from(...)`, which covers the current flattenable acquisition behavior used by the shipped helper surface. The full `primitiveHandling` matrix and every proposal-edge flattening scenario are not yet modeled as a separate standalone helper. |

### 7.4.6 ([tc39.es](https://tc39.es/ecma262/#sec-iteratornext))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IteratorNext | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js)<br>[`Generator_BasicNext.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_BasicNext.js) |  | Iterator advancement is implemented for native and dynamic iterator objects used by lowering/runtime helpers. |

### 7.4.7 ([tc39.es](https://tc39.es/ecma262/#sec-iteratorcomplete))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IteratorComplete | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js) |  | Done-state checks are implemented through IteratorResultDone in iteration paths. |

### 7.4.8 ([tc39.es](https://tc39.es/ecma262/#sec-iteratorvalue))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IteratorValue | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js) |  | Value extraction is implemented through IteratorResultValue for native and dynamic iterator result shapes. |

### 7.4.9 ([tc39.es](https://tc39.es/ecma262/#sec-iteratorstep))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IteratorStep across for-of, destructuring, and iterator helpers | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js)<br>[`Iterator_From_HelperChain.js`](../../../tests/Jroc.Tests/Iterator/JavaScript/Iterator_From_HelperChain.js) | `test/language/expressions/yield/star-iterable.js` | The runtime advances iterator records and interprets iterator result objects consistently for the covered for-of, helper, and `yield*` flows. Remaining gaps are in less common exotic iterator result edge cases rather than ordinary step semantics. |

### 7.4.10 ([tc39.es](https://tc39.es/ecma262/#sec-iteratorstepvalue))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IteratorStepValue in iterator destructuring and step/value extraction helpers | Supported with Limitations |  | `test/language/expressions/assignment/destructuring/iterator-destructuring-property-reference-target-evaluation-order.js` | Destructuring lowering uses dedicated step/value helpers that mirror `IteratorStepValue` semantics, including skipping value access when `done` is already true. Coverage is strongest in destructuring-driven consumers rather than as a separately exposed public runtime helper. |

### 7.4.11 ([tc39.es](https://tc39.es/ecma262/#sec-iteratorclose))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IteratorClose | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js) | `test/built-ins/Map/iterator-items-are-not-object-close-iterator.js`<br>`test/built-ins/Object/fromEntries/iterator-closed-for-throwing-entry-value-accessor.js` | Break/throw and abrupt-built-in paths invoke iterator return() for supported iterator forms while preserving the original completion behavior in the covered cases. |

### 7.4.12 ([tc39.es](https://tc39.es/ecma262/#sec-iteratorcloseall))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IteratorCloseAll for helper pipelines with multiple active iterators | Supported with Limitations | [`Iterator_Helper_Cleanup.js`](../../../tests/Jroc.Tests/Iterator/JavaScript/Iterator_Helper_Cleanup.js) |  | The shipped iterator-helper pipeline closes both outer and active inner iterators in the covered flatMap and abrupt-cleanup cases, which is the current runtime analogue of `IteratorCloseAll`. JROC does not currently expose a separate reusable `IteratorCloseAll` helper outside those concrete iterator-helper consumers. |

### 7.4.13 ([tc39.es](https://tc39.es/ecma262/#sec-ifabruptcloseiterator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IfAbruptCloseIterator in destructuring and iterable-consuming built-ins | Supported with Limitations |  | `test/built-ins/Map/iterator-items-are-not-object-close-iterator.js`<br>`test/built-ins/Object/fromEntries/iterator-closed-for-throwing-entry-value-accessor.js` | Abrupt completions in the covered Map and Object.fromEntries iterable-consumption paths close the active iterator before propagating the original error. The behavior is implemented inline in consumers rather than through a named public helper. |

### 7.4.14 ([tc39.es](https://tc39.es/ecma262/#sec-asynciteratorclose))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| AsyncIteratorClose | Supported with Limitations | [`Async_ForAwaitOf_AsyncIterator_BreakCloses.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ForAwaitOf_AsyncIterator_BreakCloses.js)<br>[`Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js) |  | Async iterator close is implemented for both native async iterators and sync-iterator fallback wrappers. |

### 7.4.15 ([tc39.es](https://tc39.es/ecma262/#sec-ifabruptcloseasynciterator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IfAbruptCloseAsyncIterator behavior in for-await control flow | Supported with Limitations | [`Async_ForAwaitOf_AsyncIterator_BreakCloses.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ForAwaitOf_AsyncIterator_BreakCloses.js)<br>[`Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js) |  | Abrupt loop exits trigger async iterator close in supported lowered paths. |

### 7.4.16 ([tc39.es](https://tc39.es/ecma262/#sec-createiterresultobject))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| CreateIteratorResultObject | Supported with Limitations | [`Generator_BasicNext.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_BasicNext.js)<br>[`AsyncGenerator_BasicNext.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_BasicNext.js) |  | Iterator result objects are produced via IteratorResult helpers for sync and async iterator implementations. |

### 7.4.17 ([tc39.es](https://tc39.es/ecma262/#sec-createlistiteratorRecord))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| CreateListIteratorRecord for list-backed built-in iterators | Supported with Limitations | [`Function_Call_Spread_StringIterable.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_StringIterable.js)<br>[`Iterator_Helper_Next_Return.js`](../../../tests/Jroc.Tests/Iterator/JavaScript/Iterator_Helper_Next_Return.js) | `test/built-ins/Array/prototype/values/returns-iterator.js` | Arrays, strings, typed arrays, arguments objects, and iterator helpers expose concrete list-backed iterator objects that fill the practical role of list iterator records for the covered runtime features. The implementation uses those concrete iterator classes directly rather than a separate standalone `CreateListIteratorRecord` entry point. |

### 7.4.18 ([tc39.es](https://tc39.es/ecma262/#sec-iteratortolist))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IteratorToList for helper materialization and TypedArray.from source capture | Supported with Limitations | [`Iterator_From_HelperChain.js`](../../../tests/Jroc.Tests/Iterator/JavaScript/Iterator_From_HelperChain.js) | `test/built-ins/TypedArray/from/iterated-array-changed-by-tonumber.js` | Iterator results are materialized into concrete lists for the covered `Iterator.prototype.toArray()` and `%TypedArray%.from(...)` paths, including capture-before-mutation behavior for iterated sources. The runtime still models list materialization inside specific consumers instead of through a single shared public helper. |

