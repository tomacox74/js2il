# LIR Stack Scheduler

## Status

The scheduler is being introduced incrementally under the umbrella tracked by
[GitHub issue #1617](https://github.com/tomacox74/js2il/issues/1617).

The implementation currently provides an **identity schedule with conservative
region discovery**:

- LIR instructions are emitted in their original order.
- No temp is made stack-resident by the scheduler.
- Existing `Stackify`, branch fusion, rematerialization, and local allocation
  behavior remains authoritative.
- Existing constructor/field-store peepholes remain intact.
- Every concrete LIR instruction type is inventoried by canonical
  scheduler-facing metadata.
- Straight-line candidate regions are identified between conservative
  boundaries, but their instructions are not reordered.
- Generated method bodies and Portable PDB mappings are unchanged.

The identity stage exists to establish a reviewable and testable emission-plan
boundary before later stages make real scheduling decisions.

## Why a scheduler is needed

JROC LIR names intermediate values with `TempVariable` instances. A direct IL
backend can materialize each live temp through a local:

```text
produce value
stloc temp
...
ldloc temp
consume value
```

CLR IL is a stack machine, so a single-use value can often stay on the
evaluation stack:

```text
produce value
...
consume value
```

The existing [`Stackify`](Stackify.md) pass eliminates some locals by
suppressing a definition and re-emitting it at the use site. That is
rematerialization: it is appropriate for constants and other cheap, stable
loads, but it cannot safely generalize to calls, allocations, observable
property reads, or other instructions that must execute exactly once.

The scheduler will eventually represent a different operation:

1. emit the producer once at its legal evaluation point;
2. leave its result on the CLR evaluation stack;
3. preserve the required stack shape until its consumer;
4. consume the value without `stloc`, `ldloc`, or re-execution.

Stack scheduling and rematerialization are separate optimizations. During the
migration they coexist; after scheduling coverage is complete, the useful
rematerialization behavior will remain under an explicit policy.

## Pipeline placement

Semantic and type-directed LIR normalization runs in `JsMethodCompiler`:

```text
HIRToLIRLowerer
  -> LIRIntrinsicNormalization
  -> LIRMemberCallNormalization
  -> LIRTypeNormalization
  -> late intrinsic normalization
  -> LIRCoercionCSE
```

The stack scheduler belongs to the IL backend because its decisions depend on
CLR evaluation-stack order, local materialization, and emission details:

```text
normalized MethodBodyIR
  -> LIRStackScheduler
  -> legacy branch/Stackify materialization decisions
  -> TempLocalAllocator
  -> LIRToILCompiler
  -> method body + Portable PDB metadata
```

Current integration is in:

- `src/Compiler/IL/LIRStackSchedule.cs`
- `src/Compiler/IL/LIRStackScheduler.cs`
- `src/Compiler/IL/LIRInstructionInfo.cs`
- `src/Compiler/IL/LIRToILCompiler.MethodBodyCompilation.cs`
- `src/Compiler/CompilerOptions.cs`

## Scheduler modes

`LIRStackSchedulerMode` is cumulative:

| Mode | Current behavior |
|---|---|
| `Disabled` | Bypass schedule construction and use the legacy raw LIR index path. |
| `Identity` | Emit through an explicit source-order schedule. This is the default. |
| `TypedNumeric` | Reserved for the first optimizing stage; currently rejected with `NotSupportedException`. |

Later modes must include all behavior from preceding modes so adjacent levels
remain useful for A/B diagnosis.

The option is internal and testable. It is not currently a user-facing CLI
switch.

## Schedule model

### Temp residency

`TempResidency` separates three eventual storage behaviors:

| Residency | Meaning |
|---|---|
| `MaterializedLocal` | Store/load the value through a variable or allocator temp slot. |
| `StackResident` | Emit once and carry the value on the CLR evaluation stack. |
| `Rematerialized` | Suppress the original definition and safely reproduce it at a use. |

Identity mode reports `MaterializedLocal` for every temp and marks every
`OwnedTemps` entry `false`. The `false` ownership is important: it delegates
the actual decision to the existing Stackify/allocator pipeline. Identity mode
does not force all values into locals.

### Instruction disposition

Temp residency does not say what to do with the instruction itself, especially
when its result is unused. `InstructionDisposition` defines the eventual
instruction-level choice:

| Disposition | Meaning |
|---|---|
| `EmitNormally` | Emit through the normal instruction path. |
| `EmitAndDiscardResult` | Execute exactly once and discard an unused result. |
| `ElidePureUnused` | Omit a proven pure, non-throwing instruction whose result is unused. |
| `FusedIntoEmissionUnit` | Emit an atomic group through one specialized operation. |

Identity mode currently assigns `EmitNormally` except for the two existing
constructor/field-store fusion candidates. Existing emitters still make their
legacy unused-result decisions; later stages will move those decisions into
canonical instruction metadata.

### Scheduled operations

`ScheduledOperation` owns a consecutive range of source LIR indexes:

```csharp
internal readonly record struct ScheduledOperation(
    int StartLirIndex,
    int InstructionCount,
    InstructionDisposition Disposition);
```

The order of operations is the emission order. Identity scheduling uses
source-order operations. Future scheduling may reorder whole operations within
validated regions without mutating `MethodBodyIR.Instructions`.

### Canonical instruction metadata

`LIRInstructionInfo` is the scheduler-facing source for:

- ordered temp definitions and uses;
- implicit catch/await/yield definition semantics;
- semantic stack signatures;
- default instruction disposition;
- conservative effect flags;
- internal-control-flow and scheduling-boundary classification.

`KnownInstructionTypes` explicitly inventories every concrete
`LIRInstruction` subtype. A reflection-based test compares that inventory with
the compiler assembly, so adding a new LIR instruction fails until it is
classified. A runtime instruction absent from the inventory fails closed as an
`UnsupportedBarrier`.

Effects distinguish:

- mutable slot, scope, and heap reads/writes;
- calls, allocations, and may-throw behavior;
- explicit control flow;
- suspension;
- scope replacement;
- hidden IL control flow;
- unsupported barriers.

Typed numeric constants/operators are explicitly pure. Calls and dynamic
operations receive conservative read/write/may-throw effects.
TDZ-checked scope loads are marked `MayThrow`.

Catch handler entry is represented by
`LIRImplicitStackInput.CatchException`: the CLR supplies one exception object,
and `LIRStoreException` consumes it into a materialized result. `LIRAwait` and
`LIRYield` use `ResumeResult` definitions and remain opaque suspension
boundaries. `LIRUnwrapCatchException` is an opaque internal-control-flow
boundary because its emitter expands into branches.

The allocator keeps its legacy def/use switches in this stage to guarantee no
IL change. Scheduler and final-LIR validation use canonical metadata; allocator
liveness migration is isolated to its dedicated later stage.

### Regions and metrics

`ScheduledRegion` identifies a source-order window in the flat operation array.
Regions contain only instructions that may participate in future scheduling.
They are split around:

- labels, branches, `leave`, return, throw, and `endfinally`;
- sequence points;
- EH catch entry and hidden-control-flow instructions;
- `yield`, `await`, and state-machine operations;
- scope replacement;
- unknown/unsupported instructions.

Identity mode still performs no optimization. It reports the number of
discovered regions while leaving the optimization metrics at zero:

```text
ScheduledRegionCount = <discovered straight-line region count>
StackResidentTempCount = 0
EliminatedSpillCount = 0
MaxStackDepth = 0
```

The existing IL maxstack calculation remains authoritative in identity mode.

### Effective last uses

The schedule records the last LIR index that uses each temp. Identity mode
matches raw source order and does not feed this data into allocation yet.

Canonical metadata supplements the legacy temp visitor for:

- `LIRThrow.Value`
- `LIRUnwrapCatchException.Exception`

Those operands are consumed by the emitter but are missing from the legacy
allocator visitor. Recording them makes scheduler metadata accurate without
changing allocation or generated IL. The allocator deliberately stays on its
legacy visitor until schedule-effective liveness is integrated.

## Identity operation construction

Most instructions become one `EmitNormally` operation.

Two existing source-adjacent patterns become atomic
`FusedIntoEmissionUnit` candidates:

```text
LIRNewUserClass
LIRStoreUserClassInstanceField(result)
```

and:

```text
LIRNewIntrinsicObject
LIRStoreUserClassInstanceField(result)
```

The scheduler recognizes only the structural candidate. It does **not** move
the runtime eligibility rules out of the emitter.

For example, user-class fusion still depends on:

- a non-derived constructor;
- available class/callable metadata;
- no constructor-return override field;
- a declared constructor token;
- a resolvable destination field;
- compatible constructed and declared field types.

If any eligibility check declines fusion, the identity cursor emits the first
instruction normally and then emits the field store normally. This preserves
the exact legacy fallback.

## Plan-driven emission cursor

`LIRToILCompiler.TryCompileMethodBodyToIL` has two cursor implementations:

### Disabled mode

The cursor advances raw source LIR indexes. A successful adjacent fusion
advances by two indexes, matching the previous `for` loop's `i++` skip.

### Identity mode

The cursor advances through scheduled operations and offsets inside each
operation:

- ordinary instruction: advance one operation;
- failed atomic fusion eligibility: advance to the second instruction inside
  the same operation;
- successful atomic fusion: consume the whole operation.

Labels, branches, sequence points, `leave`, and `endfinally` explicitly advance
the active cursor before continuing.

This dual path is temporary but intentional: disabled-versus-identity
equivalence isolates any problem in plan-driven emission before optimization
coverage is added.

## Current invariants

Identity mode must preserve all of the following:

- original LIR instruction order;
- each instruction's existing execute/elide/discard behavior;
- existing Stackify and branch-fusion ownership;
- `TempLocalAllocator` decisions and compatible-slot reuse;
- variable-slot and constructor-result forced materialization;
- generated IL bytes for every method body;
- local signature blobs;
- maxstack and `InitLocals`;
- exception regions;
- sequence-point mappings and source local indexes;
- constructor/field-store fusion and fallback behavior.
- every concrete compiler LIR subtype is present in the canonical inventory;
- unclassified runtime instructions become unsupported boundaries;
- candidate regions never include explicit control, sequence-point, scope,
  suspension, EH-entry, hidden-CFG, or unsupported boundaries;
- catch entry consumes exactly one implicit exception stack input;
- await/yield results are identified as resume-time definitions.

The identity scheduler does not inspect `CompilerOptions.EmitPdb`; enabling
symbols must not change schedule selection or operation order.

## Tests

`tests/Jroc.Tests/LIRStackSchedulerTests.cs` covers:

- empty identity schedules;
- source-order straight-line and control-flow plans;
- raw effective last-use metadata;
- throw and catch-unwrapping operands;
- intrinsic and user-class atomic fusion candidates;
- derived constructors remaining ordinary operations;
- disabled-mode bypass;
- rejection of not-yet-implemented coverage.

Its integration fixture compiles with scheduler `Disabled` and `Identity`, with
and without PDBs, and compares:

- exact IL bytes for every method;
- decoded local signature blobs;
- maxstack and `InitLocals`;
- exception regions;
- PDB documents and sequence-point blobs;
- local scopes, indexes, attributes, and names.

Focused regression coverage also includes:

- `StackifyTests`;
- `TempLocalAllocatorTests`;
- user/intrinsic constructor-field fusion generator snapshots;
- Portable PDB sequence-point and locals tests.

## What this stage does not do

Identity scheduling does not:

- reorder instructions;
- optimize or reorder discovered scheduling regions;
- keep a temp on the stack;
- remove a spill;
- alter rematerialization;
- change maxstack calculation;
- replace the local allocator;
- classify all LIR effects or stack signatures;
- schedule across labels, branches, sequence points, EH, `yield`, or `await`.

These limitations are deliberate. A no-change foundation makes later
instruction-family PRs smaller and reviewable.

## Planned expansion

The ordered work is tracked under
[issue #1617](https://github.com/tomacox74/js2il/issues/1617):

1. Independent schedule validation and schedule-aware maxstack.
2. Effective liveness/local allocation integration.
3. Portable PDB preservation gate.
4. Typed numeric binary expression trees.
5. Typed unary/comparison/branch expressions.
6. Conversions, stable loads, and explicit rematerialization.
7. Literal and argument construction.
8. Same-region single-use call results.
9. General legal scheduling inside straight-line regions.
10. Stackify scheduling retirement.
11. Final obsolete-code audit and deletion.

At each optimizing stage:

- supported coverage is cumulative;
- positive fixtures must prove the scheduler accepted the region;
- negative fixtures must assert a documented rejection/fallback reason;
- JavaScript evaluation order and exact expression grouping must be preserved;
- broad regression coverage comes from PR CI.

## Contributor checklist

When changing the identity scheduler foundation:

- Keep `Disabled` and `Identity` method bodies equivalent.
- Do not make scheduler behavior depend on PDB emission.
- Preserve ordinary fallback for atomic fusion candidates.
- Keep effective last uses accurate for every newly-observed operand.
- Add every new concrete LIR subtype to canonical metadata and classify it
  conservatively before enabling scheduling.
- Keep hidden-control-flow, catch-entry, suspension, scope-replacement, and
  sequence-point instructions outside candidate regions.
- Add direct schedule assertions, not only execution tests.
- Compare local signature contents, not only metadata row numbers.
- Check decoded Portable PDB mappings, not raw PDB container bytes.
- Run focused Stackify, allocator, fusion, and PDB regressions.
- Do not enable a reserved coverage mode without its own issue's validation and
  exit criteria.
