<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 19.2: Function Properties of the Global Object

[Back to Section19](Section19.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-27T12:25:27Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 19.2 | Function Properties of the Global Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-properties-of-the-global-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 19.2.1 | eval ( x ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-eval-x) |
| 19.2.1.1 | PerformEval ( x , strictCaller , direct ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-performeval) |
| 19.2.1.2 | HostEnsureCanCompileStrings ( calleeRealm , parameterStrings , bodyString , direct ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-hostensurecancompilestrings) |
| 19.2.1.3 | EvalDeclarationInstantiation ( body , varEnv , lexEnv , privateEnv , strict ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-evaldeclarationinstantiation) |
| 19.2.2 | isFinite ( number ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isfinite-number) |
| 19.2.3 | isNaN ( number ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-isnan-number) |
| 19.2.4 | parseFloat ( string ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-parsefloat-string) |
| 19.2.5 | parseInt ( string , radix ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-parseint-string-radix) |
| 19.2.6 | URI Handling Functions | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-uri-handling-functions) |
| 19.2.6.1 | decodeURI ( encodedURI ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-decodeuri-encodeduri) |
| 19.2.6.2 | decodeURIComponent ( encodedURIComponent ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-decodeuricomponent-encodeduricomponent) |
| 19.2.6.3 | encodeURI ( uri ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-encodeuri-uri) |
| 19.2.6.4 | encodeURIComponent ( uriComponent ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-encodeuricomponent-uricomponent) |
| 19.2.6.5 | Encode ( string , extraUnescaped ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-encode) |
| 19.2.6.6 | Decode ( string , preserveEscapeSet ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-decode) |
| 19.2.6.7 | ParseHexOctet ( string , position ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-parsehexoctet) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 19.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-isfinite-number))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| isFinite(number) | Supported with Limitations | [`IntrinsicCallables_ParseFloat_IsFinite_Basic.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_ParseFloat_IsFinite_Basic.js)<br>[`IntrinsicCallables_GlobalFunctions_AsValues_Basic.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_GlobalFunctions_AsValues_Basic.js)<br>[`return-false-on-nan-or-infinities.js`](../../../tests/Jroc.Test262.Tests/built-ins/isFinite/JavaScript/return-false-on-nan-or-infinities.js)<br>[`return-true-for-valid-finite-numbers.js`](../../../tests/Jroc.Test262.Tests/built-ins/isFinite/JavaScript/return-true-for-valid-finite-numbers.js)<br>[`tonumber-operations.js`](../../../tests/Jroc.Test262.Tests/built-ins/isFinite/JavaScript/tonumber-operations.js)<br>[`not-a-constructor.js`](../../../tests/Jroc.Test262.Tests/built-ins/isFinite/JavaScript/not-a-constructor.js)<br>[`return-abrupt-from-tonumber-number.js`](../../../tests/Jroc.Test262.Tests/built-ins/isFinite/JavaScript/return-abrupt-from-tonumber-number.js)<br>[`S15.1.2.5_A2.6.js`](../../../tests/Jroc.Test262.Tests/built-ins/isFinite/JavaScript/S15.1.2.5_A2.6.js)<br>[`toprimitive-call-abrupt.js`](../../../tests/Jroc.Test262.Tests/built-ins/isFinite/JavaScript/toprimitive-call-abrupt.js)<br>[`toprimitive-get-abrupt.js`](../../../tests/Jroc.Test262.Tests/built-ins/isFinite/JavaScript/toprimitive-get-abrupt.js) | `test/built-ins/isFinite/return-false-on-nan-or-infinities.js`<br>`test/built-ins/isFinite/return-true-for-valid-finite-numbers.js`<br>`test/built-ins/isFinite/tonumber-operations.js`<br>`test/built-ins/isFinite/not-a-constructor.js`<br>`test/built-ins/isFinite/return-abrupt-from-tonumber-number.js`<br>`test/built-ins/isFinite/S15.1.2.5_A2.6.js`<br>`test/built-ins/isFinite/toprimitive-call-abrupt.js`<br>`test/built-ins/isFinite/toprimitive-get-abrupt.js` | Implemented by JavaScriptRuntime.GlobalThis.isFinite using JS-like ToNumber coercion (JavaScriptRuntime.TypeUtilities.ToNumber) and IEEE-754 finiteness checks. Checked-in coverage now includes non-constructibility, additional primitive coercion cases, and abrupt completion propagation from valueOf/toString and @@toPrimitive lookup/call steps in addition to representative finite/non-finite coercion cases. |

### 19.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-isnan-number))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| isNaN(number) | Supported with Limitations | [`return-false-not-nan-numbers.js`](../../../tests/Jroc.Test262.Tests/built-ins/isNaN/JavaScript/return-false-not-nan-numbers.js)<br>[`return-abrupt-from-tonumber-number-symbol.js`](../../../tests/Jroc.Test262.Tests/built-ins/isNaN/JavaScript/return-abrupt-from-tonumber-number-symbol.js)<br>[`S15.1.2.4_A2.6.js`](../../../tests/Jroc.Test262.Tests/built-ins/isNaN/JavaScript/S15.1.2.4_A2.6.js)<br>[`tonumber-operations.js`](../../../tests/Jroc.Test262.Tests/built-ins/isNaN/JavaScript/tonumber-operations.js)<br>[`toprimitive-valid-result.js`](../../../tests/Jroc.Test262.Tests/built-ins/isNaN/JavaScript/toprimitive-valid-result.js)<br>[`return-true-nan.js`](../../../tests/Jroc.Test262.Tests/built-ins/isNaN/JavaScript/return-true-nan.js)<br>[`not-a-constructor.js`](../../../tests/Jroc.Test262.Tests/built-ins/isNaN/JavaScript/not-a-constructor.js)<br>[`toprimitive-call-abrupt.js`](../../../tests/Jroc.Test262.Tests/built-ins/isNaN/JavaScript/toprimitive-call-abrupt.js)<br>[`toprimitive-get-abrupt.js`](../../../tests/Jroc.Test262.Tests/built-ins/isNaN/JavaScript/toprimitive-get-abrupt.js) | `test/built-ins/isNaN/return-false-not-nan-numbers.js`<br>`test/built-ins/isNaN/return-abrupt-from-tonumber-number-symbol.js`<br>`test/built-ins/isNaN/S15.1.2.4_A2.6.js`<br>`test/built-ins/isNaN/tonumber-operations.js`<br>`test/built-ins/isNaN/toprimitive-valid-result.js`<br>`test/built-ins/isNaN/return-true-nan.js`<br>`test/built-ins/isNaN/not-a-constructor.js`<br>`test/built-ins/isNaN/toprimitive-call-abrupt.js`<br>`test/built-ins/isNaN/toprimitive-get-abrupt.js` | Implemented by JavaScriptRuntime.GlobalThis.isNaN using JS-like ToNumber coercion and NaN classification. Checked-in coverage now includes representative NaN inputs, non-constructibility, primitive coercion cases, abrupt completion propagation from @@toPrimitive lookup/call steps, and prior abrupt ToNumber / non-NaN coercion cases. |

### 19.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-parsefloat-string))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| parseFloat(string) | Supported with Limitations | [`IntrinsicCallables_ParseFloat_IsFinite_Basic.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_ParseFloat_IsFinite_Basic.js)<br>[`IntrinsicCallables_GlobalFunctions_AsValues_Basic.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_GlobalFunctions_AsValues_Basic.js)<br>[`S15.1.2.3_A2_T2.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseFloat/JavaScript/S15.1.2.3_A2_T2.js)<br>[`S15.1.2.3_A2_T3.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseFloat/JavaScript/S15.1.2.3_A2_T3.js)<br>[`15.1.2.3-2-1.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseFloat/JavaScript/15.1.2.3-2-1.js)<br>[`S15.1.2.3_A4_T1.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseFloat/JavaScript/S15.1.2.3_A4_T1.js)<br>[`S15.1.2.3_A4_T2.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseFloat/JavaScript/S15.1.2.3_A4_T2.js)<br>[`S15.1.2.3_A5_T1.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseFloat/JavaScript/S15.1.2.3_A5_T1.js)<br>[`S15.1.2.3_A5_T2.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseFloat/JavaScript/S15.1.2.3_A5_T2.js) | `test/built-ins/parseFloat/S15.1.2.3_A2_T2.js`<br>`test/built-ins/parseFloat/S15.1.2.3_A2_T3.js`<br>`test/built-ins/parseFloat/15.1.2.3-2-1.js`<br>`test/built-ins/parseFloat/S15.1.2.3_A4_T1.js`<br>`test/built-ins/parseFloat/S15.1.2.3_A4_T2.js`<br>`test/built-ins/parseFloat/S15.1.2.3_A5_T1.js`<br>`test/built-ins/parseFloat/S15.1.2.3_A5_T2.js` | Implemented by JavaScriptRuntime.GlobalThis.parseFloat. Checked-in coverage now includes leading ASCII and NBSP whitespace trimming, primitive ToString coercion, malformed decimal/exponent tails that stop parsing at the first invalid character, and representative invalid-suffix cases in addition to the existing intrinsic-callable coverage. |

### 19.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-parseint-string-radix))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| parseInt(string, radix) | Supported | [`IntrinsicCallables_ParseInt_Basic.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_ParseInt_Basic.js)<br>[`IntrinsicCallables_ParseInt_Spec.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_ParseInt_Spec.js)<br>[`IntrinsicCallables_GlobalFunctions_AsValues_Basic.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_GlobalFunctions_AsValues_Basic.js)<br>[`S15.1.2.2_A2_T2.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseInt/JavaScript/S15.1.2.2_A2_T2.js)<br>[`15.1.2.2-2-1.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseInt/JavaScript/15.1.2.2-2-1.js)<br>[`S15.1.2.2_A5.1_T1.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseInt/JavaScript/S15.1.2.2_A5.1_T1.js)<br>[`S15.1.2.2_A6.1_T1.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseInt/JavaScript/S15.1.2.2_A6.1_T1.js)<br>[`S15.1.2.2_A7.2_T1.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseInt/JavaScript/S15.1.2.2_A7.2_T1.js)<br>[`S15.1.2.2_A8.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseInt/JavaScript/S15.1.2.2_A8.js)<br>[`not-a-constructor.js`](../../../tests/Jroc.Test262.Tests/built-ins/parseInt/JavaScript/not-a-constructor.js) | `test/built-ins/parseInt/S15.1.2.2_A2_T2.js`<br>`test/built-ins/parseInt/15.1.2.2-2-1.js`<br>`test/built-ins/parseInt/S15.1.2.2_A5.1_T1.js`<br>`test/built-ins/parseInt/S15.1.2.2_A6.1_T1.js`<br>`test/built-ins/parseInt/S15.1.2.2_A7.2_T1.js`<br>`test/built-ins/parseInt/S15.1.2.2_A8.js`<br>`test/built-ins/parseInt/not-a-constructor.js` | Implemented by JavaScriptRuntime.GlobalThis.parseInt following ECMA-262 §19.2.5. Supports: leading/trailing whitespace, sign handling, radix coercion via ToInt32 (§7.1.6), hex prefix detection (0x/0X), digit scanning with stop-at-first-invalid, case-insensitive alphabetic digits (A-Z), ToString coercion including primitive values, Array join, and custom toString methods, and large numbers using double arithmetic. Checked-in coverage now also includes additional whitespace/radix edge cases plus non-constructibility. |

