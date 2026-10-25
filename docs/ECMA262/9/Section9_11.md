<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.11: AddToKeptObjects ( value )

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-25T02:53:22Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.11 | AddToKeptObjects ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#AddToKeptObjects) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 9.11 ([tc39.es](https://tc39.es/ecma262/#AddToKeptObjects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| AddToKeptObjects ( value ) | Supported with Limitations | [`WeakRef_Deref_KeptObjects.js`](../../../tests/Jroc.Tests/WeakRef/JavaScript/WeakRef_Deref_KeptObjects.js)<br>`tests/Jroc.Tests/RuntimeAgentSchedulingTests.cs` |  | WeakRef.deref() adds a live target to the agent-owned FinalizationRegistryHost kept-object set so it survives same-job host-opt-in gc() forcing until a cleanup checkpoint. The kept set is shared by realms in that agent, not process-global; collection timing remains host/.NET-GC dependent. |

