# Module: path

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/path.html) |

## Implementation

- `JavaScriptRuntime/Node/Path.cs`

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| basename(path[, ext]) | function | supported | [docs](https://nodejs.org/api/path.html#pathbasenamepath-suffix) |
| dirname(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathdirnamepath) |
| extname(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathextnamepath) |
| isAbsolute(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathisabsolutepath) |
| normalize(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathnormalizepath) |
| parse(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathparsepath) |
| join(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathjoinpaths) |
| relative(from, to) | function | supported | [docs](https://nodejs.org/api/path.html#pathrelativefrom-to) |
| resolve(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathresolvepaths) |
| format(pathObject) | function | supported | [docs](https://nodejs.org/api/path.html#pathformatpathobject) |
| sep | property | supported | [docs](https://nodejs.org/api/path.html#pathsep) |
| delimiter | property | supported | [docs](https://nodejs.org/api/path.html#pathdelimiter) |
| toNamespacedPath(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathtonamespacedpathpath) |

## API Details

### basename(path[, ext])

Ext trimming is supported when the name ends with the provided extension (exact match).

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Basename_And_Dirname` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### dirname(path)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Basename_And_Dirname` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### extname(path)

Supports common extension extraction semantics including empty-extension and trailing-dot cases.

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Extname_And_IsAbsolute` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### isAbsolute(path)

Uses platform-rooted path checks for common absolute/relative path scenarios.

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Extname_And_IsAbsolute` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### normalize(path)

Normalizes separators and dot segments for common relative and absolute path inputs.

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Normalize_And_Sep` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### parse(path)

Returns an object with root, dir, base, ext, and name for common path parsing scenarios.

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Parse_And_Format` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### join(...parts)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Join_Basic` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Join_NestedFunction` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Path.GeneratorTests.Require_Path_Join_Basic` (`Js2IL.Tests/Node/Path/GeneratorTests.cs`)
- `Js2IL.Tests.Node.Path.GeneratorTests.Require_Path_Join_NestedFunction` (`Js2IL.Tests/Node/Path/GeneratorTests.cs`)

### relative(from, to)

Uses platform-correct separators and returns an empty string when both paths resolve to the same location (Node-compatible behavior).

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Relative_Between_Two_Paths` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Relative_SamePath_EmptyString` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### resolve(...parts)

Resolves from right to left until an absolute path is constructed and returns a normalized absolute path via GetFullPath. Behavior is sufficient for current docs/scripts needs; edge cases like drive-letter nuances may differ from Node on non-Windows.

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Resolve_Relative_To_Absolute` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### format(pathObject)

Formats a path object using dir/root + base/name/ext precedence for common Node-style composition.

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Parse_And_Format` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### sep

Exposes the platform path segment separator character.

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Normalize_And_Sep` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### delimiter

Exposes the platform path list delimiter character (for example ':' on POSIX and ';' on Windows).

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Delimiter` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### toNamespacedPath(path)

Currently returns the provided path unchanged for common cross-platform compatibility scenarios.

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_ToNamespacedPath` (`Js2IL.Tests/Node/Path/ExecutionTests.cs`)
