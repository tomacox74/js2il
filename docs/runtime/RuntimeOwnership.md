# Runtime ownership

JROC runtime state has three explicit lifetime owners:

```text
RuntimeAgentCluster
  -> RuntimeAgent
      -> RuntimeAgentSchedulingState
          -> NodeSchedulerState
          -> NodeEventLoopPump
          -> AsyncContextRuntime
          -> FinalizationRegistryHost
      -> RuntimeRealm
          -> RuntimeIntrinsics
          -> RuntimeModuleState
          -> RuntimeRealmValueCacheState
          -> ServiceContainer
          -> RuntimeExecutionContext (while entered)
```

- `RuntimeAgentCluster` owns agents and, in later migration stages, only
  resources intentionally shared across agents.
- `RuntimeAgent` owns execution and scheduling lifecycle and can own one or
  more realms.
- `RuntimeAgentSchedulingState` owns timers, queues, pending I/O, the event-loop
  pump, async-hooks context, finalization jobs, wake-up signaling, and
  cooperative cancellation. See
  [Runtime agent scheduling](RuntimeAgentScheduling.md).
- `RuntimeRealm` owns one JavaScript object graph and exactly one runtime
  `ServiceContainer`.
- `RuntimeIntrinsics` owns the realm's well-known intrinsic object graph
  (ECMA-262 Realm Record `[[Intrinsics]]`). That includes `Object.prototype` and
  every other built-in prototype, the `JSON`/`Intl`/`Atomics` namespace objects,
  the `BuiltinDelegateFunctionAdapter` identity of every built-in constructor and
  method, the global built-in function values returned by
  `GlobalThis.GetFunctionValue`, the intrinsic descriptor baseline for
  non-`JsObject` targets, and the `[[Prototype]]` fallback slots for values that
  are not `JsObject` instances. `GlobalThis` is constructed with its owning
  realm's `RuntimeIntrinsics` and wires prototype chains, built-in methods, and
  constructor linkage against it once per realm instead of once per process. See
  [`RuntimeIntrinsics.cs`](../../src/JavaScriptRuntime/RuntimeIntrinsics.cs) for
  the slot list and lifecycle.
- `RuntimeModuleState` owns the realm's CommonJS and ESM graph, including
  module instances, live binding cells, namespace identities, `import.meta`,
  module-scoped require delegates, and the compiled module assembly.
- `RuntimeRealmValueCacheState` owns tagged-template objects, materialized class
  constructor objects, and lazy class-method metadata that captures scopes.
  See [Realm-created value caches](RuntimeRealmValueCaches.md).

The child keeps a reference to its parent so services can receive the correct
owner through constructor injection. Parents keep their children only while
the children are active. Disposing a child detaches it from its parent.
Disposing a parent disposes children in reverse creation order, then marks the
parent disposed. Disposal is idempotent.

Runtime service containers register their realm, agent, cluster, and
agent-scheduling services as reserved services. Those registrations cannot be
replaced or removed. Realms in one agent resolve the same scheduler, event
loop, async context, and finalization host. Child DI scopes retain the same
owners. After realm disposal, its service container and any child scopes reject
further use.

The factory currently creates an isolated cluster, agent, realm, and service
container for each `RuntimeServices.BuildServiceProvider()` call. Each realm
owns its intrinsic graph, module state, and realm-created value caches; its
agent owns one scheduling graph.

## Intrinsic ownership

Every JavaScript object that a realm can observe as a global, constructor,
prototype, or built-in function is created for that realm alone:

- Built-in prototypes are lazily created realm intrinsic slots. The thread that
  wins a slot runs its initializer and resolves the slot reentrantly to the
  object under construction, so the mutually recursive intrinsic bootstrap
  cycles ECMA-262 requires (for example `Function.prototype.apply` being a
  function object whose `[[Prototype]]` is `Function.prototype`) resolve instead
  of recursing. Every other thread blocks until the initializer has completed
  and therefore never observes a half-wired intrinsic; a failed initializer
  leaves the slot empty so the next resolution retries.
- The realm's one-time global bootstrap is itself an intrinsic slot
  (`RuntimeIntrinsicSlot.RealmBootstrap`), so bootstrap and lazy slot creation
  share a single coordination protocol instead of nesting two locks. Primitive
  and error prototype accessors resolve through the already-resolved
  `RuntimeIntrinsics` after a volatile bootstrap-state check, so they take no
  lock once the realm is bootstrapped.
- Lock order is `intrinsic slot gate` → `BuiltinDelegateFunctionAdapter`
  initialization lock. The adapter lock is a leaf: `Function` materializes
  `%Function.prototype%` and `%Object.prototype%` *before* acquiring it, so an
  intrinsic initializer that configures built-in function objects can never
  invert against a thread that already holds an adapter lock.
- `RuntimeIntrinsics.Current` always answers with the ambient realm, so repeated
  resolutions inside one operation cannot switch graphs when another realm is
  created. Context-less callers get one deterministic process-default graph that
  is never a live realm's graph.
- Built-in function objects are `BuiltinDelegateFunctionAdapter` instances
  resolved from the realm's adapter cache. The delegates, method handles, and
  invoke metadata behind them are immutable CLR metadata and stay process-wide;
  only the JavaScript-visible wrapper is realm-owned.
- `JsObject` stores its own descriptors and `[[Prototype]]` inline, so those are
  realm-correct once the object itself is realm-owned. Values that are not
  `JsObject` instances (`Map`, `Set`, `RegExp`, `Date`, `Promise` instances and
  the `Type` handles used as constructor markers) resolve their descriptor
  baseline and `[[Prototype]]` link through the realm's
  `IntrinsicDescriptors`/`PrototypeSlots` tables.
- Post-bootstrap descriptor writes continue to go through
  `PropertyDescriptorStore`'s per-realm overlay (`HasSharedIntrinsicBaseline`),
  so descriptor definition, redefinition, and deletion are realm-local.
- Realm disposal clears the realm's intrinsic slots, adapter cache, and global
  function values, so a disposed realm's object graph becomes collectible.

### Residual process-wide CLR metadata

`Math`, `Reflect`, `Date`, `AbortController`, `AbortSignal`,
`Intl.NumberFormat`, and `Intl.Segmenter` are represented by CLR `Type` handles
rather than JavaScript objects. The handle itself is immutable CLR metadata and
is explicitly allowed to be process-wide; all observable state hanging off it
(own property descriptors including `prototype`, and its `[[Prototype]]` link)
is realm-owned, so mutating any of it in one realm is invisible in another.
Replacing those markers with realm-owned constructor objects is the
`Type`-to-constructor materialization work tracked by #1825.

`RegExp`'s well-known-symbol fast-path flag and `Array`'s prototype-mutation
version counter remain process-wide. Both are monotonic de-optimization hints
that only ever disable a fast path, hold no realm-created object, and cannot
change observable semantics.

## Reference rules

- Process-wide code may retain immutable metadata, but not an active realm,
  agent, or cluster.
- A cluster may reference its active agents.
- An agent may reference its cluster and active realms.
- An agent owns one scheduling graph and cancellation source. Its external
  wake API queues work for the agent executor and never runs JavaScript on the
  producer thread.
- A realm may reference its agent, intrinsics graph, and service container.
- A realm owns one module state object; no mutable module graph is
  process-wide.
- A realm owns its template, materialized class constructor, and captured-scope
  metadata caches; process-wide caches contain immutable CLR metadata only.
- An entered execution context is the only ambient pointer to a realm and
  agent; thread identity is not an ownership boundary.
- Realm services may receive any of the three owners through constructor
  injection, but must not discover them through a second service locator.
- Realm-created JavaScript objects must not be stored on an agent or cluster.
- Cross-agent resources must not retain realm-created wrappers or callbacks.
