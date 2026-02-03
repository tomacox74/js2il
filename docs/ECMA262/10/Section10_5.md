<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 10.5: Proxy Object Internal Methods and Internal Slots

[Back to Section10](Section10.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 10.5 | Proxy Object Internal Methods and Internal Slots | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 10.5.1 | [[GetPrototypeOf]] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-getprototypeof) |
| 10.5.2 | [[SetPrototypeOf]] ( V ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-setprototypeof-v) |
| 10.5.3 | [[IsExtensible]] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-isextensible) |
| 10.5.4 | [[PreventExtensions]] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-preventextensions) |
| 10.5.5 | [[GetOwnProperty]] ( P ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-getownproperty-p) |
| 10.5.6 | [[DefineOwnProperty]] ( P , Desc ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-defineownproperty-p-desc) |
| 10.5.7 | [[HasProperty]] ( P ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-hasproperty-p) |
| 10.5.8 | [[Get]] ( P , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-get-p-receiver) |
| 10.5.9 | [[Set]] ( P , V , Receiver ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-set-p-v-receiver) |
| 10.5.10 | [[Delete]] ( P ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-delete-p) |
| 10.5.11 | [[OwnPropertyKeys]] ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-ownpropertykeys) |
| 10.5.12 | [[Call]] ( thisArgument , argumentsList ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-call-thisargument-argumentslist) |
| 10.5.13 | [[Construct]] ( argumentsList , newTarget ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-construct-argumentslist-newtarget) |
| 10.5.14 | ValidateNonRevokedProxy ( proxy ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-validatenonrevokedproxy) |
| 10.5.15 | ProxyCreate ( target , handler ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-proxycreate) |

## Support

Feature-level support tracking with test script references.

### 10.5.7 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-hasproperty-p))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| has trap (handler.has) | Supported with Limitations | [`Proxy_HasTrap_AffectsInOperator.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_HasTrap_AffectsInOperator.js) | Routes the in operator through handler.has(target, propertyKey) when present; does not implement full invariants/edge-case behaviors. |

### 10.5.8 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-get-p-receiver))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| get trap (handler.get) | Supported with Limitations | [`Proxy_GetTrap_OverridesProperty.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_GetTrap_OverridesProperty.js) | Routes basic property reads through handler.get(target, propertyKey, receiver) when present; does not implement full invariants/edge-case behaviors. |

### 10.5.9 ([tc39.es](https://tc39.es/ecma262/#sec-proxy-object-internal-methods-and-internal-slots-set-p-v-receiver))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| set trap (handler.set) | Supported with Limitations | [`Proxy_SetTrap_InterceptsWrites.js`](../../../Js2IL.Tests/Proxy/JavaScript/Proxy_SetTrap_InterceptsWrites.js) | Routes basic property writes through handler.set(target, propertyKey, value, receiver) when present; does not implement full invariants/edge-case behaviors. |

