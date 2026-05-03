<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 28.2: Proxy Objects

[Back to Section28](Section28.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-03T15:15:04Z

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

Feature-level support tracking with repo test references and optional test262 evidence.

### 28.2.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-target-handler))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Proxy (target, handler) (constructor) | Supported with Limitations | [`Proxy_GetTrap_OverridesProperty.js`](../../../tests/Js2IL.Tests/Proxy/JavaScript/Proxy_GetTrap_OverridesProperty.js)<br>[`Proxy_SetTrap_InterceptsWrites.js`](../../../tests/Js2IL.Tests/Proxy/JavaScript/Proxy_SetTrap_InterceptsWrites.js)<br>[`Proxy_HasTrap_AffectsInOperator.js`](../../../tests/Js2IL.Tests/Proxy/JavaScript/Proxy_HasTrap_AffectsInOperator.js)<br>[`Proxy_DeletePropertyTrap_And_Fallback.js`](../../../tests/Js2IL.Tests/Proxy/JavaScript/Proxy_DeletePropertyTrap_And_Fallback.js)<br>[`Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js`](../../../tests/Js2IL.Tests/Proxy/JavaScript/Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js)<br>[`Proxy_ApplyAndConstructTraps_WithFallback.js`](../../../tests/Js2IL.Tests/Proxy/JavaScript/Proxy_ApplyAndConstructTraps_WithFallback.js)<br>[`Proxy_Revocable_ThrowsAfterRevoke.js`](../../../tests/Js2IL.Tests/Proxy/JavaScript/Proxy_Revocable_ThrowsAfterRevoke.js)<br>[`Proxy_Validation_EdgeCases.js`](../../../tests/Js2IL.Tests/Proxy/JavaScript/Proxy_Validation_EdgeCases.js) |  | Supports creating Proxy instances via new Proxy(target, handler) with object target/handler validation. Currently implemented traps are get, set, has, deleteProperty, ownKeys, getPrototypeOf, setPrototypeOf, apply, and construct, each with fallback-to-target behavior when the trap is absent. JS2IL also enforces basic validations for the implemented trap surface (callable/constructible target gating, ownKeys object/entry validation, getPrototypeOf object-or-null results, setPrototypeOf truthy return, and construct object results), but full ECMAScript proxy invariants and descriptor-oriented traps such as getOwnPropertyDescriptor, defineProperty, isExtensible, and preventExtensions are still missing. |

### 28.2.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-proxy.revocable))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Proxy.revocable | Supported with Limitations | [`Proxy_Revocable_ThrowsAfterRevoke.js`](../../../tests/Js2IL.Tests/Proxy/JavaScript/Proxy_Revocable_ThrowsAfterRevoke.js)<br>[`Proxy_Validation_EdgeCases.js`](../../../tests/Js2IL.Tests/Proxy/JavaScript/Proxy_Validation_EdgeCases.js) |  | Proxy.revocable(target, handler) validates object target/handler values, returns { proxy, revoke }, and makes the currently implemented proxy operations (get, set, has, deleteProperty, ownKeys, getPrototypeOf, setPrototypeOf, apply, and construct) throw TypeError once revoked. |

