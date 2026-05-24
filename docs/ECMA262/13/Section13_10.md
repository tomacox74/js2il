<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.10: Relational Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T13:09:52Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.10 | Relational Operators | Supported | [tc39.es](https://tc39.es/ecma262/#sec-relational-operators) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.10.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-relational-operators-runtime-semantics-evaluation) |
| 13.10.2 | InstanceofOperator ( V , target ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-instanceofoperator) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.10.1 ([tc39.es](https://tc39.es/ecma262/#sec-relational-operators-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Relational comparison evaluation order and coercion | Supported with Limitations | `tests/Js2IL.Test262.Tests/language/expressions/greater-than/PortExpressionsBatchExecutionTests.cs` |  | The current relational operator slice exercises `>` evaluation order, abrupt-completion propagation, primitive/object coercion, and representative comparison cases from test262. The broader relational family remains marked limited because only the currently imported operator matrix is tracked here. |

### 13.10.2 ([tc39.es](https://tc39.es/ecma262/#sec-instanceofoperator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| instanceof | Supported with Limitations | [`BinaryOperator_InstanceOf_Basic.js`](../../../tests/Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_InstanceOf_Basic.js)<br>[`S11.8.6_A2.1_T1.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/instanceof/JavaScript/S11.8.6_A2.1_T1.js)<br>[`S11.8.6_A2.4_T1.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/instanceof/JavaScript/S11.8.6_A2.4_T1.js)<br>[`S11.8.6_A3.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/instanceof/JavaScript/S11.8.6_A3.js) |  | Implemented through the compiler's `LIRInstanceOfOperator` lowering and `JavaScriptRuntime.Operators.InstanceOf`, with coverage for prototype-chain matches, left-to-right evaluation order, and TypeError on primitive/non-callable right-hand sides. This remains limited because the runtime currently targets delegate-backed constructors plus prototype-chain lookup/built-in error special cases, not the full `@@hasInstance` / exotic callable surface. |

