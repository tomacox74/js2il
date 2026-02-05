<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 20.2: Function Objects

[Back to Section20](Section20.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 20.2 | Function Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-function-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 20.2.1 | The Function Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-function-constructor) |
| 20.2.1.1 | Function ( ... parameterArgs , bodyArg ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-function-p1-p2-pn-body) |
| 20.2.1.1.1 | CreateDynamicFunction ( constructor , newTarget , kind , parameterArgs , bodyArg ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-createdynamicfunction) |
| 20.2.2 | Properties of the Function Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-function-constructor) |
| 20.2.2.1 | Function.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype) |
| 20.2.3 | Properties of the Function Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-function-prototype-object) |
| 20.2.3.1 | Function.prototype.apply ( thisArg , argArray ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.apply) |
| 20.2.3.2 | Function.prototype.bind ( thisArg , ... args ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.bind) |
| 20.2.3.3 | Function.prototype.call ( thisArg , ... args ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.call) |
| 20.2.3.4 | Function.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.constructor) |
| 20.2.3.5 | Function.prototype.toString ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.tostring) |
| 20.2.3.6 | Function.prototype [ %Symbol.hasInstance% ] ( V ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype-%symbol.hasinstance%) |
| 20.2.4 | Function Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-instances) |
| 20.2.4.1 | length | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-function-instances-length) |
| 20.2.4.2 | name | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-function-instances-name) |
| 20.2.4.3 | prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-instances-prototype) |
| 20.2.5 | HostHasSourceTextAvailable ( func ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-hosthassourcetextavailable) |

## Support

Feature-level support tracking with test script references.

### 20.2.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-function.prototype.apply))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Function.prototype.apply ( thisArg , argArray ) | Supported with Limitations | [`Function_Apply_Basic.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Apply_Basic.js)<br>[`Function_Apply_ThisArg.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Apply_ThisArg.js)<br>[`Function_Apply_NullArgArray_TreatedAsEmpty.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Apply_NullArgArray_TreatedAsEmpty.js) | Implemented for delegate-backed function values via the runtime member-call dispatcher. Supports null/undefined argArray as an empty argument list. Full CreateListFromArrayLike semantics and non-delegate callable objects are not implemented. |

### 20.2.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-function.prototype.bind))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Function.prototype.bind ( thisArg , ... args ) | Supported with Limitations | [`Function_Bind_Basic_PartialApplication.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Bind_Basic_PartialApplication.js)<br>[`Function_Bind_ThisBinding_IgnoresCallReceiver.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Bind_ThisBinding_IgnoresCallReceiver.js) | Implemented for delegate-backed function values via the runtime member-call dispatcher. Supports binding this and partial application of arguments. Bound functions are modeled as delegates and do not currently implement full spec metadata (length/name/prototype). |

### 20.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-function-instances))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Closures capture and mutate outer variables | Supported with Limitations | [`Function_Closure_MultiLevel_ReadWriteAcrossScopes.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Closure_MultiLevel_ReadWriteAcrossScopes.js) | Closures are implemented via the scope-as-class model (scope instances hold variables as fields). |
| Function instances are callable (basic invocation) | Supported with Limitations | [`Function_HelloWorld.js`](../../../Js2IL.Tests/Function/JavaScript/Function_HelloWorld.js)<br>[`Function_CallViaVariable_Reassignment.js`](../../../Js2IL.Tests/Function/JavaScript/Function_CallViaVariable_Reassignment.js) | JavaScript functions are compiled to CLR delegates and invoked directly (or via a small runtime dispatcher). Function.prototype.apply and Function.prototype.bind are supported for delegate-backed function values; most other Function.prototype APIs are not implemented. |
| Method calls set dynamic this; arrow functions capture lexical this | Supported with Limitations | [`Function_ObjectLiteralMethod_ThisBinding.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js)<br>[`ArrowFunction_LexicalThis_CreatedInMethod.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_LexicalThis_CreatedInMethod.js) | Normal functions support receiver-based this for member calls; arrow functions implement lexical this binding via runtime helpers. |

### 20.2.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-function-instances-prototype))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Function instance 'prototype' property is readable/writable and used by `new` | Supported with Limitations | [`Function_Prototype_ObjectCreate_ObjectPrototype.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Prototype_ObjectCreate_ObjectPrototype.js) | Delegate-backed function values behave like JavaScript objects and can carry ad-hoc properties via the runtime descriptor store (including 'prototype'). The runtime's dynamic constructor path (Object.ConstructValue) uses ctor.prototype to set the new instance's [[Prototype]] when it is object-like. Full Function exotic object semantics, metadata, and invariants are not implemented. |

