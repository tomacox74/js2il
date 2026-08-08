<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 20.2: Function Objects

[Back to Section20](Section20.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-08T00:01:43Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 20.2 | Function Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 20.2.1 | The Function Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-constructor) |
| 20.2.1.1 | Function ( ... parameterArgs , bodyArg ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-p1-p2-pn-body) |
| 20.2.1.1.1 | CreateDynamicFunction ( constructor , newTarget , kind , parameterArgs , bodyArg ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createdynamicfunction) |
| 20.2.2 | Properties of the Function Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-function-constructor) |
| 20.2.2.1 | Function.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype) |
| 20.2.3 | Properties of the Function Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-function-prototype-object) |
| 20.2.3.1 | Function.prototype.apply ( thisArg , argArray ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.apply) |
| 20.2.3.2 | Function.prototype.bind ( thisArg , ... args ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.bind) |
| 20.2.3.3 | Function.prototype.call ( thisArg , ... args ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.call) |
| 20.2.3.4 | Function.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.constructor) |
| 20.2.3.5 | Function.prototype.toString ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.tostring) |
| 20.2.3.6 | Function.prototype [ %Symbol.hasInstance% ] ( V ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype-%symbol.hasinstance%) |
| 20.2.4 | Function Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-instances) |
| 20.2.4.1 | length | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-instances-length) |
| 20.2.4.2 | name | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-instances-name) |
| 20.2.4.3 | prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-instances-prototype) |
| 20.2.5 | HostHasSourceTextAvailable ( func ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-hosthassourcetextavailable) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 20.2.1.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-createdynamicfunction))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function/new Function with compile-time string literal parameter/body source | Supported with Limitations | [`Function_Constructor_New_ConstantString_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Constructor_New_ConstantString_Basic.js)<br>[`Function_Constructor_Call_Length_Name.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Constructor_Call_Length_Name.js)<br>[`Function_Constructor_GlobalScope_NoClosure.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Constructor_GlobalScope_NoClosure.js)<br>[`Function_Constructor_NonLiteral_RuntimeError.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Constructor_NonLiteral_RuntimeError.js)<br>[`Function_Constructor_SyntaxError.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Constructor_SyntaxError.js) |  | Stage 1 support only: direct `Function(...)` and `new Function(...)` sites are compiled ahead-of-time when every parameter/body argument is a string literal. JROC parses the generated source during compilation, emits a synthetic callable with global-scope semantics (so module locals and enclosing locals are not captured), derives `.length` from the parsed parameter list, reports `.name` as `anonymous`, throws `SyntaxError` for invalid literal source, and throws `Error` for non-literal runtime forms. |

### 20.2.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-function.prototype.apply))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function.prototype.apply ( thisArg , argArray ) | Supported with Limitations | [`Function_Apply_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Apply_Basic.js)<br>[`Function_Apply_ThisArg.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Apply_ThisArg.js)<br>[`Function_Apply_NullArgArray_TreatedAsEmpty.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Apply_NullArgArray_TreatedAsEmpty.js) |  | Implemented through centralized callable operations for generated functions, bound functions, methods, arrows, classes, proxies, and transitional delegates. Supports null/undefined argArray as an empty argument list. Full CreateListFromArrayLike semantics for every array-like exotic value remain limited. |

### 20.2.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-function.prototype.bind))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function.prototype.bind ( thisArg , ... args ) | Supported with Limitations | [`Function_Bind_Basic_PartialApplication.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Bind_Basic_PartialApplication.js)<br>[`Function_Bind_Construct_NewTargetAndPrototype.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Bind_Construct_NewTargetAndPrototype.js)<br>[`Function_Bind_Metadata_LengthNameAndPrototype.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Bind_Metadata_LengthNameAndPrototype.js)<br>[`Function_Bind_ThisBinding_IgnoresCallReceiver.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Bind_ThisBinding_IgnoresCallReceiver.js)<br>[`Function_BoundFunctionObject_UnifiedTargets.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_BoundFunctionObject_UnifiedTargets.js)<br>[`Function_Prototype_Bind_PropertyExists.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_Bind_PropertyExists.js)<br>[`15.3.4.5-0-1.js`](../../../tests/Jroc.Test262.Tests/built-ins/Function/prototype/bind/JavaScript/15.3.4.5-0-1.js)<br>[`15.3.4.5-2-1.js`](../../../tests/Jroc.Test262.Tests/built-ins/Function/prototype/bind/JavaScript/15.3.4.5-2-1.js)<br>[`15.3.4.5-10-1.js`](../../../tests/Jroc.Test262.Tests/built-ins/Function/prototype/bind/JavaScript/15.3.4.5-10-1.js)<br>[`15.3.4.5-16-1.js`](../../../tests/Jroc.Test262.Tests/built-ins/Function/prototype/bind/JavaScript/15.3.4.5-16-1.js) | `test/built-ins/Function/prototype/bind/15.3.4.5-0-1.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-1.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-10-1.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-16-1.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-11-1.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-16-2.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-10.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-11.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-12.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-13.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-14.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-15.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-16.js` | Implemented as an explicit BoundFunctionObject for every callable target. Supports chained binding, bound this and argument concatenation, constructability classification, spec-aligned newTarget rewriting, target-based instanceof behavior, bound length/name metadata, and no own prototype property. Transitional delegate targets are adapted through centralized callable operations rather than represented by new bound delegates. |

### 20.2.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-function.prototype.call))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function.prototype.call ( thisArg , ... args ) | Supported with Limitations | [`Function_Call_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Basic.js)<br>[`15.3.4.4-1-s.js`](../../../tests/Jroc.Test262.Tests/built-ins/Function/prototype/call/JavaScript/15.3.4.4-1-s.js)<br>[`S15.3.4.4_A1_T1.js`](../../../tests/Jroc.Test262.Tests/built-ins/Function/prototype/call/JavaScript/S15.3.4.4_A1_T1.js)<br>[`S15.3.4.4_A10.js`](../../../tests/Jroc.Test262.Tests/built-ins/Function/prototype/call/JavaScript/S15.3.4.4_A10.js)<br>[`S15.3.4.4_A11.js`](../../../tests/Jroc.Test262.Tests/built-ins/Function/prototype/call/JavaScript/S15.3.4.4_A11.js)<br>[`S15.3.4.4_A3_T1.js`](../../../tests/Jroc.Test262.Tests/built-ins/Function/prototype/call/JavaScript/S15.3.4.4_A3_T1.js) | `test/built-ins/Function/prototype/call/15.3.4.4-1-s.js`<br>`test/built-ins/Function/prototype/call/S15.3.4.4_A1_T1.js`<br>`test/built-ins/Function/prototype/call/S15.3.4.4_A10.js`<br>`test/built-ins/Function/prototype/call/S15.3.4.4_A11.js`<br>`test/built-ins/Function/prototype/call/S15.3.4.4_A3_T1.js`<br>`test/built-ins/Function/prototype/call/15.3.4.4-2-s.js`<br>`test/built-ins/Function/prototype/call/15.3.4.4-3-s.js`<br>`test/built-ins/Function/prototype/call/S15.3.4.4_A1_T2.js`<br>`test/built-ins/Function/prototype/call/S15.3.4.4_A15.js`<br>`test/built-ins/Function/prototype/call/S15.3.4.4_A16.js` | Implemented through centralized callable operations for all callable representations. Covered behavior includes direct argument forwarding, primitive/ordinary thisArg cases, null/undefined global-this substitution, generated and bound function objects, and transitional delegates. |

### 20.2.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-function.prototype.constructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function.prototype.constructor references the Function constructor | Supported with Limitations | [`Function_Prototype_Constructor_ReferencesFunction.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_Constructor_ReferencesFunction.js) |  | Function.prototype exposes a data property named constructor that references the runtime Function constructor value. Direct compile-time-literal `Function(...)` / `new Function(...)` forms are supported; non-literal runtime forms throw a documented `Error`. |

### 20.2.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-function.prototype.tostring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function.prototype.toString ( ) returns a function-like source string | Supported with Limitations | [`Function_Prototype_ToString_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_ToString_Basic.js) |  | Implemented through the common callable path with a native-source style string for generated, bound, method, class, proxy-backed, and transitional delegate callables. Full source text reconstruction and HostHasSourceTextAvailable semantics are not implemented. |

### 20.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-function-instances))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Closures capture and mutate outer variables | Supported with Limitations | [`Function_Closure_MultiLevel_ReadWriteAcrossScopes.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Closure_MultiLevel_ReadWriteAcrossScopes.js) |  | Closures are implemented via the scope-as-class model (scope instances hold variables as fields). |
| Function instances are callable (basic invocation) | Supported with Limitations | [`Function_HelloWorld.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_HelloWorld.js)<br>[`Function_CallViaVariable_Reassignment.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_CallViaVariable_Reassignment.js)<br>[`Function_GeneratedConstruction_Semantics.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_GeneratedConstruction_Semantics.js) |  | Synchronous ordinary functions are materialized as generated JsFunctionObject instances with typed canonical entry points plus centralized dynamic call and construct adapters. Function.prototype.apply, call, bind, and toString route every callable family through centralized operations; bind always produces an explicit bound function object. |
| intrinsic async/generator constructor objects as callable/extensible function values | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/AsyncFunction/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/AsyncGeneratorFunction/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/GeneratorFunction/ExecutionTests.cs` |  | The intrinsic constructor objects associated with async functions, generator functions, and async generator functions are exposed as first-class function values with the covered test262 length/extensibility/prototype surface. Full dynamic construction, subclassing, and the remaining advanced constructor scenarios are still tracked separately. |
| Method calls set dynamic this; arrow functions capture lexical this | Supported with Limitations | [`Function_ObjectLiteralMethod_ThisBinding.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js)<br>[`ArrowFunction_LexicalThis_CreatedInMethod.js`](../../../tests/Jroc.Tests/ArrowFunction/JavaScript/ArrowFunction_LexicalThis_CreatedInMethod.js) |  | Normal functions support receiver-based this for member calls; arrow functions implement lexical this binding via runtime helpers. |

### 20.2.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-function-instances-length))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function instance length property | Supported with Limitations | [`Function_Bind_Construct_NewTargetAndPrototype.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Bind_Construct_NewTargetAndPrototype.js)<br>[`Function_Bind_Metadata_LengthNameAndPrototype.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Bind_Metadata_LengthNameAndPrototype.js)<br>[`Function_Instance_Length_Name_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Instance_Length_Name_Basic.js)<br>[`Function_Instance_Length_Name_DescriptorOwnProperties.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Instance_Length_Name_DescriptorOwnProperties.js)<br>[`Function_Constructor_Call_Length_Name.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Constructor_Call_Length_Name.js) |  | Delegate-backed functions expose a numeric length property derived from ABI-aware callable parameter counting, and lazily materialize it as a non-enumerable own data descriptor (`writable: false`, `configurable: true`) so direct reads, `Object.getOwnPropertyDescriptor(...)`, and `Object.hasOwn(...)` agree. Bound functions compute `.length` as `max(target.length - boundArgsCount, 0)`. Function-constructor-created callables use the parsed parameter list to set `.length`. Exact ECMA-262 length metadata rules are not fully implemented. |

### 20.2.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-function-instances-name))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function instance name property | Supported with Limitations | [`Function_Bind_Construct_NewTargetAndPrototype.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Bind_Construct_NewTargetAndPrototype.js)<br>[`Function_Bind_Metadata_LengthNameAndPrototype.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Bind_Metadata_LengthNameAndPrototype.js)<br>[`Function_Instance_Length_Name_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Instance_Length_Name_Basic.js)<br>[`Function_Instance_Length_Name_DescriptorOwnProperties.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Instance_Length_Name_DescriptorOwnProperties.js)<br>[`Function_Constructor_New_ConstantString_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Constructor_New_ConstantString_Basic.js) |  | Delegate-backed functions expose a string name property based on the underlying CLR method name, and lazily materialize it as a non-enumerable own data descriptor (`writable: false`, `configurable: true`) so direct reads, `Object.getOwnPropertyDescriptor(...)`, and `Object.hasOwn(...)` agree. Bound functions prefix that name with `bound ` for each bind layer. Function-constructor-created callables default to `anonymous`. Exact SetFunctionName behavior is not implemented. |

### 20.2.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-function-instances-prototype))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function instance 'prototype' property is readable/writable and used by `new` | Supported with Limitations | [`Function_Prototype_ObjectCreate_ObjectPrototype.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_ObjectCreate_ObjectPrototype.js)<br>[`Function_GeneratedConstruction_Semantics.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_GeneratedConstruction_Semantics.js) |  | Generated ordinary function objects expose an own writable, non-enumerable, non-configurable prototype property. Construction reads the current property value, uses an object value as the receiver prototype, and falls back to Object.prototype for primitive values; each prototype object has the expected constructor descriptor. Full realm-sensitive Function exotic invariants remain limited. |

