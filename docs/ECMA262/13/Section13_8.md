<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.8: Additive Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-14T20:01:50Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.8 | Additive Operators | Supported | [tc39.es](https://tc39.es/ecma262/#sec-additive-operators) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.8.1 | The Addition Operator ( + ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-addition-operator-plus) |
| 13.8.1.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-addition-operator-plus-runtime-semantics-evaluation) |
| 13.8.2 | The Subtraction Operator ( - ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-subtraction-operator-minus) |
| 13.8.2.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-subtraction-operator-minus-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.8.1 ([tc39.es](https://tc39.es/ecma262/#sec-addition-operator-plus))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Addition operator (+) evaluation order and coercion | Supported | `tests/Jroc.Test262.Tests/language/expressions/addition/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/addition/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/addition/OperatorExpressionsConformanceBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/addition/AdditionCompletionExecutionTests.cs` |  | Native Test262 coverage verifies 47 addition cases, including mixed BigInt/string operands, GetValue, abrupt-completion ordering, numeric addition, and string concatenation. Symbol.toPrimitive must be callable when present and must return a primitive; violations throw TypeError without falling back to valueOf/toString. One eval-dependent case remains explicitly unsupported. |

### 13.8.2 ([tc39.es](https://tc39.es/ecma262/#sec-subtraction-operator-minus))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Subtraction operator (-) evaluation and coercion | Supported | `tests/Jroc.Test262.Tests/language/expressions/subtraction/ExpressionSyntaxConformanceBatchExecutionTests.cs` |  | The native Test262 harness additionally verifies 33 current all-variant subtraction fixtures, including Number and BigInt operands, primitive conversion, abrupt completion, and evaluation order. |

