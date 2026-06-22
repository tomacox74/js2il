<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 19.1: Value Properties of the Global Object

[Back to Section19](Section19.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-06-22T14:45:35Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 19.1 | Value Properties of the Global Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-value-properties-of-the-global-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 19.1.1 | globalThis | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-globalthis) |
| 19.1.2 | Infinity | Supported | [tc39.es](https://tc39.es/ecma262/#sec-value-properties-of-the-global-object-infinity) |
| 19.1.3 | NaN | Supported | [tc39.es](https://tc39.es/ecma262/#sec-value-properties-of-the-global-object-nan) |
| 19.1.4 | undefined | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-undefined) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 19.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-globalthis))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| globalThis global value | Supported with Limitations | [`global-object.js`](../../../tests/Jroc.Test262.Tests/built-ins/global/JavaScript/global-object.js)<br>[`IntrinsicCallables_GlobalThis_Basic.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_GlobalThis_Basic.js)<br>[`IntrinsicCallables_GlobalThis_Enumerability.js`](../../../tests/Jroc.Tests/IntrinsicCallables/JavaScript/IntrinsicCallables_GlobalThis_Enumerability.js) |  | Exposed as JavaScriptRuntime.GlobalThis.globalThis. Backed by a per-thread ExpandoObject seeded with common globals and delegate-valued global functions, and top-level var bindings are mirrored onto the global object so test262 global-object checks observe them. Does not attempt to model all host environment properties/attributes. |

### 19.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-value-properties-of-the-global-object-infinity))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Infinity global value | Supported | [`Math_Hypot_Infinity_NaN.js`](../../../tests/Jroc.Tests/Math/JavaScript/Math_Hypot_Infinity_NaN.js) |  | Exposed as JavaScriptRuntime.GlobalThis.Infinity (double.PositiveInfinity). |

### 19.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-value-properties-of-the-global-object-nan))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| NaN global value | Supported | [`10.2.1.1.3-4-16-s.js`](../../../tests/Jroc.Test262.Tests/built-ins/global/JavaScript/10.2.1.1.3-4-16-s.js)<br>[`Math_Hypot_Infinity_NaN.js`](../../../tests/Jroc.Tests/Math/JavaScript/Math_Hypot_Infinity_NaN.js) |  | Exposed as JavaScriptRuntime.GlobalThis.NaN (double.NaN). Strict-mode assignment to the global NaN binding now throws the expected TypeError. |

### 19.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-undefined))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| undefined value | Supported with Limitations | [`10.2.1.1.3-4-18-s.js`](../../../tests/Jroc.Test262.Tests/built-ins/global/JavaScript/10.2.1.1.3-4-18-s.js)<br>[`UnaryOperator_Typeof.js`](../../../tests/Jroc.Tests/UnaryOperator/JavaScript/UnaryOperator_Typeof.js) |  | Modeled as CLR null rather than a writable/configurable global property. This supports common read/compare/typeof patterns, and strict-mode assignment to the global undefined binding now throws the expected TypeError, but property-attribute modeling remains limited. |

