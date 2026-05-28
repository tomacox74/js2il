<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.12: The switch Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-26T20:12:10Z

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

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.12.1 ([tc39.es](https://tc39.es/ecma262/#sec-switch-statement-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| switch statement (cases, fallthrough, default, break) | Supported | [`ControlFlow_Switch_Fallthrough.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_Fallthrough.js)<br>[`ControlFlow_Switch_DefaultInMiddle_Fallthrough.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_DefaultInMiddle_Fallthrough.js)<br>[`ControlFlow_Switch_MultiCaseSharedBody.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_MultiCaseSharedBody.js)<br>[`ControlFlow_Switch_NestedBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_NestedBreak.js)<br>[`ControlFlow_Switch_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_LabeledBreak.js) |  | Supports fallthrough semantics, default placement, multiple case labels sharing a body, nested switch break behavior, and labeled break out of a switch. |
| switch statement early errors | Supported | [`S12.11_A2_T1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/switch/JavaScript/S12.11_A2_T1.js)<br>[`S12.11_A3_T1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/switch/JavaScript/S12.11_A3_T1.js)<br>[`S12.11_A3_T2.js`](../../../tests/Js2IL.Test262.Tests/language/statements/switch/JavaScript/S12.11_A3_T2.js) |  | The current negative ports assert parser rejection for duplicate `default` clauses and malformed switch heads. |

### 14.12.4 ([tc39.es](https://tc39.es/ecma262/#sec-switch-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| switch statement lexical environment isolation | Supported | [`scope-lex-const.js`](../../../tests/Js2IL.Test262.Tests/language/statements/switch/JavaScript/scope-lex-const.js)<br>[`scope-lex-let.js`](../../../tests/Js2IL.Test262.Tests/language/statements/switch/JavaScript/scope-lex-let.js)<br>[`scope-lex-class.js`](../../../tests/Js2IL.Test262.Tests/language/statements/switch/JavaScript/scope-lex-class.js)<br>[`scope-lex-generator.js`](../../../tests/Js2IL.Test262.Tests/language/statements/switch/JavaScript/scope-lex-generator.js)<br>[`scope-lex-async-function.js`](../../../tests/Js2IL.Test262.Tests/language/statements/switch/JavaScript/scope-lex-async-function.js) |  | The expanded switch slice shows that declarations introduced within the case block use the switch lexical environment and are not visible after the statement completes. |

