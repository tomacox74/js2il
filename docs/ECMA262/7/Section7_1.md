<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 7.1: Type Conversion

[Back to Section7](Section7.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-07T18:47:23Z

Type conversion in JROC is implemented on an as-needed basis for supported language features and intrinsics. Some conversions are partial/minimal implementations intended to support specific call sites (e.g., BigInt(value)).

| Clause | Title | Status | Link |
|---:|---|---|---|
| 7.1 | Type Conversion | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-type-conversion) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 7.1.1 | ToPrimitive ( input [ , preferredType ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-toprimitive) |
| 7.1.1.1 | OrdinaryToPrimitive ( O , hint ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ordinarytoprimitive) |
| 7.1.2 | ToBoolean ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-toboolean) |
| 7.1.3 | ToNumeric ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tonumeric) |
| 7.1.4 | ToNumber ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tonumber) |
| 7.1.4.1 | ToNumber Applied to the String Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tonumber-applied-to-the-string-type) |
| 7.1.4.1.1 | StringToNumber ( str ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-stringtonumber) |
| 7.1.4.1.2 | Runtime Semantics: StringNumericValue | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-stringnumericvalue) |
| 7.1.4.1.3 | RoundMVResult ( n ) | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-roundmvresult) |
| 7.1.5 | ToIntegerOrInfinity ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tointegerorinfinity) |
| 7.1.6 | ToInt32 ( argument ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-toint32) |
| 7.1.7 | ToUint32 ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-touint32) |
| 7.1.8 | ToInt16 ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-toint16) |
| 7.1.9 | ToUint16 ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-touint16) |
| 7.1.10 | ToInt8 ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-toint8) |
| 7.1.11 | ToUint8 ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-touint8) |
| 7.1.12 | ToUint8Clamp ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-touint8clamp) |
| 7.1.13 | ToBigInt ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tobigint) |
| 7.1.14 | StringToBigInt ( str ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-stringtobigint) |
| 7.1.14.1 | StringIntegerLiteral Grammar | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-stringintegerliteral-grammar) |
| 7.1.14.2 | Runtime Semantics: MV | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-mv-for-stringintegerliteral) |
| 7.1.15 | ToBigInt64 ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tobigint64) |
| 7.1.16 | ToBigUint64 ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tobiguint64) |
| 7.1.17 | ToString ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tostring) |
| 7.1.18 | ToObject ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-toobject) |
| 7.1.19 | ToPropertyKey ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-topropertykey) |
| 7.1.20 | ToLength ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tolength) |
| 7.1.21 | CanonicalNumericIndexString ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-canonicalnumericindexstring) |
| 7.1.22 | ToIndex ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-toindex) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 7.1 ([tc39.es](https://tc39.es/ecma262/#sec-type-conversion))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Type conversion across numbers, BigInt, typed arrays, and key/index coercion | Supported with Limitations | [`PrimitiveConversion_Number_Callable.js`](../../../tests/Jroc.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Number_Callable.js)<br>[`TypedArray_SignedAndClamped_ConversionSemantics.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/TypedArray_SignedAndClamped_ConversionSemantics.js)<br>`tests/Jroc.Test262.Tests/built-ins/BigInt/ExecutionTests.cs` |  | The runtime now covers the commonly exercised conversion surfaces for numbers, BigInt, typed-array element writes, property keys, and index coercion. Remaining gaps are concentrated in full object-coercion precedence, uncommon grammar corners, and broader typed-array families that are still only partially implemented. |

### 7.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-toprimitive))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| comparison/equality coercion paths that rely on ToPrimitive | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/equals/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/greater-than/PortExpressionsBatchExecutionTests.cs` |  | The covered comparison and equality cases now follow the expected ToPrimitive-driven coercion paths for the current test262 matrix, including the patched object/BigInt and hint-sensitive conversion scenarios. |

### 7.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-toboolean))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToBoolean coercion (truthiness) | Supported with Limitations | [`PrimitiveConversion_Boolean_Callable.js`](../../../tests/Jroc.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Boolean_Callable.js)<br>[`ControlFlow_If_Truthiness.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_If_Truthiness.js) |  | Implements core truthiness for primitives, null/undefined, BigInt, and objects via TypeUtilities.ToBoolean; host-specific edge cases are not modeled. |

### 7.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-tonumeric))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToNumeric dispatch between Number and BigInt paths | Supported with Limitations | [`PrimitiveConversion_Number_Callable.js`](../../../tests/Jroc.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Number_Callable.js)<br>`tests/Jroc.Test262.Tests/built-ins/BigInt/asIntN/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/BigInt/asUintN/ExecutionTests.cs` |  | Current numeric call sites correctly stay in the Number or BigInt domain for the covered built-ins and coercion helpers. Mixed-domain operator coverage and full object-to-BigInt coercion precedence remain incomplete. |

### 7.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-tonumber))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToNumber coercion | Supported with Limitations | [`PrimitiveConversion_Number_Callable.js`](../../../tests/Jroc.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Number_Callable.js)<br>[`IntrinsicCallables_ParseFloat_IsFinite_Basic.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_ParseFloat_IsFinite_Basic.js) |  | Supports common primitive conversions including trimmed decimal/hex strings, booleans, null, and undefined; does not implement full StringNumericLiteral grammar parity. |

### 7.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-tointegerorinfinity))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToIntegerOrInfinity in relative-index and length-like call sites | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Array/prototype/copyWithin/ExecutionTests.cs`<br>[`Int32Array_Infinity_Index_NoOp.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_Infinity_Index_NoOp.js)<br>[`Int32Array_Fractional_Index_NoOp.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_Fractional_Index_NoOp.js) |  | The shared truncation and infinity-handling behavior required by covered array and typed-array index consumers is implemented in the current runtime call sites. JROC still does not expose a single centralized helper for every abstract-operation consumer. |

### 7.1.6 ([tc39.es](https://tc39.es/ecma262/#sec-toint32))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToInt32 coercion | Supported | [`Int32Array_Wrapping_Semantics.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_Wrapping_Semantics.js) |  | Implements modulo-2^32 wrapping and signed interpretation used by bitwise and typed-array paths. |

### 7.1.7 ([tc39.es](https://tc39.es/ecma262/#sec-touint32))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToUint32 coercion in unsigned-shift paths | Supported with Limitations | [`BinaryOperator_UnsignedRightShiftNumberNumber.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_UnsignedRightShiftNumberNumber.js) |  | Implemented where required by >>> semantics; not exposed as a standalone abstract-operation helper. |

### 7.1.8 ([tc39.es](https://tc39.es/ecma262/#sec-toint16))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToInt16 coercion for typed-array and DataView element writes | Supported with Limitations | [`TypedArray_SignedAndClamped_ConversionSemantics.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/TypedArray_SignedAndClamped_ConversionSemantics.js)<br>[`DataView_SetGet_UintAndEndian.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/DataView_SetGet_UintAndEndian.js) |  | Narrowing to signed 16-bit values is implemented in the shared runtime helper used by Int16Array and DataView integer writes. Wider typed-array families that would exercise the same operation are still only partially implemented. |

### 7.1.9 ([tc39.es](https://tc39.es/ecma262/#sec-touint16))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToUint16 coercion for String.fromCharCode and DataView integer writes | Supported with Limitations | [`Array_Slice_FromCharCode_Apply.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_Slice_FromCharCode_Apply.js)<br>[`DataView_SetGet_UintAndEndian.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/DataView_SetGet_UintAndEndian.js) |  | Applies ToUint16-style truncation/masking in String.fromCharCode and the covered DataView integer-write paths. |

### 7.1.10 ([tc39.es](https://tc39.es/ecma262/#sec-toint8))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToInt8 coercion for typed-array and DataView byte writes | Supported with Limitations | [`TypedArray_SignedAndClamped_ConversionSemantics.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/TypedArray_SignedAndClamped_ConversionSemantics.js)<br>`tests/Jroc.Test262.Tests/language/statements/for-of/ExecutionTests.cs` |  | Signed 8-bit wrapping now flows through a shared helper used by Int8Array and DataView.setInt8. Coverage is currently focused on the typed-array and iteration paths implemented in JROC. |

### 7.1.11 ([tc39.es](https://tc39.es/ecma262/#sec-touint8))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToUint8 coercion for typed-array, DataView, and buffer-style byte writes | Supported with Limitations | [`TypedArray_SignedAndClamped_ConversionSemantics.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/TypedArray_SignedAndClamped_ConversionSemantics.js)<br>[`Uint8Array_Construct_ArrayLike_Buffer_Search.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Uint8Array_Construct_ArrayLike_Buffer_Search.js) |  | Unsigned 8-bit truncation is implemented via a shared runtime helper and is exercised by Uint8Array, DataView.setUint8, and byte-oriented Node helpers. Some broader typed-array families that would share this operation are still unimplemented. |

### 7.1.12 ([tc39.es](https://tc39.es/ecma262/#sec-touint8clamp))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToUint8Clamp coercion in Uint8ClampedArray element writes | Supported with Limitations | [`TypedArray_SignedAndClamped_ConversionSemantics.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/TypedArray_SignedAndClamped_ConversionSemantics.js)<br>`tests/Jroc.Test262.Tests/language/statements/for-of/ExecutionTests.cs` |  | JROC now implements the clamp-and-round-to-even behavior used by Uint8ClampedArray writes and iteration over clamped values. The supporting typed-array surface is still narrower than the full ECMAScript typed-array matrix. |

### 7.1.13 ([tc39.es](https://tc39.es/ecma262/#sec-tobigint))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToBigInt (minimal, via BigInt(value)) | Supported with Limitations | [`IntrinsicCallables_BigInt_Callable_Basic.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_BigInt_Callable_Basic.js) |  | Implemented to support current BigInt(value) callable behavior; full spec conversion precedence and all edge cases are not yet implemented. |

### 7.1.14 ([tc39.es](https://tc39.es/ecma262/#sec-stringtobigint))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| StringToBigInt parsing for decimal and prefixed integer strings | Supported with Limitations | [`IntrinsicCallables_BigInt_Callable_Basic.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_BigInt_Callable_Basic.js)<br>`tests/Jroc.Test262.Tests/built-ins/BigInt/ExecutionTests.cs` | `test/built-ins/BigInt/constructor-from-binary-string.js`<br>`test/built-ins/BigInt/constructor-from-hex-string.js`<br>`test/built-ins/BigInt/constructor-from-octal-string.js`<br>`test/built-ins/BigInt/constructor-from-string-syntax-errors.js` | BigInt(value) now covers decimal, binary, octal, and hexadecimal string forms plus the covered syntax-error cases. Object-to-BigInt coercion precedence and some abstract ToBigInt edge cases remain incomplete. |

### 7.1.15 ([tc39.es](https://tc39.es/ecma262/#sec-tobigint64))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToBigInt64 semantics via BigInt.asIntN | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/BigInt/asIntN/ExecutionTests.cs` | `test/built-ins/BigInt/asIntN/arithmetic.js`<br>`test/built-ins/BigInt/asIntN/asIntN.js` | The 64-bit signed narrowing behavior is covered through BigInt.asIntN, including negative modulo cases and large-width arithmetic. Broader BigInt typed-array surfaces and full ToBigInt coercion parity are still incomplete. |

### 7.1.16 ([tc39.es](https://tc39.es/ecma262/#sec-tobiguint64))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToBigUint64 semantics via BigInt.asUintN | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/BigInt/asUintN/ExecutionTests.cs` | `test/built-ins/BigInt/asUintN/arithmetic.js`<br>`test/built-ins/BigInt/asUintN/asUintN.js` | The 64-bit unsigned narrowing behavior is covered through BigInt.asUintN, including wraparound behavior for negative and oversized inputs. BigInt typed-array constructors and some ToBigInt coercion edges remain unsupported. |

### 7.1.17 ([tc39.es](https://tc39.es/ecma262/#sec-tostring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToString coercion | Supported with Limitations | [`PrimitiveConversion_String_Callable.js`](../../../tests/Jroc.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_String_Callable.js) |  | Covers primitives, arrays, and common object conversion through DotNet2JSConversions; full @@toPrimitive/valueOf precedence is incomplete. |

### 7.1.18 ([tc39.es](https://tc39.es/ecma262/#sec-toobject))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToObject coercion (Object(value) callable path) | Supported with Limitations | [`IntrinsicCallables_Object_Callable_ReturnsObject.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_Object_Callable_ReturnsObject.js)<br>`tests/Jroc.Test262.Tests/built-ins/Object/ExecutionTests.cs` | `test/built-ins/Object/S15.2.1.1_A1_T1.js`<br>`test/built-ins/Object/S15.2.1.1_A1_T2.js`<br>`test/built-ins/Object/S15.2.1.1_A1_T3.js` | The Object(value) callable path now covers the ordinary-object cases for no-argument, null, and undefined inputs and preserves primitive wrapper coercions for the covered scalar types. Full wrapper object/internal slot fidelity is still incomplete outside the documented coverage. |

### 7.1.19 ([tc39.es](https://tc39.es/ecma262/#sec-topropertykey))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToPropertyKey conversion for computed property access | Supported with Limitations | [`ObjectLiteral_ComputedKey_Basic.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_Basic.js)<br>[`ObjectLiteral_ComputedKey_EvaluationOrder.js`](../../../tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_EvaluationOrder.js) |  | Computed keys are normalized to strings, with symbol keys encoded to stable internal IDs rather than full symbol-keyed property semantics. |

### 7.1.20 ([tc39.es](https://tc39.es/ecma262/#sec-tolength))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToLength-style coercion in array and typed-array paths | Supported with Limitations | [`Array_New_Length.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_New_Length.js)<br>[`Array_Length_Set_Fractional_ThrowsRangeError.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_Length_Set_Fractional_ThrowsRangeError.js)<br>[`Int32Array_Construct_Length.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js) |  | Length coercion is implemented in key runtime call sites with truncation/clamping behavior; not centralized as a full standalone abstract operation. |

### 7.1.21 ([tc39.es](https://tc39.es/ecma262/#sec-canonicalnumericindexstring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| CanonicalNumericIndexString in indexed access checks | Supported with Limitations | [`Array_Canonical_Index_StringKeys.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_Canonical_Index_StringKeys.js)<br>[`Int32Array_Fractional_Index_NoOp.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_Fractional_Index_NoOp.js) |  | Canonical decimal index strings are recognized for implemented array/typed-array indexed paths; broader exotic object semantics remain incomplete. |

### 7.1.22 ([tc39.es](https://tc39.es/ecma262/#sec-toindex))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToIndex in typed-array indexing paths | Supported with Limitations | [`Int32Array_NaN_Index_NoOp.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_NaN_Index_NoOp.js)<br>[`Int32Array_Infinity_Index_NoOp.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_Infinity_Index_NoOp.js)<br>[`Int32Array_Index_Assign.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_Index_Assign.js) |  | Index coercion handles finite integer indexes and ignores NaN, Infinity, and fractional indexes in the current typed-array implementation. |

