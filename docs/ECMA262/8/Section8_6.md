<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 8.6: Miscellaneous

[Back to Section8](Section8.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 8.6 | Miscellaneous | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-syntax-directed-operations-miscellaneous) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 8.6.1 | Runtime Semantics: InstantiateFunctionObject | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiatefunctionobject) |
| 8.6.2 | Runtime Semantics: BindingInitialization | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-bindinginitialization) |
| 8.6.2.1 | InitializeBoundName ( name , value , environment ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-initializeboundname) |
| 8.6.3 | Runtime Semantics: IteratorBindingInitialization | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-iteratorbindinginitialization) |
| 8.6.4 | Static Semantics: AssignmentTargetType | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-assignmenttargettype) |
| 8.6.5 | Static Semantics: PropName | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-propname) |

## Support

Feature-level support tracking with test script references.

### 8.6.1 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-instantiatefunctionobject))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Instantiation of function/arrow/class callable objects | Supported with Limitations | [`Function_HelloWorld.js`](../../../Js2IL.Tests/Function/JavaScript/Function_HelloWorld.js)<br>[`Function_IIFE_Recursive.js`](../../../Js2IL.Tests/Function/JavaScript/Function_IIFE_Recursive.js)<br>[`Classes_DeclareEmptyClass.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js) | Function-like AST forms are lowered to delegate-backed callables and class types, but full spec object-internal-slot behavior for every callable form is not complete. |

### 8.6.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-bindinginitialization))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BindingInitialization for declarations and supported destructuring | Supported with Limitations | [`Variable_ArrayDestructuring_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ArrayDestructuring_Basic.js)<br>[`Variable_ObjectDestructuring_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Basic.js)<br>[`Classes_ClassConstructor_ParameterDestructuring.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_ParameterDestructuring.js) | Identifier/object/array/rest/default binding paths are implemented for supported contexts, with unsupported binding-pattern forms rejected by validation/parsing. |

### 8.6.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-initializeboundname))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| InitializeBoundName behavior (writes/const restrictions) | Supported with Limitations | [`Variable_ConstReassignmentError.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ConstReassignmentError.js)<br>[`Variable_DestructuringAssignment_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_DestructuringAssignment_Basic.js) | Binding writes are lowered with scope-aware storage operations and enforce const write restrictions in supported assignment/destructuring paths. |

### 8.6.3 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-iteratorbindinginitialization))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IteratorBindingInitialization in for-of and destructuring flows | Supported with Limitations | [`ControlFlow_ForOf_Let_Destructuring_PerIterationBinding.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Let_Destructuring_PerIterationBinding.js)<br>[`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js) | Iterator-driven binding initialization is implemented in lowered for-of/for-await and destructuring paths, with subset-based coverage of exotic iterator semantics. |

### 8.6.4 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-assignmenttargettype))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| AssignmentTargetType checks for supported assignment targets | Supported with Limitations | `Js2IL.Tests/ValidatorTests.cs`<br>[`Variable_AssignmentTargets_MemberAndIndex.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_AssignmentTargets_MemberAndIndex.js)<br>[`Variable_DestructuringAssignment_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_DestructuringAssignment_Basic.js) | Assignment lowering/validation supports identifier, member, index, and supported destructuring targets; unsupported target forms are rejected. |

### 8.6.5 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-propname))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| PropName evaluation for object/class member keys | Supported with Limitations | [`ObjectLiteral_ComputedKey_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_Basic.js)<br>[`ObjectLiteral_ComputedKey_EvaluationOrder.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_EvaluationOrder.js) | Property-name extraction/evaluation is implemented for identifier/literal/computed object keys in supported forms; several class-specific computed/private key variants remain limited. |

