<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.12: CleanupFinalizationRegistry ( finalizationRegistry )

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-10T00:19:15Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.12 | CleanupFinalizationRegistry ( finalizationRegistry ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#CleanupFinalizationRegistry) |

## Support

Feature-level support tracking with test script references.

### 9.12 ([tc39.es](https://tc39.es/ecma262/#CleanupFinalizationRegistry))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| CleanupFinalizationRegistry ( finalizationRegistry ) | Supported with Limitations | [`FinalizationRegistry_Cleanup_Order.js`](../../../Js2IL.Tests/FinalizationRegistry/JavaScript/FinalizationRegistry_Cleanup_Order.js)<br>[`FinalizationRegistry_Unregister_Basic.js`](../../../Js2IL.Tests/FinalizationRegistry/JavaScript/FinalizationRegistry_Unregister_Basic.js) | Collected registrations are converted into host cleanup jobs and invoke the cleanup callback with held values when drained. Timing remains host/.NET-GC dependent unless tests explicitly force collection with a host-opt-in non-standard global `gc()` helper. |

