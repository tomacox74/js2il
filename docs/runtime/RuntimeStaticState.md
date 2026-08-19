# Runtime static-state ownership

This is the approved mutable-static inventory for `src/JavaScriptRuntime`.
`RuntimeStaticStateAuditTests` reflects over `JavaScriptRuntime.dll` and fails
when a writable static field or readonly reference candidate is added without
an ownership decision here and in the executable matrix. Candidates include
arrays, common mutable collections, lazy/thread/async holders, weak tables,
encodings, regexes, and non-delegate runtime classes.

JavaScript-observable values belong to a `RuntimeRealm`, `RuntimeAgent`, or
`RuntimeAgentCluster`. Process state is limited to immutable CLR metadata,
weak identity metadata, execution-flow carriers, monotonic deoptimization
hints, resource identity allocators, and context-less compatibility fallbacks.

## Writable static fields

| Field | Owner | Why it may remain static |
| --- | --- | --- |
| `Array._defaultPrototypeChainHasIndexedProperties` | CLR thread | Thread-local fast-path observation only |
| `Array._observedPrototypeIntrinsicsId` | CLR thread | Thread-local fast-path observation only |
| `Array._observedPrototypeMutationVersion` | CLR thread | Thread-local fast-path observation only |
| `Array._prototypeMutationVersion` | Process | Monotonic deoptimization version; retains no JavaScript value |
| `AsyncContextRuntime._activeContextRuntimeCount` | Process | Fast-path activity count; actual async-hook state is agent-owned |
| `AsyncContextRuntime._enabledHookCount` | Process | Fast-path activity count; actual hooks are agent-owned |
| `FsCommon._nextFileDescriptor` | Process | Identity allocator for process file resources |
| `RegExp._prototypeWellKnownSymbolFastPathFlags` | Process | Monotonic deoptimization flags; can only disable an optimization |
| `RuntimeIntrinsics._initializationDepth` | CLR thread | Reentrant bootstrap state for the calling thread |
| `RuntimeIntrinsics._nextId` | Process | Metadata identity allocator |
| `RuntimeIntrinsics._processDefault` | Process | Context-less compatibility graph, never a live realm's graph |
| `RuntimeServices._constructorArgStack` | CLR thread | Synchronous invocation state |
| `RuntimeServices._constructorNewTargetStack` | CLR thread | Synchronous invocation state |
| `RuntimeServices._derivedConstructorThisStack` | CLR thread | Synchronous invocation state |
| `RuntimeServices._generatedFunctionDirectCallStack` | CLR thread | Synchronous invocation state |
| `String.substringCache` | CLR thread | Primitive strings only |
| `String.substringCacheNextIndex` | CLR thread | Index into the thread-local primitive cache |
| `Symbol._nextId` | Process | Identity allocator; symbol values are not stored by the counter |

Bootstrap-in-progress state must be thread-local. A process-wide counter is not
an acceptable substitute even when initialization work is lock-serialized:
another thread must not observe itself as reentrant.

## Mutable static holders

| Field | Owner | Retention rule |
| --- | --- | --- |
| `ArgumentsObject.FieldCaches` | Process metadata | `ConditionalWeakTable<Type, ...>`; collectible generated types are weak keys |
| `Closure._delegateInvokeMetadata` | Process metadata | Weak-keyed delegate metadata |
| `GlobalThis._fallbackGlobalObject` | CLR thread | Used only without an active runtime frame |
| `JSON.RawJsonObjects` | Process identity metadata | Raw-JSON values are weak keys |
| `JsShape._empty` | CLR thread | Context-less empty shape only |
| `AsyncResourceObject.States` | Process identity metadata | Async resource receivers are weak keys |
| `ObjectRuntime._encodedSymbolKeys` | Process identity metadata | Encoded key strings are weak keys; symbols live only with their property keys |
| `ObjectRuntime._integrityStates` | Process identity metadata | Target objects are weak keys |
| `PropertyDescriptorStore._defaultRuntimeStore` | CLR thread | Context-less descriptor fallback only |
| `PropertyDescriptorStore._intrinsicInitializationDepth` | CLR thread | Reentrant intrinsic bootstrap state |
| `RuntimeExecutionContext.Ambient` | Async flow | The sole ambient realm/agent pointer |
| `RuntimeIntrinsics._blockedThreads` | Process coordination | Transient wait graph; entries are removed when waits end |
| `RuntimeServices._currentInvocation` | Async flow | Immutable residual invocation frame captured/restored by root frames |
| `JsReturnConverter.ResultConversions` | Process metadata | Weak-keyed constructed generic methods |

A weak-keyed table is approved only when the key has the semantic lifetime of
the cached value. Ephemeron values may refer back to their keys without keeping
collectible types or JavaScript objects alive.

## Immutable process state

The remaining readonly statics fall into these audited groups:

- Built-in delegates in `GlobalThis`, `ObjectRuntime`, intrinsic types, and
  Node adapters are immutable CLR call targets. Realm-visible
  `JsFunctionObject` wrappers are materialized by `RuntimeIntrinsics`.
- `Symbol.iterator` through `Symbol.unscopables` are ECMA-262 well-known symbol
  identities. `Events.ErrorMonitorSymbol` is the immutable
  `node:events.errorMonitor` module token.
- `RuntimeServices.EmptyScopes`, `TemporalDeadZoneSentinel`, `Array.Hole`,
  `Map.NullKeySentinel`, `WeakSet._dummyValue`, and generator sentinels are ABI
  tokens. `EmptyScopes` is intentionally a one-element array and must never be
  mutated.
- Frozen intrinsic, Node module, global-binding, and delegate-type catalogs
  contain runtime-assembly types or strings only. They cannot contain
  generated/collectible types.
- Regexes, encodings, path-independent configuration records, method handles,
  and primitive lookup arrays are immutable implementation metadata.
- `GlobalThis`'s default console, process, and global object are compatibility
  fallbacks used only when no runtime frame exists. A live runtime always
  resolves the realm-owned service graph.

`Math.random` uses the BCL's process entropy source without retaining a JROC
static generator. Process-wide identifiers and conservative fast-path flags
may affect allocation order or performance, but never JavaScript ownership or
observable state.

## Migrations completed by the final audit

- `dns` default result order, `fs.constants`, and `path.posix`/`path.win32` are
  now owned by their realm-local Node module instances.
- Argument field metadata and hosted generic return-conversion methods use
  weak type keys, so collectible generated assemblies are not retained.
- Encoded symbol-key metadata uses weak string identity rather than a
  process-wide strong dictionary.
- Intrinsic and Node module discovery catalogs and the JavaScript delegate-type
  catalog are frozen after construction.

## Enforcement

- Any new writable static or readonly reference candidate must update the
  executable ownership matrix and this document. Frozen collections,
  delegates, strings, reflection handles, and value types are excluded as
  immutable metadata categories.
- Process caches must never strongly retain a realm, agent, cluster,
  JavaScript object, callback, captured scope, module instance, or collectible
  generated type.
- JavaScript-visible module wrappers and values belong to the realm even when
  their backing CLR behavior is stateless.
- Ambient state identifies the active owner; it is not an independent owner.
- Parallel tests must overlap execution. Sequential create/dispose tests do not
  prove isolation.

The stress gate overlaps four runtimes repeatedly and covers globals,
intrinsics, module state, agent symbol registries, microtasks, timers, async
call state, template objects, and Node module wrappers. Separate weak-reference
gates verify runtime graphs, encoded symbols, and collectible generated types
are reclaimed after bounded GC cycles.
