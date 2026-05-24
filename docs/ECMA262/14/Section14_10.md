<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.10: The return Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T04:57:47Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.10 | The return Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-return-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.10.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-return-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.10.1 ([tc39.es](https://tc39.es/ecma262/#sec-return-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| return statement (finally executes) | Supported | [`ControlFlow_TryFinally_Return.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryFinally_Return.js)<br>[`TryFinally_Return.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryFinally_Return.js) |  | Return exits the current function while ensuring finally blocks execute on the way out. |
| return statement (with value) | Supported | [`Function_ReturnsStaticValueAndLogs.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_ReturnsStaticValueAndLogs.js) |  |  |
| tail-position conditional/logical expressions returned from functions | Supported with Limitations | `tests/Js2IL.Test262.Tests/language/expressions/conditional/PortExpressionsBatchExecutionTests.cs`<br>`tests/Js2IL.Test262.Tests/language/expressions/logical-and/PortExpressionsBatchExecutionTests.cs`<br>`tests/Js2IL.Test262.Tests/language/expressions/logical-or/PortExpressionsBatchExecutionTests.cs` |  | Return lowering now preserves the tail-position behavior covered by the current test262 ports for conditional, logical-and, and logical-or expressions without regressing surrounding return-completion handling. |

