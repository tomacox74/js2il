<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.13: Binary Logical Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T13:09:52Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.13 | Binary Logical Operators | Supported | [tc39.es](https://tc39.es/ecma262/#sec-binary-logical-operators) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.13.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-binary-logical-operators-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.13 ([tc39.es](https://tc39.es/ecma262/#sec-binary-logical-operators))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Logical AND (&&) and logical OR (\|\|) evaluation order and short-circuiting | Supported | `tests/Jroc.Test262.Tests/language/expressions/logical-and/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/logical-and/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/logical-or/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/logical-or/PortExpressionsBatchExecutionTests.cs` |  | The current logical-operator slice covers short-circuit behavior, left-to-right abrupt-completion ordering, symbol operands, and the value-preserving truthiness/falsiness cases exercised by the imported test262 ports. |
| Nullish coalescing operator (??) | Supported | [`BinaryOperator_NullishCoalescing_Basic.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_NullishCoalescing_Basic.js)<br>`tests/Jroc.Test262.Tests/language/expressions/coalesce/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/coalesce/PortExpressionsBatchExecutionTests.cs` |  | Implements short-circuit semantics: the right-hand side is evaluated only when the left-hand side is nullish (undefined or null). The checked-in test262 coverage now includes abrupt completions, non-nullish short-circuit values, and additional chain/evaluation cases. |

