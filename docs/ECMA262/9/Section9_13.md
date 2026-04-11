<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.13: CanBeHeldWeakly ( v )

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-09T23:42:36Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.13 | CanBeHeldWeakly ( v ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#CanBeHeldWeakly) |

## Support

Feature-level support tracking with test script references.

### 9.13 ([tc39.es](https://tc39.es/ecma262/#CanBeHeldWeakly))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| CanBeHeldWeakly ( v ) | Supported with Limitations | [`WeakRef_Deref_KeptObjects.js`](../../../tests/Js2IL.Tests/WeakRef/JavaScript/WeakRef_Deref_KeptObjects.js)<br>[`FinalizationRegistry_Unregister_Basic.js`](../../../tests/Js2IL.Tests/FinalizationRegistry/JavaScript/FinalizationRegistry_Unregister_Basic.js) | js2il approximates CanBeHeldWeakly as any non-null reference type (including functions and symbols) and rejects null/undefined/string/value-type primitives. This is sufficient for the current WeakRef/FinalizationRegistry baseline but does not yet model the full ECMAScript symbol-registry distinction. |

