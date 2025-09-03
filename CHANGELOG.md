# Changelog

All notable changes to this project are documented here.

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

