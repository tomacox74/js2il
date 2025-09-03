Title: Emit member calls on expression receivers; add Array.join; align snapshots

Summary
- Fix IL emission for CallExpressions where the callee is a MemberExpression and the receiver is not an Identifier (e.g., String(expr).replace(...)).
- Route string instance methods replace/startsWith/localeCompare via a centralized emitter even when the receiver is an expression.
- Add array instance dispatch (map/join/sort) for expression receivers and implement JavaScriptRuntime.Array.join.
- Align generator snapshots for String_Replace_CallOnExpression and logical operator value tests.

Details
- ILExpressionGenerator:
  - Resolve member names from identifier or computed string-literals.
  - Handle non-identifier receivers: string and array instance methods now supported; improved error messages.
  - Use Runtime.GetRuntimeTypeHandle for castclass token when emitting array instance calls.
- JavaScriptRuntime.Array:
  - Implement join() and join(object[]? args) using JS-like ToString semantics.
- Tests:
  - String.String_Replace_CallOnExpression execution and generator tests now pass consistently.
  - BinaryOperator LogicalAnd/LogicalOr value generator snapshots updated to current IL printer style.
  - Node Path.join execution test remains green.

Validation
- Suites run locally:
  - String: all tests passed.
  - BinaryOperator logical execution: green.
  - Node Require_Path_Join_Basic: green.

Notes
- Integration compile test remains unchanged (long-running, opt-in).
- Follow-ups: broaden member-call coverage to additional intrinsics if encountered in integration.
