<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 20.3: Boolean Objects

[Back to Section20](Section20.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 20.3 | Boolean Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-boolean-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 20.3.1 | The Boolean Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-boolean-constructor) |
| 20.3.1.1 | Boolean ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-boolean-constructor-boolean-value) |
| 20.3.2 | Properties of the Boolean Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-boolean-constructor) |
| 20.3.2.1 | Boolean.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean.prototype) |
| 20.3.3 | Properties of the Boolean Prototype Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-boolean-prototype-object) |
| 20.3.3.1 | Boolean.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean.prototype.constructor) |
| 20.3.3.2 | Boolean.prototype.toString ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean.prototype.tostring) |
| 20.3.3.3 | Boolean.prototype.valueOf ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean.prototype.valueof) |
| 20.3.3.3.1 | ThisBooleanValue ( value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-thisbooleanvalue) |
| 20.3.4 | Properties of Boolean Instances | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-boolean-instances) |

## Support

Feature-level support tracking with test script references.

### 20.3.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-boolean-constructor-boolean-value))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Boolean(value) (callable primitive conversion) | Supported with Limitations | [`PrimitiveConversion_Boolean_Callable.js`](../../../Js2IL.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Boolean_Callable.js) | Lowered by the compiler to truthiness conversion (JavaScriptRuntime.TypeUtilities.ToBoolean). Limitations: some runtime-only types (e.g. BigInt/BigInteger) are currently treated as always truthy. |
| new Boolean(value) | Supported with Limitations | [`NewExpression_Boolean_Sugar.js`](../../../Js2IL.Tests/Literals/JavaScript/NewExpression_Boolean_Sugar.js) | Implemented as compiler sugar that returns a primitive boolean. This does not create a Boolean wrapper object, so behavior differs from spec in cases where object/primitive distinction matters (e.g. truthiness, prototype methods). |

