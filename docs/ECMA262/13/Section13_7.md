<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.7: Multiplicative Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-09-08T01:52:08Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.7 | Multiplicative Operators | Supported | [tc39.es](https://tc39.es/ecma262/#sec-multiplicative-operators) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.7.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-multiplicative-operators-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.7.1 ([tc39.es](https://tc39.es/ecma262/#sec-multiplicative-operators-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Multiplicative operator evaluation and coercion | Supported | `tests/Jroc.Test262.Tests/language/expressions/division/OperatorExpressionsConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/modulus/OperatorExpressionsConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/multiplication/OperatorExpressionsConformanceBatchExecutionTests.cs` | suite `language.expressions.division`<br>suite `language.expressions.modulus`<br>suite `language.expressions.multiplication` | The native Test262 harness verifies 109 current all-variant multiplicative-operator fixtures: 39 division, 35 remainder, and 35 multiplication cases. Coverage includes Number and BigInt operands, primitive conversion, abrupt completion, and evaluation order. |

