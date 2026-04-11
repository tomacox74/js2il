<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.10: ClearKeptObjects ( )

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-09T23:42:35Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.10 | ClearKeptObjects ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#ClearKeptObjects) |

## Support

Feature-level support tracking with test script references.

### 9.10 ([tc39.es](https://tc39.es/ecma262/#ClearKeptObjects))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ClearKeptObjects ( ) | Supported with Limitations | [`WeakRef_Deref_KeptObjects.js`](../../../tests/Js2IL.Tests/WeakRef/JavaScript/WeakRef_Deref_KeptObjects.js) | The finalization host clears kept objects at cleanup checkpoints around event-loop iterations. This is sufficient for the current WeakRef baseline and deterministic tests, but it is still a single-runtime approximation rather than a full multi-realm host model. |

