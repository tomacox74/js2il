# Section 6.1: ECMAScript Language Types

[Back to Section6](Section6.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-07T17:43:34Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 6.1 | ECMAScript Language Types | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 6.1.1 | The Undefined Type | Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-undefined-type) |
| 6.1.2 | The Null Type | Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-null-type) |
| 6.1.3 | The Boolean Type | Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-boolean-type) |
| 6.1.4 | The String Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-string-type) |
| 6.1.4.1 | StringIndexOf ( string , searchValue , fromIndex ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-stringindexof) |
| 6.1.4.2 | StringLastIndexOf ( string , searchValue , fromIndex ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-stringlastindexof) |
| 6.1.5 | The Symbol Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-symbol-type) |
| 6.1.5.1 | Well-Known Symbols | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-well-known-symbols) |
| 6.1.6 | Numeric Types | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types) |
| 6.1.6.1 | The Number Type | Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-number-type) |
| 6.1.6.1.1 | Number::unaryMinus ( x ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-unaryMinus) |
| 6.1.6.1.2 | Number::bitwiseNOT ( x ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseNOT) |
| 6.1.6.1.3 | Number::exponentiate ( base , exponent ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-exponentiate) |
| 6.1.6.1.4 | Number::multiply ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-multiply) |
| 6.1.6.1.5 | Number::divide ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-divide) |
| 6.1.6.1.6 | Number::remainder ( n , d ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-remainder) |
| 6.1.6.1.7 | Number::add ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-add) |
| 6.1.6.1.8 | Number::subtract ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-subtract) |
| 6.1.6.1.9 | Number::leftShift ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-leftShift) |
| 6.1.6.1.10 | Number::signedRightShift ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-signedRightShift) |
| 6.1.6.1.11 | Number::unsignedRightShift ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-unsignedRightShift) |
| 6.1.6.1.12 | Number::lessThan ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-lessThan) |
| 6.1.6.1.13 | Number::equal ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-equal) |
| 6.1.6.1.14 | Number::sameValue ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-sameValue) |
| 6.1.6.1.15 | Number::sameValueZero ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-sameValueZero) |
| 6.1.6.1.16 | NumberBitwiseOp ( op , x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numberbitwiseop) |
| 6.1.6.1.17 | Number::bitwiseAND ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseAND) |
| 6.1.6.1.18 | Number::bitwiseXOR ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseXOR) |
| 6.1.6.1.19 | Number::bitwiseOR ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseOR) |
| 6.1.6.1.20 | Number::toString ( x , radix ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-tostring) |
| 6.1.6.2 | The BigInt Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-bigint-type) |
| 6.1.6.2.1 | BigInt::unaryMinus ( x ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-unaryMinus) |
| 6.1.6.2.2 | BigInt::bitwiseNOT ( x ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseNOT) |
| 6.1.6.2.3 | BigInt::exponentiate ( base , exponent ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-exponentiate) |
| 6.1.6.2.4 | BigInt::multiply ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-multiply) |
| 6.1.6.2.5 | BigInt::divide ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-divide) |
| 6.1.6.2.6 | BigInt::remainder ( n , d ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-remainder) |
| 6.1.6.2.7 | BigInt::add ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-add) |
| 6.1.6.2.8 | BigInt::subtract ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-subtract) |
| 6.1.6.2.9 | BigInt::leftShift ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-leftShift) |
| 6.1.6.2.10 | BigInt::signedRightShift ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-signedRightShift) |
| 6.1.6.2.11 | BigInt::unsignedRightShift ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-unsignedRightShift) |
| 6.1.6.2.12 | BigInt::lessThan ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-lessThan) |
| 6.1.6.2.13 | BigInt::equal ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-equal) |
| 6.1.6.2.14 | BinaryAnd ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-binaryand) |
| 6.1.6.2.15 | BinaryOr ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-binaryor) |
| 6.1.6.2.16 | BinaryXor ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-binaryxor) |
| 6.1.6.2.17 | BigIntBitwiseOp ( op , x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigintbitwiseop) |
| 6.1.6.2.18 | BigInt::bitwiseAND ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseAND) |
| 6.1.6.2.19 | BigInt::bitwiseXOR ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseXOR) |
| 6.1.6.2.20 | BigInt::bitwiseOR ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseOR) |
| 6.1.6.2.21 | BigInt::toString ( x , radix ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-tostring) |
| 6.1.7 | The Object Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-type) |
| 6.1.7.1 | Property Attributes | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-property-attributes) |
| 6.1.7.2 | Object Internal Methods and Internal Slots | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-internal-methods-and-internal-slots) |
| 6.1.7.3 | Invariants of the Essential Internal Methods | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-invariants-of-the-essential-internal-methods) |
| 6.1.7.4 | Well-Known Intrinsic Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-well-known-intrinsic-objects) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 6.1 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ECMAScript Language Types | Supported with Limitations |  |  | The primitive-language-type clauses are broadly supported, and the Section 6.1.7 object-model slice is now covered well enough to raise the overall 6.1 status. Remaining limitations are concentrated in edge-case object/proxy semantics and in other already-documented subsection limitations. |

### 6.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-undefined-type))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| The Undefined Type | Supported |  |  |  |

### 6.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-null-type))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| The Null Type | Supported |  |  |  |

### 6.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-boolean-type))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| The Boolean Type | Supported |  |  |  |

### 6.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-string-type))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| The String Type | Supported with Limitations |  |  |  |

### 6.1.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-stringindexof))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| StringIndexOf | Supported |  |  |  |

### 6.1.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-stringlastindexof))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| StringLastIndexOf | Supported |  |  |  |

### 6.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-symbol-type))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| The Symbol Type | Supported with Limitations |  |  |  |

### 6.1.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-well-known-symbols))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Well-Known Symbols | Supported with Limitations |  |  |  |

### 6.1.6 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Numeric Types | Supported with Limitations |  |  |  |

### 6.1.6.1 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-number-type))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| The Number Type | Supported |  |  |  |

### 6.1.6.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-unaryMinus))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::unaryMinus | Supported |  |  |  |

### 6.1.6.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseNOT))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::bitwiseNOT | Supported |  |  |  |

### 6.1.6.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-exponentiate))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::exponentiate | Supported |  |  |  |

### 6.1.6.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-multiply))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::multiply | Supported |  |  |  |

### 6.1.6.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-divide))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::divide | Supported |  |  |  |

### 6.1.6.1.6 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-remainder))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::remainder | Supported |  |  |  |

### 6.1.6.1.7 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-add))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::add | Supported |  |  |  |

### 6.1.6.1.8 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-subtract))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::subtract | Supported |  |  |  |

### 6.1.6.1.9 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-leftShift))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::leftShift | Supported |  |  |  |

### 6.1.6.1.10 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-signedRightShift))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::signedRightShift | Supported |  |  |  |

### 6.1.6.1.11 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-unsignedRightShift))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::unsignedRightShift | Supported |  |  |  |

### 6.1.6.1.12 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-lessThan))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::lessThan | Supported |  |  |  |

### 6.1.6.1.13 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-equal))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::equal | Supported |  |  |  |

### 6.1.6.1.14 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-sameValue))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::sameValue | Supported |  |  |  |

### 6.1.6.1.15 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-sameValueZero))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::sameValueZero | Supported |  |  |  |

### 6.1.6.1.16 ([tc39.es](https://tc39.es/ecma262/#sec-numberbitwiseop))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| NumberBitwiseOp | Supported |  |  |  |

### 6.1.6.1.17 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseAND))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::bitwiseAND | Supported |  |  |  |

### 6.1.6.1.18 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseXOR))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::bitwiseXOR | Supported |  |  |  |

### 6.1.6.1.19 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseOR))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::bitwiseOR | Supported |  |  |  |

### 6.1.6.1.20 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-tostring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Number::toString | Supported with Limitations |  |  |  |

### 6.1.6.2 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-bigint-type))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| The BigInt Type | Supported with Limitations |  |  |  |

### 6.1.6.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-unaryMinus))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::unaryMinus | Supported with Limitations |  |  |  |

### 6.1.6.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseNOT))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::bitwiseNOT | Supported with Limitations |  |  |  |

### 6.1.6.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-exponentiate))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::exponentiate | Supported with Limitations |  |  |  |

### 6.1.6.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-multiply))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::multiply | Supported with Limitations |  |  |  |

### 6.1.6.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-divide))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::divide | Supported with Limitations |  |  |  |

### 6.1.6.2.6 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-remainder))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::remainder | Supported with Limitations |  |  |  |

### 6.1.6.2.7 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-add))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::add | Supported with Limitations |  |  |  |

### 6.1.6.2.8 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-subtract))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::subtract | Supported with Limitations |  |  |  |

### 6.1.6.2.9 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-leftShift))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::leftShift | Supported with Limitations |  |  |  |

### 6.1.6.2.10 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-signedRightShift))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::signedRightShift | Supported with Limitations |  |  |  |

### 6.1.6.2.11 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-unsignedRightShift))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::unsignedRightShift | Supported with Limitations |  |  |  |

### 6.1.6.2.12 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-lessThan))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::lessThan | Supported with Limitations |  |  |  |

### 6.1.6.2.13 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-equal))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::equal | Supported with Limitations |  |  |  |

### 6.1.6.2.14 ([tc39.es](https://tc39.es/ecma262/#sec-binaryand))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BinaryAnd | Supported with Limitations |  |  |  |

### 6.1.6.2.15 ([tc39.es](https://tc39.es/ecma262/#sec-binaryor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BinaryOr | Supported with Limitations |  |  |  |

### 6.1.6.2.16 ([tc39.es](https://tc39.es/ecma262/#sec-binaryxor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BinaryXor | Supported with Limitations |  |  |  |

### 6.1.6.2.17 ([tc39.es](https://tc39.es/ecma262/#sec-bigintbitwiseop))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigIntBitwiseOp | Supported with Limitations |  |  |  |

### 6.1.6.2.18 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseAND))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::bitwiseAND | Supported with Limitations |  |  |  |

### 6.1.6.2.19 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseXOR))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::bitwiseXOR | Supported with Limitations |  |  |  |

### 6.1.6.2.20 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseOR))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::bitwiseOR | Supported with Limitations |  |  |  |

### 6.1.6.2.21 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-tostring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt::toString | Supported with Limitations |  |  |  |

### 6.1.7 ([tc39.es](https://tc39.es/ecma262/#sec-object-type))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| The Object Type | Supported with Limitations |  | suite `built-ins/Object/internals/DefineOwnProperty`<br>suite `built-ins/Proxy/getOwnPropertyDescriptor` | Ordinary objects, property descriptors, and the covered Proxy [[GetOwnProperty]] paths now behave correctly for the newly ported 6.1.7 slice, including NaN-preserving data-property updates, proxy own-property fallback, and key proxy invariants. JROC still has broader object-model gaps outside this slice, especially around the full proxy trap surface and unported internal-method edge cases. |

### 6.1.7.1 ([tc39.es](https://tc39.es/ecma262/#sec-property-attributes))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Property Attributes | Supported with Limitations |  | `test/built-ins/Object/internals/DefineOwnProperty/nan-equivalence-define-own-property-reassign.js`<br>`test/built-ins/Object/internals/DefineOwnProperty/nan-equivalence-define-own-property-reconfigure.js` | JROC now preserves descriptor-backed data-property updates across repeated NaN writes and Object.defineProperty reconfiguration in the newly ported cases. Coverage is still narrow relative to the full property-attributes clause, so the section remains documented with limitations. |

### 6.1.7.2 ([tc39.es](https://tc39.es/ecma262/#sec-object-internal-methods-and-internal-slots))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Object Internal Methods and Internal Slots | Supported with Limitations |  | `test/built-ins/Proxy/getOwnPropertyDescriptor/trap-is-undefined.js` | The covered Proxy [[GetOwnProperty]] forwarding path now matches ordinary-target behavior closely enough for descriptor lookup, hasOwnProperty/propertyIsEnumerable, and for-in enumeration in the newly ported test262 case. Other internal methods and less-common proxy/internal-slot interactions remain only partially covered. |

### 6.1.7.3 ([tc39.es](https://tc39.es/ecma262/#sec-invariants-of-the-essential-internal-methods))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Invariants of the Essential Internal Methods | Supported with Limitations |  | `test/built-ins/Proxy/getOwnPropertyDescriptor/resultdesc-is-invalid-descriptor.js`<br>`test/built-ins/Proxy/getOwnPropertyDescriptor/result-is-undefined-target-is-not-extensible.js` | JROC now enforces the covered Proxy [[GetOwnProperty]] invariants for undefined results on non-extensible targets and for incompatible reported descriptors. The full invariant matrix across every essential internal method is still not exhaustively implemented or ported, so this clause remains supported with limitations. |

### 6.1.7.4 ([tc39.es](https://tc39.es/ecma262/#sec-well-known-intrinsic-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Well-Known Intrinsic Objects | Supported with Limitations |  |  |  |

