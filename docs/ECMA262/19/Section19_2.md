<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 19.2: Function Properties of the Global Object

[Back to Section19](Section19.md) | [Back to Index](../Index.md)

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
| 19.2.2 | isFinite ( number ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-isfinite-number) |
| 19.2.3 | isNaN ( number ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-isnan-number) |
| 19.2.4 | parseFloat ( string ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-parsefloat-string) |
| 19.2.5 | parseInt ( string , radix ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-parseint-string-radix) |
| 19.2.6 | URI Handling Functions | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-uri-handling-functions) |
| 19.2.6.1 | decodeURI ( encodedURI ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-decodeuri-encodeduri) |
| 19.2.6.2 | decodeURIComponent ( encodedURIComponent ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-decodeuricomponent-encodeduricomponent) |
| 19.2.6.3 | encodeURI ( uri ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-encodeuri-uri) |
| 19.2.6.4 | encodeURIComponent ( uriComponent ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-encodeuricomponent-uricomponent) |
| 19.2.6.5 | Encode ( string , extraUnescaped ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-encode) |
| 19.2.6.6 | Decode ( string , preserveEscapeSet ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-decode) |
| 19.2.6.7 | ParseHexOctet ( string , position ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-parsehexoctet) |

## Support

Feature-level support tracking with test script references.

### 19.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-parseint-string-radix))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| parseInt(string, radix) | Supported with Limitations | [`IntrinsicCallables_ParseInt_Basic.js`](../../../Js2IL.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_ParseInt_Basic.js) | Implemented by JavaScriptRuntime.GlobalThis.parseInt and returns a JS Number (unboxed double). Coverage is partial (e.g., larger-than-Int64 inputs and full edge-case parity may differ). |

