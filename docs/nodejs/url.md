# Module: url

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/url.html) |

## Implementation

- `JavaScriptRuntime/Node/Url.cs`
- `JavaScriptRuntime/Node/UrlQueryHelpers.cs`

## Notes

Provides a focused WHATWG URL baseline for typical http(s) and file URLs, including base-relative resolution, URLSearchParams basics, and fileURLToPath/pathToFileURL helpers. Legacy url.parse/url.format APIs and global URL exposure are not implemented yet.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| URL | class | supported | [docs](https://nodejs.org/api/url.html#new-urlinput-base) |
| URLSearchParams | class | partial | [docs](https://nodejs.org/api/url.html#class-urlsearchparams) |
| fileURLToPath(url) | function | supported | [docs](https://nodejs.org/api/url.html#urlfileurltopathurl-options) |
| pathToFileURL(path) | function | supported | [docs](https://nodejs.org/api/url.html#urlpathtofileurlpath-options) |

## API Details

### URL

Supports absolute URL parsing, base-relative resolution, href/origin/protocol/username/password/host/hostname/port/pathname/search/hash accessors, and a live searchParams object.

**Tests:**
- `Js2IL.Tests.Node.Url.ExecutionTests.Require_Url_Parse_And_Base` (`Js2IL.Tests/Node/Url/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Url.ExecutionTests.Require_Url_SearchParams_Mutate` (`Js2IL.Tests/Node/Url/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Url.ExecutionTests.Require_Url_Invalid_With_Base_Throws` (`Js2IL.Tests/Node/Url/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Url.GeneratorTests.Require_Url_Parse_And_Base` (`Js2IL.Tests/Node/Url/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Url.GeneratorTests.Require_Url_SearchParams_Mutate` (`Js2IL.Tests/Node/Url/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Url.GeneratorTests.Require_Url_Invalid_With_Base_Throws` (`Js2IL.Tests/Node/Url/GeneratorTests.cs`)

### URLSearchParams

Supports string initialization plus get/getAll/has/append/set/delete/sort/forEach/entries/keys/values/toString for common query workflows.

**Tests:**
- `Js2IL.Tests.Node.Url.ExecutionTests.Require_Url_SearchParams_Mutate` (`Js2IL.Tests/Node/Url/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Url.GeneratorTests.Require_Url_SearchParams_Mutate` (`Js2IL.Tests/Node/Url/GeneratorTests.cs`)

### fileURLToPath(url)

**Tests:**
- `Js2IL.Tests.Node.Url.ExecutionTests.Require_Url_File_Helpers` (`Js2IL.Tests/Node/Url/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Url.GeneratorTests.Require_Url_File_Helpers` (`Js2IL.Tests/Node/Url/GeneratorTests.cs`)

### pathToFileURL(path)

**Tests:**
- `Js2IL.Tests.Node.Url.ExecutionTests.Require_Url_File_Helpers` (`Js2IL.Tests/Node/Url/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Url.GeneratorTests.Require_Url_File_Helpers` (`Js2IL.Tests/Node/Url/GeneratorTests.cs`)
