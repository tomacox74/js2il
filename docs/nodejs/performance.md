# Global: performance

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 24.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/docs/latest-v24.x/api/globals.html#performance) |

## Implementation

- `src/JavaScriptRuntime/GlobalThis.cs`
- `src/JavaScriptRuntime/Node/PerfHooks.cs`

## Notes

The realm-owned global is identical to require('node:perf_hooks').performance.

## Tests

- `Jroc.Tests.Node.ExecutionTests.PerfHooks_GlobalPerformanceIdentity` (`tests/Jroc.Tests/Node/ExecutionTests.cs`)
- `Jroc.NodeContracts.Tests.PerfHooksModuleContractTests.IntrinsicPerfHooksModule_DelegatesPerformanceNowThroughTheTypedContract` (`tests/Jroc.NodeContracts.Tests/PerfHooksModuleContractTests.cs`)
