<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 22.2: RegExp (Regular Expression) Objects

[Back to Section22](Section22.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 22.2 | RegExp (Regular Expression) Objects | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-regexp-regular-expression-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 22.2.1 | Patterns | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-patterns) |
| 22.2.1.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-patterns-static-semantics-early-errors) |
| 22.2.1.2 | Static Semantics: CountLeftCapturingParensWithin ( node ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-countleftcapturingparenswithin) |
| 22.2.1.3 | Static Semantics: CountLeftCapturingParensBefore ( node ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-countleftcapturingparensbefore) |
| 22.2.1.4 | Static Semantics: MightBothParticipate ( x , y ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-mightbothparticipate) |
| 22.2.1.5 | Static Semantics: CapturingGroupNumber | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-patterns-static-semantics-capturing-group-number) |
| 22.2.1.6 | Static Semantics: IsCharacterClass | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-patterns-static-semantics-is-character-class) |
| 22.2.1.7 | Static Semantics: CharacterValue | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-patterns-static-semantics-character-value) |
| 22.2.1.8 | Static Semantics: MayContainStrings | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-maycontainstrings) |
| 22.2.1.9 | Static Semantics: GroupSpecifiersThatMatch ( thisGroupName ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-groupspecifiersthatmatch) |
| 22.2.1.10 | Static Semantics: CapturingGroupName | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-capturinggroupname) |
| 22.2.1.11 | Static Semantics: RegExpIdentifierCodePoints | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexpidentifiercodepoints) |
| 22.2.1.12 | Static Semantics: RegExpIdentifierCodePoint | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexpidentifiercodepoint) |
| 22.2.2 | Pattern Semantics | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-pattern-semantics) |
| 22.2.2.1 | Notation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-pattern-notation) |
| 22.2.2.1.1 | RegExp Records | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp-records) |
| 22.2.2.2 | Runtime Semantics: CompilePattern | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-compilepattern) |
| 22.2.2.3 | Runtime Semantics: CompileSubpattern | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-compilesubpattern) |
| 22.2.2.3.1 | RepeatMatcher ( m , min , max , greedy , x , c , parenIndex , parenCount ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-repeatmatcher-abstract-operation) |
| 22.2.2.3.2 | EmptyMatcher ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-emptymatcher) |
| 22.2.2.3.3 | MatchTwoAlternatives ( m1 , m2 ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-matchtwoalternatives) |
| 22.2.2.3.4 | MatchSequence ( m1 , m2 , direction ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-matchsequence) |
| 22.2.2.4 | Runtime Semantics: CompileAssertion | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-compileassertion) |
| 22.2.2.4.1 | IsWordChar ( rer , Input , e ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-iswordchar-abstract-operation) |
| 22.2.2.5 | Runtime Semantics: CompileQuantifier | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-compilequantifier) |
| 22.2.2.6 | Runtime Semantics: CompileQuantifierPrefix | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-compilequantifierprefix) |
| 22.2.2.7 | Runtime Semantics: CompileAtom | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-compileatom) |
| 22.2.2.7.1 | CharacterSetMatcher ( rer , A , invert , direction ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-charactersetmatcher-abstract-operation) |
| 22.2.2.7.2 | BackreferenceMatcher ( rer , ns , direction ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-backreference-matcher) |
| 22.2.2.7.3 | Canonicalize ( rer , ch ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-canonicalize-ch) |
| 22.2.2.7.4 | UpdateModifiers ( rer , add , remove ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-updatemodifiers) |
| 22.2.2.8 | Runtime Semantics: CompileCharacterClass | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-compilecharacterclass) |
| 22.2.2.9 | Runtime Semantics: CompileToCharSet | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-compiletocharset) |
| 22.2.2.9.1 | CharacterRange ( A , B ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-characterrange-abstract-operation) |
| 22.2.2.9.2 | HasEitherUnicodeFlag ( rer ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-haseitherunicodeflag-abstract-operation) |
| 22.2.2.9.3 | WordCharacters ( rer ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-wordcharacters) |
| 22.2.2.9.4 | AllCharacters ( rer ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-allcharacters) |
| 22.2.2.9.5 | MaybeSimpleCaseFolding ( rer , A ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-maybesimplecasefolding) |
| 22.2.2.9.6 | CharacterComplement ( rer , S ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-charactercomplement) |
| 22.2.2.9.7 | UnicodeMatchProperty ( rer , p ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-unicodematchproperty-p) |
| 22.2.2.9.8 | UnicodeMatchPropertyValue ( p , v ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-unicodematchpropertyvalue-p-v) |
| 22.2.2.10 | Runtime Semantics: CompileClassSetString | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-compileclasssetstring) |
| 22.2.3 | Abstract Operations for RegExp Creation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-regexp-creation) |
| 22.2.3.1 | RegExpCreate ( P , F ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexpcreate) |
| 22.2.3.2 | RegExpAlloc ( newTarget ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexpalloc) |
| 22.2.3.3 | RegExpInitialize ( obj , pattern , flags ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexpinitialize) |
| 22.2.3.4 | Static Semantics: ParsePattern ( patternText , u , v ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-parsepattern) |
| 22.2.4 | The RegExp Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp-constructor) |
| 22.2.4.1 | RegExp ( pattern , flags ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp-pattern-flags) |
| 22.2.5 | Properties of the RegExp Constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-regexp-constructor) |
| 22.2.5.1 | RegExp.escape ( S ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp.escape) |
| 22.2.5.1.1 | EncodeForRegExpEscape ( cp ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-encodeforregexpescape) |
| 22.2.5.2 | RegExp.prototype | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype) |
| 22.2.5.3 | get RegExp [ %Symbol.species% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp-%symbol.species%) |
| 22.2.6 | Properties of the RegExp Prototype Object | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-regexp-prototype-object) |
| 22.2.6.1 | RegExp.prototype.constructor | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.constructor) |
| 22.2.6.2 | RegExp.prototype.exec ( string ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.exec) |
| 22.2.6.3 | get RegExp.prototype.dotAll | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.dotAll) |
| 22.2.6.4 | get RegExp.prototype.flags | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.flags) |
| 22.2.6.4.1 | RegExpHasFlag ( R , codeUnit ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexphasflag) |
| 22.2.6.5 | get RegExp.prototype.global | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.global) |
| 22.2.6.6 | get RegExp.prototype.hasIndices | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.hasIndices) |
| 22.2.6.7 | get RegExp.prototype.ignoreCase | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.ignorecase) |
| 22.2.6.8 | RegExp.prototype [ %Symbol.match% ] ( string ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.match%) |
| 22.2.6.9 | RegExp.prototype [ %Symbol.matchAll% ] ( string ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp-prototype-%symbol.matchall%) |
| 22.2.6.10 | get RegExp.prototype.multiline | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.multiline) |
| 22.2.6.11 | RegExp.prototype [ %Symbol.replace% ] ( string , replaceValue ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.replace%) |
| 22.2.6.12 | RegExp.prototype [ %Symbol.search% ] ( string ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.search%) |
| 22.2.6.13 | get RegExp.prototype.source | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.source) |
| 22.2.6.13.1 | EscapeRegExpPattern ( P , F ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-escaperegexppattern) |
| 22.2.6.14 | RegExp.prototype [ %Symbol.split% ] ( string , limit ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype-%symbol.split%) |
| 22.2.6.15 | get RegExp.prototype.sticky | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.sticky) |
| 22.2.6.16 | RegExp.prototype.test ( S ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.test) |
| 22.2.6.17 | RegExp.prototype.toString ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.tostring) |
| 22.2.6.18 | get RegExp.prototype.unicode | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.unicode) |
| 22.2.6.19 | get RegExp.prototype.unicodeSets | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.unicodesets) |
| 22.2.7 | Abstract Operations for RegExp Matching | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-regexp-matching) |
| 22.2.7.1 | RegExpExec ( R , S ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexpexec) |
| 22.2.7.2 | RegExpBuiltinExec ( R , S ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexpbuiltinexec) |
| 22.2.7.3 | AdvanceStringIndex ( S , index , unicode ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-advancestringindex) |
| 22.2.7.4 | GetStringIndex ( S , codePointIndex ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getstringindex) |
| 22.2.7.5 | Match Records | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-match-records) |
| 22.2.7.6 | GetMatchString ( S , match ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getmatchstring) |
| 22.2.7.7 | GetMatchIndexPair ( S , match ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getmatchindexpair) |
| 22.2.7.8 | MakeMatchIndicesIndexPairArray ( S , indices , groupNames , hasGroups ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-makematchindicesindexpairarray) |
| 22.2.8 | Properties of RegExp Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-regexp-instances) |
| 22.2.8.1 | lastIndex | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-lastindex) |
| 22.2.9 | RegExp String Iterator Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regexp-string-iterator-objects) |
| 22.2.9.1 | CreateRegExpStringIterator ( R , S , global , fullUnicode ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-createregexpstringiterator) |
| 22.2.9.2 | The %RegExpStringIteratorPrototype% Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%regexpstringiteratorprototype%-object) |
| 22.2.9.2.1 | %RegExpStringIteratorPrototype%.next ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%regexpstringiteratorprototype%.next) |
| 22.2.9.2.2 | %RegExpStringIteratorPrototype% [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%regexpstringiteratorprototype%-%symbol.tostringtag%) |
| 22.2.9.3 | Properties of RegExp String Iterator Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-regexp-string-iterator-instances) |

## Support

Feature-level support tracking with test script references.

### 22.2.6.2 ([tc39.es](https://tc39.es/ecma262/#sec-regexp.prototype.exec))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| RegExp.prototype.exec | Supported with Limitations | [`String_RegExp_Exec_LastIndex_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_RegExp_Exec_LastIndex_Global.js) | Implemented in JavaScriptRuntime.RegExp.exec as a minimal subset. Supports returning an Array of captures and attaches .index and .input. Full RegExp exotic object semantics, sticky (/y), unicode modes, named groups, and @@exec overrides are not implemented. |

### 22.2.6.5 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.global))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.global | Supported with Limitations | [`IntrinsicCallables_RegExp_Prototype_Getters_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Prototype_Getters_Basic.js) | Implemented in JavaScriptRuntime.RegExp based on stored flags. Full RegExp exotic object semantics and cross-realm behaviors are not implemented. |

### 22.2.6.7 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.ignorecase))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.ignoreCase | Supported with Limitations | [`IntrinsicCallables_RegExp_Prototype_Getters_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Prototype_Getters_Basic.js) | Implemented in JavaScriptRuntime.RegExp based on stored flags. |

### 22.2.6.10 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.multiline))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.multiline | Supported with Limitations | [`IntrinsicCallables_RegExp_Prototype_Getters_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Prototype_Getters_Basic.js) | Implemented in JavaScriptRuntime.RegExp based on stored flags. |

### 22.2.6.13 ([tc39.es](https://tc39.es/ecma262/#sec-get-regexp.prototype.source))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get RegExp.prototype.source | Supported with Limitations | [`IntrinsicCallables_RegExp_Prototype_Getters_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_RegExp_Prototype_Getters_Basic.js) | Implemented in JavaScriptRuntime.RegExp using the original pattern text. Does not attempt to normalize/escape patterns exactly per spec. |

### 22.2.8.1 ([tc39.es](https://tc39.es/ecma262/#sec-lastindex))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| RegExp instance lastIndex | Supported with Limitations | [`String_RegExp_Exec_LastIndex_Global.js`](../../../Js2IL.Tests/String/JavaScript/String_RegExp_Exec_LastIndex_Global.js) | Implemented in JavaScriptRuntime.RegExp as a numeric property. Currently participates in exec() only for /g (global) regexes; when no match is found lastIndex is reset to 0. |

