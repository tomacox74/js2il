# JS2IL AI Agent Instructions

JS2IL is a JavaScript-to-.NET IL compiler that compiles JavaScript source to native .NET assemblies using System.Reflection.Metadata for direct IL emission. 

## Architecture Overview

### Compilation Pipeline (5 Phases)
1. **Parse**: JavaScript → AST (Acornima parser)
2. **Validate**: AST validation for supported features (`JavaScriptAstValidator`)
3. **Symbol Table**: Build scope tree with variable bindings (`SymbolTableBuilder` → `SymbolTable`)
4. **Type Generation**: Scopes → .NET types with fields (`TypeGenerator` → `VariableRegistry`)
5. **IL Emission**: AST + metadata → IL bytes (`MainGenerator` orchestrates specialized generators)

### Core Concepts

**Scope-as-Class Pattern**: Every JavaScript scope (global, function, block, class) becomes a .NET class. Variables become instance fields. Multiple instances enable closure semantics.

**Variable Resolution**: `Variables` class maps identifier names → storage locations:
- Parameters: method arguments
- Local scope vars: `ldloc.0` (scope instance) → `ldfld`
- Parent scope vars: `ldarg.0` (scopes array) → `ldelem_ref` → cast → `ldfld`

**Strongly-Typed Scope Locals** (recent optimization): Local variables storing scope instances use `TypeDefinitionHandle` for their specific scope class instead of `System.Object`, eliminating `castclass` after `ldloc`. Only cast when loading from parameters or scope arrays.

### Key Services

- **TypeGenerator**: Creates .NET types from `SymbolTable`, populates `VariableRegistry` with field handles
- **MainGenerator**: Orchestrates IL emission for global scope, wires function/class generators
- **ILMethodGenerator**: Statement-level IL emission (control flow, assignments, declarations)
- **ILExpressionGenerator**: Expression-level IL emission (operators, calls, member access)
- **JavaScriptFunctionGenerator**: Emits function declarations/expressions as static methods
- **JavaScriptArrowFunctionGenerator**: Emits arrow functions (inherits parent `this`)
- **ClassesGenerator**: Emits ES6 classes (constructors, methods, fields, private fields with name mangling)
- **BinaryOperators**: Handles all binary operators including scope-aware variable loading
- **Runtime**: Provides `MemberReferenceHandle` cache for JavaScriptRuntime helper methods

### Critical Files
- `Js2IL/Services/VariableBindings/Variable.cs`: Variable metadata with `GetLocalVariableType()` for typed locals
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
- Update docs\EMCAScript2025_FeatureConverage.json if it is a new javascript feature supported.  Run `node scripts/generateFeatureCoverage.js` to regenerate the the markdown file wiht the same name as the JSON file..
- Update docs\NodeSupport.json if it is a new node feature supported.  Run `node scripts/generateNodeSupportMd.js` to regenerate the the markdown file wiht the same name as the JSON file.
- Create and a coomit with the documentation updates.
- Create a PR with all the changes back to master.
- After the PR has been merged, confirm the changes are in master and delete the local and remote feature branches.



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

### Debugging
- Use ilspycmd to disassemble generated DLLs to IL for inspection

### Release Process
 - create a release branch off of master.  The branch name should be release/0.x.y where x.y is the new version number.
```powershell
npm run release:patch  # Bump version, update CHANGELOG
git add CHANGELOG.md Js2IL/Js2IL.csproj JavaScriptRuntime/JavaScriptRuntime.csproj
git commit -m "chore(release): cut v0.x.y"
```
 - create a pr back to master using the github cli
 - after the PR is merged, checkout master and pull the latest changes
 - create the release using the github cli (this creates the tag and triggers GitHub Actions):
```powershell
gh release create v0.x.y --title "v0.x.y" --notes "Release notes from CHANGELOG" --target master
```
GitHub Actions (`.github/workflows/release.yml`) builds and publishes when the release is created.

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

**Debugging IL**: Use `scripts/decompileToIL.js` or inspect `.verified.txt` snapshots. Common issues: stack imbalance (track push/pop), incorrect metadata handles, missing type casts.

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
