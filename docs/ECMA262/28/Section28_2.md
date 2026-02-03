<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 28.2: Proxy Objects

[Back to Section28](Section28.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 28.2 | Proxy Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 28.2.1 | The Proxy Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-constructor) |
| 28.2.1.1 | Proxy ( target , handler ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-target-handler) |
| 28.2.2 | Properties of the Proxy Constructor | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-proxy-constructor) |
| 28.2.2.1 | Proxy.revocable ( target , handler ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy.revocable) |

## Support

Feature-level support tracking with test script references.

### 28.2.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-target-handler))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Proxy (target, handler) (constructor) | Supported with Limitations | [`Proxy_GetTrap_OverridesProperty.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_GetTrap_OverridesProperty.js)<br>[`Proxy_SetTrap_InterceptsWrites.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_SetTrap_InterceptsWrites.js)<br>[`Proxy_HasTrap_AffectsInOperator.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_HasTrap_AffectsInOperator.js) | Supports creating Proxy instances via new Proxy(target, handler) with get/set/has trap routing for core property access and the in operator. Does not implement full spec invariants, revocation, or other traps (e.g., ownKeys, getOwnPropertyDescriptor, defineProperty, deleteProperty, apply, construct). |

### 28.2.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-proxy.revocable))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Proxy.revocable | Not Yet Supported |  |  |

