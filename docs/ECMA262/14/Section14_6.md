<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.6: The if Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.6 | The if Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-if-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.6.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-if-statement-static-semantics-early-errors) |
| 14.6.2 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-if-statement-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 14.6.2 ([tc39.es](https://tc39.es/ecma262/#sec-if-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| if condition truthiness (non-boolean) | Supported | [`ControlFlow_If_Truthiness.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_Truthiness.js) | Conditions like if (url) are coerced via JS ToBoolean semantics (empty string/0/NaN/undefined/null => false; others => true). |
| if statement (!flag) | Supported | [`ControlFlow_If_NotFlag.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_NotFlag.js) | Logical not in conditional test supported. |
| if statement (LessThan) | Supported | [`ControlFlow_If_LessThan.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_LessThan.js) |  |
| if statement (result == true) | Supported | [`Function_IsEven_CompareResultToTrue.js`](../../../Js2IL.Tests/Function/JavaScript/Function_IsEven_CompareResultToTrue.js) | Compares function-returned boolean to true and branches accordingly. |

