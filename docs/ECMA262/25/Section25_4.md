<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 25.4: The Atomics Object

[Back to Section25](Section25.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-06-24T17:00:14Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 25.4 | The Atomics Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 25.4.1 | Waiter Record | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-waiter-record) |
| 25.4.2 | WaiterList Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-waiterlist-records) |
| 25.4.3 | Abstract Operations for Atomics | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-atomics) |
| 25.4.3.1 | ValidateIntegerTypedArray ( typedArray , waitable ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-validateintegertypedarray) |
| 25.4.3.2 | ValidateAtomicAccess ( taRecord , requestIndex ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-validateatomicaccess) |
| 25.4.3.3 | ValidateAtomicAccessOnIntegerTypedArray ( typedArray , requestIndex ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-validateatomicaccessonintegertypedarray) |
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
| 25.4.3.14 | DoWait ( mode , typedArray , index , value , timeout ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-dowait) |
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
| 25.4.13 | Atomics.wait ( typedArray , index , value , timeout ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.wait) |
| 25.4.14 | Atomics.waitAsync ( typedArray , index , value , timeout ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.waitasync) |
| 25.4.15 | Atomics.notify ( typedArray , index , count ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.notify) |
| 25.4.16 | Atomics.xor ( typedArray , index , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics.xor) |
| 25.4.17 | Atomics [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics-%symbol.tostringtag%) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 25.4 ([tc39.es](https://tc39.es/ecma262/#sec-atomics-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Atomics object and shared-memory atomic operations | Not Yet Supported |  |  | JROC does not currently expose SharedArrayBuffer or the Atomics global. Shared-memory semantics are intentionally documented as not yet supported; Set.prototype entries that previously appeared in this file belonged to Section24_2 and have been removed. |

