# Module: child_process

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/child_process.html) |

## Implementation

- `JavaScriptRuntime/Node/ChildProcess.cs`

## Notes

Provides synchronous process execution plus a minimal async ChildProcess surface with piped stdout/stderr capture, exit/close events, callback completion for exec/execFile, and kill() on the returned handle.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| spawn(command[, args][, options]) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processspawncommand-args-options) |
| exec(command[, options][, callback]) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processexeccommand-options-callback) |
| execFile(file[, args][, options][, callback]) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processexecfilefile-args-options-callback) |
| spawnSync(command, args, options) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processspawnsynccommand-args-options) |
| execSync(command, options) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processexecsynccommand-options) |

## API Details

### spawn(command[, args][, options])

Returns an EventEmitter-backed child handle with pid, stdout/stderr Readable pipes, a Writable stdin pipe when piped, exit/close events, and kill(). Supports cwd, shell, and stdio ('pipe'/'inherit' plus basic first-three-entry array handling).

**Tests:**
- `Js2IL.Tests.Node.ChildProcess.ExecutionTests.Require_ChildProcess_Spawn_Basic` (`Js2IL.Tests/Node/ChildProcess/ExecutionTests.cs`)
- `Js2IL.Tests.Node.ChildProcess.GeneratorTests.Require_ChildProcess_Spawn_Basic` (`Js2IL.Tests/Node/ChildProcess/GeneratorTests.cs`)

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

### spawnSync(command, args, options)

Supports cwd, shell, and stdio ('pipe'/'inherit' plus basic first-three-entry array handling). Returns { status, stdout, stderr }.

### execSync(command, options)

Supports cwd, stdio ('pipe'/'inherit' plus basic first-three-entry array handling), and encoding ('utf8'). Throws an Error-like object with status/code/stdout/stderr on non-zero exit.
