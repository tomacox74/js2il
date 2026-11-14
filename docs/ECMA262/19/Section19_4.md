<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 19.4: Other Properties of the Global Object

[Back to Section19](Section19.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-11-14T06:53:57Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 19.4 | Other Properties of the Global Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-other-properties-of-the-global-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 19.4.1 | Atomics | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-atomics) |
| 19.4.2 | JSON | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json) |
| 19.4.3 | Math | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-math) |
| 19.4.4 | Reflect | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 19.4 ([tc39.es](https://tc39.es/ecma262/#sec-other-properties-of-the-global-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Temporal namespace: PlainTime construction and conversion | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Temporal/PlainTime/NativePortBatch_Port1000.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Temporal/PlainTime/from/NativePortBatch_Port1000.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Temporal/PlainTime/compare/NativePortBatch_Port1000.cs` | `test/built-ins/Temporal/PlainTime/argument-convert.js`<br>`test/built-ins/Temporal/PlainTime/subclass.js`<br>`test/built-ins/Temporal/PlainTime/compare/use-internal-slots.js`<br>`test/built-ins/Temporal/PlainTime/compare/argument-string-minus-sign.js` | The pinned Test262 Temporal extension is exposed through globalThis.Temporal. PlainTime supports six time fields, constructor coercion and range checks, branded accessors, newTarget prototype selection, from and compare. Conversion preserves observable field and option access order, validates ISO time strings and resolves date-like string ambiguity. PlainDateTime and ZonedDateTime interoperation remain unsupported; this does not imply full Temporal support. |
| Temporal.Duration core inputs for PlainTime arithmetic | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Temporal/PlainTime/prototype/add/NativePortBatch_Port1000.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Temporal/PlainTime/prototype/subtract/NativePortBatch_Port1000.cs` | `test/built-ins/Temporal/PlainTime/prototype/add/precision-exact-mathematical-values-1.js` | Supports the ten-field Duration constructor, from, branded field accessors, sign and blank. Duration-like property bags are read and converted in alphabetical order; ISO duration strings preserve fractional time units exactly. Mixed signs, nonintegral fields, calendar-field limits and the normalized 2^53-second bound are validated before time arithmetic. General Duration arithmetic, calendar-relative conversion and formatting remain unsupported. |
| Temporal.PlainTime value operations, rounding and formatting | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Temporal/PlainTime/prototype/add/NativePortBatch_Port1000.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Temporal/PlainTime/prototype/subtract/NativePortBatch_Port1000.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Temporal/PlainTime/prototype/round/NativePortBatch_Port1000.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Temporal/PlainTime/prototype/toString/NativePortBatch_Port1000.cs` | `test/built-ins/Temporal/PlainTime/prototype/add/argument-object.js`<br>`test/built-ins/Temporal/PlainTime/prototype/add/order-of-operations.js` | Supports with, equals, add, subtract, round, toString and toJSON, with immutable results and exact nanosecond arithmetic. Time arithmetic validates all duration fields before ignoring date units, wraps at midnight and ignores the optional second arithmetic argument. Rounding supports nine modes, six time units, increment validation and ordered option coercion. toLocaleString uses the base ECMA-262 ISO representation because Intl.DateTimeFormat is not implemented; locale-aware formatting is not claimed. since, until and broader Temporal date/calendar operations remain unsupported. |

### 19.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-atomics))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Atomics global object | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Atomics/notify/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Atomics/waitAsync/ExecutionTests.cs` | `test/built-ins/Atomics/notify/descriptor.js`<br>`test/built-ins/Atomics/waitAsync/descriptor.js` | Exposes integer typed-array atomic operations and waiter coordination. Full cross-agent memory-model conformance is not yet verified. |

### 19.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-json))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| JSON intrinsic object is available (JSON.parse, JSON.stringify) | Supported with Limitations | [`JSON_Parse_SimpleObject.js`](../../../tests/Jroc.Tests/JSON/JavaScript/JSON_Parse_SimpleObject.js)<br>`tests/Jroc.Test262.Tests/built-ins/JSON/parse/ExecutionTests.Batch.cs`<br>`tests/Jroc.Test262.Tests/built-ins/JSON/stringify/ExecutionTests.Batch.cs` | `test/built-ins/JSON/parse/15.12.1.1-0-1.js`<br>`test/built-ins/JSON/parse/reviver-call-order.js`<br>`test/built-ins/JSON/parse/reviver-wrapper.js`<br>`test/built-ins/JSON/stringify/builtin.js`<br>`test/built-ins/JSON/stringify/length.js`<br>`test/built-ins/JSON/stringify/name.js`<br>`test/built-ins/JSON/stringify/property-order.js`<br>`test/built-ins/JSON/stringify/prop-desc.js`<br>`test/built-ins/JSON/stringify/replacer-array-abrupt.js`<br>`test/built-ins/JSON/stringify/replacer-array-duplicates.js`<br>`test/built-ins/JSON/stringify/replacer-array-empty.js`<br>`test/built-ins/JSON/stringify/replacer-array-number.js`<br>`test/built-ins/JSON/stringify/replacer-array-number-object.js`<br>`test/built-ins/JSON/stringify/replacer-array-order.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy-revoked.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy-revoked-realm.js`<br>`test/built-ins/JSON/stringify/replacer-array-string-object.js`<br>`test/built-ins/JSON/stringify/replacer-array-undefined.js` | JavaScriptRuntime.JSON exposes both JSON.parse and JSON.stringify. JSON.parse creates ordinary JsObject records, translates parsing errors to JavaScript SyntaxError, and supports post-order, Proxy-aware reviver traversal with observable array lengths, holder this values, deletion, and replacement. JSON.stringify coverage includes function metadata, ordinary property ordering, array replacer filtering/order/deduplication, observable boxed primitive coercion, cyclic-value rejection, well-formed lone-surrogate escaping, and nested Proxy array/object traversal with abrupt-completion propagation. Broader cross-realm behavior remains limited. |

### 19.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-math))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Math intrinsic object is available (common Math.* functions and constants) | Supported with Limitations | [`Math_Ceil_Sqrt_Basic.js`](../../../tests/Jroc.Tests/Math/JavaScript/Math_Ceil_Sqrt_Basic.js)<br>[`Math_Imul_Clz32_Basics.js`](../../../tests/Jroc.Tests/Math/JavaScript/Math_Imul_Clz32_Basics.js)<br>[`Math_Min_Max_NaN_EmptyArgs.js`](../../../tests/Jroc.Tests/Math/JavaScript/Math_Min_Max_NaN_EmptyArgs.js) |  | Implemented by JavaScriptRuntime.Math, backed by System.Math with JS-style argument coercions. Coverage is partial (Supported with Limitations). |

