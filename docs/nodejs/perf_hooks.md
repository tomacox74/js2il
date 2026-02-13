# Module: perf_hooks

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/perf_hooks.html) |

## Implementation

- `JavaScriptRuntime/Node/PerfHooks.cs`

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| performance | property | supported | [docs](https://nodejs.org/api/perf_hooks.html#performance) |
| performance.now() | function | supported | [docs](https://nodejs.org/api/perf_hooks.html#performancenow) |

## API Details

### performance.now()

**Tests:**
- `Js2IL.Tests.Node.ExecutionTests.PerfHooks_PerformanceNow_Basic` (`Js2IL.Tests/Node/ExecutionTests.cs`)
- `Js2IL.Tests.Node.GeneratorTests.PerfHooks_PerformanceNow_Basic` (`Js2IL.Tests/Node/GeneratorTests.cs`)
