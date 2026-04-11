# Global: URLSearchParams

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/url.html#class-urlsearchparams) |

## Implementation

- `src/JavaScriptRuntime/GlobalThis.cs`
- `src/JavaScriptRuntime/Node/Url.cs`

## Notes

Exposes the WHATWG `URLSearchParams` constructor on `globalThis` and reuses the same constructor object exported by `require("url")` / `require("node:url")`, including `Function.prototype` linkage, `.prototype`, `.constructor`, and `new`-required behavior within the current focused URLSearchParams feature slice.

## Tests

- `Js2IL.Tests.Node.Url.ExecutionTests.Global_Url_And_SearchParams` (`tests/Js2IL.Tests/Node/Url/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Url.GeneratorTests.Global_Url_And_SearchParams` (`tests/Js2IL.Tests/Node/Url/GeneratorTests.cs`)
