<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.14: Conditional Operator ( ? : )

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-09-09T22:40:32Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.14 | Conditional Operator ( ? : ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-conditional-operator) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.14.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-conditional-operator-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.14 ([tc39.es](https://tc39.es/ecma262/#sec-conditional-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Conditional-expression `in` grammar early errors | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/ExpressionSyntaxConformance7BatchParseTests.cs` | `test/language/expressions/conditional/in-branch-2.js`<br>`test/language/expressions/conditional/in-condition.js` | The native Test262 harness rejects two pinned parse-negative conditional-expression forms that use `in` where the grammar disallows it. |

### 13.14.1 ([tc39.es](https://tc39.es/ecma262/#sec-conditional-operator-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Conditional operator (? :) evaluation | Supported | `tests/Jroc.Test262.Tests/language/expressions/conditional/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/conditional/PortExpressionsBatchExecutionTests.cs` |  | The checked-in conditional-expression slice covers GetValue on the test expression, truthy/falsy branch selection, branch-local abrupt completions, coalesce/conditional interaction, and the tail-position return cases exercised by the imported test262 ports. |

