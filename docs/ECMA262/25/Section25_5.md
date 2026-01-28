<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 25.5: The JSON Object

[Back to Section25](Section25.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 25.5 | The JSON Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 25.5.1 | JSON.parse ( text [ , reviver ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json.parse) |
| 25.5.1.1 | ParseJSON ( text ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-ParseJSON) |
| 25.5.1.2 | InternalizeJSONProperty ( holder , name , reviver ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-internalizejsonproperty) |
| 25.5.2 | JSON.stringify ( value [ , replacer [ , space ] ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-json.stringify) |
| 25.5.2.1 | JSON Serialization Record | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-json-serialization-record) |
| 25.5.2.2 | SerializeJSONProperty ( state , key , holder ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-serializejsonproperty) |
| 25.5.2.3 | QuoteJSONString ( value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-quotejsonstring) |
| 25.5.2.4 | UnicodeEscape ( C ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-unicodeescape) |
| 25.5.2.5 | SerializeJSONObject ( state , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-serializejsonobject) |
| 25.5.2.6 | SerializeJSONArray ( state , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-serializejsonarray) |
| 25.5.3 | JSON [ %Symbol.toStringTag% ] | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-json-%symbol.tostringtag%) |

## Support

Feature-level support tracking with test script references.

### 25.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-json.parse))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| JSON.parse | Supported with Limitations | `Js2IL.Tests/JSONRuntimeTests.cs` | Implemented via host intrinsic JavaScriptRuntime.JSON.Parse(string). Maps invalid input to SyntaxError and non-string input to TypeError. Reviver parameter is not supported. Objects become ExpandoObject, arrays use JavaScriptRuntime.Array, numbers use double. |

