<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 26.2: FinalizationRegistry Objects

[Back to Section26](Section26.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-10T00:19:15Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 26.2 | FinalizationRegistry Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 26.2.1 | The FinalizationRegistry Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry-constructor) |
| 26.2.1.1 | FinalizationRegistry ( cleanupCallback ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry-cleanup-callback) |
| 26.2.2 | Properties of the FinalizationRegistry Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-finalization-registry-constructor) |
| 26.2.2.1 | FinalizationRegistry.prototype | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry.prototype) |
| 26.2.3 | Properties of the FinalizationRegistry Prototype Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-finalization-registry-prototype-object) |
| 26.2.3.1 | FinalizationRegistry.prototype.constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry.prototype.constructor) |
| 26.2.3.2 | FinalizationRegistry.prototype.register ( target , heldValue [ , unregisterToken ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry.prototype.register) |
| 26.2.3.3 | FinalizationRegistry.prototype.unregister ( unregisterToken ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry.prototype.unregister) |
| 26.2.3.4 | FinalizationRegistry.prototype [ %Symbol.toStringTag% ] | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry.prototype-%symbol.tostringtag%) |
| 26.2.4 | Properties of FinalizationRegistry Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-finalization-registry-instances) |

## Support

Feature-level support tracking with test script references.

### 26.2 ([tc39.es](https://tc39.es/ecma262/#sec-finalization-registry-objects))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| FinalizationRegistry constructor, register/unregister, and cleanup callback baseline | Supported with Limitations | [`FinalizationRegistry_Cleanup_Order.js`](../../../Js2IL.Tests/FinalizationRegistry/JavaScript/FinalizationRegistry_Cleanup_Order.js)<br>[`FinalizationRegistry_Unregister_Basic.js`](../../../Js2IL.Tests/FinalizationRegistry/JavaScript/FinalizationRegistry_Unregister_Basic.js) | Supports `new FinalizationRegistry(cleanupCallback)` in construct positions plus `register(target, heldValue, [unregisterToken])`, `unregister(token)`, and `%Symbol.toStringTag%`. Cleanup callbacks are queued through a host-managed finalization queue and become deterministic in tests when a host-opt-in non-standard global `gc()` helper forces collection. js2il does not yet expose a full first-class `FinalizationRegistry` constructor/prototype object on `globalThis`, and cleanup timing otherwise depends on .NET GC plus event-loop checkpoints. |

