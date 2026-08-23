<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.9: Bitwise Shift Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-23T20:02:19Z

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
| Bitwise shift operators | Supported with Limitations | `tests/Jroc.Tests/BinaryOperator/ExecutionTests.cs`<br>`tests/Jroc.Tests/CompoundAssignment/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/left-shift/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/right-shift/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/unsigned-right-shift/ExecutionTests.cs` | `test/language/expressions/unsigned-right-shift/S9.6_A2.1.js`<br>`test/language/expressions/unsigned-right-shift/S9.6_A2.2.js`<br>`test/language/expressions/left-shift/S11.7.1_A5.1_T1.js`<br>`test/language/expressions/left-shift/S9.5_A1_T1.js`<br>`test/language/expressions/left-shift/S9.5_A2.1_T1.js`<br>`test/language/expressions/left-shift/S9.5_A2.2_T1.js`<br>`test/language/expressions/left-shift/S9.5_A2.3_T1.js`<br>`test/language/expressions/right-shift/S11.7.2_A5.1_T1.js`<br>`test/language/expressions/unsigned-right-shift/S11.7.3_A4_T1.js`<br>`test/language/expressions/unsigned-right-shift/S11.7.3_A4_T2.js`<br>`test/language/expressions/unsigned-right-shift/S11.7.3_A4_T3.js`<br>`test/language/expressions/unsigned-right-shift/S11.7.3_A4_T4.js`<br>`test/language/expressions/unsigned-right-shift/S11.7.3_A5.1_T1.js`<br>`test/language/expressions/unsigned-right-shift/S11.7.3_A5.2_T1.js`<br>`test/language/expressions/unsigned-right-shift/S9.6_A1.js` | Left shift, signed right shift, and unsigned right shift preserve left-to-right ToNumeric coercion and abrupt completion, reject mixed Number/BigInt operands, apply modulo-based ToInt32/ToUint32 conversion, and use native IL fast paths only when static numeric types make them semantics-preserving. Unsigned right shift remains unavailable for BigInt as required by ECMA-262. |

