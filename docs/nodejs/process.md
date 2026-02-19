# Module: process

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | completed |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/process.html) |

## Implementation

- `JavaScriptRuntime/Node/Process.cs`
- `JavaScriptRuntime/GlobalThis.cs`
- `JavaScriptRuntime/Engine/NodeSchedulerState.cs`
- `JavaScriptRuntime/Engine/NodeEventLoopPump.cs`

## APIs

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
| versions.js2il | property | supported | [docs](https://nodejs.org/api/process.html#processversions) |
| versions.dotnet | property | supported | [docs](https://nodejs.org/api/process.html#processversions) |
| env | property | supported | [docs](https://nodejs.org/api/process.html#processenv) |
| chdir(directory) | function | supported | [docs](https://nodejs.org/api/process.html#processchdirdirectory) |
| cwd() | function | supported | [docs](https://nodejs.org/api/process.html#processcwd) |
| nextTick(callback, ...args) | function | supported | [docs](https://nodejs.org/api/process.html#processnexttickcallback-args) |

## API Details

### argv

argv[0] normalized to current script filename; extra host args trimmed in tests.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Process.GeneratorTests.Environment_EnumerateProcessArgV` (`Js2IL.Tests/Node/Process/GeneratorTests.cs`)

### exit()

Sets the current exitCode on the host environment abstraction without terminating the test host.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Exit_Uses_Current_ExitCode` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)

### exit(code)

Coerces the code to int, sets it on the environment abstraction, and does not terminate the test host.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Exit_Code_Sets_ExitCode` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)

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

### versions.v8

V8 version string compatible with Node.js 22.x for compatibility checks.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Versions_Expanded` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)

### versions.modules

Node modules ABI version (127) for Node.js 22.x compatibility.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Versions_Expanded` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)

### versions.js2il

JavaScriptRuntime assembly version exposed by JS2IL runtime. JS2IL-specific extension to process.versions.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Versions_Expanded` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)

### versions.dotnet

.NET runtime version description. JS2IL-specific extension to process.versions.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Versions_Expanded` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)
### env

Returns a cached snapshot object of host environment variables as string values for the current runtime instance. Values are exposed as-is from the host process environment.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Platform_Versions_And_Env_Basics` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)

### chdir(directory)

Changes the current working directory for the running process.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Chdir_And_NextTick_Basics` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)

### cwd()

Returns the current working directory of the Node.js process.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Chdir_And_NextTick_Basics` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)
### nextTick(callback, ...args)

Queues a callback into a dedicated nextTick queue with priority over Promise microtasks and immediates at callback checkpoints.

**Tests:**
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_Chdir_And_NextTick_Basics` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_NextTick_Precedes_SetImmediate_When_Queued_Later` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)
- `Js2IL.Tests.Node.Process.ExecutionTests.Process_NextTick_And_Promise_Ordering` (`Js2IL.Tests/Node/Process/ExecutionTests.cs`)
