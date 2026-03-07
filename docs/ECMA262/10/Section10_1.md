<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.1: Ordinary Object Internal Methods and Internal Slots

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-07T02:30:25Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 10.1 | Ordinary Object Internal Methods and Internal Slots | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 10.1.1 | [[GetPrototypeOf]] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-getprototypeof) |
| 10.1.1.1 | OrdinaryGetPrototypeOf ( O ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinarygetprototypeof) |
| 10.1.2 | [[SetPrototypeOf]] ( V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-setprototypeof-v) |
| 10.1.2.1 | OrdinarySetPrototypeOf ( O , V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinarysetprototypeof) |
| 10.1.3 | [[IsExtensible]] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-isextensible) |
| 10.1.3.1 | OrdinaryIsExtensible ( O ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryisextensible) |
| 10.1.4 | [[PreventExtensions]] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-preventextensions) |
| 10.1.4.1 | OrdinaryPreventExtensions ( O ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinarypreventextensions) |
| 10.1.5 | [[GetOwnProperty]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-getownproperty-p) |
| 10.1.5.1 | OrdinaryGetOwnProperty ( O , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinarygetownproperty) |
| 10.1.6 | [[DefineOwnProperty]] ( P , Desc ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-defineownproperty-p-desc) |
| 10.1.6.1 | OrdinaryDefineOwnProperty ( O , P , Desc ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinarydefineownproperty) |
| 10.1.6.2 | IsCompatiblePropertyDescriptor ( Extensible , Desc , Current ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-iscompatiblepropertydescriptor) |
| 10.1.6.3 | ValidateAndApplyPropertyDescriptor ( O , P , extensible , Desc , current ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-validateandapplypropertydescriptor) |
| 10.1.7 | [[HasProperty]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-hasproperty-p) |
| 10.1.7.1 | OrdinaryHasProperty ( O , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryhasproperty) |
| 10.1.8 | [[Get]] ( P , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-get-p-receiver) |
| 10.1.8.1 | OrdinaryGet ( O , P , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryget) |
| 10.1.9 | [[Set]] ( P , V , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-set-p-v-receiver) |
| 10.1.9.1 | OrdinarySet ( O , P , V , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryset) |
| 10.1.9.2 | OrdinarySetWithOwnDescriptor ( O , P , V , Receiver , ownDesc ) | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-ordinarysetwithowndescriptor) |
| 10.1.10 | [[Delete]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-delete-p) |
| 10.1.10.1 | OrdinaryDelete ( O , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinarydelete) |
| 10.1.11 | [[OwnPropertyKeys]] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-ownpropertykeys) |
| 10.1.11.1 | OrdinaryOwnPropertyKeys ( O ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryownpropertykeys) |
| 10.1.12 | OrdinaryObjectCreate ( proto [ , additionalInternalSlotsList ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryobjectcreate) |
| 10.1.13 | OrdinaryCreateFromConstructor ( constructor , intrinsicDefaultProto [ , internalSlotsList ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinarycreatefromconstructor) |
| 10.1.14 | GetPrototypeFromConstructor ( constructor , intrinsicDefaultProto ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getprototypefromconstructor) |
| 10.1.15 | RequireInternalSlot ( O , internalSlot ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-requireinternalslot) |

## Support

Feature-level support tracking with test script references.

### 10.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-getprototypeof))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Prototype access and mutation via Object.getPrototypeOf/Object.setPrototypeOf | Supported with Limitations | [`PrototypeChain_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/PrototypeChain_Basic.js) | JS2IL stores an optional [[Prototype]] in the PrototypeChain side-table and only enables prototype-chain semantics when code exercises them. Null-prototype objects and explicit prototype mutation work, but ordinary/exotic invariants are not enforced. |

### 10.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-isextensible))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Object.preventExtensions / Object.isExtensible / Object.seal / Object.freeze | Supported with Limitations | [`Object_Integrity_FreezeSeal_PreventExtensions.js`](../../../Js2IL.Tests/Object/JavaScript/Object_Integrity_FreezeSeal_PreventExtensions.js) | Integrity APIs are backed by a simplified ObjectIntegrityState plus descriptor rewrites for existing own properties. They cover the common preventExtensions/seal/freeze checks, but do not model every descriptor invariant or exotic-object edge case from the specification. |

### 10.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-getownproperty-p))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Property descriptor APIs (defineProperty, defineProperties, getOwnPropertyDescriptor(s)) | Supported with Limitations | [`ObjectDefineProperty_Accessor.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectDefineProperty_Accessor.js)<br>[`Object_DefineProperties_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/Object_DefineProperties_Basic.js)<br>[`Object_GetOwnPropertyDescriptors_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/Object_GetOwnPropertyDescriptors_Basic.js)<br>[`ObjectCreate_NullPrototype_And_GetOwnPropertyDescriptor.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectCreate_NullPrototype_And_GetOwnPropertyDescriptor.js)<br>[`ObjectCreate_WithPropertyDescriptors.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectCreate_WithPropertyDescriptors.js) | Descriptors are stored in PropertyDescriptorStore and support common data/accessor scenarios, non-enumerable properties, and non-extensible object checks. Validation is still a best-effort subset of IsCompatiblePropertyDescriptor / ValidateAndApplyPropertyDescriptor rather than a full spec implementation. |

### 10.1.7 ([tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-hasproperty-p))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Property lookup, assignment, and membership on ordinary objects | Supported with Limitations | [`BinaryOperator_In_Object_OwnAndMissing.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_In_Object_OwnAndMissing.js)<br>[`Object_HasOwn_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/Object_HasOwn_Basic.js)<br>[`PrototypeChain_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/PrototypeChain_Basic.js)<br>[`Variable_AssignmentTargets_MemberAndIndex.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_AssignmentTargets_MemberAndIndex.js) | Ordinary-style [[HasProperty]], [[Get]], and [[Set]] work over JsObject/ExpandoObject/IDictionary receivers, reflection-backed host objects, descriptor-backed accessors, and opt-in prototype chains. Descriptor validation, receiver substitution, and exotic-object corner cases remain incomplete. |

### 10.1.10 ([tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-delete-p))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Deleting ordinary object properties | Supported with Limitations | [`ControlFlow_ForIn_Mutation_DeleteAndAdd.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Mutation_DeleteAndAdd.js)<br>[`Object_Integrity_FreezeSeal_PreventExtensions.js`](../../../Js2IL.Tests/Object/JavaScript/Object_Integrity_FreezeSeal_PreventExtensions.js) | delete removes configurable properties from dictionary-backed objects and respects non-configurable descriptors. Arrays, typed arrays, strings, and CLR-backed objects still use simplified no-op behavior rather than full ordinary-object deletion semantics. |

### 10.1.11 ([tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-ownpropertykeys))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Own key enumeration and reflection helpers | Supported with Limitations | [`Object_Keys_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/Object_Keys_Basic.js)<br>[`Object_Values_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/Object_Values_Basic.js)<br>[`Object_Entries_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/Object_Entries_Basic.js)<br>[`Object_GetOwnPropertyNames_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/Object_GetOwnPropertyNames_Basic.js) | Object.keys/values/entries/getOwnPropertyNames enumerate own keys and descriptor-backed enumerability for ordinary objects. Ordering and symbol-key behavior are pragmatic rather than fully ECMA-262 compliant. |

### 10.1.12 ([tc39.es](https://tc39.es/ecma262/#sec-ordinaryobjectcreate))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Object.create and constructor-based ordinary object creation | Supported with Limitations | [`ObjectCreate_WithPropertyDescriptors.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectCreate_WithPropertyDescriptors.js)<br>[`ObjectCreate_NullPrototype_And_GetOwnPropertyDescriptor.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectCreate_NullPrototype_And_GetOwnPropertyDescriptor.js)<br>[`Function_Prototype_ObjectCreate_ObjectPrototype.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Prototype_ObjectCreate_ObjectPrototype.js) | Object.create handles null and explicit prototypes, and function construction consults constructor.prototype when instantiating delegate-backed functions. newTarget-based default prototype selection and other constructor-side abstract operations are still simplified. |

