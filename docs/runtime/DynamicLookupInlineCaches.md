# Dynamic lookup inline caches

Issue #1893 introduces a conservative per-call-site cache prototype for
dynamic property reads and fixed-arity member calls that remain after AOT
specialization. The prototype targets repeated access to JavaScript-owned
shapes without weakening JavaScript lookup semantics.

Phase 3 (issues #1958 / #1324) replaced the original exact-receiver-identity
cache entries described below with **shape-keyed** entries for property
reads and `CallMember0`. An entry now records a receiver's `JsShape` reference
and a resolved slot index instead of a specific receiver object, a cached
value, or a snapshotted prototype chain. Every hit re-reads the current
receiver's live slot value. This lets thousands of distinct `JsObject`
instances that share one shape and one own plain-data property (for example,
many `GraphNode` instances each with their own `pos` value) hit the same
monomorphic entry instead of exhausting the four-entry polymorphic budget.
Phase 4 (#1963) extends the same contract to `CallMember1`, including direct
prototype methods on ordinary objects and exact `Array` receivers. A prototype
entry adds weak prototype/callable references and a prototype lookup-version
guard, so the unmodified
Kraken A* `openList.findGraphNode(neighbor)` and
`closedList.findGraphNode(neighbor)` sites resolve once and then invoke through
`CallableOperations.Call1` without an argument array.
Sections below describe this current contract; historical Phase 0/1 sections
that still describe identity-only behavior are updated in place rather than
kept as a separate legacy description, since the generated call surface and
state-machine shape (empty/monomorphic/polymorphic/megamorphic, four-entry
bound) are unchanged.

## Terminology

The terms below describe this implementation and may be narrower than their
general use in JavaScript engines.

| Term | Meaning in this feature |
| --- | --- |
| **AOT specialization** | A compile-time decision that replaces dynamic lookup with a direct or guarded operation. Inline caches apply only after AOT analysis cannot prove such a specialization. |
| **Call site** | One generated property-read or member-call instruction. Two instructions that read the same property are separate call sites. |
| **Cache site** | The realm-owned runtime state associated with one generated call site. Its site key identifies the module, generated type, method, and original LIR instruction. |
| **Terminal bypass** | A compiler-emitted static flag for one generated call site. After any realm transitions that site to megamorphic, generated code tests the flag and calls the generic operation without entering the runtime cache helper. The flag is only a conservative deoptimization hint: it contains no JavaScript value or realm reference. |
| **Receiver** | The JavaScript value to the left of the property access, such as `record` in `record.name` or `record.save()`. |
| **Dynamic lookup** | Resolving a property or callable at runtime because the compiler cannot determine the result safely in advance. |
| **Inline cache** | Per-call-site runtime state that reuses a previously resolved own or direct-prototype plain-data property slot when the receiver and holder guards remain valid. Entries and transition state are stored in the realm. Generated code contains only the terminal bypass flag, which can select the full generic algorithm but can never supply a cached value. |
| **Cache entry** | An own-slot entry has a weak receiver-shape reference, property name, and resolved slot. A direct-prototype `CallMember1` entry has a weak receiver shape, weak prototype and callable references, and the prototype lookup version observed during resolution. No entry strongly retains a receiver or JavaScript value. |
| **Empty** | A cache site that has not recorded a usable shape/property/slot triple. |
| **Monomorphic** | A site containing one live cache entry. Because entries are shape-keyed, every receiver that shares that exact `JsShape` reference hits the same entry, regardless of how many distinct receiver instances exist. |
| **Polymorphic** | A site containing two through four live cache entries, allowing a small set of distinct shapes to hit the same generated site. |
| **Megamorphic** | A site that has observed more distinct shapes than its four-entry limit. It permanently stops recording entries, publishes its generated terminal bypass flag, and uses generic lookup. |
| **Cache hit** | The property name matches an entry, the entry's weakly held `JsShape` is still alive, the receiver's current shape is reference-equal to it, and the resolved slot carries no non-default descriptor flags (plain writable/enumerable/configurable data). The receiver's current slot value is then read and returned. |
| **Cache miss** | No valid entry matches. The runtime performs exact generic lookup and may record a new entry when the result is an own plain-data property. |
| **Invalidation** | A structural change — adding, deleting, or redefining a property (including a data/accessor transition) — changes the receiver's shape or per-slot descriptor flags, so the existing entry (keyed to the old shape/slot) can no longer hit for that receiver. There is no separate value or version snapshot to invalidate: reads are always live. |
| **Generic fallback** | The existing `ObjectRuntime` lookup or call path, which preserves complete JavaScript behavior for misses and cases the cache deliberately excludes. |
| **Ordinary object** | An object whose exact CLR representation is `JsObject`. Property reads and member calls can cache own plain-data slots; `CallMember1` can also cache a plain-data method on the direct prototype. |
| **Exotic object** | A representation with specialized property behavior, such as a proxy, array, typed array, or host object. Property reads continue to bypass the cache. Phase 4 admits exact `Array` receivers only for guarded `CallMember1` direct-prototype slots; other exotic representations bypass it. |
| **Realm** | The JavaScript execution environment that owns intrinsic objects and this feature’s cache-site dictionary. Cache entries and site snapshots are never shared between realms. A generated terminal bypass may conservatively deoptimize the same compiled call site in another realm, but it never exposes or reuses another realm's values or lookup state. |

## Scope and site identity

The compiler routes materialized string-key `LIRGetItem` instructions and
dynamic `LIRCallMember0`/`LIRCallMember1` instructions through
`DynamicLookupInlineCache.GetItem`, `CallMember0`, and `CallMember1`.

### Key construction and call-site association

During IL emission, the compiler constructs the site key in this format:

```text
<module-id>:<generated-type-full-name>:<method-name>:<lir-instruction-index>
```

These components are concatenated with literal `:` separators; the runtime
treats the result as an opaque string and does not parse it.

The components are:

| Component | Source and purpose |
| --- | --- |
| `module-id` | The canonical logical module ID copied from the callable's symbol-table scope into `MethodBodyIR.ModuleId`. It separates equivalent generated type and method names belonging to different JavaScript modules. The fallback text is `<module>` when no module ID is available. |
| `generated-type-full-name` | The namespace-qualified CLR type name from the current `TypeBuilder`. It separates generated module, function, class, and helper types within a module. |
| `method-name` | The emitted CLR method name from the current `MethodDescriptor`. It separates methods on the generated type. |
| `lir-instruction-index` | The zero-based position of the exact `LIRInstruction` object in the final `MethodBodyIR.Instructions` list. It separates multiple dynamic operations in the same method. |

For example, a generated property read contains IL equivalent to:

```text
ldsfld int32 <generated-owner>::__jroc_dynamicLookup_42
brfalse cachedPath
ldarg.1
ldstr "value"
call ObjectRuntime.GetItem(object, string)
br done
cachedPath:
ldarg.1
ldstr "value"
ldstr "Object_DynamicInlineCache_Invalidation:<TwoPhaseDummy_M2>:__js_call__:3"
ldsflda int32 <generated-owner>::__jroc_dynamicLookup_42
call DynamicLookupInlineCache.GetItem(object, string, string, int32&)
done:
```

The compiler appends one assembly-private static `Int32` field for every
cache-bearing instruction to the generated
`Jroc.Generated.DynamicLookupSites` type. Field names use the deterministic
metadata field row and are implementation details; the string site key remains
the identity of the realm-owned dictionary entry. Assembly visibility permits
access from every generated method without exposing these fields as a public
runtime contract.

The compiler builds the instruction-index map with reference equality. Two
structurally identical LIR instructions at different positions therefore
receive different keys, while alternate emission paths for the same
instruction object retain the same logical site. Requesting a key for an
instruction that is not in the method body is a compilation error rather than
silently sharing or inventing a site.

The LIR index is not a JavaScript source location, IL byte offset, or durable
identifier across recompilation. Optimization can reorder or remove LIR, so a
recompiled assembly may assign different keys. Cache state is ephemeral and
realm-owned, so keys need only be stable and unique within the executing
compiled artifact.

The key does not include an assembly identity or method signature. Generated
cache-bearing methods currently have unique names within their generated
types, but two separately compiled assemblies with the same logical module,
type, method, and LIR index can address the same realm-owned site if a host
executes both in one realm. Entry validation still prevents an incorrect value
from being returned, but the sites can share transition history and become
polymorphic or megamorphic earlier than intended. A future key representation
should remove this performance-level collision as well as string lookup cost.

The property or method name is passed separately and is not part of the site
key. Member calls have a literal method name, while a string-key `LIRGetItem`
may produce different names on different executions. Cache entries validate
the property name in addition to the receiver's shape; several names observed
at one computed-property site can therefore consume separate polymorphic
entries even when every receiver shares one shape.

While the generated terminal field is zero, the key literal is used with
ordinal comparison in the current realm's
`ConcurrentDictionary<string, DynamicLookupInlineCacheSite>`. The first
cacheable observation creates the site; later executions of the same generated
instruction in that realm reuse it. The identical key literal in another
realm addresses a different dictionary, preventing cross-realm state sharing.
Realm disposal clears the dictionary.

To avoid a concurrent-dictionary probe on every nonterminal hit, each runtime
thread keeps the two most recently used `(realm cache state, site key, site)`
tuples plus an eight-entry recent-site ring. Reference identity is checked
before ordinal key comparison. The realm dictionary remains authoritative;
the front cache only retains already-published site objects and is cleared
whenever the ambient execution context changes. Benchmark-only site removal
also removes matching recent entries; generated terminal fields remain
monotonic.

When a fifth distinct live shape/property pair makes a site megamorphic, the
runtime publishes `1` to the generated field with `Volatile.Write`. Generated
code tests the field before loading the site key or entering the cache helper.
A nonzero field branches directly to the same `ObjectRuntime.GetItem`,
`ObjectRuntime.CallMember0`, or `ObjectRuntime.CallMember1` operation used by
an uncached dynamic lookup.

The field is monotonic for the lifetime of the loaded compiled assembly. It is
intentionally not realm-tagged: a transition in one realm can cause another
realm executing the same compiled artifact to choose generic lookup, but
generic lookup is always semantically valid and the second realm neither reads
nor mutates the first realm's cache site. This bounded, value-free
cross-realm deoptimization hint avoids retaining a realm and makes the terminal
path equivalent to generated generic lookup. Realm-owned entries, snapshots,
invalidation versions, and transition history remain isolated.

The key is created by the compiler and emitted with `ldstr`; it is not
formatted or allocated on each execution. Runtime lookup still hashes and
compares the string. Issue #1955 tracks replacing this representation with
integer site IDs or realm-owned per-module arrays. Any replacement must
preserve module namespacing, distinct LIR call sites, realm isolation, and
collectible-assembly lifetime behavior.

Caching is deliberately narrow. Property reads and `CallMember0` retain the
Tier 3 Phase 3 contract: only a receiver whose exact CLR type is `JsObject`,
and only an **own** property
resolved to a plain default-attributed data slot (writable, enumerable,
configurable, no accessor), is eligible. `JsObject.TryGetOwnPlainDataSlot`
(used to build an entry) and `JsObject.TryGetOwnPlainDataSlotValue` (used on
every hit) enforce this: they reject missing properties, inherited/prototype
properties, accessors, attribute-bearing data descriptors, encoded symbol
keys, and objects marked `HasSharedIntrinsicBaseline` (intrinsic prototype
objects whose descriptor storage is overlaid outside the inline
shape/property arrays). Phase 4 `CallMember1` additionally admits exact
`Array` receivers and a method found as a plain data slot on the receiver's
direct exact-`JsObject` prototype. It does not walk or cache deeper prototype
chains. Typed arrays, proxies, host objects, primitive receivers, encoded
symbols, accessors, and other exotic representations use the existing generic
lookup. AOT direct and realm-guarded calls are unchanged because they no longer
reach these dynamic LIR instructions.

## Validity contract

An entry records:

- a weak reference to the receiver's `JsShape` at resolution time;
- the property name;
- the resolved slot index within that shape.

For a direct-prototype `CallMember1` entry, the entry instead records weak
references to the direct prototype object and resolved callable plus the
prototype's `LookupVersion`. It does not record a receiver or a prototype
chain snapshot. Every hit re-validates live state:

1. The entry's weakly held `JsShape` must still be alive
   (`WeakReference<JsShape>.TryGetTarget`).
2. The receiver's *current* shape must be reference-equal to that shape
   (`JsObject.Shape`). Adding, deleting, or otherwise changing the set of own
   properties always transitions an object to a different `JsShape` instance
   (`JsShape.TransitionTo`/`TransitionAway`), so a shape change alone is
   sufficient to miss.
3. The receiver must not be `HasSharedIntrinsicBaseline`.
4. The resolved slot's per-slot descriptor flags
   (`JsObject`'s inline `JsSlotDescriptorFlags`) must still be `None` — i.e.
   still a plain writable/enumerable/configurable data property. A
   `defineProperty` call that narrows attributes or converts the slot to an
   accessor sets non-`None` flags on that slot even when the shape itself is
   otherwise unchanged (for example, redefining an existing own property in
   place does not add or remove a property name), so this check independently
   guards against descriptor-only mutation that shape identity alone would
   miss.

For a prototype entry, the receiver's current direct prototype must also be
reference-equal to the weakly held prototype and that prototype's current
`LookupVersion` must equal the recorded version. Any prototype property write,
deletion, descriptor change, or prototype mutation increments that version;
the miss path then re-resolves the property and records the new callable only
when it is still a writable/enumerable/configurable own data descriptor. The
weak callable can therefore be invoked without rereading descriptor storage.
Receiver-shape and direct-prototype guards cover own shadowing and
`Object.setPrototypeOf`. Own-slot entries continue to read `_properties[slot]`
live, so plain own-value writes are visible without rebuilding the entry.

Because entries never hold a receiver reference, cache reuse across receivers
is a natural consequence of shape sharing: `JsShape.TransitionTo` caches child
shapes by property name on the parent shape, so any two `JsObject` instances
built by adding the same own property names in the same order (starting from
the shared empty root shape) resolve to the exact same `JsShape` reference.
Thousands of otherwise-unrelated instances that each have their own value in a
same-named own data slot (the `GraphNode.pos` motivating scenario) therefore
share one monomorphic entry, and each hit still returns that specific
receiver's own current value.

Property reads and `CallMember0` still cache only own properties. The only
prototype entry is the Phase 4 direct-prototype `CallMember1` form above;
deeper inherited lookup always runs through the generic algorithm.

Entries weakly reference shapes and, for direct-prototype calls, the prototype
object and callable. A live realm cache therefore does
not keep otherwise unreachable JavaScript objects (receivers, values, or
shapes) alive. If the weak shape target has been collected, the entry misses
and generic resolution runs again. Miss updates remove collected-shape entries
before applying the polymorphic limit, so dead entries do not force a site
into megamorphic state.

## State transitions and concurrency

A new realm-owned site starts empty, becomes monomorphic after one observed
(shape, property) pair, and supports up to four distinct pairs as a small
polymorphic cache. A fifth distinct pair permanently transitions the site to
megamorphic state and releases its entries. The transition also publishes the
generated terminal flag. Subsequent executions dispatch directly to generic
lookup without discovering the ambient realm, hashing the site key, reading
the site snapshot, or constructing discarded cache entries.

Readers use an immutable published snapshot and do not lock. Miss updates are
serialized by a site-local lock and publish a replacement snapshot. Monomorphic
and polymorphic hits allocate no memory. The generated flag uses volatile
publication. A racing execution can enter the cache helper once more, but it
will observe the realm site's terminal snapshot and still execute generic
lookup; no race can return a stale cached value.

Every miss, missing-slot, accessor, symbol-key, shared-intrinsic-baseline,
unsupported inherited lookup, proxy, primitive, unsupported exotic receiver,
and megamorphic site executes the existing generic `ObjectRuntime` operation.
Member-call helpers additionally validate callability before caching or
invoking a resolved value, preserve the original receiver as JavaScript
`this`, and use fixed-arity `CallableOperations.Call0`/`Call1`.

## Cache size and lifetime policy

The cache policy follows the principle that an infinite cache is another name
for a memory leak. This prototype bounds entries at each site, uses weak
references for JavaScript object graphs, and gives the realm an explicit
bulk-disposal boundary. It does not use LRU, LFU, time-to-live, or
process-global eviction.

| Layer | Size policy | Lifetime and release policy |
| --- | --- | --- |
| Generated terminal fields | Exactly one four-byte logical flag per emitted cache-bearing property read or zero/one-argument member call, plus normal CLR metadata/alignment overhead. | The zero-initialized flag changes monotonically to one after a megamorphic transition and remains for the loaded assembly's lifetime. It contains no receiver, value, site key, cache object, or realm reference. |
| Thread-local recent sites | Two MRU slots plus an eight-entry ring per thread. Entries are keyed by realm-cache identity and ordinal site key. | All slots are discarded on every ambient execution-context transition. They are a lookup front end only; the realm dictionary remains authoritative. |
| Realm site dictionary | One site for each distinct generated site key that produces at least one cacheable own-slot or direct-prototype-call result. Uncacheable-only operations do not create sites. | The dictionary strongly retains each key and site until realm disposal. Individual sites are not currently removed. |
| Cache site | At most four live monomorphic/polymorphic entries. | A fifth distinct live (shape, property) pair changes the site permanently to megamorphic state and releases all entries. The site object and key remain in the realm dictionary so later calls can take generic fallback directly. |
| Cache entry | An own entry has one weak `JsShape`, a property name, and one slot. A direct-prototype call entry has weak receiver-shape, prototype-object, and callable references plus one lookup version. | Weak references keep receiver shapes, prototypes, and callables collectible; the entry never references a receiver. The property name and scalar guards remain strongly held while the entry remains in the current site snapshot. |
| Published snapshot | One immutable entry array visible to new readers. | A miss update atomically publishes a replacement. An older snapshot remains alive only while an in-flight reader still references it, then becomes collectible. |

A cache entry is created only after exact generic resolution finds a
cacheable own plain-data slot. Its subsequent lifetime is:

1. Valid hits reuse the entry without allocation, always reading the current
   receiver's live slot value.
2. A structural change (add/delete of an own property, or a `defineProperty`
   call that changes attributes or converts the slot to/from an accessor)
   changes the receiver's shape and/or per-slot descriptor flags. The old
   entry (keyed to the prior shape) simply no longer matches that receiver;
   there is no version counter to advance.
3. If the same (shape, property) pair later resolves again to a cacheable own
   plain-data slot — for example after a delete-then-re-add restores a
   matching shape — the miss update replaces or reuses the matching entry. If
   lookup remains missing, accessor-based, or otherwise uncacheable, generic
   fallback continues.
4. If a shape becomes unreachable (every receiver sharing it has been
   collected), its weak entry is removed during the next cacheable miss
   update at that site. There is no background sweep, so a site that is
   never touched again can retain the entry's small fixed metadata, but not
   the shape, any receiver, or any value.
5. A megamorphic transition drops the complete entry array and is
   irreversible for the lifetime of that realm-owned site.

The per-site entry limit prevents a highly variable call site from retaining
an ever-growing receiver history. Weak references prevent even the four
retained entries from extending JavaScript object lifetimes. Permanent
megamorphic fallback also avoids repeatedly allocating entries that the site
would immediately discard.

The realm dictionary itself is not numerically capped. For a realm executing a
fixed compiled application, its maximum site count is bounded by the number of
cache-bearing generated instructions that become cacheable. A long-lived host
that continually executes new compiled assemblies with new site keys can grow
the dictionary until the realm is disposed. That lifecycle is an explicit
remaining constraint, not an LRU policy. Future per-module tables considered
by #1955 should provide a module/assembly release boundary while preserving
collectible-assembly behavior.

## Realm lifecycle

Each realm owns its site dictionary. Identical site keys cannot share entries
across realms, and disposing a realm clears the dictionary. The weak entry
references also allow receivers and values to collect while their realm
remains active. Thread-local recent-site references are cleared on every
ambient execution-context transition, so they cannot keep a departed realm
alive or carry a site into another realm.

The terminal field follows compiled-assembly lifetime rather than realm
lifetime. It is safe after realm disposal because it stores only an integer
deoptimization marker. A later realm can take generic fallback immediately;
it cannot observe any disposed realm object or value through that marker.

## Benchmark

Run the focused .NET 10 ShortRun benchmark with:

```bash
dotnet run -c Release \
  --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --dynamic-inline-caches --filter "*"
```

The 2026-08-21 Phase 1 local run used BenchmarkDotNet 0.15.8 on Ubuntu 24.04.4,
.NET 10.0.11, and an Intel Xeon 6975P-C. Each row has three measurements.

| Operation | Mean | Allocated |
| --- | ---: | ---: |
| Generic ordinary property read | 28.41 ns | 0 B |
| Monomorphic property hit | 10.01 ns | 0 B |
| Four-way polymorphic property hit | 13.89 ns | 0 B |
| Generic zero-argument member call | 51.83 ns | 0 B |
| Cached zero-argument member call | 18.87 ns | 0 B |
| Generated terminal generic fallback | 25.39 ns | 0 B |
| Property mutation and invalidation | 680.05 ns | 520 B |
| Cold miss with explicit site removal | 1,039.10 ns | 624 B |
| String generic / cache-stub fallback | 29.10 / 30.08 ns | 24 / 24 B |
| Array generic / cache-stub fallback | 14.38 / 14.96 ns | 24 / 24 B |
| Boxed-then-consumed / direct numeric Array length | 14.68 / 0.65 ns | 24 / 0 B |

The Phase 0 guardrail for #1958 added two explicit numeric Array-length controls:
`ArrayLengthBoxedThenConsumed` performs the current generic property read before
numeric conversion, while `ArrayLengthDirectNumber` reads the runtime Array's
numeric length directly. The pair makes the generic path's 24 B/op boxing
visible even though both benchmark methods return `double`.

Compared with the Phase 0 run on the same host, monomorphic and four-way
polymorphic hits improved from 21.00 ns and 24.53 ns respectively. The
generated terminal path was no slower than the direct generic property-read
control in this run. The allocation profile is unchanged: steady hits and
terminal fallback allocate zero bytes, while descriptor-aware primitive reads
still expose the existing 24 B allocation.

The cold-miss control includes removal of the site from the realm dictionary,
so it is an upper bound rather than pure descriptor-resolution cost. The
invalidation row includes the property mutation. String and Array are fallback
controls, not cache-hit cases. Their small differences in this three-sample
run reinforce that broad expansion must continue to be evaluated against
receiver analysis rather than inferred from one microbenchmark.

`CachedPropertyInvalidation`'s 680.05 ns / 520 B row above predates Phase 3
and reflects the earlier identity-keyed cache, where every plain value write
to an already-cached slot forced a version-based rebuild. Under the Phase 3
shape-keyed cache the same benchmark body — a plain write to an existing own
slot on an unchanged shape — is a genuine cache hit on every iteration (see
"Validity contract" above), so this specific historical
number is expected to drop substantially and is no longer representative of
current behavior. Refreshing the full dated table with a new Phase 3
measurement run is left to the tracking issue rather than this change. Phase 3
also added `CachedPropertyHit_SameShapeAcrossInstances`, which cycles through
thousands of distinct same-shape receivers at one site to demonstrate the
cross-instance monomorphic sharing this phase introduces.

## Staged plan

The original decision was to proceed with a conservative identity-based first
stage, while limiting further expansion to measured cases:

1. Retain exact-identity caching for ordinary-object property reads and
   `CallMember0`, and collect end-to-end profiles for sites that remain after
   AOT receiver analysis.
2. ~~Add a descriptor/shape generation contract before sharing entries among
   different same-shape objects. `JsShape` identity alone is insufficient
   because value and descriptor changes can preserve shape.~~ **Done in
   Phase 3 (#1958/#1324):** property-read and `CallMember0` entries are now
   shape-keyed with a live per-slot descriptor-flag re-check on every hit (see
   "Validity contract" above), restricted to own plain-data slots. Member
   calls beyond `CallMember0` and AOT-compiled classes remain out of scope for
   this phase.
3. **Done for `CallMember1` in Phase 4 (#1963):** generated arity-1 calls use
   the shape-keyed own/direct-prototype cache and fixed-arity invocation.
   Measurements should decide whether `CallMember2..5` merit separate stubs.
4. Consider exotics or primitive families only with dedicated validity
   contracts; never generalize ordinary-object entries across proxies,
   accessors, realm prototypes, or boxing boundaries.
