<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.11: Equality Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-09-08T01:52:08Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.11 | Equality Operators | Supported | [tc39.es](https://tc39.es/ecma262/#sec-equality-operators) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.11.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-equality-operators-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.11.1 ([tc39.es](https://tc39.es/ecma262/#sec-equality-operators-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Abstract equality (==) and strict equality (===) evaluation | Supported | `tests/Jroc.Test262.Tests/language/expressions/equals/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/does-not-equals/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/equals/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/strict-equals/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/equals/OperatorExpressionsConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/does-not-equals/OperatorExpressionsConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/strict-does-not-equals/OperatorExpressionsConformanceBatchExecutionTests.cs` |  | The checked-in equality ports cover BigInt/object/string cases, SameValue-style strict comparisons, null/undefined handling, evaluation-order errors, and object-to-primitive abstract equality in both == and != forms. The native Test262 harness additionally verifies 59 current all-variant equality fixtures. |

