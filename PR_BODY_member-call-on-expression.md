Title: Generic runtime member dispatch + nested scopes fix; remove hardcoded member names; update snapshots

Summary
- Add a generic dispatcher JavaScriptRuntime.Object.CallMember(receiver, methodName, object[]? args) and remove hardcoded member-name checks from IL generation.
- Route slow-path member calls at runtime based on actual receiver type (System.String vs JavaScriptRuntime.Array), falling back to reflection for others.
- Fix nested function scopes array construction (always pass [global, local] when invoking a callee stored on the current local scope) to prevent IndexOutOfRange in inner functions.
- Add a nested String.startsWith test that exercises the slow path and update Node Path.join nested generator snapshot accordingly.

Details
- ILExpressionGenerator:
  - For MemberExpression calls with non-identifier receivers and for dynamic fallbacks, emit a call to Object.CallMember instead of hardcoding names (replace/map/join/startsWith/etc.).
  - Adjust nested callsite scopes array: include global scope at slot 0 and the caller’s local scope at slot 1 to match inner function expectations.
- JavaScriptRuntime.Object:
  - Implement CallMember routing: strings → JavaScriptRuntime.String helpers; runtime arrays → instance methods (params object[] handled); otherwise → existing reflective fallback.
- Tests/Snapshots:
  - Add String_StartsWith_NestedParam (execution + generator). Execution now returns true; generator shows Object.CallMember usage and correct scopes wiring.
  - Align Node/GeneratorTests.Require_Path_Join_NestedFunction.verified.txt to current emitter behavior.
  - Minor snapshot churn in a nested function generator due to scopes fix.

Validation
- Full test suite: 283 total, 0 failed, 9 skipped on local run.
- Node and String suites pass; new nested string test validated both runtime and IL.

Notes
- Follow-ups: expand dispatcher coverage if new intrinsics appear (Number, RegExp, Date, etc.); consider propagating intrinsic module types deeper to enable more static binding in nested contexts.
