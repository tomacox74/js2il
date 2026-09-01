<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 25.5: The JSON Object

[Back to Section25](Section25.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-09-01T18:38:04Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 25.5 | The JSON Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 25.5.1 | JSON.parse ( text [ , reviver ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json.parse) |
| 25.5.1.1 | ParseJSON ( text ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ParseJSON) |
| 25.5.1.2 | InternalizeJSONProperty ( holder , name , reviver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-internalizejsonproperty) |
| 25.5.2 | JSON.stringify ( value [ , replacer [ , space ] ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json.stringify) |
| 25.5.2.1 | JSON Serialization Record | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json-serialization-record) |
| 25.5.2.2 | SerializeJSONProperty ( state , key , holder ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-serializejsonproperty) |
| 25.5.2.3 | QuoteJSONString ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-quotejsonstring) |
| 25.5.2.4 | UnicodeEscape ( C ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-unicodeescape) |
| 25.5.2.5 | SerializeJSONObject ( state , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-serializejsonobject) |
| 25.5.2.6 | SerializeJSONArray ( state , value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-serializejsonarray) |
| 25.5.3 | JSON [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-json-%symbol.tostringtag%) |
| 25.5.4 | JSON.rawJSON ( text ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-json.rawjson) |
| 25.5.5 | JSON.isRawJSON ( O ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-json.israwjson) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 25.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-json.parse))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| JSON.parse | Supported with Limitations | [`JSON_Parse_Reviver_Holder.js`](../../../tests/Jroc.Tests/JSON/JavaScript/JSON_Parse_Reviver_Holder.js)<br>`tests/Jroc.Test262.Tests/built-ins/JSON/parse/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/JSON/parse/ExecutionTests.Batch.cs`<br>`tests/Jroc.Test262.Tests/built-ins/JSON/PortNext200Batch2ExecutionTests.cs` | `test/built-ins/JSON/parse/15.12.1.1-0-1.js`<br>`test/built-ins/JSON/parse/15.12.1.1-0-4.js`<br>`test/built-ins/JSON/parse/15.12.1.1-0-9.js`<br>`test/built-ins/JSON/parse/15.12.1.1-g1-1.js`<br>`test/built-ins/JSON/parse/15.12.1.1-g1-4.js`<br>`test/built-ins/JSON/parse/15.12.1.1-g2-1.js`<br>`test/built-ins/JSON/parse/15.12.1.1-g2-4.js`<br>`test/built-ins/JSON/parse/15.12.1.1-0-2.js`<br>`test/built-ins/JSON/parse/15.12.1.1-g1-2.js`<br>`test/built-ins/JSON/parse/revived-proxy.js`<br>`test/built-ins/JSON/parse/reviver-array-define-prop-err.js`<br>`test/built-ins/JSON/parse/reviver-array-length-coerce-err.js`<br>`test/built-ins/JSON/parse/reviver-array-length-get-err.js`<br>`test/built-ins/JSON/parse/reviver-call-order.js`<br>`test/built-ins/JSON/parse/reviver-object-define-prop-err.js`<br>`test/built-ins/JSON/parse/text-non-string-primitive.js`<br>`test/built-ins/JSON/parse/reviver-wrapper.js` | Implemented by JavaScriptRuntime.JSON.Parse with ECMAScript string coercion and SyntaxError translation for invalid input. The complete pinned JSON.parse directory is covered. Reviver processing walks properties post-order, identifies Proxy-wrapped arrays, observes and coerces array lengths, propagates Proxy internal-operation failures, observes forward object/array modifications, defines replacement properties, and supplies the ES2025 context object with exact source text for unmodified primitive parse nodes. Extreme-depth behavior remains runtime-limited. |

### 25.5.2 ([tc39.es](https://tc39.es/ecma262/#sec-json.stringify))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| JSON.stringify | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/JSON/stringify/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/JSON/stringify/ExecutionTests.Batch.cs` | `test/built-ins/JSON/stringify/builtin.js`<br>`test/built-ins/JSON/stringify/length.js`<br>`test/built-ins/JSON/stringify/name.js`<br>`test/built-ins/JSON/stringify/property-order.js`<br>`test/built-ins/JSON/stringify/prop-desc.js`<br>`test/built-ins/JSON/stringify/replacer-array-abrupt.js`<br>`test/built-ins/JSON/stringify/replacer-array-duplicates.js`<br>`test/built-ins/JSON/stringify/replacer-array-empty.js`<br>`test/built-ins/JSON/stringify/replacer-array-number.js`<br>`test/built-ins/JSON/stringify/replacer-array-number-object.js`<br>`test/built-ins/JSON/stringify/replacer-array-order.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy-revoked.js`<br>`test/built-ins/JSON/stringify/replacer-array-proxy-revoked-realm.js`<br>`test/built-ins/JSON/stringify/replacer-array-string-object.js`<br>`test/built-ins/JSON/stringify/replacer-array-undefined.js`<br>`test/built-ins/JSON/stringify/replacer-function-object-deleted-property.js`<br>`test/built-ins/JSON/stringify/replacer-function-result.js`<br>`test/built-ins/JSON/stringify/replacer-function-wrapper.js`<br>`test/built-ins/JSON/stringify/space-number-object.js`<br>`test/built-ins/JSON/stringify/space-string.js`<br>`test/built-ins/JSON/stringify/space-string-object.js`<br>`test/built-ins/JSON/stringify/value-tojson-result.js`<br>`test/built-ins/JSON/stringify/value-bigint.js`<br>`test/built-ins/JSON/stringify/value-bigint-tojson.js`<br>`test/built-ins/JSON/stringify/not-a-constructor.js`<br>`test/built-ins/JSON/stringify/replacer-array-wrong-type.js`<br>`test/built-ins/JSON/stringify/replacer-function-result-undefined.js`<br>`test/built-ins/JSON/stringify/replacer-function-tojson.js`<br>`test/built-ins/JSON/stringify/replacer-wrong-type.js`<br>`test/built-ins/JSON/stringify/space-number-float.js`<br>`test/built-ins/JSON/stringify/space-number.js`<br>`test/built-ins/JSON/stringify/space-number-range.js`<br>`test/built-ins/JSON/stringify/space-string-range.js`<br>`test/built-ins/JSON/stringify/space-wrong-type.js`<br>`test/built-ins/JSON/stringify/value-array-abrupt.js`<br>`test/built-ins/JSON/stringify/value-array-circular.js`<br>`test/built-ins/JSON/stringify/value-array-proxy.js`<br>`test/built-ins/JSON/stringify/value-array-proxy-revoked.js`<br>`test/built-ins/JSON/stringify/value-bigint-order.js`<br>`test/built-ins/JSON/stringify/value-bigint-replacer.js`<br>`test/built-ins/JSON/stringify/value-bigint-tojson-receiver.js`<br>`test/built-ins/JSON/stringify/value-boolean-object.js`<br>`test/built-ins/JSON/stringify/value-function.js`<br>`test/built-ins/JSON/stringify/value-number-negative-zero.js`<br>`test/built-ins/JSON/stringify/value-number-non-finite.js`<br>`test/built-ins/JSON/stringify/value-number-object.js`<br>`test/built-ins/JSON/stringify/value-object-abrupt.js`<br>`test/built-ins/JSON/stringify/value-object-circular.js`<br>`test/built-ins/JSON/stringify/value-object-proxy.js`<br>`test/built-ins/JSON/stringify/value-object-proxy-revoked.js`<br>`test/built-ins/JSON/stringify/value-primitive-top-level.js`<br>`test/built-ins/JSON/stringify/value-string-escape-ascii.js`<br>`test/built-ins/JSON/stringify/value-string-escape-unicode.js`<br>`test/built-ins/JSON/stringify/value-string-object.js`<br>`test/built-ins/JSON/stringify/value-symbol.js`<br>`test/built-ins/JSON/stringify/value-tojson-abrupt.js`<br>`test/built-ins/JSON/stringify/value-tojson-arguments.js`<br>`test/built-ins/JSON/stringify/value-tojson-array-circular.js`<br>`test/built-ins/JSON/stringify/value-tojson-not-function.js`<br>`test/built-ins/JSON/stringify/value-tojson-object-circular.js` | JSON.stringify supports ordinary and Proxy-backed objects and arrays with specification property ordering, cyclic-value rejection, abrupt-completion propagation, array-replacer filtering/order/deduplication, replacer functions, toJSON hooks, spacing, well-formed lone-surrogate escaping, and observable Number/String wrapper coercion. RawJSON marker values serialize as unquoted validated JSON primitives. BigInt values throw TypeError unless an observable BigInt.prototype.toJSON method supplies a serializable replacement. Cross-realm behavior remains limited. |

### 25.5.3 ([tc39.es](https://tc39.es/ecma262/#sec-json-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| JSON[@@toStringTag] descriptor | Supported | `tests/Jroc.Test262.Tests/built-ins/JSON/ExecutionTests.cs` | `test/built-ins/JSON/Symbol.toStringTag.js` | Checked-in coverage now includes JSON @@toStringTag value and descriptor attributes (`value: "JSON"`, `writable: false`, `enumerable: false`, `configurable: true`). |

### 25.5.4 ([tc39.es](https://tc39.es/ecma262/#sec-json.rawjson))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| JSON.rawJSON | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/JSON/rawJSON/ExecutionTests.cs` | `test/built-ins/JSON/rawJSON/basic.js`<br>`test/built-ins/JSON/rawJSON/bigint-raw-json-can-be-stringified.js`<br>`test/built-ins/JSON/rawJSON/builtin.js`<br>`test/built-ins/JSON/rawJSON/illegal-empty-and-start-end-chars.js`<br>`test/built-ins/JSON/rawJSON/invalid-JSON-text.js`<br>`test/built-ins/JSON/rawJSON/length.js`<br>`test/built-ins/JSON/rawJSON/name.js`<br>`test/built-ins/JSON/rawJSON/not-a-constructor.js`<br>`test/built-ins/JSON/rawJSON/prop-desc.js`<br>`test/built-ins/JSON/rawJSON/returns-expected-object.js` | JSON.rawJSON creates frozen null-prototype marker objects for validated primitive JSON text. JSON.stringify emits those markers without quoting, including replacer-produced values that preserve large integer source text. Current parse source-context support covers the root reviver value; nested source contexts remain limited. |

### 25.5.5 ([tc39.es](https://tc39.es/ecma262/#sec-json.israwjson))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| JSON.isRawJSON | Supported | `tests/Jroc.Test262.Tests/built-ins/JSON/isRawJSON/ExecutionTests.cs` | `test/built-ins/JSON/isRawJSON/basic.js`<br>`test/built-ins/JSON/isRawJSON/builtin.js`<br>`test/built-ins/JSON/isRawJSON/length.js`<br>`test/built-ins/JSON/isRawJSON/name.js`<br>`test/built-ins/JSON/isRawJSON/not-a-constructor.js`<br>`test/built-ins/JSON/isRawJSON/prop-desc.js` | JSON.isRawJSON recognizes only marker objects returned by JSON.rawJSON and exposes the standard non-constructor builtin metadata. |

