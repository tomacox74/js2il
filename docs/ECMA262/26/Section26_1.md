<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 26.1: WeakRef Objects

[Back to Section26](Section26.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-10T00:19:15Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 26.1 | WeakRef Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 26.1.1 | The WeakRef Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref-constructor) |
| 26.1.1.1 | WeakRef ( target ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref-target) |
| 26.1.2 | Properties of the WeakRef Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weak-ref-constructor) |
| 26.1.2.1 | WeakRef.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref.prototype) |
| 26.1.3 | Properties of the WeakRef Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weak-ref-prototype-object) |
| 26.1.3.1 | WeakRef.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref.prototype.constructor) |
| 26.1.3.2 | WeakRef.prototype.deref ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref.prototype.deref) |
| 26.1.3.3 | WeakRef.prototype [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref.prototype-%symbol.tostringtag%) |
| 26.1.4 | WeakRef Abstract Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakref-abstract-operations) |
| 26.1.4.1 | WeakRefDeref ( weakRef ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakrefderef) |
| 26.1.5 | Properties of WeakRef Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-weak-ref-instances) |

## Support

Feature-level support tracking with test script references.

### 26.1 ([tc39.es](https://tc39.es/ecma262/#sec-weak-ref-objects))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| WeakRef constructor, deref(), and kept-object baseline | Supported with Limitations | [`WeakRef_Deref_KeptObjects.js`](../../../Js2IL.Tests/WeakRef/JavaScript/WeakRef_Deref_KeptObjects.js) | Supports `new WeakRef(target)` in construct positions, `deref()`, and `%Symbol.toStringTag%` via instance-level descriptor wiring. `deref()` adds live targets to a host-kept set until the next cleanup checkpoint, and tests use a host-opt-in non-standard global `gc()` helper to force deterministic collection. js2il does not yet expose a full first-class `WeakRef` constructor/prototype object on `globalThis`. |

