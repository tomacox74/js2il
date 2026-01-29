<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 27.3: GeneratorFunction Objects

[Back to Section27](Section27.md) | [Back to Index](../Index.md)

_Lists clause numbers/titles/links only (no spec text) in the index above. See appendix for extracted spec text._

| Clause | Title | Status | Link |
|---:|---|---|---|
| 27.3 | GeneratorFunction Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 27.3.1 | The GeneratorFunction Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-constructor) |
| 27.3.1.1 | GeneratorFunction ( ... parameterArgs , bodyArg ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction) |
| 27.3.2 | Properties of the GeneratorFunction Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-generatorfunction-constructor) |
| 27.3.2.1 | GeneratorFunction.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction.prototype) |
| 27.3.3 | Properties of the GeneratorFunction Prototype Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-generatorfunction-prototype-object) |
| 27.3.3.1 | GeneratorFunction.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction.prototype.constructor) |
| 27.3.3.2 | GeneratorFunction.prototype.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction.prototype.prototype) |
| 27.3.3.3 | GeneratorFunction.prototype [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction.prototype-%symbol.tostringtag%) |
| 27.3.4 | GeneratorFunction Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-instances) |
| 27.3.4.1 | length | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-instances-length) |
| 27.3.4.2 | name | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-instances-name) |
| 27.3.4.3 | prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-instances-prototype) |

## Support

Feature-level support tracking with test script references.

### 27.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-generatorfunction-instances))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Generator function declarations/expressions (`function*`) compile and return generator objects | Supported | [`Generator_BasicNext.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_BasicNext.js)<br>[`Generator_YieldStar_ArrayBasic.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_YieldStar_ArrayBasic.js)<br>[`Generator_YieldStar_NestedGenerator.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_YieldStar_NestedGenerator.js)<br>[`Generator_YieldStar_ReturnForwards.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_YieldStar_ReturnForwards.js) | JS2IL supports generator syntax (`function*`, `yield`, `yield*`) but does not currently expose a spec-shaped `GeneratorFunction` constructor/prototype as global intrinsics. |

