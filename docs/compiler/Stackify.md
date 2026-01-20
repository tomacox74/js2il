# Stackify: Keeping Temps on the IL Evaluation Stack

This document explains how **Stackify** works in JS2IL.

**Goal:** teach you *why* Stackify exists, *what problem it solves*, and *how the algorithm decides* whether a temporary value (a “temp”) can stay on the .NET IL **evaluation stack** instead of being stored into an IL local variable.

---
## Table of Contents

- [Big picture: why Stackify exists](#big-picture-why-stackify-exists)
- [Where Stackify fits in the compiler](#where-stackify-fits-in-the-compiler)
- [Interaction with other optimizations](#interaction-with-other-optimizations)
- [Mental model: "materialized" vs "stackified" temps](#mental-model-materialized-vs-stackified-temps)
- [StackifyResult](#stackifyresult)
- [The Stackify algorithm (step by step)](#the-stackify-algorithm-step-by-step)
- [CanEmitInline: the most important safety check](#canemitinline-the-most-important-safety-check)
- [CanStackifyBetween: keeping the evaluation stack "well-formed"](#canstackifybetween-keeping-the-evaluation-stack-well-formed)
- [Examples: before/after IL (conceptual)](#examples-beforeafter-il-conceptual)
- [How this affects code generation](#how-this-affects-code-generation)
- [Practical guidance when adding new LIR instructions](#practical-guidance-when-adding-new-lir-instructions)
- [Summary](#summary)
- [Developer checklist (when adding/changing LIR)](#developer-checklist-when-addingchanging-lir)

---
## Big picture: why Stackify exists

.NET IL is a **stack machine**: most operations pop values from the **evaluation stack** and push results back.

Example (conceptually):

- `ldc.i4.5` pushes 5
- `ldc.i4.3` pushes 3
- `add` pops 3 and 5, pushes 8

JS2IL’s internal pipelines often make data flow explicit using **temps**:

- A temp represents a value produced by one instruction and consumed by another.
- A straightforward IL backend would implement a temp as an IL local:
  - compute value
  - `stloc temp`
  - later `ldloc temp`

That works, but it can produce unnecessary IL.

### The optimization
If a temp is produced and then immediately consumed, we can often skip `stloc`/`ldloc` and just keep the value on the evaluation stack.

This is what Stackify decides.

---

## Where Stackify fits in the compiler

Stackify operates over the **LIR** (low-level IR) for a method (`MethodBodyIR`).

In [Js2IL/IL/LIRToILCompiler.cs](../../Js2IL/IL/LIRToILCompiler.cs), the compiler does:

1. Various pre-passes (e.g. peephole optimizations)
2. `Stackify.Analyze(MethodBody)`
3. `MarkStackifiableTemps(...)` updates the “materialize temp?” mask
4. `TempLocalAllocator.Allocate(...)` allocates IL locals only for temps that need them

**Key outcome:**
- If Stackify marks a temp as stackable, we treat it as **not materialized**, so no IL local slot is allocated.

---
## Interaction with other optimizations

Stackify is not the only pass that marks temps as non-materialized. Understanding how it interacts with other optimizations helps when debugging unexpected IL output.

### BranchConditionOptimizer

Before Stackify runs, `BranchConditionOptimizer.MarkBranchOnlyComparisonTemps` identifies comparison temps that are:
- Defined by a comparison instruction (e.g., `LIRCompareNumberLessThan`)
- Used **only** as the condition of a conditional branch (`LIRBranchIfTrue`/`LIRBranchIfFalse`)

These temps are marked non-materialized because the IL emitter fuses the comparison directly into a conditional branch instruction (e.g., `blt` instead of separate `clt` + `brtrue`).

### ConsoleLogOptimizer (peephole)

The `ConsoleLogOptimizer` identifies `console.log(singleArg)` patterns and emits them in a stack-only fashion. Temps consumed solely within these sequences are also marked non-materialized via `ComputeStackOnlyMask`.

### Order of operations in LIRToILCompiler

```
1. ConsoleLogOptimizer.ComputeStackOnlyMask  →  marks some temps non-materialized
2. BranchConditionOptimizer.MarkBranchOnlyComparisonTemps  →  marks comparison temps non-materialized
3. Stackify.Analyze  →  identifies additional stackable temps
4. MarkStackifiableTemps  →  applies Stackify results to the mask
5. TempLocalAllocator.Allocate  →  allocates IL locals only for materialized temps
```

All three mechanisms write to the same `peepholeReplaced` (aka "should materialize?") boolean array. A temp needs materialization only if **none** of these passes marks it as skippable.

---
## Mental model: “materialized” vs “stackified” temps

A temp can be handled in two broad ways:

### 1) Materialized temp (stored to an IL local)

A **materialized** temp is one that gets its own slot in the method's local variables table. In IL terms:

- When the temp is **defined**: the compiler computes the value and emits `stloc <slot>` to store it.
- When the temp is **used**: the compiler emits `ldloc <slot>` to reload the value onto the evaluation stack.

#### What "materialized" means concretely

1. **An IL local slot is allocated.** The `TempLocalAllocator` reserves a slot index for this temp. The slot appears in the method's `.locals` signature.

2. **Every definition writes to that slot.** The IL sequence looks like:
   ```il
   ; compute the value (leaves result on stack)
   stloc.s V_3          ; store into local slot 3
   ```

3. **Every use reads from that slot.** The IL sequence looks like:
   ```il
   ldloc.s V_3          ; push value from local slot 3 onto stack
   ; ... consume the value ...
   ```

4. **The value persists across instructions.** Because it lives in a local variable, you can use it multiple times, across control flow, at any later point in the method.

#### When materialization is required

- The temp is used **more than once**.
- There is **control flow** (branches, labels) between definition and use.
- The defining instruction is **not safe to re-emit** (e.g., it has side effects or is expensive).
- The stack shape between def and use is incompatible with leaving the value on the evaluation stack.

### 2) Stackified temp (kept on the evaluation stack)

A **stackified** temp does **not** get an IL local slot. Instead:

- When the temp is **defined**: the compiler computes the value, which naturally lands on the evaluation stack. No `stloc` is emitted.
- When the temp is **used**: the value is still sitting on the stack (or can be trivially re-produced), so no `ldloc` is needed.

#### What "stackified" means concretely

1. **No IL local slot is allocated.** The temp is marked as "non-materialized" via `MarkStackifiableTemps`, so `TempLocalAllocator` skips it.

2. **The definition just leaves the value on the stack:**
   ```il
   ldc.r8 42.0          ; pushes 42.0 — no stloc follows
   ```

3. **The use consumes directly from the stack:**
   ```il
   ; ... 42.0 is already on top of the stack ...
   ret                  ; consumes and returns it
   ```

4. **The value does NOT persist.** Once consumed, it's gone. This only works for single-use temps with no intervening stack disruption.

#### Trade-off

Stackified temps produce smaller, faster IL (fewer instructions, fewer locals). But the analysis must be conservative: if there's any doubt, the temp should be materialized to avoid correctness bugs.

This is safe only if the evaluation stack is still in the expected shape when the temp is consumed.

---

## StackifyResult

Stackify produces a `StackifyResult`:

- `bool[] CanStackify`: index by temp index
- `IsStackable(temp)` returns whether that temp can remain on the stack

Internally, Stackify is conservative: if it’s not sure, it says **no**.

---

## The Stackify algorithm (step by step)

The main entry point is:

- `Stackify.Analyze(MethodBodyIR methodBody)`

### Step 0: trivial case
If there are no temps, return an empty result.

### Step 1: build def-use information (first pass)
Stackify scans `methodBody.Instructions` and records:

- The single defining instruction index for each temp (`defIndex`)
- The defining instruction object itself (`defInstruction`)
- A list of all instruction indices that use the temp (`useIndices[temp]`)

**Why this matters:** stackifying requires exactly one definition and exactly one use.

### Step 2: filter candidates
For each temp, Stackify checks:

1. **Exactly one def and one use**
   - If a temp is used 0 times or 2+ times, it’s not stackable.
2. **Defining instruction can be emitted inline** (`CanEmitInline`)
   - Stackified temps are effectively “re-materialized” by re-emitting their defining instruction.
   - That is only safe for cheap, side-effect-free instructions.
3. **Use comes after definition**
4. **The value can safely stay on the stack** between def and use
   - This is checked by `CanStackifyBetween(...)`.

If all checks pass, `canStackify[tempIdx] = true`.

---

## CanEmitInline: the most important safety check

When an IL emitter “loads” a temp that is not materialized, it can’t `ldloc` because there is no local.

So it **re-emits the defining instruction** to reproduce the value.

That is safe only if:

- Re-emitting does not cause **side effects**
- Re-emitting does not cause **meaningful extra cost**

In `Stackify.cs`, `CanEmitInline` currently allows:

- Constants: `LIRConstNumber`, `LIRConstString`, `LIRConstBoolean`, `LIRConstUndefined`, `LIRConstNull`
- Parameter loads: `LIRLoadParameter`
- `LIRConvertToObject` — with a **recursive check** (see below)

### The `LIRConvertToObject` recursive check

`LIRConvertToObject` boxes a value (e.g., converts a number to `System.Object`). By itself, boxing is cheap and side-effect free.

However, to re-emit the box, you must first re-emit the value being boxed. So `CanEmitInline` recursively checks whether the **source temp's defining instruction** is also inline-emittable.

Example:

```
t0 = const 42          ; inline-emittable (constant)
t1 = box t0            ; inline-emittable because t0 is inline-emittable
```

But:

```
t0 = add_dynamic a, b  ; NOT inline-emittable (binary op)
t1 = box t0            ; NOT inline-emittable (source is not safe)
```

This prevents accidentally re-computing an expensive operation just to box its result.

### Why binary ops are NOT inline-emittable
Binary ops like `LIRAddDynamic` are deliberately excluded.

Re-emitting a binary op can accidentally repeat work.

#### Real bug this prevents (from StackifyTests)
Consider JavaScript:

```js
return "Hello, " + name + "!";
```

LIR (simplified):

- `t0 = "Hello, "`
- `t1 = ldarg name`
- `t2 = t0 + t1`          // computes "Hello, " + name
- `t3 = "!"`
- `t4 = t2 + t3`          // uses t2
- `return t4`

If `t2` were stackified and the emitter re-emitted its defining instruction, it could compute `"Hello, " + name` more than once.

The test `Analyze_BinaryOperationResult_NotStackable` enforces this: binary op results must not be stackable.

---

## CanStackifyBetween: keeping the evaluation stack “well-formed”

Even if a temp is single-use and inline-emittable, it’s only safe to keep it on the stack if the stack shape between the def and use is safe.

### Special case: directly adjacent def/use
If the use instruction is immediately after the definition, Stackify usually accepts.

But it adds an extra operand-order guard:

- It checks that the temp is the **first operand consumed** by the use instruction.

Why? Because operand order matters for stack machines.

For a binary op, the usual pattern is:

1. push left
2. push right
3. opcode consumes right then left

So if you just pushed `t1` and then immediately perform `add(t0, t1)`, `t1` is on top of the stack and is consumed correctly.

**Example (from `Analyze_SingleUseConstantImmediatelyConsumed_IsStackable`):**

Instructions:

- `t0 = const 5`
- `t1 = const 3`
- `t2 = add t0, t1`

At runtime stack (conceptual):

- after `t1 = const 3`: stack top is `3`
- then evaluating `add t0, t1`: push `t0` (5) then push `t1` (3), then `add`

`t1` is “closest” and safe to consume.

### Control flow barrier
Stackify rejects stackification if there’s any control flow instruction between def and use:

- `LIRLabel`
- `LIRBranch`
- `LIRBranchIfFalse`
- `LIRBranchIfTrue`

Why? Control flow changes which instructions execute; you cannot assume your temp will still be on the stack along every path.

### Stack discipline simulation
For non-adjacent cases, Stackify simulates a simplified stack model.

It assumes:

- right after the temp def, the stack contains exactly the temp (depth = 1)
- each intervening instruction has a `(pops, pushes)` effect

The method `GetStackEffect(...)` provides those numbers per instruction type.

Stackify checks that no intervening instruction would “pop past” the target temp.

In other words: you can push other values above the temp, as long as they get consumed before you reach the use.

### Final operand-position check
At the use instruction, Stackify finds which operand index matches the temp.

It then applies a conservative rule:

- if the temp isn’t at the top of the simulated stack, Stackify only allows it when it is used as the “first operand” in the use instruction

This is intentionally conservative because getting operand ordering wrong is a classic source of stack bugs.

---

## Examples: before/after IL (conceptual)

These examples are simplified to show the idea.

### Example A: constant returned immediately (stackified)
LIR:

- `t0 = const 42`
- `return t0`

Materialized IL might look like:

```il
ldc.r8 42
stloc.s V_0
ldloc.s V_0
ret
```

Stackified IL can be:

```il
ldc.r8 42
ret
```

This is what `Analyze_ReturnImmediatelyAfterConst_IsStackable` is testing.

### Example B: control flow in between (NOT stackified)
LIR:

- `t0 = const 5`
- `label L1`
- `t1 = const 3`
- `t2 = add t0, t1`

Even though `t0` is used once, there is a label between def and use.

Stackify rejects it (`Analyze_TempWithControlFlowBetweenDefAndUse_NotStackable`).

### Example C: value used twice (NOT stackified)
LIR:

- `t0 = const 5`
- `t1 = add t0, t0`

`t0` has **two uses**, so Stackify rejects it (`Analyze_TempUsedMultipleTimes_NotStackable`).

### Example D: the “don’t inline binary ops” rule
From the bug reproduction test:

- `t2 = add_dynamic t0, t1`  // string concat step
- `t4 = add_dynamic t2, t3`

If `t2` were stackified, the emitter could re-run `add_dynamic t0, t1` when it needs `t2` again.

So Stackify says: **no**. (`Analyze_BinaryOperationResult_NotStackable`)

---

## How this affects code generation

At emission time, temps behave differently based on whether they are materialized.

In `LIRToILCompiler`, one important place is:

- `EmitStoreTemp(...)`

If a temp is not materialized:

- the compiler emits `pop` instead of storing it

That only works if:

- the temp’s value is not meant to persist as a local (because it will be re-created inline)
- stack usage stays consistent

This is why Stackify must be strict.

---

## Practical guidance when adding new LIR instructions

If you add a new LIR instruction, you may need to update Stackify in two places:

1. **`GetStackEffect`**
   - If Stackify doesn’t understand the stack effect, it defaults to `(0, 0)`, which can make the analysis wrong.
2. **`CanEmitInline`**
   - Only add instructions here if they are safe to re-emit.
   - Avoid adding anything with side effects, non-trivial cost, allocation, or dependence on mutable state.

Rule of thumb:

- “Would it be okay if this instruction ran twice?”
  - If not, don’t mark it inline-emittable.

---

## Summary

Stackify is a conservative optimization that reduces IL locals by letting some temps stay on the evaluation stack.

A temp is stackable only if:

- exactly one def, exactly one use
- its defining instruction is safe and cheap to re-emit (`CanEmitInline`)
- there is no control flow between def and use
- the simulated stack behavior never pops past the temp
- operand order at the use site is safe

If you understand those rules, you can reason about why a temp was (or wasn’t) stackified and extend the system safely.

---

## Developer checklist (when adding/changing LIR)

Use this as a quick “did I touch all the right places?” list.

### 1) Decide whether the instruction is safe to inline

Stackify only marks a temp stackable if its defining instruction returns `true` from `CanEmitInline`.

- Is the instruction **side-effect free**?
   - If it mutates state, performs I/O, throws intentionally, allocates significantly, or depends on mutable global/runtime state, treat it as **not inline-emittable**.
- Is it **cheap enough** to potentially run more than once?
   - Remember: a stackified temp can be “loaded” by **re-emitting its defining instruction**.
- Would it be correct if the instruction ran twice?
   - If the answer is “no”, do not add it to `CanEmitInline`.

Practical rule: most “pure loads” (constants, argument loads) are safe; most “real computations” (binary ops, calls, object creation) are not.

### 2) Teach Stackify the stack effect

If the new instruction can appear between a temp’s def and use, `CanStackifyBetween` needs an accurate stack model. That model comes from `GetStackEffect`.

- Add a case to `GetStackEffect` for your instruction.
- Think in terms of evaluation stack behavior:
   - **Pops**: how many values does the emitted IL consume?
   - **Pushes**: how many values does it leave on the stack?
> ⚠️ **Warning: missing cases default to `(0, 0)`**
>
> If you forget to add a case for your new instruction, `GetStackEffect` returns `(0, 0)` by default. This can cause **incorrect analysis**:
> - If the instruction actually pushes a value, Stackify won't account for it, potentially allowing unsafe stackification.
> - If the instruction pops values, Stackify won't know and may think the target temp is still accessible when it's been popped off.
>
> Always add explicit cases for new instructions.
Example template:

```csharp
// Example: unary op (consume 1, produce 1)
case LIRMyUnaryOp:
      return (1, 1);

// Example: binary op (consume 2, produce 1)
case LIRMyBinaryOp:
      return (2, 1);
```

If you’re unsure, be conservative: prefer rejecting stackification (by returning a stack effect that prevents it) over allowing an incorrect one.

### 3) Check for control-flow sensitivity

Stackify refuses to keep temps on the stack across control flow (`LIRLabel`/branches). If your new instruction behaves like control flow or can alter execution order, ensure it’s treated as a control-flow barrier.

### 4) Add/adjust tests

`Js2IL.Tests/StackifyTests.cs` is the best place to lock in expected behavior.

- Add a test for “should stackify” when it’s obviously safe.
- Add a test for “must not stackify” for:
   - multiple uses,
   - control flow between def/use,
   - expensive/recomputable operations.

If you ever add something to `CanEmitInline`, add a test that would fail if the defining instruction were re-emitted multiple times (similar to the string-concat bug test).

### 5) Sanity-check codegen behavior

Even if Stackify’s analysis says “stackable”, emission still needs to be correct.

- Verify that stackified temps are marked non-materialized via `MarkStackifiableTemps` in `LIRToILCompiler`.
- Confirm that non-materialized temps don’t get IL locals allocated (`TempLocalAllocator.Allocate`).
- If you see unexpected `pop` emissions, it often means a temp got marked non-materialized but the surrounding instruction sequence didn’t actually consume the value as expected.
