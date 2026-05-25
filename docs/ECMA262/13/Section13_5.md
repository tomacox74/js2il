<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.5: Unary Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T13:09:52Z

JS2IL covers the unary operator forms exercised by the current repo tests and imported test262 cases. `delete` remains tracked as limited because strict-mode early errors and some host-object deletion edge cases are not yet exhaustively validated.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.5 | Unary Operators | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-unary-operators) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.5.1 | The delete Operator | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-delete-operator) |
| 13.5.1.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-delete-operator-static-semantics-early-errors) |
| 13.5.1.2 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-delete-operator-runtime-semantics-evaluation) |
| 13.5.2 | The void Operator | Supported | [tc39.es](https://tc39.es/ecma262/#sec-void-operator) |
| 13.5.2.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-void-operator-runtime-semantics-evaluation) |
| 13.5.3 | The typeof Operator | Supported | [tc39.es](https://tc39.es/ecma262/#sec-typeof-operator) |
| 13.5.3.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-typeof-operator-runtime-semantics-evaluation) |
| 13.5.4 | Unary + Operator | Supported | [tc39.es](https://tc39.es/ecma262/#sec-unary-plus-operator) |
| 13.5.4.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-unary-plus-operator-runtime-semantics-evaluation) |
| 13.5.5 | Unary - Operator | Supported | [tc39.es](https://tc39.es/ecma262/#sec-unary-minus-operator) |
| 13.5.5.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-unary-minus-operator-runtime-semantics-evaluation) |
| 13.5.6 | Bitwise NOT Operator ( ~ ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-bitwise-not-operator) |
| 13.5.6.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-bitwise-not-operator-runtime-semantics-evaluation) |
| 13.5.7 | Logical NOT Operator ( ! ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-logical-not-operator) |
| 13.5.7.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-logical-not-operator-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-delete-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| delete for identifiers, properties, and index accesses | Supported with Limitations | [`11.4.1-0-1.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/delete/JavaScript/11.4.1-0-1.js)<br>[`11.4.1-2-2.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/delete/JavaScript/11.4.1-2-2.js)<br>[`11.4.1-3-1.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/delete/JavaScript/11.4.1-3-1.js)<br>[`11.4.1-3-2.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/delete/JavaScript/11.4.1-3-2.js)<br>[`11.4.1-3-3.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/delete/JavaScript/11.4.1-3-3.js) |  | Unary lowering implements property/index deletion through `ObjectRuntime.DeleteProperty` / `DeleteItem`, returns `false` for declared identifier bindings, and returns `true` for the non-strict unresolvable-identifier cases covered by test262. This clause remains limited because strict-mode early-error coverage and some host-object deletion edge cases are not yet exhaustively validated. |

### 13.5.2 ([tc39.es](https://tc39.es/ecma262/#sec-void-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Unary void operator (void) | Supported | [`UnaryOperator_VoidOperator.js`](../../../tests/Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_VoidOperator.js) |  | Evaluates operand for side effects and yields undefined (commonly used as `void 0` by transpiled/compiled JS). Implemented in IR lowering (HIR->LIR). |

### 13.5.3 ([tc39.es](https://tc39.es/ecma262/#sec-typeof-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| typeof | Supported | [`UnaryOperator_Typeof.js`](../../../tests/Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_Typeof.js)<br>[`proxy.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/typeof/JavaScript/proxy.js) |  | Implemented via unary lowering plus `TypeUtilities.Typeof` / `ObjectRuntime.TypeOfGlobalBinding`, covering ordinary values, undeclared global identifiers, and proxy callability cases (`typeof proxy === "function"` for callable proxies). |

### 13.5.4 ([tc39.es](https://tc39.es/ecma262/#sec-unary-plus-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Unary + | Supported | [`11.4.6-2-1.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/unary-plus/JavaScript/11.4.6-2-1.js)<br>[`S11.4.6_A2.1_T1.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/unary-plus/JavaScript/S11.4.6_A2.1_T1.js)<br>`tests/Js2IL.Test262.Tests/language/expressions/unary-plus/PortExpressionsBatchExecutionTests.cs` |  | Implements JavaScript `ToNumber` coercion for unary plus through the compiler's numeric conversion path. Current coverage now includes empty-string, null, object/default-value, primitive/reference coercion, and unresolvable-reference cases imported from test262. |

### 13.5.5 ([tc39.es](https://tc39.es/ecma262/#sec-unary-minus-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Unary - | Supported | [`UnaryOperator_UnaryNegation_CoercesToNumber.js`](../../../tests/Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_UnaryNegation_CoercesToNumber.js)<br>[`11.4.7-4-1.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/unary-minus/JavaScript/11.4.7-4-1.js)<br>[`S11.4.7_A2.1_T1.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/unary-minus/JavaScript/S11.4.7_A2.1_T1.js) |  | Supports numeric negation with both typed fast-paths and dynamic coercion through the runtime unary-minus helper. Repo tests cover string-to-number coercion, signed zero, and imported test262 GetValue/coercion cases. |

### 13.5.6 ([tc39.es](https://tc39.es/ecma262/#sec-bitwise-not-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Bitwise NOT (~) | Supported | [`UnaryOperator_BitwiseNot.js`](../../../tests/Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_BitwiseNot.js)<br>[`S11.4.8_A2.1_T1.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/bitwise-not/JavaScript/S11.4.8_A2.1_T1.js) |  | Implements JavaScript int32-style bitwise negation, including property/reference GetValue cases from test262. The runtime helper also has a BigInt path, while the cited tests cover the numeric cases currently exercised in-repo. |

### 13.5.7 ([tc39.es](https://tc39.es/ecma262/#sec-logical-not-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Logical NOT (!) | Supported | [`UnaryOperator_LogicalNot.js`](../../../tests/Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_LogicalNot.js)<br>[`UnaryOperator_DoubleNot_NaNTruthiness.js`](../../../tests/Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_DoubleNot_NaNTruthiness.js)<br>[`S11.4.9_A2.1_T1.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/logical-not/JavaScript/S11.4.9_A2.1_T1.js) |  | Supported end-to-end in the IR pipeline through `LIRLogicalNot`, using JavaScript truthiness coercion before inversion. Coverage includes both direct `!` / `!!` cases and imported test262 GetValue checks. |

