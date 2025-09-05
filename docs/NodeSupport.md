# Node Support Coverage

Target: `22.x LTS`
Generated: `2025-09-05T17:10:31Z`


## Modules

### path (status: partial)
Docs: [https://nodejs.org/api/path.html](https://nodejs.org/api/path.html)
Implementation:
- `JavaScriptRuntime/Node/Path.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| join(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathjoinpaths) |

Tests:
- `join(...parts)`
  - `Js2IL.Tests.Node.ExecutionTests.Require_Path_Join_Basic` (`Js2IL.Tests/Node/ExecutionTests.cs#L9`)
  - `Js2IL.Tests.Node.ExecutionTests.Require_Path_Join_NestedFunction` (`Js2IL.Tests/Node/ExecutionTests.cs#L13`)
  - `Js2IL.Tests.Node.GeneratorTests.Require_Path_Join_Basic` (`Js2IL.Tests/Node/GeneratorTests.cs`)
  - `Js2IL.Tests.Node.GeneratorTests.Require_Path_Join_NestedFunction` (`Js2IL.Tests/Node/GeneratorTests.cs`)

### fs (status: partial)
Docs: [https://nodejs.org/api/fs.html](https://nodejs.org/api/fs.html)
Implementation:
- `JavaScriptRuntime/Node/FS.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| readFileSync(path[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreadfilesyncpath-options) |
| writeFileSync(path, data[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fswritefilesyncfile-data-options) |

Tests:
- `readFileSync(path[, options])`
  - `Js2IL.Tests.Node.ExecutionTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/ExecutionTests.cs#L36`)
  - `Js2IL.Tests.Node.GeneratorTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/GeneratorTests.cs`)
- `writeFileSync(path, data[, options])`
  - `Js2IL.Tests.Node.ExecutionTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/ExecutionTests.cs#L36`)
  - `Js2IL.Tests.Node.GeneratorTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/GeneratorTests.cs`)

### process (status: partial)
Docs: [https://nodejs.org/api/process.html](https://nodejs.org/api/process.html)
Implementation:
- `JavaScriptRuntime/Node/Process.cs`
- `JavaScriptRuntime/Node/GlobalVariables.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| exitCode | property | supported | [docs](https://nodejs.org/api/process.html#processexitcode) |
| argv | property | supported | [docs](https://nodejs.org/api/process.html#processargv) |

Tests:
- `exitCode`
  - `Js2IL.Tests.Node.ProcessExitCodeTests.Process_exitCode_getter_setter_mirrors_Environment` (`Js2IL.Tests/Node/ProcessExitCodeTests.cs`)
- `argv`
  - `Js2IL.Tests.Node.ExecutionTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/ExecutionTests.cs#L21`)
  - `Js2IL.Tests.Node.GeneratorTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/GeneratorTests.cs`)


## Globals

### __dirname (status: supported)
Docs: [https://nodejs.org/api/modules.html#dirname](https://nodejs.org/api/modules.html#dirname)
Implementation:
- `JavaScriptRuntime/Node/GlobalVariables.cs`
Tests:
- `Js2IL.Tests.Node.ExecutionTests.Global___dirname_PrintsDirectory` (`Js2IL.Tests/Node/ExecutionTests.cs#L15`)
- `Js2IL.Tests.Node.GeneratorTests.Global___dirname_PrintsDirectory` (`Js2IL.Tests/Node/GeneratorTests.cs`)

### __filename (status: supported)
Docs: [https://nodejs.org/api/modules.html#filename](https://nodejs.org/api/modules.html#filename)
Implementation:
- `JavaScriptRuntime/Node/GlobalVariables.cs`
Tests:
- `Js2IL.Tests.Node.ExecutionTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/ExecutionTests.cs#L21`)
- `Js2IL.Tests.Node.GeneratorTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/GeneratorTests.cs`)

### console.log (status: supported)
Docs: [https://nodejs.org/api/console.html#consolelogdata-args](https://nodejs.org/api/console.html#consolelogdata-args)
Implementation:
- `JavaScriptRuntime/Console.cs`
Tests:
- `Js2IL.Tests.ConsoleTests` (`Js2IL.Tests/ConsoleTests.cs`)

### console.error (status: supported)
Docs: [https://nodejs.org/api/console.html#consoleerrordata-args](https://nodejs.org/api/console.html#consoleerrordata-args)
Implementation:
- `JavaScriptRuntime/Console.cs`
Tests:
- `JavaScriptRuntime.Tests.ConsoleTests.Error_PrintsAllArgumentsWithSpaces` (`Js2IL.Tests/ConsoleTests.cs`)

### console.warn (status: supported)
Docs: [https://nodejs.org/api/console.html#consolewarndata-args](https://nodejs.org/api/console.html#consolewarndata-args)
Implementation:
- `JavaScriptRuntime/Console.cs`
Tests:
- `JavaScriptRuntime.Tests.ConsoleTests.Warn_PrintsAllArgumentsWithSpaces` (`Js2IL.Tests/ConsoleTests.cs`)


## Limitations

- No Buffer support yet; fs APIs operate on UTF-8 text only.
- CommonJS globals (__dirname/__filename) are supported; ESM import.meta.url is not.
- Only a small subset of Node is implemented to support tests; many APIs are unimplemented.
