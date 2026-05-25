<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.11: Equality Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T13:09:52Z

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
| Abstract equality (==) and strict equality (===) evaluation | Supported | `tests/Js2IL.Test262.Tests/language/expressions/equals/ExecutionTests.cs`<br>`tests/Js2IL.Test262.Tests/language/expressions/equals/PortExpressionsBatchExecutionTests.cs`<br>`tests/Js2IL.Test262.Tests/language/expressions/strict-equals/PortExpressionsBatchExecutionTests.cs` |  | The checked-in equality ports now cover BigInt/object/string cases, SameValue-style strict comparisons, null/undefined handling, evaluation-order errors, and representative abstract-equality coercion cases from test262. |

