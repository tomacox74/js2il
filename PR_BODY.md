Title: Feature: Truthiness in conditionals, logical OR/AND with short-circuit, and env-gated integration compile test

Summary
- Adds JS truthiness coercion for conditionals and implements logical operators (||, &&) with full short-circuit semantics. Includes a long-running integration test gated by RUN_INTEGRATION=1 that compiles scripts/generateFeatureCoverage.js. Updates coverage docs and changelog.

Changes
- Added
	- Control flow: truthiness in if/ternary conditions via JavaScriptRuntime.TypeUtilities.ToBoolean; generator and execution tests (e.g., ControlFlow_If_Truthiness).
	- Operators: logical OR (||) and logical AND (&&) supporting both value and branching contexts with short-circuit; tests under BinaryOperator Execution/Generator.
	- Integration: Js2IL.Tests.Integration.CompilationTests.Compile_Scripts_GenerateFeatureCoverage (gated by RUN_INTEGRATION env var) to compile scripts/generateFeatureCoverage.js.
- Changed
	- IL generation: conditional branching now coerces non-boolean test expressions using ToBoolean; centralized boxing rules in ILExpressionGenerator.
	- Binary operator emitter updated to correctly handle boxing in short-circuit paths (no double-boxing), fixing incorrect outputs.
- Docs/Changelog
	- Updated ECMAScript2025_FeatureCoverage.json (+ regenerated .md) to include truthiness and logical ||/&&.
	- CHANGELOG: documented truthiness, logical operators, and the disabled integration test.

Notes
- The integration test is intentionally gated by an environment variable to keep CI time reasonable; it validates end-to-end parsing and IL generation for the docs generator script.
- Snapshot updates for new generator tests may be handled in a follow-up if necessary.

Checklist
- [x] Tests added/updated
- [x] Docs updated (coverage JSON/MD)
- [x] Integration test added and skipped
- [x] No breaking public API changes noted

