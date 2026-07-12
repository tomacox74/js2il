<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 19.4: Other Properties of the Global Object

[Back to Section19](Section19.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-12T04:40:20Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 19.4 | Other Properties of the Global Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-other-properties-of-the-global-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 19.4.1 | Atomics | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-atomics) |
| 19.4.2 | JSON | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json) |
| 19.4.3 | Math | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-math) |
| 19.4.4 | Reflect | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 19.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-json))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| JSON intrinsic object is available (JSON.parse, JSON.stringify) | Supported with Limitations | [`JSON_Parse_SimpleObject.js`](../../../tests/Jroc.Tests/JSON/JavaScript/JSON_Parse_SimpleObject.js) | `test/built-ins/JSON/parse/15.12.1.1-0-1.js`<br>`test/built-ins/JSON/parse/reviver-call-order.js`<br>`test/built-ins/JSON/parse/reviver-wrapper.js`<br>`test/built-ins/JSON/stringify/builtin.js`<br>`test/built-ins/JSON/stringify/length.js`<br>`test/built-ins/JSON/stringify/name.js`<br>`test/built-ins/JSON/stringify/property-order.js`<br>`test/built-ins/JSON/stringify/prop-desc.js`<br>`test/built-ins/JSON/stringify/replacer-array-abrupt.js`<br>`test/built-ins/JSON/stringify/replacer-array-duplicates.js`<br>`test/built-ins/JSON/stringify/replacer-array-empty.js`<br>`test/built-ins/JSON/stringify/replacer-array-number.js`<br>`test/built-ins/JSON/stringify/replacer-array-number-object.js`<br>`test/built-ins/JSON/stringify/replacer-array-order.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy-revoked.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy-revoked-realm.js`<br>`test/built-ins/JSON/stringify/replacer-array-string-object.js`<br>`test/built-ins/JSON/stringify/replacer-array-undefined.js` | JavaScriptRuntime.JSON exposes both JSON.parse and JSON.stringify. JSON.parse creates ordinary JsObject records, translates parsing errors to JavaScript SyntaxError, and supports post-order reviver traversal with holder this values, deletion, and replacement. The current bounded test262 coverage also exercises JSON.stringify function metadata, ordinary property ordering, array replacer filtering/order/deduplication, boxed string/number replacer entries, ignored undefined/sparse replacer entries, and proxy abrupt-completion paths. Broader exotic, cyclic, and cross-realm behavior remains limited. |

### 19.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-math))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Math intrinsic object is available (common Math.* functions and constants) | Supported with Limitations | [`Math_Ceil_Sqrt_Basic.js`](../../../tests/Jroc.Tests/Math/JavaScript/Math_Ceil_Sqrt_Basic.js)<br>[`Math_Imul_Clz32_Basics.js`](../../../tests/Jroc.Tests/Math/JavaScript/Math_Imul_Clz32_Basics.js)<br>[`Math_Min_Max_NaN_EmptyArgs.js`](../../../tests/Jroc.Tests/Math/JavaScript/Math_Min_Max_NaN_EmptyArgs.js) |  | Implemented by JavaScriptRuntime.Math, backed by System.Math with JS-style argument coercions. Coverage is partial (Supported with Limitations). |

