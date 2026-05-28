<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.15: The try Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-26T20:12:55Z

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
| try/catch (with binding; block-scoped catch parameter) | Supported | [`TryCatch_ScopedParam.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryCatch_ScopedParam.js)<br>[`ControlFlow_TryCatch_ScopedParam.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryCatch_ScopedParam.js)<br>[`12.14-13.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/12.14-13.js)<br>[`12.14-14.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/12.14-14.js)<br>[`12.14-15.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/12.14-15.js)<br>[`12.14-16.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/12.14-16.js)<br>[`12.14-7.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/12.14-7.js)<br>[`obj-init-null.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/dstr/obj-init-null.js) |  | Catch parameter binding is block-scoped to the catch clause, including the covered scope-removal checks after catch exits and the destructuring catch-parameter TypeError case added by the current test262 slice. |

### 14.15.3 ([tc39.es](https://tc39.es/ecma262/#sec-try-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| legacy try/catch/finally control-flow ports | Supported | [`S12.14_A1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A1.js)<br>[`S12.14_A2.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A2.js)<br>[`S12.14_A3.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A3.js)<br>[`S12.14_A4.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A4.js)<br>[`S12.14_A5.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A5.js)<br>[`S12.14_A6.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A6.js)<br>[`S12.14_A7_T1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A7_T1.js)<br>[`S12.14_A7_T2.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A7_T2.js)<br>[`S12.14_A8.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A8.js)<br>[`S12.14_A10_T1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A10_T1.js)<br>[`S12.14_A11_T1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A11_T1.js)<br>[`S12.14_A12_T1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/S12.14_A12_T1.js) |  | The added legacy ports now cover catch binding visibility, loop-inside-try throw propagation across while/for/for-in, nested try/catch/finally evaluation, try/finally sequencing, and throw propagation through surrounding control flow. |
| try/catch (no binding) | Supported | [`TryCatch_NoBinding.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding.js)<br>[`TryCatch_NoBinding_NoThrow.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding_NoThrow.js) |  | Catch blocks handle values thrown within the try region. |
| try/catch/finally | Supported | [`TryCatchFinally_ThrowValue.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryCatchFinally_ThrowValue.js)<br>[`ControlFlow_TryCatchFinally_ThrowValue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryCatchFinally_ThrowValue.js)<br>[`completion-values-fn-finally-return.js`](../../../tests/Js2IL.Test262.Tests/language/statements/try/JavaScript/completion-values-fn-finally-return.js) |  | Supports try/catch/finally completion propagation, including finally returns overriding earlier return and throw completions. |
| try/finally (no catch) | Supported with Limitations | [`TryFinally_NoCatch.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryFinally_NoCatch.js)<br>[`TryFinally_NoCatch_Throw.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryFinally_NoCatch_Throw.js)<br>[`TryFinally_Return.js`](../../../tests/Js2IL.Tests/TryCatch/JavaScript/TryFinally_Return.js)<br>[`ControlFlow_TryFinally_Return.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryFinally_Return.js) |  | Finally emission executes on normal completion, return, and escaping throws. The no-catch throw path is covered by `TryFinally_NoCatch_Throw`, but it is asserted through the execution harness's `allowUnhandledException` mode rather than full process-level unhandled-error fidelity checks, so this clause remains tracked as limited. |

