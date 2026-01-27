<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.13: Labelled Statements

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.13 | Labelled Statements | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-labelled-statements) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.13.1 | Static Semantics: Early Errors | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-labelled-statements-static-semantics-early-errors) |
| 14.13.2 | Static Semantics: IsLabelledFunction ( stmt ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-islabelledfunction) |
| 14.13.3 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-labelled-statements-runtime-semantics-evaluation) |
| 14.13.4 | Runtime Semantics: LabelledEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-labelledevaluation) |

## Support

Feature-level support tracking with test script references.

### 14.13.4 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-labelledevaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| labels used for labeled break/continue | Supported | [`ControlFlow_ForLoop_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledBreak.js)<br>[`ControlFlow_ForLoop_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledContinue.js)<br>[`ControlFlow_While_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledBreak.js)<br>[`ControlFlow_While_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledContinue.js)<br>[`ControlFlow_DoWhile_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledBreak.js)<br>[`ControlFlow_DoWhile_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledContinue.js)<br>[`ControlFlow_ForOf_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledBreak.js)<br>[`ControlFlow_ForOf_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledContinue.js)<br>[`ControlFlow_ForIn_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledBreak.js)<br>[`ControlFlow_ForIn_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledContinue.js)<br>[`ControlFlow_Switch_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_LabeledBreak.js) | Supports label targeting for break/continue across nested statements. Annex-B labeled function semantics are not exhaustively tested. |

