<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.8: The continue Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T13:24:00Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.8 | The continue Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-continue-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.8.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-continue-statement-static-semantics-early-errors) |
| 14.8.2 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-continue-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.8.2 ([tc39.es](https://tc39.es/ecma262/#sec-continue-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| continue statement (labeled) | Supported | [`ControlFlow_ForLoop_LabeledContinue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledContinue.js)<br>[`ControlFlow_While_LabeledContinue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledContinue.js)<br>[`ControlFlow_DoWhile_LabeledContinue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledContinue.js)<br>[`ControlFlow_ForOf_LabeledContinue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledContinue.js)<br>[`ControlFlow_ForIn_LabeledContinue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledContinue.js) |  | Labeled continue targets an enclosing iteration statement; implemented via loop context label mapping during lowering. |
| continue statement (unlabeled) | Supported | [`ControlFlow_ForLoop_Continue_SkipEven.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_Continue_SkipEven.js)<br>[`ControlFlow_While_Continue_SkipEven.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_Continue_SkipEven.js)<br>[`ControlFlow_DoWhile_Continue_SkipEven.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_Continue_SkipEven.js) |  |  |
| continue through catch blocks and line terminators | Supported | [`S12.7_A9_T1.js`](../../../tests/Js2IL.Test262.Tests/language/statements/continue/JavaScript/S12.7_A9_T1.js)<br>[`S12.7_A9_T2.js`](../../../tests/Js2IL.Test262.Tests/language/statements/continue/JavaScript/S12.7_A9_T2.js)<br>[`line-terminators.js`](../../../tests/Js2IL.Test262.Tests/language/statements/continue/JavaScript/line-terminators.js) |  | The added test262 ports cover labeled and unlabeled continue from catch blocks and the line-terminator cases where `continue` must not retain the following label. |

