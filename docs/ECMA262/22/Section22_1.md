<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 22.1: String Objects

[Back to Section22](Section22.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-23T04:49:10Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 22.1 | String Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 22.1.1 | The String Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string-constructor) |
| 22.1.1.1 | String ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string-constructor-string-value) |
| 22.1.2 | Properties of the String Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-string-constructor) |
| 22.1.2.1 | String.fromCharCode ( ... codeUnits ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.fromcharcode) |
| 22.1.2.2 | String.fromCodePoint ( ... codePoints ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.fromcodepoint) |
| 22.1.2.3 | String.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype) |
| 22.1.2.4 | String.raw ( template , ... substitutions ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.raw) |
| 22.1.3 | Properties of the String Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-string-prototype-object) |
| 22.1.3.1 | String.prototype.at ( index ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.at) |
| 22.1.3.2 | String.prototype.charAt ( pos ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.charat) |
| 22.1.3.3 | String.prototype.charCodeAt ( pos ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.charcodeat) |
| 22.1.3.4 | String.prototype.codePointAt ( pos ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.codepointat) |
| 22.1.3.5 | String.prototype.concat ( ... args ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.concat) |
| 22.1.3.6 | String.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.constructor) |
| 22.1.3.7 | String.prototype.endsWith ( searchString [ , endPosition ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.endswith) |
| 22.1.3.8 | String.prototype.includes ( searchString [ , position ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.includes) |
| 22.1.3.9 | String.prototype.indexOf ( searchString [ , position ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.indexof) |
| 22.1.3.10 | String.prototype.isWellFormed ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.iswellformed) |
| 22.1.3.11 | String.prototype.lastIndexOf ( searchString [ , position ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.lastindexof) |
| 22.1.3.12 | String.prototype.localeCompare ( that [ , reserved1 [ , reserved2 ] ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.localecompare) |
| 22.1.3.13 | String.prototype.match ( regexp ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.match) |
| 22.1.3.14 | String.prototype.matchAll ( regexp ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.matchall) |
| 22.1.3.15 | String.prototype.normalize ( [ form ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.normalize) |
| 22.1.3.16 | String.prototype.padEnd ( maxLength [ , fillString ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.padend) |
| 22.1.3.17 | String.prototype.padStart ( maxLength [ , fillString ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.padstart) |
| 22.1.3.17.1 | StringPaddingBuiltinsImpl ( O , maxLength , fillString , placement ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-stringpaddingbuiltinsimpl) |
| 22.1.3.17.2 | StringPad ( S , maxLength , fillString , placement ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-stringpad) |
| 22.1.3.17.3 | ToZeroPaddedDecimalString ( n , minLength ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-tozeropaddeddecimalstring) |
| 22.1.3.18 | String.prototype.repeat ( count ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.repeat) |
| 22.1.3.19 | String.prototype.replace ( searchValue , replaceValue ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.replace) |
| 22.1.3.19.1 | GetSubstitution ( matched , str , position , captures , namedCaptures , replacementTemplate ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-getsubstitution) |
| 22.1.3.20 | String.prototype.replaceAll ( searchValue , replaceValue ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.replaceall) |
| 22.1.3.21 | String.prototype.search ( regexp ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.search) |
| 22.1.3.22 | String.prototype.slice ( start , end ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.slice) |
| 22.1.3.23 | String.prototype.split ( separator , limit ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.split) |
| 22.1.3.24 | String.prototype.startsWith ( searchString [ , position ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.startswith) |
| 22.1.3.25 | String.prototype.substring ( start , end ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.substring) |
| 22.1.3.26 | String.prototype.toLocaleLowerCase ( [ reserved1 [ , reserved2 ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolocalelowercase) |
| 22.1.3.27 | String.prototype.toLocaleUpperCase ( [ reserved1 [ , reserved2 ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolocaleuppercase) |
| 22.1.3.28 | String.prototype.toLowerCase ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolowercase) |
| 22.1.3.29 | String.prototype.toString ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tostring) |
| 22.1.3.30 | String.prototype.toUpperCase ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.touppercase) |
| 22.1.3.31 | String.prototype.toWellFormed ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.towellformed) |
| 22.1.3.32 | String.prototype.trim ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trim) |
| 22.1.3.32.1 | TrimString ( string , where ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-trimstring) |
| 22.1.3.33 | String.prototype.trimEnd ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trimend) |
| 22.1.3.34 | String.prototype.trimStart ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trimstart) |
| 22.1.3.35 | String.prototype.valueOf ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.valueof) |
| 22.1.3.35.1 | ThisStringValue ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-thisstringvalue) |
| 22.1.3.36 | String.prototype [ %Symbol.iterator% ] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype-%symbol.iterator%) |
| 22.1.4 | Properties of String Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-string-instances) |
| 22.1.4.1 | length | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-string-instances-length) |
| 22.1.5 | String Iterator Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string-iterator-objects) |
| 22.1.5.1 | The %StringIteratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%-object) |
| 22.1.5.1.1 | %StringIteratorPrototype%.next ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%.next) |
| 22.1.5.1.2 | %StringIteratorPrototype% [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%-%symbol.tostringtag%) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 22.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-string-constructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String constructor (call/new) | Supported with Limitations | [`String_New_Sugar.js`](../../../tests/Jroc.Tests/String/JavaScript/String_New_Sugar.js) |  | new String(x) and String(x) are treated as sugar for DotNet2JSConversions.ToString without creating wrapper String objects; primitive string is returned and no [[StringData]]/boxed behaviors are exposed. |

### 22.1.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-string-constructor-string-value))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String ( value ) | Supported with Limitations | [`String_New_Sugar.js`](../../../tests/Jroc.Tests/String/JavaScript/String_New_Sugar.js)<br>[`S15.5.1.1_A1_T14.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/JavaScript/S15.5.1.1_A1_T14.js)<br>[`S15.5.1.1_A1_T8.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/JavaScript/S15.5.1.1_A1_T8.js) | `test/built-ins/String/S15.5.1.1_A1_T14.js`<br>`test/built-ins/String/S15.5.1.1_A1_T8.js` | Runtime coercion follows DotNet2JSConversions.ToString and always returns a primitive string. It now honors ordinary object-to-primitive coercion for arrays so overridden Array.prototype.toString can participate, and numeric -0 stringifies as "0". Wrapper object semantics (property attributes, prototype chain) are not implemented. |

### 22.1.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-string.fromcharcode))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.fromCharCode | Supported with Limitations | [`String_FromCharCode_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_FromCharCode_Basic.js)<br>[`Array_Slice_FromCharCode_Apply.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_Slice_FromCharCode_Apply.js) |  | Implemented via JavaScriptRuntime.String.FromCharCode and exposed on GlobalThis.String.fromCharCode. Supports ToNumber coercion and ToUint16 code-unit truncation for single and variadic arguments. Does not aim to match all edge-case observable behaviors (e.g., exotic receivers, property attributes) beyond common library usage. |

### 22.1.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-string.fromcodepoint))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.fromCodePoint | Supported with Limitations | [`String_NewApis_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_NewApis_Basic.js) |  | Implemented in JavaScriptRuntime.String.FromCodePoint and exposed on GlobalThis.String.fromCodePoint. Supports integer Unicode scalar values and throws RangeError for invalid code points; boxed String wrapper edge cases are still not modeled. |

### 22.1.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype object | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js)<br>[`S15.5.4_A1.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/prototype/JavaScript/S15.5.4_A1.js)<br>[`S15.5.4_A2.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/prototype/JavaScript/S15.5.4_A2.js)<br>[`S15.5.4_A3.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/prototype/JavaScript/S15.5.4_A3.js) | `test/built-ins/String/prototype/S15.5.4_A1.js`<br>`test/built-ins/String/prototype/S15.5.4_A2.js`<br>`test/built-ins/String/prototype/S15.5.4_A3.js` | GlobalThis.String.prototype is exposed as a shared runtime object with [[StringData]]="" behavior and String toStringTag metadata so legacy prototype-object checks align with the covered spec slice. Primitive string property reads consult that prototype surface for methods like constructor, toString, valueOf, at, and @@iterator; full boxed-string exotic behavior remains limited. |

### 22.1.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-string.raw))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.raw | Supported with Limitations | [`String_NewApis_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_NewApis_Basic.js) |  | Implemented in JavaScriptRuntime.String.Raw and exposed as GlobalThis.String.raw. Supports array-like template.raw values used by tagged-template helpers, but does not attempt full spec fidelity for exotic template objects or property attributes. |

### 22.1.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.at))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.at | Supported with Limitations | [`String_NewApis_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_NewApis_Basic.js)<br>[`String_Prototype_Iterator_Surface.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) |  | Implemented in JavaScriptRuntime.String.At with negative-index support and exposed both through string fast paths and String.prototype. Out-of-range results surface as jroc's undefined/null representation rather than a boxed wrapper value. |

### 22.1.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.charat))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.charAt | Supported with Limitations | [`String_CharAt_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_CharAt_Basic.js) |  | Implemented via JavaScriptRuntime.String.CharAt and exposed on the shared String.prototype surface while retaining direct string fast paths. jroc still skips boxed String wrapper semantics and full exotic-property observability. |

### 22.1.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.charcodeat))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.charCodeAt | Supported with Limitations | [`String_CharCodeAt_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_CharCodeAt_Basic.js) |  | Implemented in JavaScriptRuntime.String.CharCodeAt and exposed on the shared String.prototype surface. Returns a boxed number and preserves jroc's current undefined/null modeling rather than full boxed-wrapper semantics. |

### 22.1.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.codepointat))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.codePointAt | Supported with Limitations | [`String_NewApis_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_NewApis_Basic.js) |  | Implemented in JavaScriptRuntime.String.CodePointAt with surrogate-pair decoding for valid leading surrogates. Out-of-range results use jroc's undefined/null representation and boxed String wrapper edge cases remain unsupported. |

### 22.1.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.concat))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.concat | Supported with Limitations |  | suite `built_ins.String.prototype.concat`<br>`test/built-ins/String/prototype/concat/name.js`<br>`test/built-ins/String/prototype/concat/not-a-constructor.js`<br>`test/built-ins/String/prototype/concat/this-value-not-obj-coercible.js`<br>`test/built-ins/String/prototype/concat/S15.5.4.6_A1_T1.js`<br>`test/built-ins/String/prototype/concat/S15.5.4.6_A1_T2.js`<br>`test/built-ins/String/prototype/concat/S15.5.4.6_A1_T4.js`<br>`test/built-ins/String/prototype/concat/S15.5.4.6_A1_T5.js`<br>`test/built-ins/String/prototype/concat/S15.5.4.6_A1_T6.js`<br>`test/built-ins/String/prototype/concat/S15.5.4.6_A1_T7.js`<br>`test/built-ins/String/prototype/concat/S15.5.4.6_A1_T8.js` | Implemented with a generic receiver and left-to-right argument string coercion. Ten upstream test262 cases cover built-in metadata, receiver validation, no-argument behavior, and common primitive coercions. |

### 22.1.3.6 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.constructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.constructor | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) |  | String.prototype.constructor is exposed and points at the shared GlobalThis.String constructor value. Since jroc still treats new String(x) as primitive-string sugar, boxed constructor behaviors remain limited. |

### 22.1.3.7 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.endswith))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.endsWith | Supported with Limitations | [`Require_Path_Parse_And_Format.js`](../../../tests/Jroc.Tests/Node/Path/JavaScript/Require_Path_Parse_And_Format.js)<br>`tests/Jroc.Test262.Tests/built-ins/String/prototype/endsWith/ExecutionTests.cs` | suite `built_ins.String.prototype.endsWith`<br>`test/built-ins/String/prototype/endsWith/coerced-values-of-position.js`<br>`test/built-ins/String/prototype/endsWith/length.js`<br>`test/built-ins/String/prototype/endsWith/name.js`<br>`test/built-ins/String/prototype/endsWith/not-a-constructor.js`<br>`test/built-ins/String/prototype/endsWith/return-abrupt-from-position-as-symbol.js`<br>`test/built-ins/String/prototype/endsWith/return-abrupt-from-position.js`<br>`test/built-ins/String/prototype/endsWith/return-abrupt-from-searchstring-as-symbol.js`<br>`test/built-ins/String/prototype/endsWith/return-abrupt-from-searchstring-regexp-test.js`<br>`test/built-ins/String/prototype/endsWith/return-abrupt-from-searchstring.js`<br>`test/built-ins/String/prototype/endsWith/return-abrupt-from-this-as-symbol.js`<br>`test/built-ins/String/prototype/endsWith/return-abrupt-from-this.js`<br>`test/built-ins/String/prototype/endsWith/return-false-if-search-start-is-less-than-zero.js`<br>`test/built-ins/String/prototype/endsWith/return-true-if-searchstring-is-empty.js`<br>`test/built-ins/String/prototype/endsWith/searchstring-found-with-position.js`<br>`test/built-ins/String/prototype/endsWith/searchstring-found-without-position.js`<br>`test/built-ins/String/prototype/endsWith/searchstring-is-regexp-throws.js`<br>`test/built-ins/String/prototype/endsWith/searchstring-not-found-with-position.js`<br>`test/built-ins/String/prototype/endsWith/searchstring-not-found-without-position.js`<br>`test/built-ins/String/prototype/endsWith/this-is-null-throws.js`<br>`test/built-ins/String/prototype/endsWith/this-is-undefined-throws.js` | Implemented in JavaScriptRuntime.String.EndsWith and shared by String.prototype and optimized direct-string calls. The covered test262 cases exercise metadata, receiver validation, RegExp and Symbol rejection, Symbol.match lookup abrupt completion, string/position coercion, bounds, and empty searches. Boxed String wrapper and other exotic-object behavior remain limited. |

### 22.1.3.8 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.includes))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.includes | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../tests/Jroc.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js)<br>[`FSPromises_Realpath.js`](../../../tests/Jroc.Tests/Node/FS/JavaScript/FSPromises_Realpath.js)<br>`tests/Jroc.Test262.Tests/built-ins/String/prototype/includes/ExecutionTests.cs` | suite `built_ins.String.prototype.includes`<br>`test/built-ins/String/prototype/includes/coerced-values-of-position.js`<br>`test/built-ins/String/prototype/includes/length.js`<br>`test/built-ins/String/prototype/includes/name.js`<br>`test/built-ins/String/prototype/includes/not-a-constructor.js`<br>`test/built-ins/String/prototype/includes/return-abrupt-from-position-as-symbol.js`<br>`test/built-ins/String/prototype/includes/return-abrupt-from-position.js`<br>`test/built-ins/String/prototype/includes/return-abrupt-from-searchstring-as-symbol.js`<br>`test/built-ins/String/prototype/includes/return-abrupt-from-searchstring-regexp-test.js`<br>`test/built-ins/String/prototype/includes/return-abrupt-from-searchstring.js`<br>`test/built-ins/String/prototype/includes/return-abrupt-from-this-as-symbol.js`<br>`test/built-ins/String/prototype/includes/return-abrupt-from-this.js`<br>`test/built-ins/String/prototype/includes/return-false-with-out-of-bounds-position.js`<br>`test/built-ins/String/prototype/includes/return-true-if-searchstring-is-empty.js`<br>`test/built-ins/String/prototype/includes/searchstring-found-with-position.js`<br>`test/built-ins/String/prototype/includes/searchstring-found-without-position.js`<br>`test/built-ins/String/prototype/includes/searchstring-is-regexp-throws.js`<br>`test/built-ins/String/prototype/includes/searchstring-not-found-with-position.js`<br>`test/built-ins/String/prototype/includes/searchstring-not-found-without-position.js`<br>`test/built-ins/String/prototype/includes/this-is-null-throws.js`<br>`test/built-ins/String/prototype/includes/this-is-undefined-throws.js` | Implemented in JavaScriptRuntime.String.Includes and shared by String.prototype and optimized direct-string calls. The covered test262 cases exercise metadata, receiver validation, RegExp and Symbol rejection, Symbol.match lookup abrupt completion, string/position coercion, bounds, and empty searches. Boxed String wrapper and other exotic-object behavior remain limited. |

### 22.1.3.9 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.indexof))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.indexOf | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../tests/Jroc.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js)<br>[`Function_Prototype_ToString_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Prototype_ToString_Basic.js)<br>[`position-tointeger.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/prototype/indexOf/JavaScript/position-tointeger.js)<br>[`S15.5.4.7_A3_T1.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/prototype/indexOf/JavaScript/S15.5.4.7_A3_T1.js) | `test/built-ins/String/prototype/indexOf/S15.5.4.7_A1_T1.js`<br>`test/built-ins/String/prototype/indexOf/S15.5.4.7_A1_T4.js`<br>`test/built-ins/String/prototype/indexOf/S15.5.4.7_A1_T8.js`<br>`test/built-ins/String/prototype/indexOf/S15.5.4.7_A1_T9.js`<br>`test/built-ins/String/prototype/indexOf/S15.5.4.7_A1_T10.js`<br>`test/built-ins/String/prototype/indexOf/S15.5.4.7_A10.js`<br>`test/built-ins/String/prototype/indexOf/S15.5.4.7_A11.js`<br>`test/built-ins/String/prototype/indexOf/S15.5.4.7_A2_T1.js`<br>`test/built-ins/String/prototype/indexOf/S15.5.4.7_A2_T2.js`<br>`test/built-ins/String/prototype/indexOf/S15.5.4.7_A2_T3.js`<br>`test/built-ins/String/prototype/indexOf/S15.5.4.7_A2_T4.js`<br>`test/built-ins/String/prototype/indexOf/position-tointeger.js`<br>`test/built-ins/String/prototype/indexOf/S15.5.4.7_A3_T1.js` | Implemented in JavaScriptRuntime.String.IndexOf and routed via JavaScriptRuntime.Object string member-call fast paths. Current bounded test262 coverage exercises boxed receivers, zero-argument ToString(undefined) behavior, function-object metadata, and representative position-coercion cases including array object-to-primitive coercion. |

### 22.1.3.10 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.iswellformed))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.isWellFormed | Supported with Limitations | [`String_NewApis_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_NewApis_Basic.js) |  | Implemented in JavaScriptRuntime.String.IsWellFormed for UTF-16 surrogate validation. The helper reflects string primitive contents only; there is no separate boxed String data slot model. |

### 22.1.3.11 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.lastindexof))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.lastIndexOf | Supported with Limitations | [`String_LastIndexOf_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_LastIndexOf_Basic.js)<br>`tests/Jroc.Test262.Tests/built-ins/String/prototype/lastIndexOf/ExecutionTests.cs` | suite `built_ins.String.prototype.lastIndexOf`<br>`test/built-ins/String/prototype/lastIndexOf/name.js`<br>`test/built-ins/String/prototype/lastIndexOf/not-a-constructor.js`<br>`test/built-ins/String/prototype/lastIndexOf/not-a-substring.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A1_T1.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A1_T10.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A1_T12.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A1_T2.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A1_T4.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A1_T5.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A1_T6.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A1_T7.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A1_T8.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A1_T9.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A10.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A11.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A4_T1.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A4_T2.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A4_T3.js`<br>`test/built-ins/String/prototype/lastIndexOf/S15.5.4.8_A4_T4.js`<br>`test/built-ins/String/prototype/lastIndexOf/this-value-not-obj-coercible.js` | Implemented in JavaScriptRuntime.String.LastIndexOf with ordinal search and shared by String.prototype and optimized direct-string calls. The covered test262 cases exercise metadata, receiver validation, search-string and position coercion, zero-argument undefined coercion, bounds, and empty searches. Boxed String wrapper and other exotic-object behavior remain limited. |

### 22.1.3.12 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.localecompare))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.localeCompare (numeric compare) | Supported | [`String_LocaleCompare_Numeric.js`](../../../tests/Jroc.Tests/String/JavaScript/String_LocaleCompare_Numeric.js) |  | Returns a number (boxed double) consistent with ECMAScript compare semantics; numeric option supported. |

### 22.1.3.13 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.match))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.match | Supported with Limitations | [`String_Match_NonGlobal.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Match_NonGlobal.js)<br>[`String_Match_Global.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Match_Global.js) |  | Implemented via JavaScriptRuntime.String.Match with limited RegExp integration. For /g, returns an Array of full-match substrings (or null). For non-global, returns an exec-like Array with .index, .input, and null-prototype named .groups metadata. Symbol.match dispatch is supported, while broader RegExp exotic behavior remains limited. |

### 22.1.3.14 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.matchall))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.matchAll | Supported with Limitations | [`String_MatchAll_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_MatchAll_Basic.js)<br>[`String_RegExp_NamedGroups_Indices.js`](../../../tests/Jroc.Tests/String/JavaScript/String_RegExp_NamedGroups_Indices.js) |  | Implemented in JavaScriptRuntime.String.MatchAll for string patterns and global RegExp inputs. Exec-like match arrays include named .groups and optional .indices.groups metadata. The current runtime eagerly materializes matches as a JavaScriptRuntime.Array instead of the spec's lazy RegExp String Iterator. |

### 22.1.3.15 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.normalize))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.normalize | Supported with Limitations |  | suite `built_ins.String.prototype.normalize`<br>`test/built-ins/String/prototype/normalize/form-is-not-valid-throws.js`<br>`test/built-ins/String/prototype/normalize/length.js`<br>`test/built-ins/String/prototype/normalize/name.js`<br>`test/built-ins/String/prototype/normalize/normalize.js`<br>`test/built-ins/String/prototype/normalize/not-a-constructor.js`<br>`test/built-ins/String/prototype/normalize/return-abrupt-from-form-as-symbol.js`<br>`test/built-ins/String/prototype/normalize/return-abrupt-from-form.js`<br>`test/built-ins/String/prototype/normalize/return-normalized-string-from-coerced-form.js`<br>`test/built-ins/String/prototype/normalize/return-normalized-string-using-default-parameter.js`<br>`test/built-ins/String/prototype/normalize/return-normalized-string.js` | Implemented through .NET Unicode normalization forms with ECMA-262 NFC defaulting, form coercion, invalid-form RangeError behavior, and Symbol rejection. The exact Unicode data version follows the hosted .NET runtime. |

### 22.1.3.16 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.padend))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.padEnd | Supported with Limitations | [`String_NewApis_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_NewApis_Basic.js) | suite `built_ins.String.prototype.padEnd`<br>`test/built-ins/String/prototype/padEnd/exception-fill-string-symbol.js`<br>`test/built-ins/String/prototype/padEnd/exception-not-object-coercible.js`<br>`test/built-ins/String/prototype/padEnd/exception-symbol.js`<br>`test/built-ins/String/prototype/padEnd/fill-string-empty.js`<br>`test/built-ins/String/prototype/padEnd/fill-string-non-strings.js`<br>`test/built-ins/String/prototype/padEnd/fill-string-omitted.js`<br>`test/built-ins/String/prototype/padEnd/function-length.js`<br>`test/built-ins/String/prototype/padEnd/function-name.js`<br>`test/built-ins/String/prototype/padEnd/function-property-descriptor.js`<br>`test/built-ins/String/prototype/padEnd/normal-operation.js` | Implemented by the shared JavaScriptRuntime.String padding helper, including UTF-16 code-unit truncation, default and empty fillers, primitive filler coercion, Symbol rejection, receiver validation, and built-in metadata. Boxed String wrapper and other exotic-object edge cases remain limited. |

### 22.1.3.17 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.padstart))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.padStart | Supported with Limitations | [`String_NewApis_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_NewApis_Basic.js) | suite `built_ins.String.prototype.padStart`<br>`test/built-ins/String/prototype/padStart/exception-fill-string-symbol.js`<br>`test/built-ins/String/prototype/padStart/exception-not-object-coercible.js`<br>`test/built-ins/String/prototype/padStart/exception-symbol.js`<br>`test/built-ins/String/prototype/padStart/fill-string-empty.js`<br>`test/built-ins/String/prototype/padStart/fill-string-non-strings.js`<br>`test/built-ins/String/prototype/padStart/fill-string-omitted.js`<br>`test/built-ins/String/prototype/padStart/function-length.js`<br>`test/built-ins/String/prototype/padStart/function-name.js`<br>`test/built-ins/String/prototype/padStart/function-property-descriptor.js`<br>`test/built-ins/String/prototype/padStart/normal-operation.js` | Implemented by the shared JavaScriptRuntime.String padding helper, including UTF-16 code-unit truncation, default and empty fillers, primitive filler coercion, Symbol rejection, receiver validation, and built-in metadata. Boxed String wrapper and other exotic-object edge cases remain limited. |

### 22.1.3.17.1 ([tc39.es](https://tc39.es/ecma262/#sec-stringpaddingbuiltinsimpl))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| StringPaddingBuiltinsImpl | Supported with Limitations |  |  | Implemented by the shared JavaScriptRuntime.String.Pad helper for String.prototype.padStart and String.prototype.padEnd. It does not expose the abstract operation independently. |

### 22.1.3.17.2 ([tc39.es](https://tc39.es/ecma262/#sec-stringpad))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| StringPad | Supported with Limitations |  |  | Implemented by JavaScriptRuntime.String.BuildPadding with repeated filler truncation measured in UTF-16 code units. It is used internally by the shared padding helper. |

### 22.1.3.17.3 ([tc39.es](https://tc39.es/ecma262/#sec-tozeropaddeddecimalstring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ToZeroPaddedDecimalString | Not Yet Supported |  |  | Helper algorithm for formatting numbers during padding is not implemented. |

### 22.1.3.18 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.repeat))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.repeat | Supported with Limitations | [`String_Repeat_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Repeat_Basic.js) |  | Implemented in JavaScriptRuntime.String.Repeat with RangeError for negative / non-finite counts and a guard against extremely large outputs. |

### 22.1.3.19 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.replace))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.replace (regex literal, string replacement) | Supported with Limitations | [`String_Replace_Regex_Global.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Replace_Regex_Global.js) |  | Supported when the receiver is String(x), the pattern is a regular expression literal, and the replacement is a string. Global (g) and ignoreCase (i) flags are honored. Function replacement, non-regex patterns, and other flags are not yet implemented. Implemented via host intrinsic JavaScriptRuntime.String.Replace and dynamic resolution in IL generator. |

### 22.1.3.19.1 ([tc39.es](https://tc39.es/ecma262/#sec-getsubstitution))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| GetSubstitution | Not Yet Supported |  |  | Replacement template processing is limited; full GetSubstitution semantics (named captures, $<name>, etc.) are not present. |

### 22.1.3.20 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.replaceall))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.replaceAll | Supported with Limitations | [`String_NewApis_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_NewApis_Basic.js) |  | Implemented in JavaScriptRuntime.String.ReplaceAll for literal-string search values, callback replacements, and global RegExp inputs. It enforces the non-global RegExp TypeError but still uses jroc's simplified replacement-template semantics. |

### 22.1.3.21 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.search))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.search | Supported with Limitations | [`String_Search_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Search_Basic.js) |  | Implemented via JavaScriptRuntime.String.Search for string and RegExp inputs and routed through string member-call fast paths when receiver typing is stable. Symbol.search customization and full RegExp @@search protocol hooks are not implemented. |

### 22.1.3.22 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.slice))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.slice | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../tests/Jroc.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js)<br>[`S15.5.4.13_A2_T1.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/prototype/slice/JavaScript/S15.5.4.13_A2_T1.js)<br>[`S15.5.4.13_A2_T2.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/prototype/slice/JavaScript/S15.5.4.13_A2_T2.js)<br>[`S15.5.4.13_A2_T3.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/prototype/slice/JavaScript/S15.5.4.13_A2_T3.js) | `test/built-ins/String/prototype/slice/S15.5.4.13_A1_T1.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A1_T2.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A1_T5.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A1_T6.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A1_T7.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A1_T8.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A1_T9.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A1_T10.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A1_T15.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A10.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A11.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A2_T1.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A2_T2.js`<br>`test/built-ins/String/prototype/slice/S15.5.4.13_A2_T3.js` | Implemented in JavaScriptRuntime.String.Slice with ToIntegerOrInfinity-style coercion, negative index handling, and abrupt-completion propagation from object start/end coercions. Current bounded test262 coverage exercises boxed receivers, function-object receivers, metadata attributes, empty-string cases, and representative negative/omitted bound cases. |

### 22.1.3.23 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.split))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.split | Supported with Limitations | [`String_Split_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Split_Basic.js) | `test/built-ins/String/prototype/split/argument-is-undefined-and-instance-is-string.js`<br>`test/built-ins/String/prototype/split/argument-is-void-0-and-instance-is-string-object-object-have-overrided-to-string-function.js`<br>`test/built-ins/String/prototype/split/argument-is-new-reg-exp-and-instance-is-string-hello.js`<br>`test/built-ins/String/prototype/split/argument-is-regexp-and-instance-is-number.js`<br>`test/built-ins/String/prototype/split/argument-is-regexp-a-z-and-instance-is-string-abc.js`<br>`test/built-ins/String/prototype/split/argument-is-regexp-d-and-instance-is-string-dfe23iu-34-65.js`<br>`test/built-ins/String/prototype/split/arguments-are-new-reg-exp-and-0-and-instance-is-string-hello.js`<br>`test/built-ins/String/prototype/split/arguments-are-new-reg-exp-and-1-and-instance-is-string-hello.js`<br>`test/built-ins/String/prototype/split/arguments-are-false-and-true-and-instance-is-object.js` | Implemented via JavaScriptRuntime.String.Split and returned as JavaScriptRuntime.Array. Separator omitted or undefined returns [input], empty-string and empty-RegExp separators split into individual UTF-16 code units, and common built-in RegExp separators plus ToUint32 limit coercion are now covered by bounded test262 evidence. split still relies on JROC's pragmatic RegExp/@@split integration and does not yet claim full spec fidelity for capture insertion and other exotic separator cases. |

### 22.1.3.24 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.startswith))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.startsWith | Supported with Limitations | [`String_StartsWith_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_StartsWith_Basic.js)<br>[`String_StartsWith_NestedParam.js`](../../../tests/Jroc.Tests/String/JavaScript/String_StartsWith_NestedParam.js)<br>`tests/Jroc.Test262.Tests/built-ins/String/prototype/startsWith/ExecutionTests.cs` | suite `built_ins.String.prototype.startsWith`<br>`test/built-ins/String/prototype/startsWith/coerced-values-of-position.js`<br>`test/built-ins/String/prototype/startsWith/length.js`<br>`test/built-ins/String/prototype/startsWith/name.js`<br>`test/built-ins/String/prototype/startsWith/not-a-constructor.js`<br>`test/built-ins/String/prototype/startsWith/out-of-bounds-position.js`<br>`test/built-ins/String/prototype/startsWith/return-abrupt-from-position-as-symbol.js`<br>`test/built-ins/String/prototype/startsWith/return-abrupt-from-position.js`<br>`test/built-ins/String/prototype/startsWith/return-abrupt-from-searchstring-as-symbol.js`<br>`test/built-ins/String/prototype/startsWith/return-abrupt-from-searchstring-regexp-test.js`<br>`test/built-ins/String/prototype/startsWith/return-abrupt-from-searchstring.js`<br>`test/built-ins/String/prototype/startsWith/return-abrupt-from-this-as-symbol.js`<br>`test/built-ins/String/prototype/startsWith/return-abrupt-from-this.js`<br>`test/built-ins/String/prototype/startsWith/return-true-if-searchstring-is-empty.js`<br>`test/built-ins/String/prototype/startsWith/searchstring-found-with-position.js`<br>`test/built-ins/String/prototype/startsWith/searchstring-found-without-position.js`<br>`test/built-ins/String/prototype/startsWith/searchstring-is-regexp-throws.js`<br>`test/built-ins/String/prototype/startsWith/searchstring-not-found-with-position.js`<br>`test/built-ins/String/prototype/startsWith/searchstring-not-found-without-position.js`<br>`test/built-ins/String/prototype/startsWith/this-is-null-throws.js`<br>`test/built-ins/String/prototype/startsWith/this-is-undefined-throws.js` | Implemented in JavaScriptRuntime.String.StartsWith and shared by String.prototype and optimized direct-string calls. The covered test262 cases exercise metadata, receiver validation, RegExp and Symbol rejection, Symbol.match lookup abrupt completion, string/position coercion, bounds, and empty searches. Boxed String wrapper and other exotic-object behavior remain limited. |

### 22.1.3.25 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.substring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.substring | Supported | [`String_Substring.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Substring.js) |  | Implemented in JavaScriptRuntime.String.Substring. Coerces arguments via ToNumber, clamps to [0, length], truncates toward zero, and swaps start/end when start > end. |

### 22.1.3.26 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolocalelowercase))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.toLocaleLowerCase | Supported with Limitations |  | suite `built_ins.String.prototype.toLocaleLowerCase`<br>`test/built-ins/String/prototype/toLocaleLowerCase/name.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/not-a-constructor.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/this-value-not-obj-coercible.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A10.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A11.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T1.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T2.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A6.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T4.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T5.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T6.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T7.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T8.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T9.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T10.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T11.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T12.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T13.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A1_T14.js`<br>`test/built-ins/String/prototype/toLocaleLowerCase/S15.5.4.17_A2_T1.js` | Implemented in JavaScriptRuntime.String.ToLocaleLowerCase with the host current culture and standard String receiver validation. Twenty upstream test262 cases cover built-in metadata, non-constructibility, nullish receivers, and legacy Unicode casing behavior. Locale-list argument validation and ECMAScript's locale-specific Unicode casing requirements remain limited. |

### 22.1.3.27 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolocaleuppercase))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.toLocaleUpperCase | Supported with Limitations |  | suite `built_ins.String.prototype.toLocaleUpperCase`<br>`test/built-ins/String/prototype/toLocaleUpperCase/name.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/not-a-constructor.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/this-value-not-obj-coercible.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A10.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A11.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T1.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T2.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A6.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T4.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T5.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T6.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T7.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T8.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T9.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T10.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T11.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T12.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T13.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A1_T14.js`<br>`test/built-ins/String/prototype/toLocaleUpperCase/S15.5.4.19_A2_T1.js` | Implemented in JavaScriptRuntime.String.ToLocaleUpperCase with the host current culture and standard String receiver validation. Twenty upstream test262 cases cover built-in metadata, non-constructibility, nullish receivers, and legacy Unicode casing behavior. Locale-list argument validation and ECMAScript's locale-specific Unicode casing requirements remain limited. |

### 22.1.3.28 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolowercase))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.toLowerCase | Supported with Limitations | [`String_ToLowerCase_ToUpperCase_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_ToLowerCase_ToUpperCase_Basic.js) |  | Implemented in JavaScriptRuntime.String.ToLowerCase for definite string receivers. Uses CLR casing semantics; full ECMAScript Unicode case mapping nuances are not guaranteed. |

### 22.1.3.29 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tostring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.toString | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) |  | String.prototype.toString is now exposed on the public String.prototype object and validates that the receiver is string-compatible. Because jroc does not create boxed String objects, wrapper-object edge cases stay limited. |

### 22.1.3.30 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.touppercase))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.toUpperCase | Supported with Limitations | [`String_ToLowerCase_ToUpperCase_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_ToLowerCase_ToUpperCase_Basic.js) |  | Implemented in JavaScriptRuntime.String.ToUpperCase for definite string receivers. Uses CLR casing semantics; full ECMAScript Unicode case mapping nuances are not guaranteed. |

### 22.1.3.31 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.towellformed))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.toWellFormed | Supported with Limitations | [`String_NewApis_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_NewApis_Basic.js) |  | Implemented in JavaScriptRuntime.String.ToWellFormed by replacing lone surrogates with U+FFFD. The helper targets primitive string contents rather than a spec-observable boxed String instance. |

### 22.1.3.32 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trim))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.trim | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../tests/Jroc.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js) |  | Implemented in JavaScriptRuntime.String.Trim via TrimEcma (explicit ECMAScript whitespace set). Not exhaustively validated against all edge-case observable behaviors (e.g., exotic receivers / property attributes). |

### 22.1.3.32.1 ([tc39.es](https://tc39.es/ecma262/#sec-trimstring))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| TrimString | Not Yet Supported |  |  | Shared TrimString abstract operation is not tracked separately; runtime uses custom TrimEcma helpers. |

### 22.1.3.33 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trimend))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.trimEnd | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../tests/Jroc.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js) |  | Implemented in JavaScriptRuntime.String.TrimEnd (and TrimRight alias) via TrimEndEcma (explicit ECMAScript whitespace set). |

### 22.1.3.34 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trimstart))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.trimStart | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../tests/Jroc.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js) |  | Implemented in JavaScriptRuntime.String.TrimStart (and TrimLeft alias) via TrimStartEcma (explicit ECMAScript whitespace set). |

### 22.1.3.35 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.valueof))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype.valueOf | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) |  | String.prototype.valueOf is now exposed on the public String.prototype object and returns the primitive string value for compatible receivers. Wrapper-object and [[StringData]] fidelity remain limited because jroc does not allocate boxed String objects. |

### 22.1.3.35.1 ([tc39.es](https://tc39.es/ecma262/#sec-thisstringvalue))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ThisStringValue | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) |  | JavaScriptRuntime.String.ThisStringValue now validates and unwraps string-compatible receivers for the public String.prototype surface. It supports primitive strings plus jroc's direct string helpers, but there is still no distinct boxed String object model. |

### 22.1.3.36 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype-%symbol.iterator%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String.prototype[@@iterator] | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js)<br>[`name.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/prototype/Symbol.iterator/JavaScript/name.js)<br>[`not-a-constructor.js`](../../../tests/Jroc.Test262.Tests/built-ins/String/prototype/Symbol.iterator/JavaScript/not-a-constructor.js) | `test/built-ins/String/prototype/Symbol.iterator/name.js`<br>`test/built-ins/String/prototype/Symbol.iterator/not-a-constructor.js` | String.prototype[@@iterator] is exposed as a non-constructible built-in and returns public string iterator objects that iterate Unicode code points (surrogate pairs stay intact). Name/descriptor metadata for the method is now aligned with [Symbol.iterator] expectations in the covered test262 slice. |

### 22.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-string-instances))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String instance exotic object | Supported with Limitations | [`String_FromCharCode_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_FromCharCode_Basic.js) |  | String values remain primitives; property access relies on intrinsic fast paths rather than creating String objects with indexed exotic behaviors. Length and known member dispatch work, but property descriptors/boxing semantics are not implemented. |

### 22.1.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-string-instances-length))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String length property | Supported with Limitations | [`String_FromCharCode_Basic.js`](../../../tests/Jroc.Tests/String/JavaScript/String_FromCharCode_Basic.js) |  | Length is taken from the underlying .NET string (UTF-16 code units). Because strings are not boxed, property attributes (non-writable, non-enumerable) are not modeled. |

### 22.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-string-iterator-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| String iterator objects | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) |  | Public string iterator objects are now created by String.prototype[@@iterator] and by the internal fast path used for for..of/Array.from. They preserve code-point iteration but do not yet implement every spec-visible internal slot or helper abstraction. |

### 22.1.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| %StringIteratorPrototype% | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) |  | The runtime now exposes a shared %StringIteratorPrototype% object and uses it as the prototype for returned string iterators. It is observable through Object.getPrototypeOf on iterator instances. |

### 22.1.5.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%.next))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| %StringIteratorPrototype%.next | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) |  | The public %StringIteratorPrototype%.next implementation advances through Unicode code points and returns iterator result objects. Iterator closing/return semantics stay aligned with jroc's broader iterator model rather than the full spec algorithm text. |

### 22.1.5.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| %StringIteratorPrototype% [ %Symbol.toStringTag% ] | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../tests/Jroc.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) |  | The shared %StringIteratorPrototype% now exposes Symbol.toStringTag as "String Iterator". |

