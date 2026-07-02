<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.5: Generator Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-02T16:37:59Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.5 | Generator Function Definitions | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.5.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions-static-semantics-early-errors) |
| 15.5.2 | Runtime Semantics: EvaluateGeneratorBody | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluategeneratorbody) |
| 15.5.3 | Runtime Semantics: InstantiateGeneratorFunctionObject | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiategeneratorfunctionobject) |
| 15.5.4 | Runtime Semantics: InstantiateGeneratorFunctionExpression | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiategeneratorfunctionexpression) |
| 15.5.5 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 15.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Generator function declaration/expression (function*) | Supported with Limitations | [`Generator_BasicNext.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_BasicNext.js)<br>[`arguments-with-arguments-lex.js`](../../../tests/Jroc.Test262.Tests/language/expressions/generators/JavaScript/arguments-with-arguments-lex.js) | `test/language/expressions/generators/arguments-with-arguments-lex.js` | Synchronous generators compile via lowering to a state machine. Generator parameter initialization runs once at first start and can observe the original generator-call arguments object before body lexical declarations. Known limitations: throw/return propagation through try/finally is not fully implemented. |
| yield expression | Supported | [`Generator_BasicNext.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_BasicNext.js)<br>[`captured-free-vars.js`](../../../tests/Jroc.Test262.Tests/language/expressions/yield/JavaScript/captured-free-vars.js) |  | Supports yielding values, parent-scope captures in yielded operands, and resumption via next(). |
| yield* delegation | Supported with Limitations | [`Generator_YieldStar_ArrayBasic.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_YieldStar_ArrayBasic.js)<br>[`Generator_YieldStar_NestedGenerator.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_YieldStar_NestedGenerator.js)<br>[`Generator_YieldStar_PassNextValue.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_YieldStar_PassNextValue.js)<br>[`Generator_YieldStar_ReturnForwards.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_YieldStar_ReturnForwards.js)<br>[`star-array.js`](../../../tests/Jroc.Test262.Tests/language/expressions/yield/JavaScript/star-array.js)<br>[`star-iterable.js`](../../../tests/Jroc.Test262.Tests/language/expressions/yield/JavaScript/star-iterable.js) |  | Supports delegation through the iterator protocol for arrays, strings, typed arrays, nested jroc GeneratorObject instances, and user-defined [Symbol.iterator] iterables. Limitations: full custom throw/return forwarding and IteratorClose behavior for abrupt delegating-generator completion remain incomplete; throw/return propagation through try/finally remains incomplete. |

