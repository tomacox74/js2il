# Section 6.1: ECMAScript Language Types

[Back to Section6](Section6.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 6.1 | ECMAScript Language Types | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types) |
| 6.1.1 | The Undefined Type | Supported | [tc39.es](https://tc39.es/ecma262/#sec-undefined-type) |
| 6.1.2 | The Null Type | Supported | [tc39.es](https://tc39.es/ecma262/#sec-null-type) |
| 6.1.3 | The Boolean Type | Supported | [tc39.es](https://tc39.es/ecma262/#sec-boolean-type) |
| 6.1.4 | The String Type | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-string-type) |
| 6.1.4.1 | StringIndexOf ( string, searchValue, fromIndex ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-stringindexof) |
| 6.1.4.2 | StringLastIndexOf ( string, searchValue, fromIndex ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-stringlastindexof) |
| 6.1.5 | The Symbol Type | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-symbol-type) |
| 6.1.5.1 | Well-Known Symbols | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-well-known-symbols) |
| 6.1.6 | Numeric Types | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-numeric-types) |
| 6.1.6.1 | The Number Type | Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-number-type) |
| 6.1.6.1.1 | Number::unaryMinus ( x ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-unaryminus) |
| 6.1.6.1.2 | Number::bitwiseNOT ( x ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-bitwisenot) |
| 6.1.6.1.3 | Number::exponentiate ( base, exponent ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-exponentiate) |
| 6.1.6.1.4 | Number::multiply ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-multiply) |
| 6.1.6.1.5 | Number::divide ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-divide) |
| 6.1.6.1.6 | Number::remainder ( n, d ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-remainder) |
| 6.1.6.1.7 | Number::add ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-add) |
| 6.1.6.1.8 | Number::subtract ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-subtract) |
| 6.1.6.1.9 | Number::leftShift ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-leftshift) |
| 6.1.6.1.10 | Number::signedRightShift ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-signedrightshift) |
| 6.1.6.1.11 | Number::unsignedRightShift ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-unsignedrightshift) |
| 6.1.6.1.12 | Number::lessThan ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-lessthan) |
| 6.1.6.1.13 | Number::equal ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-equal) |
| 6.1.6.1.14 | Number::sameValue ( x, y ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-samevalue) |
| 6.1.6.1.15 | Number::sameValueZero ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-samevaluezero) |
| 6.1.6.1.16 | NumberBitwiseOp ( op, x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-numberbitwiseop) |
| 6.1.6.1.17 | Number::bitwiseAND ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-bitwiseand) |
| 6.1.6.1.18 | Number::bitwiseXOR ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-bitwisexor) |
| 6.1.6.1.19 | Number::bitwiseOR ( x, y ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-bitwiseor) |
| 6.1.6.1.20 | Number::toString ( x, radix ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-number-tostring) |
| 6.1.6.2 | The BigInt Type | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ecmascript-language-types-bigint-type) |
| 6.1.6.2.1 | BigInt::unaryMinus ( x ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-unaryminus) |
| 6.1.6.2.2 | BigInt::bitwiseNOT ( x ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-bitwisenot) |
| 6.1.6.2.3 | BigInt::exponentiate ( base, exponent ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-exponentiate) |
| 6.1.6.2.4 | BigInt::multiply ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-multiply) |
| 6.1.6.2.5 | BigInt::divide ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-divide) |
| 6.1.6.2.6 | BigInt::remainder ( n, d ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-remainder) |
| 6.1.6.2.7 | BigInt::add ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-add) |
| 6.1.6.2.8 | BigInt::subtract ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-subtract) |
| 6.1.6.2.9 | BigInt::leftShift ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-leftshift) |
| 6.1.6.2.10 | BigInt::signedRightShift ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-signedrightshift) |
| 6.1.6.2.11 | BigInt::unsignedRightShift ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-unsignedrightshift) |
| 6.1.6.2.12 | BigInt::lessThan ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-lessthan) |
| 6.1.6.2.13 | BigInt::equal ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-equal) |
| 6.1.6.2.14 | BinaryAnd ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-binaryand) |
| 6.1.6.2.15 | BinaryOr ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-binaryor) |
| 6.1.6.2.16 | BinaryXor ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-binaryxor) |
| 6.1.6.2.17 | BigIntBitwiseOp ( op, x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigintbitwiseop) |
| 6.1.6.2.18 | BigInt::bitwiseAND ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-bitwiseand) |
| 6.1.6.2.19 | BigInt::bitwiseXOR ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-bitwisexor) |
| 6.1.6.2.20 | BigInt::bitwiseOR ( x, y ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-bitwiseor) |
| 6.1.6.2.21 | BigInt::toString ( x, radix ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-tostring) |
| 6.1.7 | The Object Type | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-type) |
| 6.1.7.1 | Property Attributes | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-property-attributes) |
| 6.1.7.2 | Object Internal Methods and Internal Slots | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-internal-methods-and-internal-slots) |
| 6.1.7.3 | Invariants of the Essential Internal Methods | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-invariants-of-the-essential-internal-methods) |
| 6.1.7.4 | Well-Known Intrinsic Objects | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-well-known-intrinsic-objects) |

