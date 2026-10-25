<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.10: ClearKeptObjects ( )

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-25T02:53:22Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.10 | ClearKeptObjects ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#ClearKeptObjects) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 9.10 ([tc39.es](https://tc39.es/ecma262/#ClearKeptObjects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ClearKeptObjects ( ) | Supported with Limitations | [`WeakRef_Deref_KeptObjects.js`](../../../tests/Jroc.Tests/WeakRef/JavaScript/WeakRef_Deref_KeptObjects.js)<br>`tests/Jroc.Tests/RuntimeAgentSchedulingTests.cs` |  | The agent-owned FinalizationRegistryHost clears its kept-object set at event-loop checkpoints. Realms in one agent share that host, while other agents have independent hosts. Collection and cleanup scheduling remain dependent on .NET GC and the implemented host checkpoints. |

