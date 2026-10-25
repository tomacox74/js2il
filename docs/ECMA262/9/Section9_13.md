<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.13: CanBeHeldWeakly ( v )

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-25T02:53:22Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.13 | CanBeHeldWeakly ( v ) | Supported | [tc39.es](https://tc39.es/ecma262/#CanBeHeldWeakly) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 9.13 ([tc39.es](https://tc39.es/ecma262/#CanBeHeldWeakly))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| CanBeHeldWeakly ( v ) | Supported | [`WeakRef_Deref_KeptObjects.js`](../../../tests/Jroc.Tests/WeakRef/JavaScript/WeakRef_Deref_KeptObjects.js)<br>[`FinalizationRegistry_Unregister_Basic.js`](../../../tests/Jroc.Tests/FinalizationRegistry/JavaScript/FinalizationRegistry_Unregister_Basic.js) | `test/built-ins/WeakRef/throws-when-target-cannot-be-held-weakly.js` | TypeUtilities.CanBeHeldWeakly accepts objects and non-registered symbols, including well-known symbols, and rejects registered Symbol.for values, null, undefined and other primitives. Symbol registration is checked against the current agent's global symbol registry. WeakMap, WeakSet, WeakRef and FinalizationRegistry share this predicate; the separate collection/cleanup timing limitations do not change this eligibility decision. |

