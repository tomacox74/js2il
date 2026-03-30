<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.11: AddToKeptObjects ( value )

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-09T23:42:36Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.11 | AddToKeptObjects ( value ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#AddToKeptObjects) |

## Support

Feature-level support tracking with test script references.

### 9.11 ([tc39.es](https://tc39.es/ecma262/#AddToKeptObjects))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| AddToKeptObjects ( value ) | Supported with Limitations | [`WeakRef_Deref_KeptObjects.js`](../../../Js2IL.Tests/WeakRef/JavaScript/WeakRef_Deref_KeptObjects.js) | WeakRef.deref() adds live targets to the host-kept set so they survive same-job `gc()` forcing until the next cleanup checkpoint. The approximation is scoped to the current js2il runtime/event loop. |

