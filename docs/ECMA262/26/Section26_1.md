<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 26.1: WeakRef Objects

[Back to Section26](Section26.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-08-15T05:05:31Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 26.1 | WeakRef Objects | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref-objects) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 26.1.1 | The WeakRef Constructor | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref-constructor) |
| 26.1.1.1 | WeakRef ( target ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref-target) |
| 26.1.2 | Properties of the WeakRef Constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weak-ref-constructor) |
| 26.1.2.1 | WeakRef.prototype | Supported | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref.prototype) |
| 26.1.3 | Properties of the WeakRef Prototype Object | Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weak-ref-prototype-object) |
| 26.1.3.1 | WeakRef.prototype.constructor | Supported | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref.prototype.constructor) |
| 26.1.3.2 | WeakRef.prototype.deref ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref.prototype.deref) |
| 26.1.3.3 | WeakRef.prototype [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-weak-ref.prototype-%symbol.tostringtag%) |
| 26.1.4 | WeakRef Abstract Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakref-abstract-operations) |
| 26.1.4.1 | WeakRefDeref ( weakRef ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-weakrefderef) |
| 26.1.5 | Properties of WeakRef Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-weak-ref-instances) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 26.1.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-weak-ref-target))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| WeakRef construction and weak-target validation | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/WeakRef/ExecutionTests.cs` | `test/built-ins/WeakRef/newtarget-prototype-is-not-object.js`<br>`test/built-ins/WeakRef/throws-when-target-cannot-be-held-weakly.js`<br>`test/built-ins/WeakRef/undefined-newtarget-throws.js` | Supports construction only with new, rejects primitives and registered symbols as targets, and uses the intrinsic WeakRef prototype when a newTarget prototype is not an object. Custom newTarget prototypes and cross-realm construction remain limited. |

### 26.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weak-ref-constructor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| WeakRef constructor function surface | Supported | `tests/Jroc.Test262.Tests/built-ins/WeakRef/ExecutionTests.cs` | `test/built-ins/WeakRef/constructor.js`<br>`test/built-ins/WeakRef/is-a-constructor.js`<br>`test/built-ins/WeakRef/length.js`<br>`test/built-ins/WeakRef/name.js`<br>`test/built-ins/WeakRef/prop-desc.js`<br>`test/built-ins/WeakRef/proto.js` | globalThis.WeakRef is a constructible function with standard name, length, global-property, and prototype descriptors. |

### 26.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-properties-of-the-weak-ref-prototype-object))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| WeakRef.prototype and deref() surface | Supported | `tests/Jroc.Test262.Tests/built-ins/WeakRef/prototype/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/built-ins/WeakRef/prototype/deref/ExecutionTests.cs` | `test/built-ins/WeakRef/prototype/constructor.js`<br>`test/built-ins/WeakRef/prototype/prop-desc.js`<br>`test/built-ins/WeakRef/prototype/proto.js`<br>`test/built-ins/WeakRef/prototype/Symbol.toStringTag.js`<br>`test/built-ins/WeakRef/prototype/deref/custom-this.js`<br>`test/built-ins/WeakRef/prototype/deref/length.js`<br>`test/built-ins/WeakRef/prototype/deref/name.js`<br>`test/built-ins/WeakRef/prototype/deref/not-a-constructor.js`<br>`test/built-ins/WeakRef/prototype/deref/prop-desc.js`<br>`test/built-ins/WeakRef/prototype/deref/this-does-not-have-internal-target-throws.js`<br>`test/built-ins/WeakRef/prototype/deref/this-not-object-throws.js` | WeakRef.prototype inherits Object.prototype and owns constructor, deref, and @@toStringTag properties. deref is non-constructible, supports explicit receivers, and rejects incompatible receivers. |

