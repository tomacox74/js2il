# Dynamic lookup inline caches

Issue #1893 introduces a conservative per-call-site cache prototype for
dynamic property reads and zero-argument member calls that remain after AOT
specialization. The prototype targets repeated access to ordinary user
objects without weakening JavaScript lookup semantics.

## Scope and site identity

The compiler routes materialized string-key `LIRGetItem` instructions and
dynamic `LIRCallMember0` instructions through
`DynamicLookupInlineCache.GetItem` and `CallMember0`. A site key combines the
module ID, generated type, method, and original LIR instruction index. Cache
sites are stored in `RuntimeRealmValueCacheState`, so the same generated site
executing in another realm receives independent state.

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
