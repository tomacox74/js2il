<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 8.3: Labels

[Back to Section8](Section8.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 8.3 | Labels | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-syntax-directed-operations-labels) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 8.3.1 | Static Semantics: ContainsDuplicateLabels | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-containsduplicatelabels) |
| 8.3.2 | Static Semantics: ContainsUndefinedBreakTarget | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-containsundefinedbreaktarget) |
| 8.3.3 | Static Semantics: ContainsUndefinedContinueTarget | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-containsundefinedcontinuetarget) |

## Support

Feature-level support tracking with test script references.

### 8.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-containsduplicatelabels))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Duplicate label detection | Supported with Limitations | `Js2IL.Tests/ValidatorTests.cs` | JavaScriptAstValidator tracks active labels and rejects duplicate declarations in the same label set; explicit negative duplicate-label coverage is still light. |

### 8.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-containsundefinedbreaktarget))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Undefined break-target detection | Supported with Limitations | `Js2IL.Tests/ParserTests.cs`<br>[`ControlFlow_ForLoop_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledBreak.js) | Parser/validator reject breaks to missing labels and runtime lowering supports valid labeled break targets. |

### 8.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-containsundefinedcontinuetarget))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Undefined/invalid continue-target detection | Supported with Limitations | `Js2IL.Tests/ParserTests.cs`<br>[`ControlFlow_While_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledContinue.js) | Continue targets are validated to reference iteration labels; non-loop targets are rejected. |

