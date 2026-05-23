<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 27.4: AsyncGeneratorFunction Objects

[Back to Section27](Section27.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-23T12:48:22Z

_Lists clause numbers/titles/links only (no spec text)._

| Clause | Title | Status | Link |
|---:|---|---|---|
| 27.4 | AsyncGeneratorFunction Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 27.4.1 | The AsyncGeneratorFunction Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-constructor) |
| 27.4.1.1 | AsyncGeneratorFunction ( ... parameterArgs , bodyArg ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction) |
| 27.4.2 | Properties of the AsyncGeneratorFunction Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-asyncgeneratorfunction) |
| 27.4.2.1 | AsyncGeneratorFunction.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-prototype) |
| 27.4.3 | Properties of the AsyncGeneratorFunction Prototype Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-asyncgeneratorfunction-prototype) |
| 27.4.3.1 | AsyncGeneratorFunction.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-prototype-constructor) |
| 27.4.3.2 | AsyncGeneratorFunction.prototype.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-prototype-prototype) |
| 27.4.3.3 | AsyncGeneratorFunction.prototype [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-prototype-%symbol.tostringtag%) |
| 27.4.4 | AsyncGeneratorFunction Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-instances) |
| 27.4.4.1 | length | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-instance-length) |
| 27.4.4.2 | name | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-instance-name) |
| 27.4.4.3 | prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-instance-prototype) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 27.4 ([tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorfunction-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| async generator functions via syntax (`async function*`) compile to async iterators (next/return/throw) and integrate with `for await..of` | Supported with Limitations | [`AsyncGenerator_BasicNext.js`](../../../tests/Js2IL.Tests/AsyncGenerator/JavaScript/AsyncGenerator_BasicNext.js)<br>[`AsyncGenerator_ForAwaitOf.js`](../../../tests/Js2IL.Tests/AsyncGenerator/JavaScript/AsyncGenerator_ForAwaitOf.js)<br>[`AsyncGeneratorFunction_length.js`](../../../tests/Js2IL.Test262.Tests/built-ins/AsyncGeneratorFunction/JavaScript/AsyncGeneratorFunction_length.js) |  | Async generators are supported via syntax (`async function*`, `yield`, `await`) and a runtime async iterator object. Async generator function objects now expose a minimal `%AsyncGeneratorFunction.prototype%` constructor surface with correct `length` metadata, but the full spec-level AsyncGeneratorFunction constructor/prototype intrinsic set is still not exposed globally. |

