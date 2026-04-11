# Module: path

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/path.html) |

## Implementation

- `src/JavaScriptRuntime/Node/Path.cs`

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| sep | property | supported | [docs](https://nodejs.org/api/path.html#pathsep) |
| delimiter | property | supported | [docs](https://nodejs.org/api/path.html#pathdelimiter) |
| posix | property | partial | [docs](https://nodejs.org/api/path.html#pathposix) |
| win32 | property | partial | [docs](https://nodejs.org/api/path.html#pathwin32) |
| join(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathjoinpaths) |
| resolve(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathresolvepaths) |
| relative(from, to) | function | supported | [docs](https://nodejs.org/api/path.html#pathrelativefrom-to) |
| basename(path[, ext]) | function | supported | [docs](https://nodejs.org/api/path.html#pathbasenamepath-suffix) |
| dirname(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathdirnamepath) |
| extname(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathextnamepath) |
| isAbsolute(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathisabsolutepath) |
| normalize(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathnormalizepath) |
| parse(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathparsepath) |
| format(pathObject) | function | supported | [docs](https://nodejs.org/api/path.html#pathformatpathobject) |
| toNamespacedPath(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathtonamespacedpathpath) |

## API Details

### sep

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Normalize_And_Sep` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### delimiter

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Delimiter` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### posix

Provides a minimal POSIX-flavored path API (sep='/', delimiter=':') independent of host OS.

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Posix_Win32_Basics` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### win32

Provides a minimal Windows-flavored path API (sep='\\', delimiter=';') independent of host OS.

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Posix_Win32_Basics` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### join(...parts)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Join_Basic` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Join_NestedFunction` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Join_Normalizes_DotDot` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### resolve(...parts)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Resolve_Relative_To_Absolute` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### relative(from, to)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Relative_Between_Two_Paths` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Relative_SamePath_EmptyString` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### basename(path[, ext])

Ext trimming is supported when the name ends with the provided extension (exact match).

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Basename_And_Dirname` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### dirname(path)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Basename_And_Dirname` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### extname(path)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Extname_And_IsAbsolute` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### isAbsolute(path)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Extname_And_IsAbsolute` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### normalize(path)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Normalize_And_Sep` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### parse(path)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Parse_And_Format` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### format(pathObject)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_Parse_And_Format` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)

### toNamespacedPath(path)

**Tests:**
- `Js2IL.Tests.Node.Path.ExecutionTests.Require_Path_ToNamespacedPath` (`tests/Js2IL.Tests/Node/Path/ExecutionTests.cs`)
