# Changelog

All notable changes to this project are documented here.

## Unreleased

- IL/Performance: stackify typed numeric arithmetic ops (`LIR*Number`) when operands are inlineable, reducing temp-local materialization and emitted IL noise in arithmetic-heavy code.
- Compiler/type-inference: improve Prime sieve numeric inference (typed-array reads + numeric locals like `q`/`step`/`start`) to keep hot-loop values unboxed and reduce `ToNumber(object)` overhead.
- Credits: @tomacox74.

## v0.8.9 - 2026-02-13

- Performance: restore Int32Array element-set fast-path when index/value are boxed doubles (avoids `Object.SetItem` in hot loops; improves prime sieve performance).

## v0.8.8 - 2026-02-13

- Quality: add IL verification gate using `ilverify` to catch IL emission regressions (closes #580). Tests verify loops, control flow, exception handling, closures, and class methods.
- Compiler: remove `--prototypeChain` CLI option; prototype-chain behavior is now always automatically enabled when code uses prototype-related features (e.g., `__proto__`, `Object.getPrototypeOf`, `Object.setPrototypeOf`) to ensure ECMAScript compliance.
- Runtime/spec: support `Array.prototype.{reduce,reduceRight,indexOf}.call(arrayLike, ...)` for array-like receivers (e.g., DOM NodeList) (unblocks turndown/domino).
- Runtime/spec: implement `String.prototype.repeat`, `trim`, `trimStart`, `trimEnd`, `slice`, and `indexOf` (unblocks turndown/domino).
- IR/codegen: fix boxed-number arithmetic lowering/materialization (notably `-`, `*`, `/`, `%`) to prevent numeric corruption in real-world parsers.
- CommonJS/runtime: set `require.main` to the entry module for Node-compatible `require.main === module` checks.
- Compiler/type-inference: add type inference for `Array.prototype.some()` return values, enabling proper unboxed boolean comparisons (completes fix for #358).

## v0.8.7 - 2026-02-11

- Performance/CommonJS: invoke module-scoped `require` delegate directly (avoid `Closure.InvokeWithArgs` dispatcher and unnecessary argument packing).
- Generators/spec: unwind `throw()`/`return()` through `try/finally` while suspended at `yield` (fixes #390).
- Generators/spec: support `try/catch/finally` containing `yield`, including `.throw()`/`.return()` routing while suspended (fixes #574).
- Runtime: unwrap `TargetInvocationException` for reflected instance member calls so JS `try/catch` can observe thrown values.

## v0.8.6 - 2026-02-09

- IL: fix maxstack estimation for inlined indexed get/set/length operations (prevents InvalidProgramException in some generated modules).
- Hosting: wrap dynamic return values so nested `dynamic` member access and method calls route through JS runtime semantics.
- Runtime: allow missing constructor arguments for dynamic `new` patterns (missing args treated as undefined/null for activation).
- Samples: default `samples/**` to build with local repo `Js2IL`/`JavaScriptRuntime` projects when available (with global tool/NuGet fallback).
- Tests: add regressions for maxstack (inlined get/set) and hosting dynamic nested return values + missing-arg construction.

## v0.8.5 - 2026-02-07

- CI: run performance comparison before samples in the Linux smoke workflow.

## v0.8.4 - 2026-02-07

- Runtime/spec: support `Function.prototype.apply` and `Function.prototype.bind` for delegate-backed function values (fixes #536).
- Docs(ecma262): update Section 20.2 and 7.3 status/notes for `apply`/`bind`.
- Runtime/spec: expose global `Object` as a first-class value (including `Object.prototype`) and support Domino-style `Fn.prototype = Object.create(Object.prototype, descriptors)` patterns (fixes #546).
- Docs(ecma262): update Sections 19.3, 20.1, and 20.2 for `Object.prototype` and function-instance `prototype` behavior.
- Runtime/spec: expose global `Error` as a first-class value (including `Error.prototype`) for CommonJS/polyfill compatibility (fixes #550).
- Docs(ecma262): update Section 20.5 status/notes for `Error.prototype`.
- IR/CommonJS: compile `module.exports = class ... extends Array { ... }` without crashing (fixes #552).
- IR/CommonJS: compile chained exports assignments (`exports = module.exports = ...`) without crashing (fixes #558).
- IR: support unary `void` operator (`void 0`) lowering (fixes #554).

## v0.8.2 - 2026-02-04

- Runtime/spec: implement minimal `Proxy` support (constructor + `get`/`set`/`has` traps) (fixes #502).
- Runtime/spec: expose standard global builtins and numeric helpers (`String`, `Number`, `Function`, `parseFloat`, `isFinite`) (fixes #528).
- Runtime/spec: support `globalThis` identifier (ECMA-262 §19.1.1) as a first-class global value (fixes #532).
- Runtime/validator: expose host timer APIs as first-class global function values (`setTimeout`, `clearTimeout`, `setInterval`, `clearInterval`) so DOM polyfills (e.g., domino WindowTimers) validate and compile successfully (fixes #534).
- Compiler/validator: add `--strictMode=Warn|Ignore` to relax missing top-level `"use strict";` directive prologues for CommonJS modules (fixes #531).
- Functions/spec: implement minimal implicit `arguments` binding for non-arrow functions (including lexical capture from nested arrow functions) and preserve full call-site arguments only when needed; mapped-arguments aliasing and full Arguments Exotic Object behaviors are not implemented.
- Docs(ecma262): update Sections 10.2 and 13.2 status/notes for `arguments` binding and arrow lexical capture.

## v0.8.1 - 2026-02-03

- Runtime/spec: add opt-in prototype-chain behavior (side-table [[Prototype]] storage) with `__proto__`, `Object.getPrototypeOf`, `Object.setPrototypeOf`, prototype-walking lookup, and prototype-aware `in` (fixes #504).
- Runtime/spec: implement descriptor-based Object APIs: `Object.create`, `Object.defineProperty`, `Object.defineProperties`, and `Object.getOwnPropertyDescriptor`, including accessor descriptors and enumerable filtering for `for...in` (fixes #503).
- Docs(ecma262): update Sections 7.3, 10.1, 13.5, and 20.1 status/notes for prototype-chain support.
- Classes/spec: support `class X extends Array` (intrinsic base class) and `super(...)` Array constructor initialization semantics (fixes #505).
- Functions/runtime: support simple parameter lists up to 32 parameters (validator + codegen delegate arity) (fixes #506).
- Runtime: simplify unknown-callable dispatch via `Delegate.DynamicInvoke` fallback (fixes #513).

## v0.8.0 - 2026-02-01

- IR: support `?.` (optional chaining) lowering in the new IR pipeline (fixes #500).
- IR: support `??` (nullish coalescing) lowering in the new IR pipeline (fixes #501).
- Runtime: expose global `Boolean` as a callable function value (enables patterns like `array.filter(Boolean)`) (fixes #511).
- Validator: report references to missing global identifiers/functions as validation errors (fixes #511).
- AsyncGenerators/spec: support `async function*` (async generator functions) and async generator objects, including consumption via `for await..of` (fixes #342).
- Debugging: optionally emit Portable PDBs (sequence points + local variable names) for stepping and improved VS Code debugging (fixes #325).

## v0.7.4 - 2026-01-30

- Hosting: project JavaScript `Promise` return values to C# `Task`/`Task<T>` for typed exports and handles (enables `await` without deadlocks).
- Hosting: compiler-generated contracts now project `async` exports as `Task`/`Task<T>`.
- Samples: add .NET library hosting samples (`samples/Hosting.Basic` and `samples/Hosting.Typed`) showing `JsEngine.LoadModule<TExports>()` with generated contracts only (fixes #406).
- Packaging/docs: ship `samples/**` inside the `js2il` tool NuGet and document how to extract and run them.
- Runtime/spec: implement ECMA-262 §9.5.1–§9.5.3 JobCallback host operations and integrate them into Promise job scheduling (fixes #435).
- Classes/spec: support `class B extends A { ... }`, `super(...)` in derived constructors, and `super.m(...)` base method calls (fixes #293, #294).
- Generators/spec: support `yield*` delegation for synchronous generators (fixes #389).
- Validator/spec: enforce and consistently surface iteration-statement early errors (break/continue targets and for-in/of head constraints) and fix labeled-statement AST traversal (fixes #463).
- Validator/spec: require a directive prologue containing `"use strict";` at the start of every module/script (PR #478).
- ControlFlow/spec: implement per-iteration lexical environments for `for (let/const ...)` loops so closures capture iteration values (fixes #461).
- ControlFlow/spec: implement per-iteration lexical environments for `for (let/const ... of ...)` and `for (let/const ... in ...)`, including destructuring loop heads (fixes #462).
- ControlFlow/spec: implement iterator-protocol `for..of` (Symbol.iterator + IteratorClose on abrupt completion) and add custom iterable compliance tests (fixes #458).
- Async/spec: implement `for await...of` using the async iterator protocol (including awaited AsyncIteratorClose on abrupt completion) and add conformance tests (fixes #341).
- ArrowFunction/spec: implement lexical `this` for arrow functions (including async arrows across `await`) (fixes #219).
- Module loading: aggregate parse/validation diagnostics across the full dependency graph and include module/file context in error output (fixes #488) (PR #509).
- Docs: add prototype-chain support strategy and document domino blocker requirements for descriptor/prototype APIs (PR #477, #508).
- Docs(ecma262): audit Sections 15 and 27 and sync coverage/status taxonomy for spec documentation (PR #475, #476).

## v0.7.3 - 2026-01-23

### Added
- Object literal enhancements: spread properties (`{ ...x }`), computed keys (`{ [expr]: value }`), shorthand properties (`{ a }`), and method definitions (`{ m() { ... } }`) (fixes #290, #291, #292).
- Synchronous generators (MVP): `function*` + `yield` lowered to a state machine with iterator-style `next()` semantics. Limitations: `yield*` and `async function*` are not supported yet; `throw/return` propagation through `try/finally` is not fully implemented (PR #388).
- Hosting: initial library hosting API scaffold (`JsEngine.LoadModule(...)`) with one dedicated script thread per runtime instance, cross-thread marshalling, and deterministic disposal boundaries (fixes #402).
- Hosting: compiled-module discovery via a compiler-emitted manifest (`JsCompiledModuleAttribute`) plus runtime discovery API and tests covering discovery and module init failure propagation (fixes #403).
- Hosting: compiler-generated strongly-typed contracts for CommonJS `module.exports` (including nested exports objects and exported classes via `IJsConstructor<T>`), enabling `JsEngine.LoadModule<TExports>()` without passing an assembly or module id (fixes #426).
### Changed
- Hosting: reduced public API surface for module discovery and dynamic exports projection (kept internal for now).
- Hosting: dynamic `JsEngine.LoadModule(Assembly, string)` now returns `IDisposable` to encourage deterministic cleanup.
### Performance
- Reduced allocations in early-bound member calls by normalizing eligible dynamic member calls into direct typed calls (avoids building `object[]` args on the fast path) (PR #433).
- Reduced casts/guards for user-class `new` expressions by emitting strongly-typed locals for `newobj` results when constructor return override is not possible (PR #433).

## v0.7.2 - 2026-01-17

### Fixed
- `parseInt` now returns an unboxed JS number (`double`) consistent with the ECMAScript spec (fixes #357).
- `Array.prototype.some` now returns an unboxed JS boolean (`bool`) consistent with the ECMAScript spec (fixes #358).
### Performance
- Reduced boxing in JavaScript `+` / `+=` hot paths by selecting typed `Operators.Add(double, object)` / `Operators.Add(object, double)` overloads when exactly one operand is an unboxed `double`.
- Stackify can now inline typed (static) numeric/boolean comparisons when operands are inlineable, reducing temp-local materialization and emitted IL size.

## v0.7.1 - 2026-01-17

### Added
- `NodeModuleRegistry` to normalize module specifiers and resolve `[NodeModule]` types for `require()`.
- Validator coverage for `require('node:path')`.
- Internal punch list for missing Node module APIs needed by scripts.
- Node core-module shims for internal scripts: `child_process`, `fs/promises`, and `os`.
- Promise thenable assimilation support for `Promise.resolve` and promise chaining (PR #366, fixes #134).
- Expanded `JavaScriptRuntime.Array` API coverage (excluding iterator / `Symbol.iterator`-related behaviors), including callback-style methods and optional `sort` comparator support.
### Fixed
- Async `await` inside `try/catch/finally` is supported, including `await` inside `finally` blocks (#340).
- `this` now works inside object literal methods when invoked via member calls (e.g., `obj.method()`).
### Performance
- Type-driven intrinsic lowering for provably-safe `Array` element access and `length` reads, reducing late-bound `Object.GetItem/SetItem/GetLength` calls in hot paths.

## v0.7.0 - 2026-01-15

### Added
- Full async/await state machine with suspension and resumption support.
- `EmitAsyncStateSwitch()` in IL compiler for dispatch to resume labels based on `_asyncState`.
- `Promise.PrependScopeToArray()` runtime helper to build modified scopes array for resumption.
- `Promise.SetupAwaitContinuation()` schedules promise.then() callbacks with MoveNext closure.
- `_moveNext` field on async scope classes holds bound closure for self-invocation.
- `_awaited{N}` fields on async scope classes store awaited results across suspension points.
- `TypeUtilities.ToBoolean(double)` runtime overload to avoid boxing when coercing unboxed numbers to boolean (`!` / `!!`).
- New unary-operator regression fixture covering NaN truthiness (`!!(0.0/0.0)` is falsy).
### Changed
- Enabled `HasAwaits=true` in HIRToLIRLower.cs to activate full state machine path.
- `await` expression now marked as "Supported" (was "Partially Supported").
- `Promise.AwaitValue()` is now only used as fallback when `HasAwaits=false` (updated docs and error message).
- Async function return handling: sets `_asyncState=-1`, resolves `_deferred.promise` with return value.
- Unary `!` / `!!` lowering now preserves unboxed numeric/bool temps so IL emission can select typed truthiness/to-boolean helpers.
- Conservative stable return CLR type inference now drives typed return emission for class instance methods (keeps `object` ABI for other callables).
- Expanded numeric inference for `-`, `*`, `/` to treat number-like operands as `double` (enables typed flows in more expressions).
### Performance
- Avoided `box System.Double` in boolean coercion and truthiness checks by calling typed helpers (`ToBoolean(double)` / `IsTruthy(double)`) when possible.
- Reduced redundant numeric roundtrips (boxing just to immediately `ToNumber(object)`) in some hot-path lowering patterns.
### Fixed
- Scope persistence for async function resumption using `isinst` check to distinguish initial vs resume calls.
- Delegate creation for `_moveNext`: properly uses `newobj Func<>` after `ldftn`.
- **Nested scope registry naming**: `ScopeNaming.GetRegistryScopeName()` now uses `scope.GetQualifiedName()` instead of `scope.Name`, fixing scope lookup for nested arrow functions and other nested callables. This was causing "scope not found in registry" errors when compiling arrow functions nested inside other functions.
- Prevented an ABI mismatch that could lead to runtime crashes when typed return lowering and object-typed call sites disagreed (typed returns remain gated to class instance methods).
### Tests
- All async tests now pass including `Async_PendingPromiseAwait` and `Async_RealSuspension_SetTimeout`.
- Async tests increased from 6 to 14 (added 4 new tests, unskipped 4 previously skipped).
- New tests: `Async_ArrowFunction_SimpleAwait`, `Async_FunctionExpression_SimpleAwait` (both pass).
- `Async_TryCatch_AwaitReject` added but skipped - await inside try/catch generates invalid IL (needs proper async exception handling).
- Added execution + generator coverage for NaN truthiness in `!!`.
- Updated generator snapshots impacted by reduced boxing and typed truthiness/to-boolean call selection.

## v0.6.5 - 2026-01-14

### Added
- Added `Promise.withResolvers()`.
- Reflection-based intrinsic discovery from `JavaScriptRuntime.GlobalThis` and `[IntrinsicObject("...")]` runtime types (explicitly excludes `Buffer` for now).
- `Boolean` intrinsic stub in `JavaScriptRuntime`.
- Integration generator test that compiles `scripts/generateNodeSupportMd.js`.
- Validator checks that reject `async`/`await` usage anywhere (not supported yet).
- Verify test module initializer to omit snapshot contents from exception output.
### Fixed
- Improved IR pipeline failure diagnostics and CommonJS wrapper parameter capture/lowering for integration scripts.
### Changed
- IR pipeline metrics now record the first failure message (instead of throwing) so it can be surfaced in higher-level errors.
- Temporarily skip `Js2IL.Tests.Async` tests until async/await support is implemented.

## v0.6.4 - 2026-01-13

### Added
- Conservative CLR type inference for JavaScript class instance fields (including user class instances and selected intrinsics), enabling strongly-typed generated fields where safe.
- LIR intrinsic normalization pass that rewrites provably-safe `Int32Array` element access into explicit typed LIR instructions.
- Unit tests for intrinsic normalization plus updated generator snapshots.
### Performance
- `Int32Array` element reads/writes can compile to direct `callvirt` to `Int32Array.get_Item(double)` / `set_Item(double, double)` when proven safe, avoiding late-bound `Object.GetItem/SetItem` in hot paths.
### Changed
- `Int32Array` indexer is now numeric (`double` in/out) to better align with unboxed numeric flows.

## v0.6.3 - 2026-01-13

### Fixed
- Destructuring now throws a Node/V8-style `TypeError` when the source value is `null` or `undefined`, including a more precise message that includes source/target binding names.
### Added
- Regression tests covering closure binding when functions escape scope via object literals and CommonJS exports (#167).
### Performance
- Inlined destructuring null/undefined guards so the runtime helper is only invoked on exceptional paths.
- Stackify can now inline `LIRIsInstanceOf` (with corresponding IL emission support), reducing unnecessary temp materialization.
- Member calls can now use an early-bound fast path: when the receiver is a generated user-class instance and the target method can be uniquely resolved by name+arity, emit `isinst` + direct `callvirt` with fallback to `Object.CallMember`.

## v0.6.2 - 2026-01-12

### Performance
- Removed redundant `castclass` in emitted IL for intrinsic globals (e.g., `GlobalThis.get_console()`).
- Avoided boxing for numeric indexed reads/writes by selecting typed runtime overloads where possible (`Object.GetItem(object, double)` / `Object.SetItem(object, double, double)`).
- Lowered safe user-class field loads/stores to direct CLR field access for `this.field` and `this["field"]` (including supported compound assignments).
- Inlined JavaScript `*`/`*=` as `ToNumber` coercions plus native IL `mul`.
_Follow-up: #311 tracks avoiding boxing typed-array read values._

## v0.6.1 - 2026-01-12

### Fixed
- Fixed temp-local liveness tracking for indexed stores (`LIRSetItem`) to prevent clobbering operands during `Object.SetItem` calls (restores correct `Int32Array` writes and fixes PrimeJavaScript invalid prime counts).
### Added
- Typed-array regression tests covering Prime bit-marking paths (optimized vs naive) and related control-flow/indexing scenarios.

## v0.6.0 - 2026-01-12

### Added
- **Phase 1 Captured Variable Reads**: IR pipeline can now read captured variables from parent scopes:
  - `LIRLoadLeafScopeField`: Loads variable from current scope's field (`ldloc.X` → `ldfld`)
  - `LIRLoadParentScopeField`: Loads variable from parent scope via scopes array (`ldarg.X` → `ldelem.ref` → `castclass` → `ldfld`)
  - Class instance methods load parent scopes via `this._scopes` field
  - Integration with `EnvironmentLayout` to determine binding storage location
  - Scope field loads are stackable and support inline emission in Stackify
- **LIRBuildArray instruction**: Optimized array initialization using dup pattern:
  - Single instruction bundles array creation + element initialization
  - Emits: `newarr` → `[dup, ldc.i4 index, ldarg/ldloc, stelem.ref]*`
  - Replaces separate `LIRNewObjectArray` + `LIRBeginInitArrayElement` + `LIRStoreElementRef` sequence
- **Phase 0 Scopes ABI Facade**: New facade types describing the scopes ABI contract for callable methods:
  - `ScopesLayoutKind`: Enum distinguishing `LegacyScopesLayout` vs `GeneralizedScopesLayout`
  - `CallableAbi`: Defines method signature shape with `ScopesSource` (None/Argument/ThisField) and `JsParamToIlArgIndex()` mapping
  - `ScopeChainLayout`: Describes `object[] scopes` array layout with deterministic outermost-first ordering
  - `BindingStorage`: Runtime storage location for bindings (`IlLocal`, `IlArgument`, `LeafScopeField`, `ParentScopeField`)
  - `EnvironmentLayout`: Complete environment specification combining ABI + scope chain + storage map
  - `EnvironmentLayoutBuilder`: Consumes `SymbolTable`/`BindingInfo` to produce `EnvironmentLayout`
  - 32 new tests verifying ABI contracts, scope-chain ordering, and `_scopes` parameter presence
- **ScopeMetadataRegistry**: New minimal interface for scope and field handle lookups extracted from `VariableRegistry`:
  - Contains `_scopeTypes`, `_scopeFields`, `_allScopeTypes` collections
  - Methods: `RegisterScopeType()`, `RegisterField()`, `GetScopeTypeHandle()`, `GetFieldHandle()`
  - `TryGet` variants for safer handle lookups without exceptions
  - Registered as singleton in DI container
- **Stackify optimization pass**: New analysis pass that identifies temps which can remain on the evaluation stack instead of being stored to IL locals:
  - `Stackify.Analyze()` identifies single-use temps consumed immediately after definition
  - `StackifyResult.IsStackable()` provides O(1) lookup for temp stackability
  - Checks for control flow barriers (branches/labels) between definition and use
  - `CanEmitInline()` validates instruction types support inline emission
  - Integrated with `LIRToILCompiler` via `MarkStackifiableTemps()`
- **IR pipeline function parameter support**: Functions with simple identifier parameters can now be compiled via the IR pipeline:
  - New `LIRLoadParameter` instruction for loading function parameters by index
  - New `LIRCallFunction` instruction for calling user-defined functions with arguments
  - `MethodDescriptor.HasScopesParameter` flag distinguishes Main (no scopes) from user functions (scopes as arg0)
  - Parameter index mapping: JS param 0 → IL arg 1 for user functions, IL arg 0 for Main
  - `AllParamsAreSimpleIdentifiers` check gates IR compilation for functions with complex parameters
- **Two-phase compilation pipeline**: New default compilation mode that discovers callables first, then compiles bodies:
  - `CallableRegistry` tracks callable IDs, stable names, signatures, and MethodDef tokens
  - Dependency discovery + planning (`CallableDependencyCollector`/`CompilationPlanner`) to ensure bodies compile in a deterministic order
  - Integration tests for dependency planning and two-phase compilation
- **Lowering pipeline feature expansion (AST → HIR → LIR → IL)**:
  - Control flow: `for`, `for..of`, `for..in`, `do..while`, labeled break/continue, `switch`, `try/catch/finally`, `throw`
  - Expressions: template literals (PL3.4), `this` (PL3.5), function expressions as values (PL3.6), arrow functions (including ctor arity checks)
  - Operators: binary operators (including logical short-circuit), unary operators (`typeof`, `!`, `~`, numeric `-`), update operators (`++`/`--`)
  - Destructuring + assignment targets: destructuring declarations and destructuring assignment; member/index assignment targets
  - Calls: lowered member calls via `Object.CallMember` and improved GlobalThis intrinsic call handling
  - `new` expressions: intrinsics, built-in Error types, and user-defined class instantiation
- **JavaScript runtime additions**:
  - `Number` intrinsic implementation
  - `JsThrownValueException` for propagating non-Exception thrown values through .NET
### Changed
- **ScopeMetadataRegistry wired through compilation pipeline**: `ScopeMetadataRegistry` now flows through full compilation:
  - `LIRToILCompiler` receives `ScopeMetadataRegistry` for field handle lookups during IL emission
  - `HIRToLIRLower` consults `EnvironmentLayout` to determine binding storage location
  - Enables LIR instructions to reference scope fields without accessing legacy `VariableRegistry`
- **VariableRegistry refactored as facade**: `VariableRegistry` now wraps `ScopeMetadataRegistry` for backward compatibility:
  - Constructor accepts `ScopeMetadataRegistry` (creates default if not provided)
  - Exposes `ScopeMetadata` property for direct access to handle registry
  - All handle-related methods (`GetScopeTypeHandle`, `GetFieldHandle`, `EnsureScopeType`) delegate to `_scopeMetadata`
  - Registered as singleton in DI container, receives `ScopeMetadataRegistry` via constructor injection
  - Full backward compatibility maintained - all public methods unchanged
- **DI-based VariableRegistry wiring**: `VariableRegistry` and `ScopeMetadataRegistry` now managed via dependency injection:
  - Both registered as singletons in `CompilerServices`
  - `AssemblyGenerator` receives `VariableRegistry` via constructor injection (no longer creates it)
  - `TypeGenerator` receives `VariableRegistry` via constructor parameter (no longer uses `new VariableRegistry()`)
  - Removed `TypeGenerator.GetVariableRegistry()` method (no longer needed)
  - `EnvironmentLayoutBuilder` depends on `ScopeMetadataRegistry` (minimal interface, not full `VariableRegistry`)
- **Console.log lowering and stackification**: Intrinsic receivers can remain on the evaluation stack (avoids materializing lookups like `GlobalThis.console` into locals)
- **IR pipeline coverage expanded**: Many more statement/expression forms now compile via IR by default, with explicit fallback assertions and improved failure diagnostics
- **Classes compilation improvements**: Class constructor bodies can be compiled via the IR pipeline (with automatic fallback to legacy emission)
- **Performance improvements**:
  - Stackify keeps intrinsic receivers on the evaluation stack where possible
  - Inline stackification for `LIRConcatStrings`
- **Cleanup and maintenance**:
  - Removed legacy IL generator implementation files (keeping the IR pipeline as the primary path)
  - Removed the `samples/` folder
  - Removed `CompiledMethodCache` and the legacy `ConsoleLogPeepholeOptimizer`
  - Added scripts to run execution/generator tests and report failures
### Fixed
- **Main method parameter indexing**: Fixed `InvalidProgramException` for `__dirname`/`__filename` access in Main:
  - Main has no scopes array parameter, so JS param 0 maps to IL arg 0 (not arg 1)
  - `HasScopesParameter = false` for Main ensures correct `ldarg.X` indices
- **Validator edge case for arrow functions**: Correctly detects `this` usage in arrow functions inside class methods
- **IR pipeline nested-function correctness**: IR compilation is enabled for nested functions with additional optimization for single-assignment variables
### Known Issues
- **IR adoption remains partial**: Many JavaScript constructs still fall back to the legacy emitters; `IRPipelineMetrics` tests track coverage and regressions over time

## v0.5.4 - 2026-01-02

### Added
- **IR pipeline if-statement support**: Full support for `if`/`else` statements in the new AST→HIR→LIR→IL pipeline:
  - New HIR node: `HIRIfStatement` with `Test`, `Consequent`, and optional `Alternate` properties
  - New LIR instructions: `LIRBranchIfFalse`, `LIRBranch`, `LIRLabel` for control flow
  - Proper IL emission with conditional branching (`brfalse`, `br`) and label resolution
  - Supports nested if-else chains and block statements
### Fixed
- **Variable shadowing in IR pipeline**: Block-scoped variables with the same name now correctly get separate IL local slots:
  - Changed `_variableMap` and `_variableSlots` in `HIRToLIRLower` to key by `BindingInfo` reference instead of variable name string
  - Added `_currentScope` tracking in `HIRBuilder` to resolve shadowed variables to the correct binding
  - Each `let`/`const` declaration creates a unique `BindingInfo`, enabling correct identity comparison
  - Example: `let x = 1; { let x = 2; }` now generates 2 IL locals instead of incorrectly sharing 1
### Changed
- **Cleaner if-statement lowering**: Refactored `HIRIfStatement` handling in `HIRToLIRLower`:
  - Combined duplicate `if (Alternate != null)` checks
  - Deferred `endLabel` creation until needed (avoids wasted label IDs when no else block)
- **IR pipeline comparison operators**: Full support for comparison operators in the new AST→HIR→LIR→IL pipeline:
  - New LIR instructions: `LIRCompareNumberLessThan`, `LIRCompareNumberGreaterThan`, `LIRCompareNumberLessThanOrEqual`, `LIRCompareNumberGreaterThanOrEqual`, `LIRCompareNumberEqual`, `LIRCompareNumberNotEqual`, `LIRCompareBooleanEqual`, `LIRCompareBooleanNotEqual`
  - HIR→LIR lowering for `==`, `===`, `!=`, `!==`, `<`, `>`, `<=`, `>=` operators
  - Proper IL emission using `ceq`, `clt`, `cgt` instructions
- **Variable storage type tracking**: Added `VariableStorages` list to `MethodBodyIR` for tracking CLR types of JavaScript variables:
  - Variables now get properly typed IL locals (bool, double, string, object) instead of defaulting to double
  - Comparison results stored in bool-typed locals for correct semantics
- **Constant inline emission optimization**: Constants can now be emitted directly on the stack without local allocation:
  - `CanEmitInline` check in `TempLocalAllocator` skips slot allocation for `LIRConstNumber`, `LIRConstString`, `LIRConstBoolean`, `LIRConstUndefined`, `LIRConstNull`
  - `EmitLoadTemp` emits unmaterialized constants inline
  - Result: `var x = 1 == 2` generates 2 bool locals instead of 4 (2 bool + 2 float64)
- **Pure SSA LIR IR pipeline (experimental)**: New Low-level Intermediate Representation with pure SSA semantics for IL code generation:
  - All operations use `TempVariable` (SSA temps), eliminating mutable local variable concepts at the LIR level
  - `TempLocalAllocator` performs linear-scan register allocation mapping temps to IL locals
  - Peephole optimization framework for pattern-based IL optimization
- **Multi-argument console.log peephole optimization**: Extended stack-only emission to handle N-argument console.log calls:
  - `TryEmitConsoleLogPeephole` handles console.log with any number of arguments (previously only 1)
  - `TryMatchConsoleLogMultiArgSequence` matches console.log IR patterns with N arguments
  - `ComputeStackOnlyConsoleLogPeepholeMask` identifies temps consumed by peepholes to exclude from allocation
  - Result: Functions like `console.log("Hello", 2)` now emit 0 locals instead of 5
- **LIRSubNumber instruction**: Added subtraction instruction for decrement (`--`) operations:
  - Emits `IL_sub` instead of `add -1` for cleaner IL
- **MemberReferenceRegistry.GetOrAddField**: Added field reference caching to avoid duplicate metadata entries
### Changed
- **IL optimization for console.log**: Multi-argument console.log calls now use efficient `dup`/stack pattern:
  - Before: 56 bytes, 5 locals (Console, object[], string, float64, object)
  - After: 44 bytes, 0 locals (pure stack operations)
### Internal
- Refactored `TryMatchConsoleLogOneArgSequence` → `TryMatchConsoleLogMultiArgSequence` for N-argument support
- Removed `CanEmitConsoleLogArgStackOnly` (functionality merged into multi-arg matcher)
- Extended `CanEmitTempStackOnly` to handle variable-mapped temps via local slot loading
- **IR pipeline support for class constructors**: Extended IR compilation pipeline to handle class constructors with automatic fallback:
  - `HIRBuilder` now handles `FunctionExpression` nodes (used by class constructor bodies)
  - `JsMethodCompiler.TryCompileClassConstructor` attempts IR compilation with fail-fast guards
  - `ClassesGenerator.EmitConstructor` tries IR pipeline first, falls back to legacy emitter
  - Fail-fast conditions: `needsScopes`, parameters > 0, field initializers > 0
  - Returns both method handle and signature to avoid duplicate signature creation
- **IR pipeline support for arrow functions**: Extended `HIRBuilder` to handle `ArrowFunctionExpression` nodes with fallback guards:
  - Block-body parameterless arrow functions now compile through IR pipeline
  - Arrow functions with parameters fall back to legacy emitter (parameters not yet supported in IR)
  - Concise expression-body arrow functions fall back to legacy emitter (return value handling not yet implemented)
- **HIRReturnStatement**: New HIR node for explicit return statements in method bodies
- **LIRReturn instruction**: Low-level IR instruction for method returns, handled by `JsMethodCompiler`
- **IR pipeline support for boolean literals**: Extended IR pipeline to handle `true` and `false` literals:
  - Added `BooleanLiteral` case to `HIRBuilder.TryParseExpression`
  - Added `LIRConstBoolean` instruction for boolean constants
  - Added `JavascriptType.Boolean` case to `HIRToLIRLower`
  - Extended `LIRConvertToObject` with `SourceType` parameter for proper boxing (bool vs double)
  - IR metrics improved: 4.1% adoption (23/555 methods), 532 legacy fallbacks
- **IR pipeline support for null/undefined literals**: Extended IR pipeline to handle `null` and `undefined`:
  - Added `Literal` with null value case to `HIRBuilder.TryParseExpression` for JavaScript `null`
  - Added special handling for `undefined` identifier in `HIRBuilder.TryParseExpression`
  - Added `LIRConstNull` instruction for JavaScript null (boxed `JsNull.Null`)
  - Added `JavascriptType.Null` and `Unknown` cases to `HIRToLIRLower`
  - IL optimization: `undefined` now emits direct `ldnull` instead of `GlobalThis::Get("undefined")` call
- **IR pipeline support for unary `typeof`**: Extended IR pipeline to compile `typeof` expressions:
  - Added `HIRUnaryExpression` parsing in `HIRBuilder`
  - Added `LIRTypeof` instruction and lowering support in `HIRToLIRLower`
  - `JsMethodCompiler` emits a direct call to `JavaScriptRuntime.TypeUtilities.Typeof(object)`
- **IR pipeline support for unary/update operators**: Extended IR pipeline to compile unary `-`, unary `~`, and update expressions (`++`/`--`):
  - Added `HIRUpdateExpression` and parsing of `UpdateExpression` nodes in `HIRBuilder`
  - Added `LIRNegateNumber` and `LIRBitwiseNotNumber` instructions with lowering support in `HIRToLIRLower`
  - `JsMethodCompiler` emits IL for numeric negation and bitwise NOT
  - Generator tests can now assert on IR fallback with improved diagnostics (`IRPipelineMetrics.GetLastFailure()`)
### Changed
- **IL optimization**: IR pipeline eliminates unnecessary `castclass` instructions when loading intrinsic globals (e.g., `console`), producing smaller method bodies
### Internal
- Extended `HIRToLIRLower` to lower `HIRReturnStatement` to `LIRReturn` instructions
- `JsMethodCompiler` tracks explicit returns to avoid emitting redundant implicit return IL
- **IR pipeline metrics**: Added `IRPipelineMetrics` class to track IR vs legacy compilation statistics
  - Instrumented all IR pipeline call sites (main methods, functions, arrow functions, class methods, constructors)
  - Added `IRPipelineAuditTests` with two audit modes:
    - `ReportIRPipelineMetrics`: Quick check with representative test cases
    - `FullSuiteIRPipelineMetrics`: Comprehensive scan of all 300+ embedded test files
  - Baseline metrics (full suite): 4.0% IR adoption (22/555 methods), 533 legacy fallbacks
  - Run audit with: `dotnet test --filter "FullSuiteIRPipelineMetrics" --logger "console;verbosity=detailed"`

## v0.5.3 - 2025-12-30

### Added
- **Pre-compile JavaScript validator improvements**: Enhanced `JavaScriptAstValidator` with comprehensive checks for unsupported JavaScript features:
  - Rest parameters (`...args` in function declarations)
  - Spread in function calls (`fn(...arr)`)
  - Array destructuring in variable declarations (`const [a, b] = arr`)
  - Object rest properties (`const {a, ...rest} = obj`)
  - Nested destructuring patterns (`const {inner: {x}} = obj`)
  - for...in loops
  - switch statements
  - with statements (deprecated)
  - Labeled statements (`label: for (...)`)
  - debugger statements
  - new.target / import.meta meta properties
  - super keyword
  - Getter/setter properties in object literals and classes
  - Computed property names (`{[expr]: value}`)
  - Destructuring assignment (not in declarations)
### Changed
- **Validation directory structure**: Moved `JavaScriptAstValidator.cs` and `IAstValidator.cs` to new `Js2IL/Validation/` namespace for better organization
- **AstWalker improvements**: Extended AST traversal to visit class declarations, class bodies, method definitions, property definitions, object patterns, array patterns, and rest elements
### Internal
- Moved `AstWalker.cs` to `Js2IL/Utilities/` namespace
### Added (continued)
- **Experimental IR Compilation Pipeline**: Introduced multi-tier intermediate representation (IR) for JavaScript to IL compilation:
  - **HIR (High-level IR)**: AST-level representation with typed nodes (`HIRMethod`, `HIRBlock`, `HIRStatement`, `HIRExpression`) preserving JavaScript semantics
  - **LIR (Low-level IR)**: Stack-machine IR closely mapping to IL instructions (`LIRInstruction` with opcodes like `LIRAddNumber`, `LIRNewObjectArray`, `LIRStoreElementRef`)
  - **Three-phase pipeline**: AST → HIR (`HIRBuilder`) → LIR (`HIRToLIRLowerer`) → IL (`JsMethodCompiler`)
  - **Fallback mechanism**: Methods that fail IR compilation fall back to legacy direct AST-to-IL generator
  - Currently supports basic scenarios: variable declarations, binary expressions, console.log calls with numeric literals and arithmetic
  - Foundation for future optimizations: dead code elimination, constant folding, type propagation, and register allocation
### Changed
- **Assembly generation architecture**: `AssemblyGenerator` now attempts IR-based compilation first via `JsMethodCompiler.TryCompileMethod()` before falling back to legacy `MainGenerator` path
- **Dependency injection**: Added `JsMethodCompiler` as transient service in `CompilerServices`
### Internal
- New directory structure: `Js2IL/IR/` with `HIR/` and `LIR/` subdirectories
- `MethodBodyIR` class encapsulates method body with instructions and local variables
- `HIRBuilder` converts AST nodes to HIR representation
- `HIRToLIRLowerer` performs SSA-style lowering with temporary variables
- `JsMethodCompiler` orchestrates the full pipeline and emits final IL
_Note: This is experimental infrastructure. Full feature parity with the legacy generator is planned for future releases._

## v0.5.2 - 2025-12-24

### Fixed
- **IL generation for equality comparison with ToNumber**: Fixed `AccessViolationException` when comparing two object-type values (e.g., `knownPrimeCount == countedPrimes`). After calling `TypeUtilities.ToNumber()` which returns native float64, the code was incorrectly emitting `unbox.any` on the already-unboxed result. Added tracking flag to skip redundant unbox operations. This fixes PrimeJavaScript.js benchmark execution.

## v0.5.1 - 2025-12-24

### Fixed
- **IL generation for typed locals**: Fixed incorrect IL emission when storing boxed values (from `Operators.Add` and other runtime helpers) to strongly-typed float64 locals. Values are now properly unboxed with `unbox.any` before `stloc`. Also fixed `ExpressionResult.IsBoxed` tracking through binary expressions and identifier loads, ensuring correct boxing state propagates through expression emission. This resolves issues with arithmetic operations in loops and compound assignments producing incorrect results.

## v0.5.0 - 2025-12-24

### Added
- **CommonJS Module object**: Implemented Node.js-compatible `module` object per CommonJS specification (Fixes #164):
  - `module.exports`: The authoritative export value, properly aliased with `exports` parameter initially
  - `module.id`, `module.filename`, `module.path`: Module identity properties
  - `module.loaded`: Boolean indicating module load completion
  - `module.parent`, `module.children`: Parent-child relationship tracking across module dependencies
  - `module.paths`: Array of node_modules search paths following Node.js algorithm
  - `module.require()`: Bound require function on module object
- **CommonJS module tests**: Added 16 execution + generator tests for module object features
### Fixed
- **CommonJS require caching**: Local modules are now cached so shared dependencies execute only once per process (e.g., `b -> d` and `c -> d` only run `d` once). (Fixes #157, Fixes #123)
- **CommonJS relative require resolution**: `require('./...')` and `require('../...')` inside a module now resolve relative to the requiring module.
- **CommonJS cross-module function calls**: Calling exported functions from another module now works correctly. `Object.CallMember` handles `ExpandoObject` receivers by invoking delegate properties via `Closure.InvokeWithArgs`. (Fixes #156)
- **Test harness path normalization**: CommonJS tests now support nested module paths by normalizing embedded resource names, expected DLL naming, and mock filesystem path casing/separators.
- **Math operations alignment**: `Math.round()`, `Math.trunc()`, and `Math.imul()` now match Node.js behavior by removing custom `-0` handling that caused divergent outputs.
### Changed
- **Console dependency injection refactoring**: Console migrated from static ThreadLocal fields to constructor-injected `ConsoleOutputSinks` pattern:
  - Removed `Console.SetOutput()` and `Console.SetErrorOutput()` static methods
  - Console now resolved via `ServiceProvider.Resolve<Console>()` with DI-injected sinks
  - Test infrastructure updated to use `ServiceContainer` with per-test `ConsoleOutputSinks` instead of static setters
  - Enables thread-safe per-test Console isolation, removing static state blocking parallel execution
  - **Breaking Change**: Tests must configure Console via dependency injection rather than static methods
- **CommonJS module identity**: Compiler/runtime now use stable path-based module ids for generated type names to avoid basename collisions (e.g., `./b` vs `./helpers/b`).
- **Console array formatting**: Strings in console arrays now quoted with single quotes and special characters escaped to match Node.js output.
### Added
- **CommonJS regression tests**: Added execution + generator coverage for nested name conflicts, relative-from-module requires, and shared-dependency caching.
- **Node timers setImmediate/clearImmediate**: Added `setImmediate(callback, ...args)` and `clearImmediate(handle)` with FIFO ordering, cancellation support, and nested immediates running on the next iteration. (Fixes #124)
- **Node timers setInterval/clearInterval**: Added `setInterval(callback, delay, ...args)` and `clearInterval(handle)` with repeating timer support, proper cancellation handling, and integration with the event loop scheduler. (Fixes #125)
- **Snapshot management script**: Added `scripts/syncExecutionSnapshots.js` for execution test snapshot synchronization.

## v0.4.2 - 2025-12-18

### Added
- **Promise Combinators**: Implemented `Promise.all`, `Promise.allSettled`, `Promise.any`, and `Promise.race` static methods
  - `Promise.all(iterable)`: Returns a Promise that resolves when all input promises resolve, or rejects when any rejects
  - `Promise.allSettled(iterable)`: Returns a Promise that resolves when all input promises have settled (fulfilled or rejected)
  - `Promise.any(iterable)`: Returns a Promise that resolves as soon as any input promise resolves, or rejects with AggregateError if all reject
  - `Promise.race(iterable)`: Returns a Promise that settles as soon as any input promise settles
  - Shared `Combine` helper method centralizes iteration and handler wiring for all combinator methods
  - Handles non-Promise values in iterables by wrapping them via `Promise.resolve`
  - Supports strings as iterables (character-by-character)

## v0.4.1 - 2025-12-15

### Changed
- **Performance Optimization - Unboxed Uncaptured Variables**: Implemented static type inference for uncaptured local variables, eliminating boxing overhead for primitive types (numbers, strings, booleans). Variables that are not captured by closures and maintain a stable type throughout their scope are now stored directly as their CLR types (e.g., `double`, `string`, `bool`) instead of being boxed as `System.Object`. This optimization:
  - Adds new `InferVariableClrTypes` analysis pass in `SymbolTableBuilder` that tracks variable initialization and assignments to determine stable types
  - Introduces `IsStableType` flag on `BindingInfo` to prevent type changes after inference
  - Modifies IL generation to emit unboxed loads/stores for typed local variables, avoiding box/unbox instructions
  - Applies only to uncaptured variables (captured variables remain boxed for closure compatibility)
  - Significantly reduces memory allocations and improves runtime performance for numeric-intensive code
  - Includes comprehensive test coverage in `SymbolTableTypeInferenceTests` with 8 test cases covering literals, binary expressions, assignments, conflicts, and mixed scenarios
- **Extended Type Inference to Block Scopes in Class Methods**: Type inference now applies to variables declared in block scopes (for loops, while loops, if/else blocks, try/catch/finally, switch cases) within class methods. Previously, only variables at the direct function scope level were typed. This enables unboxed locals for loop iterator variables and intermediate calculations in nested control flow. The `isBlockScopeInClassMethod()` helper walks up the scope tree to verify the block is within a class method without crossing intermediate function boundaries.
### Fixed
- **Compound Assignment Bug in Class Methods**: Fixed incorrect IL generation for compound assignments (e.g., `+=`, `|=`) in class instance methods. Previously, the generator attempted to load the scope instance from `ldloc.0` for parent scope access in methods, but instance methods don't have a scope instance local—they receive scope arrays via parameters. The fix ensures compound assignments in class methods correctly load parent scopes from the `scopes` parameter array using `ldarg` + `ldelem_ref` + `castclass`, matching the pattern used for other variable operations in methods.
- **Block-Scope Local Variable Type Lookup**: Fixed `GetLocalVariableType` failing to find block-scope uncaptured variables, causing them to default to `System.Object` instead of their inferred CLR type. Block-scope variables are intentionally not cached in `_variables` (to support proper shadowing via lexical scope stack), but `GetLocalVariableType` needs to find them by slot index to emit typed locals. Added `_blockScopeLocalsBySlot` dictionary cache, populated by `TryResolveFieldBackedVariable` when resolving uncaptured block-scope variables. This fix enables `setBitsTrue` (PrimeJavaScript benchmark) to have 9 `float64` locals instead of 1, eliminating boxing overhead for bitwise operations in nested loops.
### Added
- **Bitwise NOT Operator (`~`)**: Implemented the unary bitwise NOT operator. The IL emission converts the operand to int32, applies the NOT instruction, and converts back to double. Type inference for bitwise NOT was already in place; this completes the implementation.
- **Unit Tests for Variables Class**: Added 12 comprehensive unit tests in `VariablesTests.cs` covering:
  - Block-scope variable resolution with correct CLR type propagation
  - Unique local slot allocation for variables in different block scopes
  - Nested block scope shadowing (inner scope shadows outer scope variable of same name)
  - `GetLocalVariableType` correctly returning `float64` for stable double-typed block-scope locals
  - Multiple block scopes with same variable name getting different slots and types
  - Non-stable and untyped variables correctly returning null from `GetLocalVariableType`
  - Lexical scope stack push/pop ordering
  - Integration test simulating `setBitsTrue` pattern with 5 `float64` variables across 2 block scopes
- **Block-Scope Type Inference Tests**: Added 15 new test cases in `SymbolTableTypeInferenceTests.cs` covering type inference for variables in:
  - For loops (iterator and body variables), for...in loops, for...of loops
  - While loops, do-while loops
  - If/else blocks, try/catch/finally blocks, switch case blocks
  - Nested block scopes (2-3 levels deep): for inside if, while inside for, if inside while inside if
- **InternalsVisibleTo**: Added `InternalsVisibleTo` attribute to `Js2IL.csproj` to expose internal classes (`Variables`, `Variable`, `VariableRegistry`) to the test project for comprehensive unit testing.

## v0.4.0 - 2025-12-14

### Added
- **Promise Support**: Implemented Promise/A+ semantics with constructor, static methods, and prototype methods
  - `new Promise(executor)`: Constructor accepts executor function with resolve/reject callbacks
  - `Promise.resolve(value)`: Returns a Promise resolved with the given value
  - `Promise.reject(reason)`: Returns a Promise rejected with the given reason
  - `Promise.prototype.then(onFulfilled, onRejected)`: Registers fulfillment and rejection handlers, returns a new Promise
  - `Promise.prototype.catch(onRejected)`: Registers rejection handler, returns a new Promise
  - `Promise.prototype.finally(onFinally)`: Registers cleanup handler that executes regardless of settlement state, returns a new Promise
  - Microtask scheduling integration via `EngineCore.IMicrotaskScheduler` for proper asynchronous execution
  - Promise chaining and handler return value propagation (including nested Promise resolution)
  - 15 comprehensive tests covering executor patterns, chaining, error handling, and microtask scheduling

### Fixed
- **Promise finally handlers**: Fixed bug where Promise returns from finally handlers were incorrectly masked. Finally handlers are now treated as observers—non-Promise return values don't alter the settled result, but returned Promises are properly awaited and propagated to the next handler in the chain.
- **Binary operator type coercion**: Fixed strict equality (`===`) comparisons in logical OR patterns (e.g., `id === 1024 || id === 2047`) when comparing captured/boxed variables to numeric literals. The IL generator now correctly applies `ToNumber` conversion when the variable type is `Unknown` (boxed in scope fields), preventing incorrect direct `object`-to-`double` `ceq` comparisons that would always fail. Added regression test `BinaryOperator_StrictEqualCapturedVariable`.

### Changed
- **Runtime organization**: Reordered members in `JavaScriptRuntime/Promise.cs` to follow StyleCop conventions (nested types, fields, constructors, public methods, private methods)
- **Test organization**: Alphabetically sorted test methods in `BinaryOperator` and `Promise` test classes for consistency

### Documentation
- Updated `docs/ECMA262/FeatureCoverage.json` with new Promise section (27.1) documenting constructor, static methods, and prototype methods
- Enhanced Binary || operator notes to document the strict-equality type coercion fix for captured variables

## v0.3.6 - 2025-12-11

### Added
- **Timers API**: Implemented `setTimeout` and `clearTimeout` global functions following Node.js timer semantics
  - `setTimeout(callback, delay, ...args)`: Schedules a callback to execute after a specified delay in milliseconds, returns a timer handle
  - `clearTimeout(handle)`: Cancels a previously scheduled timer
  - Timer implementation uses `IScheduler` and `ITickSource` abstractions for testability and flexible execution environments
  - Added `Timers` internal class to manage timer lifecycle via the scheduler
  - Test infrastructure includes `MockTickSource` and `MockWaitHandle` for deterministic timer testing
  - Comprehensive test coverage: `SetTimeout_ZeroDelay`, `SetTimeout_MultipleZeroDelay_ExecutedInOrder`, `SetTimeout_OneSecondDelay`, `ClearTimeout_ZeroDelay`, `ClearTimeout_MultipleZeroDelay_ClearSecondTimer`
  - Updated `NodeSupport.json` documentation with timer API details and test references
### Fixed
- **Variable Capture Optimization**: Block scopes (while/for/if bodies) no longer cause parent function variables to be incorrectly marked as "captured". Previously, any variable referenced from a child scope—including block scopes—was marked as captured and stored in a scope class instance. Now only function and class scopes trigger variable capture, since block scopes don't create closures. This eliminates unnecessary scope class allocations in functions with loops or conditionals, significantly improving performance for hot paths like the `setBitTrue`/`testBitTrue` methods in the prime sieve benchmark.
### Changed
- **Install Script**: Updated `scripts/installLocalTool.js` to use `dotnet pack` instead of `dotnet publish` and clear the tool store cache before reinstalling, ensuring the newest version is always installed.
- **Code Quality**: Refactored timer initialization in `GlobalThis` to use shared `EnsureTimers()` helper method, eliminating code duplication between `setTimeout` and `clearTimeout`

## v0.3.5 - 2025-12-06

### Changed
- **Architecture**: Comprehensive refactoring of IL metadata generation infrastructure for centralized registry pattern
  - **AssemblyReferenceRegistry**: Introduced centralized assembly reference management to eliminate duplicate assembly references in emitted metadata. Single `AssemblyReferenceHandle` per referenced assembly per `MetadataBuilder` using `ConditionalWeakTable` for lifetime management.
  - **TypeReferenceRegistry**: Extracted from `BaseClassLibraryReferences` into shared singleton pattern. Both `BaseClassLibraryReferences` and `Runtime` now share the same `TypeReferenceRegistry` instance to avoid duplicate type references.
  - **MemberReferenceRegistry**: New centralized registry for `MemberReferenceHandle` creation with automatic signature building via reflection. Supports:
    - Constructed generic types (e.g., `Func<object[], object, object>`) with automatic `TypeSpecification` creation
    - Generic type parameter encoding (!0, !1, !2) for proper method signatures on open generic definitions
    - Auto-discovery of method/constructor signatures from reflection
    - Smart declaring type resolution (TypeReference for simple types, TypeSpecification for constructed generics)
  - **On-Demand Pattern**: Converted all BCL member and type references to on-demand getters that invoke registry methods directly, eliminating manual initialization code:
    - Type references: `BooleanType`, `DoubleType`, `Int32Type`, `ObjectType`, `StringType`, `ExceptionType`, `SystemMathType`, `MethodBaseType`
    - Constructor references: `Expando_Ctor_Ref`, `Action_Ctor_Ref`, all Func delegate constructors
    - Method references: `IDictionary_SetItem_Ref`, `Array_Add_Ref`, `Array_SetItem_Ref`, `Array_GetCount_Ref`, `MethodBase_GetCurrentMethod_Ref`, all Func `Invoke` methods
  - **BaseClassLibraryReferences**: Reduced from ~90 lines of manual BlobBuilder initialization to 5-line constructor. Now serves as clean facade over registry classes.
  - **Code Reduction**: Deleted ~250+ lines of initialization code, duplicate metadata handling, and manual signature building
  - **Runtime**: Shared `MemberReferenceRegistry` instance across `BaseClassLibraryReferences` and `Runtime` for consistent metadata generation
  - **Type Specifications Cache**: `_typeSpecCache` in `MemberReferenceRegistry` eliminates duplicate TypeSpec entries for constructed generic types
- **Runtime Organization**: Renamed `GlobalVariables` to `GlobalThis` and introduced `ModuleIntrinsics` class for better separation of global scope vs module-level intrinsics
- **Event Loop Foundation**: Added scaffolding for future async/event loop support:
  - `Engine.Execute()` method as placeholder for event loop integration
  - Thread-safe scheduler state + single-threaded event loop pump for async coordination
### Fixed
- **Metadata Generation**: Generic method signatures now correctly use generic type parameters (!0, !1, !2) instead of concrete types, fixing `System.MissingMethodException` for `Func<...>.Invoke` methods
- **IL Correctness**: Eliminated potential metadata corruption from duplicate assembly/type/member references

## v0.3.4 - 2025-12-01

### Changed
- IL Generation: scope local variables are now strongly-typed as their scope class instead of System.Object. This eliminates unnecessary castclass instructions after ldloc operations, improving performance and reducing IL size. The type information is determined at signature creation time using metadata lookups, with non-scope locals defaulting to Object type.
### Fixed
- Compound bitwise assignments: fixed RHS type coercion bug when the right-hand side is a scope variable (let/const in class methods). Scope variables are stored as boxed objects in scope fields, and the RHS was not being properly unboxed before bitwise operations (|=, &=, ^=, <<=, >>=, >>>=). The generated IL code was loading the RHS as a boxed object then directly converting to int32 with `conv.i4`, which treated the object reference itself as an integer producing garbage values. Fixed by using `CoerceToInt32` (same pattern as LHS) to safely unbox and convert the RHS value. Added test case `CompoundAssignment_LocalVarIndex` that reproduces the bug with scope variables in compound assignments.
- Equality comparisons: fixed object-to-object equality comparisons by adding type coercion when comparing two non-literal, non-numeric, non-boolean values (likely boxed objects). When both operands are not numbers or booleans and neither is a literal, the IL generator now converts both to numbers using `TypeUtilities.ToNumber()` before comparison, ensuring value equality instead of reference equality. This handles cases where type tracking returns Unknown or Object, including dynamic property lookups (e.g., `obj[this.prop]`), method return values, and variable comparisons. Added test case `BinaryOperator_EqualObjectPropertyVsMethodReturn`.
- Equality comparisons: fixed parameter equality comparisons by adding type coercion when comparing function parameters (boxed as Object) with numeric literals. The IL generator now emits `TypeUtilities.ToNumber()` call to convert boxed parameters to double before comparison with numeric literals, ensuring `ceq` instruction operates on compatible types.

## v0.3.3 - 2025-11-28

### Added
- Compound Assignment Operators: implemented all 11 compound assignment operators with proper type conversions:
  - Bitwise operators: `|=`, `&=`, `^=`, `<<=`, `>>=`, `>>>=` (convert to int32, apply operation, convert back to double)
  - Arithmetic operators: `-=`, `*=`, `/=`, `%=`, `**=` (operate on double values, exponentiation uses System.Math.Pow)
  - Extensible architecture with pattern matching for easy addition of future operators
  - Works with scope variables (local and global)
  - Comprehensive test coverage with 11 test cases verifying execution results and IL generation
- Tests: added `CompoundAssignment` test group with JavaScript test files and snapshot verification for all compound operators
- Tests: added `BinaryOperator_LeftShiftBit31` test to verify left shift of bit 31 produces correct signed result
- Runtime: added `Object.CoerceToInt32` public static method for safe type conversion following JavaScript semantics (handles null, numeric types, strings, booleans)

### Fixed
- Int32Array: fixed `InvalidCastException` when calling Int32Array methods via reflection by changing indexer signature from `int this[int]` to `object this[object]`. Indexer now returns boxed double values to match JavaScript number semantics.
- IL Generation: added `CoerceToJsNumber` helper in `Object.CallInstanceMethod` to convert all primitive numeric CLR types (int, float, long, short, byte, etc.) to double before reflection invoke, ensuring type compatibility.
- IL Generation: updated Int32Array fast path to pass boxed values to get_Item/set_Item methods instead of unboxing parameters.
- IL Generation: null values now coerce to 0.0 in numeric contexts instead of passing through as null, matching JavaScript semantics.
- Compound Assignments: added support for compound bitwise operations (`|=`, `&=`, `^=`, `<<=`, `>>=`, `>>>=`) on dynamically-accessed array elements (e.g., `this.array[i] |= value`). Operations now correctly: 1) get current value, 2) apply operation, 3) store result, instead of replacing the value.
- IL Generation: dynamic fallback path now uses `CoerceToInt32` instead of unsafe `Unbox_any` cast, preventing `InvalidCastException` for non-numeric objects.
- IL Generation: extracted duplicate compound operator mapping into `GetCompoundBitwiseOpCode` helper method to eliminate code duplication.
- Equality comparisons: fixed boxing issues in equality operators by introducing explicit `IsBoxed` property to `ExpressionResult`. Replaced brittle AST-based heuristics with explicit boxing state tracking. Fixes:
  - Method return value comparisons (e.g., `methodResult == 4` and `4 == methodResult` now both work)
  - Boolean literal over-unboxing (raw boolean values no longer incorrectly unboxed)
  - Function return comparisons with boolean literals (e.g., `isEven(4) == true` now works correctly)
- IL Generation: added proper type coercion for Object-to-Boolean comparisons when comparing function return values to boolean literals.
- Console: align `console.log` array formatting with Node. Arrays now print as `[ 1, 2, 3 ]` (brackets with comma+space separators and outer spacing) instead of `JavaScriptRuntime.Array` or `1,2,3`.

### Tests
- Array: add focused test `Array_ConsoleLog_PrintsArrayContent` (generator + execution) and commit verified snapshots to lock in Node-style output.
- Int32Array: updated 2 generator test snapshots to reflect new method signatures (get_Item and set_Item now use object parameters).

## v0.3.2 - 2025-11-26

### Added
- Functions: default parameter values for function declarations, function expressions, and arrow functions. Supports literal defaults (numbers, strings, booleans) and expression defaults that reference previous parameters (e.g., `function f(a, b = a * 2)`). Default values are applied via starg IL pattern when arguments are null.
- Classes: default parameter values for class constructors and instance methods. Call sites validate argument count against min/max bounds and pad missing optional arguments with ldnull.
- Symbol Table: `CountRequiredParameters()` helper to distinguish required parameters from optional ones with defaults.
- ClassRegistry: method tracking with `RegisterMethod()` and `TryGetMethod()` to store min/max parameter counts for instance methods.
### Changed
- IL Generation: function parameter handling now uses starg pattern for defaults instead of requiring all arguments. Parameter signatures always include all params (required + optional).
- ClassRegistry: constructor tracking extended from single parameter count to min/max range (MinParamCount, MaxParamCount) to support optional parameters.
- Call sites: both new-expressions (constructors) and call-expressions (methods) now validate argument count ranges and pad with ldnull for missing optional parameters.
### Fixed
- Functions: recursive IIFE crash when function pre-registered in registry with nil handle. Implemented three-way branch logic:
  1. Registered function with valid handle → compile-time ldftn + newobj Func
  2. Pre-registered function with nil handle → direct delegate construction (ldftn + newobj) and closure binding
  3. Not registered → ldnull (uses InvokeWithArgs for dynamic calls)
- Functions: eliminated TypeLoadException when emitting ldftn with nil method handles by adding runtime self-binding path for pre-registered functions.
- Classes: constructor calls now support fewer arguments than parameters when defaults are present (e.g., `new Person("Alice")` for constructor with 2 params).
- Classes: method calls now support fewer arguments than parameters when defaults are present (e.g., `calc.greet()` for method with 1 param).

## v0.3.1 - 2025-11-26




## v0.3.0 - 2025-11-25

### Fixed
- Classes: class methods and constructors can now access variables from all ancestor scopes (global, function, block), not just the global scope. The `DetermineParentScopesForClassMethod` now walks the scope tree to build the complete parent scope chain, enabling proper multi-level scope access for classes declared inside functions. Both `EmitMethod` and `EmitExplicitConstructor` use this mechanism to provide consistent scope access.
- Classes: class methods now correctly access parent scope variables through `this._scopes` field instead of incorrectly casting `this` to array. Modified `BinaryOperators.LoadVariable` to check class method context and emit proper IL for scope field access.
- IL Generation: conditional `_scopes` field generation for classes - only classes that reference parent scope variables now include the field and constructor parameter, avoiding unnecessary overhead for simple classes.
- Code quality: fixed indentation in `BinaryOperators.cs` to conform to C# coding guidelines (4-space indentation).
### Changed
- Architecture: refactored free variable analysis from `ClassesGenerator` into `SymbolTable` infrastructure for better separation of concerns. Analysis now happens once during symbol table construction with results cached in `Scope.ReferencesParentScopeVariables` property.
- Symbol Table: added `ReferencesParentScopeVariables` property to `Scope` class to track whether a scope references variables from parent scopes.
- Symbol Table Builder: extended with comprehensive free variable analysis:
  - `AnalyzeFreeVariables`: bottom-up recursive scope traversal
  - `ContainsFreeVariable`: static AST walker handling 20+ node types (identifiers, declarations, control flow, expressions, etc.)
  - `IsKnownGlobalIntrinsic`: centralized detection of 24 known global intrinsics (console, setTimeout, Math, etc.)
- Code cleanup: removed ~174 lines of duplicated AST walking logic from `ClassesGenerator` (`ClassAccessesParentScopeVariables`, `MethodAccessesParentScopeVariables`, `IsGlobalIntrinsic` methods now obsolete).
- Performance: scope analysis now single-pass during symbol table build instead of repeated AST walking during code generation.

## v0.2.0 - 2025-11-22

Added
- Functions: basic object parameter destructuring for function declarations, function expressions, and arrow functions. Supports shorthand properties (`{a,b}`) and simple aliasing (`{ a: x }`). Each destructured identifier is bound into the function's lexical scope prior to body execution.
- Symbol/Table & IL Generation: parameter ObjectPattern binding and emission across all function kinds (declaration/expression/arrow) retrieving properties via `JavaScriptRuntime.Object.GetProperty` and storing into scope fields.
- Docs: updated `docs/ECMA262/FeatureCoverage.json` and regenerated `docs/ECMA262/FeatureCoverage.md` to reflect support for object parameter destructuring.
Changed
- Docs: arrow function feature notes no longer list parameter destructuring as unsupported; new feature entry added under binding patterns.
- Tests: adopted received generator snapshots for destructuring tests (Function/Arrow) to align verified output with current emitter formatting and reduce non-semantic churn.
Fixed
- Parameter destructuring: previously emitted undefined values due to missing binding/shorthand handling for `{a,b}` and alias patterns; now bindings populate scope fields correctly and execution snapshots log expected values.
- Build: migrated to .NET 10 (net10.0); CI workflow updated to use 10.0.x SDK.

## v0.1.7 - 2025-11-12

Added
- Functions: internal self-binding for named function expressions to enable recursion (e.g., const f = function g(){ return g(); }). Implemented by constructing the function delegate directly (ldftn + newobj) and binding the internal name to that delegate on first entry.
- Tests: generator and execution coverage for classic IIFE and recursive IIFE; new SymbolTable tests for IIFE scopes (anonymous and named) and internal self-binding visibility.
Changed
- Hoisting: ensure local function variables are initialized before top-level statement emission so functions can reference each other by variable name prior to IIFE invocation.
- Scope naming: unified to `FunctionExpression_*` for function expressions; class .NET namespace unified under `Classes`.
Fixed
- IL generation: corrected call-site emission to preserve the callee delegate across null-initialization branches and maintain stack balance.
- Named function expression recursion: eliminated NullReferenceException by eagerly binding the internal name on method entry.
- Symbol table: removed duplicate child scope registration and added visited sets for function/arrow expressions to prevent duplicate scopes and nested type emission; TypeGenerator defensively skips duplicate nested types.

## v0.1.6 - 2025-09-23

Added
	- ceil, sqrt, abs, floor, round, trunc, sign
	- sin, cos, tan, asin, acos, atan, atan2
	- sinh, cosh, tanh, asinh, acosh, atanh
	- exp, expm1, log, log10, log1p, log2, pow
	- min, max, random, cbrt, hypot, fround, imul, clz32
	Notes: JS ToNumber coercion, correct NaN/±Infinity propagation, and signed zero (-0) preservation where applicable.
	- GetItem(object, double index): indexer for Int32Array
	- GetLength(object): length for Int32Array
	- Compiler/Runtime: dynamic object property assignment (obj.prop = value) for non-computed MemberExpressions. Emitter now lowers to JavaScriptRuntime.Object.SetProperty for dynamic objects; typed property setters/fields are used when available. Supports ExpandoObject (object literal) and reflection-backed host objects; arrays/typed arrays ignore arbitrary dot properties. New Literals tests cover generator and execution for property assignment.
	- Compiler: object literals now support Identifier, StringLiteral, and NumericLiteral property keys. Numeric keys are coerced to strings using invariant culture (JS ToPropertyKey semantics) during IL emission.
	- Runtime Object: GetItem(object, double index) supports ExpandoObject (object literal) by coercing the numeric index to a string property name and returning its value (null to model undefined when absent).
	- Runtime: Math intrinsic ([IntrinsicObject("Math")]) — implemented the full function set:
	  - ceil, sqrt, abs, floor, round, trunc, sign
	  - sin, cos, tan, asin, acos, atan, atan2
	  - sinh, cosh, tanh, asinh, acosh, atanh
	  - exp, expm1, log, log10, log1p, log2, pow
	  - min, max, random, cbrt, hypot, fround, imul, clz32
	  Notes: JS ToNumber coercion, correct NaN/±Infinity propagation, and signed zero (-0) preservation where applicable.
	- Runtime: Math value properties (constants): E, LN10, LN2, LOG10E, LOG2E, PI, SQRT1_2, SQRT2.
	- Runtime: Int32Array intrinsic (minimal typed array) with constructor from number or array-like, numeric length, index get/set, and set(source[, offset]) coercing values via ToInt32-style truncation (NaN/±∞/±0 → 0). Registered as [IntrinsicObject("Int32Array")].
	- Runtime Object: integrated Int32Array support in JavaScriptRuntime.Object helpers:
	  - GetItem(object, double index): indexer for Int32Array
	  - GetLength(object): length for Int32Array
	- Compiler: dynamic indexed element assignment (target[index] = value) for Int32Array inside class methods using JavaScriptRuntime.Object.AssignItem fallback; leaves assigned value available for expression contexts while statement contexts discard it.
	- Operators: binary "in" operator (property existence) with runtime helper Object.HasPropertyIn covering: ExpandoObject/anonymous objects, arrays (numeric index bounds check), Int32Array, strings (character index), and reflection fallback for host objects. Emits early in BinaryOperators to avoid duplicate side-effects. Limitations: no prototype chain traversal yet; non-object RHS throws TypeError only for null/undefined (remaining primitives TODO); numeric LHS coerced via ToString for object keys.
	- Operators: inequality (!=) and strict inequality (!==) in both value and branching contexts. Uses Ceq inversion for value results and bne.un for conditional branches; unboxing/coercion aligned with existing equality semantics.
	- Compiler: heuristic class method scope instantiation (ShouldCreateMethodScopeInstance) plus unconditional method scope type registration; enables correct closure binding and removes prior experimental gaps.
	- Validation: reflection-based require() module discovery via [NodeModule] attribute scanning; fail fast if an unknown module name is required.
	- Dispatch Refactor: Removed indirect dispatcher table; functions now emitted as static methods with direct ldftn/newobj delegate creation. Introduced FunctionRegistry for name→MethodDefinitionHandle lookup (improves nested function resolution) and reduced IL indirection.
	- Closures: Guarded closure binding on returns—only bind identifiers classified as functions; prevents erroneous delegate construction when returning non-function expressions.
	- Nested Functions: Predeclare synthetic *_Nested container types and register nested functions first to eliminate TypeLoadException ordering issues.
	- Class Methods & Closures: Skip closure binding for class instance methods to avoid capturing method delegates incorrectly.

Tests
	- ObjectLiteral_PropertyAssign (prints { a: 1, b: 2 })
	- ObjectLiteral_NumericKey (prints 1)
	- Math_Ceil_Sqrt_Basic
	- Math_Round_Trunc_NegativeHalves, Math_Sign_ZeroVariants, Math_Min_Max_NaN_EmptyArgs,
	  Math_Hypot_Infinity_NaN, Math_Fround_SignedZero, Math_Imul_Clz32_Basics,
	  Math_Log_Exp_Identity, Math_Cbrt_Negative
	- TypedArray: Int32Array_Construct_Length
	- TypedArray: Int32Array_FromArray_CopyAndCoerce
	- TypedArray: Int32Array_Set_FromArray_WithOffset

Docs

- Strings: String.prototype.split with string/regex separator and optional limit. Implemented in JavaScriptRuntime.String.Split and routed via JavaScriptRuntime.Object.CallMember for CLR string receivers. Returns JavaScriptRuntime.Array. Basic generator and execution tests added (String_Split_Basic).
Changed
- Runtime: qualify BCL Math usages to global::System.Math in String/Array to avoid name collision with JavaScriptRuntime.Math.
- IL generation diagnostics: centralized all NotSupportedException throwing through ILEmitHelpers (BinaryOperators, ILMethodGenerator, ILExpressionGenerator, JavaScriptFunctionGenerator) to enrich messages with source file:line:column when AST node info is available.
- Tooling/docs: compiled scripts/ECMA262/generateFeatureCoverage.js with js2il and used the generated DLL to update docs.
- Class scope architecture: all class method scope types are now generated; instantiation controlled by a lightweight heuristic to avoid unnecessary objects while enabling closures.
- IL generation: removed experimental class / method scope warnings; snapshots updated accordingly.
- Dispatch: unified dynamic instance member calls through the generic dispatcher (string/array/host) reducing special-cases; refined call-on-expression handling (e.g., obj.method() where obj is an expression).
- Console intrinsic: simplified to singleton access pattern (removed prior intrinsic special-casing) and reordered emitter paths accordingly.

Reverted
- Removed experimental BitArray intrinsic and its smoke test pending a fix for intrinsic instance-call IL emission.

Fixed
- Computed member indexing: emitter now always boxes the index and routes to Runtime Object.GetItem(object, object), fixing failures like arr[arr.length - 1] and stabilizing array/string/typed array indexing across dynamic receivers.
- Simple identifier assignment: always box the RHS for '=' so storing into object-typed fields is verifiable and correct (fixes counter mutation in Function_GlobalFunctionChangesGlobalVariableValue).
- IL generation: eliminated "Unsupported object property key type: Literal" for object literals with numeric keys by handling literal key kinds and emitting IDictionary<string, object> set_Item calls with the coerced key.
- Try/Catch: restored block (lexical) scope local allocation for const/let inside try blocks (e.g., TryCatch_NoBinding_NoThrow) by allocating locals for Block_L* scopes. Removed temporary lazy fallback.
- Member dispatch: resolved dynamic instance call regression for certain chained member expressions after dispatcher generalization.
- Class method scope: ensured variable registry/lookup consistency preventing "Scope '<method>' not found in local slots" during generation of method bodies in performance scripts.
- Integration test gating: Prime performance compilation test now optional (requires RUN_INTEGRATION=1) so it no longer blocks CI/PR when reproducing historical class scope issues.
- IL generation: corrected dynamic Int32Array indexed assignment stack handling (previously left AssignItem result plus ldnull causing potential stack imbalance); now explicitly discards the AssignItem return in statement contexts.

Tests (maintenance)
- Gated PrimePerformanceCompilationTests behind RUN_INTEGRATION.
- Removed stale PrimePerformanceCompilationTests.cs (now gated workflow covers scenario) and deleted obsolete empty FunctionGenerator.cs placeholder.
- Stabilized TryCatch_NoBinding_NoThrow after block scope fix (both generator & execution variants green).

## v0.1.5 - 2025-09-08

Added
- Compiler: object destructuring (binding patterns) for object patterns in variable declarations (basic). Single-evaluation of initializer with per-property extraction; when the receiver is a known CLR-backed type, use typed getters; otherwise fall back to runtime Object.GetProperty.
- Runtime: minimal JavaScript Date intrinsic with constructor overloads and core APIs:
	- Constructors: new Date(), new Date(milliseconds)
	- Static: Date.now(), Date.parse(string)
	- Prototype: getTime(), toISOString()

Changed
- Type propagation: propagate CLR runtime types through object destructuring targets (e.g., const { performance } = require('perf_hooks')) to enable direct callvirt to property getters and instance methods.
- IL generation: lower object patterns in variable declarations via a synthesized temp and scope field writes; prefer typed property access when possible with reflection fallback.
- IL generation: special-case emission for new Date(...); improved host intrinsic static call handling (argument coercion and boxing aligned to CLR signatures). Date.now/Date.parse return boxed numbers (object) to match JS semantics.

Fixed
- Node perf_hooks: stabilized PerfHooks_PerformanceNow_Basic (generator + execution). Direct typed calls to PerfHooks.get_performance() and Performance.now(); elapsed time check matches runtime semantics. Updated generator snapshot to align with current IL.
- Metadata: deduplicate JavaScriptRuntime AssemblyReference entries by caching a single AssemblyReferenceHandle per emitted assembly (per MetadataBuilder). This eliminates multiple runtime AssemblyRef rows in generated DLLs and reduces metadata bloat. Verified on a compiled sample (scripts/ECMA262/generateFeatureCoverage.js) via a small Reflection.Metadata checker.
- Generator: avoid AccessViolation at CastHelpers.StelemRef by fixing once-only boxing before Stelem_ref when constructing object[] for call sites.
- Tests: resolved Verify newline mismatch in Date execution snapshots.

Docs
- Updated NodeSupport to note that destructuring perf_hooks (const { performance } = require('perf_hooks')) is supported and enables typed calls.
- Updated ECMAScript 2025 feature coverage to include partial support for object destructuring in variable declarations (with scope and limitations called out); regenerated markdown.
- Updated ECMAScript 2025 feature coverage to include Array.prototype.slice, Array.prototype.splice, Array.prototype.push, Array.prototype.pop, and Array.isArray with exact ECMA-262 spec anchors and linked test references; regenerated docs/ECMA262/FeatureCoverage.md from JSON.
- Updated ECMAScript 2025 feature coverage to include Date constructor, Date.now, Date.parse, Date.prototype.getTime, and Date.prototype.toISOString; regenerated docs/ECMA262/FeatureCoverage.md.

Tooling
- Compiled scripts/ECMA262/generateFeatureCoverage.js with js2il; emitted generateFeatureCoverage.dll and runtimeconfig.json next to the script for faster local runs.

Tests
- Added/updated Node generator and execution tests around perf_hooks performance.now; snapshots aligned with typed-call IL.
- Added Date execution tests (construct from ms → getTime/toISOString; parse ISO string). Removed the obsolete Date generator test and neutralized its snapshot.

## v0.1.4 - 2025-09-06

Added
- Node interop: perf_hooks module with Performance.now() based on Stopwatch (monotonic). now() returns a boxed double (object) to match JS number semantics.
- IL generation: general typed member access for known receivers. When a receiver’s CLR type is known (e.g., from require()), emit direct callvirt to property getters and instance methods instead of reflection.

Changed
- Type propagation: carry CLR runtime types from require() bindings into identifiers, enabling direct calls like perf.performance and performance.now without dynamic dispatch.
- Member access emitter: return both JavaScript type and CLR type (ExpressionResult) so downstream code can choose typed vs dynamic emission.
- Node Process: removed special-cased getters for argv/exitCode in the emitter; the generic typed getter path is used instead.
- Runtime metadata: nested-type aware type references and method reference caching keyed by FullName to support nested types like JavaScriptRuntime.Node.PerfHooks+Performance.
- Runtime Object: GetProperty now exposes host public instance properties/fields via reflection as a fallback for non-dynamic objects.

Fixed
- Subtract on Performance.now values by returning an object (boxed double), avoiding null/unboxing issues in Operators.Subtract.
- Generator snapshot drift for new perf_hooks tests; aligned verified output to direct typed calls.

Tests
- Added Node generator and execution tests: PerfHooks_PerformanceNow_Basic (validates direct callvirt to get_performance/now and non-negative elapsed time).
- Node suite remains green after moving Process property access to the generic typed path.

## v0.1.3 - 2025-09-03

Added
- Control flow: truthiness in conditionals (if/while/ternary) using JavaScript ToBoolean semantics; execution and generator tests.
- Operators: logical OR (||) and logical AND (&&) with correct short-circuit semantics in both value and branching contexts; execution and generator tests.
- Runtime: generic JavaScriptRuntime.Object.CallMember(receiver, methodName, object[]? args) to dispatch member calls at runtime (routes System.String and runtime Array; reflection fallback for others).
- Arrays: Array.prototype.join(sep?) implemented with JS semantics; execution and generator tests.
- String/Array nested slow-path tests that exercise dynamic member dispatch in nested functions.

Changed
- Codegen: remove hardcoded member-name checks in ILExpressionGenerator (e.g., replace/map/join/startsWith) and route dynamic/unknown receivers through Object.CallMember.
- Generator: minor IL snapshot churn due to scopes array construction and dispatcher usage.

Fixed
- Nested function scopes: always pass [global, local] when invoking a callee stored on the current local scope to prevent IndexOutOfRangeException in inner functions.

Docs
- Updated ECMAScript 2025 feature coverage to include conditional truthiness coercion and logical operators (||, &&); regenerated markdown.
- Updated coverage to include Array.join and notes on dynamic member dispatch.

Tests
- Added ControlFlow tests for truthiness in if conditions and BinaryOperator tests for logical OR/AND value results; added generator tests for logical operators.
- Added String_StartsWith_NestedParam (execution + generator) to validate slow-path member dispatch in nested function.
- Added Array_Map_NestedParam (execution + generator) to validate dynamic dispatch on Array and callback delegate wiring.
- Aligned Node generator snapshot for nested Require('path').join to current emitter.

## v0.1.2 - 2025-09-03

Added
- Arrays: Array.prototype.map (basic value-callback) returning a new array; execution and generator tests.
- Arrays: Array.prototype.sort default comparator (lexicographic) returning the array; execution and generator tests.
- Emitter: FunctionExpression support to enable function-literal callbacks (e.g., in map).
- Literals: array spread copy [...arr] with support for SpreadElement in array literals; backed by JavaScriptRuntime.Array.PushRange; execution and generator tests.
- Strings: String.localeCompare returning a number; execution and generator tests.
- Strings: String.startsWith(searchString[, position]); execution and generator tests.
- Control flow: for-of over arrays and strings using JavaScriptRuntime.Object.GetLength and GetItem; execution and generator tests.
- Operators: compound assignment "+=" for identifiers via runtime Operators.Add (full JS coercion); execution and generator tests.
- Node interop: process.argv exposure and enumeration in execution environment; added Environment_EnumerateProcessArgV test.
- Integration: long-running compile test that parses and emits scripts/ECMA262/generateFeatureCoverage.js (skipped in CI by default).

Changed
- Codegen: centralized call emission in ILExpressionGenerator; it now dispatches member/host/intrinsic calls.
- Types: ExpressionResult now carries both JsType and ClrType; removed bespoke require() clrtype tagging.
- Property access: .length emission goes through JavaScriptRuntime.Object.GetLength(object) for arrays/strings/collections.
- Tests: stabilized generator snapshots after GetLength change across Array/Function/Literals suites.
- Strings: routed instance method calls through a single reflection-based path (EmitStringInstanceMethodCall) and added an IsDefinitelyString analyzer to prefer CLR string receivers when safe. TemplateLiteral emission now produces a CLR System.String directly.
- Calls: corrected routing so general call expression generation recognizes CLR string receivers for method dispatch.
- Control flow: for-of codegen uses object locals to maintain loop state (iterable, length, index) for clearer IL and reliable continue/break behavior.

Fixed
- Resolved generator snapshot mismatches (including BOM/encoding) by updating verified files from received outputs.
- Snapshot stability for new for-of and string tests; aligned exact-match expectations to emitted IL.

Docs
- Updated ECMAScript 2025 feature coverage: added Array.prototype.map, clarified Array.length emission; regenerated markdown.
- Updated ECMAScript 2025 feature coverage with array spread in literals, String.startsWith, String.localeCompare, compound "+=" for strings, and for-of over arrays/strings; regenerated markdown from JSON.
	Also documented Node process.argv support.
 

Tests
- New Array subgroup with execution and generator tests: length, empty length, sort basic, map basic. Targeted isEven test remains green.
- Added execution and generator tests for: array spread literal copy, String.startsWith, String.localeCompare, string "+=" append, and ControlFlow for-of over arrays/strings. Updated verified snapshots accordingly.
- Added Node execution test for process argv enumeration (Environment_EnumerateProcessArgV).
 - Added Integration test (Compile_Scripts_GenerateFeatureCoverage) that compiles scripts/ECMA262/generateFeatureCoverage.js; marked [Skip] to avoid CI runtime cost.

## v0.1.1 - 2025-08-29

Added
- Strings: String.replace(...) with RegExp literal support (flags), via JavaScriptRuntime.String.Replace and codegen pattern detection.
- Control flow: conditional operator (?:) emission and execution.
- Strings: template literals with correct concatenation semantics via runtime Operators.Add.
- Node interop: __dirname intrinsic global. Added JavaScriptRuntime.Node.GlobalVariables and wired execution harness to set module context.

Changed
- Expression statements: directly emit expressions in statement context and explicitly discard results for call/new/ternary to keep stack balanced.
- Generator output: stabilized snapshots with narrow, per-test scrubbers (path and trailing whitespace), avoiding global formatting changes.

Fixed
- Resolved generator snapshot whitespace/line-ending diffs; trimmed trailing spaces and ensured consistent EOF newline.

Docs
- Updated ECMAScript 2025 feature coverage to include String.replace, template literals, and the conditional operator.

Tests
- Added execution and generator tests for template literals, conditional operator, and Node __dirname. Normalized snapshots for reliability.

## v0.1.0 - 2025-08-28

Added
- Exceptions: throw; try/catch (no binding) and try/finally regions; only catch JavaScriptRuntime.Error.
- Const semantics: reassigning a const (including ++/--) emits a TypeError at runtime.
- Node interop: require(string) with attribute-based discovery of Node core modules (path, fs scaffolding).
- Host intrinsics: [IntrinsicObject("console")] with reflection-based static call (console.log).
- Variable metadata: RuntimeIntrinsicType to bind CLR-backed methods directly.
- Tooling: Node scripts for updateVerifiedFiles and snapshot cleanup; removed legacy PowerShell scripts.

Changed
- Scope lifecycle: GenerateStatementsForBody(scopeName, createScopeInstance) controls block-scope creation; Block_L{line}C{col} naming; PopLexicalScope clears local to null.
- Runtime refs: centralized Error ctor/type refs; helpers for Operators, Array, Object, Console, Closure.
- IL generator cleanup and helper extraction for call/host/intrinsic paths.

Fixed
- Duplicate block scope type emission; improved equality branching with unboxing rules.

Notes
- See docs/ECMA262/FeatureCoverage.md for updated coverage.

## v0.1.0-preview.7 - 2025-08-14

Added
- Control flow: while, do-while, continue, and break across for/while/do-while; execution and generator tests; normalized snapshot formatting.
- Booleans: literals (true/false), logical not (!), boolean equality; improved conditional branching for nested expressions.
- Operators: dynamic plus via runtime Operators.Add (JS semantics); subtract via Operators.Subtract with ToNumber coercion; fixed unsigned right shift (>>>).

Changed
- Emitter: streamlined value emission and boolean branching; unified LoadValue path; selective unboxing for numbers/booleans in equality.
- For-loops: expression initializers; wired continue labels across loops; fixed equality branching with arithmetic results.

Docs
- Updated and regenerated ECMAScript 2025 feature coverage; aligned test references.

Tests
- Added/updated control-flow tests and JS fixtures; aligned generator snapshots to emitted IL; general test cleanups.

## v0.1.0-preview.6 - 2025-08-05

Added
- Classes: constructors, instance fields/methods, static methods; static class fields with .cctor; this.prop reads/writes; explicit constructors.
- Private fields: name-mangling for #private and end-to-end access; tests and updated generator snapshots.

Changed
- Emitter refactors: helpers for MemberExpression, NewExpression, AssignmentExpression; centralized boxing via TypeCoercion.boxResult; removed redundant site-level boxing.
- Operators: dynamic "+" and "-" routed through runtime with proper coercion/boxing; fixed >>> IL conversion.

CLI
- Added --version and improved help; added CLI tests.

Docs
- Updated and regenerated feature coverage for classes and operators.

## v0.1.0-preview.5 - 2025-07-28

CLI
- Improved UX: short flags, --version output, colored messages, output directory handling.

CI
- Enabled manual release trigger; run tests in Release configuration.

## v0.1.0-preview.4 - 2025-07-22

CI/NuGet
- Publish hardening: set NUGET_API_KEY via env, proper argument quoting, correct push options ordering.

## v0.1.0-preview.3 - 2025-07-18

- Initial preview release.

