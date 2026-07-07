<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 7.2: Testing and Comparison Operations

[Back to Section7](Section7.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-07T20:03:34Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 7.2 | Testing and Comparison Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-testing-and-comparison-operations) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 7.2.1 | RequireObjectCoercible ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-requireobjectcoercible) |
| 7.2.2 | IsArray ( argument ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-isarray) |
| 7.2.3 | IsCallable ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iscallable) |
| 7.2.4 | IsConstructor ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isconstructor) |
| 7.2.5 | IsExtensible ( O ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isextensible-o) |
| 7.2.6 | IsRegExp ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isregexp) |
| 7.2.7 | Static Semantics: IsStringWellFormedUnicode ( string ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isstringwellformedunicode) |
| 7.2.8 | SameType ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-sametype) |
| 7.2.9 | SameValue ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-samevalue) |
| 7.2.10 | SameValueZero ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-samevaluezero) |
| 7.2.11 | SameValueNonNumber ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-samevaluenonnumber) |
| 7.2.12 | IsLessThan ( x , y , LeftFirst ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-islessthan) |
| 7.2.13 | IsLooselyEqual ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-islooselyequal) |
| 7.2.14 | IsStrictlyEqual ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isstrictlyequal) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 7.2 ([tc39.es](https://tc39.es/ecma262/#sec-testing-and-comparison-operations))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Testing and comparison operations across coercion guards, identity, relational comparison, and integrity checks | Supported with Limitations | [`Object_Integrity_FreezeSeal_PreventExtensions.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Integrity_FreezeSeal_PreventExtensions.js)<br>[`String_NewApis_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_NewApis_Basic.js)<br>[`BinaryOperator_Equal.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_Equal.js)<br>[`BinaryOperator_StrictEqualCapturedVariable.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_StrictEqualCapturedVariable.js) |  | JROC covers the commonly exercised object-coercion, identity, extensibility, and comparison helpers that current language/runtime features depend on. Remaining gaps are mostly in rarely-used constructability/callability edge cases and in full abstract-operation parity for every host and proxy scenario. |

### 7.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-requireobjectcoercible))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| RequireObjectCoercible guards in object-destructuring and object built-ins | Supported with Limitations | `tests/Jroc.Test262.Tests/language/destructuring/binding/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Object/setPrototypeOf/ExecutionTests.cs` |  | The runtime consistently throws when object-requiring operations receive nullish values in the covered destructuring and Object built-in paths. Coverage is still anchored to the current object-operation call sites rather than a separately documented standalone helper surface. |

### 7.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-isarray))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsArray | Supported | [`Array_IsArray_Basic.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_IsArray_Basic.js)<br>[`Array_AsArray_Ternary.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_AsArray_Ternary.js)<br>[`15.4.3.2-0-5.js`](../../../tests/Jroc.Test262.Tests/built-ins/Array/isArray/JavaScript/15.4.3.2-0-5.js) |  | Implemented by JavaScriptRuntime.Array.isArray for JS array instances and the intrinsic Array.prototype object. |

### 7.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-iscallable))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsCallable checks in invocation paths | Supported with Limitations | [`Function_CallViaVariable_Reassignment.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_CallViaVariable_Reassignment.js)<br>[`Function_Apply_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Apply_Basic.js) |  | Callable detection is primarily delegate-based in runtime dispatch and intrinsic paths. |

### 7.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-isconstructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsConstructor checks for new-expression paths | Supported with Limitations | [`Classes_DeclareEmptyClass.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js)<br>[`BinaryOperator_InstanceOf_Basic.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_InstanceOf_Basic.js) |  | Dynamic construction supports delegate/type/Construct-member shapes used by JROC; full constructability semantics are not complete. |

### 7.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-isextensible-o))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsExtensible checks through Object.isExtensible and integrity operations | Supported with Limitations | [`Object_Integrity_FreezeSeal_PreventExtensions.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Integrity_FreezeSeal_PreventExtensions.js)<br>`tests/Jroc.Test262.Tests/language/expressions/arrow-function/ExecutionTests.cs` |  | Object.isExtensible and the shared integrity-state tracking behave correctly for the covered ordinary-object and function-like cases. Proxy extensibility traps and some host-object edge cases are not fully modeled. |

### 7.2.6 ([tc39.es](https://tc39.es/ecma262/#sec-isregexp))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsRegExp classification in RegExp-aware operations | Supported with Limitations | [`IntrinsicCallables_RegExp_Callable_CreatesRegex.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Callable_CreatesRegex.js)<br>[`String_RegExp_Exec_LastIndex_Global.js`](../../../tests/Jroc.Tests/String/JavaScript/String_RegExp_Exec_LastIndex_Global.js) |  | RegExp-aware behavior is implemented for JavaScriptRuntime.RegExp call sites; Symbol.match override semantics are not fully modeled. |

### 7.2.7 ([tc39.es](https://tc39.es/ecma262/#sec-isstringwellformedunicode))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsStringWellFormedUnicode via String.prototype.isWellFormed and toWellFormed | Supported with Limitations | [`String_NewApis_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_NewApis_Basic.js) |  | The runtime exposes well-formed Unicode detection and repair through the current String.prototype APIs, including lone-surrogate detection. Coverage is centered on the shipped string built-ins rather than the spec's internal static-semantics presentation. |

### 7.2.8 ([tc39.es](https://tc39.es/ecma262/#sec-sametype))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SameType discrimination across current runtime representations | Supported with Limitations | [`Object_Is_SameValue.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Is_SameValue.js)<br>[`BinaryOperator_StrictEqualCapturedVariable.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_StrictEqualCapturedVariable.js) |  | The runtime distinguishes current primitive, object, and BigInt shapes closely enough for the covered SameValue and strict-equality consumers. Some host-object and wrapper-object distinctions are still approximated through JROC's CLR-backed representation model. |

### 7.2.9 ([tc39.es](https://tc39.es/ecma262/#sec-samevalue))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SameValue | Supported | [`Object_Is_SameValue.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Is_SameValue.js) |  | Implemented by Operators.SameValue and exposed via Object.is. |

### 7.2.10 ([tc39.es](https://tc39.es/ecma262/#sec-samevaluezero))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SameValueZero | Supported | [`Array_SearchOps_Basic.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_SearchOps_Basic.js) |  | Implemented in array includes/search paths and runtime helper comparisons. |

### 7.2.11 ([tc39.es](https://tc39.es/ecma262/#sec-samevaluenonnumber))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SameValueNonNumber behavior for non-numeric strict identity paths | Supported with Limitations | [`Object_Is_SameValue.js`](../../../tests/Jroc.Tests/Object/JavaScript/Object_Is_SameValue.js)<br>[`BinaryOperator_StrictEqualCapturedVariable.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_StrictEqualCapturedVariable.js) |  | Non-number identity checks follow the covered object, string, boolean, nullish, and symbol-like paths used by Object.is and strict comparison. The remaining limitations come from JROC's partial wrapper-object and host-object fidelity rather than from the covered comparison logic itself. |

### 7.2.12 ([tc39.es](https://tc39.es/ecma262/#sec-islessthan))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsLessThan relational comparison | Supported with Limitations | [`BinaryOperator_LessThan.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_LessThan.js)<br>[`BinaryOperator_GreaterThan.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_GreaterThan.js)<br>[`BinaryOperator_LessThanOrEqual.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_LessThanOrEqual.js)<br>[`BinaryOperator_GreaterThanOrEqual.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_GreaterThanOrEqual.js) |  | Implements numeric and BigInt relational behavior for supported operand kinds; full coercion-order edge cases are incomplete. |

### 7.2.13 ([tc39.es](https://tc39.es/ecma262/#sec-islooselyequal))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsLooselyEqual (==) | Supported with Limitations | [`BinaryOperator_Equal.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_Equal.js)<br>[`BinaryOperator_NotEqual.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_NotEqual.js)<br>[`BinaryOperator_EqualBoolean.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_EqualBoolean.js) |  | Covers core loose-equality coercions (including nullish and numeric cases); complete spec edge-case parity is not yet implemented. |

### 7.2.14 ([tc39.es](https://tc39.es/ecma262/#sec-isstrictlyequal))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsStrictlyEqual (===) | Supported with Limitations | [`BinaryOperator_StrictEqualCapturedVariable.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_StrictEqualCapturedVariable.js) |  | Implements strict-equality behavior across supported runtime representations, with CLR/JS representation normalization in key paths. |

