# Node Support Coverage

Target: `22.x LTS`
Generated: `2026-01-16T21:16:04Z`


## Modules

### child_process (status: partial)
Docs: [https://nodejs.org/api/child_process.html](https://nodejs.org/api/child_process.html)
Implementation:
- `JavaScriptRuntime/Node/ChildProcess.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| spawnSync(command, args, options) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processspawnsynccommand-args-options) |
| execSync(command, options) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processexecsynccommand-options) |

### fs/promises (status: partial)
Docs: [https://nodejs.org/api/fs.html#fspromisesapi](https://nodejs.org/api/fs.html#fspromisesapi)
Implementation:
- `JavaScriptRuntime/Node/FSPromises.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| access(path, mode) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesaccesspath-mode) |
| readdir(path, { withFileTypes: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesreaddirpath-options) |
| mkdir(path, { recursive: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesmkdirpath-options) |
| copyFile(src, dest) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisescopyfilesrc-dest-mode) |

### os (status: partial)
Docs: [https://nodejs.org/api/os.html](https://nodejs.org/api/os.html)
Implementation:
- `JavaScriptRuntime/Node/OS.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| tmpdir() | function | supported | [docs](https://nodejs.org/api/os.html#ostmpdir) |
| homedir() | function | supported | [docs](https://nodejs.org/api/os.html#oshomedir) |

### fs (status: partial)
Docs: [https://nodejs.org/api/fs.html](https://nodejs.org/api/fs.html)
Implementation:
- `JavaScriptRuntime/Node/FS.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| existsSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsexistssyncpath) |
| readdirSync(path, { withFileTypes: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreaddirsyncpath-options) |
| readdirSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreaddirsyncpath-options) |
| readFileSync(path[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreadfilesyncpath-options) |
| rmSync(path[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fsrmsyncpath-options) |
| statSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsstatsyncpath-options) |
| writeFileSync(path, data[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fswritefilesyncfile-data-options) |

Tests:
- `existsSync(path)`
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ExistsSync_File_And_Directory` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ExistsSync_EmptyPath_ReturnsFalse` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
- `readdirSync(path, { withFileTypes: true })`
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ReaddirSync_WithFileTypes` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ReaddirSync_NonExistent_ReturnsEmpty` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
- `readdirSync(path)`
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ReaddirSync_Basic_Names` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_ReaddirSync_NonExistent_ReturnsEmpty` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
- `readFileSync(path[, options])`
  - `Js2IL.Tests.Node.ExecutionTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/ExecutionTests.cs#L36`)
  - `Js2IL.Tests.Node.GeneratorTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/GeneratorTests.cs`)
- `rmSync(path[, options])`
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_RmSync_Removes_File_And_Directory` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
- `statSync(path)`
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_StatSync_FileSize` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
  - `Js2IL.Tests.Node.FSAdditionalTests.FS_StatSync_NonExistentPath_ReturnsZero` (`Js2IL.Tests/Node/FSAdditionalTests.cs`)
- `writeFileSync(path, data[, options])`
  - `Js2IL.Tests.Node.ExecutionTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/ExecutionTests.cs#L36`)
  - `Js2IL.Tests.Node.GeneratorTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/GeneratorTests.cs`)

### path (status: partial)
Docs: [https://nodejs.org/api/path.html](https://nodejs.org/api/path.html)
Implementation:
- `JavaScriptRuntime/Node/Path.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| basename(path[, ext]) | function | supported | [docs](https://nodejs.org/api/path.html#pathbasenamepath-suffix) |
| dirname(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathdirnamepath) |
| join(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathjoinpaths) |
| relative(from, to) | function | supported | [docs](https://nodejs.org/api/path.html#pathrelativefrom-to) |
| resolve(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathresolvepaths) |

Tests:
- `basename(path[, ext])`
  - `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Basename_And_Dirname` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)
- `dirname(path)`
  - `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Basename_And_Dirname` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)
- `join(...parts)`
  - `Js2IL.Tests.Node.ExecutionTests.Require_Path_Join_Basic` (`Js2IL.Tests/Node/ExecutionTests.cs#L9`)
  - `Js2IL.Tests.Node.ExecutionTests.Require_Path_Join_NestedFunction` (`Js2IL.Tests/Node/ExecutionTests.cs#L13`)
  - `Js2IL.Tests.Node.GeneratorTests.Require_Path_Join_Basic` (`Js2IL.Tests/Node/GeneratorTests.cs`)
  - `Js2IL.Tests.Node.GeneratorTests.Require_Path_Join_NestedFunction` (`Js2IL.Tests/Node/GeneratorTests.cs`)
- `relative(from, to)`
  - `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Relative_Between_Two_Paths` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)
- `resolve(...parts)`
  - `Js2IL.Tests.Node.PathAdditionalTests.Require_Path_Resolve_Relative_To_Absolute` (`Js2IL.Tests/Node/PathAdditionalTests.cs`)

### perf_hooks (status: partial)
Docs: [https://nodejs.org/api/perf_hooks.html](https://nodejs.org/api/perf_hooks.html)
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

### process (status: partial)
Docs: [https://nodejs.org/api/process.html](https://nodejs.org/api/process.html)
Implementation:
- `JavaScriptRuntime/Node/Process.cs`
- `JavaScriptRuntime/GlobalVariables.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| argv | property | supported | [docs](https://nodejs.org/api/process.html#processargv) |
| exit() | function | supported | [docs](https://nodejs.org/api/process.html#processexitcode) |
| exit(code) | function | supported | [docs](https://nodejs.org/api/process.html#process_exit_code) |
| exitCode | property | supported | [docs](https://nodejs.org/api/process.html#processexitcode) |

Tests:
- `argv`
  - `Js2IL.Tests.Node.ExecutionTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/ExecutionTests.cs#L21`)
  - `Js2IL.Tests.Node.GeneratorTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/GeneratorTests.cs`)
- `exit()`
  - `Js2IL.Tests.Node.ProcessAdditionalTests.Process_Exit_Uses_Current_ExitCode` (`Js2IL.Tests/Node/ProcessAdditionalTests.cs`)
- `exit(code)`
  - `Js2IL.Tests.Node.ProcessAdditionalTests.Process_Exit_Code_Sets_ExitCode` (`Js2IL.Tests/Node/ProcessAdditionalTests.cs`)
- `exitCode`
  - `Js2IL.Tests.Node.ProcessExitCodeTests.Process_exitCode_getter_setter_mirrors_Environment` (`Js2IL.Tests/Node/ProcessExitCodeTests.cs`)


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

### require(id) (status: supported)
Docs: [https://nodejs.org/api/modules.html#requireid](https://nodejs.org/api/modules.html#requireid)
Implementation:
- `JavaScriptRuntime/CommonJS/Require.cs, JavaScriptRuntime/CommonJS/ModuleContext.cs, JavaScriptRuntime/CommonJS/ModuleName.cs, JavaScriptRuntime/CommonJS/Module.cs`
Notes:
Supports requiring implemented Node core modules (e.g., fs/path) and compiled local modules. Local requires support ./ and ../ resolution relative to the importing module and are cached (module body executes once). Includes full module object support (module.exports, module.id, module.filename, module.path, module.loaded, module.parent, module.children, module.paths, module.require). Does not implement node_modules/package.json resolution.
Tests:
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Require_Basic` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Require_NestedNameConflict` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Require_RelativeFromModule` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Require_SharedDependency_ExecutedOnce` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Object` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Reassign` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Function` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Identity` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Loaded` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Require` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Paths` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_ParentChildren` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)

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
Notes:
Writes to stderr.
Tests:
- `JavaScriptRuntime.Tests.ConsoleTests.Error_PrintsAllArgumentsWithSpaces` (`Js2IL.Tests/ConsoleTests.cs`)

### console.warn (status: supported)
Docs: [https://nodejs.org/api/console.html#consolewarndata-args](https://nodejs.org/api/console.html#consolewarndata-args)
Implementation:
- `JavaScriptRuntime/Console.cs`
Notes:
Writes to stderr.
Tests:
- `JavaScriptRuntime.Tests.ConsoleTests.Warn_PrintsAllArgumentsWithSpaces` (`Js2IL.Tests/ConsoleTests.cs`)

### setTimeout (status: supported)
Docs: [https://nodejs.org/api/timers.html#settimeoutcallback-delay-args](https://nodejs.org/api/timers.html#settimeoutcallback-delay-args)
Implementation:
- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs`
Notes:
Schedules a callback to be executed after a specified delay in milliseconds. Returns a timer handle that can be used with clearTimeout.
Tests:
- `Js2IL.Tests.Node.ExecutionTests.SetTimeout_ZeroDelay` (`Js2IL.Tests/Node/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ExecutionTests.SetTimeout_MultipleZeroDelay_ExecutedInOrder` (`Js2IL.Tests/Node/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ExecutionTests.SetTimeout_OneSecondDelay` (`Js2IL.Tests/Node/ExecutionTests.cs`)

### clearTimeout (status: supported)
Docs: [https://nodejs.org/api/timers.html#cleartimeouttimeout](https://nodejs.org/api/timers.html#cleartimeouttimeout)
Implementation:
- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs`
Notes:
Cancels a timer that was previously created with setTimeout. Returns undefined (null in .NET).
Tests:
- `Js2IL.Tests.Node.ExecutionTests.ClearTimeout_ZeroDelay` (`Js2IL.Tests/Node/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ExecutionTests.ClearTimeout_MultipleZeroDelay_ClearSecondTimer` (`Js2IL.Tests/Node/ExecutionTests.cs`)

### setImmediate (status: supported)
Docs: [https://nodejs.org/api/timers.html#setimmediatecallback-args](https://nodejs.org/api/timers.html#setimmediatecallback-args)
Implementation:
- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs, JavaScriptRuntime/Engine/NodeSychronizationContext.cs`
Notes:
Schedules a callback to run on the next event loop iteration. Callbacks execute in FIFO order. Nested setImmediate calls run on the next iteration. Returns a handle that can be used with clearImmediate.
Tests:
- `Js2IL.Tests.Node.ExecutionTests.SetImmediate_ExecutesCallback` (`Js2IL.Tests/Node/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ExecutionTests.SetImmediate_WithArgs_PassesCorrectly` (`Js2IL.Tests/Node/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ExecutionTests.SetImmediate_Multiple_ExecuteInOrder` (`Js2IL.Tests/Node/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ExecutionTests.SetImmediate_ExecutesBeforeSetTimeout` (`Js2IL.Tests/Node/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ExecutionTests.SetImmediate_Nested_ExecutesInNextIteration` (`Js2IL.Tests/Node/ExecutionTests.cs`)

### clearImmediate (status: supported)
Docs: [https://nodejs.org/api/timers.html#clearimmediateimmediate](https://nodejs.org/api/timers.html#clearimmediateimmediate)
Implementation:
- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs, JavaScriptRuntime/Engine/NodeSychronizationContext.cs`
Notes:
Cancels an immediate that was previously created with setImmediate. Returns undefined (null in .NET).
Tests:
- `Js2IL.Tests.Node.ExecutionTests.ClearImmediate_CancelsCallback` (`Js2IL.Tests/Node/ExecutionTests.cs`)

### setInterval (status: supported)
Docs: [https://nodejs.org/api/timers.html#setintervalcallback-delay-args](https://nodejs.org/api/timers.html#setintervalcallback-delay-args)
Implementation:
- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs, JavaScriptRuntime/Engine/NodeSychronizationContext.cs`
Notes:
Schedules a callback to run repeatedly with the specified delay in milliseconds. Returns a handle that can be used with clearInterval. Supports additional arguments passed to the callback.
Tests:
- `Js2IL.Tests.Node.ExecutionTests.SetInterval_ExecutesThreeTimes_ThenClears` (`Js2IL.Tests/Node/ExecutionTests.cs`)

### clearInterval (status: supported)
Docs: [https://nodejs.org/api/timers.html#clearintervaltimeout](https://nodejs.org/api/timers.html#clearintervaltimeout)
Implementation:
- `JavaScriptRuntime/GlobalThis.cs, JavaScriptRuntime/Timers.cs, JavaScriptRuntime/Engine/NodeSychronizationContext.cs`
Notes:
Cancels a repeating timer that was previously created with setInterval. Returns undefined (null in .NET).
Tests:
- `Js2IL.Tests.Node.ExecutionTests.SetInterval_ExecutesThreeTimes_ThenClears` (`Js2IL.Tests/Node/ExecutionTests.cs`)

### Promise (status: supported)
Docs: [https://nodejs.org/api/globals.html#promise](https://nodejs.org/api/globals.html#promise)
Implementation:
- `JavaScriptRuntime/Promise.cs, JavaScriptRuntime/Engine/EngineCore.cs`
Notes:
Promise/A+ compliant implementation with microtask scheduling via IMicrotaskScheduler. Supports constructor, Promise.resolve(), Promise.reject(), then(), catch(), and finally(). Includes proper handling of returned Promises in handlers and chaining semantics.
Tests:
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Executor_Resolved` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Executor_Rejected` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_Then` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Reject_Then` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_ThenFinally` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Reject_FinallyCatch` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_FinallyThen` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Resolve_FinallyThrows` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Then_ReturnsResolvedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Then_ReturnsRejectedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Catch_ReturnsResolvedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Catch_ReturnsRejectedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Finally_ReturnsResolvedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Finally_ReturnsRejectedPromise` (`Js2IL.Tests/Promise/ExecutionTests.cs`)
- `Js2IL.Tests.Promise.ExecutionTests.Promise_Scheduling_StarvationTest` (`Js2IL.Tests/Promise/ExecutionTests.cs`)


## Limitations

- No Buffer support yet; fs APIs operate on UTF-8 text only.
- CommonJS globals (__dirname/__filename) are supported; require() is partially supported for compiled local modules and implemented core modules; ESM import.meta.url is not.
- Only a small subset of Node is implemented to support tests; many APIs are unimplemented.
