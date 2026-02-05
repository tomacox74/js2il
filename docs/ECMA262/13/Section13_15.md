<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.15: Assignment Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.15 | Assignment Operators | Supported | [tc39.es](https://tc39.es/ecma262/#sec-assignment-operators) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.15.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-assignment-operators-static-semantics-early-errors) |
| 13.15.2 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-assignment-operators-runtime-semantics-evaluation) |
| 13.15.3 | ApplyStringOrNumericBinaryOperator ( lVal , opText , rVal ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-applystringornumericbinaryoperator) |
| 13.15.4 | EvaluateStringOrNumericBinaryExpression ( leftOperand , opText , rightOperand ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-evaluatestringornumericbinaryexpression) |
| 13.15.5 | Destructuring Assignment | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-destructuring-assignment) |
| 13.15.5.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-destructuring-assignment-static-semantics-early-errors) |
| 13.15.5.2 | Runtime Semantics: DestructuringAssignmentEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-destructuringassignmentevaluation) |
| 13.15.5.3 | Runtime Semantics: PropertyDestructuringAssignmentEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-propertydestructuringassignmentevaluation) |
| 13.15.5.4 | Runtime Semantics: RestDestructuringAssignmentEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-restdestructuringassignmentevaluation) |
| 13.15.5.5 | Runtime Semantics: IteratorDestructuringAssignmentEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-iteratordestructuringassignmentevaluation) |
| 13.15.5.6 | Runtime Semantics: KeyedDestructuringAssignmentEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-keyeddestructuringassignmentevaluation) |

## Support

Feature-level support tracking with test script references.

### 13.15.1 ([tc39.es](https://tc39.es/ecma262/#sec-assignment-operators-static-semantics-early-errors))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Computed property/index assignment (obj[index] = value; arr[i] = value) | Supported | [`Variable_AssignmentTargets_MemberAndIndex.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_AssignmentTargets_MemberAndIndex.js) | Supports computed MemberExpression assignment via JavaScriptRuntime.Object.SetItem(obj, index, value). Works for object literals (ExpandoObject), JavaScriptRuntime.Array index writes (including extending length), and Int32Array index writes. |
| Destructuring assignment ({a} = obj; [a] = arr) | Supported | [`Variable_DestructuringAssignment_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_DestructuringAssignment_Basic.js) | Supports destructuring assignment expressions with object/array patterns. Applies defaults when extracted values are undefined and supports rest elements/properties. The overall assignment expression evaluates to the RHS value. |
| Property assignment on objects (obj.prop = value) | Supported | [`ObjectLiteral_PropertyAssign.js`](../../../Js2IL.Tests/Literals/JavaScript/ObjectLiteral_PropertyAssign.js)<br>[`Variable_AssignmentTargets_MemberAndIndex.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_AssignmentTargets_MemberAndIndex.js)<br>[`CommonJS_Module_Exports_ChainedAssignment.js`](../../../Js2IL.Tests/CommonJS/JavaScript/CommonJS_Module_Exports_ChainedAssignment.js)<br>[`CommonJS_Module_Exports_ChainedAssignment_Lib.js`](../../../Js2IL.Tests/CommonJS/JavaScript/CommonJS_Module_Exports_ChainedAssignment_Lib.js) | Supports assignment to non-computed member targets and returns the assigned value (JavaScript assignment-expression semantics), including chained assignment patterns where the result is consumed (e.g., exports = module.exports = {...}; fixes #558). Emitted via JavaScriptRuntime.Object.SetItem(obj, key, value) with a string key. Supports ExpandoObject (object literals) and reflection-backed host objects; strings are treated as immutable and ignore writes. |

### 13.15.2 ([tc39.es](https://tc39.es/ecma262/#sec-assignment-operators-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Arithmetic compound assignments (-=, *=, /=, %=, **=) | Supported | [`CompoundAssignment_SubtractionAssignment.js`](../../../Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_SubtractionAssignment.js)<br>[`CompoundAssignment_MultiplicationAssignment.js`](../../../Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_MultiplicationAssignment.js)<br>[`CompoundAssignment_DivisionAssignment.js`](../../../Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_DivisionAssignment.js)<br>[`CompoundAssignment_RemainderAssignment.js`](../../../Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_RemainderAssignment.js)<br>[`CompoundAssignment_ExponentiationAssignment.js`](../../../Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_ExponentiationAssignment.js) | Arithmetic operations work on double values. Exponentiation (**=) uses System.Math.Pow. All operators properly preserve JavaScript numeric semantics. |
| Bitwise compound assignments (\|=, &=, ^=, <<=, >>=, >>>=) | Supported | [`CompoundAssignment_BitwiseOrAssignment.js`](../../../Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_BitwiseOrAssignment.js)<br>[`CompoundAssignment_BitwiseAndAssignment.js`](../../../Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_BitwiseAndAssignment.js)<br>[`CompoundAssignment_BitwiseXorAssignment.js`](../../../Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_BitwiseXorAssignment.js)<br>[`CompoundAssignment_LeftShiftAssignment.js`](../../../Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_LeftShiftAssignment.js)<br>[`CompoundAssignment_RightShiftAssignment.js`](../../../Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_RightShiftAssignment.js)<br>[`CompoundAssignment_UnsignedRightShiftAssignment.js`](../../../Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_UnsignedRightShiftAssignment.js) | Emits load-convert-operate-convert-store pattern with int32 operations. Operands are converted to int32, operation applied, result converted back to double for storage. |
| Compound assignment += with strings | Supported | [`String_PlusEquals_Append.js`](../../../Js2IL.Tests/String/JavaScript/String_PlusEquals_Append.js) | += on identifiers uses JavaScriptRuntime.Operators.Add for JS coercion and stores back to the same binding; validated by generator snapshot. |

