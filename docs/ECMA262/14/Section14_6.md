<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.6: The if Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T13:24:00Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.6 | The if Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-if-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.6.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-if-statement-static-semantics-early-errors) |
| 14.6.2 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-if-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.6.2 ([tc39.es](https://tc39.es/ecma262/#sec-if-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| if condition truthiness (non-boolean) | Supported | [`ControlFlow_If_Truthiness.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_Truthiness.js) |  | Conditions like if (url) are coerced via JS ToBoolean semantics (empty string/0/NaN/undefined/null => false; others => true). |
| if statement (!flag) | Supported | [`ControlFlow_If_NotFlag.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_NotFlag.js) |  | Logical not in conditional test supported. |
| if statement (LessThan) | Supported | [`ControlFlow_If_LessThan.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_LessThan.js) |  |  |
| if statement (result == true) | Supported | [`Function_IsEven_CompareResultToTrue.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_IsEven_CompareResultToTrue.js) |  | Compares function-returned boolean to true and branches accordingly. |
| if statement nested branches and abrupt condition evaluation | Supported | [`S12.5_A1.1_T2.js`](../../../tests/Js2IL.Test262.Tests/language/statements/if/JavaScript/S12.5_A1.1_T2.js)<br>[`S12.5_A1.2_T2.js`](../../../tests/Js2IL.Test262.Tests/language/statements/if/JavaScript/S12.5_A1.2_T2.js)<br>[`S12.5_A1_T2.js`](../../../tests/Js2IL.Test262.Tests/language/statements/if/JavaScript/S12.5_A1_T2.js)<br>[`S12.5_A3.js`](../../../tests/Js2IL.Test262.Tests/language/statements/if/JavaScript/S12.5_A3.js)<br>[`S12.5_A12_T2.js`](../../../tests/Js2IL.Test262.Tests/language/statements/if/JavaScript/S12.5_A12_T2.js)<br>[`S12.5_A12_T3.js`](../../../tests/Js2IL.Test262.Tests/language/statements/if/JavaScript/S12.5_A12_T3.js)<br>[`S12.5_A12_T4.js`](../../../tests/Js2IL.Test262.Tests/language/statements/if/JavaScript/S12.5_A12_T4.js)<br>[`let-block-with-newline.js`](../../../tests/Js2IL.Test262.Tests/language/statements/if/JavaScript/let-block-with-newline.js) |  | The current test262 ports extend evidence for if/else truthiness, condition-first abrupt completion, nested branch selection, and the noStrict ASI form using `let {` in statement position. |

