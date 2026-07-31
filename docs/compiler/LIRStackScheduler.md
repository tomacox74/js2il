# LIR Stack Scheduler

## Status

The scheduler is being introduced incrementally under the umbrella tracked by
[GitHub issue #1617](https://github.com/tomacox74/js2il/issues/1617).

The implementation currently provides **typed numeric, comparison, conversion,
concat, and stable typed-load scheduling** on top of the validated identity
foundation:

- LIR instructions are emitted in their original order.
- Single-use typed numeric binary, unary, comparison, conversion, concat, and
  approved typed length/element results can remain stack-resident.
- `LIRRematerializationPolicy` separately owns the decision to reproduce cheap,
  stable definitions at their uses.
- Existing `Stackify`, branch fusion, and local allocation behavior remains
  authoritative for unsupported shapes.
- Existing constructor/field-store peepholes remain intact.
- Every concrete LIR instruction type is inventoried by canonical
  scheduler-facing metadata.
- Straight-line candidate regions are identified between conservative
  boundaries, but their instructions are not reordered.
- Generated method bodies and Portable PDB mappings are unchanged.

All unsupported shapes retain identity/legacy behavior.

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

Stack scheduling and rematerialization are separate optimizations.
`LIRRematerializationPolicy` is the common entry point for the allocator and
legacy Stackify decisions. It answers only whether a definition can be
reproduced safely; it does not select stack residency or instruction order.

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
- `src/Compiler/IL/LIRRematerializationPolicy.cs`
- `src/Compiler/IL/LIRInstructionInfo.cs`
- `src/Compiler/IL/LIRToILCompiler.MethodBodyCompilation.cs`
- `src/Compiler/CompilerOptions.cs`

## Scheduler modes

`LIRStackSchedulerMode` is cumulative:

| Mode | Current behavior |
|---|---|
| `Disabled` | Bypass schedule construction and use the legacy raw LIR index path. |
| `Identity` | Emit through an explicit source-order schedule with no scheduler-owned residency. |
| `TypedNumeric` | Identity plus typed numeric binary stack residency. |
| `TypedComparisons` | TypedNumeric plus typed unary/comparison and direct branch/return consumption. |
| `ConversionsAndStableLoads` | TypedComparisons plus numeric/object conversions, string concat, and approved typed string/array length and element loads. This is the default. |

Later modes must include all behavior from preceding modes so adjacent levels
remain useful for A/B diagnosis.

## Typed numeric binary coverage

`TypedNumeric` owns single-definition/single-use results from:

```text
LIRAddNumber
LIRSubNumber
LIRMulNumber
LIRDivNumber
LIRModNumber
LIRExpNumber
```

Supported consumers include another typed numeric binary operation and safe
terminal plumbing such as `LIRConvertToObject`, `LIRCopyTemp`, numeric stores,
and `LIRReturn`.

The scheduler initially preserves LIR order. It:

1. builds canonical def/use counts;
2. requires definition and use in the same scheduling region, or a supported
   terminal immediately after that region;
3. rejects variable-slot-backed results;
4. restricts intervening instructions to numeric producers/loads supported by
   this stage;
5. marks candidates stack-resident;
6. performs one forward stack simulation that prunes operand orders the
   emitter cannot consume safely.

The emitter executes a scheduler-owned numeric operation at its original
definition site and leaves its result on the CLR stack. When a later consumer
loads that temp, `EmitLoadTemp` emits nothing because validation proved the
value is already at the correct stack position.

Legacy Stackify cannot claim an instruction that consumes a scheduler-owned
temp, preventing rematerialization from duplicating or delaying the scheduled
numeric tree.

This is scheduling, not algebraic optimization. The exact expression tree,
operand positions, IEEE-754 grouping, signed-zero/NaN/infinity behavior,
division/remainder, and `Math.Pow` call order are preserved. There is no
reassociation, commutative swapping, or constant folding.

Examples now emitted without numeric intermediate spills:

```js
factor * 2 + 1
a + b + c + d
a * b + c * d
a - b - c
a / b % c
a ** b + c
```

Calls, dynamic operators, values crossing sequence points/branches,
scope-field stores that push a receiver before their value, and unsupported
operand orders remain materialized or legacy-owned. `LIRConvertToObject` is
stack-resident only for the proven simple synchronous-return shape; other
boxing consumers retain legacy behavior.

## Conversion, concat, and stable typed-load coverage

`ConversionsAndStableLoads` extends cumulative coverage to:

```text
LIRConvertToNumber
LIRConvertToObject
LIRConcatStrings
LIRGetStringLength
LIRGetJsArrayLength
LIRGetInt32ArrayLength
LIRGetJsArrayElement
LIRGetInt32ArrayElement
```

Candidates still require one definition, one use, no backing variable slot,
and a supported same-region consumer or synchronous terminal return. The
forward stack simulation rejects any operand order the source-order emitter
cannot consume. Definitions execute once at their original position; a
scheduler-owned conversion or typed load is never reproduced at its use.

Only statically proven string, `Array`, and `Int32Array` operations are covered.
Generic property/index reads remain materialized because getters, proxies, and
coercion may be observable. Calls, allocations, mutable slot loads, TDZ-checked
scope loads, async/generator returns, and values crossing scheduling boundaries
remain outside this mode.

Cheap constants, parameter/`this` loads, and other legacy-approved stable loads
may instead be `Rematerialized`. Multi-use values are never claimed as
stack-resident by this stage; for example, a two-use numeric constant can be
reproduced at both uses. `TempMaterializationPlan` records this distinction:

```text
StackResident  = emit once at the definition and carry on the CLR stack
Rematerialized = suppress the definition and safely reproduce at a use
Materialized   = store in and reload from a local or variable slot
```

## Typed unary and comparison coverage

`TypedComparisons` extends cumulative coverage to:

```text
LIRNegateNumber
LIRBitwiseNotNumber
LIRCompareNumberLessThan
LIRCompareNumberGreaterThan
LIRCompareNumberLessThanOrEqual
LIRCompareNumberGreaterThanOrEqual
LIRCompareNumberEqual
LIRCompareNumberNotEqual
LIRCompareBooleanEqual
LIRCompareBooleanNotEqual
```

Covered results can feed another typed unary/comparison/numeric operation, safe
boxing-to-return, `LIRReturn`, or an immediately following
`LIRBranchIfTrue`/`LIRBranchIfFalse`.

For a branch consumer, the comparison executes once at its original definition
site and leaves the Boolean on the evaluation stack. `EmitBranchCondition`
recognizes scheduler ownership and emits no duplicate inline comparison;
`brtrue`/`brfalse` consumes the carried value directly.

Because scheduler ownership is established before
`BranchConditionOptimizer`, a covered comparison belongs to the scheduler and
cannot also belong to legacy branch fusion. Unsupported comparison shapes
remain eligible for the legacy branch optimizer.

Numeric intermediates feeding a comparison are promoted only when the complete
operand prefix passes the persistent-stack validator. Calls remain
materialized, execute left-to-right exactly once, and are not moved.
TDZ-checked reads remain may-throw and source-ordered.

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
the initial decision to `TempMaterializationPlan`. Identity mode does not force
all values into locals.

### Exclusive materialization ownership

`TempMaterializationPlan` is the single mutable owner map used between schedule
validation and local allocation. Every temp has one `TempValueOwner`:

| Owner | Responsibility |
|---|---|
| `MaterializedLocal` | Default unclaimed local candidate. |
| `Scheduler` | Stack-resident or explicitly rematerialized by the new scheduler. |
| `BranchConditionFusion` | Comparison emitted directly into its branch. |
| `LegacyStackify` | Legacy Stackify rematerialization/deferral. |
| `Rematerialization` | Allocator's cheap stable inline/rematerialization path. |
| `VariableSlot` | Source/anonymous variable slot is authoritative. |
| `SnapshotBarrier` | `LIRCopyTemp` snapshot must materialize. |
| `ResumeResult` | Await/yield result populated on resume must materialize. |
| `CatchResult` | Catch-entry exception temp must materialize. |
| `ConstructorResultOverride` | Constructor post-processing requires a stable local. |

Claims are exclusive. Scheduler ownership is established first. Mandatory
materialization then rejects conflicts with scheduler ownership. Branch fusion
and Stackify use `TryClaim`, so neither can also claim a scheduler-owned or
already-owned temp. The allocator may claim rematerialization only while the
temp still has the default owner.

Multi-definition temps preserve legacy identity behavior: snapshot/resume
ownership is based on the legacy selected definition rather than every
assignment to the temp. This matters for generator state-machine result temps.

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

### Regions and metrics

`ScheduledRegion` identifies a source-order window in the flat operation array.
Each region records the active `LIRSequencePoint` ordinal and `SourceSpan`.
Regions contain only instructions that may participate in future scheduling.
They are split around:

- labels, branches, `leave`, return, throw, and `endfinally`;
- sequence points;
- EH catch entry and hidden-control-flow instructions;
- `yield`, `await`, and state-machine operations;
- scope replacement;
- unknown/unsupported instructions.

The validator reconstructs the original sequence-point interval for every LIR
instruction and rejects any operation whose region claims a different ordinal
or source span. Sequence points remain hard scheduler fences; crossing one is
not enabled by proving an instruction pure.

Identity mode still performs no optimization. It reports the number of
discovered regions while leaving optimization metrics at zero. Validation may
report a nonzero stack maximum for implicit catch entry:

```text
ScheduledRegionCount = <discovered straight-line region count>
StackResidentTempCount = 0
EliminatedSpillCount = 0
ValidationFallbackCount = 0
MaxStackDepth = 0 normally, 1 when catch entry supplies an exception object
```

## Schedule validation

`LIRStackScheduleValidator` independently validates a schedule before it reaches
IL emission. It checks:

- every raw LIR index is owned exactly once;
- multi-instruction operations have explicit fused ownership;
- effectful instructions preserve source order;
- region operation windows are valid, non-overlapping, and contain no boundary;
- every non-boundary operation belongs to one region;
- temp residency and scheduler ownership agree;
- stack-resident temps have exactly one definition and one use;
- stack-resident operands are available in valid LIFO order;
- stack values do not cross sequence points, control flow, EH, suspension,
  scope replacement, or internal-control-flow boundaries;
- catch entry supplies one implicit exception consumed by
  `LIRStoreException`;
- await/yield resume results remain materialized;
- declared schedule/region stack maxima are not smaller than validated depth.

The validator annotates:

- `CarriedStackDepthBeforeInstructions`;
- each region's maximum carried depth;
- the method schedule's maximum carried depth.

Identity mode carries no scheduler-owned temps, so its carried depth is zero
except for the implicit catch exception. Generated IL remains unchanged.

### Strict validation and fallback

`LIRStackScheduleValidationBehavior` controls failure handling:

| Behavior | Result |
|---|---|
| `Throw` | Raise an actionable `LIRStackScheduleValidationException`. This is the Debug/test default. |
| `FallbackToIdentity` | Reject an invalid optimized plan, validate a fresh identity schedule, record the rejection reason/counter, and emit only the identity plan. This is the Release default. |

Identity validation itself never silently falls back. Tests can require strict
mode so semantic coverage cannot pass by accidentally exercising the legacy
identity plan.

`IRPipelineMetrics` records scheduler validation fallbacks when metrics are
enabled.

## Schedule-aware maxstack

The existing emitter estimator still measures instruction-internal peak stack
usage from an empty starting stack. The validated schedule contributes the
persistent depth carried into each LIR instruction:

```text
required peak =
  validated carried depth before instruction
  + existing instruction-internal peak estimate
```

The emitted method maxstack is at least both:

- the legacy baseline/estimator;
- the validated schedule maximum.

This deliberately overestimates rather than underestimates when a later
scheduler avoids loading an operand already on the stack. Underestimation can
produce `InvalidProgramException`; conservative overestimation is valid.

Identity mode's carried depths are zero, so method-body maxstack remains
byte-identical to scheduler-disabled output.

### Effective last uses

The schedule records the last scheduled position that uses each temp. The
allocator now walks the validated schedule order rather than assuming raw LIR
order.

Identity mode deliberately recomputes last uses in scheduled order through the
legacy def/use visitor. This preserves byte-identical local allocation during
the migration. Optimized modes consume canonical
`LIRStackSchedule.EffectiveLastUses`, including reordered uses.

Canonical metadata supplements the legacy temp visitor for:

- `LIRThrow.Value`
- `LIRUnwrapCatchException.Exception`

Those operands are consumed by the emitter but are missing from the legacy
allocator visitor. They participate in optimized schedule liveness; identity
mode retains legacy compatibility until an optimizing mode is enabled.

`TempLocalAllocator` remains a compatible-storage linear-scan allocator. It:

- frees slots at schedule-effective last use;
- reuses compatible slots for non-overlapping scheduled ranges;
- prevents reuse for ranges that overlap only after scheduling;
- gives scheduler-owned stack temps no local;
- records cheap stable rematerialization through the ownership plan;
- leaves variable slots outside temp-slot allocation.

Async functions persist final allocated variable and temp slots through
`AsyncScope._locals`. Spill and restore enumerate `allocation.SlotStorages`, so
they automatically use the final schedule-aware mapping. Values crossing
`await`/`yield` and resume-result temps remain materialized.

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
- every schedule passes independent ownership, effect-order, region, LIFO,
  boundary, and maxstack validation before emission;
- invalid optimized plans either fail strictly or fall back to a newly
  validated identity plan;
- carried stack depth is included in emitted maxstack accounting.
- every non-materialized temp has one explicit owner;
- scheduler, branch fusion, Stackify, and rematerialization cannot overlap;
- identity allocation remains byte-identical while optimized modes use
  canonical schedule-effective liveness;
- async/generator spill and restore use the final allocated slot mapping.
- every region owns one explicit sequence-point ordinal/source span and cannot
  contain an instruction from another source interval;
- final PDB metadata uses actual post-schedule offsets, signature, IL length,
  and stable source-local indexes.
- every accepted typed numeric region records scheduler ownership and
  eliminated-spill metrics;
- numeric definitions execute once at their source position and are consumed
  without rematerialization;
- unsupported or invalid numeric candidates are pruned before strict schedule
  validation.
- covered branch comparisons are scheduler-owned and excluded from legacy
  branch fusion;
- branch target stacks remain empty after the conditional branch consumes the
  carried Boolean.
- conversion, concat, and approved typed length/element definitions execute
  exactly once at their source position;
- observable generic getters, mutable loads, and TDZ-checked loads are never
  accepted as stable scheduler loads;
- rematerialization decisions are explicit and independent from scheduler
  residency and instruction order.

The identity scheduler does not inspect `CompilerOptions.EmitPdb`; enabling
symbols must not change schedule selection or operation order.

## Portable PDB contract

Schedule optimization may change IL offsets and remove temp locals, but it must
preserve source meaning:

- the same source documents and hashes;
- the same ordered non-hidden source spans;
- valid, nondecreasing sequence-point offsets in the final method body;
- breakpoints before the statement's observable work;
- source-mapped exception file/line;
- rewritten-module mappings to original `.mjs` sources;
- hidden sequence points where the lowering emits them;
- stable source-local names and indexes;
- no compiler temp exposed as a PDB local;
- `LocalScope` length based on final emitted IL.

`LIRToILCompiler` records `MethodSequencePoint` values from the actual emitted
IL offset. After emission, `DebugSymbolRegistry` receives the final local
signature, IL byte length, and source-local metadata; `PortablePdbEmitter` then
encodes those final values.

PDB and non-PDB compilations may differ by debug-only `nop` instructions and
the assembly's `DebuggableAttribute`. Scheduler mode, region ownership,
residency, liveness, and operation order must not depend on `EmitPdb`.

Decoded semantic PDB content—not raw PDB container bytes—is the compatibility
contract.

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
- typed numeric chains/trees, metrics, call-result exclusions, and
  sequence-point rejection;
- typed unary/comparison chains, scheduler-owned branch consumption, call
  order, TDZ, NaN/infinity/equality, and signed-zero negation;
- exact generated IL proving no intermediate numeric `stloc`/`ldloc` pairs;
- execution coverage for all six operators, NaN, signed zero,
  non-commutative order, calls, and postfix snapshots;

Its integration fixture compiles with scheduler `Disabled` and `Identity`, with
and without PDBs, and compares:

- exact IL bytes for every method;
- decoded local signature blobs;
- maxstack and `InitLocals`;
- exception regions;
- PDB documents and sequence-point blobs;
- local scopes, indexes, attributes, and names.

Focused regression coverage also includes:

- `LIRStackScheduleValidatorTests` for positive, rejection, catch-entry,
  deep-stack, strict, and fallback behavior;
- `StackifyTests`;
- `TempLocalAllocatorTests`;
- async/generator local persistence across `await` and `yield`;
- user/intrinsic constructor-field fusion generator snapshots;
- Portable PDB sequence-point and locals tests;
- `SchedulerPdbPreservationTests` for disabled/identity decoded-symbol
  equivalence, source-local identity, final `LocalScope` length, async/generator
  source lines, and compilation with/without PDBs;
- source-mapped stack traces after scheduler-covered arithmetic;
- `ILVerificationTests`, including deep arithmetic and arbitrary-value
  try/catch/finally.

## Current non-goals

The current cumulative scheduling mode does not:

- reorder LIR instructions;
- schedule calls, allocations, dynamic operators, coercive equality, logical
  not, or `LIRIsInstanceOf`;
- carry values across sequence points, control flow, EH, `yield`, or `await`;
- replace the local allocator;
- schedule multi-use values as stack-resident.

These limitations are deliberate. A no-change foundation makes later
instruction-family PRs smaller and reviewable.

## Planned expansion

The ordered work is tracked under
[issue #1617](https://github.com/tomacox74/js2il/issues/1617):

1. Literal and argument construction.
2. Same-region single-use call results.
3. General legal scheduling inside straight-line regions.
4. Stackify scheduling retirement.
5. Final obsolete-code audit and deletion.

At each optimizing stage:

- supported coverage is cumulative;
- positive fixtures must prove the scheduler accepted the region;
- negative fixtures must assert a documented rejection/fallback reason;
- JavaScript evaluation order and exact expression grouping must be preserved;
- broad regression coverage comes from PR CI.

## Contributor checklist

When changing the scheduler foundation:

- Keep `Disabled` and `Identity` method bodies equivalent.
- Do not make scheduler behavior depend on PDB emission.
- Keep each scheduled region inside one sequence-point ordinal/source span.
- Compare decoded PDB semantics after every optimizing coverage expansion.
- Preserve ordinary fallback for atomic fusion candidates.
- Keep effective last uses accurate for every newly-observed operand.
- Claim every temp through `TempMaterializationPlan`; do not add another
  independent materialization mask.
- Add ownership-overlap tests whenever a new optimizer claims a temp.
- Preserve async/generator cross-suspension materialization and verify final
  spill/restore slot mappings.
- Add every new concrete LIR subtype to canonical metadata and classify it
  conservatively before enabling scheduling.
- Keep hidden-control-flow, catch-entry, suspension, scope-replacement, and
  sequence-point instructions outside candidate regions.
- Keep positive scheduler fixtures in strict mode so fallback cannot hide a
  missing optimization.
- Add validator rejection coverage for every new residency or operation shape.
- Include persistent carried depth in maxstack before enabling a new
  stack-resident instruction family.
- Add direct schedule assertions, not only execution tests.
- Compare local signature contents, not only metadata row numbers.
- Check decoded Portable PDB mappings, not raw PDB container bytes.
- Run focused Stackify, allocator, fusion, and PDB regressions.
- Do not enable a reserved coverage mode without its own issue's validation and
  exit criteria.
- Keep numeric operator selection separate from algebraic reassociation or
  constant folding.
- Prove every positive fixture was scheduler-accepted and did not silently
  fall back to identity.
