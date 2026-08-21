# Dynamic lookup inline caches

Issue #1893 introduces a conservative per-call-site cache prototype for
dynamic property reads and zero-argument member calls that remain after AOT
specialization. The prototype targets repeated access to ordinary user
objects without weakening JavaScript lookup semantics.

## Terminology

The terms below describe this implementation and may be narrower than their
general use in JavaScript engines.

| Term | Meaning in this feature |
| --- | --- |
| **AOT specialization** | A compile-time decision that replaces dynamic lookup with a direct or guarded operation. Inline caches apply only after AOT analysis cannot prove such a specialization. |
| **Call site** | One generated property-read or member-call instruction. Two instructions that read the same property are separate call sites. |
| **Cache site** | The realm-owned runtime state associated with one generated call site. Its site key identifies the module, generated type, method, and original LIR instruction. |
| **Receiver** | The JavaScript value to the left of the property access, such as `record` in `record.name` or `record.save()`. |
| **Dynamic lookup** | Resolving a property or callable at runtime because the compiler cannot determine the result safely in advance. |
| **Inline cache** | Per-call-site runtime state that reuses a previously resolved data property when its recorded assumptions remain valid. “Inline” refers to association with the generated call site; the cache is stored in the realm rather than embedded as mutable process-global state in generated code. |
| **Cache entry** | One recorded exact receiver/property identity, resolved value, prototype path, and set of lookup versions. |
| **Empty** | A cache site that has not recorded a usable receiver/property identity. |
| **Monomorphic** | A site containing one live cache entry. In this first-stage identity cache, repeated access must use the same receiver object; another object with the same shape is still a distinct identity. |
| **Polymorphic** | A site containing two through four live cache entries, allowing a small set of exact receiver identities to hit the same generated site. |
| **Megamorphic** | A site that has observed more receiver identities than its four-entry limit. It permanently stops recording entries and uses generic lookup. |
| **Cache hit** | The receiver, property name, weak references, and every recorded `LookupVersion` still match, so the cached value can be reused. |
| **Cache miss** | No valid entry matches. The runtime performs exact generic lookup and may record a new entry when the result is cacheable. |
| **Invalidation** | A property, descriptor, or prototype change advances a recorded version, making an existing entry stale and therefore unable to hit. |
| **Generic fallback** | The existing `ObjectRuntime` lookup or call path, which preserves complete JavaScript behavior for misses and cases the cache deliberately excludes. |
| **Ordinary object** | In this stage, an object whose exact CLR representation is `JsObject` and whose traversed prototype objects are also exact `JsObject` instances. |
| **Exotic object** | A representation with specialized property behavior, such as a proxy, array, typed array, or host object. These bypass this cache prototype. |
| **Realm** | The JavaScript execution environment that owns intrinsic objects and this feature’s cache-site dictionary. Cache state is never shared between realms. |

## Scope and site identity

The compiler routes materialized string-key `LIRGetItem` instructions and
dynamic `LIRCallMember0` instructions through
`DynamicLookupInlineCache.GetItem` and `CallMember0`.

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

For example, a generated property read currently contains IL equivalent to:

```text
ldarg.1
ldstr "value"
ldstr "Object_DynamicInlineCache_Invalidation:<TwoPhaseDummy_M2>:__js_call__:3"
call DynamicLookupInlineCache.GetItem(object, string, string)
```

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
key. `CallMember0` has a literal method name, while a string-key `LIRGetItem`
may produce different names on different executions. Cache entries validate
the property name in addition to the receiver identity; several names observed
at one computed-property site can therefore consume separate polymorphic
entries.

At runtime, the generated key literal is used with ordinal comparison in the
current realm's
`ConcurrentDictionary<string, DynamicLookupInlineCacheSite>`. The first
cacheable observation creates the site; later executions of the same generated
instruction in that realm reuse it. The identical key literal in another
realm addresses a different dictionary, preventing cross-realm state sharing.
Realm disposal clears the dictionary.

The key is created by the compiler and emitted with `ldstr`; it is not
formatted or allocated on each execution. Runtime lookup still hashes and
compares the string. Issue #1955 tracks replacing this representation with
integer site IDs or realm-owned per-module arrays. Any replacement must
preserve module namespacing, distinct LIR call sites, realm isolation, and
collectible-assembly lifetime behavior.

The first stage deliberately caches only receivers whose exact CLR type is
`JsObject` and whose complete prototype chain also consists of exact
`JsObject` instances. Arrays, typed arrays, proxies, host objects, primitive
receivers, and other exotic representations use the existing generic lookup.
AOT direct and realm-guarded calls are unchanged because they no longer reach
these dynamic LIR instructions.

## Validity contract

An entry records:

- the exact receiver identity and property name;
- the resolved data-property value;
- every ordinary object from the receiver through the resolving prototype;
- each object's `LookupVersion` at resolution time.

`LookupVersion` advances when an ordinary object's observable lookup result
can change, including value writes, descriptor definition or reconfiguration,
deletion, descriptor-store overlays, property reset/clear, and prototype-link
changes. A hit requires the same live receiver, the same property name, and an
unchanged version for every recorded chain object.

The resolver caches only a data descriptor that it actually finds. Missing
properties and accessors always use generic lookup. This preserves getter
execution and runtime fallbacks such as legacy `__proto__`. Shadowing a
prototype property, deleting an own property, replacing a value, changing a
data property into an accessor, or replacing any prototype link invalidates
the old entry before reuse.

Entries weakly reference receivers, resolved object values, and prototype
objects. A live realm cache therefore does not keep otherwise unreachable
JavaScript objects alive. If a weak target has been collected, the entry
misses and generic resolution runs again. Miss updates remove collected
receivers before applying the polymorphic limit, so dead entries do not force
a site into megamorphic state.

## State transitions and concurrency

A new site starts empty, becomes monomorphic after one observed identity, and
supports up to four identities as a small polymorphic cache. A fifth distinct
identity permanently transitions the site to megamorphic state and releases
its entries. Megamorphic sites dispatch directly to generic lookup without
constructing discarded cache entries.

Readers use an immutable published snapshot and do not lock. Miss updates are
serialized by a site-local lock and publish a replacement snapshot. Monomorphic
and polymorphic hits allocate no memory.

Every miss, stale entry, accessor, proxy, primitive, exotic receiver, and
megamorphic site executes the existing generic `ObjectRuntime` operation.
`CallMember0` additionally validates callability before caching or invoking a
resolved value.

## Cache size and lifetime policy

The cache policy follows the principle that an infinite cache is another name
for a memory leak. This prototype bounds receiver entries at each site, uses
weak references for JavaScript object graphs, and gives the realm an explicit
bulk-disposal boundary. It does not use LRU, LFU, time-to-live, or
process-global eviction.

| Layer | Size policy | Lifetime and release policy |
| --- | --- | --- |
| Realm site dictionary | One site for each distinct generated site key that produces at least one cacheable ordinary-data-property result. Uncacheable-only operations do not create sites. | The dictionary strongly retains each key and site until realm disposal. Individual sites are not currently removed. |
| Cache site | At most four live monomorphic/polymorphic entries. | A fifth distinct live receiver/property identity changes the site permanently to megamorphic state and releases all entries. The site object and key remain in the realm dictionary so later calls can take generic fallback directly. |
| Cache entry | One exact receiver/property identity. The entry also has arrays proportional to the traversed prototype-chain depth, so four entries is a count bound rather than a fixed byte bound. | The receiver, non-null resolved value, and prototype objects are weakly referenced. The property-name string, weak-reference arrays, and version array remain strongly held while the entry remains in the current site snapshot. |
| Published snapshot | One immutable entry array visible to new readers. | A miss update atomically publishes a replacement. An older snapshot remains alive only while an in-flight reader still references it, then becomes collectible. |

A cache entry is created only after exact generic resolution finds a cacheable
data descriptor. Its subsequent lifetime is:

1. Valid hits reuse the entry without allocation.
2. A descriptor, value, or prototype mutation makes the recorded versions
   stale. The entry immediately stops hitting, but it is not removed merely by
   detecting the stale version.
3. If the same live receiver/property identity later resolves to a cacheable
   data property, the miss update replaces that entry. If lookup remains
   missing, accessor-based, or otherwise uncacheable, generic fallback
   continues and the stale metadata can remain until replacement for that
   identity, megamorphic transition, or realm disposal.
4. If a receiver is collected, its weak entry is removed during the next
   cacheable miss update at that site. There is no background sweep, so a site
   that is never touched again can retain the entry's bounded metadata, but
   not its receiver, value, or prototype graph.
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
remains active.

## Benchmark

Run the focused .NET 10 ShortRun benchmark with:

```bash
dotnet run -c Release \
  --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --dynamic-inline-caches --filter "*"
```

The 2026-08-21 local run used BenchmarkDotNet 0.15.8 on Ubuntu 24.04.4,
.NET 10.0.11, and an Intel Xeon 6975P-C. Each row has three measurements.

| Operation | Mean | Allocated |
| --- | ---: | ---: |
| Generic ordinary property read | 39.72 ns | 0 B |
| Monomorphic property hit | 19.61 ns | 0 B |
| Four-way polymorphic property hit | 22.87 ns | 0 B |
| Generic zero-argument member call | 63.26 ns | 0 B |
| Cached zero-argument member call | 29.40 ns | 0 B |
| Megamorphic generic fallback | 53.30 ns | 0 B |
| Property mutation and invalidation | 543.39 ns | 520 B |
| Cold miss with explicit site removal | 596.01 ns | 624 B |
| String generic / cache-stub fallback | 50.93 / 51.17 ns | 24 / 24 B |
| Array generic / cache-stub fallback | 30.33 / 30.56 ns | 24 / 24 B |

The cold-miss control includes removal of the site from the realm dictionary,
so it is an upper bound rather than pure descriptor-resolution cost. The
invalidation row includes the property mutation. String and Array are fallback
controls, not cache-hit cases. Their small differences in this three-sample
run reinforce that broad expansion must continue to be evaluated against
receiver analysis rather than inferred from one microbenchmark.

## Staged plan

The decision is to proceed with this conservative identity-based first stage,
while limiting further expansion to measured cases:

1. Retain exact-identity caching for ordinary-object property reads and
   `CallMember0`, and collect end-to-end profiles for sites that remain after
   AOT receiver analysis.
2. Add a descriptor/shape generation contract before sharing entries among
   different same-shape objects. `JsShape` identity alone is insufficient
   because value and descriptor changes can preserve shape.
3. Use profile and receiver-analysis evidence to gate cache emission and to
   decide whether `CallMember1..5` merit additional stubs.
4. Consider exotics or primitive families only with dedicated validity
   contracts; never generalize ordinary-object entries across proxies,
   accessors, realm prototypes, or boxing boundaries.
