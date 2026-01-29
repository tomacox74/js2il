<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.4: Method Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 15.4 | Method Definitions | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-method-definitions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 15.4.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-method-definitions-static-semantics-early-errors) |
| 15.4.2 | Static Semantics: HasDirectSuper | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-hasdirectsuper) |
| 15.4.3 | Static Semantics: SpecialMethod | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-specialmethod) |
| 15.4.4 | Runtime Semantics: DefineMethod | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-definemethod) |
| 15.4.5 | Runtime Semantics: MethodDefinitionEvaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-methoddefinitionevaluation) |

## Support

Feature-level support tracking with test script references.

### 15.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-method-definitions-static-semantics-early-errors))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Getter/setter method definitions (get x() / set x(v)) | Not Yet Supported |  | Getters/setters are currently rejected by the validator in both object literals and classes. |

### 15.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-definemethod))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Computed property names in object literals | Supported | [`ObjectLiteral_ComputedKey_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_Basic.js)<br>[`ObjectLiteral_ComputedKey_EvaluationOrder.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_EvaluationOrder.js) | Supports computed property keys and preserves evaluation order. |

### 15.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-methoddefinitionevaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Async/generator methods (class elements) | Supported with Limitations | [`Async_ClassMethod_SimpleAwait.js`](../../../Js2IL.Tests/Async/JavaScript/Async_ClassMethod_SimpleAwait.js)<br>[`Generator_ClassMethod_SimpleYield.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_ClassMethod_SimpleYield.js) | Async and generator methods are supported as class elements; broader async/generator limitations are tracked under Async Function Definitions / Generator Function Definitions. |
| Class instance/static method definitions | Supported with Limitations | [`Classes_ClassWithMethod_HelloWorld.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassWithMethod_HelloWorld.js)<br>[`Classes_ClassWithStaticMethod_HelloWorld.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassWithStaticMethod_HelloWorld.js)<br>[`Classes_Inheritance_SuperMethodCall.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_Inheritance_SuperMethodCall.js) | Covers instance/static methods and super method calls. Limitations are tracked under Class Definitions (e.g., getters/setters, computed method names, and private methods are rejected). |
| Object literal method definition (shorthand method) | Supported | [`ObjectLiteral_ShorthandAndMethod.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ShorthandAndMethod.js)<br>[`Function_ObjectLiteralMethod_ThisBinding.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js) | Supports method syntax in object literals and correct this binding when called as obj.m(). |

