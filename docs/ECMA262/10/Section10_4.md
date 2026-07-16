<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.4: Built-in Exotic Object Internal Methods and Slots

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-16T19:51:29Z

JROC implements spec-defined exotic-object behavior for the features it currently supports. Arrays provide dedicated index and length internal operations over dense and sparse storage; bound functions, typed arrays, and namespace imports retain the limitations documented below.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 10.4 | Built-in Exotic Object Internal Methods and Slots | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-built-in-exotic-object-internal-methods-and-slots) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 10.4.1 | Bound Function Exotic Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bound-function-exotic-objects) |
| 10.4.1.1 | [[Call]] ( thisArgument , argumentsList ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bound-function-exotic-objects-call-thisargument-argumentslist) |
| 10.4.1.2 | [[Construct]] ( argumentsList , newTarget ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-bound-function-exotic-objects-construct-argumentslist-newtarget) |
| 10.4.1.3 | BoundFunctionCreate ( targetFunction , boundThis , boundArgs ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-boundfunctioncreate) |
| 10.4.2 | Array Exotic Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-array-exotic-objects) |
| 10.4.2.1 | [[DefineOwnProperty]] ( P , Desc ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-array-exotic-objects-defineownproperty-p-desc) |
| 10.4.2.2 | ArrayCreate ( length [ , proto ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arraycreate) |
| 10.4.2.3 | ArraySpeciesCreate ( originalArray , length ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-arrayspeciescreate) |
| 10.4.2.4 | ArraySetLength ( A , Desc ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-arraysetlength) |
| 10.4.3 | String Exotic Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-string-exotic-objects) |
| 10.4.3.1 | [[GetOwnProperty]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string-exotic-objects-getownproperty-p) |
| 10.4.3.2 | [[DefineOwnProperty]] ( P , Desc ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-string-exotic-objects-defineownproperty-p-desc) |
| 10.4.3.3 | [[OwnPropertyKeys]] ( ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-string-exotic-objects-ownpropertykeys) |
| 10.4.3.4 | StringCreate ( value , prototype ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-stringcreate) |
| 10.4.3.5 | StringGetOwnProperty ( S , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-stringgetownproperty) |
| 10.4.4 | Arguments Exotic Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects) |
| 10.4.4.1 | [[GetOwnProperty]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects-getownproperty-p) |
| 10.4.4.2 | [[DefineOwnProperty]] ( P , Desc ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects-defineownproperty-p-desc) |
| 10.4.4.3 | [[Get]] ( P , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects-get-p-receiver) |
| 10.4.4.4 | [[Set]] ( P , V , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects-set-p-v-receiver) |
| 10.4.4.5 | [[Delete]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects-delete-p) |
| 10.4.4.6 | CreateUnmappedArgumentsObject ( argumentsList ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createunmappedargumentsobject) |
| 10.4.4.7 | CreateMappedArgumentsObject ( func , formals , argumentsList , env ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createmappedargumentsobject) |
| 10.4.4.7.1 | MakeArgGetter ( name , env ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makearggetter) |
| 10.4.4.7.2 | MakeArgSetter ( name , env ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makeargsetter) |
| 10.4.5 | TypedArray Exotic Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-exotic-objects) |
| 10.4.5.1 | [[PreventExtensions]] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-preventextensions) |
| 10.4.5.2 | [[GetOwnProperty]] ( P ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-getownproperty) |
| 10.4.5.3 | [[HasProperty]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-hasproperty) |
| 10.4.5.4 | [[DefineOwnProperty]] ( P , Desc ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-defineownproperty) |
| 10.4.5.5 | [[Get]] ( P , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-get) |
| 10.4.5.6 | [[Set]] ( P , V , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-set) |
| 10.4.5.7 | [[Delete]] ( P ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-delete) |
| 10.4.5.8 | [[OwnPropertyKeys]] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-ownpropertykeys) |
| 10.4.5.9 | TypedArray With Buffer Witness Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-with-buffer-witness-records) |
| 10.4.5.10 | MakeTypedArrayWithBufferWitnessRecord ( obj , order ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-maketypedarraywithbufferwitnessrecord) |
| 10.4.5.11 | TypedArrayCreate ( prototype ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-typedarraycreate) |
| 10.4.5.12 | TypedArrayByteLength ( taRecord ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-typedarraybytelength) |
| 10.4.5.13 | TypedArrayLength ( taRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-typedarraylength) |
| 10.4.5.14 | IsTypedArrayOutOfBounds ( taRecord ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-istypedarrayoutofbounds) |
| 10.4.5.15 | IsTypedArrayFixedLength ( O ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-istypedarrayfixedlength) |
| 10.4.5.16 | IsValidIntegerIndex ( O , index ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isvalidintegerindex) |
| 10.4.5.17 | TypedArrayGetElement ( O , index ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-typedarraygetelement) |
| 10.4.5.18 | TypedArraySetElement ( O , index , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-typedarraysetelement) |
| 10.4.5.19 | IsArrayBufferViewOutOfBounds ( O ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-isarraybufferviewoutofbounds) |
| 10.4.6 | Module Namespace Exotic Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects) |
| 10.4.6.1 | [[GetPrototypeOf]] ( ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-getprototypeof) |
| 10.4.6.2 | [[SetPrototypeOf]] ( V ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-setprototypeof-v) |
| 10.4.6.3 | [[IsExtensible]] ( ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-isextensible) |
| 10.4.6.4 | [[PreventExtensions]] ( ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-preventextensions) |
| 10.4.6.5 | [[GetOwnProperty]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-getownproperty-p) |
| 10.4.6.6 | [[DefineOwnProperty]] ( P , Desc ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-defineownproperty-p-desc) |
| 10.4.6.7 | [[HasProperty]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-hasproperty-p) |
| 10.4.6.8 | [[Get]] ( P , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-get-p-receiver) |
| 10.4.6.9 | [[Set]] ( P , V , Receiver ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-set-p-v-receiver) |
| 10.4.6.10 | [[Delete]] ( P ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-delete-p) |
| 10.4.6.11 | [[OwnPropertyKeys]] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-ownpropertykeys) |
| 10.4.6.12 | ModuleNamespaceCreate ( module , exports ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-modulenamespacecreate) |
| 10.4.7 | Immutable Prototype Exotic Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-immutable-prototype-exotic-objects) |
| 10.4.7.1 | [[SetPrototypeOf]] ( V ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-immutable-prototype-exotic-objects-setprototypeof-v) |
| 10.4.7.2 | SetImmutablePrototype ( O , V ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-set-immutable-prototype) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 10.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-bound-function-exotic-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Bound functions created by Function.prototype.bind | Supported with Limitations | [`Function_Bind_Basic_PartialApplication.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Bind_Basic_PartialApplication.js)<br>[`Function_Bind_ThisBinding_IgnoresCallReceiver.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Bind_ThisBinding_IgnoresCallReceiver.js)<br>[`Function_Prototype_Bind_PropertyExists.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_Bind_PropertyExists.js)<br>[`10.4.3-1-97-s.js`](../../../tests/Jroc.Test262.Tests/language/function-code/JavaScript/10.4.3-1-97-s.js) | `test/language/function-code/10.4.3-1-97-s.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-0-1.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-1.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-10-1.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-16-1.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-11-1.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-16-2.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-10.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-11.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-12.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-13.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-14.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-15.js`<br>`test/built-ins/Function/prototype/bind/15.3.4.5-2-16.js` | bind creates wrapper delegates with captured this/arguments and remembers the original target for metadata lookups. Bound calls normalize null/undefined thisArg according to the target function's this mode, so non-strict bound functions observe the global object while strict functions preserve the provided value. Current bounded test262 coverage also exercises bound length/name metadata, constructor forwarding, and ordinary-object/boxed thisArg cases. True [[BoundTargetFunction]] / [[BoundArguments]] slot fidelity and broader exotic-function invariants remain incomplete. |

### 10.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-array-exotic-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Array exotic indexing and length behavior | Supported | [`Array_New_Length.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_New_Length.js)<br>[`Array_New_MultipleArgs.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_New_MultipleArgs.js)<br>[`Array_Length_Set_Fractional_ThrowsRangeError.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_Length_Set_Fractional_ThrowsRangeError.js)<br>[`Array_Exotic_Property_Descriptors.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_Exotic_Property_Descriptors.js)<br>[`Array_Exotic_Sloppy_Assignment.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_Exotic_Sloppy_Assignment.js)<br>[`define-own-prop-length-overflow-order.js`](../../../tests/Jroc.Test262.Tests/built-ins/Array/length/JavaScript/define-own-prop-length-overflow-order.js)<br>[`define-own-prop-length-no-value-order.js`](../../../tests/Jroc.Test262.Tests/built-ins/Array/length/JavaScript/define-own-prop-length-no-value-order.js) | `test/built-ins/Array/length/define-own-prop-length-overflow-order.js`<br>`test/built-ins/Array/length/define-own-prop-length-no-value-order.js` | Canonical array indices use dedicated dense storage for ordinary elements and descriptor-backed sparse storage for exceptional indices. Indexed data/accessor descriptors, holes, deletion, non-writable length, failed highest-to-lowest truncation, strict/sloppy writes, maximum index boundaries, and preventExtensions/seal/freeze invariants are enforced through the Array [[DefineOwnProperty]] and ArraySetLength paths. ArraySpeciesCreate remains separately unsupported. |

### 10.4.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-stringgetownproperty))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String indexed property lookup | Supported with Limitations | [`15.5.5.5.2-3-6.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/JavaScript/15.5.5.5.2-3-6.js) | `test/built-ins/String/15.5.5.5.2-3-6.js` | String primitive indexed access returns code-unit substrings for canonical integer indexes and falls back to ordinary property lookup for non-canonical numeric keys such as NaN. Full String exotic object creation, property descriptors, and own-key ordering remain incomplete. |

### 10.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Arguments objects materialize lazily with mapped/unmapped aliasing semantics | Supported with Limitations | [`Function_Arguments_Basics.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_Basics.js)<br>[`Function_Arguments_ComputedKey_TriggersBinding.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_ComputedKey_TriggersBinding.js)<br>[`Function_Arguments_Length_Delete.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_Length_Delete.js)<br>[`Function_Arguments_MappedParameterAliasing.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_MappedParameterAliasing.js)<br>[`Function_Arguments_Unmapped_StrictAndComplex.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Arguments_Unmapped_StrictAndComplex.js)<br>[`10.6-12-2.js`](../../../tests/Jroc.Test262.Tests/language/arguments-object/JavaScript/10.6-12-2.js)<br>[`10.6-13-c-2-s.js`](../../../tests/Jroc.Test262.Tests/language/arguments-object/JavaScript/10.6-13-c-2-s.js)<br>[`10.6-13-c-3-s.js`](../../../tests/Jroc.Test262.Tests/language/arguments-object/JavaScript/10.6-13-c-3-s.js)<br>[`10.6-13-a-2.js`](../../../tests/Jroc.Test262.Tests/language/arguments-object/JavaScript/10.6-13-a-2.js)<br>[`10.6-13-a-3.js`](../../../tests/Jroc.Test262.Tests/language/arguments-object/JavaScript/10.6-13-a-3.js)<br>[`ArgumentsObject_callee-descriptor-non-strict.js`](../../../tests/Jroc.Test262.Tests/language/arguments-object/JavaScript/ArgumentsObject_callee-descriptor-non-strict.js)<br>[`ArgumentsObject_global-TypeError.js`](../../../tests/Jroc.Test262.Tests/language/arguments-object/JavaScript/ArgumentsObject_global-TypeError.js) | suite `pr`<br>suite `nightly`<br>`test/language/arguments-object/10.5-1gs.js`<br>`test/language/arguments-object/10.5-7-b-2-s.js`<br>`test/language/arguments-object/10.5-7-b-3-s.js`<br>`test/language/arguments-object/10.5-7-b-4-s.js`<br>`test/language/arguments-object/10.6-10-c-ii-1-s.js`<br>`test/language/arguments-object/10.6-10-c-ii-1.js`<br>`test/language/arguments-object/10.6-10-c-ii-2.js`<br>`test/language/arguments-object/10.6-12-1.js`<br>`test/language/statements/for-of/arguments-unmapped-aliasing.js`<br>`test/language/arguments-object/10.6-12-2.js`<br>`test/language/arguments-object/10.6-13-a-2.js`<br>`test/language/arguments-object/10.6-13-a-3.js`<br>`test/language/arguments-object/10.6-13-c-2-s.js`<br>`test/language/arguments-object/10.6-13-c-3-s.js`<br>`test/language/arguments-object/ArgumentsObject_callee-descriptor-non-strict.js`<br>`test/language/arguments-object/ArgumentsObject_global-TypeError.js` | When referenced, non-arrow functions lazily materialize a dedicated ArgumentsObject. Non-strict functions with simple parameter lists use mapped aliasing against parameter storage, while strict-mode and complex-parameter functions use unmapped semantics; covered cases include indexed reads/writes, length property access and deletion, own-key enumeration, for..of value iteration, non-strict callee data descriptors, and strict restricted callee accessors. Remaining gaps are mainly full [[DefineOwnProperty]] invariants, exhaustive legacy caller/callee edge cases, and exact abstract-operation factoring for 10.4.4.7 rather than absence of mapped arguments support. |

### 10.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-typedarray-exotic-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Int32Array construction, element access, and integer-index semantics | Supported with Limitations | [`Int32Array_Construct_Length.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js)<br>[`Int32Array_Index_Assign.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_Index_Assign.js)<br>[`Int32Array_FromArray_CopyAndCoerce.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_FromArray_CopyAndCoerce.js)<br>[`Int32Array_NaN_Index_NoOp.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_NaN_Index_NoOp.js) |  | JROC currently implements only Int32Array, with construction from length/iterables, integer index reads and writes, and pragmatic out-of-bounds handling. It does not model ArrayBuffer-backed typed-array witness records, detach/out-of-bounds abstract operations, or the full family of typed array exotics. |

### 10.4.6 ([tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Namespace import interop objects with live getter properties | Supported with Limitations | [`Import_Namespace_Esm_Basic.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_Namespace_Esm_Basic.js)<br>[`Import_Namespace_FromCjs_Stable.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_Namespace_FromCjs_Stable.js) |  | JSImport namespace lowering rewrites imports to a helper that builds a plain object with accessor descriptors for live reads of exported members. That captures common namespace-import behavior, but the result is not a true Module Namespace Exotic Object with the full non-extensible, non-configurable invariant set from the spec. |

### 10.4.7 ([tc39.es](https://tc39.es/ecma262/#sec-immutable-prototype-exotic-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Immutable prototype exotic invariants | Not Yet Supported |  |  | JROC does not currently model any object with a dedicated immutable-prototype exotic internal method. Prototype mutation is handled through the ordinary PrototypeChain side-table instead. |

