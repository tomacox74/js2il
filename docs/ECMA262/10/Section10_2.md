<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.2: ECMAScript Function Objects

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-09T18:37:51Z

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
| Function calls, this binding, and lazy arguments object materialization | Supported with Limitations | [`Function_Arguments_Basics.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_Basics.js)<br>[`Function_Arguments_CalleeIdentity.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_CalleeIdentity.js)<br>[`Function_Arguments_MappedParameterAliasing.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_MappedParameterAliasing.js)<br>[`Function_Arguments_Unmapped_StrictAndComplex.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_Unmapped_StrictAndComplex.js)<br>[`Function_Call_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Basic.js)<br>[`Function_ObjectLiteralMethod_ThisBinding.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js)<br>[`Classes_ClassMethod_ReturnsThis_IsSelf_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassMethod_ReturnsThis_IsSelf_Log.js)<br>[`10.4.3-1-35-s.js`](../../../tests/Jroc.Test262.Tests/language/function-code/JavaScript/10.4.3-1-35-s.js) | `test/language/function-code/10.4.3-1-35-s.js` | Generated JsFunctionObject subclasses support ordinary calls, receiver-sensitive method calls, lexical this for arrow functions, strict bare-call `this` preserving undefined, and lazy materialization of a dedicated arguments object when the implicit binding is referenced. Typed direct array adapters receive the actual function object, preserving non-strict arguments.callee identity and nested invocation state without exposing a CLR delegate. Non-arrow functions use mapped arguments objects for non-strict simple parameter lists and unmapped arguments objects for strict-mode or complex parameter lists; remaining gaps are the narrower 10.4.4 descriptor/invariant edge cases rather than wholesale lack of arguments-object support. |

### 10.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-function-objects-construct-argumentslist-newtarget))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Constructor calls and new.target-aware function execution | Supported with Limitations | [`Function_NewTarget_NewVsCall.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NewTarget_NewVsCall.js)<br>[`Function_NewTarget_Arrow_Inherits.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NewTarget_Arrow_Inherits.js)<br>[`Function_GeneratedConstruction_Semantics.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_GeneratedConstruction_Semantics.js)<br>[`Classes_Constructor_ReturnObjectOverridesThis.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_Constructor_ReturnObjectOverridesThis.js)<br>[`Classes_DeclareEmptyClass.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js) |  | Generated ordinary function objects implement a separate construction-body adapter rather than routing construction through ordinary [[Call]]. The runtime allocates from newTarget.prototype with Object.prototype fallback, propagates new.target, honors object-versus-primitive return overrides, forwards bound construction, and supports function-valued bases of derived classes. Less-common exotic and realm-sensitive OrdinaryCreateFromConstructor cases remain limited. |

### 10.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-addrestrictedfunctionproperties))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Restricted function caller/arguments properties | Supported with Limitations | [`prototype-rules.js`](../../../tests/Jroc.Test262.Tests/language/expressions/arrow-function/JavaScript/prototype-rules.js)<br>[`restricted-properties.js`](../../../tests/Jroc.Test262.Tests/language/expressions/class/JavaScript/restricted-properties.js) | suite `pr`<br>suite `nightly`<br>`test/language/expressions/arrow-function/prototype-rules.js`<br>`test/language/expressions/class/restricted-properties.js` | JROC installs throwing restricted caller/arguments accessors for the covered non-ordinary callable surfaces, including arrow functions and class constructors. Full %ThrowTypeError% identity/intrinsics coverage remains limited. |

### 10.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-makeconstructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function prototype objects and constructor-style metadata | Supported with Limitations | [`Function_Prototype_ObjectCreate_ObjectPrototype.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_ObjectCreate_ObjectPrototype.js)<br>[`Function_Prototype_Bind_PropertyExists.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_Bind_PropertyExists.js)<br>[`Function_GeneratedConstruction_Semantics.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_GeneratedConstruction_Semantics.js)<br>[`Function_Prototype_ToString_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_ToString_Basic.js) |  | Generated ordinary function objects receive writable, non-enumerable, non-configurable prototype properties whose prototype objects carry the expected writable, non-enumerable, configurable constructor link. Prototype replacement affects subsequent construction. Runtime-owned built-in/host delegate adapters retain explicit lazy metadata, but compiled JavaScript function values do not use delegates. Exact realm-sensitive MakeConstructor behavior remains limited. |

### 10.2.7 ([tc39.es](https://tc39.es/ecma262/#sec-makemethod))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Generated method function objects with home-object state | Supported with Limitations | [`Classes_GeneratedMethodFunctionObjects.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_GeneratedMethodFunctionObjects.js)<br>[`ObjectLiteral_GeneratedMethodFunctionObjects.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_GeneratedMethodFunctionObjects.js) |  | Class and object-literal methods/accessors, including async and generator families, materialize as generated JsFunctionObject instances. Generated objects capture home-object and lexical super state only when required, preserve dynamic receiver behavior and per-evaluation identity, and remain non-constructable without an ordinary-function prototype property. |

### 10.2.8 ([tc39.es](https://tc39.es/ecma262/#sec-definemethodproperty))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Generated method and accessor descriptor installation | Supported with Limitations | [`Classes_GeneratedMethodFunctionObjects.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_GeneratedMethodFunctionObjects.js)<br>[`ObjectLiteral_GeneratedMethodFunctionObjects.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_GeneratedMethodFunctionObjects.js) |  | Class prototype/static properties and object-literal data/accessor descriptors install the same generated method objects returned by descriptor reflection. Covered computed names, getter/setter names, private-brand checks, and repeated class/object evaluations preserve observable identity. |

### 10.2.9 ([tc39.es](https://tc39.es/ecma262/#sec-setfunctionname))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SetFunctionName for anonymous functions in destructuring defaults | Supported with Limitations | [`obj-ptrn-id-init-fn-name-gen.js`](../../../tests/Jroc.Test262.Tests/language/expressions/function/dstr/JavaScript/obj-ptrn-id-init-fn-name-gen.js)<br>[`gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js`](../../../tests/Jroc.Test262.Tests/language/expressions/object/dstr/JavaScript/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js) | `test/language/expressions/function/dstr/obj-ptrn-id-init-fn-name-gen.js`<br>`test/language/expressions/object/dstr/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js` | Anonymous function and generator expressions created by destructuring default initializers infer the target binding name in the covered object/array binding cases. Broader SetFunctionName coverage remains limited for less-common syntactic forms. |

### 10.2.11 ([tc39.es](https://tc39.es/ecma262/#sec-functiondeclarationinstantiation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function declaration instantiation and closures | Supported with Limitations | [`Function_ClosureMutatesOuterVariable.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ClosureMutatesOuterVariable.js)<br>[`Function_NestedFunctionAccessesMultipleScopes.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NestedFunctionAccessesMultipleScopes.js)<br>[`Function_ClosureEscapesScope_ObjectLiteralProperty.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ClosureEscapesScope_ObjectLiteralProperty.js) |  | Scope-as-class lowering gives functions stable lexical captures and nested-scope access. Non-arrow functions lazily install the implicit arguments binding, with mapped aliasing for simple non-strict parameter lists and unmapped semantics for strict-mode or complex parameter lists; remaining limitations are concentrated in stricter environment-record and exotic-object edge cases. |

