<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 22.1: String Objects

[Back to Section22](Section22.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 22.1 | String Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-string-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 22.1.1 | The String Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string-constructor) |
| 22.1.1.1 | String ( value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string-constructor-string-value) |
| 22.1.2 | Properties of the String Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-string-constructor) |
| 22.1.2.1 | String.fromCharCode ( ... codeUnits ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.fromcharcode) |
| 22.1.2.2 | String.fromCodePoint ( ... codePoints ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.fromcodepoint) |
| 22.1.2.3 | String.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype) |
| 22.1.2.4 | String.raw ( template , ... substitutions ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.raw) |
| 22.1.3 | Properties of the String Prototype Object | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-string-prototype-object) |
| 22.1.3.1 | String.prototype.at ( index ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.at) |
| 22.1.3.2 | String.prototype.charAt ( pos ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.charat) |
| 22.1.3.3 | String.prototype.charCodeAt ( pos ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.charcodeat) |
| 22.1.3.4 | String.prototype.codePointAt ( pos ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.codepointat) |
| 22.1.3.5 | String.prototype.concat ( ... args ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.concat) |
| 22.1.3.6 | String.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.constructor) |
| 22.1.3.7 | String.prototype.endsWith ( searchString [ , endPosition ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.endswith) |
| 22.1.3.8 | String.prototype.includes ( searchString [ , position ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.includes) |
| 22.1.3.9 | String.prototype.indexOf ( searchString [ , position ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.indexof) |
| 22.1.3.10 | String.prototype.isWellFormed ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.iswellformed) |
| 22.1.3.11 | String.prototype.lastIndexOf ( searchString [ , position ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.lastindexof) |
| 22.1.3.12 | String.prototype.localeCompare ( that [ , reserved1 [ , reserved2 ] ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.localecompare) |
| 22.1.3.13 | String.prototype.match ( regexp ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.match) |
| 22.1.3.14 | String.prototype.matchAll ( regexp ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.matchall) |
| 22.1.3.15 | String.prototype.normalize ( [ form ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.normalize) |
| 22.1.3.16 | String.prototype.padEnd ( maxLength [ , fillString ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.padend) |
| 22.1.3.17 | String.prototype.padStart ( maxLength [ , fillString ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.padstart) |
| 22.1.3.17.1 | StringPaddingBuiltinsImpl ( O , maxLength , fillString , placement ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-stringpaddingbuiltinsimpl) |
| 22.1.3.17.2 | StringPad ( S , maxLength , fillString , placement ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-stringpad) |
| 22.1.3.17.3 | ToZeroPaddedDecimalString ( n , minLength ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-tozeropaddeddecimalstring) |
| 22.1.3.18 | String.prototype.repeat ( count ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.repeat) |
| 22.1.3.19 | String.prototype.replace ( searchValue , replaceValue ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.replace) |
| 22.1.3.19.1 | GetSubstitution ( matched , str , position , captures , namedCaptures , replacementTemplate ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getsubstitution) |
| 22.1.3.20 | String.prototype.replaceAll ( searchValue , replaceValue ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.replaceall) |
| 22.1.3.21 | String.prototype.search ( regexp ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.search) |
| 22.1.3.22 | String.prototype.slice ( start , end ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.slice) |
| 22.1.3.23 | String.prototype.split ( separator , limit ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.split) |
| 22.1.3.24 | String.prototype.startsWith ( searchString [ , position ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.startswith) |
| 22.1.3.25 | String.prototype.substring ( start , end ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.substring) |
| 22.1.3.26 | String.prototype.toLocaleLowerCase ( [ reserved1 [ , reserved2 ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolocalelowercase) |
| 22.1.3.27 | String.prototype.toLocaleUpperCase ( [ reserved1 [ , reserved2 ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolocaleuppercase) |
| 22.1.3.28 | String.prototype.toLowerCase ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolowercase) |
| 22.1.3.29 | String.prototype.toString ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tostring) |
| 22.1.3.30 | String.prototype.toUpperCase ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.touppercase) |
| 22.1.3.31 | String.prototype.toWellFormed ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.towellformed) |
| 22.1.3.32 | String.prototype.trim ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trim) |
| 22.1.3.32.1 | TrimString ( string , where ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-trimstring) |
| 22.1.3.33 | String.prototype.trimEnd ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trimend) |
| 22.1.3.34 | String.prototype.trimStart ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trimstart) |
| 22.1.3.35 | String.prototype.valueOf ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.valueof) |
| 22.1.3.35.1 | ThisStringValue ( value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-thisstringvalue) |
| 22.1.3.36 | String.prototype [ %Symbol.iterator% ] ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype-%symbol.iterator%) |
| 22.1.4 | Properties of String Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-string-instances) |
| 22.1.4.1 | length | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-string-instances-length) |
| 22.1.5 | String Iterator Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-string-iterator-objects) |
| 22.1.5.1 | The %StringIteratorPrototype% Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%-object) |
| 22.1.5.1.1 | %StringIteratorPrototype%.next ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%.next) |
| 22.1.5.1.2 | %StringIteratorPrototype% [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%-%symbol.tostringtag%) |

## Support

Feature-level support tracking with test script references.

### 22.1.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.charcodeat))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.charCodeAt | Supported with Limitations | [`String_CharCodeAt_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_CharCodeAt_Basic.js) | Implemented in JavaScriptRuntime.String.CharCodeAt for definite string receivers. Returns a boxed number. Edge cases such as out-of-range indices and surrogate handling are not fully validated. |

### 22.1.3.7 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.endswith))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.endsWith | Supported |  | Implemented in JavaScriptRuntime.String and wired via IL generator for definite string receivers. Supports optional end position. Returns a boolean value. (No dedicated JS fixture currently referenced in this doc.) |

### 22.1.3.8 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.includes))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.includes | Supported |  | Reflection-based dispatch recognizes definite string receivers and routes to JavaScriptRuntime.String.Includes; supports optional position argument. Returns a boolean value. (No dedicated JS fixture currently referenced in this doc.) |

### 22.1.3.12 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.localecompare))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.localeCompare (numeric compare) | Supported | [`String_LocaleCompare_Numeric.js`](../../../Js2IL.Tests/String/JavaScript/String_LocaleCompare_Numeric.js) | Returns a number (boxed double) consistent with ECMAScript compare semantics; numeric option supported. |

### 22.1.3.19 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.replace))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.replace (regex literal, string replacement) | Supported with Limitations | [`String_Replace_Regex_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_Replace_Regex_Global.js) | Supported when the receiver is String(x), the pattern is a regular expression literal, and the replacement is a string. Global (g) and ignoreCase (i) flags are honored. Function replacement, non-regex patterns, and other flags are not yet implemented. Implemented via host intrinsic JavaScriptRuntime.String.Replace and dynamic resolution in IL generator. |

### 22.1.3.23 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.split))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.split | Supported | [`String_Split_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_Split_Basic.js) | Supports string and regular-expression separators and optional limit. Implemented via JavaScriptRuntime.String.Split and returned as JavaScriptRuntime.Array. Separator omitted or undefined returns [input]. Empty string separator splits into individual UTF-16 code units. |

### 22.1.3.24 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.startswith))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.startsWith | Supported | [`String_StartsWith_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_StartsWith_Basic.js) | Reflection-based string dispatch routes CLR string receivers to JavaScriptRuntime.String.StartsWith with optional position argument. Returns a boolean value (boxed). |

### 22.1.3.28 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolowercase))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.toLowerCase | Supported with Limitations | [`String_ToLowerCase_ToUpperCase_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_ToLowerCase_ToUpperCase_Basic.js) | Implemented in JavaScriptRuntime.String.ToLowerCase for definite string receivers. Uses CLR casing semantics; full ECMAScript Unicode case mapping nuances are not guaranteed. |

### 22.1.3.30 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.touppercase))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.toUpperCase | Supported with Limitations | [`String_ToLowerCase_ToUpperCase_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_ToLowerCase_ToUpperCase_Basic.js) | Implemented in JavaScriptRuntime.String.ToUpperCase for definite string receivers. Uses CLR casing semantics; full ECMAScript Unicode case mapping nuances are not guaranteed. |

