<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 25.4: The Atomics Object

[Back to Section25](Section25.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-25T20:54:02Z

JROC exposes the Atomics global, its %Symbol.toStringTag%, and a synchronous Atomics.wait implementation for SharedArrayBuffer-backed Int32Array values. The runtime has no shared-agent waiter lists, notification, or other atomic read-modify-write operations.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 25.4 | The Atomics Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-atomics-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 25.4.1 | Waiter Record | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-waiter-record) |
| 25.4.2 | WaiterList Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-waiterlist-records) |
| 25.4.3 | Abstract Operations for Atomics | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-atomics) |
| 25.4.3.1 | ValidateIntegerTypedArray ( typedArray , waitable ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-validateintegertypedarray) |
| 25.4.3.2 | ValidateAtomicAccess ( taRecord , requestIndex ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-validateatomicaccess) |
| 25.4.3.3 | ValidateAtomicAccessOnIntegerTypedArray ( typedArray , requestIndex ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-validateatomicaccessonintegertypedarray) |
| 25.4.3.4 | RevalidateAtomicAccess ( typedArray , byteIndexInBuffer ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-revalidateatomicaccess) |
| 25.4.3.5 | GetWaiterList ( block , i ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-getwaiterlist) |
| 25.4.3.6 | EnterCriticalSection ( WL ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-entercriticalsection) |
| 25.4.3.7 | LeaveCriticalSection ( WL ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-leavecriticalsection) |
| 25.4.3.8 | AddWaiter ( WL , waiterRecord ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-addwaiter) |
| 25.4.3.9 | RemoveWaiter ( WL , waiterRecord ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-removewaiter) |
| 25.4.3.10 | RemoveWaiters ( WL , c ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-removewaiters) |
| 25.4.3.11 | SuspendThisAgent ( WL , waiterRecord ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-suspendthisagent) |
| 25.4.3.12 | NotifyWaiter ( WL , waiterRecord ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-notifywaiter) |
| 25.4.3.13 | EnqueueResolveInAgentJob ( agentSignifier , promiseCapability , resolution ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-enqueueresolveinagentjob) |
| 25.4.3.14 | DoWait ( mode , typedArray , index , value , timeout ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-dowait) |
| 25.4.3.15 | EnqueueAtomicsWaitAsyncTimeoutJob ( WL , waiterRecord ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-enqueueatomicswaitasynctimeoutjob) |
| 25.4.3.16 | AtomicCompareExchangeInSharedBlock ( block , byteIndexInBuffer , elementSize , expectedBytes , replacementBytes ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomiccompareexchangeinsharedblock) |
| 25.4.3.17 | AtomicReadModifyWrite ( typedArray , index , value , op ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomicreadmodifywrite) |
| 25.4.3.18 | ByteListBitwiseOp ( op , xBytes , yBytes ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bytelistbitwiseop) |
| 25.4.3.19 | ByteListEqual ( xBytes , yBytes ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bytelistequal) |
| 25.4.4 | Atomics.add ( typedArray , index , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.add) |
| 25.4.5 | Atomics.and ( typedArray , index , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.and) |
| 25.4.6 | Atomics.compareExchange ( typedArray , index , expectedValue , replacementValue ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.compareexchange) |
| 25.4.7 | Atomics.exchange ( typedArray , index , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.exchange) |
| 25.4.8 | Atomics.isLockFree ( size ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.islockfree) |
| 25.4.9 | Atomics.load ( typedArray , index ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.load) |
| 25.4.10 | Atomics.or ( typedArray , index , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.or) |
| 25.4.11 | Atomics.store ( typedArray , index , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.store) |
| 25.4.12 | Atomics.sub ( typedArray , index , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.sub) |
| 25.4.13 | Atomics.wait ( typedArray , index , value , timeout ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-atomics.wait) |
| 25.4.14 | Atomics.waitAsync ( typedArray , index , value , timeout ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.waitasync) |
| 25.4.15 | Atomics.notify ( typedArray , index , count ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.notify) |
| 25.4.16 | Atomics.xor ( typedArray , index , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.xor) |
| 25.4.17 | Atomics [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics-%symbol.tostringtag%) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 25.4 ([tc39.es](https://tc39.es/ecma262/#sec-atomics-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Atomics object and shared-memory atomic operations | Supported with Limitations | [`Symbol.toStringTag.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/JavaScript/Symbol.toStringTag.js)<br>[`non-views.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/add/JavaScript/non-views.js)<br>[`non-views.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/and/JavaScript/non-views.js)<br>[`non-views.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/compareExchange/JavaScript/non-views.js)<br>[`non-views.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/exchange/JavaScript/non-views.js)<br>[`non-views.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/load/JavaScript/non-views.js)<br>[`non-views.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/notify/JavaScript/non-views.js)<br>[`non-views.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/or/JavaScript/non-views.js)<br>[`non-integral-iterationnumber-throws.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/pause/JavaScript/non-integral-iterationnumber-throws.js)<br>[`non-views.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/store/JavaScript/non-views.js)<br>[`non-views.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/sub/JavaScript/non-views.js)<br>[`bad-range.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/wait/JavaScript/bad-range.js)<br>[`negative-index-throws.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/wait/JavaScript/negative-index-throws.js)<br>[`non-shared-bufferdata-throws.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/wait/JavaScript/non-shared-bufferdata-throws.js)<br>[`not-a-typedarray-throws.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/wait/JavaScript/not-a-typedarray-throws.js)<br>[`not-an-object-throws.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/wait/JavaScript/not-an-object-throws.js)<br>[`out-of-range-index-throws.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/wait/JavaScript/out-of-range-index-throws.js)<br>[`symbol-for-index-throws.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/wait/JavaScript/symbol-for-index-throws.js)<br>[`symbol-for-timeout-throws.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/wait/JavaScript/symbol-for-timeout-throws.js)<br>[`non-views.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/xor/JavaScript/non-views.js) | `test/built-ins/Atomics/Symbol.toStringTag.js`<br>`test/built-ins/Atomics/add/non-views.js`<br>`test/built-ins/Atomics/and/non-views.js`<br>`test/built-ins/Atomics/compareExchange/non-views.js`<br>`test/built-ins/Atomics/exchange/non-views.js`<br>`test/built-ins/Atomics/load/non-views.js`<br>`test/built-ins/Atomics/notify/non-views.js`<br>`test/built-ins/Atomics/or/non-views.js`<br>`test/built-ins/Atomics/pause/non-integral-iterationnumber-throws.js`<br>`test/built-ins/Atomics/store/non-views.js`<br>`test/built-ins/Atomics/sub/non-views.js`<br>`test/built-ins/Atomics/wait/bad-range.js`<br>`test/built-ins/Atomics/wait/negative-index-throws.js`<br>`test/built-ins/Atomics/wait/non-shared-bufferdata-throws.js`<br>`test/built-ins/Atomics/wait/not-a-typedarray-throws.js`<br>`test/built-ins/Atomics/wait/not-an-object-throws.js`<br>`test/built-ins/Atomics/wait/out-of-range-index-throws.js`<br>`test/built-ins/Atomics/wait/symbol-for-index-throws.js`<br>`test/built-ins/Atomics/wait/symbol-for-timeout-throws.js`<br>`test/built-ins/Atomics/xor/non-views.js` | Ported coverage verifies Atomics branding plus validation and abrupt-completion behavior for non-typed-array receivers, invalid wait indices and coercions, and pause iteration counts. JROC implements Atomics.wait for SharedArrayBuffer-backed Int32Array values, but atomic read-modify-write operations, wait/notify coordination, and multi-agent shared-memory semantics remain unsupported. |

### 25.4.13 ([tc39.es](https://tc39.es/ecma262/#sec-atomics.wait))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Atomics.wait (Int32Array) | Supported with Limitations | [`SharedArrayBuffer_Int32Array_AtomicsWait.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/SharedArrayBuffer_Int32Array_AtomicsWait.js) |  | Supports SharedArrayBuffer-backed Int32Array validation, bounds checks, not-equal results, and timeout results. BigInt64Array, agent wakeup/notification, and cross-agent shared-memory semantics are unsupported. |

### 25.4.17 ([tc39.es](https://tc39.es/ecma262/#sec-atomics-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Atomics %Symbol.toStringTag% | Supported | [`Symbol.toStringTag.js`](../../../tests/Jroc.Test262.Tests/built-ins/Atomics/JavaScript/Symbol.toStringTag.js) | `test/built-ins/Atomics/Symbol.toStringTag.js` | Atomics exposes the standard non-writable, non-enumerable, configurable "Atomics" tag. |

