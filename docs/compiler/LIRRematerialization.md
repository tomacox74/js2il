# LIR Rematerialization

Rematerialization suppresses a cheap, stable definition and emits it again at
each use. It is distinct from scheduler residency: the scheduler emits a value
once and carries it on the CLR evaluation stack, while rematerialization
reproduces a value at the use site.

`LIRRematerializationPolicy` is the only policy for this optimization.
`TempLocalAllocator` claims an eligible temp with
`TempValueOwner.Rematerialization`; all other temps remain materialized unless
they have an explicit scheduler, branch-fusion, or mandatory-materialization
owner.

## Eligible values

The policy accepts only values whose repeated evaluation is safe and stable:

- constants, parameters, `this`, and direct user-class instance-field loads;
- boxing conversions whose source is recursively rematerializable and is not
  backed by a mutable variable slot.

Calls, allocations with observable identity, mutable-slot reads, scope loads,
TDZ-checked loads, getters, writes, imports, and async/generator resume values
are denied by default. Pure typed operations, literals, and stable loads belong
to validated scheduler residency when they can avoid a local; otherwise they
remain materialized.

## Adding an LIR instruction

Before allowing a new instruction to rematerialize:

1. Classify its effects, throws, allocation identity, and mutable-state reads.
   Default to denying rematerialization.
2. Add canonical def/use, stack-effect/type, boundary, and disposition metadata
   in `LIRInstructionInfo` and its exhaustiveness test.
3. Decide separately whether a single-use value is scheduler-resident. Do not
   infer scheduler residency from a missing local allocation.
4. Add policy tests for both the accepted stable case and a rejected
   mutable/effectful case.
5. Add execution and generator coverage; include PDB/stack-trace coverage when
   source-mapped code or sequence points are involved.

The scheduler's contributor checklist in
[LIRStackScheduler.md](LIRStackScheduler.md) covers additional region,
validation, maxstack, and symbol-preservation requirements.
