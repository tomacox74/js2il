# Module: process

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/process.html) |

## Implementation

- `JavaScriptRuntime/Node/Process.cs`
- `JavaScriptRuntime/GlobalVariables.cs`

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| argv | property | supported | [docs](https://nodejs.org/api/process.html#processargv) |
| exit() | function | supported | [docs](https://nodejs.org/api/process.html#processexitcode) |
| exit(code) | function | supported | [docs](https://nodejs.org/api/process.html#process_exit_code) |
| exitCode | property | supported | [docs](https://nodejs.org/api/process.html#processexitcode) |
| platform | property | supported | [docs](https://nodejs.org/api/process.html#processplatform) |
| versions.node | property | supported | [docs](https://nodejs.org/api/process.html#processversions) |
| env | property | supported | [docs](https://nodejs.org/api/process.html#processenv) |
| chdir(directory) | function | supported | [docs](https://nodejs.org/api/process.html#processchdirdirectory) |
| nextTick(callback, ...args) | function | supported | [docs](https://nodejs.org/api/process.html#processnexttickcallback-args) |

## API Details

### argv

argv[0] normalized to current script filename; extra host args trimmed in tests.

**Tests:**
- `Js2IL.Tests.Node.ExecutionTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/ExecutionTests.cs#L21`)
- `Js2IL.Tests.Node.GeneratorTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/GeneratorTests.cs`)

### exit()

Sets the current exitCode on the host environment abstraction without terminating the test host.

**Tests:**
- `Js2IL.Tests.Node.ProcessAdditionalTests.Process_Exit_Uses_Current_ExitCode` (`Js2IL.Tests/Node/ProcessAdditionalTests.cs`)

### exit(code)

Coerces the code to int, sets it on the environment abstraction, and does not terminate the test host.

**Tests:**
- `Js2IL.Tests.Node.ProcessAdditionalTests.Process_Exit_Code_Sets_ExitCode` (`Js2IL.Tests/Node/ProcessAdditionalTests.cs`)

### exitCode

**Tests:**
- `Js2IL.Tests.Node.Process.ProcessExitCodeTests.Process_exitCode_getter_setter_mirrors_Environment` (`Js2IL.Tests/Node/Process/ProcessExitCodeTests.cs`)

### platform

Returns a Node-compatible platform identifier: win32, linux, darwin, or unknown.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Platform_Versions_And_Env_Basics` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)

### versions.node

Exposes a minimal process.versions object with node version identity.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Platform_Versions_And_Env_Basics` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)

### env

Returns a cached snapshot object of host environment variables as string values for the current runtime instance. Values are exposed as-is from the host process environment.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Platform_Versions_And_Env_Basics` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)

### chdir(directory)

Changes the current working directory for the running process.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Chdir_And_NextTick_Basics` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)

### nextTick(callback, ...args)

Queues a callback for next-turn execution using the immediate queue. This is an approximation and does not implement full Node nextTick queue semantics.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Chdir_And_NextTick_Basics` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)
