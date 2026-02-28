# JS2IL AI Agent Instructions

JS2IL is a JavaScript-to-.NET IL compiler that compiles JavaScript source to native .NET assemblies using System.Reflection.Metadata for direct IL emission. 

## Project goals are sorted by priority
* JS2IL should be easy to to use for both library authors and end users.  This means good documentation, good error messages, and a good developer experience.  It should be low friction.
* JS2IL completely conform to the JavaScript language specification (ECMA-262).  Currently we are targeting ES2025.
* The JS2IL runtime behavior should be completely compatible with Node.js.  Any script that runs in Node.js should compile with run with js2il.
* The JS2IL compiler and runtime should be NPM compatibale and have the same module resolution rules as node.js.
* Given that js2il is a true compiler there is a very reasonable expectation that its output should be faster than other solutions. A lot of the performance comparisons are against jint with is a dotnet interpreted solution.
* it is not expected to ever match node.js performance because that is a project that has had a decade of optimiizations but it should be able to run real world node applications with acceptable performance.

These goals drive choices made.  And their ranking above should be clear. For example if I have a 10x perf inprovement but it breaks compatiblity with node or ECMA 262 it is a non-starter.

## Architecture Overview

### Compilation Pipeline (6 Phases)
1. **Parse**: JavaScript → AST (Acornima parser)
2. **Validate**: AST validation for supported features (`JavaScriptAstValidator`)
3. **Symbol Table**: Build scope tree with variable bindings (`SymbolTableBuilder` → `SymbolTable`)
4. **Type Generation**: Scopes → .NET types with fields (`TypeGenerator` → `VariableRegistry`)
5. **Two-Phase Callable Planning**: `TwoPhaseCompilationCoordinator` runs `CallableDiscovery`, `CallableDependencyCollector`, `CompilationPlanner`, and `CallableRegistry` setup to discover callables, compute dependency order, and predeclare callable tokens.
6. **Lowering + IL Emission**: `JsMethodCompiler` runs AST → HIR (`HIRBuilder`) → LIR (`HIRToLIRLowerer`), applies LIR normalization/optimization (`LIRIntrinsicNormalization`, `LIRMemberCallNormalization`, `LIRTypeNormalization`, `LIRCoercionCSE`), then emits IL via `LIRToILCompiler` and finalizes preallocated MethodDefs via `MethodDefinitionFinalizer`.

### Core Concepts

**Scope-as-Class Pattern**: Every JavaScript scope (global, function, block, class) becomes a .NET class. Variables become instance fields. Multiple instances enable closure semantics.

**Variable Resolution**: `Variables` class maps identifier names → storage locations:
- Parameters: method arguments
- Local scope vars: `ldloc.0` (scope instance) → `ldfld`
- Parent scope vars: `ldarg.0` (scopes array) → `ldelem_ref` → cast → `ldfld`

**Strongly-Typed Scope Locals** (recent optimization): Local variables storing scope instances use `TypeDefinitionHandle` for their specific scope class instead of `System.Object`, eliminating `castclass` after `ldloc`. Only cast when loading from parameters or scope arrays.

### Key Compiler Services

- **MainGenerator**: Entry-point orchestrator that delegates callable planning/compilation to the two-phase pipeline before module-main compilation.
- **TwoPhaseCompilationCoordinator**: Coordinates Phase 1 discovery/declaration and Phase 2 planned callable compilation/finalization.
- **CallableDiscovery + CallableDependencyCollector + CompilationPlanner**: Discover callables, build dependency edges, and compute deterministic SCC/topological stage order.
- **CallableRegistry**: Canonical `CallableId`-keyed callable signature/token registry with strict lookup mode during body compilation.
- **ClassesGenerator**: Declares class TypeDefs/fields and two-phase class callable metadata needed for class body compilation.
- **JavaScriptArrowFunctionGenerator**: Compiles/finalizes arrow callable bodies against preallocated MethodDefs in the two-phase flow.
- **JsMethodCompiler**: Per-callable compiler that orchestrates AST → HIR → LIR → IL compilation.
- **HIRBuilder + HIRToLIRLowerer**: Core lowering pipeline from AST semantics into SSA-friendly `MethodBodyIR`.
- **LIRIntrinsicNormalization / LIRMemberCallNormalization / LIRTypeNormalization / LIRCoercionCSE**: Key LIR rewrite and optimization passes before IL emission.
- **LIRToILCompiler + MethodDefinitionFinalizer**: Emit IL bodies and finalize deterministic MethodDef rows.
- **TypeGenerator**: Creates .NET scope types from `SymbolTable` and populates `VariableRegistry` field bindings.
- **Runtime**: Provides `MemberReferenceHandle` cache for JavaScriptRuntime helper methods.

### Critical Files
- `Js2IL/Services/ILGenerators/MethodBuilder.cs`: `CreateLocalVariableSignature()` - centralizes local sig creation
- `Js2IL/SymbolTable/`: Scope tree infrastructure with free variable analysis
- `JavaScriptRuntime/`: Runtime library (Array, Object, Operators, Math, String, Closure helpers)

## Development Workflows
- the typical pattern for adding support for javascript and node features is to add tests first under JS2IL.Tests/*area*/ExecutionTests and GeneratorTests
- Confirm the tests are failing but and also create the expected snapshot file for the ExecutionTest output.  At this point there is no snapshot for the GeneratorTest.
- Implmenent the new feature
- Run all tests and confirm that all execution tests are running.  It is ok if generator tests are failing.
- Run the script to update the generator test snapshots: `node scripts/updateVerifiedFiles.js`
- Confirm that all generator tests are now passing.
- Commit the changes with a descriptive message.
- Update changelog.md if necessary.
- Update the relevant `docs\ECMA262\**\Section*_*.json` subsection doc(s) (use `support.entries` for feature-level support details). Run `node scripts/ECMA262/generateEcma262SectionMarkdown.js --section <section.subsection>` to regenerate the markdown.
- Update Node.js documentation if it is a new node feature:
  - For new modules: Create `docs/nodejs/<module_name>.json` (following `ModuleDoc.schema.json`)
  - For new globals: Create `docs/nodejs/<global_name>.json` (following `ModuleDoc.schema.json`)
  - For updates to existing modules/globals: Edit the corresponding JSON file
  - Run `npm run generate:node-index` to update the Index.md
  - Run `npm run generate:node-module-docs` to regenerate individual markdown files
  - Alternatively, run `npm run generate:node-modules` to do both steps
  - Legacy: You can also update `docs/nodejs/NodeSupport.json` and run `npm run generate:node-support` for the monolithic doc
- Create and a coomit with the documentation updates.
- Create a PR with all the changes back to master.
- After the PR has been merged, confirm the changes are in master and delete the local and remote feature branches.

### Documentation Structure

#### Node.js Documentation (`/docs/nodejs`)
- **Index.md**: Auto-generated index of all modules and globals with status
- **Individual module/global files**: One JSON + MD pair per module/global (e.g., `path.json` + `path.md`)
- **ModuleDoc.schema.json**: JSON schema for individual module/global documentation
- **NodeLimitations.json**: Shared limitations across all Node.js features
- **Scripts**:
  - `npm run generate:node-modules`: Full regeneration (split → index → docs)
  - `npm run generate:node-index`: Regenerate Index.md only
  - `npm run generate:node-module-docs`: Regenerate individual markdown files only
- **Legacy files** (maintained for compatibility):
  - NodeSupport.json: Monolithic source (can be updated via `scripts/splitNodeSupportIntoModules.js`)
  - NodeSupport.md: Generated monolithic documentation

#### ECMA-262 Documentation (`/docs/ECMA262`)
- **Index.md**: Coverage index for all ECMA-262 sections
- **Section directories**: One per major section (e.g., `19/` for Global Object)
- **Section files**: JSON + MD pairs for each section/subsection (e.g., `Section19_1.json` + `Section19_1.md`)
- **SectionDoc.schema.json**: JSON schema for section documentation
- **Scripts**: `npm run ecma262:generate-section-md -- <section>`



### Building & Running
```powershell
dotnet build                                    # Debug build
dotnet publish -c Release                       # Release build
dotnet run --project Js2IL -- input.js output  # Run from source
js2il input.js output                           # Installed tool
```

### Testing
- **Execution tests**: `ExecutionTestsBase` - compile JS → run .dll → verify output
- **Generator tests**: `GeneratorTestsBase` - compile JS → decompile IL → snapshot test via Verify
- The same test cases are used for both execution and generator tests. The test javascript is shared.
- Snapshot updates: `node scripts/updateVerifiedFiles.js` (updates all `*.received.*` → `*.verified.*`).  This tool is useful when a IL change affects many tests.
- Test categories: Array, BinaryOperator, Classes, CompoundAssignment, ControlFlow, Function, etc.
- Currently manually running the script tests\performance\PrimeJavaScript.js to compare node performance vs js2il performance.
- Only run all tests if explicitly asked.. its time consuming and all tests will be run for PRs automatically

### Debugging
- Use ilspycmd to disassemble generated DLLs to IL for inspection
- Only run all tests if explicitly asked.. its time consuming and all tests will be run for PRs automatically

### Release Process

**IMPORTANT**: Always create the release branch FIRST, before running any version bump commands.

**Preferred (Automated):**

Use the release automation script (handles branch → bump → commit → PR, and optionally merge + GitHub release/tag):

```powershell
# Patch / minor / major
npm run release:cut -- patch
# npm run release:cut -- minor
# npm run release:cut -- major

# Fully automate through merge + release creation
npm run release:cut -- patch --merge
```

Notes:
- Requires `gh auth status` to be OK (GitHub CLI authenticated)
- Without `--merge`, it stops after creating the PR
- With `--merge`, it waits for checks (if any are configured), merges, then creates the GitHub release using the relevant `CHANGELOG.md` section
- Optional flags: `--skip-empty`, `--dry-run`, `--repo owner/name`, `--base master`, `--verbose`

**Fallback (Manual):**

Follow these steps IN ORDER:

1. **Create release branch** (from master):
   ```powershell
   git checkout master
   git pull
   git checkout -b release/0.x.y
   ```

2. **Bump version** (on the release branch):
   ```powershell
   npm run release:patch  # For patch version (0.x.y -> 0.x.y+1)
   # OR
   npm run release:minor  # For minor version (0.x.y -> 0.x+1.0)
   # OR
   npm run release:major  # For major version (0.x.y -> x+1.0.0)
   ```
   This updates CHANGELOG.md, Js2IL/Js2IL.csproj, and JavaScriptRuntime/JavaScriptRuntime.csproj

3. **Commit version bump** (on the release branch):
   ```powershell
   git add CHANGELOG.md Js2IL/Js2IL.csproj JavaScriptRuntime/JavaScriptRuntime.csproj
   git commit -m "chore(release): cut v0.x.y"
   ```

4. **Push release branch and create PR**:
   ```powershell
   git push -u origin release/0.x.y
   gh pr create --title "chore(release): Release v0.x.y" --body-file <release-notes.md> --base master --head release/0.x.y
   ```

5. **After PR is merged**, create the release (this creates the tag and triggers GitHub Actions):
   create a release-notes.md file with the release notes copied from changelog.md for the new version.

   ```powershell
   git checkout master
   git pull
   gh release create v0.x.y --title "v0.x.y" --notes-file release-notes.md --target master
   ```

GitHub Actions (`.github/workflows/release.yml`) builds and publishes to NuGet when the release is created.

## Project Conventions

### IL Generation Patterns

**Loading Variables**:
```csharp
// Scope local (strongly typed, no cast needed)
il.LoadLocal(0);                    // ldloc.0 - already correct type
il.LoadField(fieldHandle);          // ldfld

// Parameter or scope array (cast required)
il.LoadArgument(paramIndex);        // ldarg - object type
il.Token(scopeTypeHandle);          // castclass
il.LoadField(fieldHandle);          // ldfld
```

**Method Signatures**: All use `MethodBuilder.BuildMethodSignature()`:
- Instance methods: first param is `object[] scopes` when accessing parent scopes
- Parameters/returns: always `System.Object` (boxed)
- Use `MethodBuilder.CreateLocalVariableSignature()` for locals (handles typed scopes)

**Conditional Casting**: Only cast after `ldarg`/`ldelem_ref`, never after `ldloc` (see `ILExpressionGenerator.EmitLoadScopeObjectTyped`)

### Error Handling
All `NotSupportedException` thrown via `ILEmitHelpers.ThrowNotSupported()` with source location (file:line:col) when AST node available.

### Metadata Caching
- `Runtime` class caches `MemberReferenceHandle` for JavaScriptRuntime methods (avoid duplicates)
- Single `AssemblyReferenceHandle` per JavaScriptRuntime per `MetadataBuilder` (via `ConditionalWeakTable`)
- `BaseClassLibraryReferences` caches BCL type handles (Object, String, Int32, etc.)

### Scope Naming
- Global: `GlobalScope`
- Functions: `<FunctionName>` or `FunctionExpression_L<line>C<col>`
- Blocks: `Block_L<line>C<col>`
- Classes: `<ClassName>`, methods as nested types: `<ClassName>+<MethodName>`

### Type System
`JavascriptType` enum tracks static type info (Number, String, Boolean, Array, Object, Unknown). Used for optimization (avoid runtime coercion when type known).

## Common Patterns

**Adding New Operators**: Extend `BinaryOperators` or `ILExpressionGenerator`, use `Runtime` for helper calls (e.g., `Operators.Add`).

**Supporting New Syntax**: 
1. Add AST node handling in `JavaScriptAstValidator` (if validation needed)
2. Extend `SymbolTableBuilder` if introducing new binding scope
3. Add IL emission in appropriate generator (statement → `ILMethodGenerator`, expression → `ILExpressionGenerator`)
4. Add execution + generator tests with snapshot

**Debugging IL**: Use `scripts/decompileGeneratorTest.js` or inspect `.verified.txt` snapshots. Common issues: stack imbalance (track push/pop), incorrect metadata handles, missing type casts.

## External Dependencies
- **Acornima**: JavaScript parser (ES2025 target)
- **System.Reflection.Metadata**: IL/metadata emission
- **Verify**: Snapshot testing framework
- **PowerArgs**: CLI argument parsing

## Recent Major Changes
- **v0.3.0**: Unified free variable analysis in `SymbolTableBuilder`, class method parent scope access
- **v0.2.0**: Object destructuring in function parameters with default values
- **v0.1.6**: Dynamic property access, Math intrinsic, Int32Array, compound assignments

## Node.js Interop
Modules discovered via `[NodeModule]` attribute (e.g., `fs`, `path`, `perf_hooks`). Module instances typed via `RuntimeIntrinsicType` for direct callvirt (avoids reflection). See `JavaScriptRuntime/Node/` for implementations.

## Tools
- use ilspycmd to inspect generated assemblies
- use the github cli to create pull requests and releases

## GitHub Issue Management

When creating or editing GitHub issues with the GitHub CLI:
- **Always use `issue-body.md` for the body content** instead of inline `--body` strings
- Create issues: `gh issue create --title "Title" --body-file issue-body.md --label "enhancement"`
- Edit issues: `gh issue edit <issue-number> --body-file issue-body.md`
- Delete `issue-body.md` after use (it's in `.gitignore`)

This approach:
- Avoids command-line length limits
- Supports complex markdown formatting
- Prevents CLI hanging issues with long descriptions
- Makes issue content easier to review before submission

## Resolving PR Review Comment Threads

When branch protection requires all conversations to be resolved before merging, use the GitHub GraphQL API to programmatically resolve review threads.

### Step 1: Get Review Thread IDs
```powershell
gh api graphql -f query='query { repository(owner: \"tomacox74\", name: \"js2il\") { pullRequest(number: <PR_NUMBER>) { reviewThreads(first: 50) { nodes { id isResolved } } } } }'
```

### Step 2: Resolve Each Thread
The GraphQL mutation requires proper JSON escaping. Use `[System.IO.File]::WriteAllText` to create a properly formatted JSON body file (avoids PowerShell escaping issues and BOM characters):

```powershell
# Single thread resolution
[System.IO.File]::WriteAllText("body.json", '{"query": "mutation { resolveReviewThread(input: {threadId: \"<THREAD_ID>\"}) { thread { isResolved } } }"}')
gh api graphql --input body.json
```

### Step 3: Batch Resolution (Multiple Threads)
```powershell
$threadIds = @("PRRT_xxx", "PRRT_yyy", "PRRT_zzz")
foreach ($id in $threadIds) {
    [System.IO.File]::WriteAllText("body.json", "{`"query`": `"mutation { resolveReviewThread(input: {threadId: \`"$id\`"}) { thread { isResolved } } }`"}")
    gh api graphql --input body.json
}
Remove-Item body.json -ErrorAction SilentlyContinue
```

### Why This Approach
- **PowerShell escaping is complex**: Direct `-f query='...'` fails with nested quotes in GraphQL
- **BOM issues**: `Out-File -Encoding utf8` adds BOM which breaks JSON parsing
- **`WriteAllText` works**: Writes UTF-8 without BOM, handles escaping correctly
- Thread IDs have format `PRRT_kwDO...` (Pull Request Review Thread)
