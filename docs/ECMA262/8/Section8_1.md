<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 8.1: Runtime Semantics: Evaluation

[Back to Section8](Section8.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T10:40:21Z

JROC lowers runtime evaluation semantics for the supported JavaScript subset through its HIR/LIR/IL pipeline. This clause remains incomplete because full dynamic evaluation semantics, including general direct/indirect eval, are not yet supported.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 8.1 | Runtime Semantics: Evaluation | Incomplete | [tc39.es](https://tc39.es/ecma262/#Evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 8.1 ([tc39.es](https://tc39.es/ecma262/#Evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Runtime evaluation lowering for supported statements and expressions | Supported with Limitations | [`Function_HelloWorld.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_HelloWorld.js)<br>[`BinaryOperator_AddNumberNumber.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_AddNumberNumber.js)<br>[`ControlFlow_If_BooleanLiteral.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_If_BooleanLiteral.js) |  | Evaluation is implemented for the supported syntax subset via HIR parsing and LIR/IL lowering. Full clause coverage is incomplete: general direct/indirect eval, eval-introduced bindings, and unsupported grammar/runtime forms remain outside current support. |

