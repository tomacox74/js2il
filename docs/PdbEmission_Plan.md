# Plan: Portable PDB Emission (Debug Symbols)

## Goal
Enable JS2IL to optionally emit **Portable PDB** debug symbols alongside generated .NET assemblies, so users can:
- set breakpoints and step through generated code,
- view source locations in stack traces,
- inspect locals (where possible),
- improve diagnostics and developer experience.

Non-goals (initially):
- Perfect JavaScript-level debugging experience identical to a native JS debugger
- Fully accurate variable scoping visualization for all constructs (closures, destructuring, temporaries)
- Source maps *from* transpiled JS (we’ll start with original JS files)

## Scope & Constraints
- JS2IL emits IL via `System.Reflection.Metadata`. PDB emission should use the same low-level approach:
  - `System.Reflection.Metadata.Ecma335.MetadataBuilder`
  - `PortablePdbBuilder`
  - `MetadataRootBuilder`
- Symbols should be **opt-in** via CLI / API.
- When enabled, output should include:
  - `<assembly>.pdb` next to `<assembly>.dll`
  - `DebuggableAttribute` and portable PDB debug directory entry
- Preserve current release pipeline and performance by default (symbols off).

## Design Overview
### 1) Public Surface
- CLI: add a flag (names TBD):
  - `--pdb` or `--debug` (emit portable PDB)
  - optionally `--pdb-embedded` (embed PDB into PE)
- API: extend `CompilerOptions` with:
  - `EmitPdb` (bool)
  - `PdbPath` (optional; defaults to output dll path + `.pdb`)
  - `EmbedPdb` (bool; optional)

### 2) Core Output Pipeline Changes
- Where the PE is written today, add an optional branch to:
  - build Portable PDB metadata
  - write `.pdb` file (or embed)
  - ensure PE includes the correct debug directory entry

Expected code touchpoints (names may vary):
- `Js2IL/Compiler.cs`, `Js2IL/CompilerOptions.cs`, possibly `Js2IL/Program.cs`
- IL/metadata writer services (the stage that currently creates `PEBuilder`)

### 3) Mapping JavaScript Source Locations
To emit meaningful sequence points we need stable source locations:
- Acornima nodes carry `Start`/`End` with `Line`/`Column`.
- Decide the canonical mapping strategy:
  - **Statement-level** sequence points first (simpler, very useful)
  - Add expression-level points incrementally

We’ll need to ensure IL emission has access to:
- source file path (document)
- line/column for each emitted statement

Recommended approach:
- Introduce a small `SourceSpan` struct (file, start line/col, end line/col)
- Thread it through IR/LIR where practical, or attach it to LIR instructions

## Implementation Phases

### Phase 0 — Spike / Feasibility
1. Create a minimal prototype that emits:
   - a DLL with one method
   - a PDB with a single document + a few sequence points
2. Validate with:
   - `ilspycmd` / ILSpy showing source mapping
   - `dotnet-symbol` or `System.Diagnostics.StackTrace` showing file/line in exceptions

Exit criteria:
- Can open generated PDB in ILSpy and see sequence points

### Phase 1 — Plumbing: options + file writing
1. Add `CompilerOptions.EmitPdb` (+ optional path/embed options)
2. Update CLI parsing + help text
3. Update the output writer to write `.pdb` when enabled

Exit criteria:
- Running `js2il input.js outDir --pdb` produces `.dll` + `.pdb`

### Phase 2 — Documents
1. Emit a `DocumentHandle` per input JS file
2. Decide document name:
   - absolute path vs relative path (recommend relative to project / input directory for portability)
3. Ensure consistent hashing / language:
   - language can be JavaScript (no dedicated GUID required; can use vendor-neutral or omit)

Exit criteria:
- PDB contains the document(s) representing input JS

### Phase 3 — Sequence Points (statement-level)
1. Add sequence point emission for:
   - top-level statements in global scope
   - function bodies
   - class methods
2. Ensure sequence points align with IL offsets:
   - capture IL offset just before statement emission
   - emit a `SequencePoint` per statement
3. Handle hidden sequence points where needed:
   - prologue/epilogue
   - compiler-generated control flow

Exit criteria:
- Debugger can step statement-by-statement in common code

### Phase 4 — Locals & Scopes (best-effort)
Initial plan:
- Emit local names for stable locals (where they correspond to JS variables)
- Do **not** attempt to model every temporary as a user-visible local

Work items:
1. Identify variable slots (`VariableRegistry` / scope fields) that correspond to user vars
2. Emit `LocalScope` / `LocalVariable` info for:
   - IL locals representing JS variables
   - method arguments that correspond to JS params

Exit criteria:
- Debugger shows meaningful locals for simple cases

### Phase 5 — Async / Generators / Closures
These are more complex because:
- state machines and lowering transform code shape
- IL offsets no longer map 1:1 to JS source

Strategy:
- First: statement-level sequence points anchored to the *lowered* method body
- Next: add mapping hooks during lowering to retain original spans

Exit criteria:
- Reasonable stepping experience in async/generator code (even if not perfect)

### Phase 6 — Embed PDB (optional)
- Add `EmbedPdb` option
- Emit embedded portable PDB into the PE debug directory

Exit criteria:
- Single-file `.dll` that carries symbols

## Testing Plan
### Unit/Integration Tests
Add tests under `Js2IL.Tests`:
1. Compiler emits `.pdb` file when enabled
2. PDB contains expected document name(s)
3. PDB contains non-empty sequence points for a known sample

Suggested approach:
- Add a new test category (e.g., `DebugSymbols`)
- Use `System.Reflection.Metadata` to read the `.pdb` and assert:
  - documents count
  - at least one method has sequence points
  - sequence points reference the correct document

### Manual Validation Checklist
- Launch generated binary under debugger; verify breakpoints hit
- Throw an exception and verify stack trace file/line
- Inspect with ILSpy/ilspycmd

## Risks & Mitigations
- **Line/column drift** due to lowering: start with statement-level points and mark generated IL as hidden
- **Performance overhead**: keep feature opt-in; avoid heavy per-instruction bookkeeping
- **Path leakage**: prefer relative paths; optionally allow `--pdb-path-mode (absolute|relative)`
- **Cross-platform path separators**: normalize document paths to `/` in PDB

## Deliverables
- CLI + `CompilerOptions` support for `EmitPdb`
- PDB emission (file-based initially)
- Documents + statement-level sequence points
- Tests verifying PDB structure
- Follow-up roadmap for scopes/locals/async

## Open Questions
- Should we emit symbols for generated helper/runtime methods?
- How should we represent “scope-as-class” variables in debugger locals?
- Should the default document path be relative to input file directory or current working directory?
- Do we want deterministic PDBs for reproducible builds?
