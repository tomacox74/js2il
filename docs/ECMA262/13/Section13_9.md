<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.9: Bitwise Shift Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-06-24T17:00:14Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.9 | Bitwise Shift Operators | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bitwise-shift-operators) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.9.1 | The Left Shift Operator ( << ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-left-shift-operator) |
| 13.9.1.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-left-shift-operator-runtime-semantics-evaluation) |
| 13.9.2 | The Signed Right Shift Operator ( >> ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-signed-right-shift-operator) |
| 13.9.2.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-signed-right-shift-operator-runtime-semantics-evaluation) |
| 13.9.3 | The Unsigned Right Shift Operator ( >>> ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-unsigned-right-shift-operator) |
| 13.9.3.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-unsigned-right-shift-operator-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.9 ([tc39.es](https://tc39.es/ecma262/#sec-bitwise-shift-operators))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Bitwise shift operators | Supported with Limitations | `tests/Jroc.Tests/BinaryOperator/ExecutionTests.cs`<br>`tests/Jroc.Tests/CompoundAssignment/ExecutionTests.cs` |  | Left shift, signed right shift, and unsigned right shift are lowered through LIR/native IL paths for numeric operands with runtime dynamic fallback. Coverage includes direct binary operators and compound shift assignments. |

