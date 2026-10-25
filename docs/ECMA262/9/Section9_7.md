<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.7: Agent Clusters

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-25T02:53:22Z

RuntimeAgentCluster owns host-created agents and their explicitly shared transport, broadcast, shared-memory and Atomics coordination services. Independent clusters isolate those services.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.7 | Agent Clusters | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-agent-clusters) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 9.7 ([tc39.es](https://tc39.es/ecma262/#sec-agent-clusters))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Cluster-shared services for host-created agents | Supported with Limitations | `tests/Jroc.Tests/RuntimeOwnershipTests.cs`<br>`tests/Jroc.Tests/RuntimeAgentClusterSharedServicesTests.cs` |  | Agents in one cluster share opaque message queues, broadcast registries, SharedArrayBuffer backing stores and Atomics waiter coordination. Realms retain their own JavaScript wrappers, while symbol registries and scheduling belong to individual agents. Cross-cluster backing-store access is rejected and agent disposal releases endpoints and waiters. These host services do not implement a complete JavaScript worker_threads or structured-clone surface. |

