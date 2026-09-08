<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.12: Binary Bitwise Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-09-08T01:52:08Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.12 | Binary Bitwise Operators | Supported | [tc39.es](https://tc39.es/ecma262/#sec-binary-bitwise-operators) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.12.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-binary-bitwise-operators-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.12.1 ([tc39.es](https://tc39.es/ecma262/#sec-binary-bitwise-operators-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number and BigInt binary bitwise operators | Supported | `tests/Jroc.Test262.Tests/language/expressions/bitwise-and/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/bitwise-or/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/bitwise-xor/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/bitwise-and/OperatorExpressionsConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/bitwise-or/OperatorExpressionsConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/bitwise-xor/OperatorExpressionsConformanceBatchExecutionTests.cs` |  | Bitwise AND, OR, and XOR use ToNumeric and preserve BigInt operands after object-to-primitive coercion, including wrapped BigInt values and custom conversion hooks. The native Test262 harness additionally verifies 63 current all-variant bitwise fixtures. |

