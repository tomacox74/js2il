<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 7.3: Operations on Objects

[Back to Section7](Section7.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-07T20:14:57Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 7.3 | Operations on Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-operations-on-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 7.3.1 | MakeBasicObject ( internalSlotsList ) | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-makebasicobject) |
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
| 7.3.15 | SetIntegrityLevel ( O , level ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setintegritylevel) |
| 7.3.16 | TestIntegrityLevel ( O , level ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-testintegritylevel) |
| 7.3.17 | CreateArrayFromList ( elements ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createarrayfromlist) |
| 7.3.18 | LengthOfArrayLike ( obj ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-lengthofarraylike) |
| 7.3.19 | CreateListFromArrayLike ( obj [ , validElementTypes ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createlistfromarraylike) |
| 7.3.20 | Invoke ( V , P [ , argumentsList ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-invoke) |
| 7.3.21 | OrdinaryHasInstance ( C , O ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinaryhasinstance) |
| 7.3.22 | SpeciesConstructor ( O , defaultConstructor ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-speciesconstructor) |
| 7.3.23 | EnumerableOwnProperties ( O , kind ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-enumerableownproperties) |
| 7.3.24 | GetFunctionRealm ( obj ) | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-getfunctionrealm) |
| 7.3.25 | CopyDataProperties ( target , source , excludedItems ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-copydataproperties) |
| 7.3.26 | PrivateElementFind ( O , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privateelementfind) |
| 7.3.27 | PrivateFieldAdd ( O , P , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privatefieldadd) |
| 7.3.28 | PrivateMethodOrAccessorAdd ( O , method ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privatemethodoraccessoradd) |
| 7.3.29 | HostEnsureCanAddPrivateElement ( O ) | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-hostensurecanaddprivateelement) |
| 7.3.30 | PrivateGet ( O , P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privateget) |
| 7.3.31 | PrivateSet ( O , P , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privateset) |
| 7.3.32 | DefineField ( receiver , fieldRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-definefield) |
| 7.3.33 | InitializeInstanceElements ( O , constructor ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-initializeinstanceelements) |
| 7.3.34 | AddValueToKeyedGroup ( groups , key , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-add-value-to-keyed-group) |
| 7.3.35 | GroupBy ( items , callback , keyCoercion ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-groupby) |
| 7.3.36 | GetOptionsObject ( options ) | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-getoptionsobject) |
| 7.3.37 | SetterThatIgnoresPrototypeProperties ( thisValue , home , p , v ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-SetterThatIgnoresPrototypeProperties) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 7.3 ([tc39.es](https://tc39.es/ecma262/#sec-operations-on-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Object operations across integrity, argument-list normalization, private elements, species accessors, and grouping | Supported with Limitations | [`Object_Integrity_FreezeSeal_PreventExtensions.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Integrity_FreezeSeal_PreventExtensions.js)<br>[`Function_Apply_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Apply_Basic.js)<br>[`Classes_ClassPrivateMethodAndAccessor_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateMethodAndAccessor_Log.js)<br>[`Object_GroupBy_Basic.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_GroupBy_Basic.js)<br>[`Promise_SymbolSpecies.js`](../../../tests/Jroc.Tests/Promise/JavaScript/Promise_SymbolSpecies.js) |  | JROC covers the object-operation helpers most visible through today's runtime surface, including integrity-state transitions, current argument-list normalization paths, private elements, built-in species accessors, class field own-property definition, and Object.groupBy. Remaining limitations are concentrated in exotic object/proxy edge cases and in abstract helpers that are still modeled inline rather than as fully general standalone operations. |

### 7.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-get-o-p))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Property get on object literals and host objects | Supported with Limitations | [`JSON_Parse_SimpleObject.js`](../../../tests/Jroc.Tests/JSON/JavaScript/JSON_Parse_SimpleObject.js)<br>[`Function_ClosureEscapesScope_ObjectLiteralProperty.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ClosureEscapesScope_ObjectLiteralProperty.js) |  | Supported via runtime get dispatch for ExpandoObject/object literals and reflection for host objects. Descriptor APIs enable accessor/prototype lookup via side-tables; symbol-keyed properties and many exotic behaviors are incomplete. |

### 7.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-set-o-p-v-throw))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Property set on object literals and host objects | Supported with Limitations | [`ObjectLiteral_PropertyAssign.js`](../../../tests/Jroc.Tests/Literals/JavaScript/ObjectLiteral_PropertyAssign.js)<br>[`Variable_AssignmentTargets_MemberAndIndex.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_AssignmentTargets_MemberAndIndex.js) |  | Supported via runtime set dispatch for ExpandoObject/object literals and reflection for host objects. Throw/invariant behavior is not fully spec-complete. |

### 7.3.7 ([tc39.es](https://tc39.es/ecma262/#sec-createnonenumerabledatapropertyorthrow))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| CreateNonEnumerableDataPropertyOrThrow (approx.) | Supported with Limitations | [`ObjectDefineProperty_Enumerable_ForIn.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectDefineProperty_Enumerable_ForIn.js) |  | Non-enumerable data properties can be created through descriptor APIs; strict throw/invariant semantics are approximate. |

### 7.3.8 ([tc39.es](https://tc39.es/ecma262/#sec-definepropertyorthrow))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| DefinePropertyOrThrow (approx.) | Supported with Limitations | [`ObjectDefineProperty_Accessor.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectDefineProperty_Accessor.js)<br>[`ObjectDefineProperty_Enumerable_ForIn.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectDefineProperty_Enumerable_ForIn.js)<br>[`ObjectCreate_WithPropertyDescriptors.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectCreate_WithPropertyDescriptors.js) |  | Approximated by runtime defineProperty/defineProperties/create using PropertyDescriptorStore; many spec validation and throw paths are incomplete. |

### 7.3.9 ([tc39.es](https://tc39.es/ecma262/#sec-deletepropertyorthrow))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| DeletePropertyOrThrow (approx.) | Supported with Limitations | [`ControlFlow_ForIn_Mutation_DeleteAndAdd.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Mutation_DeleteAndAdd.js) |  | Delete operations are supported for dynamic object shapes; full non-configurable property behavior and strict throw fidelity are incomplete. |

### 7.3.11 ([tc39.es](https://tc39.es/ecma262/#sec-hasproperty))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Property presence checks (in operator / HasProperty) | Supported with Limitations | [`BinaryOperator_In_Object_OwnAndMissing.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_In_Object_OwnAndMissing.js)<br>[`ControlFlow_ForIn_Object_Basic.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Object_Basic.js) |  | Implemented for object literals, arrays, typed arrays, strings, and host objects. Prototype traversal is supported with prototype-chain mode; symbol support is limited. |

### 7.3.12 ([tc39.es](https://tc39.es/ecma262/#sec-hasownproperty))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| HasOwnProperty | Supported with Limitations | [`Object_Prototype_HasOwnProperty_Basic.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Prototype_HasOwnProperty_Basic.js) |  | Object.prototype.hasOwnProperty is provided with descriptor-store, dictionary, and host-object fallbacks. |

### 7.3.13 ([tc39.es](https://tc39.es/ecma262/#sec-call))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function and method calls | Supported with Limitations | [`Function_HelloWorld.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_HelloWorld.js)<br>[`Function_ObjectLiteralMethod_ThisBinding.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js)<br>[`Function_CallViaVariable_Reassignment.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_CallViaVariable_Reassignment.js)<br>[`Function_Apply_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Apply_Basic.js)<br>[`Function_Bind_Basic_PartialApplication.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Bind_Basic_PartialApplication.js) |  | Supports common declared/function-expression/delegate call paths. Some exotic call semantics are not implemented. |

### 7.3.14 ([tc39.es](https://tc39.es/ecma262/#sec-construct))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Construction (new) including dynamic constructor values | Supported with Limitations | [`Classes_DeclareEmptyClass.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js)<br>[`CommonJS_Export_ClassWithConstructor.js`](../../../tests/Jroc.Tests/CommonJS/JavaScript/CommonJS_Export_ClassWithConstructor.js)<br>[`NewExpression_Number_Sugar.js`](../../../tests/Jroc.Tests/Literals/JavaScript/NewExpression_Number_Sugar.js)<br>[`ctorPadding.js`](../../../tests/Jroc.Tests/Hosting/JavaScript/ctorPadding.js) |  | Supports statically-known constructors and runtime fallback for dynamic constructor values; full newTarget and exotic constructor behavior is incomplete. |

### 7.3.15 ([tc39.es](https://tc39.es/ecma262/#sec-setintegritylevel))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SetIntegrityLevel through Object.preventExtensions, Object.seal, and Object.freeze | Supported with Limitations | [`Object_Integrity_FreezeSeal_PreventExtensions.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Integrity_FreezeSeal_PreventExtensions.js)<br>[`Object_Integrity_DefineProperty_And_StrictWrites.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Integrity_DefineProperty_And_StrictWrites.js) |  | Ordinary-object integrity state is tracked and enforced for the covered preventExtensions/seal/freeze paths, including strict-mode write failures after freezing. Full proxy invariants and every exotic-object integrity rule are still incomplete. |

### 7.3.16 ([tc39.es](https://tc39.es/ecma262/#sec-testintegritylevel))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| TestIntegrityLevel through Object.isExtensible, Object.isSealed, and Object.isFrozen | Supported with Limitations | [`Object_Integrity_FreezeSeal_PreventExtensions.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Integrity_FreezeSeal_PreventExtensions.js)<br>[`Object_Integrity_DefineProperty_And_StrictWrites.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Integrity_DefineProperty_And_StrictWrites.js) |  | The runtime reports integrity state consistently for the covered ordinary-object transitions and descriptor combinations. Coverage is still strongest for ordinary objects rather than every proxy and host-object edge case. |

### 7.3.18 ([tc39.es](https://tc39.es/ecma262/#sec-lengthofarraylike))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| LengthOfArrayLike | Supported with Limitations | [`Array_LengthProperty_ReturnsCount.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_LengthProperty_ReturnsCount.js)<br>[`Int32Array_Construct_Length.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js)<br>[`String_Split_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Split_Basic.js) |  | Implemented for supported array-like runtime types with partial coercion behavior. |

### 7.3.19 ([tc39.es](https://tc39.es/ecma262/#sec-createlistfromarraylike))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| CreateListFromArrayLike in apply/construct-style argument normalization | Supported with Limitations | [`Function_Apply_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Apply_Basic.js)<br>[`Function_Apply_NullArgArray_TreatedAsEmpty.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Apply_NullArgArray_TreatedAsEmpty.js) | `test/language/rest-parameters/rest-parameters-apply.js` | Current apply/construct normalization accepts JavaScript arrays, CLR arrays, and enumerable argument sources for the covered call sites. Generic length-indexed array-like objects and validElementTypes filtering are not yet modeled as a complete standalone abstract operation. |

### 7.3.21 ([tc39.es](https://tc39.es/ecma262/#sec-ordinaryhasinstance))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| OrdinaryHasInstance (instanceof paths) | Supported with Limitations | [`BinaryOperator_InstanceOf_Basic.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_InstanceOf_Basic.js) |  | Implemented with prototype-chain checks for supported callable constructor values; full spec hooks are incomplete. |

### 7.3.22 ([tc39.es](https://tc39.es/ecma262/#sec-speciesconstructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SpeciesConstructor through built-in @@species accessors and Promise constructor-receiver helpers | Supported with Limitations | [`Promise_SymbolSpecies.js`](../../../tests/Jroc.Tests/Promise/JavaScript/Promise_SymbolSpecies.js) | `test/built-ins/Map/Symbol.species/return-value.js`<br>`test/built-ins/Set/Symbol.species/return-value.js` | Promise, Map, and Set expose the covered @@species accessor behavior, and Promise constructor-receiver helpers honor the active constructor in the supported resolve/try paths. General species-based derived construction across all spec consumers is still incomplete. |

### 7.3.23 ([tc39.es](https://tc39.es/ecma262/#sec-enumerableownproperties))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| EnumerableOwnProperties (keys/values/entries) | Supported with Limitations | [`Object_Keys_Basic.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Keys_Basic.js)<br>[`Object_Values_Basic.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Values_Basic.js)<br>[`Object_Entries_Basic.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Entries_Basic.js) |  | Implemented for supported object shapes with descriptor-store enumerable filtering. |

### 7.3.25 ([tc39.es](https://tc39.es/ecma262/#sec-copydataproperties))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| CopyDataProperties (object spread) | Supported with Limitations | [`ObjectLiteral_Spread_Basic.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_Spread_Basic.js)<br>[`ObjectLiteral_Spread_SkipsNonEnumerable.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_Spread_SkipsNonEnumerable.js) |  | Implemented via SpreadInto/Object spread for enumerable own string-keyed properties; full symbol/exotic semantics are incomplete. |

### 7.3.26 ([tc39.es](https://tc39.es/ecma262/#sec-privateelementfind))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| PrivateElementFind for private fields | Supported with Limitations | [`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js)<br>[`Classes_ClassPrivateProperty_HelperMethod_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateProperty_HelperMethod_Log.js)<br>[`fields-multiple-definitions-static-private-methods-proxy.js`](../../../tests/Jroc.Test262.Tests/language/expressions/class/elements/JavaScript/fields-multiple-definitions-static-private-methods-proxy.js) |  | Private field access works for supported class forms. Supported private methods participate in receiver brand checks, including static private methods rejecting proxy receivers; private accessors and complete validation coverage remain limited. |

### 7.3.28 ([tc39.es](https://tc39.es/ecma262/#sec-privatemethodoraccessoradd))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| PrivateMethodOrAccessorAdd for private methods and private accessors | Supported with Limitations | [`Classes_ClassPrivateMethodAndAccessor_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateMethodAndAccessor_Log.js)<br>[`Classes_ClassPrivateAccessor_EdgeCases_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateAccessor_EdgeCases_Log.js)<br>[`Classes_ClassPrivateAccessor_ClassExpression_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateAccessor_ClassExpression_Log.js) | `test/language/expressions/class/elements/fields-multiple-definitions-static-private-methods-proxy.js` | JROC initializes and brands the covered private methods and private accessors, including static private method receiver checks and accessor invocation. Broader spec parity for every duplicate-definition and exotic evaluation edge case is still incomplete. |

### 7.3.33 ([tc39.es](https://tc39.es/ecma262/#sec-initializeinstanceelements))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| InitializeInstanceElements (class fields + private fields) | Supported with Limitations | [`Classes_ClassProperty_DefaultAndLog.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassProperty_DefaultAndLog.js)<br>[`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js)<br>[`constructor-this-tdz-during-initializers.js`](../../../tests/Jroc.Test262.Tests/language/expressions/class/JavaScript/constructor-this-tdz-during-initializers.js) |  | Supports instance field initializers and private instance fields, including derived-constructor initialization after super() for supported class bases. Static blocks, supported private methods, and current private accessor forms are implemented; some inheritance and exotic receiver details remain incomplete. |

### 7.3.34 ([tc39.es](https://tc39.es/ecma262/#sec-add-value-to-keyed-group))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| AddValueToKeyedGroup through Object.groupBy accumulation | Supported with Limitations | [`Object_GroupBy_Basic.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_GroupBy_Basic.js) |  | The Object.groupBy implementation groups callback results under the covered property keys and appends values into the correct per-key buckets. Edge cases around the full spec key-coercion matrix and every exotic iterable input remain incomplete. |

### 7.3.35 ([tc39.es](https://tc39.es/ecma262/#sec-groupby))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| GroupBy via Object.groupBy | Supported with Limitations | [`Object_GroupBy_Basic.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_GroupBy_Basic.js) |  | Object.groupBy provides the covered callback-driven grouping behavior over supported iterable inputs. The current implementation does not yet claim full parity for all abrupt-completion, coercion-order, and exotic iterator corner cases. |

### 7.3.37 ([tc39.es](https://tc39.es/ecma262/#sec-SetterThatIgnoresPrototypeProperties))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SetterThatIgnoresPrototypeProperties behavior in class field definition and class member installation | Supported with Limitations |  | `test/language/statements/class/elements/class-field-is-observable-by-proxy.js`<br>`test/language/statements/class/subclass/superclass-prototype-setter-constructor.js`<br>`test/language/statements/class/subclass/superclass-prototype-setter-method-override.js` | The covered class-field and class-member definition paths create own properties without accidentally dispatching inherited prototype setters, while still remaining observable through defineProperty-style proxy interception where the spec requires it. This documentation does not yet claim full parity for every built-in accessor that reuses the helper. |

