# Changelog

All notable changes to this project are documented here.

## Unreleased

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
  2. Pre-registered function with nil handle → runtime GetCurrentMethod() + CreateSelfDelegate()
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
- Docs: updated `ECMAScript2025_FeatureCoverage.json` and regenerated `ECMAScript2025_FeatureCoverage.md` to reflect support for object parameter destructuring.
Changed
- Docs: arrow function feature notes no longer list parameter destructuring as unsupported; new feature entry added under binding patterns.
- Tests: adopted received generator snapshots for destructuring tests (Function/Arrow) to align verified output with current emitter formatting and reduce non-semantic churn.
Fixed
- Parameter destructuring: previously emitted undefined values due to missing binding/shorthand handling for `{a,b}` and alias patterns; now bindings populate scope fields correctly and execution snapshots log expected values.
- Build: upgrade branch targets .NET 10 (net10.0) for early compatibility testing; CI workflow updated to use 10.0.x SDK. Master remains on net8.0 until upgrade validated.

## v0.1.7 - 2025-11-12

Added
- Functions: internal self-binding for named function expressions to enable recursion (e.g., const f = function g(){ return g(); }). Implemented via a small prologue that binds the internal name on first entry using a new runtime helper `JavaScriptRuntime.Closure.CreateSelfDelegate`.
- Runtime: `Closure.CreateSelfDelegate(MethodBase, int paramCount)` to construct the correct `Func<object[], ... , object>` delegate shape for self-calls across arities.
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
- Tooling/docs: compiled scripts/generateFeatureCoverage.js with js2il and used the generated DLL to update docs.
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
- Metadata: deduplicate JavaScriptRuntime AssemblyReference entries by caching a single AssemblyReferenceHandle per emitted assembly (per MetadataBuilder). This eliminates multiple runtime AssemblyRef rows in generated DLLs and reduces metadata bloat. Verified on a compiled sample (scripts/generateFeatureCoverage.js) via a small Reflection.Metadata checker.
- Generator: avoid AccessViolation at CastHelpers.StelemRef by fixing once-only boxing before Stelem_ref when constructing object[] for call sites.
- Tests: resolved Verify newline mismatch in Date execution snapshots.

Docs
- Updated NodeSupport to note that destructuring perf_hooks (const { performance } = require('perf_hooks')) is supported and enables typed calls.
- Updated ECMAScript 2025 feature coverage to include partial support for object destructuring in variable declarations (with scope and limitations called out); regenerated markdown.
- Updated ECMAScript 2025 feature coverage to include Array.prototype.slice, Array.prototype.splice, Array.prototype.push, Array.prototype.pop, and Array.isArray with exact ECMA-262 spec anchors and linked test references; regenerated docs/ECMAScript2025_FeatureCoverage.md from JSON.
- Updated ECMAScript 2025 feature coverage to include Date constructor, Date.now, Date.parse, Date.prototype.getTime, and Date.prototype.toISOString; regenerated docs/ECMAScript2025_FeatureCoverage.md.

Tooling
- Compiled scripts/generateFeatureCoverage.js with js2il; emitted generateFeatureCoverage.dll and runtimeconfig.json next to the script for faster local runs.

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
- Integration: long-running compile test that parses and emits scripts/generateFeatureCoverage.js (skipped in CI by default).

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
 - Added Integration test (Compile_Scripts_GenerateFeatureCoverage) that compiles scripts/generateFeatureCoverage.js; marked [Skip] to avoid CI runtime cost.

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
- See docs/ECMAScript2025_FeatureCoverage.md for updated coverage.

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

