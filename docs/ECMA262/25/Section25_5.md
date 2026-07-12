<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 25.5: The JSON Object

[Back to Section25](Section25.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-12T04:40:20Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 25.5 | The JSON Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 25.5.1 | JSON.parse ( text [ , reviver ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json.parse) |
| 25.5.1.1 | ParseJSON ( text ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-ParseJSON) |
| 25.5.1.2 | InternalizeJSONProperty ( holder , name , reviver ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-internalizejsonproperty) |
| 25.5.2 | JSON.stringify ( value [ , replacer [ , space ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json.stringify) |
| 25.5.2.1 | JSON Serialization Record | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-json-serialization-record) |
| 25.5.2.2 | SerializeJSONProperty ( state , key , holder ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-serializejsonproperty) |
| 25.5.2.3 | QuoteJSONString ( value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-quotejsonstring) |
| 25.5.2.4 | UnicodeEscape ( C ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-unicodeescape) |
| 25.5.2.5 | SerializeJSONObject ( state , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-serializejsonobject) |
| 25.5.2.6 | SerializeJSONArray ( state , value ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-serializejsonarray) |
| 25.5.3 | JSON [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-json-%symbol.tostringtag%) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 25.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-json.parse))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| JSON.parse | Supported with Limitations | [`JSON_Parse_Reviver_Holder.js`](../../../tests/Jroc.Tests/JSON/JavaScript/JSON_Parse_Reviver_Holder.js)<br>`tests/Jroc.Test262.Tests/built-ins/JSON/parse/ExecutionTests.cs` | `test/built-ins/JSON/parse/15.12.1.1-0-1.js`<br>`test/built-ins/JSON/parse/15.12.1.1-0-4.js`<br>`test/built-ins/JSON/parse/15.12.1.1-0-9.js`<br>`test/built-ins/JSON/parse/15.12.1.1-g1-1.js`<br>`test/built-ins/JSON/parse/15.12.1.1-g1-4.js`<br>`test/built-ins/JSON/parse/15.12.1.1-g2-1.js`<br>`test/built-ins/JSON/parse/15.12.1.1-g2-4.js`<br>`test/built-ins/JSON/parse/reviver-call-order.js`<br>`test/built-ins/JSON/parse/reviver-wrapper.js` | Implemented by JavaScriptRuntime.JSON.Parse with ECMAScript string coercion and SyntaxError translation for invalid input. Parsed objects and the reviver root wrapper are ordinary JsObject instances; arrays use JavaScriptRuntime.Array and numbers use double. Reviver processing walks properties post-order, calls the reviver with the current holder as this, supports deletion through an undefined result, and supports replacement values. Broader proxy, cross-realm, and extreme-depth behavior remains limited. |

### 25.5.2 ([tc39.es](https://tc39.es/ecma262/#sec-json.stringify))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| JSON.stringify | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/JSON/stringify/ExecutionTests.cs` | `test/built-ins/JSON/stringify/builtin.js`<br>`test/built-ins/JSON/stringify/length.js`<br>`test/built-ins/JSON/stringify/name.js`<br>`test/built-ins/JSON/stringify/property-order.js`<br>`test/built-ins/JSON/stringify/prop-desc.js`<br>`test/built-ins/JSON/stringify/replacer-array-abrupt.js`<br>`test/built-ins/JSON/stringify/replacer-array-duplicates.js`<br>`test/built-ins/JSON/stringify/replacer-array-empty.js`<br>`test/built-ins/JSON/stringify/replacer-array-number.js`<br>`test/built-ins/JSON/stringify/replacer-array-number-object.js`<br>`test/built-ins/JSON/stringify/replacer-array-order.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy-revoked.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy-revoked-realm.js`<br>`test/built-ins/JSON/stringify/replacer-array-string-object.js`<br>`test/built-ins/JSON/stringify/replacer-array-undefined.js`<br>`test/built-ins/JSON/stringify/replacer-function-result.js`<br>`test/built-ins/JSON/stringify/space-string.js`<br>`test/built-ins/JSON/stringify/value-tojson-result.js` | JSON.stringify is implemented for ordinary objects and arrays. Current bounded test262 coverage exercises the builtin function object surface (`JSON.stringify`, `.length`, `.name`, and property descriptor metadata), ordinary property-order serialization, array-replacer ordering/filtering/deduplication, boxed string/number replacer entries, ignored undefined/sparse replacer entries, proxy/revoked-proxy abrupt completion, replacer function return-value serialization, selected toJSON return-value serialization, and string space/gap formatting. Broader cyclic, exotic, BigInt, and cross-realm behavior remains limited. |

### 25.5.3 ([tc39.es](https://tc39.es/ecma262/#sec-json-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| JSON[@@toStringTag] descriptor | Supported | `tests/Jroc.Test262.Tests/built-ins/JSON/ExecutionTests.cs` | `test/built-ins/JSON/Symbol.toStringTag.js` | Checked-in coverage now includes JSON @@toStringTag value and descriptor attributes (`value: "JSON"`, `writable: false`, `enumerable: false`, `configurable: true`). |

