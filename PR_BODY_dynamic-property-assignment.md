Title: Support dynamic object property assignment (obj.prop = value)

Summary
- Adds support for simple assignment to non-computed MemberExpression targets (obj.prop = value).
- Lowers to JavaScriptRuntime.Object.SetProperty for dynamic receivers; uses typed setters/fields when available.
- Adds generator and execution tests under Literals to validate property assignment on object literals.
- Updates feature coverage JSON and changelog.

Changes
- Compiler/Emitter
  - ILExpressionGenerator: handle assignment to non-computed MemberExpression by emitting a call to Object.SetProperty(receiver, name, value). Drops unnecessary temp locals and preserves stack balance in statement context.
- Runtime
  - JavaScriptRuntime.Object.SetProperty: supports ExpandoObject (object literals) and reflection-backed host objects; arrays/typed arrays ignore arbitrary dot properties and return the value.
- Tests
  - Literals: ObjectLiteral_PropertyAssign (generator + execution) validating that { a: 1 } becomes { a: 1, b: 2 }.
- Docs
  - ECMAScript2025_FeatureCoverage.json: mark property assignment as Supported under Assignment Operators.
  - CHANGELOG.md: document the new support.

Notes
- Computed assignment (obj[key] = value) is already handled via AssignItem; this change covers the dot-property case.
- Compound property assignment operators (e.g., obj.prop += x, |=) are not included in this PR.

Checklist
- [x] Tests added/updated and passing
- [x] Feature coverage updated
- [x] Changelog updated
- [x] No public API breaking changes
