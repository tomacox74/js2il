<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.6: Agents

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.6 | Agents | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-agents) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 9.6.1 | AgentSignifier ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-agentsignifier) |
| 9.6.2 | AgentCanSuspend ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-agentcansuspend) |
| 9.6.3 | IncrementModuleAsyncEvaluationCount ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-IncrementModuleAsyncEvaluationCount) |

## Support

Feature-level support tracking with test script references.

### 9.6.2 ([tc39.es](https://tc39.es/ecma262/#sec-agentcansuspend))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| AgentCanSuspend behavior in host event loop | Supported with Limitations | [`Hosting_EventLoopKeepAlive.js`](../../../Js2IL.Tests/Hosting/JavaScript/Hosting_EventLoopKeepAlive.js)<br>[`Process_NextTick_And_Promise_Ordering.js`](../../../Js2IL.Tests/Node/Process/JavaScript/Process_NextTick_And_Promise_Ordering.js) | Current runtime can suspend/resume work via event-loop pumping, but only for the implemented single-agent host model. |

### 9.6.3 ([tc39.es](https://tc39.es/ecma262/#sec-IncrementModuleAsyncEvaluationCount))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IncrementModuleAsyncEvaluationCount | Not Yet Supported | `Js2IL.Tests/ValidatorTests.cs` | Depends on ES module async evaluation machinery, which is not implemented. |

