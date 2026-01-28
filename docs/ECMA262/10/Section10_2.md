<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.2: ECMAScript Function Objects

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 10.2 | ECMAScript Function Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-function-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 10.2.1 | [[Call]] ( thisArgument , argumentsList ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-function-objects-call-thisargument-argumentslist) |
| 10.2.1.1 | PrepareForOrdinaryCall ( F , newTarget ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-prepareforordinarycall) |
| 10.2.1.2 | OrdinaryCallBindThis ( F , calleeContext , thisArgument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinarycallbindthis) |
| 10.2.1.3 | Runtime Semantics: EvaluateBody | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-evaluatebody) |
| 10.2.1.4 | OrdinaryCallEvaluateBody ( F , argumentsList ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinarycallevaluatebody) |
| 10.2.2 | [[Construct]] ( argumentsList , newTarget ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-function-objects-construct-argumentslist-newtarget) |
| 10.2.3 | OrdinaryFunctionCreate ( functionPrototype , sourceText , ParameterList , Body , thisMode , env , privateEnv ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryfunctioncreate) |
| 10.2.4 | AddRestrictedFunctionProperties ( F , realm ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-addrestrictedfunctionproperties) |
| 10.2.4.1 | %ThrowTypeError% ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%throwtypeerror%) |
| 10.2.5 | MakeConstructor ( F [ , writablePrototype [ , prototype ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makeconstructor) |
| 10.2.6 | MakeClassConstructor ( F ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makeclassconstructor) |
| 10.2.7 | MakeMethod ( F , homeObject ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makemethod) |
| 10.2.8 | DefineMethodProperty ( homeObject , key , closure , enumerable ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-definemethodproperty) |
| 10.2.9 | SetFunctionName ( F , name [ , prefix ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-setfunctionname) |
| 10.2.10 | SetFunctionLength ( F , length ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-setfunctionlength) |
| 10.2.11 | FunctionDeclarationInstantiation ( func , argumentsList ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-functiondeclarationinstantiation) |

## Support

Feature-level support tracking with test script references.

### 10.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-function-objects-call-thisargument-argumentslist))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Calling functions with parameters and return values | Supported with Limitations | [`Function_HelloWorld.js`](../../../Js2IL.Tests/Function/JavaScript/Function_HelloWorld.js)<br>[`Function_GlobalFunctionWithMultipleParameters.js`](../../../Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionWithMultipleParameters.js)<br>[`Function_ReturnsStaticValueAndLogs.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ReturnsStaticValueAndLogs.js) | Supports basic calling convention and return semantics for supported language subset. Full argument object semantics and strict-mode differences are not implemented. |
| this binding for methods and non-arrow functions | Supported with Limitations | [`Function_ObjectLiteralMethod_ThisBinding.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js)<br>[`Classes_ClassMethod_ReturnsThis_IsSelf_Log.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_ReturnsThis_IsSelf_Log.js) | Supports this binding for object-literal and class method calls. Arrow-function lexical this and strict-mode edge cases may differ from spec. |

### 10.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-function-objects-construct-argumentslist-newtarget))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Constructing with class constructors (new) and constructor return override | Supported with Limitations | [`Classes_DeclareEmptyClass.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js)<br>[`Classes_Constructor_ReturnObjectOverridesThis.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_Constructor_ReturnObjectOverridesThis.js)<br>[`CommonJS_Export_ClassWithConstructor.js`](../../../Js2IL.Tests/CommonJS/JavaScript/CommonJS_Export_ClassWithConstructor.js) | Supports constructor calls for supported class subset. newTarget/super/inheritance and many exotic construction behaviors are not implemented. |

### 10.2.11 ([tc39.es](https://tc39.es/ecma262/#sec-functiondeclarationinstantiation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Closures and function declaration instantiation | Supported with Limitations | [`Function_ClosureMutatesOuterVariable.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ClosureMutatesOuterVariable.js)<br>[`Function_NestedFunctionAccessesMultipleScopes.js`](../../../Js2IL.Tests/Function/JavaScript/Function_NestedFunctionAccessesMultipleScopes.js)<br>[`Function_ClosureEscapesScope_ObjectLiteralProperty.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ClosureEscapesScope_ObjectLiteralProperty.js) | Closure semantics are implemented via scope-as-class instances and parent scope capture. Arguments object aliasing and several environment-record details are not implemented. |

