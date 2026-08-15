<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 26.2: FinalizationRegistry Objects

[Back to Section26](Section26.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-15T06:08:55Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 26.2 | FinalizationRegistry Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 26.2.1 | The FinalizationRegistry Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry-constructor) |
| 26.2.1.1 | FinalizationRegistry ( cleanupCallback ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry-cleanup-callback) |
| 26.2.2 | Properties of the FinalizationRegistry Constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-finalization-registry-constructor) |
| 26.2.2.1 | FinalizationRegistry.prototype | Supported | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry.prototype) |
| 26.2.3 | Properties of the FinalizationRegistry Prototype Object | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-finalization-registry-prototype-object) |
| 26.2.3.1 | FinalizationRegistry.prototype.constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry.prototype.constructor) |
| 26.2.3.2 | FinalizationRegistry.prototype.register ( target , heldValue [ , unregisterToken ] ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry.prototype.register) |
| 26.2.3.3 | FinalizationRegistry.prototype.unregister ( unregisterToken ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry.prototype.unregister) |
| 26.2.3.4 | FinalizationRegistry.prototype [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-finalization-registry.prototype-%symbol.tostringtag%) |
| 26.2.4 | Properties of FinalizationRegistry Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-finalization-registry-instances) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 26.2 ([tc39.es](https://tc39.es/ecma262/#sec-finalization-registry-objects))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| FinalizationRegistry construction and cleanup callback | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/FinalizationRegistry/ExecutionTests.cs`<br>[`FinalizationRegistry_Cleanup_Order.js`](../../../tests/Jroc.Tests/FinalizationRegistry/JavaScript/FinalizationRegistry_Cleanup_Order.js)<br>[`FinalizationRegistry_Unregister_Basic.js`](../../../tests/Jroc.Tests/FinalizationRegistry/JavaScript/FinalizationRegistry_Unregister_Basic.js) | `test/built-ins/FinalizationRegistry/newtarget-prototype-is-not-object.js`<br>`test/built-ins/FinalizationRegistry/returns-new-object-from-constructor.js`<br>`test/built-ins/FinalizationRegistry/undefined-newtarget-throws.js` | Supports construction only with new and uses the intrinsic FinalizationRegistry prototype when a newTarget prototype is not an object. Cleanup callbacks are queued through a host-managed finalization queue and become deterministic when a host-opt-in non-standard gc() helper forces collection. Custom newTarget prototypes, cross-realm construction, and cleanup timing outside that helper remain limited. |

### 26.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-finalization-registry-constructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| FinalizationRegistry constructor function surface | Supported | `tests/Jroc.Test262.Tests/built-ins/FinalizationRegistry/ExecutionTests.cs` | `test/built-ins/FinalizationRegistry/constructor.js`<br>`test/built-ins/FinalizationRegistry/is-a-constructor.js`<br>`test/built-ins/FinalizationRegistry/length.js`<br>`test/built-ins/FinalizationRegistry/name.js`<br>`test/built-ins/FinalizationRegistry/prop-desc.js`<br>`test/built-ins/FinalizationRegistry/proto.js` | globalThis.FinalizationRegistry is a constructible function with standard name, length, global-property, and prototype descriptors. |

### 26.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-finalization-registry-prototype-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| FinalizationRegistry.prototype register/unregister surface | Supported | `tests/Jroc.Test262.Tests/built-ins/FinalizationRegistry/prototype/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/FinalizationRegistry/prototype/register/ExecutionTests.cs`<br>[`FinalizationRegistry_Unregister_Basic.js`](../../../tests/Jroc.Tests/FinalizationRegistry/JavaScript/FinalizationRegistry_Unregister_Basic.js) | `test/built-ins/FinalizationRegistry/prototype/constructor.js`<br>`test/built-ins/FinalizationRegistry/prototype/prop-desc.js`<br>`test/built-ins/FinalizationRegistry/prototype/proto.js`<br>`test/built-ins/FinalizationRegistry/prototype/Symbol.toStringTag.js`<br>`test/built-ins/FinalizationRegistry/prototype/register/custom-this.js`<br>`test/built-ins/FinalizationRegistry/prototype/register/length.js`<br>`test/built-ins/FinalizationRegistry/prototype/register/name.js`<br>`test/built-ins/FinalizationRegistry/prototype/register/not-a-constructor.js`<br>`test/built-ins/FinalizationRegistry/prototype/register/prop-desc.js`<br>`test/built-ins/FinalizationRegistry/prototype/register/this-does-not-have-internal-target-throws.js`<br>`test/built-ins/FinalizationRegistry/prototype/register/this-not-object-throws.js` | FinalizationRegistry.prototype inherits Object.prototype and owns constructor, register, unregister, and @@toStringTag properties. register and unregister are non-constructible, support explicit receivers, and reject incompatible receivers. |

