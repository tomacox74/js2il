<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 15.4: Method Definitions

[Back to Section15](Section15.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-18T20:54:14Z

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
| Computed property names in object literals | Supported | [`ObjectLiteral_ComputedKey_Basic.js`](../../../tests/Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_Basic.js)<br>[`ObjectLiteral_ComputedKey_EvaluationOrder.js`](../../../tests/Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_EvaluationOrder.js) |  | Supports computed property keys and preserves evaluation order. |

### 15.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-methoddefinitionevaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Async/generator methods (class elements) | Supported with Limitations | [`Async_ClassMethod_SimpleAwait.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_ClassMethod_SimpleAwait.js)<br>[`Generator_ClassMethod_SimpleYield.js`](../../../tests/Js2IL.Tests/Generator/JavaScript/Generator_ClassMethod_SimpleYield.js) |  | Async and generator methods are supported as class elements; broader async/generator limitations are tracked under Async Function Definitions / Generator Function Definitions. |
| Class instance/static method definitions | Supported with Limitations | [`Classes_ClassWithMethod_HelloWorld.js`](../../../tests/Js2IL.Tests/Classes/JavaScript/Classes_ClassWithMethod_HelloWorld.js)<br>[`Classes_ClassWithStaticMethod_HelloWorld.js`](../../../tests/Js2IL.Tests/Classes/JavaScript/Classes_ClassWithStaticMethod_HelloWorld.js)<br>[`Classes_Inheritance_SuperMethodCall.js`](../../../tests/Js2IL.Tests/Classes/JavaScript/Classes_Inheritance_SuperMethodCall.js)<br>[`fields-multiple-definitions-static-private-methods-proxy.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/class/elements/JavaScript/fields-multiple-definitions-static-private-methods-proxy.js) |  | Covers instance/static methods, super method calls, and function-valued method properties on constructors/prototypes. Limitations are tracked under Class Definitions (for example arbitrary runtime-computed method names and some private accessor cases remain limited). |
| Class method function object metadata | Supported with Limitations | [`gen-method-length-dflt.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/class/JavaScript/gen-method-length-dflt.js) |  | Class methods are exposed as JavaScript function-valued data properties on the prototype or constructor with non-enumerable configurable descriptors. Function.length is computed from the formal parameter list before the first default/rest parameter; broader function metadata limitations are tracked with function object support. |
| Getter/setter method definitions (get x() / set x(v)) | Supported with Limitations | [`ObjectLiteral_AccessorDefinitions.js`](../../../tests/Js2IL.Tests/Object/JavaScript/ObjectLiteral_AccessorDefinitions.js)<br>[`Classes_AccessorMethods_InstanceAndStatic.js`](../../../tests/Js2IL.Tests/Classes/JavaScript/Classes_AccessorMethods_InstanceAndStatic.js)<br>`tests/Js2IL.Tests/ValidatorTests.cs` |  | Object literal getters/setters now lower through descriptor-backed property definition helpers, preserving accessor/data replacement behavior across duplicate keys and spread overwrites. Identifier-named class instance/static accessors compile and dispatch through the existing runtime property/accessor surface with correct this binding for normal property reads/writes. Computed/private class accessors remain part of the broader unsupported computed/private class element gap. |
| Object literal method definition (shorthand method) | Supported | [`ObjectLiteral_ShorthandAndMethod.js`](../../../tests/Js2IL.Tests/Object/JavaScript/ObjectLiteral_ShorthandAndMethod.js)<br>[`Function_ObjectLiteralMethod_ThisBinding.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js) |  | Supports method syntax in object literals and correct this binding when called as obj.m(). |

