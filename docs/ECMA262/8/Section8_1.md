<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 8.1: Runtime Semantics: Evaluation

[Back to Section8](Section8.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-07T20:48:13Z

JROC lowers runtime evaluation semantics for the supported JavaScript subset through its HIR/LIR/IL pipeline. This clause is supported with limitations because normal evaluation flows are implemented broadly across supported syntax, while general direct/indirect eval and some unsupported grammar/runtime forms are still intentionally outside current support.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 8.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#Evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 8.1 ([tc39.es](https://tc39.es/ecma262/#Evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Runtime evaluation lowering for supported statements and expressions | Supported with Limitations | [`Function_HelloWorld.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_HelloWorld.js)<br>[`BinaryOperator_AddNumberNumber.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_AddNumberNumber.js)<br>[`ControlFlow_If_BooleanLiteral.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_If_BooleanLiteral.js)<br>[`ObjectLiteral_ComputedKey_EvaluationOrder.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_EvaluationOrder.js)<br>[`Function_Call_Spread_EvaluationOrder.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_EvaluationOrder.js)<br>[`Generator_BasicNext.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_BasicNext.js)<br>[`Async_ForAwaitOf_AsyncIterator_BreakCloses.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ForAwaitOf_AsyncIterator_BreakCloses.js) | `test/language/expressions/yield/star-iterable.js`<br>`test/language/statements/for-await-of/head-lhs-async.js`<br>`test/language/expressions/assignment/destructuring/iterator-destructuring-property-reference-target-evaluation-order.js`<br>`test/language/expressions/arrow-function/lexical-this.js`<br>`test/language/expressions/async-generator/yield-star-async-next.js` | Evaluation is implemented broadly for the supported syntax subset via the HIR -> LIR -> IL lowering pipeline, including expression evaluation order, control flow, generators, async iteration, destructuring, and lexical-this behavior. The main intentional limitation remains general direct/indirect eval and eval-introduced bindings, which are still rejected during validation, alongside other unsupported grammar/runtime forms. |

