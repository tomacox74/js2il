<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 27.4: AsyncGeneratorFunction Objects

[Back to Section27](Section27.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-05T19:01:16Z

_Lists clause numbers/titles/links only (no spec text)._

| Clause | Title | Status | Link |
|---:|---|---|---|
| 27.4 | AsyncGeneratorFunction Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 27.4.1 | The AsyncGeneratorFunction Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-constructor) |
| 27.4.1.1 | AsyncGeneratorFunction ( ... parameterArgs , bodyArg ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction) |
| 27.4.2 | Properties of the AsyncGeneratorFunction Constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-asyncgeneratorfunction) |
| 27.4.2.1 | AsyncGeneratorFunction.prototype | Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-prototype) |
| 27.4.3 | Properties of the AsyncGeneratorFunction Prototype Object | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-asyncgeneratorfunction-prototype) |
| 27.4.3.1 | AsyncGeneratorFunction.prototype.constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-prototype-constructor) |
| 27.4.3.2 | AsyncGeneratorFunction.prototype.prototype | Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-prototype-prototype) |
| 27.4.3.3 | AsyncGeneratorFunction.prototype [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-prototype-%symbol.tostringtag%) |
| 27.4.4 | AsyncGeneratorFunction Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-instances) |
| 27.4.4.1 | length | Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-instance-length) |
| 27.4.4.2 | name | Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-instance-name) |
| 27.4.4.3 | prototype | Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-instance-prototype) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 27.4 ([tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| async generator functions via syntax (`async function*`) compile to async iterators (next/return/throw) and integrate with `for await..of` | Supported with Limitations | [`AsyncGenerator_BasicNext.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_BasicNext.js)<br>[`AsyncGenerator_ForAwaitOf.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_ForAwaitOf.js)<br>[`AsyncGeneratorFunction_length.js`](../../../tests/Jroc.Test262.Tests/built-ins/AsyncGeneratorFunction/JavaScript/AsyncGeneratorFunction_length.js)<br>[`length.js`](../../../tests/Jroc.Test262.Tests/built-ins/AsyncGeneratorFunction/JavaScript/length.js)<br>[`instance-length.js`](../../../tests/Jroc.Test262.Tests/built-ins/AsyncGeneratorFunction/JavaScript/instance-length.js)<br>[`instance-prototype.js`](../../../tests/Jroc.Test262.Tests/built-ins/AsyncGeneratorFunction/JavaScript/instance-prototype.js)<br>[`constructor.js`](../../../tests/Jroc.Test262.Tests/built-ins/AsyncGeneratorFunction/prototype/JavaScript/constructor.js)<br>[`prototype.js`](../../../tests/Jroc.Test262.Tests/built-ins/AsyncGeneratorFunction/prototype/JavaScript/prototype.js)<br>[`Symbol.toStringTag.js`](../../../tests/Jroc.Test262.Tests/built-ins/AsyncGeneratorFunction/prototype/JavaScript/Symbol.toStringTag.js)<br>[`prop-desc.js`](../../../tests/Jroc.Test262.Tests/built-ins/AsyncGeneratorFunction/prototype/JavaScript/prop-desc.js)<br>[`not-callable.js`](../../../tests/Jroc.Test262.Tests/built-ins/AsyncGeneratorFunction/prototype/JavaScript/not-callable.js) |  | Async generators are supported via syntax (`async function*`, `yield`, `await`) and a runtime async iterator object. The intrinsic constructor and prototype expose the specified length, constructor, prototype, and toStringTag metadata. Dynamic construction returns distinct metadata-correct async-generator function objects, but invoking their dynamically parsed bodies remains unsupported. |

### 27.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-instances))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Compiled async-generator callables are generated function objects | Supported | [`AsyncGenerator_GeneratedFunctionObject_Semantics.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_GeneratedFunctionObject_Semantics.js)<br>`tests/Jroc.Test262.Tests/built-ins/AsyncGeneratorFunction/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/async-generator/ExecutionTests.cs` |  | Async-generator declarations, expressions, object methods, and class methods use generated JsFunctionObject instances with stable identity, own name/length/prototype properties, AsyncGeneratorFunction.prototype inheritance, arbitrary properties, and no [[Construct]]. Each invocation creates a fresh async iterator and execution scope. |

