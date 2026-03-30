# Module: querystring

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/querystring.html) |

## Implementation

- `src/JavaScriptRuntime/Node/QueryString.cs`
- `src/JavaScriptRuntime/Node/UrlQueryHelpers.cs`

## Notes

Provides a focused legacy querystring baseline with parse/stringify support for common key/value scenarios, duplicate-key parsing to arrays, and custom separator/assignment tokens. escape/unescape helpers and advanced options are not implemented yet.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| parse(str[, sep[, eq[, options]]]) | function | supported | [docs](https://nodejs.org/api/querystring.html#querystringparsestr-sep-eq-options) |
| stringify(obj[, sep[, eq[, options]]]) | function | supported | [docs](https://nodejs.org/api/querystring.html#querystringstringifyobj-sep-eq-options) |

## API Details

### parse(str[, sep[, eq[, options]]])

Decodes '+' as space, percent-decodes keys/values, and materializes duplicate keys as JavaScript arrays.

**Tests:**
- `Js2IL.Tests.Node.QueryString.ExecutionTests.Require_QueryString_Parse_And_Stringify` (`Js2IL.Tests/Node/QueryString/ExecutionTests.cs`)
- `Js2IL.Tests.Node.QueryString.GeneratorTests.Require_QueryString_Parse_And_Stringify` (`Js2IL.Tests/Node/QueryString/GeneratorTests.cs`)

### stringify(obj[, sep[, eq[, options]]])

Serializes scalar values and JavaScript arrays, repeating duplicate keys in the output order encountered on the source object.

**Tests:**
- `Js2IL.Tests.Node.QueryString.ExecutionTests.Require_QueryString_Parse_And_Stringify` (`Js2IL.Tests/Node/QueryString/ExecutionTests.cs`)
- `Js2IL.Tests.Node.QueryString.GeneratorTests.Require_QueryString_Parse_And_Stringify` (`Js2IL.Tests/Node/QueryString/GeneratorTests.cs`)
