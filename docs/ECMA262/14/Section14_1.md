<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.1: Statement Semantics

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

JS2IL supports most common statement forms (control flow, loops, switch, try/catch/finally, throw/return) but rejects some statement types in the validator and has known semantic gaps (notably iterator-protocol fidelity for general `for..of`).

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.1 | Statement Semantics | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-statement-semantics) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.1.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-statement-semantics-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 14.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-statement-semantics-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| break/continue (including labeled) | Supported | [`ControlFlow_ForLoop_Break_AtThree.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_Break_AtThree.js)<br>[`ControlFlow_ForLoop_Continue_SkipEven.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_Continue_SkipEven.js)<br>[`ControlFlow_ForLoop_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledBreak.js)<br>[`ControlFlow_ForLoop_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledContinue.js)<br>[`ControlFlow_While_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledBreak.js)<br>[`ControlFlow_While_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledContinue.js) | Covers non-local control flow across nested loops via labels. |
| debugger statement | Not Yet Supported |  | Explicitly rejected by the AST validator. |
| for await..of statement | Supported | [`Async_ForAwaitOf_Array.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_Array.js)<br>[`Async_ForAwaitOf_AsyncIterator_BreakCloses.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_AsyncIterator_BreakCloses.js)<br>[`Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js) | Supported within async functions. Lowered using the async iterator protocol (GetAsyncIterator/AsyncIteratorNext/AsyncIteratorClose). |
| for..in statement (object key enumeration) | Supported with Limitations | [`ControlFlow_ForIn_Object_Basic.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Object_Basic.js)<br>[`ControlFlow_ForIn_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledBreak.js)<br>[`ControlFlow_ForIn_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledContinue.js) | Implemented via runtime key enumeration helpers; edge-case fidelity (e.g., exotic objects / exact ordering semantics) is not exhaustively validated. |
| for..of statement (arrays) | Supported with Limitations | [`ControlFlow_ForOf_Array_Basic.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Array_Basic.js)<br>[`ControlFlow_ForOf_Break.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Break.js)<br>[`ControlFlow_ForOf_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledBreak.js)<br>[`ControlFlow_ForOf_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledContinue.js) | Lowered using array-like normalization + index/length loop; does not implement full iterator-protocol semantics for arbitrary iterables. |
| if statement (truthiness) | Supported | [`ControlFlow_If_Truthiness.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_Truthiness.js) |  |
| iteration statements: for/while/do-while (basic) | Supported | [`ControlFlow_ForLoop_CountToFive.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_CountToFive.js)<br>[`ControlFlow_While_CountDownFromFive.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_CountDownFromFive.js)<br>[`ControlFlow_DoWhile_CountUp_AtLeastOnce.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_CountUp_AtLeastOnce.js) |  |
| return statement (basic) | Supported | [`Function_ReturnsStaticValueAndLogs.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ReturnsStaticValueAndLogs.js) | Return values propagate to callers (boxed as object); validated by execution snapshot. |
| switch statement (fallthrough, nested) | Supported | [`ControlFlow_Switch_Fallthrough.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_Fallthrough.js)<br>[`ControlFlow_Switch_NestedBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_NestedBreak.js)<br>[`ControlFlow_Switch_DefaultInMiddle_Fallthrough.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_DefaultInMiddle_Fallthrough.js) |  |
| try/catch/finally + throw (basic) | Supported | [`ControlFlow_TryCatch_ScopedParam.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryCatch_ScopedParam.js)<br>[`ControlFlow_TryCatchFinally_ThrowValue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryCatchFinally_ThrowValue.js)<br>[`ControlFlow_TryFinally_Return.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryFinally_Return.js) |  |
| with statement | Not Yet Supported |  | Explicitly rejected by the AST validator. |

