# Module: child_process

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/child_process.html) |

## Implementation

- `src/JavaScriptRuntime/Node/ChildProcess.cs`

## Notes

Provides synchronous process execution, async spawn/exec/execFile, and a documented fork baseline for compiled child modules in the current assembly. The supported slice includes authenticated JSON-only parent/child IPC over loopback (`child.on('message')`, `child.send(...)`, `process.on('message')`, `process.send(...)`), environment overlays, and basic signal/kill reporting with explicit diagnostics for unsupported detached, advanced serialization, and non-IPC stdio modes. Hosted `JsEngine` runtimes can also use `fork()` when the host supplies a launchable compiled assembly path (for example via `JsModuleLoadOptions.CompiledAssemblyPath`), and may override process creation through `IChildProcessLauncher`.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| spawn(command[, args][, options]) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processspawncommand-args-options) |
| exec(command[, options][, callback]) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processexeccommand-options-callback) |
| execFile(file[, args][, options][, callback]) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processexecfilefile-args-options-callback) |
| fork(modulePath[, args][, options]) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processforkmodulepath-args-options) |
| spawnSync(command, args, options) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processspawnsynccommand-args-options) |
| execSync(command, options) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processexecsynccommand-options) |

## API Details

### spawn(command[, args][, options])

Returns an EventEmitter-backed child handle with pid, stdout/stderr Readable pipes, a Writable stdin pipe when piped, exit/close events, and kill(). Supports cwd, shell, and stdio ('pipe'/'inherit'/'ignore' plus basic first-three-entry array handling).

**Tests:**
- `Js2IL.Tests.Node.ChildProcess.ExecutionTests.Require_ChildProcess_Spawn_Basic` (`Js2IL.Tests/Node/ChildProcess/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ChildProcess.ExecutionTests.Require_ChildProcess_Spawn_Ignore` (`Js2IL.Tests/Node/ChildProcess/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ChildProcess.GeneratorTests.Require_ChildProcess_Spawn_Basic` (`Js2IL.Tests/Node/ChildProcess/GeneratorTests.cs`)
- `Js2IL.Tests.Node.ChildProcess.GeneratorTests.Require_ChildProcess_Spawn_Ignore` (`Js2IL.Tests/Node/ChildProcess/GeneratorTests.cs`)

### exec(command[, options][, callback])

Executes via a shell and optionally invokes an error-first callback with (err, stdout, stderr). Returns the child handle immediately. Non-zero exit codes surface an Error-like object carrying status/code/stdout/stderr.

**Tests:**
- `Js2IL.Tests.Node.ChildProcess.ExecutionTests.Require_ChildProcess_Exec_Callback` (`Js2IL.Tests/Node/ChildProcess/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ChildProcess.GeneratorTests.Require_ChildProcess_Exec_Callback` (`Js2IL.Tests/Node/ChildProcess/GeneratorTests.cs`)

### execFile(file[, args][, options][, callback])

Runs a file directly without an implicit shell and optionally invokes an error-first callback with (err, stdout, stderr). Non-zero exits surface the same Error-like callback shape used by exec().

**Tests:**
- `Js2IL.Tests.Node.ChildProcess.ExecutionTests.Require_ChildProcess_ExecFile_NonZero` (`Js2IL.Tests/Node/ChildProcess/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ChildProcess.GeneratorTests.Require_ChildProcess_ExecFile_NonZero` (`Js2IL.Tests/Node/ChildProcess/GeneratorTests.cs`)

### fork(modulePath[, args][, options])

Launches another compiled JS2IL child from the current assembly, resolves `modulePath` relative to the compiled program entry module, and enables an authenticated JSON-only IPC channel by default. Supports `cwd`, merged `env` overrides, stdio values `'pipe'`, `'inherit'`, `'ignore'`, plus `'ipc'` at `stdio[3]`, `child.send(...)`, `child.on('message')`, `process.send(...)`, `process.on('message')`, deterministic `disconnect` before `exit`/`close`, and `kill('SIGTERM'|'SIGKILL'|'SIGINT')` reporting. In hosted `JsEngine` scenarios, the child is launched from the compiled assembly path supplied by the host (for example `JsModuleLoadOptions.CompiledAssemblyPath`); if no launchable assembly path is available, `fork()` throws an explicit runtime error. Hosts can customize process creation through `IChildProcessLauncher`. Detached children, advanced serialization, handle passing, and Node-internal IPC behaviors remain unsupported.

**Tests:**
- `Js2IL.Tests.Node.ChildProcess.ExecutionTests.Require_ChildProcess_Fork_MessagePassing` (`Js2IL.Tests/Node/ChildProcess/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ChildProcess.ExecutionTests.Require_ChildProcess_Fork_Kill_And_Env` (`Js2IL.Tests/Node/ChildProcess/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ChildProcess.ExecutionTests.Require_ChildProcess_Fork_Unsupported_Options` (`Js2IL.Tests/Node/ChildProcess/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ChildProcess.GeneratorTests.Require_ChildProcess_Fork_MessagePassing` (`Js2IL.Tests/Node/ChildProcess/GeneratorTests.cs`)
- `Js2IL.Tests.Node.ChildProcess.GeneratorTests.Require_ChildProcess_Fork_Kill_And_Env` (`Js2IL.Tests/Node/ChildProcess/GeneratorTests.cs`)
- `Js2IL.Tests.Node.ChildProcess.GeneratorTests.Require_ChildProcess_Fork_Unsupported_Options` (`Js2IL.Tests/Node/ChildProcess/GeneratorTests.cs`)

### spawnSync(command, args, options)

Supports cwd, shell, environment overrides, and stdio ('pipe'/'inherit'/'ignore' for the first three slots). Returns { status, stdout, stderr }.

### execSync(command, options)

Supports cwd, environment overrides, stdio ('pipe'/'inherit'/'ignore' for the first three slots), and encoding ('utf8'). Throws an Error-like object with status/code/stdout/stderr on non-zero exit.
