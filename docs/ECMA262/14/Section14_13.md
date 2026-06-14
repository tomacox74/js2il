<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.13: Labelled Statements

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-26T20:12:10Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.13 | Labelled Statements | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-labelled-statements) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.13.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-labelled-statements-static-semantics-early-errors) |
| 14.13.2 | Static Semantics: IsLabelledFunction ( stmt ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-islabelledfunction) |
| 14.13.3 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-labelled-statements-runtime-semantics-evaluation) |
| 14.13.4 | Runtime Semantics: LabelledEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-labelledevaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.13.1 ([tc39.es](https://tc39.es/ecma262/#sec-labelled-statements-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| labeled statement early errors | Supported | [`continue.js`](../../../tests/Jroc.Test262.Tests/language/statements/labeled/JavaScript/continue.js)<br>[`let-array-with-newline.js`](../../../tests/Jroc.Test262.Tests/language/statements/labeled/JavaScript/let-array-with-newline.js)<br>[`decl-const.js`](../../../tests/Jroc.Test262.Tests/language/statements/labeled/JavaScript/decl-const.js) |  | The added ports cover parser rejection for invalid labeled continue targets, the `let [` lookahead restriction, and lexical declarations used directly as labelled items. |

### 14.13.3 ([tc39.es](https://tc39.es/ecma262/#sec-labelled-statements-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| labeled identifier forms accepted in non-strict code | Supported | [`let-identifier-with-newline.js`](../../../tests/Jroc.Test262.Tests/language/statements/labeled/JavaScript/let-identifier-with-newline.js)<br>[`value-yield-non-strict-escaped.js`](../../../tests/Jroc.Test262.Tests/language/statements/labeled/JavaScript/value-yield-non-strict-escaped.js) |  | The current slice also covers non-strict forms where `let <bindingIdentifier>` and an escaped `yield` token remain valid label identifiers. |

### 14.13.4 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-labelledevaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| labels consumed by nested break and continue ports | Supported | [`S12.8_A4_T1.js`](../../../tests/Jroc.Test262.Tests/language/statements/break/JavaScript/S12.8_A4_T1.js)<br>[`S12.8_A4_T2.js`](../../../tests/Jroc.Test262.Tests/language/statements/break/JavaScript/S12.8_A4_T2.js)<br>[`S12.8_A4_T3.js`](../../../tests/Jroc.Test262.Tests/language/statements/break/JavaScript/S12.8_A4_T3.js)<br>[`S12.7_A9_T1.js`](../../../tests/Jroc.Test262.Tests/language/statements/continue/JavaScript/S12.7_A9_T1.js)<br>[`line-terminators.js`](../../../tests/Jroc.Test262.Tests/language/statements/continue/JavaScript/line-terminators.js) |  | The expanded test262 slice covers label-targeted break/continue across nested loops and the line-terminator case where `continue` must be treated as unlabeled. |
| labels used for labeled break/continue | Supported | [`ControlFlow_ForLoop_LabeledBreak.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledBreak.js)<br>[`ControlFlow_ForLoop_LabeledContinue.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledContinue.js)<br>[`ControlFlow_While_LabeledBreak.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledBreak.js)<br>[`ControlFlow_While_LabeledContinue.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledContinue.js)<br>[`ControlFlow_DoWhile_LabeledBreak.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledBreak.js)<br>[`ControlFlow_DoWhile_LabeledContinue.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledContinue.js)<br>[`ControlFlow_ForOf_LabeledBreak.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledBreak.js)<br>[`ControlFlow_ForOf_LabeledContinue.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledContinue.js)<br>[`ControlFlow_ForIn_LabeledBreak.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledBreak.js)<br>[`ControlFlow_ForIn_LabeledContinue.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledContinue.js)<br>[`ControlFlow_Switch_LabeledBreak.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_Switch_LabeledBreak.js)<br>[`cptn-break.js`](../../../tests/Jroc.Test262.Tests/language/statements/labeled/JavaScript/cptn-break.js)<br>[`cptn-nrml.js`](../../../tests/Jroc.Test262.Tests/language/statements/labeled/JavaScript/cptn-nrml.js) |  | Supports label targeting for break/continue across nested statements and the non-eval labeled completion scenarios covered by the test262 ports. Annex-B labeled function semantics are not exhaustively tested. |

