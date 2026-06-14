<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.6: The if Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-26T20:12:10Z

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

### 14.6.1 ([tc39.es](https://tc39.es/ecma262/#sec-if-statement-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| if statement early errors | Supported | [`S12.5_A6_T1.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/S12.5_A6_T1.js)<br>[`S12.5_A6_T2.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/S12.5_A6_T2.js)<br>[`S12.5_A8.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/S12.5_A8.js)<br>[`S12.5_A11.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/S12.5_A11.js)<br>[`if-cls-no-else.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/if-cls-no-else.js) |  | The expanded test262 slice now explicitly covers parser rejection for missing parenthesized conditions, empty if conditions, invalid object-literal-looking conditions, and class declarations used directly in statement position. |

### 14.6.2 ([tc39.es](https://tc39.es/ecma262/#sec-if-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| if condition truthiness (non-boolean) | Supported | [`ControlFlow_If_Truthiness.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_If_Truthiness.js) |  | Conditions like if (url) are coerced via JS ToBoolean semantics (empty string/0/NaN/undefined/null => false; others => true). |
| if statement (!flag) | Supported | [`ControlFlow_If_NotFlag.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_If_NotFlag.js) |  | Logical not in conditional test supported. |
| if statement (LessThan) | Supported | [`ControlFlow_If_LessThan.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_If_LessThan.js) |  |  |
| if statement (result == true) | Supported | [`Function_IsEven_CompareResultToTrue.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_IsEven_CompareResultToTrue.js) |  | Compares function-returned boolean to true and branches accordingly. |
| if statement nested branches and abrupt condition evaluation | Supported | [`S12.5_A1.1_T2.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/S12.5_A1.1_T2.js)<br>[`S12.5_A1.2_T2.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/S12.5_A1.2_T2.js)<br>[`S12.5_A1_T2.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/S12.5_A1_T2.js)<br>[`S12.5_A3.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/S12.5_A3.js)<br>[`S12.5_A12_T2.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/S12.5_A12_T2.js)<br>[`S12.5_A12_T3.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/S12.5_A12_T3.js)<br>[`S12.5_A12_T4.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/S12.5_A12_T4.js)<br>[`let-block-with-newline.js`](../../../tests/Jroc.Test262.Tests/language/statements/if/JavaScript/let-block-with-newline.js) |  | The current test262 ports extend evidence for if/else truthiness, condition-first abrupt completion, nested branch selection, and the noStrict ASI form using `let {` in statement position. |

