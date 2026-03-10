<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.9: Processing Model of WeakRef and FinalizationRegistry Targets

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-10T00:19:15Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.9 | Processing Model of WeakRef and FinalizationRegistry Targets | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakref-processing-model) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 9.9.1 | Objectives | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakref-invariants) |
| 9.9.2 | Liveness | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-liveness) |
| 9.9.3 | Execution | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakref-execution) |
| 9.9.4 | Host Hooks | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakref-host-hooks) |
| 9.9.4.1 | HostEnqueueFinalizationRegistryCleanupJob ( finalizationRegistry ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-host-cleanup-finalization-registry) |

## Support

Feature-level support tracking with test script references.

### 9.9 ([tc39.es](https://tc39.es/ecma262/#sec-weakref-processing-model))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| WeakRef/FinalizationRegistry processing model | Supported with Limitations | [`WeakRef_Deref_KeptObjects.js`](../../../Js2IL.Tests/WeakRef/JavaScript/WeakRef_Deref_KeptObjects.js)<br>[`FinalizationRegistry_Cleanup_Order.js`](../../../Js2IL.Tests/FinalizationRegistry/JavaScript/FinalizationRegistry_Cleanup_Order.js) | Implements a host-managed WeakRef/FinalizationRegistry processing model with kept objects, a cleanup-job queue, and deterministic test forcing via a host-opt-in non-standard global `gc()` helper. Cleanup timing outside that helper still depends on .NET GC and active event-loop checkpoints, so the model remains host-safe rather than fully browser/Node equivalent. |

