<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.10: The return Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-11-04T02:43:50Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.10 | The return Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-return-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.10.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-return-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.10 ([tc39.es](https://tc39.es/ecma262/#sec-return-statement))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Return-statement Test262 coverage | Supported | `tests/Jroc.Test262.Tests/language/statements/return/RemainingStatementExecutionTests.cs` | `test/language/statements/return/12.9-1.js`<br>`test/language/statements/return/line-terminators.js`<br>`test/language/statements/return/S12.9_A1_T1.js` | All 16 applicable pinned return cases have verified passing results across all 31 required native variants. Coverage includes return values, automatic semicolon insertion and line terminators, and ten parse-negative cases rejecting return outside a function. These negatives do not require runtime eval and remain runnable. Evidence is branch-local PR validation, not central trusted acceptance. |

### 14.10.1 ([tc39.es](https://tc39.es/ecma262/#sec-return-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| return statement (finally executes) | Supported | [`ControlFlow_TryFinally_Return.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_TryFinally_Return.js)<br>[`TryFinally_Return.js`](../../../tests/Jroc.Tests/TryCatch/JavaScript/TryFinally_Return.js) |  | Return exits the current function while ensuring finally blocks execute on the way out. |
| return statement (with value) | Supported | [`Function_ReturnsStaticValueAndLogs.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ReturnsStaticValueAndLogs.js) |  |  |
| tail-position conditional/logical expressions returned from functions | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/conditional/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/logical-and/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/logical-or/PortExpressionsBatchExecutionTests.cs` |  | Return lowering now preserves the tail-position behavior covered by the current test262 ports for conditional, logical-and, and logical-or expressions without regressing surrounding return-completion handling. |

