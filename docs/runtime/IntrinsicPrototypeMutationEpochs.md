# Intrinsic prototype mutation epochs

Generated specialization must never bypass observable JavaScript property
lookup based only on a receiver's static type. Issue #1892 adds realm-owned
mutation epochs that generated code can use to validate assumptions about an
intrinsic prototype chain before taking a direct helper path.

## Design

Epochs are per realm and per intrinsic family. The first family is
`IntrinsicPrototypeFamily.String`.

```csharp
if (IntrinsicPrototypeEpochs.IsPristine(
        IntrinsicPrototypeFamily.String))
{
    // Guarded specialization.
}
else
{
    // Full JavaScript property lookup.
}
```

Reads perform one ambient realm lookup and one volatile counter read. They
allocate no memory and are marked for aggressive inlining.

Each epoch is monotonic. Restoring a default descriptor does not make an old
assumption valid again; generated code must continue using its exact fallback
after any relevant mutation.

Only `PristineEpoch` (`0`) proves that the realm still has its default
intrinsic chain. `Read` and `IsCurrent` support diagnostics and future hoisting
work, but generated code must not capture an already-mutated epoch and treat it
as permission to specialize.

## Invalidation

The String family increments when user code mutates:

- any own property or descriptor on `String.prototype`;
- any own property or descriptor on `Object.prototype`, the default ancestor;
- the `[[Prototype]]` link of either intrinsic prototype.

This covers assignment, `Object.defineProperty`, descriptor reconfiguration,
accessor installation, deletion, default callable replacement,
`Object.setPrototypeOf`, `Reflect.setPrototypeOf`, and legacy `__proto__`
mutation because those operations converge on `PropertyDescriptorStore` or
`PrototypeChain`.

Intrinsic bootstrap runs under initialization suppression, so constructing the
default descriptor graph and prototype links leaves the initial epoch stable.
Unrelated object and intrinsic mutations do not invalidate the String family.

`Object.prototype` is a shared ancestor, so its mutations fan out to every
family that depends on it. The current implementation has only the String
family; future families can add their own counters and ancestor dependencies
without introducing a process-wide coarse invalidation token.

## Realm ownership

Counters live directly on `RuntimeIntrinsics`, alongside the realm's intrinsic
objects. No static table retains a realm or prototype. Mutation hooks compare
the target against already-published intrinsic slots and never materialize a
new intrinsic graph merely because an ordinary object changed.

A mutation in one realm therefore cannot change another realm's epoch.
Disposing the realm releases both its intrinsic objects and counters.

## Array tracking

Array's existing process-wide prototype mutation version remains separate.
It drives a thread-local dense-write rescan cache and also reacts to descriptor
store switches; it is not a compiler assumption token. Compiler specialization
should use the realm-owned epoch API when an Array family is added rather than
depending on that legacy cache version.

## Validation

`IntrinsicPrototypeEpochTests` covers:

- stable initialization and unrelated mutations;
- assignment and default callable replacement;
- property definition and descriptor reconfiguration;
- accessor installation and deletion;
- intrinsic and ancestor prototype-link changes;
- cross-realm isolation;
- failed guard validation after mutation;
- zero-allocation epoch reads and validation.

Run the focused microbenchmark with:

```bash
dotnet run -c Release \
  --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --intrinsic-epochs --filter "*"
```

The 2026-08-19 .NET 10 ShortRun measured:

| Operation | Mean | Allocated |
| --- | ---: | ---: |
| Validate pristine String epoch | 5.623 ns | 0 B |
| Read String epoch | 5.776 ns | 0 B |
| Invalidate String epoch | 9.820 ns | 0 B |

The read and invalidation costs support per-family counters: ordinary guarded
reads remain single-digit nanoseconds, while unrelated intrinsic families avoid
coarse invalidation and unnecessary fallback.
