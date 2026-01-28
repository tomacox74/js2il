# Section 6.1: ECMAScript Language Types

[Back to Section6](Section6.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 6.1 | ECMAScript Language Types | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 6.1.1 | The Undefined Type | Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-undefined-type) |
| 6.1.2 | The Null Type | Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-null-type) |
| 6.1.3 | The Boolean Type | Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-boolean-type) |
| 6.1.4 | The String Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-string-type) |
| 6.1.4.1 | StringIndexOf ( string , searchValue , fromIndex ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-stringindexof) |
| 6.1.4.2 | StringLastIndexOf ( string , searchValue , fromIndex ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-stringlastindexof) |
| 6.1.5 | The Symbol Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-symbol-type) |
| 6.1.5.1 | Well-Known Symbols | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-well-known-symbols) |
| 6.1.6 | Numeric Types | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types) |
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
| 6.1.6.1.14 | Number::sameValue ( x , y ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-sameValue) |
| 6.1.6.1.15 | Number::sameValueZero ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-sameValueZero) |
| 6.1.6.1.16 | NumberBitwiseOp ( op , x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numberbitwiseop) |
| 6.1.6.1.17 | Number::bitwiseAND ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseAND) |
| 6.1.6.1.18 | Number::bitwiseXOR ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseXOR) |
| 6.1.6.1.19 | Number::bitwiseOR ( x , y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseOR) |
| 6.1.6.1.20 | Number::toString ( x , radix ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-tostring) |
| 6.1.6.2 | The BigInt Type | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-bigint-type) |
| 6.1.6.2.1 | BigInt::unaryMinus ( x ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-unaryMinus) |
| 6.1.6.2.2 | BigInt::bitwiseNOT ( x ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseNOT) |
| 6.1.6.2.3 | BigInt::exponentiate ( base , exponent ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-exponentiate) |
| 6.1.6.2.4 | BigInt::multiply ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-multiply) |
| 6.1.6.2.5 | BigInt::divide ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-divide) |
| 6.1.6.2.6 | BigInt::remainder ( n , d ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-remainder) |
| 6.1.6.2.7 | BigInt::add ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-add) |
| 6.1.6.2.8 | BigInt::subtract ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-subtract) |
| 6.1.6.2.9 | BigInt::leftShift ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-leftShift) |
| 6.1.6.2.10 | BigInt::signedRightShift ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-signedRightShift) |
| 6.1.6.2.11 | BigInt::unsignedRightShift ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-unsignedRightShift) |
| 6.1.6.2.12 | BigInt::lessThan ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-lessThan) |
| 6.1.6.2.13 | BigInt::equal ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-equal) |
| 6.1.6.2.14 | BinaryAnd ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-binaryand) |
| 6.1.6.2.15 | BinaryOr ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-binaryor) |
| 6.1.6.2.16 | BinaryXor ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-binaryxor) |
| 6.1.6.2.17 | BigIntBitwiseOp ( op , x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigintbitwiseop) |
| 6.1.6.2.18 | BigInt::bitwiseAND ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseAND) |
| 6.1.6.2.19 | BigInt::bitwiseXOR ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseXOR) |
| 6.1.6.2.20 | BigInt::bitwiseOR ( x , y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseOR) |
| 6.1.6.2.21 | BigInt::toString ( x , radix ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-tostring) |
| 6.1.7 | The Object Type | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-object-type) |
| 6.1.7.1 | Property Attributes | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-property-attributes) |
| 6.1.7.2 | Object Internal Methods and Internal Slots | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-object-internal-methods-and-internal-slots) |
| 6.1.7.3 | Invariants of the Essential Internal Methods | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-invariants-of-the-essential-internal-methods) |
| 6.1.7.4 | Well-Known Intrinsic Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-well-known-intrinsic-objects) |

## Support

Feature-level support tracking with test script references.

### 6.1 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ECMAScript Language Types | Incomplete |  |  |

### 6.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-undefined-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| The Undefined Type | Supported |  |  |

### 6.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-null-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| The Null Type | Supported |  |  |

### 6.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-boolean-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| The Boolean Type | Supported |  |  |

### 6.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-string-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| The String Type | Supported with Limitations |  |  |

### 6.1.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-stringindexof))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| StringIndexOf | Untracked |  |  |

### 6.1.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-stringlastindexof))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| StringLastIndexOf | Untracked |  |  |

### 6.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-symbol-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| The Symbol Type | Supported with Limitations |  |  |

### 6.1.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-well-known-symbols))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Well-Known Symbols | Supported with Limitations |  |  |

### 6.1.6 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Numeric Types | Incomplete |  |  |

### 6.1.6.1 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-number-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| The Number Type | Supported |  |  |

### 6.1.6.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-unaryMinus))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::unaryMinus | Supported |  |  |

### 6.1.6.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseNOT))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::bitwiseNOT | Supported |  |  |

### 6.1.6.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-exponentiate))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::exponentiate | Supported |  |  |

### 6.1.6.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-multiply))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::multiply | Supported |  |  |

### 6.1.6.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-divide))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::divide | Supported |  |  |

### 6.1.6.1.6 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-remainder))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::remainder | Supported |  |  |

### 6.1.6.1.7 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-add))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::add | Supported |  |  |

### 6.1.6.1.8 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-subtract))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::subtract | Supported |  |  |

### 6.1.6.1.9 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-leftShift))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::leftShift | Supported |  |  |

### 6.1.6.1.10 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-signedRightShift))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::signedRightShift | Supported |  |  |

### 6.1.6.1.11 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-unsignedRightShift))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::unsignedRightShift | Supported |  |  |

### 6.1.6.1.12 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-lessThan))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::lessThan | Supported |  |  |

### 6.1.6.1.13 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-equal))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::equal | Supported |  |  |

### 6.1.6.1.14 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-sameValue))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::sameValue | Untracked |  |  |

### 6.1.6.1.15 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-sameValueZero))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::sameValueZero | Supported |  |  |

### 6.1.6.1.16 ([tc39.es](https://tc39.es/ecma262/#sec-numberbitwiseop))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| NumberBitwiseOp | Supported |  |  |

### 6.1.6.1.17 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseAND))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::bitwiseAND | Supported |  |  |

### 6.1.6.1.18 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseXOR))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::bitwiseXOR | Supported |  |  |

### 6.1.6.1.19 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-bitwiseOR))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::bitwiseOR | Supported |  |  |

### 6.1.6.1.20 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-number-tostring))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Number::toString | Supported with Limitations |  |  |

### 6.1.6.2 ([tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-bigint-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| The BigInt Type | Supported with Limitations |  |  |

### 6.1.6.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-unaryMinus))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::unaryMinus | Not Yet Supported |  |  |

### 6.1.6.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseNOT))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::bitwiseNOT | Not Yet Supported |  |  |

### 6.1.6.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-exponentiate))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::exponentiate | Not Yet Supported |  |  |

### 6.1.6.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-multiply))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::multiply | Not Yet Supported |  |  |

### 6.1.6.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-divide))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::divide | Not Yet Supported |  |  |

### 6.1.6.2.6 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-remainder))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::remainder | Not Yet Supported |  |  |

### 6.1.6.2.7 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-add))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::add | Not Yet Supported |  |  |

### 6.1.6.2.8 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-subtract))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::subtract | Not Yet Supported |  |  |

### 6.1.6.2.9 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-leftShift))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::leftShift | Not Yet Supported |  |  |

### 6.1.6.2.10 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-signedRightShift))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::signedRightShift | Not Yet Supported |  |  |

### 6.1.6.2.11 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-unsignedRightShift))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::unsignedRightShift | Not Yet Supported |  |  |

### 6.1.6.2.12 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-lessThan))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::lessThan | Not Yet Supported |  |  |

### 6.1.6.2.13 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-equal))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::equal | Not Yet Supported |  |  |

### 6.1.6.2.14 ([tc39.es](https://tc39.es/ecma262/#sec-binaryand))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BinaryAnd | Not Yet Supported |  |  |

### 6.1.6.2.15 ([tc39.es](https://tc39.es/ecma262/#sec-binaryor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BinaryOr | Not Yet Supported |  |  |

### 6.1.6.2.16 ([tc39.es](https://tc39.es/ecma262/#sec-binaryxor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BinaryXor | Not Yet Supported |  |  |

### 6.1.6.2.17 ([tc39.es](https://tc39.es/ecma262/#sec-bigintbitwiseop))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigIntBitwiseOp | Not Yet Supported |  |  |

### 6.1.6.2.18 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseAND))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::bitwiseAND | Not Yet Supported |  |  |

### 6.1.6.2.19 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseXOR))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::bitwiseXOR | Not Yet Supported |  |  |

### 6.1.6.2.20 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-bitwiseOR))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::bitwiseOR | Not Yet Supported |  |  |

### 6.1.6.2.21 ([tc39.es](https://tc39.es/ecma262/#sec-numeric-types-bigint-tostring))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt::toString | Not Yet Supported |  |  |

### 6.1.7 ([tc39.es](https://tc39.es/ecma262/#sec-object-type))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| The Object Type | Incomplete |  |  |

### 6.1.7.1 ([tc39.es](https://tc39.es/ecma262/#sec-property-attributes))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Property Attributes | Incomplete |  |  |

### 6.1.7.2 ([tc39.es](https://tc39.es/ecma262/#sec-object-internal-methods-and-internal-slots))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Object Internal Methods and Internal Slots | Incomplete |  |  |

### 6.1.7.3 ([tc39.es](https://tc39.es/ecma262/#sec-invariants-of-the-essential-internal-methods))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Invariants of the Essential Internal Methods | Incomplete |  |  |

### 6.1.7.4 ([tc39.es](https://tc39.es/ecma262/#sec-well-known-intrinsic-objects))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Well-Known Intrinsic Objects | Supported with Limitations |  |  |

