## Summary

Make the planning docs read as “ideal/spec-first” rather than historical implementation notes, and record recent metadata-emission findings that impact the two-phase pipeline design.

## Changes

### Docs
- Reframe the two-phase compilation plan as ideal-first (spec-first), with migration notes clearly non-normative.
- Add a reader-friendly SCC definition and link all SCC mentions back to it.
- Clarify `CallableRegistry` responsibilities and propose role-based views to keep concerns separated.
- Record PoC-validated finding: `MethodDefinitionHandle`/MethodDef tokens can be referenced before IL bodies are emitted, but this requires deterministic precomputed metadata table layout (type/method/field row ordering).

### Samples (supporting evidence)
- `samples/MemberReferencePoC`: emits a tiny assembly via `System.Reflection.Metadata` showing both MemberRef (`0x0A...`) and MethodDef (`0x06...`) call sites; demonstrates MethodDef token usage before body emission when row layout is predetermined.
- `samples/CSharpMutualRecursion`: real C# compiler output demonstrating mutual recursion and showing intra-assembly calls use MethodDef tokens.

## Notes
- Docs + samples only; no compiler behavior changes.