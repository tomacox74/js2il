<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.5: Unary Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.5 | Unary Operators | Supported | [tc39.es](https://tc39.es/ecma262/#sec-unary-operators) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.5.1 | The delete Operator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-delete-operator) |
| 13.5.1.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-delete-operator-static-semantics-early-errors) |
| 13.5.1.2 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-delete-operator-runtime-semantics-evaluation) |
| 13.5.2 | The void Operator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-void-operator) |
| 13.5.2.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-void-operator-runtime-semantics-evaluation) |
| 13.5.3 | The typeof Operator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typeof-operator) |
| 13.5.3.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typeof-operator-runtime-semantics-evaluation) |
| 13.5.4 | Unary + Operator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-unary-plus-operator) |
| 13.5.4.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-unary-plus-operator-runtime-semantics-evaluation) |
| 13.5.5 | Unary - Operator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-unary-minus-operator) |
| 13.5.5.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-unary-minus-operator-runtime-semantics-evaluation) |
| 13.5.6 | Bitwise NOT Operator ( ~ ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bitwise-not-operator) |
| 13.5.6.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bitwise-not-operator-runtime-semantics-evaluation) |
| 13.5.7 | Logical NOT Operator ( ! ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-logical-not-operator) |
| 13.5.7.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-logical-not-operator-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 13.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-delete-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Binary + (Addition) | Supported | [`BinaryOperator_AddNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_AddNumberNumber.js)<br>[`BinaryOperator_AddStringNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_AddStringNumber.js)<br>[`BinaryOperator_AddStringString.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_AddStringString.js)<br>[`Classes_ClassConstructor_TwoParams_AddMethod.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_TwoParams_AddMethod.js) | Fast-path string concat; general '+' follows JS coercion via runtime helper. |

### 13.5.2 ([tc39.es](https://tc39.es/ecma262/#sec-void-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Binary - (Subtraction) | Supported | [`BinaryOperator_SubNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_SubNumberNumber.js)<br>[`Classes_ClassConstructor_TwoParams_SubtractMethod.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_TwoParams_SubtractMethod.js) | Numeric subtraction; matches JS semantics for non-numeric via coercion helpers where applicable. |

### 13.5.3 ([tc39.es](https://tc39.es/ecma262/#sec-typeof-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Binary * (Multiplication) | Supported | [`BinaryOperator_MulNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_MulNumberNumber.js) |  |
| Binary / (Division) | Supported | [`BinaryOperator_DivNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_DivNumberNumber.js) |  |
| Binary % (Remainder) | Supported | [`BinaryOperator_ModNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_ModNumberNumber.js) |  |

### 13.5.4 ([tc39.es](https://tc39.es/ecma262/#sec-unary-plus-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Binary ** (Exponentiation) | Supported | [`BinaryOperator_ExpNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_ExpNumberNumber.js) |  |

### 13.5.6 ([tc39.es](https://tc39.es/ecma262/#sec-bitwise-not-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Binary & (Bitwise AND) | Supported | [`BinaryOperator_BitwiseAndNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_BitwiseAndNumberNumber.js) |  |
| Binary ^ (Bitwise XOR) | Supported | [`BinaryOperator_BitwiseXorNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_BitwiseXorNumberNumber.js) |  |
| Binary \| (Bitwise OR) | Supported | [`BinaryOperator_BitwiseOrNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_BitwiseOrNumberNumber.js) |  |

### 13.5.7 ([tc39.es](https://tc39.es/ecma262/#sec-logical-not-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Binary << (Left shift) | Supported | [`BinaryOperator_LeftShiftNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LeftShiftNumberNumber.js) |  |
| Binary >> (Signed right shift) | Supported | [`BinaryOperator_RightShiftNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_RightShiftNumberNumber.js) |  |
| Binary >>> (Unsigned right shift) | Supported | [`BinaryOperator_UnsignedRightShiftNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_UnsignedRightShiftNumberNumber.js) |  |

### 13.5.8 ([tc39.es](https://tc39.es/ecma262/#sec-relational-operators))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Binary < (Less than) | Supported | [`BinaryOperator_LessThan.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LessThan.js) |  |
| Binary <= (Less than or equal) | Supported | [`BinaryOperator_LessThanOrEqual.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LessThanOrEqual.js) |  |
| Binary > (Greater than) | Supported | [`BinaryOperator_GreaterThan.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_GreaterThan.js) |  |
| Binary >= (Greater than or equal) | Supported | [`BinaryOperator_GreaterThanOrEqual.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_GreaterThanOrEqual.js) |  |

### 13.5.9 ([tc39.es](https://tc39.es/ecma262/#sec-equality-operators))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Binary != (Inequality) | Supported | [`BinaryOperator_NotEqual.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_NotEqual.js)<br>[`ControlFlow_If_NotEqual.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_NotEqual.js) | Value result emitted via Ceq inversion; conditional form branches with bne.un. Unboxing/coercion rules mirror equality operator handling for numbers/booleans. |
| Binary !== (Strict inequality) | Supported |  | Implemented alongside != in the IL generator (value + branching). Dedicated tests to be added; semantics match JavaScript strict inequality (no type coercion). |
| Binary == (Equality) | Supported | [`BinaryOperator_Equal.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_Equal.js)<br>[`Function_IsEven_CompareResultToTrue.js`](../../../Js2IL.Tests/Function/JavaScript/Function_IsEven_CompareResultToTrue.js) | Covers numeric and boolean equality, including comparisons against literals and function-returned booleans with selective boxing/unboxing. See also generator snapshot: Js2IL.Tests/BinaryOperator/GeneratorTests.BinaryOperator_EqualBoolean.verified.txt. |

### 13.5.10 ([tc39.es](https://tc39.es/ecma262/#sec-binary-logical-operators))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Binary && (Logical AND) with short-circuit | Supported | [`BinaryOperator_LogicalAnd_Value.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LogicalAnd_Value.js)<br>[`BinaryOperator_LogicalAnd_ShortCircuit.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LogicalAnd_ShortCircuit.js) | Value form returns left if falsy, otherwise right; branching form uses JS ToBoolean for conditions. Right-hand side is not evaluated when short-circuited. |
| Binary \|\| (Logical OR) with short-circuit | Supported | [`BinaryOperator_LogicalOr_Value.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LogicalOr_Value.js)<br>[`BinaryOperator_LogicalOr_ShortCircuit.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LogicalOr_ShortCircuit.js) | Value form returns left if truthy, otherwise right; branching form uses JS ToBoolean for conditions. Right-hand side is not evaluated when short-circuited. Recent fixes ensure strict-equality patterns (e.g. `id === 1024 \|\| id === 2047`) correctly handle captured/boxed variables by performing ToNumber conversion when the variable type is unknown, preventing incorrect direct object-to-number `ceq` comparisons. |
| Binary in (property existence on object) | Supported | [`BinaryOperator_In_Object_OwnAndMissing.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_In_Object_OwnAndMissing.js) | Implements key in obj via JavaScriptRuntime.Operators.In / runtime helpers. Supports own-property checks for ExpandoObject/object literals and array-like indices, with optional prototype-chain traversal when prototype-chain mode is enabled. Does not yet implement full spec TypeError behavior for non-object RHS beyond null/undefined and does not support symbols. |

### 13.5.16 ([tc39.es](https://tc39.es/ecma262/#sec-conditional-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Conditional operator (?:) | Supported | [`ControlFlow_Conditional_Ternary.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Conditional_Ternary.js)<br>[`ControlFlow_Conditional_Ternary_ShortCircuit.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Conditional_Ternary_ShortCircuit.js) | Expression-level branching with both arms coerced to object where needed; only the selected arm is evaluated. Verified via generator and execution tests in ControlFlow subgroup. |

