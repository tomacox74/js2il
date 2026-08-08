<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.5: Proxy Object Internal Methods and Internal Slots

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-08T05:06:20Z

JROC implements get/set/has/deleteProperty/ownKeys, descriptor, prototype, extensibility, apply, construct, and revocation proxy operations. Trap invocation uses centralized callable dispatch across generated and adapted callables. Proxy support remains incomplete for some descriptor-heavy, seal/freeze, and full invariant cases.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 10.5 | Proxy Object Internal Methods and Internal Slots | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 10.5.1 | [[GetPrototypeOf]] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-getprototypeof) |
| 10.5.2 | [[SetPrototypeOf]] ( V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-setprototypeof-v) |
| 10.5.3 | [[IsExtensible]] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-isextensible) |
| 10.5.4 | [[PreventExtensions]] ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-preventextensions) |
| 10.5.5 | [[GetOwnProperty]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-getownproperty-p) |
| 10.5.6 | [[DefineOwnProperty]] ( P , Desc ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-defineownproperty-p-desc) |
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

Feature-level support tracking with repo test references and optional test262 evidence.

### 10.5.3 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-isextensible))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Proxy callable, descriptor, and extensibility integration | Supported with Limitations | [`Function_CallableReflection_ProxyIntegration.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_CallableReflection_ProxyIntegration.js)<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/getOwnPropertyDescriptor/ExecutionTests.cs` |  | Callable classification, typeof, descriptor traps/accessor invocation, own keys, prototype operations, isExtensible, and preventExtensions route through proxy internal operations. Extensibility trap results are checked against target state; descriptor-heavy invariants remain partial. |

### 10.5.7 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-hasproperty-p))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Proxy has trap (handler.has) | Supported with Limitations | [`Proxy_HasTrap_AffectsInOperator.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_HasTrap_AffectsInOperator.js) |  | The in operator routes through handler.has(target, propertyKey) when present. Proxy invariants and descriptor-based validation are not enforced. |

### 10.5.8 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-get-p-receiver))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Proxy get trap (handler.get) | Supported with Limitations | [`Proxy_GetTrap_OverridesProperty.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_GetTrap_OverridesProperty.js) |  | Property reads call handler.get(target, propertyKey, receiver) before falling back to the target. Non-configurable target invariants and other edge cases are not checked. |

### 10.5.9 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-set-p-v-receiver))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Proxy set trap (handler.set) | Supported with Limitations | [`Proxy_SetTrap_InterceptsWrites.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_SetTrap_InterceptsWrites.js) |  | Property writes call handler.set(target, propertyKey, value, receiver) before falling back to the target. The return value is not validated against target descriptors or other proxy invariants. |

### 10.5.10 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-delete-p))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Advanced proxy traps and revocation | Supported with Limitations | [`Proxy_DeletePropertyTrap_And_Fallback.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_DeletePropertyTrap_And_Fallback.js)<br>[`Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js)<br>[`Proxy_ApplyAndConstructTraps_WithFallback.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_ApplyAndConstructTraps_WithFallback.js)<br>[`Proxy_Revocable_ThrowsAfterRevoke.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_Revocable_ThrowsAfterRevoke.js)<br>[`Proxy_Validation_EdgeCases.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_Validation_EdgeCases.js) |  | deleteProperty, ownKeys, getPrototypeOf, setPrototypeOf, apply, construct, isExtensible, preventExtensions, getOwnPropertyDescriptor, defineProperty, and revocation route through proxy handlers and throw once revoked. Full invariant validation remains incomplete for some descriptor and seal/freeze combinations. |

### 10.5.12 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-call-thisargument-argumentslist))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Centralized proxy [[Call]] and [[Construct]] | Supported with Limitations | [`Function_CallableReflection_ProxyIntegration.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_CallableReflection_ProxyIntegration.js)<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/apply/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/construct/ExecutionTests.cs` |  | Proxy apply/construct traps invoke generated or adapted trap functions through CallableOperations with the correct handler this, target, thisArgument/argument list, and newTarget. Missing traps forward through centralized target call/construct; non-callable traps and non-object construct results throw TypeError. |

### 10.5.15 ([tc39.es](https://tc39.es/ecma262/#sec-proxycreate))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| new Proxy(target, handler) | Supported with Limitations | [`Proxy_GetTrap_OverridesProperty.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_GetTrap_OverridesProperty.js)<br>[`Proxy_SetTrap_InterceptsWrites.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_SetTrap_InterceptsWrites.js)<br>[`Proxy_HasTrap_AffectsInOperator.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_HasTrap_AffectsInOperator.js)<br>[`Proxy_DeletePropertyTrap_And_Fallback.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_DeletePropertyTrap_And_Fallback.js)<br>[`Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js)<br>[`Proxy_ApplyAndConstructTraps_WithFallback.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_ApplyAndConstructTraps_WithFallback.js)<br>[`Proxy_Revocable_ThrowsAfterRevoke.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_Revocable_ThrowsAfterRevoke.js)<br>[`Proxy_Validation_EdgeCases.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_Validation_EdgeCases.js) |  | ProxyCreate validates object targets/handlers and routes get, set, has, deleteProperty, ownKeys, apply, construct, getPrototypeOf, and setPrototypeOf through handler traps when present. Absent traps fall back to the target/default runtime behavior, and the supported trap surface now enforces basic result-shape checks (for example ownKeys/getPrototypeOf/construct) plus callable/constructible target gating for apply/construct. Full Proxy invariant enforcement and descriptor-heavy traps remain unimplemented. |

