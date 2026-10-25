<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.6: Agents

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-25T02:53:21Z

RuntimeAgent owns scheduling, asynchronous context and the global symbol registry and may own multiple realms. Host-created agents can run independently and share narrow cluster services. The section remains Incomplete because the agent-wide asynchronous module evaluation order counter is not implemented.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.6 | Agents | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-agents) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 9.6.1 | AgentSignifier ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-agentsignifier) |
| 9.6.2 | AgentCanSuspend ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-agentcansuspend) |
| 9.6.3 | IncrementModuleAsyncEvaluationCount ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-IncrementModuleAsyncEvaluationCount) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 9.6.1 ([tc39.es](https://tc39.es/ecma262/#sec-agentsignifier))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Opaque identity for host-created runtime agents | Supported with Limitations | `tests/Jroc.Tests/RuntimeOwnershipTests.cs`<br>`tests/Jroc.Tests/RuntimeAgentClusterSharedServicesTests.cs` |  | Distinct RuntimeAgent instances supply stable opaque ownership identities for realms, schedulers, symbol registries, shared-memory access and Atomics waiters. This is an internal host ownership model, not a public JavaScript agent or worker API. |

### 9.6.2 ([tc39.es](https://tc39.es/ecma262/#sec-agentcansuspend))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| AgentCanSuspend behavior in host event loop | Supported with Limitations | `tests/Jroc.Tests/RuntimeAgentSchedulingTests.cs`<br>`tests/Jroc.Tests/AtomicsWaitAsyncRuntimeTests.cs` |  | The host implements blocking Atomics.wait, asynchronous waits, independent per-agent event-loop scheduling and cooperative shutdown. Agents can notify waiters in the same cluster. A configurable ECMAScript agent suspension-policy slot is not modeled. |

### 9.6.3 ([tc39.es](https://tc39.es/ecma262/#sec-IncrementModuleAsyncEvaluationCount))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IncrementModuleAsyncEvaluationCount | Not Yet Supported | `tests/Jroc.Tests/Import/NativeEsmOverheadTests.cs` |  | Native ESM already supports top-level await, promise-backed dependency completion and cached evaluation failures. Require.EsModuleEvaluation uses module-local pending-dependency counts, not the agent-wide monotonic asynchronous evaluation order counter specified here. That operation and full general asynchronous cyclic-module ordering remain unimplemented; the referenced tests cover supported async graphs, not this missing counter. |

