<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 21.2: BigInt Objects

[Back to Section21](Section21.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-07T16:01:14Z

JROC provides a BigInt callable and core prototype surface backed by System.Numerics.BigInteger, including `BigInt(value)`, `BigInt.asIntN`, `BigInt.asUintN`, `BigInt.prototype`, `toString`, `toLocaleString`, `valueOf`, and wrapper-object integration for the currently covered test262 surface. Full BigInt conversion rules and broader locale/radix coverage still remain incomplete.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 21.2 | BigInt Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 21.2.1 | The BigInt Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint-constructor) |
| 21.2.1.1 | BigInt ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint-constructor-number-value) |
| 21.2.1.1.1 | NumberToBigInt ( number ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-numbertobigint) |
| 21.2.2 | Properties of the BigInt Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-bigint-constructor) |
| 21.2.2.1 | BigInt.asIntN ( bits , bigint ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint.asintn) |
| 21.2.2.2 | BigInt.asUintN ( bits , bigint ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint.asuintn) |
| 21.2.2.3 | BigInt.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype) |
| 21.2.3 | Properties of the BigInt Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-bigint-prototype-object) |
| 21.2.3.1 | BigInt.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype.constructor) |
| 21.2.3.2 | BigInt.prototype.toLocaleString ( [ reserved1 [ , reserved2 ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype.tolocalestring) |
| 21.2.3.3 | BigInt.prototype.toString ( [ radix ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype.tostring) |
| 21.2.3.4 | BigInt.prototype.valueOf ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype.valueof) |
| 21.2.3.4.1 | ThisBigIntValue ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-thisbigintvalue) |
| 21.2.3.5 | BigInt.prototype [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype-%symbol.tostringtag%) |
| 21.2.4 | Properties of BigInt Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-bigint-instances) |

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

### 21.2.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-bigint.asintn))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt.asIntN(bits, bigint) | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/BigInt/asIntN/ExecutionTests.cs` | `test/built-ins/BigInt/asIntN/arithmetic.js`<br>`test/built-ins/BigInt/asIntN/asIntN.js`<br>`test/built-ins/BigInt/asIntN/length.js`<br>`test/built-ins/BigInt/asIntN/name.js`<br>`test/built-ins/BigInt/asIntN/not-a-constructor.js` | Supports `BigInt.asIntN` arithmetic truncation and callable metadata (name/length, non-constructible) for the covered test262 surface. Very large bit widths beyond current implementation limits remain unsupported. |

### 21.2.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-bigint.asuintn))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt.asUintN(bits, bigint) | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/BigInt/asUintN/ExecutionTests.cs` | `test/built-ins/BigInt/asUintN/arithmetic.js`<br>`test/built-ins/BigInt/asUintN/asUintN.js`<br>`test/built-ins/BigInt/asUintN/length.js`<br>`test/built-ins/BigInt/asUintN/name.js`<br>`test/built-ins/BigInt/asUintN/not-a-constructor.js` | Supports `BigInt.asUintN` modulo truncation and callable metadata (name/length, non-constructible) for the covered test262 surface. Very large bit widths beyond current implementation limits remain unsupported. |

### 21.2.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt.prototype descriptor and prototype linkage | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/BigInt/prototype/ExecutionTests.cs` | `test/built-ins/BigInt/prototype/prop-desc.js`<br>`test/built-ins/BigInt/prototype/proto.js` | Checked-in coverage now verifies the `BigInt.prototype` property descriptor on `%BigInt%` and the BigInt prototype object's `[[Prototype]]` linkage to `%Object.prototype%`. |

### 21.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-bigint-prototype-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt prototype surface and wrapper integration | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/BigInt/prototype/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Object/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Object/prototype/toString/ExecutionTests.cs` | `test/built-ins/BigInt/prototype/constructor.js`<br>`test/built-ins/BigInt/prototype/Symbol.toStringTag.js`<br>`test/built-ins/Object/bigint.js`<br>`test/built-ins/Object/prototype/toString/Object.prototype.toString.call-bigint.js` | JROC now exposes a concrete BigInt prototype object with constructor and `@@toStringTag` metadata, and `Object(0n)` produces a BigInt wrapper that participates in `instanceof`, `valueOf`, and `Object.prototype.toString` for the covered cases. |

### 21.2.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype.tolocalestring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt.prototype.toLocaleString metadata and non-constructibility | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/BigInt/prototype/toLocaleString/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/intl402/BigInt/prototype/toLocaleString/ExecutionTests.cs` | `test/built-ins/BigInt/prototype/toLocaleString/not-a-constructor.js`<br>`test/intl402/BigInt/prototype/toLocaleString/length.js`<br>`test/intl402/BigInt/prototype/toLocaleString/name.js` | Checked-in coverage now verifies `BigInt.prototype.toLocaleString` exists with the expected built-in metadata and is not constructible. Locale-sensitive formatting semantics still need broader dedicated coverage. |

### 21.2.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype.tostring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt.prototype.toString basic semantics | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/BigInt/prototype/toString/ExecutionTests.cs` | `test/built-ins/BigInt/prototype/toString/default-radix.js`<br>`test/built-ins/BigInt/prototype/toString/not-a-constructor.js`<br>`test/built-ins/BigInt/prototype/toString/thisbigintvalue-not-valid-throws.js` | Checked-in coverage now exercises default-radix formatting, `thisBigIntValue` receiver validation, and non-constructibility for `BigInt.prototype.toString`. Wider radix and coercion coverage is still incomplete. |

### 21.2.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype.valueof))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt.prototype.valueOf basic semantics | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/BigInt/prototype/valueOf/ExecutionTests.cs` | `test/built-ins/BigInt/prototype/valueOf/return.js`<br>`test/built-ins/BigInt/prototype/valueOf/this-value-invalid-object-throws.js` | Checked-in coverage now verifies `BigInt.prototype.valueOf` returns the primitive BigInt from both primitive and wrapper receivers and rejects incompatible object receivers. |

### 21.2.3.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-thisbigintvalue))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ThisBigIntValue primitive and wrapper extraction | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/BigInt/prototype/toString/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/BigInt/prototype/valueOf/ExecutionTests.cs` | `test/built-ins/BigInt/prototype/toString/thisbigintvalue-not-valid-throws.js`<br>`test/built-ins/BigInt/prototype/valueOf/return.js`<br>`test/built-ins/BigInt/prototype/valueOf/this-value-invalid-object-throws.js` | The current BigInt prototype implementation now extracts primitive BigInt values from both raw primitives and BigInt wrapper objects for the covered methods, and throws `TypeError` for incompatible receivers. |

### 21.2.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-bigint.prototype-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt.prototype[@@toStringTag] | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/BigInt/prototype/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Object/prototype/toString/ExecutionTests.cs` | `test/built-ins/BigInt/prototype/Symbol.toStringTag.js`<br>`test/built-ins/Object/prototype/toString/Object.prototype.toString.call-bigint.js` | Checked-in coverage now verifies the `@@toStringTag` descriptor on `BigInt.prototype` and the resulting `[object BigInt]` branding for both primitive and wrapper receivers. |

### 21.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-bigint-instances))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BigInt wrapper object properties | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Object/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Object/prototype/toString/ExecutionTests.cs` | `test/built-ins/Object/bigint.js`<br>`test/built-ins/Object/prototype/toString/Object.prototype.toString.call-bigint.js` | Checked-in coverage now verifies that `Object(0n)` produces a branded BigInt wrapper with the expected prototype linkage and primitive `valueOf()` behavior for the covered cases. |

