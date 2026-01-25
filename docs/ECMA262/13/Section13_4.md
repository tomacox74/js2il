<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.4: Update Expressions

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.4 | Update Expressions | Supported | [tc39.es](https://tc39.es/ecma262/#sec-update-expressions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.4.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-update-expressions-static-semantics-early-errors) |
| 13.4.2 | Postfix Increment Operator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-postfix-increment-operator) |
| 13.4.2.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-postfix-increment-operator-runtime-semantics-evaluation) |
| 13.4.3 | Postfix Decrement Operator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-postfix-decrement-operator) |
| 13.4.3.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-postfix-decrement-operator-runtime-semantics-evaluation) |
| 13.4.4 | Prefix Increment Operator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-prefix-increment-operator) |
| 13.4.4.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-prefix-increment-operator-runtime-semantics-evaluation) |
| 13.4.5 | Prefix Decrement Operator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-prefix-decrement-operator) |
| 13.4.5.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-prefix-decrement-operator-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 13.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-postfix-decrement-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| typeof | Supported | [`UnaryOperator_Typeof.js`](../../../Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_Typeof.js) | Implemented via JavaScriptRuntime.TypeUtilities::Typeof and IL emission for UnaryExpression(typeof). typeof null returns 'object'; functions report 'function'; objects report 'object'. |

### 13.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-prefix-increment-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Unary ++ (Prefix increment) | Supported | [`UnaryOperator_PlusPlusPrefix.js`](../../../Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_PlusPlusPrefix.js) | Increments the value first, then returns the new value. |

### 13.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-prefix-decrement-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Unary -- (Prefix decrement) | Supported | [`UnaryOperator_MinusMinusPrefix.js`](../../../Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_MinusMinusPrefix.js) | Decrements the value first, then returns the new value. |

### 13.4.6 ([tc39.es](https://tc39.es/ecma262/#sec-bitwise-not-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Unary ~ (Bitwise NOT) | Supported | [`UnaryOperator_BitwiseNot.js`](../../../Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_BitwiseNot.js) | Converts operand to int32, applies bitwise NOT, converts back to double. Used in bit manipulation patterns. |

### 13.4.7 ([tc39.es](https://tc39.es/ecma262/#sec-logical-not-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Unary ! (Logical not) | Supported | [`UnaryOperator_LogicalNot.js`](../../../Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_LogicalNot.js)<br>[`ControlFlow_If_NotFlag.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_NotFlag.js) | Supported end-to-end in IR pipeline (HIR unary + LIRLogicalNot) using JavaScriptRuntime.TypeUtilities.ToBoolean for JS truthiness, then invert. Covered both in a dedicated unary-operator fixture and in control-flow conditionals (if (!x) ...). |

### 13.4.9 ([tc39.es](https://tc39.es/ecma262/#sec-postfix-increment-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Unary ++ (Postfix increment) | Supported | [`UnaryOperator_PlusPlusPostfix.js`](../../../Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_PlusPlusPostfix.js) |  |

### 13.4.10 ([tc39.es](https://tc39.es/ecma262/#sec-postfix-decrement-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Unary -- (Postfix decrement) | Supported | [`UnaryOperator_MinusMinusPostfix.js`](../../../Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_MinusMinusPostfix.js) |  |

