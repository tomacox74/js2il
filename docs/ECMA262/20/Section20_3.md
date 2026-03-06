<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 20.3: Boolean Objects

[Back to Section20](Section20.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 20.3 | Boolean Objects | Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 20.3.1 | The Boolean Constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean-constructor) |
| 20.3.1.1 | Boolean ( value ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean-constructor-boolean-value) |
| 20.3.2 | Properties of the Boolean Constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-boolean-constructor) |
| 20.3.2.1 | Boolean.prototype | Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean.prototype) |
| 20.3.3 | Properties of the Boolean Prototype Object | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-boolean-prototype-object) |
| 20.3.3.1 | Boolean.prototype.constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean.prototype.constructor) |
| 20.3.3.2 | Boolean.prototype.toString ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean.prototype.tostring) |
| 20.3.3.3 | Boolean.prototype.valueOf ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean.prototype.valueof) |
| 20.3.3.3.1 | ThisBooleanValue ( value ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-thisbooleanvalue) |
| 20.3.4 | Properties of Boolean Instances | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-boolean-instances) |

## Support

Feature-level support tracking with test script references.

### 20.3.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-boolean-constructor-boolean-value))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Boolean(value) (callable primitive conversion) | Supported | [`PrimitiveConversion_Boolean_Callable.js`](../../../Js2IL.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Boolean_Callable.js) | Lowered by the compiler to truthiness conversion (JavaScriptRuntime.TypeUtilities.ToBoolean). |
| new Boolean(value) | Supported | [`NewExpression_Boolean_Sugar.js`](../../../Js2IL.Tests/Literals/JavaScript/NewExpression_Boolean_Sugar.js) | Constructs a Boolean wrapper object with `Boolean.prototype` semantics. |

### 20.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-boolean-prototype-object))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Boolean.prototype constructor/toString/valueOf | Supported | [`NewExpression_Boolean_Sugar.js`](../../../Js2IL.Tests/Literals/JavaScript/NewExpression_Boolean_Sugar.js) | Implements `Boolean.prototype.constructor`, `Boolean.prototype.toString()`, `Boolean.prototype.valueOf()`, and `ThisBooleanValue` behavior for primitives and Boolean wrapper instances. |

