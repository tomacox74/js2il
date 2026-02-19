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
