<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.15: The try Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

try/catch/finally is supported for common cases, including throwing and catching arbitrary JS values. Some edge-case fidelity (notably top-level unhandled throw behavior and certain early errors) is not exhaustively validated.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.15 | The try Statement | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-try-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.15.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-try-statement-static-semantics-early-errors) |
| 14.15.2 | Runtime Semantics: CatchClauseEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-catchclauseevaluation) |
| 14.15.3 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-try-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 14.15.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-catchclauseevaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| try/catch (with binding; block-scoped catch parameter) | Supported | [`TryCatch_ScopedParam.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_ScopedParam.js)<br>[`ControlFlow_TryCatch_ScopedParam.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryCatch_ScopedParam.js) | Catch parameter binding is block-scoped to the catch clause. |

### 14.15.3 ([tc39.es](https://tc39.es/ecma262/#sec-try-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| try/catch (no binding) | Supported | [`TryCatch_NoBinding.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding.js)<br>[`TryCatch_NoBinding_NoThrow.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding_NoThrow.js) | Catch blocks handle values thrown within the try region. |
| try/catch/finally | Supported | [`TryCatchFinally_ThrowValue.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatchFinally_ThrowValue.js)<br>[`ControlFlow_TryCatchFinally_ThrowValue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryCatchFinally_ThrowValue.js) |  |
| try/finally (no catch) | Supported with Limitations | [`TryFinally_NoCatch.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryFinally_NoCatch.js)<br>[`TryFinally_NoCatch_Throw.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryFinally_NoCatch_Throw.js)<br>[`TryFinally_Return.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryFinally_Return.js)<br>[`ControlFlow_TryFinally_Return.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryFinally_Return.js) | Finally emission executes on normal and return exits. Execution coverage for unhandled throws is incomplete (some tests are skipped pending stable top-level unhandled error semantics). |

