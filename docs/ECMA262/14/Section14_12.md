<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.12: The switch Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.12 | The switch Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-switch-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.12.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-switch-statement-static-semantics-early-errors) |
| 14.12.2 | Runtime Semantics: CaseBlockEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-caseblockevaluation) |
| 14.12.3 | CaseClauseIsSelected ( C , input ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-caseclauseisselected) |
| 14.12.4 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-switch-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 14.12.1 ([tc39.es](https://tc39.es/ecma262/#sec-switch-statement-static-semantics-early-errors))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| switch statement (cases, fallthrough, default, break) | Supported | [`ControlFlow_Switch_Fallthrough.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_Fallthrough.js)<br>[`ControlFlow_Switch_DefaultInMiddle_Fallthrough.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_DefaultInMiddle_Fallthrough.js)<br>[`ControlFlow_Switch_MultiCaseSharedBody.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_MultiCaseSharedBody.js)<br>[`ControlFlow_Switch_NestedBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_NestedBreak.js)<br>[`ControlFlow_Switch_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_LabeledBreak.js) | Supports fallthrough semantics, default placement, multiple case labels sharing a body, nested switch break behavior, and labeled break out of a switch. |

