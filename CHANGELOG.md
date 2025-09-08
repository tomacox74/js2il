# Changelog

All notable changes to this project are documented here.

## Unreleased

Added
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
- Operators: binary "in" operator (property existence) with runtime helper Object.HasPropertyIn covering: ExpandoObject/anonymous objects, arrays (numeric index bounds check), Int32Array, strings (character index), and reflection fallback for host objects. Emits early in BinaryOperators to avoid duplicate side-effects. Limitations: no prototype chain traversal yet; non-object RHS throws TypeError only for null/undefined (remaining primitives TODO); numeric LHS coerced via ToString for object keys.

Tests
- New Math execution test validating Math.sqrt and Math.ceil:
	- Math_Ceil_Sqrt_Basic
- New Math execution tests covering additional methods and semantics:
	- Math_Round_Trunc_NegativeHalves, Math_Sign_ZeroVariants, Math_Min_Max_NaN_EmptyArgs,
	  Math_Hypot_Infinity_NaN, Math_Fround_SignedZero, Math_Imul_Clz32_Basics,
	  Math_Log_Exp_Identity, Math_Cbrt_Negative
- New TypedArray execution tests validating Int32Array basics and semantics:
	- TypedArray: Int32Array_Construct_Length
	- TypedArray: Int32Array_FromArray_CopyAndCoerce
	- TypedArray: Int32Array_Set_FromArray_WithOffset

Docs
- ECMAScript 2025 Feature Coverage: updated "The Math Object" to document the full set of Math function properties listed above. Regenerated docs/ECMAScript2025_FeatureCoverage.md from JSON.
- ECMAScript 2025 Feature Coverage: marked Math value properties (E, LN10, LN2, LOG10E, LOG2E, PI, SQRT1_2, SQRT2) as Supported. Regenerated docs/ECMAScript2025_FeatureCoverage.md from JSON.
- ECMAScript 2025 Feature Coverage: added a new "TypedArray Objects" section and documented Int32Array (constructor/length/indexing/set). Regenerated docs/ECMAScript2025_FeatureCoverage.md from JSON.
- ECMAScript 2025 Feature Coverage: marked binary "in" operator as Supported (own property / array index / string index / typed array index only; prototype chain and full RHS TypeError semantics pending) and regenerated markdown.

Changed
- Runtime: qualify BCL Math usages to global::System.Math in String/Array to avoid name collision with JavaScriptRuntime.Math.
- Tooling/docs: compiled scripts/generateFeatureCoverage.js with js2il and used the generated DLL to update docs.

Reverted
- Removed experimental BitArray intrinsic and its smoke test pending a fix for intrinsic instance-call IL emission.

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

