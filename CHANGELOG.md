# Changelog

All notable changes to this project are documented here.

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

