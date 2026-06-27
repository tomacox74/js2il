# Typed temp optimization notes

This document explains the typed-temp cleanup work introduced for issue #451.
It is written in two layers:

1. An "explain like I'm 5" section with small examples.
2. A technical section that maps those examples to the LIR and IL compiler code.

The goal is to make it easier for humans to understand why these optimizations
exist, what they are allowed to change, and what they must never change.

## Part 1: Explain like I'm 5

### The toy-box version

Imagine the compiler has a number, like `42`.

Sometimes the compiler puts that number in a plain number box:

```text
42
```

Sometimes it wraps that number in a bigger "anything can go here" box:

```text
object box containing 42
```

That bigger box is useful when JavaScript needs a value that could be anything:
a number, a string, an object, `undefined`, and so on. But if the next thing we
do already expects a number, putting the number into the big box and then taking
it back out is wasted work.

This PR teaches the compiler to notice some of those cases and skip the wasted
box.

### Simple example: passing a number to a method

JavaScript:

```javascript
class Counter {
  add(x) {
    return x + 1;
  }
}

const c = new Counter();
console.log(c.add(41));
```

The method `add` has a parameter `x`. After earlier compiler passes, Jroc may
know that `x` is used as a number. The old path could still do work like this:

```text
make number 41
put 41 into an object box
call Counter.add, which expects a number
take 41 back out as a number
```

The optimized path is:

```text
make number 41
call Counter.add with the number directly
```

The JavaScript behavior is the same, but the generated IL has less boxing and
fewer temporary object locals.

### Simple example: why we cannot always skip the box

JavaScript arguments are evaluated left to right. That means the first argument
must keep the value it had when the first argument was evaluated.

```javascript
let x = 1;
obj.method(x, x = 2);
```

The first argument is `1`. The second argument changes `x` to `2`. The method
must still receive:

```text
first argument: 1
second argument: 2
```

If the compiler is careless and says, "I will just load `x` later when I call
the method," it might accidentally pass:

```text
first argument: 2
second argument: 2
```

That would be wrong.

So the optimization only forwards a typed value when it is safe:

- the value is a constant, parameter, `this`, or another already-safe value;
- or the value lives in a variable slot that cannot change;
- or the value lives in a variable slot that does not get written again between
  the original boxing point and the call.

In other words, the compiler only skips the object box when it can still preserve
the original JavaScript value.

### Simple example: removing an unused object box

Sometimes the compiler used to build an object box, then later no instruction
actually needed it:

```text
make number 10
put 10 into object box tempA
call typed method with number 10 directly
```

After the call was rewritten to use the number directly, `tempA` became dead.
The optimized compiler removes the dead object-box instruction:

```text
make number 10
call typed method with number 10 directly
```

### Simple example: pinned variables are special

Some temps are not just disposable scratch values. They are pinned to a real IL
local that represents an important compiler variable, such as loop state.

For those temps, a "copy" or "convert" instruction can be doing important work
even if it looks unused in a simple temp-use scan. It might be updating the local
that the next loop iteration reads.

This PR is careful not to remove writes to pinned variable slots.

## Part 2: Technical details

### Background: values have storage facts

Jroc's LIR tracks how each temp is represented through `ValueStorage`:

```csharp
public sealed record ValueStorage(
    ValueStorageKind Kind,
    Type? ClrType = null,
    EntityHandle TypeHandle = default,
    string? ScopeName = null);
```

The most relevant storage kinds for this PR are:

- `UnboxedValue`: a CLR value type such as `double` or `bool`.
- `BoxedValue`: a value that has been boxed for object-shaped JavaScript flow.
- `Reference`: a CLR reference type such as `object`, `string`, a runtime type,
  or a generated user-class type handle.
- `Unknown`: storage is not statically known.

Correctness depends on distinguishing "same JavaScript value" from "same runtime
representation." A `double` temp and an `object` temp might both represent the
JavaScript number `42`, but they are not interchangeable at an IL call boundary
unless the boundary expects the representation we are providing.

### `ValueStorageFacts`

This PR adds `ValueStorageFacts` as a small central place for storage
compatibility checks used by normalization passes.

`IsSameRuntimeRepresentation(left, right)` returns true when both storages can
share the same IL local representation:

- same `ValueStorageKind`;
- same CLR type;
- same metadata type handle when either side has one;
- otherwise same scope name.

This is stricter than "can be converted." It is used for cases such as deciding
whether a typed call result can safely retag a temp that is pinned to an existing
variable slot.

`CanFlowTo(source, target)` answers whether a source representation can be used
where a target representation is expected without changing semantics:

- identical runtime representations can flow;
- any known unboxed, boxed, or reference value can flow to `object`;
- reference values can flow to assignable reference targets when both sides use
  CLR types rather than generated metadata handles.

The helper intentionally does not claim that unboxed `double` can flow to
`object` by magic at every boundary. The caller still decides whether a boundary
requires materialization or whether it is trying to avoid materialization.

### Optimization 1: forwarding boxed arguments for typed member calls

`LIRMemberCallNormalization` rewrites generic member calls into typed member
calls when the class registry can prove a unique user-class method target.

Before this PR, a generic call site could carry arguments through an object array
or object-shaped temps even after the call was normalized to a typed method call.
That produced patterns like:

```text
LIRConvertToObject(numberTemp -> boxedTemp)
LIRBuildArray([boxedTemp] -> argsArray)
LIRCallMember(receiver, "method", argsArray -> result)
```

After method resolution, the call can become:

```text
LIRCallTypedMember(receiver, ..., [numberTemp] -> result)
```

when the target parameter is known to be a non-`object` numeric or boolean type
and the original source temp can safely flow to that parameter type.

The relevant helper is:

```csharp
ForwardBoxedArgumentsToTypedSources(...)
```

It checks each call argument and rewrites only when all of these are true:

1. The argument temp is defined by `LIRConvertToObject`.
2. The target parameter type is known.
3. The target parameter type is not `object`.
4. `ValueStorageFacts.CanFlowTo(sourceStorage, targetStorage)` is true.
5. The source temp is stable from the original conversion point through the call.

The non-`object` parameter check matters. If a method parameter is `object`,
forwarding an unboxed source would just move boxing into both the typed-call path
and the fallback path. That can increase boxing rather than reduce it.

### Optimization 2: forwarding boxed arguments for direct user-class calls

`LIRTypeNormalization` also handles direct `LIRCallUserClassInstanceMethod`
instructions. These are already direct calls to generated user-class methods,
but their argument list can still contain object materializations created before
the method signature was known precisely enough.

This PR adds:

```csharp
ForwardBoxedArgumentsForDirectUserClassCalls(...)
```

It performs the same core rewrite as member-call normalization, but for direct
user-class call instructions:

```text
before:
  LIRConvertToObject(sourceNumber -> boxedArg)
  LIRCallUserClassInstanceMethod(..., [boxedArg])

after:
  LIRConvertToObject(sourceNumber -> boxedArg)   // later removed if dead
  LIRCallUserClassInstanceMethod(..., [sourceNumber])
```

The follow-up dead-materialization pass can then remove the conversion if no
remaining instruction operand uses `boxedArg`.

### Stability checks and JavaScript evaluation order

The most important safety rule in this PR is that forwarding must not change
when a value is observed.

`LIRConvertToObject` acts like a snapshot point for the value it boxes. If the
source temp is backed by a mutable variable slot, replacing the boxed temp with
the source temp can accidentally observe a later value.

To prevent that, both normalization passes use a stability check with the
conversion instruction index and call instruction index.

A temp is considered stable when:

- it is pinned to a variable slot listed in `MethodBodyIR.SingleAssignmentSlots`;
- or it is pinned to a variable slot that is not written between the conversion
  instruction and the call instruction;
- or it is defined by a stable definition such as a constant, `LIRLoadParameter`,
  `LIRLoadThis`, or a copy chain whose source is stable.

The range check is deliberately conservative:

```text
convert index < checked instructions < call index
```

If the same variable slot is written in that interval, forwarding is rejected.
When the instruction order is unexpected, the check also rejects forwarding.

This protects patterns where JavaScript argument evaluation or intermediate LIR
instructions mutate the source variable after the object materialization point.

### Optimization 3: removing dead object materializations

Once a typed call uses the original typed temp, the old boxed temp may become
dead. `LIRTypeNormalization.RemoveDeadObjectMaterializations` removes
`LIRConvertToObject` instructions when:

- the result is not pinned to a variable slot; and
- no other instruction operand uses the result temp.

The pinned-slot guard is required because a temp can be both a temp and a write
to a stable IL local. Removing such an instruction can break loop-carried state,
try/finally state, generator state, or other compiler-created variables.

The use scan relies on `TempLocalAllocator.EnumerateUsedTemps`, so this PR also
adds missing tracking for `LIRGetItemAsNumberString`. Without that, a boxed temp
used by string-key item access could be incorrectly deleted.

### Optimization 4: skipping dead IL emission for object conversions

The IL emitter also has a defensive skip for dead `LIRConvertToObject`
instructions:

```csharp
if (GetTempVariableSlot(convertToObject.Result) < 0
    && !IsTempUsedByAnyInstructionOperand(convertToObject.Result, convertToObject))
{
    break;
}
```

This is intentionally narrower than "not used anywhere." It only skips emission
when the conversion result is not mapped to a variable slot. If it is mapped to a
variable slot, emitting the conversion may be the write that keeps that slot up
to date.

This mirrors the LIR-level dead-materialization rule.

### Optimization 5: skipping dead temp copies

`LIRCopyTemp` emission now skips a copy only when:

- the destination is not pinned to a variable slot; and
- the destination is not consumed by another instruction operand.

That avoids object-local churn for dead scratch copies while preserving copies
that are actually writes to compiler variables.

This distinction matters because `TempVariableSlots` can map an SSA temp to a
declared or anonymous variable local. Such a temp may have no later temp operand
uses but still be read through the variable slot by later generated IL.

### Temp allocation and `LIRGetItemAsNumberString`

This PR updates `TempLocalAllocator` in two related ways:

1. `EnumerateUsedTemps` now reports the `Object` and `Index` operands for
   `LIRGetItemAsNumberString`.
2. `TryGetDefinedTemp` now reports its `Result`.

This keeps liveness, dead-temp checks, and stackification decisions consistent
with IL emission. If an instruction emits IL that loads a temp, the allocator
must report that temp as used.

### Why snapshots changed beyond the headline examples

The primary human-readable wins are reductions in `box` instructions in these
snapshots:

- `Prime_SetBitsTrue_LargeStep_OptimizedVsNaive`: `box` 33 -> 30.
- `Math_PrimeJavaScript_SieveSize1000_OnePass_LogsPrimes`: `box` 72 -> 71.
- `Compile_Performance_PrimeJavaScript`: `box` 52 -> 51.
- `Classes_ClassMethod_ForLoop_CallsAnotherMethod`: `box` 5 -> 4.

Other generator snapshots can change because removing or preserving a temp
materialization changes local signatures, local numbering, stackification, or
method body offsets. Those changes are expected when the IL is semantically
equivalent and the relevant execution/generator tests pass.

### Invariants for future changes

Future typed-temp optimizations should preserve these invariants:

1. Do not forward through an object materialization if doing so can observe a
   later value than JavaScript would observe.
2. Do not remove an instruction that writes to a pinned variable slot unless the
   slot write is proven redundant.
3. Do not use `ValueStorageFacts.IsSameRuntimeRepresentation` as a general
   conversion test. It is intentionally stricter.
4. Keep `TempLocalAllocator.EnumerateUsedTemps` and `TryGetDefinedTemp` in sync
   with every instruction shape that IL emission reads or writes.
5. Treat `object` call boundaries differently from typed boundaries. Avoiding a
   box for a typed parameter is good; moving a box to a runtime-dispatch fallback
   can be neutral or worse.
6. Prefer conservative rejection over clever forwarding when instruction order,
   storage facts, or variable-slot mutation is unclear.

### Where to look in code

- `IR/LIR/ValueStorageFacts.cs`: shared storage compatibility helpers.
- `IR/LIR/LIRMemberCallNormalization.cs`: generic member-call to typed-call
  normalization and boxed-argument forwarding.
- `IR/LIR/LIRTypeNormalization.cs`: direct user-class call forwarding and dead
  object-materialization removal.
- `IL/LIRToILCompiler.InstructionEmission.Arithmetic.cs`: defensive skip for
  dead object conversions at IL emission time.
- `IL/LIRToILCompiler.InstructionEmission.TempsAndExceptions.cs`: defensive skip
  for dead non-pinned temp copies.
- `IL/LIRToILCompiler.TempsLocals.cs`: helper methods used by IL emission to
  check temp operand use and variable-slot pinning.
- `IL/TempLocalAllocator.cs`: temp use/def tracking that liveness,
  materialization, and stackification depend on.
