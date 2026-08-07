<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.4: Method Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-07T18:30:40Z

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

Feature-level support tracking with repo test references and optional test262 evidence.

### 15.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-definemethod))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Computed property names in object literals | Supported | [`ObjectLiteral_ComputedKey_Basic.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_Basic.js)<br>[`ObjectLiteral_ComputedKey_EvaluationOrder.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_EvaluationOrder.js) |  | Supports computed property keys and preserves evaluation order. |

### 15.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-methoddefinitionevaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Async/generator methods (class elements) | Supported with Limitations | [`Async_ClassMethod_SimpleAwait.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_ClassMethod_SimpleAwait.js)<br>[`Generator_ClassMethod_SimpleYield.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_ClassMethod_SimpleYield.js) |  | Async and generator methods are supported as class elements; broader async/generator limitations are tracked under Async Function Definitions / Generator Function Definitions. |
| Class instance/static method definitions | Supported with Limitations | [`Classes_ClassWithMethod_HelloWorld.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassWithMethod_HelloWorld.js)<br>[`Classes_ClassWithStaticMethod_HelloWorld.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassWithStaticMethod_HelloWorld.js)<br>[`Classes_GeneratedMethodFunctionObjects.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_GeneratedMethodFunctionObjects.js)<br>[`Classes_Inheritance_SuperMethodCall.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_Inheritance_SuperMethodCall.js)<br>[`fields-multiple-definitions-static-private-methods-proxy.js`](../../../tests/Jroc.Test262.Tests/language/expressions/class/elements/JavaScript/fields-multiple-definitions-static-private-methods-proxy.js) |  | Synchronous instance/static methods materialize as generated JsFunctionObject properties while direct known calls retain typed dispatch. Covered behavior includes super, computed names, extracted calls with compatible receivers, private-brand enforcement, non-constructability, descriptor identity, and distinct identities across repeated class evaluation. Async/generator methods remain on their dedicated transitional paths. |
| Class method function object metadata | Supported with Limitations | [`gen-method-length-dflt.js`](../../../tests/Jroc.Test262.Tests/language/expressions/class/JavaScript/gen-method-length-dflt.js) |  | Generated class method objects are exposed through non-enumerable configurable writable properties on the prototype or constructor. They carry specification-aligned name/length metadata, lack an ordinary-function prototype property, and are the same identities returned by descriptor APIs. |
| Getter/setter method definitions (get x() / set x(v)) | Supported with Limitations | [`ObjectLiteral_AccessorDefinitions.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_AccessorDefinitions.js)<br>[`ObjectLiteral_GeneratedMethodFunctionObjects.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_GeneratedMethodFunctionObjects.js)<br>[`Classes_AccessorMethods_InstanceAndStatic.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_AccessorMethods_InstanceAndStatic.js)<br>[`Classes_GeneratedMethodFunctionObjects.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_GeneratedMethodFunctionObjects.js)<br>`tests/Jroc.Tests/ValidatorTests.cs` |  | Synchronous object/class getters and setters materialize as generated non-constructable function objects and are installed directly into descriptor-backed properties. Descriptor reflection returns the same getter/setter identities used by reads and writes. Async/generator accessor-family edge cases remain transitional. |
| Object literal method definition (shorthand method) | Supported | [`ObjectLiteral_ShorthandAndMethod.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_ShorthandAndMethod.js)<br>[`ObjectLiteral_GeneratedMethodFunctionObjects.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_GeneratedMethodFunctionObjects.js)<br>[`Function_ObjectLiteralMethod_ThisBinding.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js) |  | Synchronous object-literal methods materialize as generated JsFunctionObject instances with correct member/extracted receiver behavior, computed names, home-object super state, non-constructability, and distinct identity for each object evaluation. |

