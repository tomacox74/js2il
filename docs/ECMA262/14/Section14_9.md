<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.9: The break Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-26T20:12:10Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.9 | The break Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-break-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.9.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-break-statement-static-semantics-early-errors) |
| 14.9.2 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-break-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.9.1 ([tc39.es](https://tc39.es/ecma262/#sec-break-statement-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| break statement early errors | Supported | [`S12.8_A1_T1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/break/JavaScript/S12.8_A1_T1.js)<br>[`S12.8_A1_T2.js`](../../../tests/Js2IL.Test262.Tests/language/statements/break/JavaScript/S12.8_A1_T2.js)<br>[`S12.8_A5_T1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/break/JavaScript/S12.8_A5_T1.js)<br>[`S12.8_A5_T2.js`](../../../tests/Js2IL.Test262.Tests/language/statements/break/JavaScript/S12.8_A5_T2.js)<br>[`S12.8_A6.js`](../../../tests/Js2IL.Test262.Tests/language/statements/break/JavaScript/S12.8_A6.js) |  | The added test262 ports cover parser rejection for break outside breakable statements and for label targets that are not present in the enclosing statement stack. |

### 14.9.2 ([tc39.es](https://tc39.es/ecma262/#sec-break-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| break statement (labeled) | Supported | [`ControlFlow_ForLoop_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledBreak.js)<br>[`ControlFlow_While_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledBreak.js)<br>[`ControlFlow_DoWhile_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledBreak.js)<br>[`ControlFlow_ForOf_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledBreak.js)<br>[`ControlFlow_ForIn_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledBreak.js)<br>[`ControlFlow_Switch_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_LabeledBreak.js) |  | Labeled break exits the targeted statement (loop/switch). Implemented using label stacks during statement lowering. |
| break statement (unlabeled) | Supported | [`ControlFlow_ForLoop_Break_AtThree.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_Break_AtThree.js)<br>[`ControlFlow_While_Break_AtThree.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_Break_AtThree.js)<br>[`ControlFlow_DoWhile_Break_AtThree.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_Break_AtThree.js)<br>[`ControlFlow_ForOf_Break.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Break.js)<br>[`ControlFlow_ForIn_Break.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Break.js) |  |  |
| labeled break from nested do-while statements | Supported | [`S12.8_A4_T1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/break/JavaScript/S12.8_A4_T1.js)<br>[`S12.8_A4_T2.js`](../../../tests/Js2IL.Test262.Tests/language/statements/break/JavaScript/S12.8_A4_T2.js)<br>[`S12.8_A4_T3.js`](../../../tests/Js2IL.Test262.Tests/language/statements/break/JavaScript/S12.8_A4_T3.js) |  | These upstream ports exercise break completions that target the current loop label and an outer loop label from within nested do-while statements. |

