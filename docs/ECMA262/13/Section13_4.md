<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.4: Update Expressions

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T13:09:52Z

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

### 13.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-update-expressions-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Update-expression early errors for invalid assignment targets | Supported with Limitations | `tests/Js2IL.Test262.Tests/language/expressions/postfix-increment/PortExpressionsBatchExecutionTests.cs`<br>`tests/Js2IL.Test262.Tests/language/expressions/postfix-decrement/PortExpressionsBatchExecutionTests.cs`<br>`tests/Js2IL.Test262.Tests/language/expressions/prefix-increment/PortExpressionsBatchExecutionTests.cs`<br>`tests/Js2IL.Test262.Tests/language/expressions/prefix-decrement/PortExpressionsBatchExecutionTests.cs` |  | The checked-in update-expression ports cover valid identifier targets and the runtime ToNumber/GetValue/PutValue flows behind them. Broader early-error coverage (for example every invalid complex target form) is still not exhaustively tracked in-repo. |

### 13.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-postfix-increment-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Postfix increment (x++) | Supported | [`UnaryOperator_PlusPlusPostfix.js`](../../../tests/Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_PlusPlusPostfix.js)<br>`tests/Js2IL.Test262.Tests/language/expressions/postfix-increment/PortExpressionsBatchExecutionTests.cs` |  | Postfix increment preserves the original result value while writing back the incremented ToNumber result. The current test262 port covers primitive boolean coercion in addition to the project-local postfix regression. |

### 13.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-postfix-decrement-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Postfix decrement (x--) | Supported | [`UnaryOperator_MinusMinusPostfix.js`](../../../tests/Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_MinusMinusPostfix.js)<br>`tests/Js2IL.Test262.Tests/language/expressions/postfix-decrement/PortExpressionsBatchExecutionTests.cs` |  | Postfix decrement returns the pre-update value while storing the decremented ToNumber result. The new test262 coverage exercises the same observable update flow for primitive boolean inputs. |

### 13.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-prefix-increment-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Prefix increment (++x) | Supported | [`UnaryOperator_PlusPlusPrefix.js`](../../../tests/Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_PlusPlusPrefix.js)<br>`tests/Js2IL.Test262.Tests/language/expressions/prefix-increment/PortExpressionsBatchExecutionTests.cs` |  | Prefix increment writes back the incremented ToNumber result and returns that updated value. The checked-in test262 port adds explicit primitive-coercion coverage alongside the existing project-local prefix regression. |

### 13.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-prefix-decrement-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Prefix decrement (--x) | Supported | [`UnaryOperator_MinusMinusPrefix.js`](../../../tests/Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_MinusMinusPrefix.js)<br>`tests/Js2IL.Test262.Tests/language/expressions/prefix-decrement/PortExpressionsBatchExecutionTests.cs` |  | Prefix decrement writes back and returns the decremented ToNumber result. The checked-in test262 port adds a direct coercion/evaluation sanity check on top of the existing project-local regression. |

