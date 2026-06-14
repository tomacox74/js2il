<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 21.1: Number Objects

[Back to Section21](Section21.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-29T14:16:08Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 21.1 | Number Objects | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 21.1.1 | The Number Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number-constructor) |
| 21.1.1.1 | Number ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-number-constructor-number-value) |
| 21.1.2 | Properties of the Number Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-number-constructor) |
| 21.1.2.1 | Number.EPSILON | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.epsilon) |
| 21.1.2.2 | Number.isFinite ( number ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.isfinite) |
| 21.1.2.3 | Number.isInteger ( number ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-number.isinteger) |
| 21.1.2.4 | Number.isNaN ( number ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.isnan) |
| 21.1.2.5 | Number.isSafeInteger ( number ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.issafeinteger) |
| 21.1.2.6 | Number.MAX_SAFE_INTEGER | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.max_safe_integer) |
| 21.1.2.7 | Number.MAX_VALUE | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.max_value) |
| 21.1.2.8 | Number.MIN_SAFE_INTEGER | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.min_safe_integer) |
| 21.1.2.9 | Number.MIN_VALUE | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.min_value) |
| 21.1.2.10 | Number.NaN | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.nan) |
| 21.1.2.11 | Number.NEGATIVE_INFINITY | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.negative_infinity) |
| 21.1.2.12 | Number.parseFloat ( string ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.parsefloat) |
| 21.1.2.13 | Number.parseInt ( string , radix ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.parseint) |
| 21.1.2.14 | Number.POSITIVE_INFINITY | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number.positive_infinity) |
| 21.1.2.15 | Number.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype) |
| 21.1.3 | Properties of the Number Prototype Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-number-prototype-object) |
| 21.1.3.1 | Number.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.constructor) |
| 21.1.3.2 | Number.prototype.toExponential ( fractionDigits ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.toexponential) |
| 21.1.3.3 | Number.prototype.toFixed ( fractionDigits ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.tofixed) |
| 21.1.3.4 | Number.prototype.toLocaleString ( [ reserved1 [ , reserved2 ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.tolocalestring) |
| 21.1.3.5 | Number.prototype.toPrecision ( precision ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.toprecision) |
| 21.1.3.6 | Number.prototype.toString ( [ radix ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.tostring) |
| 21.1.3.7 | Number.prototype.valueOf ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-number.prototype.valueof) |
| 21.1.3.7.1 | ThisNumberValue ( value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-thisnumbervalue) |
| 21.1.4 | Properties of Number Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-number-instances) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 21.1.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-number-constructor-number-value))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number ( value ) | Supported with Limitations |  | `built-ins/Number/return-abrupt-tonumber-value.js`<br>`built-ins/Number/S9.3_A1_T1.js`<br>`built-ins/Number/S9.3.1_A17.js`<br>`built-ins/Number/S9.3_A2_T1.js`<br>`built-ins/Number/S9.3_A3_T1.js`<br>`built-ins/Number/S9.3_A4.1_T1.js`<br>`built-ins/Number/S9.3.1_A1.js`<br>`built-ins/Number/S9.3.1_A7.js` | Direct calls to the global Number function are lowered to TypeUtilities.ToNumber and preserve observable abrupt completions even when the converted value is discarded. Checked-in coverage now includes representative undefined, null, boolean, numeric, empty-string, decimal-string, NaN, and hexadecimal-string coercion cases. Wrapper construction is supported for the currently modeled Number object surface, but full Number constructor/prototype semantics remain incomplete. |

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

