<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.5: Generator Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

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

Feature-level support tracking with test script references.

### 15.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-generator-function-definitions-static-semantics-early-errors))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Generator function declaration/expression (function*) | Supported with Limitations | [`Generator_BasicNext.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_BasicNext.js) | MVP support for synchronous generators compiled via lowering to a state machine. Known limitations: async generators are rejected; throw/return propagation through try/finally is not fully implemented. |
| yield expression | Supported | [`Generator_BasicNext.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_BasicNext.js) | Supports yielding values and resumption via next(). |
| yield* delegation | Supported with Limitations | [`Generator_YieldStar_ArrayBasic.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_YieldStar_ArrayBasic.js)<br>[`Generator_YieldStar_NestedGenerator.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_YieldStar_NestedGenerator.js)<br>[`Generator_YieldStar_PassNextValue.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_YieldStar_PassNextValue.js)<br>[`Generator_YieldStar_ReturnForwards.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_YieldStar_ReturnForwards.js) | Supports delegation to normalized indexables (arrays/strings/typed arrays) via NormalizeForOfIterable, and to nested js2il GeneratorObject instances with next/throw/return forwarding. Limitations: delegation to arbitrary user-defined iterators (custom next/return/throw objects) is not implemented; the normalized indexable path does not currently perform full iterator-protocol closing (IteratorClose/iterator.return) when the delegating generator is stopped via return()/throw(); throw/return propagation through try/finally remains incomplete. |

