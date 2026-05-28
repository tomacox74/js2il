<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.2: ECMAScript Function Objects

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-24T08:22:42Z

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
| 10.2.4 | AddRestrictedFunctionProperties ( F , realm ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-addrestrictedfunctionproperties) |
| 10.2.4.1 | %ThrowTypeError% ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%throwtypeerror%) |
| 10.2.5 | MakeConstructor ( F [ , writablePrototype [ , prototype ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makeconstructor) |
| 10.2.6 | MakeClassConstructor ( F ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makeclassconstructor) |
| 10.2.7 | MakeMethod ( F , homeObject ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makemethod) |
| 10.2.8 | DefineMethodProperty ( homeObject , key , closure , enumerable ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-definemethodproperty) |
| 10.2.9 | SetFunctionName ( F , name [ , prefix ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setfunctionname) |
| 10.2.10 | SetFunctionLength ( F , length ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setfunctionlength) |
| 10.2.11 | FunctionDeclarationInstantiation ( func , argumentsList ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-functiondeclarationinstantiation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 10.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-function-objects-call-thisargument-argumentslist))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function calls, this binding, and lazy arguments object materialization | Supported with Limitations | [`Function_Arguments_Basics.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Arguments_Basics.js)<br>[`Function_Arguments_MappedParameterAliasing.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Arguments_MappedParameterAliasing.js)<br>[`Function_Arguments_Unmapped_StrictAndComplex.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Arguments_Unmapped_StrictAndComplex.js)<br>[`Function_Call_Basic.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Call_Basic.js)<br>[`Function_ObjectLiteralMethod_ThisBinding.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js)<br>[`Classes_ClassMethod_ReturnsThis_IsSelf_Log.js`](../../../tests/Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_ReturnsThis_IsSelf_Log.js)<br>[`10.4.3-1-10-s.js`](../../../tests/Js2IL.Test262.Tests/language/function-code/JavaScript/10.4.3-1-10-s.js)<br>[`10.4.3-1-11-s.js`](../../../tests/Js2IL.Test262.Tests/language/function-code/JavaScript/10.4.3-1-11-s.js)<br>[`10.4.3-1-12-s.js`](../../../tests/Js2IL.Test262.Tests/language/function-code/JavaScript/10.4.3-1-12-s.js)<br>[`10.4.3-1-13-s.js`](../../../tests/Js2IL.Test262.Tests/language/function-code/JavaScript/10.4.3-1-13-s.js)<br>[`10.4.3-1-14-s.js`](../../../tests/Js2IL.Test262.Tests/language/function-code/JavaScript/10.4.3-1-14-s.js)<br>[`10.4.3-1-15-s.js`](../../../tests/Js2IL.Test262.Tests/language/function-code/JavaScript/10.4.3-1-15-s.js)<br>[`10.4.3-1-27-s.js`](../../../tests/Js2IL.Test262.Tests/language/function-code/JavaScript/10.4.3-1-27-s.js)<br>[`10.4.3-1-28-s.js`](../../../tests/Js2IL.Test262.Tests/language/function-code/JavaScript/10.4.3-1-28-s.js)<br>[`10.4.3-1-29-s.js`](../../../tests/Js2IL.Test262.Tests/language/function-code/JavaScript/10.4.3-1-29-s.js)<br>[`10.4.3-1-30-s.js`](../../../tests/Js2IL.Test262.Tests/language/function-code/JavaScript/10.4.3-1-30-s.js) | suite `pr`<br>suite `nightly`<br>`test/language/function-code/10.4.3-1-10-s.js`<br>`test/language/function-code/10.4.3-1-11-s.js`<br>`test/language/function-code/10.4.3-1-12-s.js`<br>`test/language/function-code/10.4.3-1-13-s.js`<br>`test/language/function-code/10.4.3-1-14-s.js`<br>`test/language/function-code/10.4.3-1-15-s.js`<br>`test/language/function-code/10.4.3-1-27-s.js`<br>`test/language/function-code/10.4.3-1-28-s.js`<br>`test/language/function-code/10.4.3-1-29-s.js`<br>`test/language/function-code/10.4.3-1-30-s.js` | Delegate-backed user functions support ordinary calls, receiver-sensitive method calls, lexical this for arrow functions, and lazy materialization of a dedicated arguments object when the implicit binding is referenced. The checked-in function-code slice now also covers strict ordinary-call this binding for nested function declarations/expressions plus Function-constructor call behavior; remaining gaps are concentrated in the narrower 10.4.4 descriptor/invariant edge cases rather than wholesale lack of arguments-object support. |

### 10.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-function-objects-construct-argumentslist-newtarget))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Constructor calls and new.target-aware function execution | Supported with Limitations | [`Function_NewTarget_NewVsCall.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_NewTarget_NewVsCall.js)<br>[`Function_NewTarget_Arrow_Inherits.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_NewTarget_Arrow_Inherits.js)<br>[`Classes_Constructor_ReturnObjectOverridesThis.js`](../../../tests/Js2IL.Tests/Classes/JavaScript/Classes_Constructor_ReturnObjectOverridesThis.js)<br>[`Classes_DeclareEmptyClass.js`](../../../tests/Js2IL.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js) |  | JS2IL constructs delegate-backed functions and class constructors by creating an instance, binding this, and forwarding a newTarget value into the call path. Constructor return override works, but super/inheritance semantics and full OrdinaryCreateFromConstructor behavior remain incomplete. |

### 10.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-addrestrictedfunctionproperties))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Restricted function caller/arguments properties | Supported with Limitations | [`prototype-rules.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/arrow-function/JavaScript/prototype-rules.js)<br>[`restricted-properties.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/class/JavaScript/restricted-properties.js)<br>[`13.2-5-s.js`](../../../tests/Js2IL.Test262.Tests/language/statements/function/JavaScript/13.2-5-s.js)<br>[`13.2-6-s.js`](../../../tests/Js2IL.Test262.Tests/language/statements/function/JavaScript/13.2-6-s.js)<br>[`13.2-9-s.js`](../../../tests/Js2IL.Test262.Tests/language/statements/function/JavaScript/13.2-9-s.js)<br>[`13.2-10-s.js`](../../../tests/Js2IL.Test262.Tests/language/statements/function/JavaScript/13.2-10-s.js)<br>[`13.2-13-s.js`](../../../tests/Js2IL.Test262.Tests/language/statements/function/JavaScript/13.2-13-s.js)<br>[`13.2-14-s.js`](../../../tests/Js2IL.Test262.Tests/language/statements/function/JavaScript/13.2-14-s.js)<br>[`13.2-17-s.js`](../../../tests/Js2IL.Test262.Tests/language/statements/function/JavaScript/13.2-17-s.js)<br>[`13.2-18-s.js`](../../../tests/Js2IL.Test262.Tests/language/statements/function/JavaScript/13.2-18-s.js) | suite `pr`<br>suite `nightly`<br>`test/language/expressions/arrow-function/prototype-rules.js`<br>`test/language/expressions/class/restricted-properties.js`<br>`test/language/statements/function/13.2-5-s.js`<br>`test/language/statements/function/13.2-6-s.js`<br>`test/language/statements/function/13.2-9-s.js`<br>`test/language/statements/function/13.2-10-s.js`<br>`test/language/statements/function/13.2-13-s.js`<br>`test/language/statements/function/13.2-14-s.js`<br>`test/language/statements/function/13.2-17-s.js`<br>`test/language/statements/function/13.2-18-s.js` | JS2IL installs throwing restricted caller/arguments accessors for the covered callable surfaces, including arrow functions, class constructors, and ordinary strict functions created through declarations/Function constructor code generation. Full %ThrowTypeError% identity/intrinsics coverage remains limited. |

### 10.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-makeconstructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function prototype objects and constructor-style metadata | Supported with Limitations | [`Function_Prototype_ObjectCreate_ObjectPrototype.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Prototype_ObjectCreate_ObjectPrototype.js)<br>[`Function_Prototype_Bind_PropertyExists.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Prototype_Bind_PropertyExists.js)<br>[`Function_Prototype_ToString_Basic.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Prototype_ToString_Basic.js) |  | Delegate-backed functions lazily synthesize prototype, constructor, name, length, and toString metadata so common library patterns continue to work. These properties are inferred from CLR delegates rather than being installed through the exact SetFunctionName / SetFunctionLength / MakeConstructor abstract-operation flow. |

### 10.2.11 ([tc39.es](https://tc39.es/ecma262/#sec-functiondeclarationinstantiation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function declaration instantiation and closures | Supported with Limitations | [`Function_ClosureMutatesOuterVariable.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_ClosureMutatesOuterVariable.js)<br>[`Function_NestedFunctionAccessesMultipleScopes.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_NestedFunctionAccessesMultipleScopes.js)<br>[`Function_ClosureEscapesScope_ObjectLiteralProperty.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_ClosureEscapesScope_ObjectLiteralProperty.js)<br>[`scope-paramsbody-var-close.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/arrow-function/JavaScript/scope-paramsbody-var-close.js)<br>[`named-no-strict-reassign-fn-name-in-body-in-arrow.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/function/JavaScript/named-no-strict-reassign-fn-name-in-body-in-arrow.js)<br>[`named-strict-error-reassign-fn-name-in-body-in-arrow.js`](../../../tests/Js2IL.Test262.Tests/language/expressions/function/JavaScript/named-strict-error-reassign-fn-name-in-body-in-arrow.js) | suite `pr`<br>suite `nightly`<br>`test/language/expressions/arrow-function/scope-paramsbody-var-close.js`<br>`test/language/expressions/function/named-no-strict-reassign-fn-name-in-body-in-arrow.js`<br>`test/language/expressions/function/named-strict-error-reassign-fn-name-in-body-in-arrow.js` | Scope-as-class lowering gives functions stable lexical captures and nested-scope access. The checked-in expression slice now also covers parameter-vs-body environment separation for arrows and named function-expression self-bindings observed through nested arrows; remaining limitations are concentrated in stricter environment-record and exotic-object edge cases. |

