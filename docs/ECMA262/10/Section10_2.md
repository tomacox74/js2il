<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.2: ECMAScript Function Objects

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-02T16:37:57Z

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
| Function calls, this binding, and lazy arguments object materialization | Supported with Limitations | [`Function_Arguments_Basics.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_Basics.js)<br>[`Function_Arguments_MappedParameterAliasing.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_MappedParameterAliasing.js)<br>[`Function_Arguments_Unmapped_StrictAndComplex.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_Unmapped_StrictAndComplex.js)<br>[`Function_Call_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Basic.js)<br>[`Function_ObjectLiteralMethod_ThisBinding.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js)<br>[`Classes_ClassMethod_ReturnsThis_IsSelf_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassMethod_ReturnsThis_IsSelf_Log.js)<br>[`10.4.3-1-35-s.js`](../../../tests/Jroc.Test262.Tests/language/function-code/JavaScript/10.4.3-1-35-s.js) | `test/language/function-code/10.4.3-1-35-s.js` | Delegate-backed user functions support ordinary calls, receiver-sensitive method calls, lexical this for arrow functions, strict bare-call `this` preserving undefined, and lazy materialization of a dedicated arguments object when the implicit binding is referenced. Non-arrow functions use mapped arguments objects for non-strict simple parameter lists and unmapped arguments objects for strict-mode or complex parameter lists; remaining gaps are the narrower 10.4.4 descriptor/invariant edge cases rather than wholesale lack of arguments-object support. |

### 10.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-function-objects-construct-argumentslist-newtarget))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Constructor calls and new.target-aware function execution | Supported with Limitations | [`Function_NewTarget_NewVsCall.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NewTarget_NewVsCall.js)<br>[`Function_NewTarget_Arrow_Inherits.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NewTarget_Arrow_Inherits.js)<br>[`Classes_Constructor_ReturnObjectOverridesThis.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_Constructor_ReturnObjectOverridesThis.js)<br>[`Classes_DeclareEmptyClass.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js) |  | JROC constructs delegate-backed functions and class constructors by creating an instance, binding this, and forwarding a newTarget value into the call path. Constructor return override works, but super/inheritance semantics and full OrdinaryCreateFromConstructor behavior remain incomplete. |

### 10.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-addrestrictedfunctionproperties))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Restricted function caller/arguments properties | Supported with Limitations | [`prototype-rules.js`](../../../tests/Jroc.Test262.Tests/language/expressions/arrow-function/JavaScript/prototype-rules.js)<br>[`restricted-properties.js`](../../../tests/Jroc.Test262.Tests/language/expressions/class/JavaScript/restricted-properties.js) | suite `pr`<br>suite `nightly`<br>`test/language/expressions/arrow-function/prototype-rules.js`<br>`test/language/expressions/class/restricted-properties.js` | JROC installs throwing restricted caller/arguments accessors for the covered non-ordinary callable surfaces, including arrow functions and class constructors. Full %ThrowTypeError% identity/intrinsics coverage remains limited. |

### 10.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-makeconstructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function prototype objects and constructor-style metadata | Supported with Limitations | [`Function_Prototype_ObjectCreate_ObjectPrototype.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_ObjectCreate_ObjectPrototype.js)<br>[`Function_Prototype_Bind_PropertyExists.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_Bind_PropertyExists.js)<br>[`Function_Prototype_ToString_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_ToString_Basic.js) |  | Delegate-backed functions lazily synthesize prototype, constructor, name, length, and toString metadata so common library patterns continue to work. These properties are inferred from CLR delegates rather than being installed through the exact SetFunctionName / SetFunctionLength / MakeConstructor abstract-operation flow. |

### 10.2.9 ([tc39.es](https://tc39.es/ecma262/#sec-setfunctionname))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SetFunctionName for anonymous functions in destructuring defaults | Supported with Limitations | [`obj-ptrn-id-init-fn-name-gen.js`](../../../tests/Jroc.Test262.Tests/language/expressions/function/dstr/JavaScript/obj-ptrn-id-init-fn-name-gen.js)<br>[`gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js`](../../../tests/Jroc.Test262.Tests/language/expressions/object/dstr/JavaScript/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js) | `test/language/expressions/function/dstr/obj-ptrn-id-init-fn-name-gen.js`<br>`test/language/expressions/object/dstr/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js` | Anonymous function and generator expressions created by destructuring default initializers infer the target binding name in the covered object/array binding cases. Broader SetFunctionName coverage remains limited for less-common syntactic forms. |

### 10.2.11 ([tc39.es](https://tc39.es/ecma262/#sec-functiondeclarationinstantiation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function declaration instantiation and closures | Supported with Limitations | [`Function_ClosureMutatesOuterVariable.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ClosureMutatesOuterVariable.js)<br>[`Function_NestedFunctionAccessesMultipleScopes.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NestedFunctionAccessesMultipleScopes.js)<br>[`Function_ClosureEscapesScope_ObjectLiteralProperty.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ClosureEscapesScope_ObjectLiteralProperty.js) |  | Scope-as-class lowering gives functions stable lexical captures and nested-scope access. Non-arrow functions lazily install the implicit arguments binding, with mapped aliasing for simple non-strict parameter lists and unmapped semantics for strict-mode or complex parameter lists; remaining limitations are concentrated in stricter environment-record and exotic-object edge cases. |

