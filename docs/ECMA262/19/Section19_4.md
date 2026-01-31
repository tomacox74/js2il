<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 19.4: Other Properties of the Global Object

[Back to Section19](Section19.md) | [Back to Index](../Index.md)

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

Feature-level support tracking with test script references.

### 19.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-json))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| JSON intrinsic object is available (JSON.parse) | Supported with Limitations | [`JSON_Parse_SimpleObject.js`](../../../Js2IL.Tests/JSON/JavaScript/JSON_Parse_SimpleObject.js) | JavaScriptRuntime.JSON implements JSON.parse only. JSON.stringify and the optional reviver/replacer behaviors are not implemented. Parsing errors are translated to JavaScript SyntaxError. |

### 19.4.3 ([tc39.es](https://tc39.es/ecma262/#sec-math))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Math intrinsic object is available (common Math.* functions and constants) | Supported with Limitations | [`Math_Ceil_Sqrt_Basic.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Ceil_Sqrt_Basic.js)<br>[`Math_Imul_Clz32_Basics.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Imul_Clz32_Basics.js)<br>[`Math_Min_Max_NaN_EmptyArgs.js`](../../../Js2IL.Tests/Math/JavaScript/Math_Min_Max_NaN_EmptyArgs.js) | Implemented by JavaScriptRuntime.Math, backed by System.Math with JS-style argument coercions. Coverage is partial (Supported with Limitations). |

