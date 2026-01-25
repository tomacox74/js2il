<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 25.4: The Atomics Object

[Back to Section25](Section25.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 25.4 | The Atomics Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 25.4.1 | Waiter Record | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-waiter-record) |
| 25.4.2 | WaiterList Records | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-waiterlist-records) |
| 25.4.3 | Abstract Operations for Atomics | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-atomics) |
| 25.4.3.1 | ValidateIntegerTypedArray ( typedArray , waitable ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-validateintegertypedarray) |
| 25.4.3.2 | ValidateAtomicAccess ( taRecord , requestIndex ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-validateatomicaccess) |
| 25.4.3.3 | ValidateAtomicAccessOnIntegerTypedArray ( typedArray , requestIndex ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-validateatomicaccessonintegertypedarray) |
| 25.4.3.4 | RevalidateAtomicAccess ( typedArray , byteIndexInBuffer ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-revalidateatomicaccess) |
| 25.4.3.5 | GetWaiterList ( block , i ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getwaiterlist) |
| 25.4.3.6 | EnterCriticalSection ( WL ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-entercriticalsection) |
| 25.4.3.7 | LeaveCriticalSection ( WL ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-leavecriticalsection) |
| 25.4.3.8 | AddWaiter ( WL , waiterRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-addwaiter) |
| 25.4.3.9 | RemoveWaiter ( WL , waiterRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-removewaiter) |
| 25.4.3.10 | RemoveWaiters ( WL , c ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-removewaiters) |
| 25.4.3.11 | SuspendThisAgent ( WL , waiterRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-suspendthisagent) |
| 25.4.3.12 | NotifyWaiter ( WL , waiterRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-notifywaiter) |
| 25.4.3.13 | EnqueueResolveInAgentJob ( agentSignifier , promiseCapability , resolution ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-enqueueresolveinagentjob) |
| 25.4.3.14 | DoWait ( mode , typedArray , index , value , timeout ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-dowait) |
| 25.4.3.15 | EnqueueAtomicsWaitAsyncTimeoutJob ( WL , waiterRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-enqueueatomicswaitasynctimeoutjob) |
| 25.4.3.16 | AtomicCompareExchangeInSharedBlock ( block , byteIndexInBuffer , elementSize , expectedBytes , replacementBytes ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomiccompareexchangeinsharedblock) |
| 25.4.3.17 | AtomicReadModifyWrite ( typedArray , index , value , op ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomicreadmodifywrite) |
| 25.4.3.18 | ByteListBitwiseOp ( op , xBytes , yBytes ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bytelistbitwiseop) |
| 25.4.3.19 | ByteListEqual ( xBytes , yBytes ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bytelistequal) |
| 25.4.4 | Atomics.add ( typedArray , index , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.add) |
| 25.4.5 | Atomics.and ( typedArray , index , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.and) |
| 25.4.6 | Atomics.compareExchange ( typedArray , index , expectedValue , replacementValue ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.compareexchange) |
| 25.4.7 | Atomics.exchange ( typedArray , index , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.exchange) |
| 25.4.8 | Atomics.isLockFree ( size ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.islockfree) |
| 25.4.9 | Atomics.load ( typedArray , index ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.load) |
| 25.4.10 | Atomics.or ( typedArray , index , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.or) |
| 25.4.11 | Atomics.store ( typedArray , index , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.store) |
| 25.4.12 | Atomics.sub ( typedArray , index , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.sub) |
| 25.4.13 | Atomics.wait ( typedArray , index , value , timeout ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.wait) |
| 25.4.14 | Atomics.waitAsync ( typedArray , index , value , timeout ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.waitasync) |
| 25.4.15 | Atomics.notify ( typedArray , index , count ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.notify) |
| 25.4.16 | Atomics.xor ( typedArray , index , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics.xor) |
| 25.4.17 | Atomics [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-atomics-%symbol.tostringtag%) |

## Support

Feature-level support tracking with test script references.

### 25.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-atomics.and))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Set.prototype.add | Supported |  | Backed by JavaScriptRuntime.Set.add; returns the Set instance to allow chaining. (No dedicated JS fixture currently referenced in this doc.) |
| Set.prototype.has | Supported |  | Backed by JavaScriptRuntime.Set.has; strict equality for keys based on .NET object identity and string/double value semantics. (No dedicated JS fixture currently referenced in this doc.) |
| Set.prototype.size (getter) | Supported |  | Exposed via a 'size' property on JavaScriptRuntime.Set returning a JS number (double). (No dedicated JS fixture currently referenced in this doc.) |

