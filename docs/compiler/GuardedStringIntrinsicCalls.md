# Guarded String intrinsic calls

Issue #1891 restores direct AOT calls for common primitive String operations
without bypassing JavaScript prototype semantics. Receiver type alone is not a
valid specialization assumption because user code can replace, delete, or
reconfigure methods on `String.prototype` or its `Object.prototype` ancestor.

## LIR contract

`LIRCallGuardedStringIntrinsic` records:

- the original receiver, member name, arguments, and result;
- the exact fixed-arity `JavaScriptRuntime.String` helper signature;
- whether static analysis already proves a primitive-string receiver;
- any result conversion that both branches must perform.

The instruction is a scheduling boundary because its IL contains internal
control flow.

## Emitted paths

The generated fast path:

1. validates
   `IntrinsicPrototypeEpochs.IsPristine(IntrinsicPrototypeFamily.String)`;
2. performs `isinst string` when the receiver type is uncertain;
3. calls the exact fixed-arity String helper.

The cold path executes the original `ObjectRuntime.CallMember0..5` operation
with the original receiver and arguments. Argument expressions are evaluated
before the guard, so both branches preserve JavaScript evaluation order and
coercion behavior.

The compiler keeps result storage as `object` when an override can return an
arbitrary value. The `charCodeAt` plus `ToNumber` fusion is the current
exception: both branches produce the same numeric result because the fallback
performs ordinary member dispatch and then applies `ToNumber`, so the result
can remain an unboxed `double`.

Stable `substring` calls now pass through this guarded instruction instead of
using their former unconditional HIR-level direct call. The previous
unguarded `charCodeAt` numeric fusion is guarded as well.

## Receiver candidates

The symbol-table fixed point separately records reference receiver candidates
observed at declarations and writes. A candidate is not a CLR storage claim:
conflicting assignments, captured fields, and deferred callbacks can all make
the value at a particular read unknown. Candidates therefore never remove a
runtime check or authorize an unguarded intrinsic call.

When a captured binding has `string` among its candidates, lowering retains the
guarded String path even if its field is otherwise object-typed or has another
specialized CLR classification. The generated `isinst string` check selects
the helper only for a primitive string; every other value, including a boxed
String object, takes the original generic member-call fallback. Candidate sets
also retain Array and typed-array observations for future guarded
specializations, but do not bypass their prototype lookup today.

## Current specialization surface

The initial allowlist matches the previously supported String early-binding
surface:

- arity 0: `charAt`, `charCodeAt`, trim aliases, and case conversion;
- arity 1: character access, substring/slice operations, searches, and prefix
  or suffix checks;
- arity 2: substring/slice operations, searches, and prefix or suffix checks.

A call is specialized only when a fixed-arity helper preserves each argument's
existing representation. Otherwise the original generic call remains.

## Performance

Run the focused benchmark with:

```bash
dotnet run -c Release \
  --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --guarded-string-intrinsics
```

The 2026-08-19 .NET 10 ShortRun measured the post-#1889 residual lookup cost:

| Path | Mean | Allocated |
| --- | ---: | ---: |
| Guarded proven `String.trim` | 7.835 ns | 0 B |
| Guarded uncertain `String.trim` | 8.069 ns | 0 B |
| Generic `CallMember0` `String.trim` | 193.275 ns | 88 B |

The guard removes about 96% of the generic dispatch time in this focused case
and eliminates its steady-state allocation. The uncertain receiver type test
adds about 0.2 ns while retaining the complete generic fallback.

The 2026-08-19 local DefaultJob run of `dromaeo-object-string` on .NET
10.0.11 measured JROC execution at 22.03 ms (N=13) with 44.24 MB allocated
per module load. Compiling that exact fixture before and after receiver
candidate tracking produced the same 22,528-byte module assembly. This is
expected: candidates preserve guarded specialization at object-typed captured
reads rather than turning them into an unguarded direct call.

## Validation

Coverage includes:

- direct-helper and exact-fallback generator snapshots;
- replacement, accessor, deletion, inherited override, and aliased-call
  behavior;
- uncertain receivers that alternate between strings and ordinary objects;
- cross-realm prototype isolation using one compiled module loaded twice;
- guarded numeric fallback conversion;
- zero-allocation guard plus `String.trim` fast-path execution;
- the `dromaeo-object-string` fixture, including captured writes and deferred
  callbacks whose hot calls must keep an `isinst string` plus generic fallback;
- targeted `built-ins/String/prototype/**` test262 tests.

The realm-owned invalidation contract is documented in
[Intrinsic prototype mutation epochs](../runtime/IntrinsicPrototypeMutationEpochs.md).
