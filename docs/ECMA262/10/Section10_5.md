<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.5: Proxy Object Internal Methods and Internal Slots

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-29T04:47:28Z

JS2IL implements the core get/set/has proxy traps plus deleteProperty, ownKeys, apply, construct, getPrototypeOf, setPrototypeOf, and revocable proxies. Proxy support is still incomplete because descriptor-heavy traps and full Proxy invariant enforcement are not modeled yet.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 10.5 | Proxy Object Internal Methods and Internal Slots | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 10.5.1 | [[GetPrototypeOf]] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-getprototypeof) |
| 10.5.2 | [[SetPrototypeOf]] ( V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-setprototypeof-v) |
| 10.5.3 | [[IsExtensible]] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-isextensible) |
| 10.5.4 | [[PreventExtensions]] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-preventextensions) |
| 10.5.5 | [[GetOwnProperty]] ( P ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-getownproperty-p) |
| 10.5.6 | [[DefineOwnProperty]] ( P , Desc ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-defineownproperty-p-desc) |
| 10.5.7 | [[HasProperty]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-hasproperty-p) |
| 10.5.8 | [[Get]] ( P , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-get-p-receiver) |
| 10.5.9 | [[Set]] ( P , V , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-set-p-v-receiver) |
| 10.5.10 | [[Delete]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-delete-p) |
| 10.5.11 | [[OwnPropertyKeys]] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-ownpropertykeys) |
| 10.5.12 | [[Call]] ( thisArgument , argumentsList ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-call-thisargument-argumentslist) |
| 10.5.13 | [[Construct]] ( argumentsList , newTarget ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-construct-argumentslist-newtarget) |
| 10.5.14 | ValidateNonRevokedProxy ( proxy ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-validatenonrevokedproxy) |
| 10.5.15 | ProxyCreate ( target , handler ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxycreate) |

## Support

Feature-level support tracking with test script references.

### 10.5.7 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-hasproperty-p))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Proxy has trap (handler.has) | Supported with Limitations | [`Proxy_HasTrap_AffectsInOperator.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_HasTrap_AffectsInOperator.js) | The in operator routes through handler.has(target, propertyKey) when present. Proxy invariants and descriptor-based validation are not enforced. |

### 10.5.8 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-get-p-receiver))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Proxy get trap (handler.get) | Supported with Limitations | [`Proxy_GetTrap_OverridesProperty.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_GetTrap_OverridesProperty.js) | Property reads call handler.get(target, propertyKey, receiver) before falling back to the target. Non-configurable target invariants and other edge cases are not checked. |

### 10.5.9 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-set-p-v-receiver))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Proxy set trap (handler.set) | Supported with Limitations | [`Proxy_SetTrap_InterceptsWrites.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_SetTrap_InterceptsWrites.js) | Property writes call handler.set(target, propertyKey, value, receiver) before falling back to the target. The return value is not validated against target descriptors or other proxy invariants. |

### 10.5.10 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-delete-p))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Advanced proxy traps and revocation | Supported with Limitations | [`Proxy_DeletePropertyTrap_And_Fallback.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_DeletePropertyTrap_And_Fallback.js)<br>[`Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js)<br>[`Proxy_ApplyAndConstructTraps_WithFallback.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_ApplyAndConstructTraps_WithFallback.js)<br>[`Proxy_Revocable_ThrowsAfterRevoke.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_Revocable_ThrowsAfterRevoke.js) | deleteProperty, ownKeys, getPrototypeOf, setPrototypeOf, apply, construct, and revocation now route through proxy handlers and throw once revoked. isExtensible, preventExtensions, getOwnProperty, defineProperty, and full spec invariant validation are still not modeled. |

### 10.5.15 ([tc39.es](https://tc39.es/ecma262/#sec-proxycreate))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| new Proxy(target, handler) | Supported with Limitations | [`Proxy_GetTrap_OverridesProperty.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_GetTrap_OverridesProperty.js)<br>[`Proxy_SetTrap_InterceptsWrites.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_SetTrap_InterceptsWrites.js)<br>[`Proxy_HasTrap_AffectsInOperator.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_HasTrap_AffectsInOperator.js)<br>[`Proxy_DeletePropertyTrap_And_Fallback.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_DeletePropertyTrap_And_Fallback.js)<br>[`Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js)<br>[`Proxy_ApplyAndConstructTraps_WithFallback.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_ApplyAndConstructTraps_WithFallback.js)<br>[`Proxy_Revocable_ThrowsAfterRevoke.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_Revocable_ThrowsAfterRevoke.js) | ProxyCreate stores the target/handler and routes get, set, has, deleteProperty, ownKeys, apply, construct, getPrototypeOf, and setPrototypeOf through handler traps when present. Absent traps fall back to the target/default runtime behavior, but full Proxy invariant enforcement and descriptor-heavy traps remain unimplemented. |

