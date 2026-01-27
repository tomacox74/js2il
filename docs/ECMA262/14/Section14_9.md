<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.9: The break Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.9 | The break Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-break-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.9.1 | Static Semantics: Early Errors | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-break-statement-static-semantics-early-errors) |
| 14.9.2 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-break-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 14.9.2 ([tc39.es](https://tc39.es/ecma262/#sec-break-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| break statement (labeled) | Supported | [`ControlFlow_ForLoop_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledBreak.js)<br>[`ControlFlow_While_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledBreak.js)<br>[`ControlFlow_DoWhile_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledBreak.js)<br>[`ControlFlow_ForOf_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledBreak.js)<br>[`ControlFlow_ForIn_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledBreak.js)<br>[`ControlFlow_Switch_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_LabeledBreak.js) | Labeled break exits the targeted statement (loop/switch). Implemented using label stacks during statement lowering. |
| break statement (unlabeled) | Supported | [`ControlFlow_ForLoop_Break_AtThree.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_Break_AtThree.js)<br>[`ControlFlow_While_Break_AtThree.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_Break_AtThree.js)<br>[`ControlFlow_DoWhile_Break_AtThree.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_Break_AtThree.js)<br>[`ControlFlow_ForOf_Break.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Break.js)<br>[`ControlFlow_ForIn_Break.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Break.js) |  |

