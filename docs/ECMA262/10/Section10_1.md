<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.1: Ordinary Object Internal Methods and Internal Slots

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 10.1 | Ordinary Object Internal Methods and Internal Slots | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 10.1.1 | [[GetPrototypeOf]] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-getprototypeof) |
| 10.1.1.1 | OrdinaryGetPrototypeOf ( O ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinarygetprototypeof) |
| 10.1.2 | [[SetPrototypeOf]] ( V ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-setprototypeof-v) |
| 10.1.2.1 | OrdinarySetPrototypeOf ( O , V ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinarysetprototypeof) |
| 10.1.3 | [[IsExtensible]] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-isextensible) |
| 10.1.3.1 | OrdinaryIsExtensible ( O ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryisextensible) |
| 10.1.4 | [[PreventExtensions]] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-preventextensions) |
| 10.1.4.1 | OrdinaryPreventExtensions ( O ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinarypreventextensions) |
| 10.1.5 | [[GetOwnProperty]] ( P ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-getownproperty-p) |
| 10.1.5.1 | OrdinaryGetOwnProperty ( O , P ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinarygetownproperty) |
| 10.1.6 | [[DefineOwnProperty]] ( P , Desc ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-defineownproperty-p-desc) |
| 10.1.6.1 | OrdinaryDefineOwnProperty ( O , P , Desc ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinarydefineownproperty) |
| 10.1.6.2 | IsCompatiblePropertyDescriptor ( Extensible , Desc , Current ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-iscompatiblepropertydescriptor) |
| 10.1.6.3 | ValidateAndApplyPropertyDescriptor ( O , P , extensible , Desc , current ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-validateandapplypropertydescriptor) |
| 10.1.7 | [[HasProperty]] ( P ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-hasproperty-p) |
| 10.1.7.1 | OrdinaryHasProperty ( O , P ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryhasproperty) |
| 10.1.8 | [[Get]] ( P , Receiver ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-get-p-receiver) |
| 10.1.8.1 | OrdinaryGet ( O , P , Receiver ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryget) |
| 10.1.9 | [[Set]] ( P , V , Receiver ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-set-p-v-receiver) |
| 10.1.9.1 | OrdinarySet ( O , P , V , Receiver ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryset) |
| 10.1.9.2 | OrdinarySetWithOwnDescriptor ( O , P , V , Receiver , ownDesc ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinarysetwithowndescriptor) |
| 10.1.10 | [[Delete]] ( P ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-delete-p) |
| 10.1.10.1 | OrdinaryDelete ( O , P ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinarydelete) |
| 10.1.11 | [[OwnPropertyKeys]] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-ownpropertykeys) |
| 10.1.11.1 | OrdinaryOwnPropertyKeys ( O ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryownpropertykeys) |
| 10.1.12 | OrdinaryObjectCreate ( proto [ , additionalInternalSlotsList ] ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryobjectcreate) |
| 10.1.13 | OrdinaryCreateFromConstructor ( constructor , intrinsicDefaultProto [ , internalSlotsList ] ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-ordinarycreatefromconstructor) |
| 10.1.14 | GetPrototypeFromConstructor ( constructor , intrinsicDefaultProto ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-getprototypefromconstructor) |
| 10.1.15 | RequireInternalSlot ( O , internalSlot ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-requireinternalslot) |

## Support

Feature-level support tracking with test script references.

### 10.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-getprototypeof))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Ordinary object property get/set (object literals + host objects) | Partially Supported | [`ObjectLiteral_PropertyAssign.js`](../../../Js2IL.Tests/Literals/JavaScript/ObjectLiteral_PropertyAssign.js)<br>[`Variable_AssignmentTargets_MemberAndIndex.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_AssignmentTargets_MemberAndIndex.js)<br>[`BinaryOperator_In_Object_OwnAndMissing.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_In_Object_OwnAndMissing.js) | Supports a pragmatic subset of [[Get]]/[[Set]]/[[HasProperty]] over ExpandoObject/object literals and reflection-backed host objects. Full internal-slot and descriptor mechanics are not implemented. |

### 10.1.6 ([tc39.es](https://tc39.es/ecma262/#sec-ordinary-object-internal-methods-and-internal-slots-defineownproperty-p-desc))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Property existence and enumeration in ordinary objects | Partially Supported | [`ControlFlow_ForIn_Object_Basic.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Object_Basic.js)<br>[`BinaryOperator_In_Object_OwnAndMissing.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_In_Object_OwnAndMissing.js) | Supports a for-in style enumerable key iteration and basic key presence checks. Ordering, symbols, and descriptor attributes are not fully aligned with spec. |

### 10.1.12 ([tc39.es](https://tc39.es/ecma262/#sec-ordinaryobjectcreate))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Object creation from object literals and JSON.parse | Partially Supported | [`ObjectLiteral.js`](../../../Js2IL.Tests/Literals/JavaScript/ObjectLiteral.js)<br>[`JSON_Parse_SimpleObject.js`](../../../Js2IL.Tests/JSON/JavaScript/JSON_Parse_SimpleObject.js) | Creates runtime objects for literals/JSON, but does not implement prototype selection and descriptor initialization per spec. |

### 10.1.13 ([tc39.es](https://tc39.es/ecma262/#sec-ordinarycreatefromconstructor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Create-from-constructor patterns used by built-ins and user code | Partially Supported | [`Array_Map_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Map_Basic.js)<br>[`Int32Array_Construct_Length.js`](../../../Js2IL.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js) | Many built-ins are implemented directly in the runtime rather than via species/prototype mechanics, so behavior may differ from spec for custom constructors and @@species. |

