<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.15: Assignment Operators

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-23T08:21:08Z

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
| 13.15.5 | Destructuring Assignment | Supported | [tc39.es](https://tc39.es/ecma262/#sec-destructuring-assignment) |
| 13.15.5.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-destructuring-assignment-static-semantics-early-errors) |
| 13.15.5.2 | Runtime Semantics: DestructuringAssignmentEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-destructuringassignmentevaluation) |
| 13.15.5.3 | Runtime Semantics: PropertyDestructuringAssignmentEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-propertydestructuringassignmentevaluation) |
| 13.15.5.4 | Runtime Semantics: RestDestructuringAssignmentEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-restdestructuringassignmentevaluation) |
| 13.15.5.5 | Runtime Semantics: IteratorDestructuringAssignmentEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-iteratordestructuringassignmentevaluation) |
| 13.15.5.6 | Runtime Semantics: KeyedDestructuringAssignmentEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-keyeddestructuringassignmentevaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.15.1 ([tc39.es](https://tc39.es/ecma262/#sec-assignment-operators-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Computed property/index assignment (obj[index] = value; arr[i] = value) | Supported | [`Variable_AssignmentTargets_MemberAndIndex.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_AssignmentTargets_MemberAndIndex.js) |  | Supports computed MemberExpression assignment via JavaScriptRuntime.Object.SetItem(obj, index, value). Works for object literals (ExpandoObject), JavaScriptRuntime.Array index writes (including extending length), and Int32Array index writes. |
| Destructuring assignment ({a} = obj; [a] = arr) | Supported | [`Variable_DestructuringAssignment_Basic.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_DestructuringAssignment_Basic.js) |  | Supports destructuring assignment expressions with object/array patterns. Applies defaults when extracted values are undefined and supports rest elements/properties. The overall assignment expression evaluates to the RHS value. |
| Property assignment on objects (obj.prop = value) | Supported | [`ObjectLiteral_PropertyAssign.js`](../../../tests/Jroc.Tests/Literals/JavaScript/ObjectLiteral_PropertyAssign.js)<br>[`Variable_AssignmentTargets_MemberAndIndex.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_AssignmentTargets_MemberAndIndex.js)<br>[`CommonJS_Module_Exports_ChainedAssignment.js`](../../../tests/Jroc.Tests/CommonJS/JavaScript/CommonJS_Module_Exports_ChainedAssignment.js)<br>[`CommonJS_Module_Exports_ChainedAssignment_Lib.js`](../../../tests/Jroc.Tests/CommonJS/JavaScript/CommonJS_Module_Exports_ChainedAssignment_Lib.js) | `test/language/expressions/assignment/11.13.1-4-28gs.js` | Supports assignment to non-computed member targets and returns the assigned value (JavaScript assignment-expression semantics), including chained assignment patterns where the result is consumed (e.g., exports = module.exports = {...}; fixes #558). Emitted via JavaScriptRuntime.Object.SetItem(obj, key, value) with a string key. Supports ExpandoObject (object literals) and reflection-backed host objects; strings are treated as immutable and ignore writes. Strict assignment to read-only intrinsic data properties such as Math.PI throws TypeError. |

### 13.15.2 ([tc39.es](https://tc39.es/ecma262/#sec-assignment-operators-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Arithmetic compound assignments (-=, *=, /=, %=, **=) | Supported | [`CompoundAssignment_SubtractionAssignment.js`](../../../tests/Jroc.Tests/CompoundAssignment/JavaScript/CompoundAssignment_SubtractionAssignment.js)<br>[`CompoundAssignment_MultiplicationAssignment.js`](../../../tests/Jroc.Tests/CompoundAssignment/JavaScript/CompoundAssignment_MultiplicationAssignment.js)<br>[`CompoundAssignment_DivisionAssignment.js`](../../../tests/Jroc.Tests/CompoundAssignment/JavaScript/CompoundAssignment_DivisionAssignment.js)<br>[`CompoundAssignment_RemainderAssignment.js`](../../../tests/Jroc.Tests/CompoundAssignment/JavaScript/CompoundAssignment_RemainderAssignment.js)<br>[`CompoundAssignment_ExponentiationAssignment.js`](../../../tests/Jroc.Tests/CompoundAssignment/JavaScript/CompoundAssignment_ExponentiationAssignment.js) |  | Arithmetic operations work on double values. Exponentiation (**=) uses System.Math.Pow. All operators properly preserve JavaScript numeric semantics. |
| Bitwise compound assignments (\|=, &=, ^=, <<=, >>=, >>>=) | Supported | [`CompoundAssignment_BitwiseOrAssignment.js`](../../../tests/Jroc.Tests/CompoundAssignment/JavaScript/CompoundAssignment_BitwiseOrAssignment.js)<br>[`CompoundAssignment_BitwiseAndAssignment.js`](../../../tests/Jroc.Tests/CompoundAssignment/JavaScript/CompoundAssignment_BitwiseAndAssignment.js)<br>[`CompoundAssignment_BitwiseXorAssignment.js`](../../../tests/Jroc.Tests/CompoundAssignment/JavaScript/CompoundAssignment_BitwiseXorAssignment.js)<br>[`CompoundAssignment_LeftShiftAssignment.js`](../../../tests/Jroc.Tests/CompoundAssignment/JavaScript/CompoundAssignment_LeftShiftAssignment.js)<br>[`CompoundAssignment_RightShiftAssignment.js`](../../../tests/Jroc.Tests/CompoundAssignment/JavaScript/CompoundAssignment_RightShiftAssignment.js)<br>[`CompoundAssignment_UnsignedRightShiftAssignment.js`](../../../tests/Jroc.Tests/CompoundAssignment/JavaScript/CompoundAssignment_UnsignedRightShiftAssignment.js) |  | Emits load-convert-operate-convert-store pattern with int32 operations. Operands are converted to int32, operation applied, result converted back to double for storage. |
| Compound assignment += with strings | Supported | [`String_PlusEquals_Append.js`](../../../tests/Jroc.Tests/String/JavaScript/String_PlusEquals_Append.js) |  | += on identifiers uses JavaScriptRuntime.Operators.Add for JS coercion and stores back to the same binding; validated by generator snapshot. |

### 13.15.5.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-destructuringassignmentevaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| array destructuring assignment preserves abrupt-completion precedence during IteratorClose | Supported |  | `test/language/expressions/assignment/destructuring/default-expr-throws-iterator-return-get-throws.js`<br>`test/language/expressions/assignment/destructuring/target-assign-throws-iterator-return-get-throws.js` | Array destructuring assignment now consumes the iterator protocol and preserves the original throw completion when closing an iterator also throws while resolving `return`. |

### 13.15.5.5 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-iteratordestructuringassignmentevaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| iterator destructuring assignment evaluates property-reference targets in spec order | Supported |  | `test/language/expressions/assignment/destructuring/iterator-destructuring-property-reference-target-evaluation-order.js` | Iterator-based array destructuring assignment evaluates member targets before iterator stepping, but delays key coercion and writes until after the iterator result is observed, matching the spec's ordering requirements. |

### 13.15.5.6 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-keyeddestructuringassignmentevaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| computed object destructuring assignment keys and keyed target ordering | Supported |  | `test/language/expressions/assignment/destructuring/keyed-destructuring-property-reference-target-evaluation-order.js`<br>`test/language/expressions/assignment/destructuring/keyed-destructuring-property-reference-target-evaluation-order-with-bindings.js` | Computed property names in object destructuring assignments are supported, and assignment targets such as `target[targetKey]` keep the required ordering between property-name evaluation, binding resolution, default evaluation, key coercion, and the final write. |

