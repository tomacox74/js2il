# Changelog

All notable changes to this project are documented here. Entries summarize changes between released tags.

## v0.1.0-preview.7

Added
- Control flow: while, do-while, continue, and break support across for/while/do-while loops; execution and generator tests with snapshots; normalized snapshot formatting.
- Boolean support: literals (true/false), logical not (!), and boolean equality; improved conditional branching with nested expressions.
- Numeric ops: dynamic plus routed through runtime Operators.Add (JS semantics); subtract routed through Operators.Subtract with JS ToNumber coercion; fixed unsigned right shift (>>>).

Changed
- Generator/IL: streamlined value emission and boolean branching; unified LoadValue path for nested BinaryExpression; selective unboxing for numbers/booleans in equality.
- For-loops: support expression initializers; wired continue labels across for/while/do-while; fixed equality branching for arithmetic results.

Docs
- Updated and regenerated ECMAScript2025 feature coverage; aligned test references.

Tests
- Added/updated control flow tests and JS fixtures; aligned generator snapshots to emitted IL; general test cleanups and normalizations.

## v0.1.0-preview.6

Added
- Classes: instance constructors, fields, and methods; static methods; support for static class fields (emit static field and .cctor); support for this.prop reads/writes; explicit class constructors.
- Private fields: name-mangling for #private and end-to-end access support; tests and updated generator snapshots.

Changed
- Emitter refactors: extracted helpers for MemberExpression, NewExpression, and AssignmentExpression; centralized boxing via TypeCoercion.boxResult; removed redundant site-level boxing.
- Operators: dynamic "+" and "-" routed through runtime with proper coercion/boxing; fixed >>> IL conversion.

CLI
- Added --version and improved help and CLI tests.

Docs
- Updated and regenerated feature coverage for classes and operators.

## v0.1.0-preview.5

CLI
- Improved CLI UX: short flags, --version output, colored messages, and output directory handling.

CI
- Enabled manual release trigger; tests run using Release configuration.

## v0.1.0-preview.4

CI/NuGet
- Publish pipeline hardening: set NUGET_API_KEY via env, proper argument quoting, and correct push options ordering.

## v0.1.0-preview.3

- Initial preview release.
