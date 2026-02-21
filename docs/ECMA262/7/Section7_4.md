<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 7.4: Operations on Iterator Objects

[Back to Section7](Section7.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 7.4 | Operations on Iterator Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-operations-on-iterator-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 7.4.1 | Iterator Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iterator-records) |
| 7.4.2 | GetIteratorDirect ( obj ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getiteratordirect) |
| 7.4.3 | GetIteratorFromMethod ( obj , method ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getiteratorfrommethod) |
| 7.4.4 | GetIterator ( obj , kind ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getiterator) |
| 7.4.5 | GetIteratorFlattenable ( obj , primitiveHandling ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-getiteratorflattenable) |
| 7.4.6 | IteratorNext ( iteratorRecord [ , value ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratornext) |
| 7.4.7 | IteratorComplete ( iteratorResult ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratorcomplete) |
| 7.4.8 | IteratorValue ( iteratorResult ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratorvalue) |
| 7.4.9 | IteratorStep ( iteratorRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratorstep) |
| 7.4.10 | IteratorStepValue ( iteratorRecord ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iteratorstepvalue) |
| 7.4.11 | IteratorClose ( iteratorRecord , completion ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteratorclose) |
| 7.4.12 | IteratorCloseAll ( iters , completion ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iteratorcloseall) |
| 7.4.13 | IfAbruptCloseIterator ( value , iteratorRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ifabruptcloseiterator) |
| 7.4.14 | AsyncIteratorClose ( iteratorRecord , completion ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asynciteratorclose) |
| 7.4.15 | IfAbruptCloseAsyncIterator ( value , iteratorRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ifabruptcloseasynciterator) |
| 7.4.16 | CreateIteratorResultObject ( value , done ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createiterresultobject) |
| 7.4.17 | CreateListIteratorRecord ( list ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-createlistiteratorRecord) |
| 7.4.18 | IteratorToList ( iteratorRecord ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iteratortolist) |

## Support

Feature-level support tracking with test script references.

### 7.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-getiterator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| GetIterator for for..of sources | Supported with Limitations | [`ControlFlow_ForOf_Array_Basic.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Array_Basic.js)<br>[`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js) | Supports built-in iterables, user-defined Symbol.iterator sources, and IEnumerable fallback paths. |

### 7.4.6 ([tc39.es](https://tc39.es/ecma262/#sec-iteratornext))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IteratorNext | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js)<br>[`Generator_BasicNext.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_BasicNext.js) | Iterator advancement is implemented for native and dynamic iterator objects used by lowering/runtime helpers. |

### 7.4.7 ([tc39.es](https://tc39.es/ecma262/#sec-iteratorcomplete))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IteratorComplete | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js) | Done-state checks are implemented through IteratorResultDone in iteration paths. |

### 7.4.8 ([tc39.es](https://tc39.es/ecma262/#sec-iteratorvalue))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IteratorValue | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js) | Value extraction is implemented through IteratorResultValue for native and dynamic iterator result shapes. |

### 7.4.11 ([tc39.es](https://tc39.es/ecma262/#sec-iteratorclose))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IteratorClose | Supported with Limitations | [`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js) | Break/throw paths invoke iterator return() for supported iterator forms. |

### 7.4.14 ([tc39.es](https://tc39.es/ecma262/#sec-asynciteratorclose))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| AsyncIteratorClose | Supported with Limitations | [`Async_ForAwaitOf_AsyncIterator_BreakCloses.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_AsyncIterator_BreakCloses.js)<br>[`Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js) | Async iterator close is implemented for both native async iterators and sync-iterator fallback wrappers. |

### 7.4.15 ([tc39.es](https://tc39.es/ecma262/#sec-ifabruptcloseasynciterator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IfAbruptCloseAsyncIterator behavior in for-await control flow | Supported with Limitations | [`Async_ForAwaitOf_AsyncIterator_BreakCloses.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_AsyncIterator_BreakCloses.js)<br>[`Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js) | Abrupt loop exits trigger async iterator close in supported lowered paths. |

### 7.4.16 ([tc39.es](https://tc39.es/ecma262/#sec-createiterresultobject))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| CreateIteratorResultObject | Supported with Limitations | [`Generator_BasicNext.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_BasicNext.js)<br>[`AsyncGenerator_BasicNext.js`](../../../Js2IL.Tests/AsyncGenerator/JavaScript/AsyncGenerator_BasicNext.js) | Iterator result objects are produced via IteratorResult helpers for sync and async iterator implementations. |

