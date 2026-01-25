<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 7.1: Type Conversion

[Back to Section7](Section7.md) | [Back to Index](../Index.md)

Type conversion in JS2IL is implemented on an as-needed basis for supported language features and intrinsics. Some conversions are partial/minimal implementations intended to support specific call sites (e.g., BigInt(value)).

| Clause | Title | Status | Link |
|---:|---|---|---|
| 7.1 | Type Conversion | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-type-conversion) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 7.1.1 | ToPrimitive ( input [ , preferredType ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-toprimitive) |
| 7.1.1.1 | OrdinaryToPrimitive ( O , hint ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-ordinarytoprimitive) |
| 7.1.2 | ToBoolean ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-toboolean) |
| 7.1.3 | ToNumeric ( value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-tonumeric) |
| 7.1.4 | ToNumber ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-tonumber) |
| 7.1.4.1 | ToNumber Applied to the String Type | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-tonumber-applied-to-the-string-type) |
| 7.1.4.1.1 | StringToNumber ( str ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-stringtonumber) |
| 7.1.4.1.2 | Runtime Semantics: StringNumericValue | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-stringnumericvalue) |
| 7.1.4.1.3 | RoundMVResult ( n ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-roundmvresult) |
| 7.1.5 | ToIntegerOrInfinity ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-tointegerorinfinity) |
| 7.1.6 | ToInt32 ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-toint32) |
| 7.1.7 | ToUint32 ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-touint32) |
| 7.1.8 | ToInt16 ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-toint16) |
| 7.1.9 | ToUint16 ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-touint16) |
| 7.1.10 | ToInt8 ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-toint8) |
| 7.1.11 | ToUint8 ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-touint8) |
| 7.1.12 | ToUint8Clamp ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-touint8clamp) |
| 7.1.13 | ToBigInt ( argument ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-tobigint) |
| 7.1.14 | StringToBigInt ( str ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-stringtobigint) |
| 7.1.14.1 | StringIntegerLiteral Grammar | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-stringintegerliteral-grammar) |
| 7.1.14.2 | Runtime Semantics: MV | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-mv-for-stringintegerliteral) |
| 7.1.15 | ToBigInt64 ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-tobigint64) |
| 7.1.16 | ToBigUint64 ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-tobiguint64) |
| 7.1.17 | ToString ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-tostring) |
| 7.1.18 | ToObject ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-toobject) |
| 7.1.19 | ToPropertyKey ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-topropertykey) |
| 7.1.20 | ToLength ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-tolength) |
| 7.1.21 | CanonicalNumericIndexString ( argument ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-canonicalnumericindexstring) |
| 7.1.22 | ToIndex ( value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-toindex) |

## Support

Feature-level support tracking with test script references.

### 7.1.13 ([tc39.es](https://tc39.es/ecma262/#sec-tobigint))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToBigInt (minimal, via BigInt(value)) | Partially Supported | [`IntrinsicCallables_BigInt_Callable_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_BigInt_Callable_Basic.js) | Implemented only to the extent required by the current BigInt(value) callable support. Full spec conversion rules (including full ToPrimitive/valueOf/toString behavior and error message/edge-case fidelity) are not yet implemented. |

### 7.1.14 ([tc39.es](https://tc39.es/ecma262/#sec-stringtobigint))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| StringToBigInt (minimal decimal parsing) | Partially Supported | [`IntrinsicCallables_BigInt_Callable_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_BigInt_Callable_Basic.js) | Currently supports basic decimal string parsing used by BigInt(value). Does not implement the full StringIntegerLiteral grammar (binary/octal/hex prefixes, separators, etc.) or all spec-mandated error cases. |

