<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 24.5: Abstract Operations for Keyed Collections

[Back to Section24](Section24.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 24.5 | Abstract Operations for Keyed Collections | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-abstract-operations-for-keyed-collections) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 24.5.1 | CanonicalizeKeyedCollectionKey ( key ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-canonicalizekeyedcollectionkey) |

## Support

Feature-level support tracking with test script references.

### 24.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-canonicalizekeyedcollectionkey))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| JSON.parse | Partially Supported | `Js2IL.Tests/JSONRuntimeTests.cs` | Implemented via host intrinsic JavaScriptRuntime.JSON.Parse(string). Maps invalid input to SyntaxError and non-string input to TypeError. Reviver parameter is not supported. Objects become ExpandoObject, arrays use JavaScriptRuntime.Array, numbers use double. |

