# Node Support Coverage

Target: `22.x LTS`
Generated: `2026-03-14T19:50:36Z`


## Modules

### child_process (status: partial)
Docs: [https://nodejs.org/api/child_process.html](https://nodejs.org/api/child_process.html)
Implementation:
- `src/JavaScriptRuntime/Node/ChildProcess.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| spawnSync(command, args, options) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processspawnsynccommand-args-options) |
| execSync(command, options) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processexecsynccommand-options) |

### fs/promises (status: partial)
Docs: [https://nodejs.org/api/fs.html#fspromisesapi](https://nodejs.org/api/fs.html#fspromisesapi)
Implementation:
- `src/JavaScriptRuntime/Node/FSPromises.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| access(path, mode) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesaccesspath-mode) |
| readdir(path, { withFileTypes: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesreaddirpath-options) |
| mkdir(path, { recursive: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesmkdirpath-options) |
| copyFile(src, dest) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisescopyfilesrc-dest-mode) |

### os (status: partial)
Docs: [https://nodejs.org/api/os.html](https://nodejs.org/api/os.html)
Implementation:
- `src/JavaScriptRuntime/Node/OS.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| tmpdir() | function | supported | [docs](https://nodejs.org/api/os.html#ostmpdir) |
| homedir() | function | supported | [docs](https://nodejs.org/api/os.html#oshomedir) |

### fs (status: partial)
Docs: [https://nodejs.org/api/fs.html](https://nodejs.org/api/fs.html)
Implementation:
- `src/JavaScriptRuntime/Node/FS.cs`

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
  - `Jroc.Tests.Node.FSAdditionalTests.FS_ExistsSync_File_And_Directory` (`tests/Jroc.Tests/Node/FSAdditionalTests.cs`)
  - `Jroc.Tests.Node.FSAdditionalTests.FS_ExistsSync_EmptyPath_ReturnsFalse` (`tests/Jroc.Tests/Node/FSAdditionalTests.cs`)
- `readdirSync(path, { withFileTypes: true })`
  - `Jroc.Tests.Node.FSAdditionalTests.FS_ReaddirSync_WithFileTypes` (`tests/Jroc.Tests/Node/FSAdditionalTests.cs`)
  - `Jroc.Tests.Node.FSAdditionalTests.FS_ReaddirSync_NonExistent_ReturnsEmpty` (`tests/Jroc.Tests/Node/FSAdditionalTests.cs`)
- `readdirSync(path)`
  - `Jroc.Tests.Node.FSAdditionalTests.FS_ReaddirSync_Basic_Names` (`tests/Jroc.Tests/Node/FSAdditionalTests.cs`)
  - `Jroc.Tests.Node.FSAdditionalTests.FS_ReaddirSync_NonExistent_ReturnsEmpty` (`tests/Jroc.Tests/Node/FSAdditionalTests.cs`)
- `readFileSync(path[, options])`
  - `Jroc.Tests.Node.ExecutionTests.FS_ReadWrite_Utf8` (`tests/Jroc.Tests/Node/ExecutionTests.cs#L36`)
  - `Jroc.Tests.Node.GeneratorTests.FS_ReadWrite_Utf8` (`tests/Jroc.Tests/Node/GeneratorTests.cs`)
- `rmSync(path[, options])`
  - `Jroc.Tests.Node.FSAdditionalTests.FS_RmSync_Removes_File_And_Directory` (`tests/Jroc.Tests/Node/FSAdditionalTests.cs`)
- `statSync(path)`
  - `Jroc.Tests.Node.FSAdditionalTests.FS_StatSync_FileSize` (`tests/Jroc.Tests/Node/FSAdditionalTests.cs`)
  - `Jroc.Tests.Node.FSAdditionalTests.FS_StatSync_NonExistentPath_ReturnsZero` (`tests/Jroc.Tests/Node/FSAdditionalTests.cs`)
- `writeFileSync(path, data[, options])`
  - `Jroc.Tests.Node.ExecutionTests.FS_ReadWrite_Utf8` (`tests/Jroc.Tests/Node/ExecutionTests.cs#L36`)
  - `Jroc.Tests.Node.GeneratorTests.FS_ReadWrite_Utf8` (`tests/Jroc.Tests/Node/GeneratorTests.cs`)

### path (status: partial)
Docs: [https://nodejs.org/api/path.html](https://nodejs.org/api/path.html)
Implementation:
- `src/JavaScriptRuntime/Node/Path.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| basename(path[, ext]) | function | supported | [docs](https://nodejs.org/api/path.html#pathbasenamepath-suffix) |
| dirname(path) | function | supported | [docs](https://nodejs.org/api/path.html#pathdirnamepath) |
| join(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathjoinpaths) |
| relative(from, to) | function | supported | [docs](https://nodejs.org/api/path.html#pathrelativefrom-to) |
| resolve(...parts) | function | supported | [docs](https://nodejs.org/api/path.html#pathresolvepaths) |

Tests:
- `basename(path[, ext])`
  - `Jroc.Tests.Node.PathAdditionalTests.Require_Path_Basename_And_Dirname` (`tests/Jroc.Tests/Node/PathAdditionalTests.cs`)
- `dirname(path)`
  - `Jroc.Tests.Node.PathAdditionalTests.Require_Path_Basename_And_Dirname` (`tests/Jroc.Tests/Node/PathAdditionalTests.cs`)
- `join(...parts)`
  - `Jroc.Tests.Node.ExecutionTests.Require_Path_Join_Basic` (`tests/Jroc.Tests/Node/ExecutionTests.cs#L9`)
  - `Jroc.Tests.Node.ExecutionTests.Require_Path_Join_NestedFunction` (`tests/Jroc.Tests/Node/ExecutionTests.cs#L13`)
  - `Jroc.Tests.Node.GeneratorTests.Require_Path_Join_Basic` (`tests/Jroc.Tests/Node/GeneratorTests.cs`)
  - `Jroc.Tests.Node.GeneratorTests.Require_Path_Join_NestedFunction` (`tests/Jroc.Tests/Node/GeneratorTests.cs`)
- `relative(from, to)`
  - `Jroc.Tests.Node.PathAdditionalTests.Require_Path_Relative_Between_Two_Paths` (`tests/Jroc.Tests/Node/PathAdditionalTests.cs`)
- `resolve(...parts)`
  - `Jroc.Tests.Node.PathAdditionalTests.Require_Path_Resolve_Relative_To_Absolute` (`tests/Jroc.Tests/Node/PathAdditionalTests.cs`)

### perf_hooks (status: partial)
Docs: [https://nodejs.org/api/perf_hooks.html](https://nodejs.org/api/perf_hooks.html)
Implementation:
- `src/JavaScriptRuntime/Node/PerfHooks.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| performance | property | supported | [docs](https://nodejs.org/api/perf_hooks.html#performance) |
| performance.now() | function | supported | [docs](https://nodejs.org/api/perf_hooks.html#performancenow) |

Tests:
- `performance.now()`
  - `Jroc.Tests.Node.ExecutionTests.PerfHooks_PerformanceNow_Basic` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
  - `Jroc.Tests.Node.GeneratorTests.PerfHooks_PerformanceNow_Basic` (`tests/Jroc.Tests/Node/GeneratorTests.cs`)

### process (status: partial)
Docs: [https://nodejs.org/api/process.html](https://nodejs.org/api/process.html)
Implementation:
- `src/JavaScriptRuntime/Node/Process.cs`
- `src/JavaScriptRuntime/GlobalVariables.cs`

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| argv | property | supported | [docs](https://nodejs.org/api/process.html#processargv) |
| exit() | function | supported | [docs](https://nodejs.org/api/process.html#processexitcode) |
| exit(code) | function | supported | [docs](https://nodejs.org/api/process.html#process_exit_code) |
| exitCode | property | supported | [docs](https://nodejs.org/api/process.html#processexitcode) |
| platform | property | supported | [docs](https://nodejs.org/api/process.html#processplatform) |
| versions.node | property | supported | [docs](https://nodejs.org/api/process.html#processversions) |
| versions.v8 | property | supported | [docs](https://nodejs.org/api/process.html#processversions) |
| versions.modules | property | supported | [docs](https://nodejs.org/api/process.html#processversions) |
| versions.jroc | property | supported | [docs](https://nodejs.org/api/process.html#processversions) |
| versions.dotnet | property | supported | [docs](https://nodejs.org/api/process.html#processversions) |
| env | property | supported | [docs](https://nodejs.org/api/process.html#processenv) |
| chdir(directory) | function | supported | [docs](https://nodejs.org/api/process.html#processchdirdirectory) |
| cwd() | function | supported | [docs](https://nodejs.org/api/process.html#processcwd) |
| nextTick(callback, ...args) | function | supported | [docs](https://nodejs.org/api/process.html#processnexttickcallback-args) |

Tests:
- `argv`
  - `Jroc.Tests.Node.ExecutionTests.Environment_EnumerateProcessArgV` (`tests/Jroc.Tests/Node/ExecutionTests.cs#L21`)
  - `Jroc.Tests.Node.GeneratorTests.Environment_EnumerateProcessArgV` (`tests/Jroc.Tests/Node/GeneratorTests.cs`)
- `exit()`
  - `Jroc.Tests.Node.ProcessAdditionalTests.Process_Exit_Uses_Current_ExitCode` (`tests/Jroc.Tests/Node/ProcessAdditionalTests.cs`)
- `exit(code)`
  - `Jroc.Tests.Node.ProcessAdditionalTests.Process_Exit_Code_Sets_ExitCode` (`tests/Jroc.Tests/Node/ProcessAdditionalTests.cs`)
- `exitCode`
  - `Jroc.Tests.Node.ProcessExitCodeTests.Process_exitCode_getter_setter_mirrors_Environment` (`tests/Jroc.Tests/Node/ProcessExitCodeTests.cs`)
- `platform`
  - `Jroc.Tests.Node.Process.ExecutionTests.Process_Platform_Versions_And_Env_Basics` (`tests/Jroc.Tests/Node/Process/ExecutionTests.cs`)
- `versions.node`
  - `Jroc.Tests.Node.Process.ExecutionTests.Process_Platform_Versions_And_Env_Basics` (`tests/Jroc.Tests/Node/Process/ExecutionTests.cs`)
- `versions.v8`
  - `Jroc.Tests.Node.Process.ExecutionTests.Process_Versions_Expanded` (`tests/Jroc.Tests/Node/Process/ExecutionTests.cs`)
- `versions.modules`
  - `Jroc.Tests.Node.Process.ExecutionTests.Process_Versions_Expanded` (`tests/Jroc.Tests/Node/Process/ExecutionTests.cs`)
- `versions.jroc`
  - `Jroc.Tests.Node.Process.ExecutionTests.Process_Versions_Expanded` (`tests/Jroc.Tests/Node/Process/ExecutionTests.cs`)
- `versions.dotnet`
  - `Jroc.Tests.Node.Process.ExecutionTests.Process_Versions_Expanded` (`tests/Jroc.Tests/Node/Process/ExecutionTests.cs`)
- `env`
  - `Jroc.Tests.Node.Process.ExecutionTests.Process_Platform_Versions_And_Env_Basics` (`tests/Jroc.Tests/Node/Process/ExecutionTests.cs`)
- `chdir(directory)`
  - `Jroc.Tests.Node.Process.ExecutionTests.Process_Chdir_And_NextTick_Basics` (`tests/Jroc.Tests/Node/Process/ExecutionTests.cs`)
- `cwd()`
  - `Jroc.Tests.Node.Process.ExecutionTests.Process_Chdir_And_NextTick_Basics` (`tests/Jroc.Tests/Node/Process/ExecutionTests.cs`)
- `nextTick(callback, ...args)`
  - `Jroc.Tests.Node.Process.ExecutionTests.Process_Chdir_And_NextTick_Basics` (`tests/Jroc.Tests/Node/Process/ExecutionTests.cs`)


## Globals

### __dirname (status: supported)
Docs: [https://nodejs.org/api/modules.html#dirname](https://nodejs.org/api/modules.html#dirname)
Implementation:
- `src/JavaScriptRuntime/Node/GlobalVariables.cs`
Tests:
- `Jroc.Tests.Node.ExecutionTests.Global___dirname_PrintsDirectory` (`tests/Jroc.Tests/Node/ExecutionTests.cs#L15`)
- `Jroc.Tests.Node.GeneratorTests.Global___dirname_PrintsDirectory` (`tests/Jroc.Tests/Node/GeneratorTests.cs`)

### __filename (status: supported)
Docs: [https://nodejs.org/api/modules.html#filename](https://nodejs.org/api/modules.html#filename)
Implementation:
- `src/JavaScriptRuntime/Node/GlobalVariables.cs`
Tests:
- `Jroc.Tests.Node.ExecutionTests.Environment_EnumerateProcessArgV` (`tests/Jroc.Tests/Node/ExecutionTests.cs#L21`)
- `Jroc.Tests.Node.GeneratorTests.Environment_EnumerateProcessArgV` (`tests/Jroc.Tests/Node/GeneratorTests.cs`)

### require(id) (status: supported)
Docs: [https://nodejs.org/api/modules.html#requireid](https://nodejs.org/api/modules.html#requireid)
Implementation:
- `src/JavaScriptRuntime/CommonJS/Require.cs, src/JavaScriptRuntime/CommonJS/ModuleContext.cs, src/JavaScriptRuntime/CommonJS/ModuleName.cs, src/JavaScriptRuntime/CommonJS/Module.cs`
Notes:
Supports requiring implemented Node core modules (e.g., fs/path) and compiled local modules. Local requires support ./ and ../ resolution relative to the importing module and are cached (module body executes once). Also supports compile-time resolution of npm packages via node_modules discovery, .js/.mjs/.cjs files, package.json main, package.json type=module entry graphs, conditional exports/imports with import/require/node/default conditions, single-* subpath patterns, and package.json imports aliases that target either package-local relative paths (./...) or bare package specifiers in the supported deterministic slice. Static import/export declarations and literal import()/require() package requests are resolved at compile time so import and require can target different entries from the same package graph. Runtime require does not probe the file system; packages must be discovered at compile time. Custom loaders/hooks, nested package-imports aliases, and broader runtime probing remain unsupported. Includes full module object support (module.exports, module.id, module.filename, module.path, module.loaded, module.parent, module.children, module.paths, module.require).
Tests:
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Require_Basic` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Require_NestedNameConflict` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Require_RelativeFromModule` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Require_SharedDependency_ExecutedOnce` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Object` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Reassign` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Function` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Identity` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Loaded` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Require` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Paths` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_ParentChildren` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesExecutionTests.CommonJS_Require_NodeModules_PackageJson_Exports_And_NestedDependency` (`tests/Jroc.Tests/CommonJS/NodeModulesExecutionTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesExecutionTests.CommonJS_Require_NodeModules_DualMode_Exports_Imports_TypeModule_And_MjsEntry` (`tests/Jroc.Tests/CommonJS/NodeModulesExecutionTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesExecutionTests.CommonJS_Require_NodeModules_PackageImports_BarePackageAlias` (`tests/Jroc.Tests/CommonJS/NodeModulesExecutionTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesExecutionTests.CommonJS_Require_NodeModules_UnsupportedConditions_ReportDiagnostic` (`tests/Jroc.Tests/CommonJS/NodeModulesExecutionTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesGeneratorTests.CommonJS_NodeModules_DualMode_Exports_Imports_TypeModule_And_MjsEntry_EmitsManifest` (`tests/Jroc.Tests/CommonJS/NodeModulesGeneratorTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesGeneratorTests.CommonJS_NodeModules_PackageImports_BarePackageAlias_EmitsManifest` (`tests/Jroc.Tests/CommonJS/NodeModulesGeneratorTests.cs`)

### console.log (status: supported)
Docs: [https://nodejs.org/api/console.html#consolelogdata-args](https://nodejs.org/api/console.html#consolelogdata-args)
Implementation:
- `src/JavaScriptRuntime/Console.cs`
Tests:
- `Jroc.Tests.ConsoleTests` (`tests/Jroc.Tests/ConsoleTests.cs`)

### console.error (status: supported)
Docs: [https://nodejs.org/api/console.html#consoleerrordata-args](https://nodejs.org/api/console.html#consoleerrordata-args)
Implementation:
- `src/JavaScriptRuntime/Console.cs`
Notes:
Writes to stderr.
Tests:
- `JavaScriptRuntime.Tests.ConsoleTests.Error_PrintsAllArgumentsWithSpaces` (`tests/Jroc.Tests/ConsoleTests.cs`)

### console.warn (status: supported)
Docs: [https://nodejs.org/api/console.html#consolewarndata-args](https://nodejs.org/api/console.html#consolewarndata-args)
Implementation:
- `src/JavaScriptRuntime/Console.cs`
Notes:
Writes to stderr.
Tests:
- `JavaScriptRuntime.Tests.ConsoleTests.Warn_PrintsAllArgumentsWithSpaces` (`tests/Jroc.Tests/ConsoleTests.cs`)

### setTimeout (status: supported)
Docs: [https://nodejs.org/api/timers.html#settimeoutcallback-delay-args](https://nodejs.org/api/timers.html#settimeoutcallback-delay-args)
Implementation:
- `src/JavaScriptRuntime/GlobalThis.cs, src/JavaScriptRuntime/Timers.cs`
Notes:
Schedules a callback to be executed after a specified delay in milliseconds. Returns a timer handle that can be used with clearTimeout.
Tests:
- `Jroc.Tests.Node.ExecutionTests.SetTimeout_ZeroDelay` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
- `Jroc.Tests.Node.ExecutionTests.SetTimeout_MultipleZeroDelay_ExecutedInOrder` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
- `Jroc.Tests.Node.ExecutionTests.SetTimeout_OneSecondDelay` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)

### clearTimeout (status: supported)
Docs: [https://nodejs.org/api/timers.html#cleartimeouttimeout](https://nodejs.org/api/timers.html#cleartimeouttimeout)
Implementation:
- `src/JavaScriptRuntime/GlobalThis.cs, src/JavaScriptRuntime/Timers.cs`
Notes:
Cancels a timer that was previously created with setTimeout. Returns undefined (null in .NET).
Tests:
- `Jroc.Tests.Node.ExecutionTests.ClearTimeout_ZeroDelay` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
- `Jroc.Tests.Node.ExecutionTests.ClearTimeout_MultipleZeroDelay_ClearSecondTimer` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)

### setImmediate (status: supported)
Docs: [https://nodejs.org/api/timers.html#setimmediatecallback-args](https://nodejs.org/api/timers.html#setimmediatecallback-args)
Implementation:
- `src/JavaScriptRuntime/GlobalThis.cs, src/JavaScriptRuntime/Timers.cs, src/JavaScriptRuntime/Engine/NodeSchedulerState.cs, src/JavaScriptRuntime/Engine/NodeEventLoopPump.cs`
Notes:
Schedules a callback to run on the next event loop iteration. Callbacks execute in FIFO order. Nested setImmediate calls run on the next iteration. Returns a handle that can be used with clearImmediate.
Tests:
- `Jroc.Tests.Node.ExecutionTests.SetImmediate_ExecutesCallback` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
- `Jroc.Tests.Node.ExecutionTests.SetImmediate_WithArgs_PassesCorrectly` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
- `Jroc.Tests.Node.ExecutionTests.SetImmediate_Multiple_ExecuteInOrder` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
- `Jroc.Tests.Node.ExecutionTests.SetImmediate_ExecutesBeforeSetTimeout` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
- `Jroc.Tests.Node.ExecutionTests.SetImmediate_Nested_ExecutesInNextIteration` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)

### clearImmediate (status: supported)
Docs: [https://nodejs.org/api/timers.html#clearimmediateimmediate](https://nodejs.org/api/timers.html#clearimmediateimmediate)
Implementation:
- `src/JavaScriptRuntime/GlobalThis.cs, src/JavaScriptRuntime/Timers.cs, src/JavaScriptRuntime/Engine/NodeSchedulerState.cs, src/JavaScriptRuntime/Engine/NodeEventLoopPump.cs`
Notes:
Cancels an immediate that was previously created with setImmediate. Returns undefined (null in .NET).
Tests:
- `Jroc.Tests.Node.ExecutionTests.ClearImmediate_CancelsCallback` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)

### setInterval (status: supported)
Docs: [https://nodejs.org/api/timers.html#setintervalcallback-delay-args](https://nodejs.org/api/timers.html#setintervalcallback-delay-args)
Implementation:
- `src/JavaScriptRuntime/GlobalThis.cs, src/JavaScriptRuntime/Timers.cs, src/JavaScriptRuntime/Engine/NodeSchedulerState.cs, src/JavaScriptRuntime/Engine/NodeEventLoopPump.cs`
Notes:
Schedules a callback to run repeatedly with the specified delay in milliseconds. Returns a handle that can be used with clearInterval. Supports additional arguments passed to the callback.
Tests:
- `Jroc.Tests.Node.ExecutionTests.SetInterval_ExecutesThreeTimes_ThenClears` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)

### clearInterval (status: supported)
Docs: [https://nodejs.org/api/timers.html#clearintervaltimeout](https://nodejs.org/api/timers.html#clearintervaltimeout)
Implementation:
- `src/JavaScriptRuntime/GlobalThis.cs, src/JavaScriptRuntime/Timers.cs, src/JavaScriptRuntime/Engine/NodeSchedulerState.cs, src/JavaScriptRuntime/Engine/NodeEventLoopPump.cs`
Notes:
Cancels a repeating timer that was previously created with setInterval. Returns undefined (null in .NET).
Tests:
- `Jroc.Tests.Node.ExecutionTests.SetInterval_ExecutesThreeTimes_ThenClears` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)

### Promise (status: supported)
Docs: [https://nodejs.org/api/globals.html#promise](https://nodejs.org/api/globals.html#promise)
Implementation:
- `src/JavaScriptRuntime/Promise.cs, src/JavaScriptRuntime/Engine/EngineCore.cs`
Notes:
Promise/A+ compliant implementation with microtask scheduling via IMicrotaskScheduler. Supports constructor, Promise.resolve(), Promise.reject(), then(), catch(), and finally(). Includes proper handling of returned Promises in handlers and chaining semantics.
Tests:
- `Jroc.Tests.Promise.ExecutionTests.Promise_Executor_Resolved` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Executor_Rejected` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Resolve_Then` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Reject_Then` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Resolve_ThenFinally` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Reject_FinallyCatch` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Resolve_FinallyThen` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Resolve_FinallyThrows` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Then_ReturnsResolvedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Then_ReturnsRejectedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Catch_ReturnsResolvedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Catch_ReturnsRejectedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Finally_ReturnsResolvedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Finally_ReturnsRejectedPromise` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)
- `Jroc.Tests.Promise.ExecutionTests.Promise_Scheduling_StarvationTest` (`tests/Jroc.Tests/Promise/ExecutionTests.cs`)


## Limitations

- No Buffer support yet; fs APIs operate on UTF-8 text only.
- CommonJS globals (__dirname/__filename) are supported; require() supports compiled local modules, implemented core modules, and compile-time node_modules discovery across .js/.mjs/.cjs files, package.json type=module entry graphs, package exports/imports condition selection for import/require/node/default, and package.json imports aliases that target either package-local ./... paths or bare package specifiers in the supported deterministic slice. Static import/export declarations and literal import()/require() package requests are resolved at compile time so import and require can target different package entries in one graph, and import.meta.url is available for compiled modules as a deterministic file:// URL. Runtime probing, custom loaders/hooks, nested package-imports aliases, and broader Node loader semantics beyond the documented slice remain unsupported.
- Only a small subset of Node is implemented to support tests; many APIs are unimplemented.
