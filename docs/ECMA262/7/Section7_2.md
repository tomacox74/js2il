<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 7.2: Testing and Comparison Operations

[Back to Section7](Section7.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 7.2 | Testing and Comparison Operations | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-testing-and-comparison-operations) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 7.2.1 | RequireObjectCoercible ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-requireobjectcoercible) |
| 7.2.2 | IsArray ( argument ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-isarray) |
| 7.2.3 | IsCallable ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iscallable) |
| 7.2.4 | IsConstructor ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isconstructor) |
| 7.2.5 | IsExtensible ( O ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-isextensible-o) |
| 7.2.6 | IsRegExp ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isregexp) |
| 7.2.7 | Static Semantics: IsStringWellFormedUnicode ( string ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-isstringwellformedunicode) |
| 7.2.8 | SameType ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-sametype) |
| 7.2.9 | SameValue ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-samevalue) |
| 7.2.10 | SameValueZero ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-samevaluezero) |
| 7.2.11 | SameValueNonNumber ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-samevaluenonnumber) |
| 7.2.12 | IsLessThan ( x , y , LeftFirst ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-islessthan) |
| 7.2.13 | IsLooselyEqual ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-islooselyequal) |
| 7.2.14 | IsStrictlyEqual ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isstrictlyequal) |

## Support

Feature-level support tracking with test script references.

### 7.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-isarray))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IsArray | Supported | [`Array_IsArray_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_IsArray_Basic.js)<br>[`Array_AsArray_Ternary.js`](../../../Js2IL.Tests/Array/JavaScript/Array_AsArray_Ternary.js) | Implemented by JavaScriptRuntime.Array.isArray for JS array instances. |

### 7.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-iscallable))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IsCallable checks in invocation paths | Supported with Limitations | [`Function_CallViaVariable_Reassignment.js`](../../../Js2IL.Tests/Function/JavaScript/Function_CallViaVariable_Reassignment.js)<br>[`Function_Apply_Basic.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Apply_Basic.js) | Callable detection is primarily delegate-based in runtime dispatch and intrinsic paths. |

### 7.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-isconstructor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IsConstructor checks for new-expression paths | Supported with Limitations | [`Classes_DeclareEmptyClass.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js)<br>[`BinaryOperator_InstanceOf_Basic.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_InstanceOf_Basic.js) | Dynamic construction supports delegate/type/Construct-member shapes used by JS2IL; full constructability semantics are not complete. |

### 7.2.6 ([tc39.es](https://tc39.es/ecma262/#sec-isregexp))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IsRegExp classification in RegExp-aware operations | Supported with Limitations | [`IntrinsicCallables_RegExp_Callable_CreatesRegex.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Callable_CreatesRegex.js)<br>[`String_RegExp_Exec_LastIndex_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_RegExp_Exec_LastIndex_Global.js) | RegExp-aware behavior is implemented for JavaScriptRuntime.RegExp call sites; Symbol.match override semantics are not fully modeled. |

### 7.2.9 ([tc39.es](https://tc39.es/ecma262/#sec-samevalue))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| SameValue | Supported | [`Object_Is_SameValue.js`](../../../Js2IL.Tests/Object/JavaScript/Object_Is_SameValue.js) | Implemented by Operators.SameValue and exposed via Object.is. |

### 7.2.10 ([tc39.es](https://tc39.es/ecma262/#sec-samevaluezero))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| SameValueZero | Supported | [`Array_SearchOps_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_SearchOps_Basic.js) | Implemented in array includes/search paths and runtime helper comparisons. |

### 7.2.12 ([tc39.es](https://tc39.es/ecma262/#sec-islessthan))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IsLessThan relational comparison | Supported with Limitations | [`BinaryOperator_LessThan.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LessThan.js)<br>[`BinaryOperator_GreaterThan.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_GreaterThan.js)<br>[`BinaryOperator_LessThanOrEqual.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LessThanOrEqual.js)<br>[`BinaryOperator_GreaterThanOrEqual.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_GreaterThanOrEqual.js) | Implements numeric and BigInt relational behavior for supported operand kinds; full coercion-order edge cases are incomplete. |

### 7.2.13 ([tc39.es](https://tc39.es/ecma262/#sec-islooselyequal))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IsLooselyEqual (==) | Supported with Limitations | [`BinaryOperator_Equal.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_Equal.js)<br>[`BinaryOperator_NotEqual.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_NotEqual.js)<br>[`BinaryOperator_EqualBoolean.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_EqualBoolean.js) | Covers core loose-equality coercions (including nullish and numeric cases); complete spec edge-case parity is not yet implemented. |

### 7.2.14 ([tc39.es](https://tc39.es/ecma262/#sec-isstrictlyequal))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IsStrictlyEqual (===) | Supported with Limitations | [`BinaryOperator_StrictEqualCapturedVariable.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_StrictEqualCapturedVariable.js) | Implements strict-equality behavior across supported runtime representations, with CLR/JS representation normalization in key paths. |

