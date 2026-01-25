<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 20.2: Function Objects

[Back to Section20](Section20.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 20.2 | Function Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 20.2.1 | The Function Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function-constructor) |
| 20.2.1.1 | Function ( ... parameterArgs , bodyArg ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function-p1-p2-pn-body) |
| 20.2.1.1.1 | CreateDynamicFunction ( constructor , newTarget , kind , parameterArgs , bodyArg ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-createdynamicfunction) |
| 20.2.2 | Properties of the Function Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-function-constructor) |
| 20.2.2.1 | Function.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype) |
| 20.2.3 | Properties of the Function Prototype Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-function-prototype-object) |
| 20.2.3.1 | Function.prototype.apply ( thisArg , argArray ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.apply) |
| 20.2.3.2 | Function.prototype.bind ( thisArg , ... args ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.bind) |
| 20.2.3.3 | Function.prototype.call ( thisArg , ... args ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.call) |
| 20.2.3.4 | Function.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.constructor) |
| 20.2.3.5 | Function.prototype.toString ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype.tostring) |
| 20.2.3.6 | Function.prototype [ %Symbol.hasInstance% ] ( V ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function.prototype-%symbol.hasinstance%) |
| 20.2.4 | Function Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function-instances) |
| 20.2.4.1 | length | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function-instances-length) |
| 20.2.4.2 | name | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function-instances-name) |
| 20.2.4.3 | prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function-instances-prototype) |
| 20.2.5 | HostHasSourceTextAvailable ( func ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-hosthassourcetextavailable) |

## Support

Feature-level support tracking with test script references.

### 20.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-function-constructor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.E | Supported |  | Euler’s number e. |
| Math.LN2 | Supported |  | Natural logarithm of 2. |
| Math.LN10 | Supported |  | Natural logarithm of 10. |
| Math.LOG2E | Supported |  | Base-2 logarithm of e. |
| Math.LOG10E | Supported |  | Base-10 logarithm of e. |
| Math.PI | Supported |  | Ratio of a circle’s circumference to its diameter. |
| Math.SQRT1_2 | Supported |  | Square root of 1/2. |
| Math.SQRT2 | Supported |  | Square root of 2. |

### 20.2.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-function.prototype))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.abs(x) | Supported |  | Returns the absolute value; NaN propagates; ±Infinity preserved. |

### 20.2.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-math.acos))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.acos(x) | Supported |  | Returns arc cosine in radians; out-of-domain yields NaN. |

### 20.2.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-math.acosh))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.acosh(x) | Supported |  | Inverse hyperbolic cosine; x < 1 yields NaN; Infinity preserved. |

### 20.2.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-math.asin))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.asin(x) | Supported |  | Returns arc sine in radians; out-of-domain yields NaN. |

### 20.2.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-math.asinh))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.asinh(x) | Supported |  | Inverse hyperbolic sine; handles ±0, NaN, ±Infinity per spec. |

### 20.2.2.6 ([tc39.es](https://tc39.es/ecma262/#sec-math.atan))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.atan(x) | Supported |  | Returns arc tangent in radians; NaN propagates; ±Infinity maps to ±π/2. |

### 20.2.2.7 ([tc39.es](https://tc39.es/ecma262/#sec-math.atan2))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.atan2(y, x) | Supported |  | Quadrant-aware arc tangent; handles zeros, NaN, and infinities per spec. |

### 20.2.2.9 ([tc39.es](https://tc39.es/ecma262/#sec-math.ceil))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.ceil(x) | Supported | [`Math_Ceil_Sqrt_Basic.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Ceil_Sqrt_Basic.js) | Implements ceiling for numbers represented as double; arguments coerced via minimal ToNumber semantics. Returns NaN for NaN/undefined or negative zero preserved via .NET semantics. |

### 20.2.2.10 ([tc39.es](https://tc39.es/ecma262/#sec-math.clz32))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.clz32(x) | Supported | [`Math_Imul_Clz32_Basics.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Imul_Clz32_Basics.js) | Counts leading zero bits in the 32-bit unsigned integer representation. |

### 20.2.2.11 ([tc39.es](https://tc39.es/ecma262/#sec-math.cos))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.cos(x) | Supported |  | Cosine of x (radians); NaN propagates; Infinity yields NaN. |

### 20.2.2.12 ([tc39.es](https://tc39.es/ecma262/#sec-math.cosh))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.cosh(x) | Supported |  | Hyperbolic cosine; handles ±0, NaN, ±Infinity per spec. |

### 20.2.2.13 ([tc39.es](https://tc39.es/ecma262/#sec-math.exp))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.exp(x) | Supported | [`Math_Log_Exp_Identity.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Log_Exp_Identity.js) | e^x; consistent with JS semantics for NaN and infinities. |

### 20.2.2.14 ([tc39.es](https://tc39.es/ecma262/#sec-math.expm1))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.expm1(x) | Supported |  | Returns e^x - 1 with improved precision for small x. |

### 20.2.2.15 ([tc39.es](https://tc39.es/ecma262/#sec-math.floor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.floor(x) | Supported |  | Largest integer less than or equal to x; preserves -0 when appropriate. |

### 20.2.2.16 ([tc39.es](https://tc39.es/ecma262/#sec-math.fround))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.fround(x) | Supported | [`Math_Fround_SignedZero.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Fround_SignedZero.js) | Rounds to nearest 32-bit float; preserves signed zero. |

### 20.2.2.17 ([tc39.es](https://tc39.es/ecma262/#sec-math.hypot))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.hypot(...values) | Supported | [`Math_Hypot_Infinity_NaN.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Hypot_Infinity_NaN.js) | Computes sqrt(sum(x_i^2)); returns Infinity if any arg is ±Infinity; NaN if any arg is NaN and none are Infinity. |

### 20.2.2.18 ([tc39.es](https://tc39.es/ecma262/#sec-math.imul))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.imul(a, b) | Supported | [`Math_Imul_Clz32_Basics.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Imul_Clz32_Basics.js) | C-style 32-bit integer multiplication with wrapping. |

### 20.2.2.19 ([tc39.es](https://tc39.es/ecma262/#sec-math.log))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.log(x) | Supported | [`Math_Log_Exp_Identity.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Log_Exp_Identity.js) | Natural logarithm; log(1) = 0; negative x yields NaN; log(0) = -Infinity. |

### 20.2.2.20 ([tc39.es](https://tc39.es/ecma262/#sec-math.log10))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.log10(x) | Supported |  | Base-10 logarithm; JS semantics for 0, negatives, NaN, and infinities. |

### 20.2.2.21 ([tc39.es](https://tc39.es/ecma262/#sec-math.log1p))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.log1p(x) | Supported |  | log(1 + x) with improved precision for small x. |

### 20.2.2.22 ([tc39.es](https://tc39.es/ecma262/#sec-math.log2))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.log2(x) | Supported |  | Base-2 logarithm; JS semantics for 0, negatives, NaN, and infinities. |

### 20.2.2.23 ([tc39.es](https://tc39.es/ecma262/#sec-math.max))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.max(...values) | Supported | [`Math_Min_Max_NaN_EmptyArgs.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Min_Max_NaN_EmptyArgs.js) | Returns the largest of the given numbers; with no arguments returns -Infinity; if any argument is NaN returns NaN. |

### 20.2.2.24 ([tc39.es](https://tc39.es/ecma262/#sec-math.sqrt))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.sqrt(x) | Supported | [`Math_Ceil_Sqrt_Basic.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Ceil_Sqrt_Basic.js) | Returns the square root for non-negative inputs; negative or NaN yields NaN; Infinity maps to Infinity. |

### 20.2.2.25 ([tc39.es](https://tc39.es/ecma262/#sec-math.pow))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.pow(x, y) | Supported |  | Exponentiation; consistent with JS semantics including NaN and Infinity cases. |

### 20.2.2.26 ([tc39.es](https://tc39.es/ecma262/#sec-math.random))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.random() | Supported |  | Returns a pseudo-random number in the range [0, 1). |

### 20.2.2.27 ([tc39.es](https://tc39.es/ecma262/#sec-math.round))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.round(x) | Supported | [`Math_Round_Trunc_NegativeHalves.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Round_Trunc_NegativeHalves.js) | Rounds to the nearest integer; ties at .5 round up toward +∞; exact -0.5 returns -0. |

### 20.2.2.28 ([tc39.es](https://tc39.es/ecma262/#sec-math.sign))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.sign(x) | Supported | [`Math_Sign_ZeroVariants.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Sign_ZeroVariants.js) | Returns 1, -1, 0, -0, or NaN depending on the sign of x; ±Infinity map to ±1. |

### 20.2.2.29 ([tc39.es](https://tc39.es/ecma262/#sec-math.sin))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.sin(x) | Supported |  | Sine of x (radians); NaN propagates; Infinity yields NaN. |

### 20.2.2.30 ([tc39.es](https://tc39.es/ecma262/#sec-math.sinh))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.sinh(x) | Supported |  | Hyperbolic sine; handles ±0, NaN, ±Infinity per spec. |

### 20.2.2.31 ([tc39.es](https://tc39.es/ecma262/#sec-math.tan))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.tan(x) | Supported |  | Tangent of x (radians); NaN propagates; Infinity yields NaN. |

### 20.2.2.32 ([tc39.es](https://tc39.es/ecma262/#sec-math.tanh))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.tanh(x) | Supported |  | Hyperbolic tangent; handles ±0, NaN, ±Infinity per spec. |

### 20.2.2.33 ([tc39.es](https://tc39.es/ecma262/#sec-math.trunc))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.trunc(x) | Supported | [`Math_Round_Trunc_NegativeHalves.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Round_Trunc_NegativeHalves.js) | Removes fractional part; preserves sign for zero (can return -0). |

### 20.2.2.34 ([tc39.es](https://tc39.es/ecma262/#sec-math.min))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.min(...values) | Supported | [`Math_Min_Max_NaN_EmptyArgs.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Min_Max_NaN_EmptyArgs.js) | Returns the smallest of the given numbers; with no arguments returns Infinity; if any argument is NaN returns NaN. |

### 20.2.2.35 ([tc39.es](https://tc39.es/ecma262/#sec-math.cbrt))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math.cbrt(x) | Supported | [`Math_Cbrt_Negative.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Cbrt_Negative.js) | Cube root; handles negative values returning negative result; NaN propagates; Infinity preserved. |

