<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 19.1: Value Properties of the Global Object

[Back to Section19](Section19.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 19.1 | Value Properties of the Global Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-value-properties-of-the-global-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 19.1.1 | globalThis | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-globalthis) |
| 19.1.2 | Infinity | Supported | [tc39.es](https://tc39.es/ecma262/#sec-value-properties-of-the-global-object-infinity) |
| 19.1.3 | NaN | Supported | [tc39.es](https://tc39.es/ecma262/#sec-value-properties-of-the-global-object-nan) |
| 19.1.4 | undefined | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-undefined) |

## Support

Feature-level support tracking with test script references.

### 19.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-value-properties-of-the-global-object-infinity))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Infinity global value | Supported | [`Math_Hypot_Infinity_NaN.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Hypot_Infinity_NaN.js) | Exposed as JavaScriptRuntime.GlobalThis.Infinity (double.PositiveInfinity). |

### 19.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-value-properties-of-the-global-object-nan))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| NaN global value | Supported | [`Math_Hypot_Infinity_NaN.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Hypot_Infinity_NaN.js) | Exposed as JavaScriptRuntime.GlobalThis.NaN (double.NaN). |

### 19.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-undefined))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| undefined value | Supported with Limitations | [`UnaryOperator_Typeof.js`](../../../Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_Typeof.js) | Modeled as CLR null rather than a writable/configurable global property. This supports common read/compare/typeof patterns, but does not model property attributes of the global 'undefined' binding. |

