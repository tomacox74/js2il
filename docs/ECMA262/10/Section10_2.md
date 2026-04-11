<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.2: ECMAScript Function Objects

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-07T02:30:25Z

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
| 10.2.9 | SetFunctionName ( F , name [ , prefix ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setfunctionname) |
| 10.2.10 | SetFunctionLength ( F , length ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setfunctionlength) |
| 10.2.11 | FunctionDeclarationInstantiation ( func , argumentsList ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-functiondeclarationinstantiation) |

## Support

Feature-level support tracking with test script references.

### 10.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-function-objects-call-thisargument-argumentslist))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Function calls, this binding, and arguments snapshots | Supported with Limitations | [`Function_Arguments_Basics.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Arguments_Basics.js)<br>[`Function_Call_Basic.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Call_Basic.js)<br>[`Function_ObjectLiteralMethod_ThisBinding.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js)<br>[`Classes_ClassMethod_ReturnsThis_IsSelf_Log.js`](../../../tests/Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_ReturnsThis_IsSelf_Log.js) | Delegate-backed user functions support ordinary calls, receiver-sensitive method calls, lexical this for arrow functions, and a lazily materialized arguments array when the binding is referenced. The runtime still does not implement the full Arguments Exotic Object or every environment-record edge case from the spec. |

### 10.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-function-objects-construct-argumentslist-newtarget))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Constructor calls and new.target-aware function execution | Supported with Limitations | [`Function_NewTarget_NewVsCall.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_NewTarget_NewVsCall.js)<br>[`Function_NewTarget_Arrow_Inherits.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_NewTarget_Arrow_Inherits.js)<br>[`Classes_Constructor_ReturnObjectOverridesThis.js`](../../../tests/Js2IL.Tests/Classes/JavaScript/Classes_Constructor_ReturnObjectOverridesThis.js)<br>[`Classes_DeclareEmptyClass.js`](../../../tests/Js2IL.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js) | JS2IL constructs delegate-backed functions and class constructors by creating an instance, binding this, and forwarding a newTarget value into the call path. Constructor return override works, but super/inheritance semantics and full OrdinaryCreateFromConstructor behavior remain incomplete. |

### 10.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-makeconstructor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Function prototype objects and constructor-style metadata | Supported with Limitations | [`Function_Prototype_ObjectCreate_ObjectPrototype.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Prototype_ObjectCreate_ObjectPrototype.js)<br>[`Function_Prototype_Bind_PropertyExists.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Prototype_Bind_PropertyExists.js)<br>[`Function_Prototype_ToString_Basic.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Prototype_ToString_Basic.js) | Delegate-backed functions lazily synthesize prototype, constructor, name, length, and toString metadata so common library patterns continue to work. These properties are inferred from CLR delegates rather than being installed through the exact SetFunctionName / SetFunctionLength / MakeConstructor abstract-operation flow. |

### 10.2.11 ([tc39.es](https://tc39.es/ecma262/#sec-functiondeclarationinstantiation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Function declaration instantiation and closures | Supported with Limitations | [`Function_ClosureMutatesOuterVariable.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_ClosureMutatesOuterVariable.js)<br>[`Function_NestedFunctionAccessesMultipleScopes.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_NestedFunctionAccessesMultipleScopes.js)<br>[`Function_ClosureEscapesScope_ObjectLiteralProperty.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_ClosureEscapesScope_ObjectLiteralProperty.js) | Scope-as-class lowering gives functions stable lexical captures and nested-scope access. Parameter environments, rest/arguments interactions, and strict-mode corner cases are supported only to the extent needed by the current compiler/runtime feature set. |

