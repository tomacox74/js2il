<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 7.1: Type Conversion

[Back to Section7](Section7.md) | [Back to Index](../Index.md)

Type conversion in JS2IL is implemented on an as-needed basis for supported language features and intrinsics. Some conversions are partial/minimal implementations intended to support specific call sites (e.g., BigInt(value)).

| Clause | Title | Status | Link |
|---:|---|---|---|
| 7.1 | Type Conversion | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-type-conversion) |

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
| 7.1.4.1.3 | RoundMVResult ( n ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-roundmvresult) |
| 7.1.5 | ToIntegerOrInfinity ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tointegerorinfinity) |
| 7.1.6 | ToInt32 ( argument ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-toint32) |
| 7.1.7 | ToUint32 ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-touint32) |
| 7.1.8 | ToInt16 ( argument ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-toint16) |
| 7.1.9 | ToUint16 ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-touint16) |
| 7.1.10 | ToInt8 ( argument ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-toint8) |
| 7.1.11 | ToUint8 ( argument ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-touint8) |
| 7.1.12 | ToUint8Clamp ( argument ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-touint8clamp) |
| 7.1.13 | ToBigInt ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tobigint) |
| 7.1.14 | StringToBigInt ( str ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-stringtobigint) |
| 7.1.14.1 | StringIntegerLiteral Grammar | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-stringintegerliteral-grammar) |
| 7.1.14.2 | Runtime Semantics: MV | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-mv-for-stringintegerliteral) |
| 7.1.15 | ToBigInt64 ( argument ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-tobigint64) |
| 7.1.16 | ToBigUint64 ( argument ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-tobiguint64) |
| 7.1.17 | ToString ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tostring) |
| 7.1.18 | ToObject ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-toobject) |
| 7.1.19 | ToPropertyKey ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-topropertykey) |
| 7.1.20 | ToLength ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tolength) |
| 7.1.21 | CanonicalNumericIndexString ( argument ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-canonicalnumericindexstring) |
| 7.1.22 | ToIndex ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-toindex) |

## Support

Feature-level support tracking with test script references.

### 7.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-toboolean))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToBoolean coercion (truthiness) | Supported with Limitations | [`PrimitiveConversion_Boolean_Callable.js`](../../../Js2IL.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Boolean_Callable.js)<br>[`ControlFlow_If_Truthiness.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_Truthiness.js) | Implements core truthiness for primitives, null/undefined, BigInt, and objects via TypeUtilities.ToBoolean; host-specific edge cases are not modeled. |

### 7.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-tonumber))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToNumber coercion | Supported with Limitations | [`PrimitiveConversion_Number_Callable.js`](../../../Js2IL.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Number_Callable.js)<br>[`IntrinsicCallables_ParseFloat_IsFinite_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_ParseFloat_IsFinite_Basic.js) | Supports common primitive conversions including trimmed decimal/hex strings, booleans, null, and undefined; does not implement full StringNumericLiteral grammar parity. |

### 7.1.6 ([tc39.es](https://tc39.es/ecma262/#sec-toint32))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToInt32 coercion | Supported | [`Int32Array_Wrapping_Semantics.js`](../../../Js2IL.Tests/TypedArray/JavaScript/Int32Array_Wrapping_Semantics.js) | Implements modulo-2^32 wrapping and signed interpretation used by bitwise and typed-array paths. |

### 7.1.7 ([tc39.es](https://tc39.es/ecma262/#sec-touint32))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToUint32 coercion in unsigned-shift paths | Supported with Limitations | [`BinaryOperator_UnsignedRightShiftNumberNumber.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_UnsignedRightShiftNumberNumber.js) | Implemented where required by >>> semantics; not exposed as a standalone abstract-operation helper. |

### 7.1.9 ([tc39.es](https://tc39.es/ecma262/#sec-touint16))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToUint16 coercion for String.fromCharCode | Supported with Limitations | [`Array_Slice_FromCharCode_Apply.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Slice_FromCharCode_Apply.js) | Applies ToUint16-style truncation/masking for fromCharCode arguments. |

### 7.1.13 ([tc39.es](https://tc39.es/ecma262/#sec-tobigint))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToBigInt (minimal, via BigInt(value)) | Supported with Limitations | [`IntrinsicCallables_BigInt_Callable_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_BigInt_Callable_Basic.js) | Implemented to support current BigInt(value) callable behavior; full spec conversion precedence and all edge cases are not yet implemented. |

### 7.1.14 ([tc39.es](https://tc39.es/ecma262/#sec-stringtobigint))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| StringToBigInt (minimal decimal parsing) | Supported with Limitations | [`IntrinsicCallables_BigInt_Callable_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_BigInt_Callable_Basic.js) | Supports basic decimal string parsing used by BigInt(value), but does not yet implement the full StringIntegerLiteral grammar variants. |

### 7.1.17 ([tc39.es](https://tc39.es/ecma262/#sec-tostring))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToString coercion | Supported with Limitations | [`PrimitiveConversion_String_Callable.js`](../../../Js2IL.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_String_Callable.js) | Covers primitives, arrays, and common object conversion through DotNet2JSConversions; full @@toPrimitive/valueOf precedence is incomplete. |

### 7.1.18 ([tc39.es](https://tc39.es/ecma262/#sec-toobject))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToObject coercion (Object(value) callable path) | Supported with Limitations | [`IntrinsicCallables_Object_Callable_ReturnsObject.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_Object_Callable_ReturnsObject.js) | Object(value) callable behavior is implemented minimally; full wrapper object/internal slot fidelity is incomplete. |

### 7.1.19 ([tc39.es](https://tc39.es/ecma262/#sec-topropertykey))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToPropertyKey conversion for computed property access | Supported with Limitations | [`ObjectLiteral_ComputedKey_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_Basic.js)<br>[`ObjectLiteral_ComputedKey_EvaluationOrder.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_EvaluationOrder.js) | Computed keys are normalized to strings, with symbol keys encoded to stable internal IDs rather than full symbol-keyed property semantics. |

### 7.1.20 ([tc39.es](https://tc39.es/ecma262/#sec-tolength))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToLength-style coercion in array and typed-array paths | Supported with Limitations | [`Array_New_Length.js`](../../../Js2IL.Tests/Array/JavaScript/Array_New_Length.js)<br>[`Array_Length_Set_Fractional_ThrowsRangeError.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Length_Set_Fractional_ThrowsRangeError.js)<br>[`Int32Array_Construct_Length.js`](../../../Js2IL.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js) | Length coercion is implemented in key runtime call sites with truncation/clamping behavior; not centralized as a full standalone abstract operation. |

### 7.1.21 ([tc39.es](https://tc39.es/ecma262/#sec-canonicalnumericindexstring))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| CanonicalNumericIndexString in indexed access checks | Supported with Limitations | [`Array_Canonical_Index_StringKeys.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Canonical_Index_StringKeys.js)<br>[`Int32Array_Fractional_Index_NoOp.js`](../../../Js2IL.Tests/TypedArray/JavaScript/Int32Array_Fractional_Index_NoOp.js) | Canonical decimal index strings are recognized for implemented array/typed-array indexed paths; broader exotic object semantics remain incomplete. |

### 7.1.22 ([tc39.es](https://tc39.es/ecma262/#sec-toindex))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToIndex in typed-array indexing paths | Supported with Limitations | [`Int32Array_NaN_Index_NoOp.js`](../../../Js2IL.Tests/TypedArray/JavaScript/Int32Array_NaN_Index_NoOp.js)<br>[`Int32Array_Infinity_Index_NoOp.js`](../../../Js2IL.Tests/TypedArray/JavaScript/Int32Array_Infinity_Index_NoOp.js)<br>[`Int32Array_Index_Assign.js`](../../../Js2IL.Tests/TypedArray/JavaScript/Int32Array_Index_Assign.js) | Index coercion handles finite integer indexes and ignores NaN, Infinity, and fractional indexes in the current typed-array implementation. |

