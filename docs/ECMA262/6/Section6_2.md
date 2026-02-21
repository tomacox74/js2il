<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 6.2: ECMAScript Specification Types

[Back to Section6](Section6.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 6.2 | ECMAScript Specification Types | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-specification-types) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 6.2.1 | The Enum Specification Type | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-enum-specification-type) |
| 6.2.2 | The List and Record Specification Types | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-list-and-record-specification-type) |
| 6.2.3 | The Set and Relation Specification Types | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-set-and-relation-specification-type) |
| 6.2.4 | The Completion Record Specification Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-completion-record-specification-type) |
| 6.2.4.1 | NormalCompletion ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-normalcompletion) |
| 6.2.4.2 | ThrowCompletion ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-throwcompletion) |
| 6.2.4.3 | ReturnCompletion ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-returncompletion) |
| 6.2.4.4 | UpdateEmpty ( completionRecord , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-updateempty) |
| 6.2.5 | The Reference Record Specification Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reference-record-specification-type) |
| 6.2.5.1 | IsPropertyReference ( V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ispropertyreference) |
| 6.2.5.2 | IsUnresolvableReference ( V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isunresolvablereference) |
| 6.2.5.3 | IsSuperReference ( V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-issuperreference) |
| 6.2.5.4 | IsPrivateReference ( V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isprivatereference) |
| 6.2.5.5 | GetValue ( V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getvalue) |
| 6.2.5.6 | PutValue ( V , W ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-putvalue) |
| 6.2.5.7 | GetThisValue ( V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getthisvalue) |
| 6.2.5.8 | InitializeReferencedBinding ( V , W ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-initializereferencedbinding) |
| 6.2.5.9 | MakePrivateReference ( baseValue , privateIdentifier ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makeprivatereference) |
| 6.2.6 | The Property Descriptor Specification Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-property-descriptor-specification-type) |
| 6.2.6.1 | IsAccessorDescriptor ( Desc ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isaccessordescriptor) |
| 6.2.6.2 | IsDataDescriptor ( Desc ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isdatadescriptor) |
| 6.2.6.3 | IsGenericDescriptor ( Desc ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isgenericdescriptor) |
| 6.2.6.4 | FromPropertyDescriptor ( Desc ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-frompropertydescriptor) |
| 6.2.6.5 | ToPropertyDescriptor ( Obj ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-topropertydescriptor) |
| 6.2.6.6 | CompletePropertyDescriptor ( Desc ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-completepropertydescriptor) |
| 6.2.7 | The Environment Record Specification Type | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-lexical-environment-and-environment-record-specification-types) |
| 6.2.8 | The Abstract Closure Specification Type | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-abstract-closure) |
| 6.2.9 | Data Blocks | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-data-blocks) |
| 6.2.9.1 | CreateByteDataBlock ( size ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-createbytedatablock) |
| 6.2.9.2 | CreateSharedByteDataBlock ( size ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-createsharedbytedatablock) |
| 6.2.9.3 | CopyDataBlockBytes ( toBlock , toIndex , fromBlock , fromIndex , count ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-copydatablockbytes) |
| 6.2.10 | The PrivateElement Specification Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-privateelement-specification-type) |
| 6.2.11 | The ClassFieldDefinition Record Specification Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-classfielddefinition-record-specification-type) |
| 6.2.12 | Private Names | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-private-names) |
| 6.2.13 | The ClassStaticBlockDefinition Record Specification Type | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-classstaticblockdefinition-record-specification-type) |

## Support

Feature-level support tracking with test script references.

### 6.2 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-specification-types))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ECMAScript Specification Types (overall) | Incomplete |  | This section is normative and implementation-relevant. JS2IL partially maps these spec types via runtime/compiler behavior, but Environment Records and several internal-spec types remain untracked. |

### 6.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-completion-record-specification-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Completion records (normal/throw/return control flow) | Supported with Limitations | [`ControlFlow_TryCatchFinally_ThrowValue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryCatchFinally_ThrowValue.js)<br>[`ControlFlow_TryFinally_Return.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_TryFinally_Return.js) | Core completion behavior is represented by emitted control flow and exception handling, not as first-class Completion Record values. |

### 6.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-reference-record-specification-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Reference records (GetValue/PutValue for identifiers and members) | Supported with Limitations | [`Object_AssignmentExpression_PropertySet_ResultStoredToScopeField.js`](../../../Js2IL.Tests/Object/JavaScript/Object_AssignmentExpression_PropertySet_ResultStoredToScopeField.js)<br>[`Classes_Inheritance_SuperMethodCall.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_Inheritance_SuperMethodCall.js) | Identifier/member references work for supported language forms; spec-exact modeling of all Reference Record variants remains incomplete. |

### 6.2.6 ([tc39.es](https://tc39.es/ecma262/#sec-property-descriptor-specification-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Property descriptor specification type | Supported with Limitations | [`ObjectDefineProperty_Accessor.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectDefineProperty_Accessor.js)<br>[`ObjectCreate_WithPropertyDescriptors.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectCreate_WithPropertyDescriptors.js)<br>[`ObjectCreate_NullPrototype_And_GetOwnPropertyDescriptor.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectCreate_NullPrototype_And_GetOwnPropertyDescriptor.js) | Descriptor creation and usage are implemented for supported object/runtime paths via PropertyDescriptorStore; full spec parity is incomplete. |

### 6.2.9 ([tc39.es](https://tc39.es/ecma262/#sec-data-blocks))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Data Blocks | Not Yet Supported |  | Spec-level byte data block operations depend on ArrayBuffer/SharedArrayBuffer machinery, which is not yet implemented. |

### 6.2.10 ([tc39.es](https://tc39.es/ecma262/#sec-privateelement-specification-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| PrivateElement specification type | Supported with Limitations | [`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js)<br>[`Classes_ClassPrivateProperty_HelperMethod_Log.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassPrivateProperty_HelperMethod_Log.js) | Private instance fields are supported; private methods/accessors are rejected. |

### 6.2.11 ([tc39.es](https://tc39.es/ecma262/#sec-classfielddefinition-record-specification-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ClassFieldDefinition record | Supported with Limitations | [`Classes_ClassProperty_DefaultAndLog.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassProperty_DefaultAndLog.js)<br>[`Classes_ClassWithStaticProperty_DefaultAndLog.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassWithStaticProperty_DefaultAndLog.js) | Supports public/private instance and static fields with initializer constraints; computed names and some class element forms remain unsupported. |

### 6.2.12 ([tc39.es](https://tc39.es/ecma262/#sec-private-names))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Private Names | Supported with Limitations | [`Classes_ClassPrivateField_HelperMethod_Log.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js) | Private names for fields are supported with validator/runtime constraints; invalid/private-name edge cases are not fully spec-complete. |

### 6.2.13 ([tc39.es](https://tc39.es/ecma262/#sec-classstaticblockdefinition-record-specification-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ClassStaticBlockDefinition record | Not Yet Supported |  | Class static blocks are rejected by validation. |

