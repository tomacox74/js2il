<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 28.2: Proxy Objects

[Back to Section28](Section28.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-09-10T22:10:46Z

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
| Proxy (target, handler) (constructor) | Supported with Limitations | [`Proxy_GetTrap_OverridesProperty.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_GetTrap_OverridesProperty.js)<br>[`Proxy_SetTrap_InterceptsWrites.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_SetTrap_InterceptsWrites.js)<br>[`Proxy_HasTrap_AffectsInOperator.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_HasTrap_AffectsInOperator.js)<br>[`Proxy_DeletePropertyTrap_And_Fallback.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_DeletePropertyTrap_And_Fallback.js)<br>[`Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_OwnKeys_And_PrototypeTraps_WithFallback.js)<br>[`Proxy_ApplyAndConstructTraps_WithFallback.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_ApplyAndConstructTraps_WithFallback.js)<br>[`Proxy_Revocable_ThrowsAfterRevoke.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_Revocable_ThrowsAfterRevoke.js)<br>[`Proxy_Validation_EdgeCases.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_Validation_EdgeCases.js)<br>`tests/Jroc.Tests/ErrorProxyObjectRepresentationTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/apply/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/construct/FailingBatch100ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/defineProperty/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/deleteProperty/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/get/FailingBatch100ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/getPrototypeOf/FailingBatch100ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/has/FailingBatch100ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/ownKeys/FailingBatch100ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/preventExtensions/FailingBatch100ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/set/FailingBatch100ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/setPrototypeOf/FailingBatch100ExecutionTests.cs` |  | Supports creating Proxy instances via new Proxy(target, handler) with object target/handler validation. Implemented traps include get, set, has, getOwnPropertyDescriptor, defineProperty, deleteProperty, ownKeys, getPrototypeOf, setPrototypeOf, isExtensible, preventExtensions, apply, and construct, with fallback-to-target behavior when absent. The new FailingBatch100ExecutionTests classes cover nested-Proxy fallback and receiver handling plus non-configurable, non-writable, non-extensible, duplicate-key, prototype, call-order, abrupt-completion, and boolean-result invariants across get, set, has, ownKeys, getPrototypeOf, setPrototypeOf, preventExtensions, and construct. Full Proxy coverage, including cross-realm cases, remains incomplete. |

### 28.2.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-proxy.revocable))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Proxy.revocable | Supported with Limitations | [`Proxy_Revocable_ThrowsAfterRevoke.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_Revocable_ThrowsAfterRevoke.js)<br>[`Proxy_Validation_EdgeCases.js`](../../../tests/Jroc.Tests/Proxy/JavaScript/Proxy_Validation_EdgeCases.js)<br>`tests/Jroc.Test262.Tests/built-ins/Proxy/revocable/FailingBatch100ExecutionTests.cs` |  | Proxy.revocable(target, handler) validates object target/handler values, returns { proxy, revoke }, and makes the currently implemented proxy operations throw TypeError once revoked. FailingBatch100ExecutionTests adds coverage for the built-in surface and the revocation function name. Broader realm and exotic-target cases remain limited. |

