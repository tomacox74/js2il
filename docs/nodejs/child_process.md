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

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| spawnSync(command, args, options) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processspawnsynccommand-args-options) |
| execSync(command, options) | function | supported | [docs](https://nodejs.org/api/child_process.html#child_processexecsynccommand-options) |

## API Details

### spawnSync(command, args, options)

Supports options: cwd, stdio ('inherit'), shell. Returns { status, stdout, stderr }.

### execSync(command, options)

Supports options: cwd, stdio ('inherit'), encoding ('utf8'). Throws an Error-like object with stdout/stderr on non-zero exit.
