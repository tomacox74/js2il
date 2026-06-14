<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.8: Additive Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T13:09:52Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.8 | Additive Operators | Supported | [tc39.es](https://tc39.es/ecma262/#sec-additive-operators) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.8.1 | The Addition Operator ( + ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-addition-operator-plus) |
| 13.8.1.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-addition-operator-plus-runtime-semantics-evaluation) |
| 13.8.2 | The Subtraction Operator ( - ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-subtraction-operator-minus) |
| 13.8.2.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-subtraction-operator-minus-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.8.1 ([tc39.es](https://tc39.es/ecma262/#sec-addition-operator-plus))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Addition operator (+) evaluation order and coercion | Supported | `tests/Jroc.Test262.Tests/language/expressions/addition/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/addition/PortExpressionsBatchExecutionTests.cs` |  | The checked-in addition ports now cover mixed BigInt/string cases plus GetValue, abrupt-completion ordering, numeric addition, and string-concatenation coercion paths from test262. |

