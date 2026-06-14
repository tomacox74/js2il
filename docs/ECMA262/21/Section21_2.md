<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 21.2: BigInt Objects

[Back to Section21](Section21.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-29T14:16:08Z

JROC provides a minimal BigInt callable implementation backed by System.Numerics.BigInteger, sufficient for basic BigInt(value) usage and typeof semantics. The broader BigInt constructor/prototype surface and full spec conversion rules are not yet implemented.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 21.2 | BigInt Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-bigint-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 21.2.1 | The BigInt Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint-constructor) |
| 21.2.1.1 | BigInt ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint-constructor-number-value) |
| 21.2.1.1.1 | NumberToBigInt ( number ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numbertobigint) |
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

Feature-level support tracking with repo test references and optional test262 evidence.

### 21.2 ([tc39.es](https://tc39.es/ecma262/#sec-bigint-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt primitive operator semantics (representative unary, bitwise, and shift cases) | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/bigint/ExecutionTests.cs` | `test/language/expressions/unary-minus/bigint.js`<br>`test/language/expressions/bitwise-not/bigint.js`<br>`test/language/expressions/left-shift/bigint.js`<br>`test/language/expressions/unsigned-right-shift/bigint.js` | Checked-in coverage now includes representative primitive BigInt unary negation, bitwise NOT, left shift, and unsigned-right-shift TypeError behavior. The broader BigInt constructor/prototype surface remains incomplete. |

### 21.2.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-bigint-constructor-number-value))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt(value) callable (basic) | Supported with Limitations | [`IntrinsicCallables_BigInt_Callable_Basic.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_BigInt_Callable_Basic.js)<br>`tests/Jroc.Test262.Tests/built-ins/BigInt/ExecutionTests.cs` | `test/built-ins/BigInt/constructor-empty-string.js`<br>`test/built-ins/BigInt/constructor-from-binary-string.js`<br>`test/built-ins/BigInt/constructor-from-decimal-string.js`<br>`test/built-ins/BigInt/constructor-from-hex-string.js`<br>`test/built-ins/BigInt/constructor-from-octal-string.js`<br>`test/built-ins/BigInt/constructor-from-string-syntax-errors.js`<br>`test/built-ins/BigInt/constructor-trailing-leading-spaces.js` | Supports BigInt(value) with basic conversion from integral Numbers and from decimal, binary, octal, and hexadecimal strings, including empty/trimmed strings and representative string syntax errors; typeof === 'bigint'. The full BigInt constructor/prototype surface remains incomplete. |

