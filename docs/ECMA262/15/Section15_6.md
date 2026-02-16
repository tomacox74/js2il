<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.6: Async Generator Function Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.6 | Async Generator Function Definitions | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-generator-function-definitions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.6.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-generator-function-definitions-static-semantics-early-errors) |
| 15.6.2 | Runtime Semantics: EvaluateAsyncGeneratorBody | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluateasyncgeneratorbody) |
| 15.6.3 | Runtime Semantics: InstantiateAsyncGeneratorFunctionObject | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncgeneratorfunctionobject) |
| 15.6.4 | Runtime Semantics: InstantiateAsyncGeneratorFunctionExpression | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiateasyncgeneratorfunctionexpression) |
| 15.6.5 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgenerator-definitions-evaluation) |

## Support

Feature-level support tracking with test script references.

### 15.6 ([tc39.es](https://tc39.es/ecma262/#sec-async-generator-function-definitions))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| async generator functions (async function*) | Supported with Limitations | [`AsyncGenerator_BasicNext.js`](../../../Js2IL.Tests/AsyncGenerator/JavaScript/AsyncGenerator_BasicNext.js)<br>[`AsyncGenerator_ForAwaitOf.js`](../../../Js2IL.Tests/AsyncGenerator/JavaScript/AsyncGenerator_ForAwaitOf.js)<br>[`AsyncGenerator_YieldAwait.js`](../../../Js2IL.Tests/AsyncGenerator/JavaScript/AsyncGenerator_YieldAwait.js) | Async generators are compiled via dual-dispatch state machine combining async (Promise-based suspension) with generator (user-controlled iteration) semantics. AsyncGeneratorScope inherits from AsyncScope and includes generator state. AsyncGeneratorObject implements IJavaScriptAsyncIterator with next() returning Promises. Known limitations: throw() and return() protocol methods have known issues; try/catch/finally with async generators may generate invalid IL in some cases. |
| await expression in async generators | Supported | [`AsyncGenerator_YieldAwait.js`](../../../Js2IL.Tests/AsyncGenerator/JavaScript/AsyncGenerator_YieldAwait.js) | Await expressions work within async generator functions, allowing promises to be awaited before yielding values. |
| for await...of with async generators | Supported | [`AsyncGenerator_ForAwaitOf.js`](../../../Js2IL.Tests/AsyncGenerator/JavaScript/AsyncGenerator_ForAwaitOf.js) | for await...of loops correctly consume async generators, awaiting each yielded value. |
| yield expression in async generators | Supported | [`AsyncGenerator_BasicNext.js`](../../../Js2IL.Tests/AsyncGenerator/JavaScript/AsyncGenerator_BasicNext.js) | Yield expressions work within async generator functions, suspending execution and returning Promise-wrapped iterator results. |

