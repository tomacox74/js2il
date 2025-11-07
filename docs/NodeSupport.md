# Node Support Coverage

Target: `22.x LTS`
Generated: `2025-11-07T00:32:36.047Z`


## Modules

### path (status: partial)
Docs: [docs](https://nodejs.org/api/path.html)
Implementation:
- `JavaScriptRuntime/Node/Path.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| join(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathjoinpaths) |
| resolve(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathresolvepaths) |
| relative(from, to) | function | supported | [docs](https://nodejs.org/api/path.html#pathrelativefrom-to) |
| basename(path[, ext]) | function | supported | [docs](https://nodejs.org/api/path.html#pathbasenamepath-suffix) |
| dirname(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathdirnamepath) |

Tests:
- `join(...parts)`
  - `Js2IL.Tests.Node.ExecutionTests.Require_Path_Join_Basic` (`Js2IL.Tests/Node/ExecutionTests.cs#L9`)
  - `Js2IL.Tests.Node.ExecutionTests.Require_Path_Join_NestedFunction` (`Js2IL.Tests/Node/ExecutionTests.cs#L13`)
  - `Js2IL.Tests.Node.GeneratorTests.Require_Path_Join_Basic` (`Js2IL.Tests/Node/GeneratorTests.cs`)
  - `Js2IL.Tests.Node.GeneratorTests.Require_Path_Join_NestedFunction` (`Js2IL.Tests/Node/GeneratorTests.cs`)
- `resolve(...parts)`
  - `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Resolve_Relative_To_Absolute` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)
- `relative(from, to)`
  - `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Relative_Between_Two_Paths` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)
- `basename(path[, ext])`
  - `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Basename_And_Dirname` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)
- `dirname(path)`
  - `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Basename_And_Dirname` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)

### perf_hooks (status: partial)
Docs: [docs](https://nodejs.org/api/perf_hooks.html)
Implementation:
- `JavaScriptRuntime/Node/PerfHooks.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| performance | property | supported | [docs](https://nodejs.org/api/perf_hooks.html#performance) |
| performance.now() | function | supported | [docs](https://nodejs.org/api/perf_hooks.html#performancenow) |

Tests:
- `performance.now()`
  - `Js2IL.Tests.Node.ExecutionTests.PerfHooks_PerformanceNow_Basic` (`Js2IL.Tests/Node/ExecutionTests.cs`)
  - `Js2IL.Tests.Node.GeneratorTests.PerfHooks_PerformanceNow_Basic` (`Js2IL.Tests/Node/GeneratorTests.cs`)

### fs (status: partial)
Docs: [docs](https://nodejs.org/api/fs.html)
Implementation:
- `JavaScriptRuntime/Node/FS.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| readFileSync(path[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreadfilesyncpath-options) |
| writeFileSync(path, data[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fswritefilesyncfile-data-options) |
| existsSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsexistssyncpath) |
| readdirSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreaddirsyncpath-options) |
| readdirSync(path, { withFileTypes: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreaddirsyncpath-options) |
| statSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsstatsyncpath-options) |
| rmSync(path[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fsrmsyncpath-options) |

Tests:
- `readFileSync(path[, options])`
  - `Js2IL.Tests.Node.ExecutionTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/ExecutionTests.cs#L36`)
  - `Js2IL.Tests.Node.GeneratorTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/GeneratorTests.cs`)
- `writeFileSync(path, data[, options])`
  - `Js2IL.Tests.Node.ExecutionTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/ExecutionTests.cs#L36`)
  - `Js2IL.Tests.Node.GeneratorTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/GeneratorTests.cs`)
- `existsSync(path)`
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ExistsSync_File_And_Directory` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ExistsSync_EmptyPath_ReturnsFalse` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
- `readdirSync(path)`
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ReaddirSync_Basic_Names` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ReaddirSync_NonExistent_ReturnsEmpty` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
- `readdirSync(path, { withFileTypes: true })`
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ReaddirSync_WithFileTypes` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ReaddirSync_NonExistent_ReturnsEmpty` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
- `statSync(path)`
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_StatSync_FileSize` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_StatSync_NonExistentPath_ReturnsZero` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
- `rmSync(path[, options])`
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_RmSync_Removes_File_And_Directory` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)

### process (status: partial)
Docs: [docs](https://nodejs.org/api/process.html)
Implementation:
- `JavaScriptRuntime/Node/Process.cs`
- `JavaScriptRuntime/GlobalVariables.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| exitCode | property | supported | [docs](https://nodejs.org/api/process.html#processexitcode) |
| exit() | function | supported | [docs](https://nodejs.org/api/process.html#processexitcode) |
| exit(code) | function | supported | [docs](https://nodejs.org/api/process.html#process_exit_code) |
| argv | property | supported | [docs](https://nodejs.org/api/process.html#processargv) |

Tests:
- `exitCode`
  - `Js2IL.Tests.Node.ProcessExitCodeTests.Process_exitCode_getter_setter_mirrors_Environment` (`Js2IL.Tests/Node/ProcessExitCodeTests.cs`)
- `exit()`
  - `Js2IL.Tests.Node.ProcessAdditionalTests.Process_Exit_Uses_Current_ExitCode` (`Js2IL.Tests/Node/ProcessAdditionalTests.cs`)
- `exit(code)`
  - `Js2IL.Tests.Node.ProcessAdditionalTests.Process_Exit_Code_Sets_ExitCode` (`Js2IL.Tests/Node/ProcessAdditionalTests.cs`)
- `argv`
  - `Js2IL.Tests.Node.ExecutionTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/ExecutionTests.cs#L21`)
  - `Js2IL.Tests.Node.GeneratorTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/GeneratorTests.cs`)


## Globals

### __dirname (status: supported)
Docs: [docs](https://nodejs.org/api/modules.html#dirname)
Implementation:
- `JavaScriptRuntime/Node/GlobalVariables.cs`
Tests:
- `Js2IL.Tests.Node.ExecutionTests.Global___dirname_PrintsDirectory` (`Js2IL.Tests/Node/ExecutionTests.cs#L15`)
- `Js2IL.Tests.Node.GeneratorTests.Global___dirname_PrintsDirectory` (`Js2IL.Tests/Node/GeneratorTests.cs`)

### __filename (status: supported)
Docs: [docs](https://nodejs.org/api/modules.html#filename)
Implementation:
- `JavaScriptRuntime/Node/GlobalVariables.cs`
Tests:
- `Js2IL.Tests.Node.ExecutionTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/ExecutionTests.cs#L21`)
- `Js2IL.Tests.Node.GeneratorTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/GeneratorTests.cs`)

### console.log (status: supported)
Docs: [docs](https://nodejs.org/api/console.html#consolelogdata-args)
Implementation:
- `JavaScriptRuntime/Console.cs`
Tests:
- `Js2IL.Tests.ConsoleTests` (`Js2IL.Tests/ConsoleTests.cs`)

### console.error (status: supported)
Docs: [docs](https://nodejs.org/api/console.html#consoleerrordata-args)
Implementation:
- `JavaScriptRuntime/Console.cs`
Notes:
Writes to stderr.
Tests:
- `JavaScriptRuntime.Tests.ConsoleTests.Error_PrintsAllArgumentsWithSpaces` (`Js2IL.Tests/ConsoleTests.cs`)

### console.warn (status: supported)
Docs: [docs](https://nodejs.org/api/console.html#consolewarndata-args)
Implementation:
- `JavaScriptRuntime/Console.cs`
Notes:
Writes to stderr.
Tests:
- `JavaScriptRuntime.Tests.ConsoleTests.Warn_PrintsAllArgumentsWithSpaces` (`Js2IL.Tests/ConsoleTests.cs`)


## Limitations

- No Buffer support yet; fs APIs operate on UTF-8 text only.
- CommonJS globals (__dirname/__filename) are supported; ESM import.meta.url is not.
- Only a small subset of Node is implemented to support tests; many APIs are unimplemented.
