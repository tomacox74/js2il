<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.4: Update Expressions

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-15T07:30:47Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.4 | Update Expressions | Supported | [tc39.es](https://tc39.es/ecma262/#sec-update-expressions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.4.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-update-expressions-static-semantics-early-errors) |
| 13.4.2 | Postfix Increment Operator | Supported | [tc39.es](https://tc39.es/ecma262/#sec-postfix-increment-operator) |
| 13.4.2.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-postfix-increment-operator-runtime-semantics-evaluation) |
| 13.4.3 | Postfix Decrement Operator | Supported | [tc39.es](https://tc39.es/ecma262/#sec-postfix-decrement-operator) |
| 13.4.3.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-postfix-decrement-operator-runtime-semantics-evaluation) |
| 13.4.4 | Prefix Increment Operator | Supported | [tc39.es](https://tc39.es/ecma262/#sec-prefix-increment-operator) |
| 13.4.4.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-prefix-increment-operator-runtime-semantics-evaluation) |
| 13.4.5 | Prefix Decrement Operator | Supported | [tc39.es](https://tc39.es/ecma262/#sec-prefix-decrement-operator) |
| 13.4.5.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-prefix-decrement-operator-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.4 ([tc39.es](https://tc39.es/ecma262/#sec-update-expressions))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Computed update reference evaluation and coercion order | Supported | `tests/Jroc.Test262.Tests/language/expressions/postfix-increment/OperatorCompletionExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/postfix-decrement/OperatorCompletionExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/prefix-increment/OperatorCompletionExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/prefix-decrement/OperatorCompletionExecutionTests.cs` |  | Computed updates evaluate the base and key expression once, propagate key-expression errors before rejecting null/undefined bases, and reject those bases before observable property-key coercion. The prepared reference is retained through ToNumeric and the write; numeric index fast paths are preserved. |

### 13.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-update-expressions-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Update-expression early errors for invalid assignment targets | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/postfix-increment/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/postfix-decrement/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/prefix-increment/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/prefix-decrement/PortExpressionsBatchExecutionTests.cs` |  | All 142 applicable pinned prefix/postfix increment/decrement cases have verified native results, including strict-mode early errors and invalid assignment targets. Tests named eval.js reject the restricted identifier in strict code; they do not execute eval and remain runnable. |

### 13.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-postfix-increment-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Postfix increment (x++) | Supported | [`UnaryOperator_PlusPlusPostfix.js`](../../../tests/Jroc.Tests/UnaryOperator/JavaScript/UnaryOperator_PlusPlusPostfix.js)<br>`tests/Jroc.Test262.Tests/language/expressions/postfix-increment/PortExpressionsBatchExecutionTests.cs` |  | Postfix increment preserves the original result value while writing back the incremented ToNumber result. The current test262 port covers primitive boolean coercion in addition to the project-local postfix regression. |

### 13.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-postfix-decrement-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Postfix decrement (x--) | Supported | [`UnaryOperator_MinusMinusPostfix.js`](../../../tests/Jroc.Tests/UnaryOperator/JavaScript/UnaryOperator_MinusMinusPostfix.js)<br>`tests/Jroc.Test262.Tests/language/expressions/postfix-decrement/PortExpressionsBatchExecutionTests.cs` |  | Postfix decrement returns the pre-update value while storing the decremented ToNumber result. The new test262 coverage exercises the same observable update flow for primitive boolean inputs. |

### 13.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-prefix-increment-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Prefix increment (++x) | Supported | [`UnaryOperator_PlusPlusPrefix.js`](../../../tests/Jroc.Tests/UnaryOperator/JavaScript/UnaryOperator_PlusPlusPrefix.js)<br>`tests/Jroc.Test262.Tests/language/expressions/prefix-increment/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/prefix-increment/ExecutionTests.cs` | `test/language/expressions/prefix-increment/S11.4.4_A5_T1.js`<br>`test/language/expressions/prefix-increment/S11.4.4_A5_T2.js`<br>`test/language/expressions/prefix-increment/S11.4.4_A6_T3.js` | Prefix increment evaluates its reference once, performs ToNumeric with abrupt completion, preserves BigInt results, writes the incremented value back (including with-environment references), and returns the updated value. |

### 13.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-prefix-decrement-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Prefix decrement (--x) | Supported | [`UnaryOperator_MinusMinusPrefix.js`](../../../tests/Jroc.Tests/UnaryOperator/JavaScript/UnaryOperator_MinusMinusPrefix.js)<br>`tests/Jroc.Test262.Tests/language/expressions/prefix-decrement/PortExpressionsBatchExecutionTests.cs` |  | Prefix decrement writes back and returns the decremented ToNumber result. The checked-in test262 port adds a direct coercion/evaluation sanity check on top of the existing project-local regression. |

