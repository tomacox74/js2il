<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.15: The try Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T04:57:25Z

try/catch/finally is supported for the synchronous cases covered by the current repo tests, including throwing and catching arbitrary JS values plus `finally` blocks on normal completion, return, and escaping throw paths. Remaining caveats are early-error coverage and exact host-level fidelity for uncaught throws after `finally` runs.

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

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.15.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-catchclauseevaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| try/catch (with binding; block-scoped catch parameter) | Supported | [`TryCatch_ScopedParam.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryCatch_ScopedParam.js)<br>[`ControlFlow_TryCatch_ScopedParam.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryCatch_ScopedParam.js)<br>`tests/Js2IL.Test262.Tests/language/statements/try/ExecutionTests.cs` |  | Catch parameter binding is block-scoped to the catch clause, including the covered destructuring catch-parameter cases added by the current test262 slice. |

### 14.15.3 ([tc39.es](https://tc39.es/ecma262/#sec-try-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| try/catch (no binding) | Supported | [`TryCatch_NoBinding.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding.js)<br>[`TryCatch_NoBinding_NoThrow.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding_NoThrow.js) |  | Catch blocks handle values thrown within the try region. |
| try/catch/finally | Supported | [`TryCatchFinally_ThrowValue.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryCatchFinally_ThrowValue.js)<br>[`ControlFlow_TryCatchFinally_ThrowValue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryCatchFinally_ThrowValue.js) |  |  |
| try/finally (no catch) | Supported with Limitations | [`TryFinally_NoCatch.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryFinally_NoCatch.js)<br>[`TryFinally_NoCatch_Throw.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryFinally_NoCatch_Throw.js)<br>[`TryFinally_Return.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryFinally_Return.js)<br>[`ControlFlow_TryFinally_Return.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryFinally_Return.js) |  | Finally emission executes on normal completion, return, and escaping throws. The no-catch throw path is covered by `TryFinally_NoCatch_Throw`, but it is asserted through the execution harness's `allowUnhandledException` mode rather than full process-level unhandled-error fidelity checks, so this clause remains tracked as limited. |

