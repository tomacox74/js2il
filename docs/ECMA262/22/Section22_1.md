<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 22.1: String Objects

[Back to Section22](Section22.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-29T04:34:40Z

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
| 22.1.3.5 | String.prototype.concat ( ... args ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.concat) |
| 22.1.3.6 | String.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.constructor) |
| 22.1.3.7 | String.prototype.endsWith ( searchString [ , endPosition ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.endswith) |
| 22.1.3.8 | String.prototype.includes ( searchString [ , position ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.includes) |
| 22.1.3.9 | String.prototype.indexOf ( searchString [ , position ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.indexof) |
| 22.1.3.10 | String.prototype.isWellFormed ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.iswellformed) |
| 22.1.3.11 | String.prototype.lastIndexOf ( searchString [ , position ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.lastindexof) |
| 22.1.3.12 | String.prototype.localeCompare ( that [ , reserved1 [ , reserved2 ] ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.localecompare) |
| 22.1.3.13 | String.prototype.match ( regexp ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.match) |
| 22.1.3.14 | String.prototype.matchAll ( regexp ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.matchall) |
| 22.1.3.15 | String.prototype.normalize ( [ form ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.normalize) |
| 22.1.3.16 | String.prototype.padEnd ( maxLength [ , fillString ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.padend) |
| 22.1.3.17 | String.prototype.padStart ( maxLength [ , fillString ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.padstart) |
| 22.1.3.17.1 | StringPaddingBuiltinsImpl ( O , maxLength , fillString , placement ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-stringpaddingbuiltinsimpl) |
| 22.1.3.17.2 | StringPad ( S , maxLength , fillString , placement ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-stringpad) |
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
| 22.1.3.26 | String.prototype.toLocaleLowerCase ( [ reserved1 [ , reserved2 ] ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolocalelowercase) |
| 22.1.3.27 | String.prototype.toLocaleUpperCase ( [ reserved1 [ , reserved2 ] ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolocaleuppercase) |
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

Feature-level support tracking with test script references.

### 22.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-string-constructor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String constructor (call/new) | Supported with Limitations | [`String_New_Sugar.js`](../../../Js2IL.Tests/String/JavaScript/String_New_Sugar.js) | new String(x) and String(x) are treated as sugar for DotNet2JSConversions.ToString without creating wrapper String objects; primitive string is returned and no [[StringData]]/boxed behaviors are exposed. |

### 22.1.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-string-constructor-string-value))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String ( value ) | Supported with Limitations | [`String_New_Sugar.js`](../../../Js2IL.Tests/String/JavaScript/String_New_Sugar.js) | Runtime coercion follows DotNet2JSConversions.ToString and always returns a primitive string; wrapper object semantics (property attributes, prototype chain) are not implemented. |

### 22.1.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-string.fromcharcode))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.fromCharCode | Supported with Limitations | [`String_FromCharCode_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_FromCharCode_Basic.js)<br>[`Array_Slice_FromCharCode_Apply.js`](../../../Js2IL.Tests/Array/JavaScript/Array_Slice_FromCharCode_Apply.js) | Implemented via JavaScriptRuntime.String.FromCharCode and exposed on GlobalThis.String.fromCharCode. Supports ToNumber coercion and ToUint16 code-unit truncation for single and variadic arguments. Does not aim to match all edge-case observable behaviors (e.g., exotic receivers, property attributes) beyond common library usage. |

### 22.1.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-string.fromcodepoint))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.fromCodePoint | Supported with Limitations | [`String_NewApis_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_NewApis_Basic.js) | Implemented in JavaScriptRuntime.String.FromCodePoint and exposed on GlobalThis.String.fromCodePoint. Supports integer Unicode scalar values and throws RangeError for invalid code points; boxed String wrapper edge cases are still not modeled. |

### 22.1.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype object | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../Js2IL.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) | GlobalThis.String.prototype is now exposed as a shared runtime object. Primitive string property reads consult that prototype surface for methods like constructor, toString, valueOf, at, and @@iterator, but js2il still does not create observable boxed String wrapper objects. |

### 22.1.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-string.raw))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.raw | Supported with Limitations | [`String_NewApis_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_NewApis_Basic.js) | Implemented in JavaScriptRuntime.String.Raw and exposed as GlobalThis.String.raw. Supports array-like template.raw values used by tagged-template helpers, but does not attempt full spec fidelity for exotic template objects or property attributes. |

### 22.1.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.at))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.at | Supported with Limitations | [`String_NewApis_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_NewApis_Basic.js)<br>[`String_Prototype_Iterator_Surface.js`](../../../Js2IL.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) | Implemented in JavaScriptRuntime.String.At with negative-index support and exposed both through string fast paths and String.prototype. Out-of-range results surface as js2il's undefined/null representation rather than a boxed wrapper value. |

### 22.1.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.charat))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.charAt | Supported with Limitations | [`String_CharAt_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_CharAt_Basic.js) | Implemented via JavaScriptRuntime.String.CharAt and exposed on the shared String.prototype surface while retaining direct string fast paths. js2il still skips boxed String wrapper semantics and full exotic-property observability. |

### 22.1.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.charcodeat))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.charCodeAt | Supported with Limitations | [`String_CharCodeAt_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_CharCodeAt_Basic.js) | Implemented in JavaScriptRuntime.String.CharCodeAt and exposed on the shared String.prototype surface. Returns a boxed number and preserves js2il's current undefined/null modeling rather than full boxed-wrapper semantics. |

### 22.1.3.4 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.codepointat))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.codePointAt | Supported with Limitations | [`String_NewApis_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_NewApis_Basic.js) | Implemented in JavaScriptRuntime.String.CodePointAt with surrogate-pair decoding for valid leading surrogates. Out-of-range results use js2il's undefined/null representation and boxed String wrapper edge cases remain unsupported. |

### 22.1.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.concat))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.concat | Not Yet Supported |  | No dedicated concat method is wired; string concatenation is handled via operators/ToString coercion only. |

### 22.1.3.6 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.constructor))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.constructor | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../Js2IL.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) | String.prototype.constructor is exposed and points at the shared GlobalThis.String constructor value. Since js2il still treats new String(x) as primitive-string sugar, boxed constructor behaviors remain limited. |

### 22.1.3.7 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.endswith))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.endsWith | Supported with Limitations | [`Require_Path_Parse_And_Format.js`](../../../Js2IL.Tests/Node/Path/JavaScript/Require_Path_Parse_And_Format.js) | Routed via JavaScriptRuntime.Object string member-call fast paths to JavaScriptRuntime.String.EndsWith with optional endPosition argument. Known differences vs spec: does not reject RegExp searchString, and when called with zero arguments the fast path uses an empty string instead of ToString(undefined). |

### 22.1.3.8 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.includes))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.includes | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../Js2IL.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js)<br>[`FSPromises_Realpath.js`](../../../Js2IL.Tests/Node/FS/JavaScript/FSPromises_Realpath.js) | Routed via JavaScriptRuntime.Object string member-call fast paths to JavaScriptRuntime.String.Includes. Known differences vs spec: does not reject RegExp searchString, and when called with zero arguments the fast path uses an empty string instead of ToString(undefined). |

### 22.1.3.9 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.indexof))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.indexOf | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../Js2IL.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js)<br>[`Function_Prototype_ToString_Basic.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Prototype_ToString_Basic.js) | Implemented in JavaScriptRuntime.String.IndexOf and routed via JavaScriptRuntime.Object string member-call fast paths. Known differences vs spec: when called with zero arguments the fast path searches for an empty string instead of ToString(undefined). |

### 22.1.3.10 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.iswellformed))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.isWellFormed | Supported with Limitations | [`String_NewApis_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_NewApis_Basic.js) | Implemented in JavaScriptRuntime.String.IsWellFormed for UTF-16 surrogate validation. The helper reflects string primitive contents only; there is no separate boxed String data slot model. |

### 22.1.3.11 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.lastindexof))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.lastIndexOf | Supported with Limitations | [`String_LastIndexOf_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_LastIndexOf_Basic.js) | Implemented in JavaScriptRuntime.String.LastIndexOf with ordinal search. Argument defaulting follows the fast-path behavior rather than full ToString/ToIntegerOrInfinity edge cases. |

### 22.1.3.12 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.localecompare))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.localeCompare (numeric compare) | Supported | [`String_LocaleCompare_Numeric.js`](../../../Js2IL.Tests/String/JavaScript/String_LocaleCompare_Numeric.js) | Returns a number (boxed double) consistent with ECMAScript compare semantics; numeric option supported. |

### 22.1.3.13 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.match))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.match | Supported with Limitations | [`String_Match_NonGlobal.js`](../../../Js2IL.Tests/String/JavaScript/String_Match_NonGlobal.js)<br>[`String_Match_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_Match_Global.js) | Implemented via JavaScriptRuntime.String.Match with limited RegExp integration. For /g, returns an Array of full-match substrings (or null). For non-global, returns an exec-like Array with groups and sets .index/.input. Symbol.match and advanced RegExp behaviors are not implemented. |

### 22.1.3.14 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.matchall))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.matchAll | Supported with Limitations | [`String_MatchAll_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_MatchAll_Basic.js) | Implemented in JavaScriptRuntime.String.MatchAll for string patterns and global RegExp inputs. The current runtime eagerly materializes the matches as a JavaScriptRuntime.Array of exec-like match arrays instead of the spec's lazy RegExp String Iterator. |

### 22.1.3.15 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.normalize))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.normalize | Not Yet Supported |  | Unicode normalization forms are not exposed; there is no runtime hook for normalize. |

### 22.1.3.16 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.padend))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.padEnd | Supported with Limitations | [`String_NewApis_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_NewApis_Basic.js) | Implemented in JavaScriptRuntime.String.PadEnd using a shared padding helper. Behavior covers common string/fill-string cases but does not yet model every abstract-operation edge case from the spec text. |

### 22.1.3.17 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.padstart))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.padStart | Supported with Limitations | [`String_NewApis_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_NewApis_Basic.js) | Implemented in JavaScriptRuntime.String.PadStart using a shared padding helper. Behavior covers common string/fill-string cases but does not yet model every abstract-operation edge case from the spec text. |

### 22.1.3.17.1 ([tc39.es](https://tc39.es/ecma262/#sec-stringpaddingbuiltinsimpl))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| StringPaddingBuiltinsImpl | Not Yet Supported |  | Helper algorithm for padStart/padEnd is not present. |

### 22.1.3.17.2 ([tc39.es](https://tc39.es/ecma262/#sec-stringpad))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| StringPad | Not Yet Supported |  | No shared padding implementation exists in the runtime. |

### 22.1.3.17.3 ([tc39.es](https://tc39.es/ecma262/#sec-tozeropaddeddecimalstring))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ToZeroPaddedDecimalString | Not Yet Supported |  | Helper algorithm for formatting numbers during padding is not implemented. |

### 22.1.3.18 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.repeat))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.repeat | Supported with Limitations | [`String_Repeat_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_Repeat_Basic.js) | Implemented in JavaScriptRuntime.String.Repeat with RangeError for negative / non-finite counts and a guard against extremely large outputs. |

### 22.1.3.19 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.replace))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.replace (regex literal, string replacement) | Supported with Limitations | [`String_Replace_Regex_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_Replace_Regex_Global.js) | Supported when the receiver is String(x), the pattern is a regular expression literal, and the replacement is a string. Global (g) and ignoreCase (i) flags are honored. Function replacement, non-regex patterns, and other flags are not yet implemented. Implemented via host intrinsic JavaScriptRuntime.String.Replace and dynamic resolution in IL generator. |

### 22.1.3.19.1 ([tc39.es](https://tc39.es/ecma262/#sec-getsubstitution))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| GetSubstitution | Not Yet Supported |  | Replacement template processing is limited; full GetSubstitution semantics (named captures, $<name>, etc.) are not present. |

### 22.1.3.20 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.replaceall))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.replaceAll | Supported with Limitations | [`String_NewApis_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_NewApis_Basic.js) | Implemented in JavaScriptRuntime.String.ReplaceAll for literal-string search values, callback replacements, and global RegExp inputs. It enforces the non-global RegExp TypeError but still uses js2il's simplified replacement-template semantics. |

### 22.1.3.21 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.search))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.search | Supported with Limitations | [`String_Search_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_Search_Basic.js) | Implemented via JavaScriptRuntime.String.Search for string and RegExp inputs and routed through string member-call fast paths when receiver typing is stable. Symbol.search customization and full RegExp @@search protocol hooks are not implemented. |

### 22.1.3.22 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.slice))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.slice | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../Js2IL.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js) | Implemented in JavaScriptRuntime.String.Slice with best-effort ToIntegerOrInfinity-style coercion and negative index handling. Edge-case observable behaviors are not exhaustively validated. |

### 22.1.3.23 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.split))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.split | Supported with Limitations | [`String_Split_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_Split_Basic.js) | Implemented via JavaScriptRuntime.String.Split and returned as JavaScriptRuntime.Array. Separator omitted or undefined returns [input]. Empty string separator splits into individual UTF-16 code units. RegExp separator behavior is incomplete: JavaScriptRuntime.RegExp is currently coerced to string rather than treated as a regex separator, and @@split hooks are not implemented. |

### 22.1.3.24 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.startswith))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.startsWith | Supported with Limitations | [`String_StartsWith_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_StartsWith_Basic.js)<br>[`String_StartsWith_NestedParam.js`](../../../Js2IL.Tests/String/JavaScript/String_StartsWith_NestedParam.js) | Routed via JavaScriptRuntime.Object string member-call fast paths to JavaScriptRuntime.String.StartsWith. Known differences vs spec: does not reject RegExp searchString, and when called with zero arguments the fast path uses an empty string instead of ToString(undefined). |

### 22.1.3.25 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.substring))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.substring | Supported | [`String_Substring.js`](../../../Js2IL.Tests/String/JavaScript/String_Substring.js) | Implemented in JavaScriptRuntime.String.Substring. Coerces arguments via ToNumber, clamps to [0, length], truncates toward zero, and swaps start/end when start > end. |

### 22.1.3.26 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolocalelowercase))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.toLocaleLowerCase | Not Yet Supported |  | Locale-aware casing helpers are not wired; only invariant toLowerCase/toUpperCase are available. |

### 22.1.3.27 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolocaleuppercase))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.toLocaleUpperCase | Not Yet Supported |  | Locale-aware casing helpers are not wired; only invariant toLowerCase/toUpperCase are available. |

### 22.1.3.28 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tolowercase))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.toLowerCase | Supported with Limitations | [`String_ToLowerCase_ToUpperCase_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_ToLowerCase_ToUpperCase_Basic.js) | Implemented in JavaScriptRuntime.String.ToLowerCase for definite string receivers. Uses CLR casing semantics; full ECMAScript Unicode case mapping nuances are not guaranteed. |

### 22.1.3.29 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.tostring))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.toString | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../Js2IL.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) | String.prototype.toString is now exposed on the public String.prototype object and validates that the receiver is string-compatible. Because js2il does not create boxed String objects, wrapper-object edge cases stay limited. |

### 22.1.3.30 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.touppercase))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.toUpperCase | Supported with Limitations | [`String_ToLowerCase_ToUpperCase_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_ToLowerCase_ToUpperCase_Basic.js) | Implemented in JavaScriptRuntime.String.ToUpperCase for definite string receivers. Uses CLR casing semantics; full ECMAScript Unicode case mapping nuances are not guaranteed. |

### 22.1.3.31 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.towellformed))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.toWellFormed | Supported with Limitations | [`String_NewApis_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_NewApis_Basic.js) | Implemented in JavaScriptRuntime.String.ToWellFormed by replacing lone surrogates with U+FFFD. The helper targets primitive string contents rather than a spec-observable boxed String instance. |

### 22.1.3.32 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trim))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.trim | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../Js2IL.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js) | Implemented in JavaScriptRuntime.String.Trim via TrimEcma (explicit ECMAScript whitespace set). Not exhaustively validated against all edge-case observable behaviors (e.g., exotic receivers / property attributes). |

### 22.1.3.32.1 ([tc39.es](https://tc39.es/ecma262/#sec-trimstring))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| TrimString | Not Yet Supported |  | Shared TrimString abstract operation is not tracked separately; runtime uses custom TrimEcma helpers. |

### 22.1.3.33 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trimend))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.trimEnd | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../Js2IL.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js) | Implemented in JavaScriptRuntime.String.TrimEnd (and TrimRight alias) via TrimEndEcma (explicit ECMAScript whitespace set). |

### 22.1.3.34 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.trimstart))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.trimStart | Supported with Limitations | [`String_MemberCall_FastPath_CommonMethods.js`](../../../Js2IL.Tests/String/JavaScript/String_MemberCall_FastPath_CommonMethods.js) | Implemented in JavaScriptRuntime.String.TrimStart (and TrimLeft alias) via TrimStartEcma (explicit ECMAScript whitespace set). |

### 22.1.3.35 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype.valueof))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype.valueOf | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../Js2IL.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) | String.prototype.valueOf is now exposed on the public String.prototype object and returns the primitive string value for compatible receivers. Wrapper-object and [[StringData]] fidelity remain limited because js2il does not allocate boxed String objects. |

### 22.1.3.35.1 ([tc39.es](https://tc39.es/ecma262/#sec-thisstringvalue))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ThisStringValue | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../Js2IL.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) | JavaScriptRuntime.String.ThisStringValue now validates and unwraps string-compatible receivers for the public String.prototype surface. It supports primitive strings plus js2il's direct string helpers, but there is still no distinct boxed String object model. |

### 22.1.3.36 ([tc39.es](https://tc39.es/ecma262/#sec-string.prototype-%symbol.iterator%))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String.prototype[@@iterator] | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../Js2IL.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) | String.prototype[@@iterator] is now exposed and returns public string iterator objects that iterate Unicode code points (surrogate pairs stay intact). The runtime still models strings as primitives without full wrapper-object/property-attribute fidelity. |

### 22.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-string-instances))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String instance exotic object | Supported with Limitations | [`String_FromCharCode_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_FromCharCode_Basic.js) | String values remain primitives; property access relies on intrinsic fast paths rather than creating String objects with indexed exotic behaviors. Length and known member dispatch work, but property descriptors/boxing semantics are not implemented. |

### 22.1.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-string-instances-length))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String length property | Supported with Limitations | [`String_FromCharCode_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_FromCharCode_Basic.js) | Length is taken from the underlying .NET string (UTF-16 code units). Because strings are not boxed, property attributes (non-writable, non-enumerable) are not modeled. |

### 22.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-string-iterator-objects))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| String iterator objects | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../Js2IL.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) | Public string iterator objects are now created by String.prototype[@@iterator] and by the internal fast path used for for..of/Array.from. They preserve code-point iteration but do not yet implement every spec-visible internal slot or helper abstraction. |

### 22.1.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%-object))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| %StringIteratorPrototype% | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../Js2IL.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) | The runtime now exposes a shared %StringIteratorPrototype% object and uses it as the prototype for returned string iterators. It is observable through Object.getPrototypeOf on iterator instances. |

### 22.1.5.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%.next))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| %StringIteratorPrototype%.next | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../Js2IL.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) | The public %StringIteratorPrototype%.next implementation advances through Unicode code points and returns iterator result objects. Iterator closing/return semantics stay aligned with js2il's broader iterator model rather than the full spec algorithm text. |

### 22.1.5.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-%stringiteratorprototype%-%symbol.tostringtag%))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| %StringIteratorPrototype% [ %Symbol.toStringTag% ] | Supported with Limitations | [`String_Prototype_Iterator_Surface.js`](../../../Js2IL.Tests/String/JavaScript/String_Prototype_Iterator_Surface.js) | The shared %StringIteratorPrototype% now exposes Symbol.toStringTag as "String Iterator". |

