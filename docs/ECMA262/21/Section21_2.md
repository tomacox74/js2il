<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 21.2: BigInt Objects

[Back to Section21](Section21.md) | [Back to Index](../Index.md)

JS2IL provides a minimal BigInt callable implementation backed by System.Numerics.BigInteger, sufficient for basic BigInt(value) usage and typeof semantics. The broader BigInt constructor/prototype surface and full spec conversion rules are not yet implemented.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 21.2 | BigInt Objects | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 21.2.1 | The BigInt Constructor | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-constructor) |
| 21.2.1.1 | BigInt ( value ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-bigint-constructor-number-value) |
| 21.2.1.1.1 | NumberToBigInt ( number ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-numbertobigint) |
| 21.2.2 | Properties of the BigInt Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-bigint-constructor) |
| 21.2.2.1 | BigInt.asIntN ( bits , bigint ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bigint.asintn) |
| 21.2.2.2 | BigInt.asUintN ( bits , bigint ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bigint.asuintn) |
| 21.2.2.3 | BigInt.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype) |
| 21.2.3 | Properties of the BigInt Prototype Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-bigint-prototype-object) |
| 21.2.3.1 | BigInt.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype.constructor) |
| 21.2.3.2 | BigInt.prototype.toLocaleString ( [ reserved1 [ , reserved2 ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype.tolocalestring) |
| 21.2.3.3 | BigInt.prototype.toString ( [ radix ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype.tostring) |
| 21.2.3.4 | BigInt.prototype.valueOf ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype.valueof) |
| 21.2.3.4.1 | ThisBigIntValue ( value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-thisbigintvalue) |
| 21.2.3.5 | BigInt.prototype [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype-%symbol.tostringtag%) |
| 21.2.4 | Properties of BigInt Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-bigint-instances) |

## Support

Feature-level support tracking with test script references.

### 21.2.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-bigint-constructor-number-value))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BigInt(value) callable (basic) | Partially Supported | [`IntrinsicCallables_BigInt_Callable_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_BigInt_Callable_Basic.js) | Supports BigInt(value) with basic conversion from integral Numbers and from decimal strings; typeof === 'bigint'. Does not implement the full spec ToBigInt / StringToBigInt grammar or the full BigInt constructor/prototype surface. |

