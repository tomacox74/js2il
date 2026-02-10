<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 27.5: Generator Objects

[Back to Section27](Section27.md) | [Back to Index](../Index.md)

_Lists clause numbers/titles/links only (no spec text)._

| Clause | Title | Status | Link |
|---:|---|---|---|
| 27.5 | Generator Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generator-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 27.5.1 | The %GeneratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-generator-prototype) |
| 27.5.1.1 | %GeneratorPrototype%.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-generator.prototype.constructor) |
| 27.5.1.2 | %GeneratorPrototype%.next ( value ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-generator.prototype.next) |
| 27.5.1.3 | %GeneratorPrototype%.return ( value ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-generator.prototype.return) |
| 27.5.1.4 | %GeneratorPrototype%.throw ( exception ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generator.prototype.throw) |
| 27.5.1.5 | %GeneratorPrototype% [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-generator.prototype-%symbol.tostringtag%) |
| 27.5.2 | Properties of Generator Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-generator-instances) |
| 27.5.3 | Generator Abstract Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generator-abstract-operations) |
| 27.5.3.1 | GeneratorStart ( generator , generatorBody ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generatorstart) |
| 27.5.3.2 | GeneratorValidate ( generator , generatorBrand ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generatorvalidate) |
| 27.5.3.3 | GeneratorResume ( generator , value , generatorBrand ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generatorresume) |
| 27.5.3.4 | GeneratorResumeAbrupt ( generator , abruptCompletion , generatorBrand ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generatorresumeabrupt) |
| 27.5.3.5 | GetGeneratorKind ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getgeneratorkind) |
| 27.5.3.6 | GeneratorYield ( iteratorResult ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generatoryield) |
| 27.5.3.7 | Yield ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-yield) |
| 27.5.3.8 | CreateIteratorFromClosure ( closure , generatorBrand , generatorPrototype [ , extraSlots ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createiteratorfromclosure) |

## Support

Feature-level support tracking with test script references.

### 27.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-generator-prototype))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Generator objects support next/return/throw, yield*/iterator delegation, and unwinding through try/finally on return/throw while suspended | Supported with Limitations | [`Generator_BasicNext.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_BasicNext.js)<br>[`Generator_TryFinally_ReturnWhileSuspended.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_TryFinally_ReturnWhileSuspended.js)<br>[`Generator_TryFinally_ThrowWhileSuspended.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_TryFinally_ThrowWhileSuspended.js)<br>[`Generator_TryFinally_Nested_ReturnWhileSuspended.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_TryFinally_Nested_ReturnWhileSuspended.js)<br>[`Generator_YieldStar_ArrayBasic.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_YieldStar_ArrayBasic.js)<br>[`Generator_YieldStar_NestedGenerator.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_YieldStar_NestedGenerator.js)<br>[`Generator_YieldStar_PassNextValue.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_YieldStar_PassNextValue.js)<br>[`Generator_YieldStar_ReturnForwards.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_YieldStar_ReturnForwards.js) | `throw()` and `return()` route through `try/finally` blocks (including while suspended at `yield`). Limitations: `try/catch/finally` with `yield` is not supported yet (tracked by #574); spec-shaped prototype properties like `constructor` and `Symbol.toStringTag` are not currently exposed. |

