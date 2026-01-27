<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.10: The return Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.10 | The return Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-return-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.10.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-return-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 14.10.1 ([tc39.es](https://tc39.es/ecma262/#sec-return-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| return statement (finally executes) | Supported | [`ControlFlow_TryFinally_Return.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryFinally_Return.js)<br>[`TryFinally_Return.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryFinally_Return.js) | Return exits the current function while ensuring finally blocks execute on the way out. |
| return statement (with value) | Supported | [`Function_ReturnsStaticValueAndLogs.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ReturnsStaticValueAndLogs.js) |  |

