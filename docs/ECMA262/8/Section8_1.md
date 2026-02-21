<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 8.1: Runtime Semantics: Evaluation

[Back to Section8](Section8.md) | [Back to Index](../Index.md)

JS2IL lowers runtime evaluation semantics for the supported JavaScript subset through its HIR/LIR/IL pipeline; unsupported grammar and early-error cases are rejected in parse/validation stages.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 8.1 | Runtime Semantics: Evaluation | Incomplete | [tc39.es](https://tc39.es/ecma262/#Evaluation) |

## Support

Feature-level support tracking with test script references.

### 8.1 ([tc39.es](https://tc39.es/ecma262/#Evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Runtime evaluation lowering for supported statements and expressions | Supported with Limitations | [`Function_HelloWorld.js`](../../../Js2IL.Tests/Function/JavaScript/Function_HelloWorld.js)<br>[`BinaryOperator_AddNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_AddNumberNumber.js)<br>[`ControlFlow_If_BooleanLiteral.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_BooleanLiteral.js) | Evaluation is implemented for the supported syntax subset via HIR parsing and LIR/IL lowering, not as a full coverage implementation of every grammar production in ECMA-262. |

