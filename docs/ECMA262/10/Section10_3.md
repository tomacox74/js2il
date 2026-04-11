<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.3: Built-in Function Objects

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-08T21:51:25Z

JS2IL exposes many ECMAScript built-ins as CLR-backed delegates or intrinsic runtime constructors. That gives everyday built-in call/construct behavior for the implemented subset, but built-in function objects are not created through the full ECMA-262 realm/prototype/length/name machinery.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 10.3 | Built-in Function Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-built-in-function-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 10.3.1 | [[Call]] ( thisArgument , argumentsList ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-built-in-function-objects-call-thisargument-argumentslist) |
| 10.3.2 | [[Construct]] ( argumentsList , newTarget ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-built-in-function-objects-construct-argumentslist-newtarget) |
| 10.3.3 | BuiltinCallOrConstruct ( F , thisArgument , argumentsList , newTarget ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-builtincallorconstruct) |
| 10.3.4 | CreateBuiltinFunction ( behaviour , length , name , additionalInternalSlotsList [ , realm [ , prototype [ , prefix [ , async ] ] ] ] ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-createbuiltinfunction) |

## Support

Feature-level support tracking with test script references.

### 10.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-built-in-function-objects-call-thisargument-argumentslist))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Built-in callable dispatch for Array/Object/Function helpers | Supported with Limitations | [`Array_PrototypeMethods_ArrayLike_Call.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_PrototypeMethods_ArrayLike_Call.js)<br>[`Object_Keys_Basic.js`](../../../tests/Js2IL.Tests/Object/JavaScript/Object_Keys_Basic.js)<br>[`Function_Apply_Basic.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Apply_Basic.js) | Implemented built-ins are surfaced as delegates and CLR instance/static methods, then invoked through the same Closure dispatch machinery as user functions. Realm-specific callable creation, full Function.prototype inheritance, and exhaustive built-in coverage are still incomplete. |

### 10.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-built-in-function-objects-construct-argumentslist-newtarget))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Constructible built-ins such as Array, Object, Int32Array, and Proxy | Supported with Limitations | [`Array_Callable_Construct.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_Callable_Construct.js)<br>[`Array_New_Length.js`](../../../tests/Js2IL.Tests/Array/JavaScript/Array_New_Length.js)<br>[`Int32Array_Construct_Length.js`](../../../tests/Js2IL.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js)<br>[`Proxy_GetTrap_OverridesProperty.js`](../../../tests/Js2IL.Tests/Proxy/JavaScript/Proxy_GetTrap_OverridesProperty.js) | The implemented subset of built-ins can usually be called/constructed directly through dedicated intrinsic lowering or runtime helpers. First-class constructor values are still uneven; for example, the global `Function` constructor only supports direct compile-time string literal call/new forms and throws for non-literal runtime usage. |

### 10.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-createbuiltinfunction))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Intrinsic built-in registration and prototype wiring | Incomplete | [`Function_Prototype_Bind_PropertyExists.js`](../../../tests/Js2IL.Tests/Function/JavaScript/Function_Prototype_Bind_PropertyExists.js) | Built-ins are discovered through IntrinsicObject attributes and selectively wired onto GlobalThis/prototype objects, which is sufficient for many library patterns. The engine does not yet implement CreateBuiltinFunction in the full spec sense with realms, exact length/name attributes, and uniform constructor/call behavior across every intrinsic. |

