<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.14: Conditional Operator ( ? : )

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-26T19:57:35Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.14 | Conditional Operator ( ? : ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-conditional-operator) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.14.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-conditional-operator-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.14.1 ([tc39.es](https://tc39.es/ecma262/#sec-conditional-operator-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Conditional operator (? :) evaluation | Supported | `tests/Js2IL.Test262.Tests/language/expressions/conditional/ExecutionTests.cs`<br>`tests/Js2IL.Test262.Tests/language/expressions/conditional/PortExpressionsBatchExecutionTests.cs` |  | The checked-in conditional-expression slice covers GetValue on the test expression, truthy/falsy branch selection, branch-local abrupt completions, coalesce/conditional interaction, and the tail-position return cases exercised by the imported test262 ports. |

