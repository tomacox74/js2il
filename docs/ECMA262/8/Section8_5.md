<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 8.5: Contains

[Back to Section8](Section8.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 8.5 | Contains | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-syntax-directed-operations-contains) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 8.5.1 | Static Semantics: Contains | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-contains) |
| 8.5.2 | Static Semantics: ComputedPropertyContains | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-computedpropertycontains) |

## Support

Feature-level support tracking with test script references.

### 8.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-contains))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Targeted AST containment checks for early-error validation | Supported with Limitations | `Js2IL.Tests/ValidatorTests.cs` | JS2IL performs explicit AST-walk checks for required early-error contexts (for example await/yield/new.target/label rules) rather than implementing every spec Contains production generically. |

### 8.5.2 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-computedpropertycontains))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Computed-property containment checks | Supported with Limitations | `Js2IL.Tests/ValidatorTests.cs`<br>[`ObjectLiteral_ComputedKey_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_Basic.js)<br>[`ObjectLiteral_ComputedKey_EvaluationOrder.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_EvaluationOrder.js) | Computed keys are supported for object literals and tracked during evaluation-order/lowering, but computed keys in object binding patterns are currently rejected. |

