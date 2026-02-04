<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.4: Built-in Exotic Object Internal Methods and Slots

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

This section covers spec-defined *exotic objects* and their internal methods/slots (Bound Function, Array, String, Arguments, TypedArray, etc.). JS2IL currently does not attempt to model these internal-method details as specified. For the intrinsic function-scope `arguments` binding, JS2IL implements a minimal behavior tracked under Section 10.2 (non-arrow functions materialize `arguments` lazily as a JS Array snapshot; arrow functions capture `arguments` lexically). The full spec **Arguments Exotic Object** behavior (including mapped-arguments aliasing) remains not implemented.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 10.4 | Built-in Exotic Object Internal Methods and Slots | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-built-in-exotic-object-internal-methods-and-slots) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 10.4.1 | Bound Function Exotic Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bound-function-exotic-objects) |
| 10.4.1.1 | [[Call]] ( thisArgument , argumentsList ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bound-function-exotic-objects-call-thisargument-argumentslist) |
| 10.4.1.2 | [[Construct]] ( argumentsList , newTarget ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bound-function-exotic-objects-construct-argumentslist-newtarget) |
| 10.4.1.3 | BoundFunctionCreate ( targetFunction , boundThis , boundArgs ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-boundfunctioncreate) |
| 10.4.2 | Array Exotic Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array-exotic-objects) |
| 10.4.2.1 | [[DefineOwnProperty]] ( P , Desc ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array-exotic-objects-defineownproperty-p-desc) |
| 10.4.2.2 | ArrayCreate ( length [ , proto ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-arraycreate) |
| 10.4.2.3 | ArraySpeciesCreate ( originalArray , length ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-arrayspeciescreate) |
| 10.4.2.4 | ArraySetLength ( A , Desc ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-arraysetlength) |
| 10.4.3 | String Exotic Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string-exotic-objects) |
| 10.4.3.1 | [[GetOwnProperty]] ( P ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string-exotic-objects-getownproperty-p) |
| 10.4.3.2 | [[DefineOwnProperty]] ( P , Desc ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string-exotic-objects-defineownproperty-p-desc) |
| 10.4.3.3 | [[OwnPropertyKeys]] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string-exotic-objects-ownpropertykeys) |
| 10.4.3.4 | StringCreate ( value , prototype ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-stringcreate) |
| 10.4.3.5 | StringGetOwnProperty ( S , P ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-stringgetownproperty) |
| 10.4.4 | Arguments Exotic Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects) |
| 10.4.4.1 | [[GetOwnProperty]] ( P ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects-getownproperty-p) |
| 10.4.4.2 | [[DefineOwnProperty]] ( P , Desc ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects-defineownproperty-p-desc) |
| 10.4.4.3 | [[Get]] ( P , Receiver ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects-get-p-receiver) |
| 10.4.4.4 | [[Set]] ( P , V , Receiver ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects-set-p-v-receiver) |
| 10.4.4.5 | [[Delete]] ( P ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects-delete-p) |
| 10.4.4.6 | CreateUnmappedArgumentsObject ( argumentsList ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-createunmappedargumentsobject) |
| 10.4.4.7 | CreateMappedArgumentsObject ( func , formals , argumentsList , env ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-createmappedargumentsobject) |
| 10.4.4.7.1 | MakeArgGetter ( name , env ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-makearggetter) |
| 10.4.4.7.2 | MakeArgSetter ( name , env ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-makeargsetter) |
| 10.4.5 | TypedArray Exotic Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-exotic-objects) |
| 10.4.5.1 | [[PreventExtensions]] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-preventextensions) |
| 10.4.5.2 | [[GetOwnProperty]] ( P ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-getownproperty) |
| 10.4.5.3 | [[HasProperty]] ( P ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-hasproperty) |
| 10.4.5.4 | [[DefineOwnProperty]] ( P , Desc ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-defineownproperty) |
| 10.4.5.5 | [[Get]] ( P , Receiver ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-get) |
| 10.4.5.6 | [[Set]] ( P , V , Receiver ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-set) |
| 10.4.5.7 | [[Delete]] ( P ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-delete) |
| 10.4.5.8 | [[OwnPropertyKeys]] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-ownpropertykeys) |
| 10.4.5.9 | TypedArray With Buffer Witness Records | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarray-with-buffer-witness-records) |
| 10.4.5.10 | MakeTypedArrayWithBufferWitnessRecord ( obj , order ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-maketypedarraywithbufferwitnessrecord) |
| 10.4.5.11 | TypedArrayCreate ( prototype ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarraycreate) |
| 10.4.5.12 | TypedArrayByteLength ( taRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarraybytelength) |
| 10.4.5.13 | TypedArrayLength ( taRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarraylength) |
| 10.4.5.14 | IsTypedArrayOutOfBounds ( taRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-istypedarrayoutofbounds) |
| 10.4.5.15 | IsTypedArrayFixedLength ( O ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-istypedarrayfixedlength) |
| 10.4.5.16 | IsValidIntegerIndex ( O , index ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isvalidintegerindex) |
| 10.4.5.17 | TypedArrayGetElement ( O , index ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarraygetelement) |
| 10.4.5.18 | TypedArraySetElement ( O , index , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-typedarraysetelement) |
| 10.4.5.19 | IsArrayBufferViewOutOfBounds ( O ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isarraybufferviewoutofbounds) |
| 10.4.6 | Module Namespace Exotic Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects) |
| 10.4.6.1 | [[GetPrototypeOf]] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-getprototypeof) |
| 10.4.6.2 | [[SetPrototypeOf]] ( V ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-setprototypeof-v) |
| 10.4.6.3 | [[IsExtensible]] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-isextensible) |
| 10.4.6.4 | [[PreventExtensions]] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-preventextensions) |
| 10.4.6.5 | [[GetOwnProperty]] ( P ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-getownproperty-p) |
| 10.4.6.6 | [[DefineOwnProperty]] ( P , Desc ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-defineownproperty-p-desc) |
| 10.4.6.7 | [[HasProperty]] ( P ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-hasproperty-p) |
| 10.4.6.8 | [[Get]] ( P , Receiver ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-get-p-receiver) |
| 10.4.6.9 | [[Set]] ( P , V , Receiver ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-set-p-v-receiver) |
| 10.4.6.10 | [[Delete]] ( P ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-delete-p) |
| 10.4.6.11 | [[OwnPropertyKeys]] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-namespace-exotic-objects-ownpropertykeys) |
| 10.4.6.12 | ModuleNamespaceCreate ( module , exports ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-modulenamespacecreate) |
| 10.4.7 | Immutable Prototype Exotic Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-immutable-prototype-exotic-objects) |
| 10.4.7.1 | [[SetPrototypeOf]] ( V ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-immutable-prototype-exotic-objects-setprototypeof-v) |
| 10.4.7.2 | SetImmutablePrototype ( O , V ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-set-immutable-prototype) |

## Support

Feature-level support tracking with test script references.

### 10.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-arguments-exotic-objects))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Arguments Exotic Objects (mapped/unmapped semantics) | Not Yet Supported |  | JS2IL does not implement the spec Arguments Exotic Object internal methods (including mapped-arguments aliasing). The compiler/runtime currently materialize a minimal function-scope `arguments` value as a JavaScriptRuntime.Array snapshot when referenced; see Section 10.2 tracking for the supported subset. |

