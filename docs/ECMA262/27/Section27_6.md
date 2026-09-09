<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 27.6: AsyncGenerator Objects

[Back to Section27](Section27.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-09-09T07:52:45Z

_Lists clause numbers/titles/links only (no spec text)._

| Clause | Title | Status | Link |
|---:|---|---|---|
| 27.6 | AsyncGenerator Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgenerator-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 27.6.1 | The %AsyncGeneratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-asyncgenerator-prototype) |
| 27.6.1.1 | %AsyncGeneratorPrototype%.constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgenerator-prototype-constructor) |
| 27.6.1.2 | %AsyncGeneratorPrototype%.next ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgenerator-prototype-next) |
| 27.6.1.3 | %AsyncGeneratorPrototype%.return ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgenerator-prototype-return) |
| 27.6.1.4 | %AsyncGeneratorPrototype%.throw ( exception ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgenerator-prototype-throw) |
| 27.6.1.5 | %AsyncGeneratorPrototype% [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgenerator-prototype-%symbol.tostringtag%) |
| 27.6.2 | Properties of AsyncGenerator Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-asyncgenerator-intances) |
| 27.6.3 | AsyncGenerator Abstract Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgenerator-abstract-operations) |
| 27.6.3.1 | AsyncGeneratorRequest Records | Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorrequest-records) |
| 27.6.3.2 | AsyncGeneratorStart ( generator , generatorBody ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorstart) |
| 27.6.3.3 | AsyncGeneratorValidate ( generator , generatorBrand ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorvalidate) |
| 27.6.3.4 | AsyncGeneratorEnqueue ( generator , completion , promiseCapability ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorenqueue) |
| 27.6.3.5 | AsyncGeneratorCompleteStep ( generator , completion , done [ , realm ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorcompletestep) |
| 27.6.3.6 | AsyncGeneratorResume ( generator , completion ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorresume) |
| 27.6.3.7 | AsyncGeneratorUnwrapYieldResumption ( resumptionValue ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorunwrapyieldresumption) |
| 27.6.3.8 | AsyncGeneratorYield ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratoryield) |
| 27.6.3.9 | AsyncGeneratorAwaitReturn ( generator ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratorawaitreturn) |
| 27.6.3.10 | AsyncGeneratorDrainQueue ( generator ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-asyncgeneratordrainqueue) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 27.6.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-asyncgenerator-prototype-next))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| async generator objects returned from `async function*` support next/return/throw and are consumable by `for await..of` | Supported with Limitations | [`AsyncGenerator_BasicNext.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_BasicNext.js)<br>[`AsyncGenerator_ForAwaitOf.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_ForAwaitOf.js)<br>[`AsyncGenerator_GeneratedFunctionObject_Semantics.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_GeneratedFunctionObject_Semantics.js)<br>[`expression-await-as-yield-operand.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-generator/JavaScript/expression-await-as-yield-operand.js)<br>[`expression-await-promise-as-yield-operand.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-generator/JavaScript/expression-await-promise-as-yield-operand.js)<br>[`expression-await-thenable-as-yield-operand.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-generator/JavaScript/expression-await-thenable-as-yield-operand.js)<br>[`expression-yield-as-operand.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-generator/JavaScript/expression-yield-as-operand.js)<br>[`expression-yield-as-statement.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-generator/JavaScript/expression-yield-as-statement.js)<br>[`async-func-decl-dstr-array-rest-iteration.js`](../../../tests/Jroc.Test262.Tests/language/statements/for-await-of/JavaScript/async-func-decl-dstr-array-rest-iteration.js)<br>[`async-func-dstr-let-obj-ptrn-empty.js`](../../../tests/Jroc.Test262.Tests/language/statements/for-await-of/JavaScript/async-func-dstr-let-obj-ptrn-empty.js)<br>[`head-lhs-async.js`](../../../tests/Jroc.Test262.Tests/language/statements/for-await-of/JavaScript/head-lhs-async.js)<br>[`async-func-decl-dstr-obj-empty-bool.js`](../../../tests/Jroc.Test262.Tests/language/statements/for-await-of/JavaScript/async-func-decl-dstr-obj-empty-bool.js)<br>[`async-func-decl-dstr-obj-empty-num.js`](../../../tests/Jroc.Test262.Tests/language/statements/for-await-of/JavaScript/async-func-decl-dstr-obj-empty-num.js)<br>[`async-func-decl-dstr-obj-empty-obj.js`](../../../tests/Jroc.Test262.Tests/language/statements/for-await-of/JavaScript/async-func-decl-dstr-obj-empty-obj.js)<br>[`async-func-decl-dstr-obj-empty-string.js`](../../../tests/Jroc.Test262.Tests/language/statements/for-await-of/JavaScript/async-func-decl-dstr-obj-empty-string.js) |  | Async generator objects use a dedicated %AsyncGeneratorPrototype% surface with next/return/throw, constructor, and AsyncGenerator toStringTag properties above %AsyncIteratorPrototype%. Generated async-generator function objects create a fresh iterator scope per invocation and apply their mutable own prototype property to each returned iterator. |

### 27.6.3 ([tc39.es](https://tc39.es/ecma262/#sec-asyncgenerator-abstract-operations))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Queued async-generator requests and awaited yield values | Supported with Limitations | [`expression-await-thenable-as-yield-operand.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-generator/JavaScript/expression-await-thenable-as-yield-operand.js)<br>[`yield-promise-reject-next.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-generator/JavaScript/yield-promise-reject-next.js)<br>[`yield-star-async-next.js`](../../../tests/Jroc.Test262.Tests/language/expressions/async-generator/JavaScript/yield-star-async-next.js)<br>[`AsyncGenerator_YieldAwait.js`](../../../tests/Jroc.Tests/AsyncGenerator/JavaScript/AsyncGenerator_YieldAwait.js) |  | Concurrent next/return/throw requests are serialized without overwriting pending promise capabilities. Yield operands are awaited, rejected yields complete the generator, and yield* selects the async iterator protocol with a sync fallback. Resumed exceptions reject the original request; captured destructuring bindings retain TDZ checks across suspension. Full cross-realm and generator-brand conformance is not claimed. |

