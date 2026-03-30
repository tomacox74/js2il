<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 28.2: Proxy Objects

[Back to Section28](Section28.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-29T08:04:15Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 28.2 | Proxy Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 28.2.1 | The Proxy Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-constructor) |
| 28.2.1.1 | Proxy ( target , handler ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-target-handler) |
| 28.2.2 | Properties of the Proxy Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-proxy-constructor) |
| 28.2.2.1 | Proxy.revocable ( target , handler ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy.revocable) |

## Support

Feature-level support tracking with test script references.

### 28.2.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-target-handler))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Proxy (target, handler) (constructor) | Supported with Limitations | [`Proxy_GetTrap_OverridesProperty.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_GetTrap_OverridesProperty.js)<br>[`Proxy_SetTrap_InterceptsWrites.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_SetTrap_InterceptsWrites.js)<br>[`Proxy_HasTrap_AffectsInOperator.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_HasTrap_AffectsInOperator.js)<br>[`Proxy_DeletePropertyTrap_And_Fallback.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_DeletePropertyTrap_And_Fallback.js)<br>[`Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js)<br>[`Proxy_ApplyAndConstructTraps_WithFallback.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_ApplyAndConstructTraps_WithFallback.js)<br>[`Proxy_Revocable_ThrowsAfterRevoke.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_Revocable_ThrowsAfterRevoke.js)<br>[`Proxy_Validation_EdgeCases.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_Validation_EdgeCases.js) | Supports creating Proxy instances via new Proxy(target, handler) with object target/handler validation, get/set/has/deleteProperty/ownKeys/apply/construct/getPrototypeOf/setPrototypeOf routing, and fallback-to-target behavior when a trap is absent. The supported trap surface now also enforces callable/constructible target gating plus basic getPrototypeOf/ownKeys/construct result validation. Full spec invariants plus descriptor-oriented traps (e.g., getOwnPropertyDescriptor, defineProperty) are still missing. |

### 28.2.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-proxy.revocable))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Proxy.revocable | Supported with Limitations | [`Proxy_Revocable_ThrowsAfterRevoke.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_Revocable_ThrowsAfterRevoke.js)<br>[`Proxy_Validation_EdgeCases.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_Validation_EdgeCases.js) | Proxy.revocable(target, handler) validates object target/handler values, returns { proxy, revoke }, and makes subsequent property access, deletion, ownKeys, prototype operations, calls, and construction throw TypeError once revoked. |

