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
| join(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathjoinpaths) |
| relative(from, to) | function | supported | [docs](https://nodejs.org/api/path.html#pathrelativefrom-to) |
| resolve(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathresolvepaths) |

## API Details

### basename(path[, ext])

Ext trimming is supported when the name ends with the provided extension (exact match).

**Tests:**
- `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Basename_And_Dirname` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)

### dirname(path)

**Tests:**
- `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Basename_And_Dirname` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)

### join(...parts)

**Tests:**
- `Js2IL.Tests.Node.ExecutionTests.Require_Path_Join_Basic` (`Js2IL.Tests/Node/ExecutionTests.cs#L9`)
- `Js2IL.Tests.Node.ExecutionTests.Require_Path_Join_NestedFunction` (`Js2IL.Tests/Node/ExecutionTests.cs#L13`)
- `Js2IL.Tests.Node.GeneratorTests.Require_Path_Join_Basic` (`Js2IL.Tests/Node/GeneratorTests.cs`)
- `Js2IL.Tests.Node.GeneratorTests.Require_Path_Join_NestedFunction` (`Js2IL.Tests/Node/GeneratorTests.cs`)

### relative(from, to)

Uses System.IO.Path.GetRelativePath for platform-correct separators.

**Tests:**
- `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Relative_Between_Two_Paths` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)

### resolve(...parts)

Resolves from right to left until an absolute path is constructed and returns a normalized absolute path via GetFullPath. Behavior is sufficient for current docs/scripts needs; edge cases like drive-letter nuances may differ from Node on non-Windows.

**Tests:**
- `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Resolve_Relative_To_Absolute` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)
