<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 7.3: Operations on Objects

[Back to Section7](Section7.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 7.3 | Operations on Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-operations-on-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 7.3.1 | MakeBasicObject ( internalSlotsList ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-makebasicobject) |
| 7.3.2 | Get ( O , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-o-p) |
| 7.3.3 | GetV ( V , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getv) |
| 7.3.4 | Set ( O , P , V , Throw ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-set-o-p-v-throw) |
| 7.3.5 | CreateDataProperty ( O , P , V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createdataproperty) |
| 7.3.6 | CreateDataPropertyOrThrow ( O , P , V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createdatapropertyorthrow) |
| 7.3.7 | CreateNonEnumerableDataPropertyOrThrow ( O , P , V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createnonenumerabledatapropertyorthrow) |
| 7.3.8 | DefinePropertyOrThrow ( O , P , desc ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-definepropertyorthrow) |
| 7.3.9 | DeletePropertyOrThrow ( O , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-deletepropertyorthrow) |
| 7.3.10 | GetMethod ( V , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getmethod) |
| 7.3.11 | HasProperty ( O , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-hasproperty) |
| 7.3.12 | HasOwnProperty ( O , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-hasownproperty) |
| 7.3.13 | Call ( F , V [ , argumentsList ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-call) |
| 7.3.14 | Construct ( F [ , argumentsList [ , newTarget ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-construct) |
| 7.3.15 | SetIntegrityLevel ( O , level ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-setintegritylevel) |
| 7.3.16 | TestIntegrityLevel ( O , level ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-testintegritylevel) |
| 7.3.17 | CreateArrayFromList ( elements ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createarrayfromlist) |
| 7.3.18 | LengthOfArrayLike ( obj ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-lengthofarraylike) |
| 7.3.19 | CreateListFromArrayLike ( obj [ , validElementTypes ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-createlistfromarraylike) |
| 7.3.20 | Invoke ( V , P [ , argumentsList ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-invoke) |
| 7.3.21 | OrdinaryHasInstance ( C , O ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryhasinstance) |
| 7.3.22 | SpeciesConstructor ( O , defaultConstructor ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-speciesconstructor) |
| 7.3.23 | EnumerableOwnProperties ( O , kind ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-enumerableownproperties) |
| 7.3.24 | GetFunctionRealm ( obj ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-getfunctionrealm) |
| 7.3.25 | CopyDataProperties ( target , source , excludedItems ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-copydataproperties) |
| 7.3.26 | PrivateElementFind ( O , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privateelementfind) |
| 7.3.27 | PrivateFieldAdd ( O , P , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privatefieldadd) |
| 7.3.28 | PrivateMethodOrAccessorAdd ( O , method ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-privatemethodoraccessoradd) |
| 7.3.29 | HostEnsureCanAddPrivateElement ( O ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-hostensurecanaddprivateelement) |
| 7.3.30 | PrivateGet ( O , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privateget) |
| 7.3.31 | PrivateSet ( O , P , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privateset) |
| 7.3.32 | DefineField ( receiver , fieldRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-definefield) |
| 7.3.33 | InitializeInstanceElements ( O , constructor ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-initializeinstanceelements) |
| 7.3.34 | AddValueToKeyedGroup ( groups , key , value ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-add-value-to-keyed-group) |
| 7.3.35 | GroupBy ( items , callback , keyCoercion ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-groupby) |
| 7.3.36 | GetOptionsObject ( options ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-getoptionsobject) |
| 7.3.37 | SetterThatIgnoresPrototypeProperties ( thisValue , home , p , v ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-SetterThatIgnoresPrototypeProperties) |

## Support

Feature-level support tracking with test script references.

### 7.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-get-o-p))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Property get on object literals and host objects | Supported with Limitations | [`JSON_Parse_SimpleObject.js`](../../../Js2IL.Tests/JSON/JavaScript/JSON_Parse_SimpleObject.js)<br>[`Function_ClosureEscapesScope_ObjectLiteralProperty.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ClosureEscapesScope_ObjectLiteralProperty.js) | Supported via runtime get dispatch for ExpandoObject/object literals and reflection for host objects. Descriptor APIs enable accessor/prototype lookup via side-tables; symbol-keyed properties and many exotic behaviors are incomplete. |

### 7.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-set-o-p-v-throw))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Property set on object literals and host objects | Supported with Limitations | [`ObjectLiteral_PropertyAssign.js`](../../../Js2IL.Tests/Literals/JavaScript/ObjectLiteral_PropertyAssign.js)<br>[`Variable_AssignmentTargets_MemberAndIndex.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_AssignmentTargets_MemberAndIndex.js) | Supported via runtime set dispatch for ExpandoObject/object literals and reflection for host objects. Throw/invariant behavior is not fully spec-complete. |

### 7.3.7 ([tc39.es](https://tc39.es/ecma262/#sec-createnonenumerabledatapropertyorthrow))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| CreateNonEnumerableDataPropertyOrThrow (approx.) | Supported with Limitations | [`ObjectDefineProperty_Enumerable_ForIn.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectDefineProperty_Enumerable_ForIn.js) | Non-enumerable data properties can be created through descriptor APIs; strict throw/invariant semantics are approximate. |

### 7.3.8 ([tc39.es](https://tc39.es/ecma262/#sec-definepropertyorthrow))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| DefinePropertyOrThrow (approx.) | Supported with Limitations | [`ObjectDefineProperty_Accessor.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectDefineProperty_Accessor.js)<br>[`ObjectDefineProperty_Enumerable_ForIn.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectDefineProperty_Enumerable_ForIn.js)<br>[`ObjectCreate_WithPropertyDescriptors.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectCreate_WithPropertyDescriptors.js) | Approximated by runtime defineProperty/defineProperties/create using PropertyDescriptorStore; many spec validation and throw paths are incomplete. |

### 7.3.9 ([tc39.es](https://tc39.es/ecma262/#sec-deletepropertyorthrow))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| DeletePropertyOrThrow (approx.) | Supported with Limitations | [`ControlFlow_ForIn_Mutation_DeleteAndAdd.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Mutation_DeleteAndAdd.js) | Delete operations are supported for dynamic object shapes; full non-configurable property behavior and strict throw fidelity are incomplete. |

### 7.3.11 ([tc39.es](https://tc39.es/ecma262/#sec-hasproperty))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Property presence checks (in operator / HasProperty) | Supported with Limitations | [`BinaryOperator_In_Object_OwnAndMissing.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_In_Object_OwnAndMissing.js)<br>[`ControlFlow_ForIn_Object_Basic.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Object_Basic.js) | Implemented for object literals, arrays, typed arrays, strings, and host objects. Prototype traversal is supported with prototype-chain mode; symbol support is limited. |

### 7.3.12 ([tc39.es](https://tc39.es/ecma262/#sec-hasownproperty))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| HasOwnProperty | Supported with Limitations | [`Object_Prototype_HasOwnProperty_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/Object_Prototype_HasOwnProperty_Basic.js) | Object.prototype.hasOwnProperty is provided with descriptor-store, dictionary, and host-object fallbacks. |

### 7.3.13 ([tc39.es](https://tc39.es/ecma262/#sec-call))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Function and method calls | Supported with Limitations | [`Function_HelloWorld.js`](../../../Js2IL.Tests/Function/JavaScript/Function_HelloWorld.js)<br>[`Function_ObjectLiteralMethod_ThisBinding.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js)<br>[`Function_CallViaVariable_Reassignment.js`](../../../Js2IL.Tests/Function/JavaScript/Function_CallViaVariable_Reassignment.js)<br>[`Function_Apply_Basic.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Apply_Basic.js)<br>[`Function_Bind_Basic_PartialApplication.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Bind_Basic_PartialApplication.js) | Supports common declared/function-expression/delegate call paths. Some exotic call semantics are not implemented. |

### 7.3.14 ([tc39.es](https://tc39.es/ecma262/#sec-construct))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Construction (new) including dynamic constructor values | Supported with Limitations | [`Classes_DeclareEmptyClass.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js)<br>[`CommonJS_Export_ClassWithConstructor.js`](../../../Js2IL.Tests/CommonJS/JavaScript/CommonJS_Export_ClassWithConstructor.js)<br>[`NewExpression_Number_Sugar.js`](../../../Js2IL.Tests/Literals/JavaScript/NewExpression_Number_Sugar.js)<br>[`ctorPadding.js`](../../../Js2IL.Tests/Hosting/JavaScript/ctorPadding.js) | Supports statically-known constructors and runtime fallback for dynamic constructor values; full newTarget and exotic constructor behavior is incomplete. |

### 7.3.18 ([tc39.es](https://tc39.es/ecma262/#sec-lengthofarraylike))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| LengthOfArrayLike | Supported with Limitations | [`Array_LengthProperty_ReturnsCount.js`](../../../Js2IL.Tests/Array/JavaScript/Array_LengthProperty_ReturnsCount.js)<br>[`Int32Array_Construct_Length.js`](../../../Js2IL.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js)<br>[`String_Split_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_Split_Basic.js) | Implemented for supported array-like runtime types with partial coercion behavior. |

### 7.3.21 ([tc39.es](https://tc39.es/ecma262/#sec-ordinaryhasinstance))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| OrdinaryHasInstance (instanceof paths) | Supported with Limitations | [`BinaryOperator_InstanceOf_Basic.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_InstanceOf_Basic.js) | Implemented with prototype-chain checks for supported callable constructor values; full spec hooks are incomplete. |

### 7.3.23 ([tc39.es](https://tc39.es/ecma262/#sec-enumerableownproperties))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| EnumerableOwnProperties (keys/values/entries) | Supported with Limitations | [`Object_Keys_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/Object_Keys_Basic.js)<br>[`Object_Values_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/Object_Values_Basic.js)<br>[`Object_Entries_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/Object_Entries_Basic.js) | Implemented for supported object shapes with descriptor-store enumerable filtering. |

### 7.3.25 ([tc39.es](https://tc39.es/ecma262/#sec-copydataproperties))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| CopyDataProperties (object spread) | Supported with Limitations | [`ObjectLiteral_Spread_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_Spread_Basic.js)<br>[`ObjectLiteral_Spread_SkipsNonEnumerable.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_Spread_SkipsNonEnumerable.js) | Implemented via SpreadInto/Object spread for enumerable own string-keyed properties; full symbol/exotic semantics are incomplete. |

### 7.3.26 ([tc39.es](https://tc39.es/ecma262/#sec-privateelementfind))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| PrivateElementFind for private fields | Supported with Limitations | [`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js)<br>[`Classes_ClassPrivateProperty_HelperMethod_Log.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassPrivateProperty_HelperMethod_Log.js) | Private field access works for supported class forms; private methods/accessors and complete validation coverage are not yet implemented. |

### 7.3.33 ([tc39.es](https://tc39.es/ecma262/#sec-initializeinstanceelements))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| InitializeInstanceElements (class fields + private fields) | Supported with Limitations | [`Classes_ClassProperty_DefaultAndLog.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassProperty_DefaultAndLog.js)<br>[`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js) | Supports instance field initializers and private instance fields. Static blocks, private methods/accessors, and some inheritance details are incomplete. |

