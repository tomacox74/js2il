<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 21.1: Number Objects

[Back to Section21](Section21.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-07T02:52:27Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 21.1 | Number Objects | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 21.1.1 | The Number Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-number-constructor) |
| 21.1.1.1 | Number ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-number-constructor-number-value) |
| 21.1.2 | Properties of the Number Constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-number-constructor) |
| 21.1.2.1 | Number.EPSILON | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.epsilon) |
| 21.1.2.2 | Number.isFinite ( number ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.isfinite) |
| 21.1.2.3 | Number.isInteger ( number ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-number.isinteger) |
| 21.1.2.4 | Number.isNaN ( number ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.isnan) |
| 21.1.2.5 | Number.isSafeInteger ( number ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.issafeinteger) |
| 21.1.2.6 | Number.MAX_SAFE_INTEGER | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.max_safe_integer) |
| 21.1.2.7 | Number.MAX_VALUE | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.max_value) |
| 21.1.2.8 | Number.MIN_SAFE_INTEGER | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.min_safe_integer) |
| 21.1.2.9 | Number.MIN_VALUE | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.min_value) |
| 21.1.2.10 | Number.NaN | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.nan) |
| 21.1.2.11 | Number.NEGATIVE_INFINITY | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.negative_infinity) |
| 21.1.2.12 | Number.parseFloat ( string ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.parsefloat) |
| 21.1.2.13 | Number.parseInt ( string , radix ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.parseint) |
| 21.1.2.14 | Number.POSITIVE_INFINITY | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.positive_infinity) |
| 21.1.2.15 | Number.prototype | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype) |
| 21.1.3 | Properties of the Number Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-number-prototype-object) |
| 21.1.3.1 | Number.prototype.constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.constructor) |
| 21.1.3.2 | Number.prototype.toExponential ( fractionDigits ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.toexponential) |
| 21.1.3.3 | Number.prototype.toFixed ( fractionDigits ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.tofixed) |
| 21.1.3.4 | Number.prototype.toLocaleString ( [ reserved1 [ , reserved2 ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.tolocalestring) |
| 21.1.3.5 | Number.prototype.toPrecision ( precision ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.toprecision) |
| 21.1.3.6 | Number.prototype.toString ( [ radix ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.tostring) |
| 21.1.3.7 | Number.prototype.valueOf ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.valueof) |
| 21.1.3.7.1 | ThisNumberValue ( value ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-thisnumbervalue) |
| 21.1.4 | Properties of Number Instances | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-number-instances) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 21.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-number-constructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| The Number Constructor | Supported with Limitations |  | `built-ins/Number/S15.7.2.1_A1.js`<br>`built-ins/Number/S15.7.2.1_A2.js`<br>`built-ins/Number/S15.7.5_A1_T01.js`<br>`built-ins/Number/S15.7.5_A1_T02.js` | Number wrapper construction is supported for the currently modeled Number object surface, including representative primitive construction and instance-shape coverage. Direct Number(value) calls remain tracked separately in 21.1.1.1, and broader constructor edge cases remain limited to the checked-in test262 slice. |

### 21.1.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-number-constructor-number-value))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number ( value ) | Supported with Limitations |  | `built-ins/Number/return-abrupt-tonumber-value.js`<br>`built-ins/Number/S15.7.1.1_A1.js`<br>`built-ins/Number/S9.3_A1_T1.js`<br>`built-ins/Number/S9.3.1_A17.js`<br>`built-ins/Number/S9.3_A2_T1.js`<br>`built-ins/Number/S9.3_A3_T1.js`<br>`built-ins/Number/S9.3_A4.1_T1.js`<br>`built-ins/Number/S9.3.1_A1.js`<br>`built-ins/Number/S9.3.1_A7.js` | Direct calls to the global Number function are lowered to TypeUtilities.ToNumber and preserve observable abrupt completions even when the converted value is discarded. Checked-in coverage now includes representative undefined, null, boolean, numeric, empty-string, decimal-string, NaN, hexadecimal-string, object-wrapper, and non-canonical infinity-string coercion cases. Wrapper construction is supported for the currently modeled Number object surface, but full Number constructor/prototype semantics remain incomplete. |

### 21.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-number-constructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Properties of the Number Constructor | Supported |  | `built-ins/Number/S15.7.3_A1.js`<br>`built-ins/Number/S15.7.3_A2.js`<br>`built-ins/Number/S15.7.3_A3.js`<br>`built-ins/Number/S15.7.3_A4.js` | The Number constructor exposes the covered standard own properties with representative descriptor and existence coverage. |

### 21.1.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-number.epsilon))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.EPSILON | Supported |  | `built-ins/Number/EPSILON.js` | Exposed on the Number constructor with non-writable, non-enumerable, non-configurable data-property attributes. |

### 21.1.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-number.isfinite))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.isFinite(number) | Supported | `tests/Jroc.Test262.Tests/built-ins/Number/isFinite/ExecutionTests.cs` | `test/built-ins/Number/isFinite/arg-is-not-number.js`<br>`test/built-ins/Number/isFinite/finite-numbers.js`<br>`test/built-ins/Number/isFinite/infinity.js`<br>`test/built-ins/Number/isFinite/length.js`<br>`test/built-ins/Number/isFinite/name.js`<br>`test/built-ins/Number/isFinite/nan.js`<br>`test/built-ins/Number/isFinite/not-a-constructor.js`<br>`test/built-ins/Number/isFinite/prop-desc.js` | Number.isFinite is exposed on the Number constructor with the expected callable metadata and property descriptor, does not coerce non-number arguments, rejects infinities and NaN, and accepts representative finite Number values. |

### 21.1.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-number.isinteger))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.isInteger(number) | Supported with Limitations | [`nan.js`](../../../tests/Jroc.Test262.Tests/built-ins/Number/isInteger/JavaScript/nan.js) | `test/built-ins/Number/isInteger/nan.js` | Checked-in coverage now includes representative NaN classification for Number.isInteger. Broader Number.isInteger metadata and argument-shape coverage remain limited to the current test262 slice. |

### 21.1.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-number.isnan))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.isNaN(number) | Supported | `tests/Jroc.Test262.Tests/built-ins/Number/isNaN/ExecutionTests.cs` | `test/built-ins/Number/isNaN/arg-is-not-number.js`<br>`test/built-ins/Number/isNaN/length.js`<br>`test/built-ins/Number/isNaN/name.js`<br>`test/built-ins/Number/isNaN/nan.js`<br>`test/built-ins/Number/isNaN/not-a-constructor.js`<br>`test/built-ins/Number/isNaN/not-nan.js`<br>`test/built-ins/Number/isNaN/prop-desc.js` | Number.isNaN is exposed on the Number constructor with the expected callable metadata and property descriptor, does not coerce non-number arguments, and returns true only for Number NaN values. |

### 21.1.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-number.issafeinteger))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.isSafeInteger(number) | Supported | `tests/Jroc.Test262.Tests/built-ins/Number/isSafeInteger/ExecutionTests.cs` | `built-ins/Number/isSafeInteger/not-safe-integer.js`<br>`built-ins/Number/isSafeInteger/safe-integers.js` | Number.isSafeInteger accepts safe integral Number values and rejects NaN, infinities, fractional values, unsafe integers, and non-number inputs without coercion. |

### 21.1.2.6 ([tc39.es](https://tc39.es/ecma262/#sec-number.max_safe_integer))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.MAX_SAFE_INTEGER | Supported |  | `built-ins/Number/MAX_SAFE_INTEGER.js` | Exposed on the Number constructor as a non-writable, non-enumerable, non-configurable data property with value 9007199254740991. |

### 21.1.2.7 ([tc39.es](https://tc39.es/ecma262/#sec-number.max_value))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.MAX_VALUE | Supported |  | `built-ins/Number/MAX_VALUE/S15.7.3.2_A2.js`<br>`built-ins/Number/MAX_VALUE/S15.7.3.2_A3.js`<br>`built-ins/Number/MAX_VALUE/value.js` | Exposed on the Number constructor with the expected maximum finite double value and non-writable, non-enumerable, non-configurable data-property attributes. |

### 21.1.2.8 ([tc39.es](https://tc39.es/ecma262/#sec-number.min_safe_integer))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.MIN_SAFE_INTEGER | Supported |  | `built-ins/Number/MIN_SAFE_INTEGER.js` | Exposed on the Number constructor as a non-writable, non-enumerable, non-configurable data property with value -9007199254740991. |

### 21.1.2.9 ([tc39.es](https://tc39.es/ecma262/#sec-number.min_value))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.MIN_VALUE | Supported |  | `built-ins/Number/MIN_VALUE/S15.7.3.3_A3.js`<br>`built-ins/Number/MIN_VALUE/value.js` | Exposed on the Number constructor with the expected smallest positive double value and non-configurable data-property attribute coverage. |

### 21.1.2.10 ([tc39.es](https://tc39.es/ecma262/#sec-number.nan))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.NaN | Supported |  | `built-ins/Number/NaN.js` | Exposed on the Number constructor with the expected NaN value. |

### 21.1.2.11 ([tc39.es](https://tc39.es/ecma262/#sec-number.negative_infinity))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.NEGATIVE_INFINITY | Supported |  | `built-ins/Number/NEGATIVE_INFINITY/value.js` | Exposed on the Number constructor with the expected negative infinity numeric value. |

### 21.1.2.12 ([tc39.es](https://tc39.es/ecma262/#sec-number.parsefloat))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.parseFloat | Supported |  | `built-ins/Number/parseFloat.js` | Exposed as the same function value as the global parseFloat property with writable, non-enumerable, configurable data-property attributes. |

### 21.1.2.13 ([tc39.es](https://tc39.es/ecma262/#sec-number.parseint))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.parseInt | Supported |  | `built-ins/Number/parseInt.js` | Exposed as the same function value as the global parseInt property with writable, non-enumerable, configurable data-property attributes. |

### 21.1.2.14 ([tc39.es](https://tc39.es/ecma262/#sec-number.positive_infinity))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.POSITIVE_INFINITY | Supported |  | `built-ins/Number/POSITIVE_INFINITY/value.js` | Exposed on the Number constructor with the expected positive infinity numeric value. |

### 21.1.2.15 ([tc39.es](https://tc39.es/ecma262/#sec-number.prototype))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.prototype | Supported |  | `built-ins/Number/prototype/15.7.3.1-2.js`<br>`built-ins/Number/prototype/prop-desc.js`<br>`built-ins/Number/prototype/S15.7.4_A1.js`<br>`built-ins/Number/prototype/S15.7.4_A2.js` | Number.prototype is exposed as the initial Number prototype object, has the expected Number.prototype property descriptor on the constructor, inherits from Object.prototype, and carries [[NumberData]] value +0 for prototype method calls. |

### 21.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-number-prototype-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Properties of the Number Prototype Object | Supported with Limitations |  | `built-ins/Number/prototype/S15.7.4_A1.js`<br>`built-ins/Number/prototype/S15.7.4_A2.js`<br>`built-ins/Number/prototype/S15.7.4_A3.1.js`<br>`built-ins/Number/prototype/toExponential/prop-desc.js`<br>`built-ins/Number/prototype/toFixed/prop-desc.js`<br>`built-ins/Number/prototype/toLocaleString/prop-desc.js`<br>`built-ins/Number/prototype/toPrecision/prop-desc.js`<br>`built-ins/Number/prototype/toString/prop-desc.js`<br>`built-ins/Number/prototype/valueOf/prop-desc.js` | The covered Number.prototype object shape and prototype method descriptors are implemented. Formatting and locale behavior for some methods remains limited to the currently ported test262 slice and is tracked by method-specific entries. |

### 21.1.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-number.prototype.constructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.prototype.constructor | Supported |  | `built-ins/Number/prototype/constructor.js`<br>`built-ins/Number/prototype/S15.7.4_A3.1.js` | Number.prototype.constructor points to the Number intrinsic and has the expected writable, non-enumerable, configurable descriptor. |

### 21.1.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-number.prototype.toexponential))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.prototype.toExponential(fractionDigits) | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Number/prototype/toExponential/ExecutionTests.cs` | `built-ins/Number/prototype/toExponential/prop-desc.js`<br>`built-ins/Number/prototype/toExponential/return-values.js` | Implemented with expected descriptor metadata, non-constructor behavior through builtin-function initialization, representative return values, special NaN/infinity handling, and RangeError validation for digits outside 0..100. Decimal formatting precision remains limited to the current implementation and test262 slice. |

### 21.1.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-number.prototype.tofixed))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.prototype.toFixed(fractionDigits) | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Number/prototype/toFixed/ExecutionTests.cs` | `built-ins/Number/prototype/toFixed/prop-desc.js`<br>`built-ins/Number/prototype/toFixed/return-type.js` | Implemented with expected descriptor metadata, string return behavior, special NaN/infinity handling, and RangeError validation for digits outside 0..100. Exhaustive ECMAScript decimal-rounding edge cases are not yet fully tracked. |

### 21.1.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-number.prototype.tolocalestring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.prototype.toLocaleString([ reserved1 [, reserved2 ] ]) | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Number/prototype/toLocaleString/ExecutionTests.cs` | `built-ins/Number/prototype/toLocaleString/not-a-constructor.js`<br>`built-ins/Number/prototype/toLocaleString/prop-desc.js` | Exposed as a non-constructible builtin with the expected property descriptor. Locale/options-sensitive formatting is intentionally limited today and currently delegates to the invariant JavaScript number string conversion. |

### 21.1.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-number.prototype.toprecision))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.prototype.toPrecision(precision) | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Number/prototype/toPrecision/ExecutionTests.cs` | `built-ins/Number/prototype/toPrecision/prop-desc.js`<br>`built-ins/Number/prototype/toPrecision/return-values.js` | Implemented with expected descriptor metadata, representative precision formatting, undefined-precision fallback, special NaN/infinity handling, and RangeError validation for precision outside 1..100. Exhaustive ECMAScript decimal-rounding edge cases are not yet fully tracked. |

### 21.1.3.6 ([tc39.es](https://tc39.es/ecma262/#sec-number.prototype.tostring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.prototype.toString([ radix ]) | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Number/prototype/toString/ExecutionTests.cs` | `built-ins/Number/prototype/toString/not-a-constructor.js`<br>`built-ins/Number/prototype/toString/prop-desc.js`<br>`built-ins/Number/prototype/toString/S15.7.4.2_A1_T01.js` | Implemented for the covered default/decimal conversion cases, including Number.prototype itself, wrapper values, NaN, and infinities; exposed as a non-constructible builtin with expected property descriptor. Non-decimal radix formatting remains limited to existing runtime conversion support. |

### 21.1.3.7 ([tc39.es](https://tc39.es/ecma262/#sec-number.prototype.valueof))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number.prototype.valueOf() | Supported | `tests/Jroc.Test262.Tests/built-ins/Number/prototype/valueOf/ExecutionTests.cs` | `built-ins/Number/prototype/valueOf/not-a-constructor.js`<br>`built-ins/Number/prototype/valueOf/prop-desc.js`<br>`built-ins/Number/prototype/valueOf/S15.7.4.4_A2_T01.js` | Returns the wrapped NumberData value for Number primitives/wrappers and rejects incompatible receivers with TypeError; exposed as a non-constructible builtin with expected property descriptor. |

### 21.1.3.7.1 ([tc39.es](https://tc39.es/ecma262/#sec-thisnumbervalue))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ThisNumberValue(value) | Supported |  | `built-ins/Number/prototype/S15.7.4_A1.js`<br>`built-ins/Number/prototype/toString/S15.7.4.2_A1_T01.js`<br>`built-ins/Number/prototype/valueOf/S15.7.4.4_A2_T01.js` | The shared Number prototype receiver extraction accepts Number primitives, Number wrapper objects, and Number.prototype itself, and throws TypeError for incompatible receivers. |

### 21.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-number-instances))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Properties of Number Instances | Supported |  | `built-ins/Number/S15.7.5_A1_T01.js`<br>`built-ins/Number/S15.7.5_A1_T02.js` | Constructed Number instances expose the covered Number object shape and prototype relationship. |

