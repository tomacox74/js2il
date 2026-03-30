<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 22.2: RegExp (Regular Expression) Objects

[Back to Section22](Section22.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-08T07:05:04Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 22.2 | RegExp (Regular Expression) Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-regexp-regular-expression-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 22.2.1 | Patterns | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-patterns) |
| 22.2.1.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-patterns-static-semantics-early-errors) |
| 22.2.1.2 | Static Semantics: CountLeftCapturingParensWithin ( node ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-countleftcapturingparenswithin) |
| 22.2.1.3 | Static Semantics: CountLeftCapturingParensBefore ( node ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-countleftcapturingparensbefore) |
| 22.2.1.4 | Static Semantics: MightBothParticipate ( x , y ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-mightbothparticipate) |
| 22.2.1.5 | Static Semantics: CapturingGroupNumber | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-patterns-static-semantics-capturing-group-number) |
| 22.2.1.6 | Static Semantics: IsCharacterClass | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-patterns-static-semantics-is-character-class) |
| 22.2.1.7 | Static Semantics: CharacterValue | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-patterns-static-semantics-character-value) |
| 22.2.1.8 | Static Semantics: MayContainStrings | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-maycontainstrings) |
| 22.2.1.9 | Static Semantics: GroupSpecifiersThatMatch ( thisGroupName ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-groupspecifiersthatmatch) |
| 22.2.1.10 | Static Semantics: CapturingGroupName | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-capturinggroupname) |
| 22.2.1.11 | Static Semantics: RegExpIdentifierCodePoints | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexpidentifiercodepoints) |
| 22.2.1.12 | Static Semantics: RegExpIdentifierCodePoint | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexpidentifiercodepoint) |
| 22.2.2 | Pattern Semantics | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-pattern-semantics) |
| 22.2.2.1 | Notation | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-pattern-notation) |
| 22.2.2.1.1 | RegExp Records | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-regexp-records) |
| 22.2.2.2 | Runtime Semantics: CompilePattern | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-compilepattern) |
| 22.2.2.3 | Runtime Semantics: CompileSubpattern | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-compilesubpattern) |
| 22.2.2.3.1 | RepeatMatcher ( m , min , max , greedy , x , c , parenIndex , parenCount ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-repeatmatcher-abstract-operation) |
| 22.2.2.3.2 | EmptyMatcher ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-emptymatcher) |
| 22.2.2.3.3 | MatchTwoAlternatives ( m1 , m2 ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-matchtwoalternatives) |
| 22.2.2.3.4 | MatchSequence ( m1 , m2 , direction ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-matchsequence) |
| 22.2.2.4 | Runtime Semantics: CompileAssertion | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-compileassertion) |
| 22.2.2.4.1 | IsWordChar ( rer , Input , e ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-iswordchar-abstract-operation) |
| 22.2.2.5 | Runtime Semantics: CompileQuantifier | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-compilequantifier) |
| 22.2.2.6 | Runtime Semantics: CompileQuantifierPrefix | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-compilequantifierprefix) |
| 22.2.2.7 | Runtime Semantics: CompileAtom | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-compileatom) |
| 22.2.2.7.1 | CharacterSetMatcher ( rer , A , invert , direction ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-charactersetmatcher-abstract-operation) |
| 22.2.2.7.2 | BackreferenceMatcher ( rer , ns , direction ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-backreference-matcher) |
| 22.2.2.7.3 | Canonicalize ( rer , ch ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-canonicalize-ch) |
| 22.2.2.7.4 | UpdateModifiers ( rer , add , remove ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-updatemodifiers) |
| 22.2.2.8 | Runtime Semantics: CompileCharacterClass | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-compilecharacterclass) |
| 22.2.2.9 | Runtime Semantics: CompileToCharSet | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-compiletocharset) |
| 22.2.2.9.1 | CharacterRange ( A , B ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-characterrange-abstract-operation) |
| 22.2.2.9.2 | HasEitherUnicodeFlag ( rer ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-haseitherunicodeflag-abstract-operation) |
| 22.2.2.9.3 | WordCharacters ( rer ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-wordcharacters) |
| 22.2.2.9.4 | AllCharacters ( rer ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-allcharacters) |
| 22.2.2.9.5 | MaybeSimpleCaseFolding ( rer , A ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-maybesimplecasefolding) |
| 22.2.2.9.6 | CharacterComplement ( rer , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-charactercomplement) |
| 22.2.2.9.7 | UnicodeMatchProperty ( rer , p ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-unicodematchproperty-p) |
| 22.2.2.9.8 | UnicodeMatchPropertyValue ( p , v ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-unicodematchpropertyvalue-p-v) |
| 22.2.2.10 | Runtime Semantics: CompileClassSetString | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-compileclasssetstring) |
| 22.2.3 | Abstract Operations for RegExp Creation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-regexp-creation) |
| 22.2.3.1 | RegExpCreate ( P , F ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexpcreate) |
| 22.2.3.2 | RegExpAlloc ( newTarget ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexpalloc) |
| 22.2.3.3 | RegExpInitialize ( obj , pattern , flags ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexpinitialize) |
| 22.2.3.4 | Static Semantics: ParsePattern ( patternText , u , v ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-parsepattern) |
| 22.2.4 | The RegExp Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexp-constructor) |
| 22.2.4.1 | RegExp ( pattern , flags ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexp-pattern-flags) |
| 22.2.5 | Properties of the RegExp Constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-regexp-constructor) |
| 22.2.5.1 | RegExp.escape ( S ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-regexp.escape) |
| 22.2.5.1.1 | EncodeForRegExpEscape ( cp ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-encodeforregexpescape) |
| 22.2.5.2 | RegExp.prototype | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype) |
| 22.2.5.3 | get RegExp [ %Symbol.species% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp-%symbol.species%) |
| 22.2.6 | Properties of the RegExp Prototype Object | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-regexp-prototype-object) |
| 22.2.6.1 | RegExp.prototype.constructor | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.constructor) |
| 22.2.6.2 | RegExp.prototype.exec ( string ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.exec) |
| 22.2.6.3 | get RegExp.prototype.dotAll | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.dotAll) |
| 22.2.6.4 | get RegExp.prototype.flags | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.flags) |
| 22.2.6.4.1 | RegExpHasFlag ( R , codeUnit ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexphasflag) |
| 22.2.6.5 | get RegExp.prototype.global | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.global) |
| 22.2.6.6 | get RegExp.prototype.hasIndices | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.hasIndices) |
| 22.2.6.7 | get RegExp.prototype.ignoreCase | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.ignorecase) |
| 22.2.6.8 | RegExp.prototype [ %Symbol.match% ] ( string ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.match%) |
| 22.2.6.9 | RegExp.prototype [ %Symbol.matchAll% ] ( string ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-regexp-prototype-%symbol.matchall%) |
| 22.2.6.10 | get RegExp.prototype.multiline | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.multiline) |
| 22.2.6.11 | RegExp.prototype [ %Symbol.replace% ] ( string , replaceValue ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.replace%) |
| 22.2.6.12 | RegExp.prototype [ %Symbol.search% ] ( string ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.search%) |
| 22.2.6.13 | get RegExp.prototype.source | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.source) |
| 22.2.6.13.1 | EscapeRegExpPattern ( P , F ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-escaperegexppattern) |
| 22.2.6.14 | RegExp.prototype [ %Symbol.split% ] ( string , limit ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.split%) |
| 22.2.6.15 | get RegExp.prototype.sticky | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.sticky) |
| 22.2.6.16 | RegExp.prototype.test ( S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.test) |
| 22.2.6.17 | RegExp.prototype.toString ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.tostring) |
| 22.2.6.18 | get RegExp.prototype.unicode | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.unicode) |
| 22.2.6.19 | get RegExp.prototype.unicodeSets | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.unicodesets) |
| 22.2.7 | Abstract Operations for RegExp Matching | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-regexp-matching) |
| 22.2.7.1 | RegExpExec ( R , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexpexec) |
| 22.2.7.2 | RegExpBuiltinExec ( R , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexpbuiltinexec) |
| 22.2.7.3 | AdvanceStringIndex ( S , index , unicode ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-advancestringindex) |
| 22.2.7.4 | GetStringIndex ( S , codePointIndex ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-getstringindex) |
| 22.2.7.5 | Match Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-match-records) |
| 22.2.7.6 | GetMatchString ( S , match ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getmatchstring) |
| 22.2.7.7 | GetMatchIndexPair ( S , match ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getmatchindexpair) |
| 22.2.7.8 | MakeMatchIndicesIndexPairArray ( S , indices , groupNames , hasGroups ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makematchindicesindexpairarray) |
| 22.2.8 | Properties of RegExp Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-regexp-instances) |
| 22.2.8.1 | lastIndex | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-lastindex) |
| 22.2.9 | RegExp String Iterator Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-regexp-string-iterator-objects) |
| 22.2.9.1 | CreateRegExpStringIterator ( R , S , global , fullUnicode ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-createregexpstringiterator) |
| 22.2.9.2 | The %RegExpStringIteratorPrototype% Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%regexpstringiteratorprototype%-object) |
| 22.2.9.2.1 | %RegExpStringIteratorPrototype%.next ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%regexpstringiteratorprototype%.next) |
| 22.2.9.2.2 | %RegExpStringIteratorPrototype% [ %Symbol.toStringTag% ] | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%regexpstringiteratorprototype%-%symbol.tostringtag%) |
| 22.2.9.3 | Properties of RegExp String Iterator Instances | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-regexp-string-iterator-instances) |

## Support

Feature-level support tracking with test script references.

### 22.2.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-regexp-pattern-flags))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| RegExp ( pattern , flags ) | Supported with Limitations | [`IntrinsicCallables_RegExp_Callable_CreatesRegex.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Callable_CreatesRegex.js)<br>[`IntrinsicCallables_RegExp_Flags_Getter.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Flags_Getter.js)<br>[`IntrinsicCallables_RegExp_ModernFlags_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_ModernFlags_Basic.js)<br>[`IntrinsicCallables_RegExp_Sticky_Getters.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Sticky_Getters.js)<br>[`IntrinsicCallables_RegExp_ToString_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_ToString_Basic.js) | Lowered by the compiler as an intrinsic constructor-like call to JavaScriptRuntime.RegExp (and regex literals /.../flags are lowered as new RegExp(pattern, flags)). The g, i, m, s, u, d, and y flags are parsed and reflected today; v is explicitly rejected with a SyntaxError diagnostic. Unicode behavior is still a limited subset backed by .NET Regex plus common-case rewrites for code-point escapes and dot handling, not full ECMAScript parity. |

### 22.2.6.2 ([tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.exec))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| RegExp.prototype.exec | Supported with Limitations | [`String_RegExp_Exec_LastIndex_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_RegExp_Exec_LastIndex_Global.js)<br>[`String_RegExp_Exec_LastIndex_Sticky.js`](../../../Js2IL.Tests/String/JavaScript/String_RegExp_Exec_LastIndex_Sticky.js)<br>[`IntrinsicCallables_RegExp_Indices_Exec.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Indices_Exec.js) | Implemented in JavaScriptRuntime.RegExp.exec as a minimal subset. Supports returning an Array of captures, attaching .index and .input, honoring lastIndex for both /g and /y regexes, and attaching a minimal .indices array when the d flag is present. Full RegExp exotic object semantics, named groups on indices, and complete Unicode mode parity are still not implemented. |

### 22.2.6.3 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.dotAll))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.dotAll | Supported with Limitations | [`IntrinsicCallables_RegExp_Getters_Extended.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Getters_Extended.js)<br>[`IntrinsicCallables_RegExp_ModernFlags_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_ModernFlags_Basic.js) | Implemented in JavaScriptRuntime.RegExp.dotAll from the stored s flag. Backed by .NET Regex Singleline for the non-unicode path and by the runtime's limited unicode-pattern rewriting when /su is used. |

### 22.2.6.4 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.flags))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.flags | Supported with Limitations | [`IntrinsicCallables_RegExp_Flags_Getter.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Flags_Getter.js)<br>[`IntrinsicCallables_RegExp_ModernFlags_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_ModernFlags_Basic.js)<br>[`IntrinsicCallables_RegExp_Sticky_Getters.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Sticky_Getters.js) | Implemented in JavaScriptRuntime.RegExp.flags. Returns flags in canonical order (dgimsuvy). The g, i, m, s, u, d, and y flags are currently reflected; v is rejected during construction. |

### 22.2.6.5 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.global))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.global | Supported with Limitations | [`IntrinsicCallables_RegExp_Prototype_Getters_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Prototype_Getters_Basic.js) | Implemented in JavaScriptRuntime.RegExp based on stored flags. Full RegExp exotic object semantics and cross-realm behaviors are not implemented. |

### 22.2.6.6 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.hasIndices))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.hasIndices | Supported with Limitations | [`IntrinsicCallables_RegExp_Getters_Extended.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Getters_Extended.js)<br>[`IntrinsicCallables_RegExp_Indices_Exec.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Indices_Exec.js)<br>[`IntrinsicCallables_RegExp_ModernFlags_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_ModernFlags_Basic.js) | Implemented in JavaScriptRuntime.RegExp.hasIndices from the stored d flag. exec() attaches a minimal .indices property containing [start, end] pairs for the overall match and each capture, with unmatched captures represented as null. Named groups on indices are not yet implemented. |

### 22.2.6.7 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.ignorecase))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.ignoreCase | Supported with Limitations | [`IntrinsicCallables_RegExp_Prototype_Getters_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Prototype_Getters_Basic.js) | Implemented in JavaScriptRuntime.RegExp based on stored flags. |

### 22.2.6.8 ([tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.match%))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| RegExp.prototype [ %Symbol.match% ] | Supported with Limitations | [`String_Match_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_Match_Global.js)<br>[`String_Match_NonGlobal.js`](../../../Js2IL.Tests/String/JavaScript/String_Match_NonGlobal.js)<br>[`String_RegExp_SymbolDispatch_Custom.js`](../../../Js2IL.Tests/String/JavaScript/String_RegExp_SymbolDispatch_Custom.js) | Implemented by exposing a symbol-keyed matcher on RegExp instances and by routing String.prototype.match through well-known symbol dispatch. Custom objects with Symbol.match overrides are honored. The runtime still models only a subset of RegExp exotic behavior. |

### 22.2.6.10 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.multiline))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.multiline | Supported with Limitations | [`IntrinsicCallables_RegExp_Prototype_Getters_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Prototype_Getters_Basic.js) | Implemented in JavaScriptRuntime.RegExp based on stored flags. |

### 22.2.6.11 ([tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.replace%))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| RegExp.prototype [ %Symbol.replace% ] | Supported with Limitations | [`String_Replace_Regex_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_Replace_Regex_Global.js)<br>[`String_RegExp_SymbolDispatch_Custom.js`](../../../Js2IL.Tests/String/JavaScript/String_RegExp_SymbolDispatch_Custom.js) | Implemented by exposing a symbol-keyed replacer on RegExp instances and by routing String.prototype.replace through well-known symbol dispatch. Custom Symbol.replace overrides are honored. Replacement pattern semantics remain a pragmatic subset and do not yet cover every ECMAScript replacement token edge case. |

### 22.2.6.12 ([tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.search%))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| RegExp.prototype [ %Symbol.search% ] | Supported with Limitations | [`String_Search_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_Search_Basic.js)<br>[`String_RegExp_SymbolDispatch_Custom.js`](../../../Js2IL.Tests/String/JavaScript/String_RegExp_SymbolDispatch_Custom.js) | Implemented by exposing a symbol-keyed searcher on RegExp instances and by routing String.prototype.search through well-known symbol dispatch. The built-in path preserves the caller's lastIndex after searching. Custom Symbol.search overrides are honored. |

### 22.2.6.13 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.source))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.source | Supported with Limitations | [`IntrinsicCallables_RegExp_Prototype_Getters_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Prototype_Getters_Basic.js) | Implemented in JavaScriptRuntime.RegExp using the original pattern text. Does not attempt to normalize/escape patterns exactly per spec. |

### 22.2.6.14 ([tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.split%))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| RegExp.prototype [ %Symbol.split% ] | Supported with Limitations | [`String_Split_Regex_Basic.js`](../../../Js2IL.Tests/String/JavaScript/String_Split_Regex_Basic.js)<br>[`String_RegExp_SymbolDispatch_Custom.js`](../../../Js2IL.Tests/String/JavaScript/String_RegExp_SymbolDispatch_Custom.js) | Implemented by exposing a symbol-keyed splitter on RegExp instances and by routing String.prototype.split through well-known symbol dispatch. Custom Symbol.split overrides are honored. Built-in regex splitting currently uses a pragmatic subset and does not yet model every capture-insertion corner case. |

### 22.2.6.15 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.sticky))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.sticky | Supported with Limitations | [`IntrinsicCallables_RegExp_Sticky_Getters.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Sticky_Getters.js) | Implemented in JavaScriptRuntime.RegExp.sticky from the stored y flag. Sticky regexes use lastIndex-constrained matching in exec() and test(); matchAll and some broader RegExp exotic semantics remain incomplete. |

### 22.2.6.16 ([tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.test))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| RegExp.prototype.test | Supported with Limitations | [`IntrinsicCallables_RegExp_Test_LastIndex_Global.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Test_LastIndex_Global.js)<br>[`IntrinsicCallables_RegExp_Test_LastIndex_Sticky.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Test_LastIndex_Sticky.js)<br>[`IntrinsicCallables_RegExp_ModernFlags_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_ModernFlags_Basic.js) | Implemented in JavaScriptRuntime.RegExp.test. Updates lastIndex for both global (/g) and sticky (/y) regexes using the same matching rules as exec(), and participates in the runtime's limited unicode-aware empty-match advancement for /u regexes. Complete Unicode mode parity is still not implemented. |

### 22.2.6.17 ([tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.tostring))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| RegExp.prototype.toString | Supported with Limitations | [`IntrinsicCallables_RegExp_ToString_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_ToString_Basic.js) | Implemented in JavaScriptRuntime.RegExp.toString returning /source/flags format. Does not handle all exotic object semantics. |

### 22.2.6.18 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.unicode))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.unicode | Supported with Limitations | [`IntrinsicCallables_RegExp_Getters_Extended.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Getters_Extended.js)<br>[`IntrinsicCallables_RegExp_ModernFlags_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_ModernFlags_Basic.js) | Implemented in JavaScriptRuntime.RegExp.unicode from the stored u flag. The runtime supports a limited subset of unicode behavior by rewriting common code-point escapes and dot handling before compiling with .NET Regex. Property escapes, unicode sets, and full ECMAScript Unicode-mode parity are still not implemented. |

### 22.2.6.19 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.unicodesets))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.unicodeSets | Supported with Limitations | [`IntrinsicCallables_RegExp_Getters_Extended.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Getters_Extended.js)<br>[`IntrinsicCallables_RegExp_ModernFlags_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_ModernFlags_Basic.js) | Implemented in JavaScriptRuntime.RegExp.unicodeSets. The getter remains false for supported regexes, and the v flag is explicitly rejected during RegExp construction with a SyntaxError because UnicodeSets mode is not implemented. |

### 22.2.8.1 ([tc39.es](https://tc39.es/ecma262/#sec-lastindex))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| RegExp instance lastIndex | Supported with Limitations | [`String_RegExp_Exec_LastIndex_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_RegExp_Exec_LastIndex_Global.js)<br>[`String_RegExp_Exec_LastIndex_Sticky.js`](../../../Js2IL.Tests/String/JavaScript/String_RegExp_Exec_LastIndex_Sticky.js)<br>[`IntrinsicCallables_RegExp_Test_LastIndex_Sticky.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Test_LastIndex_Sticky.js) | Implemented in JavaScriptRuntime.RegExp as a numeric property. Participates in exec() and test() for both /g and /y regexes, and resets to 0 after failed sticky/global matches or when the stored start position is past the end of the input. |

