<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 28.1: The Reflect Object

[Back to Section28](Section28.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-06-24T17:00:14Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 28.1 | The Reflect Object | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-reflect-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 28.1.1 | Reflect.apply ( target , thisArgument , argumentsList ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.apply) |
| 28.1.2 | Reflect.construct ( target , argumentsList [ , newTarget ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.construct) |
| 28.1.3 | Reflect.defineProperty ( target , propertyKey , attributes ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.defineproperty) |
| 28.1.4 | Reflect.deleteProperty ( target , propertyKey ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.deleteproperty) |
| 28.1.5 | Reflect.get ( target , propertyKey [ , receiver ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.get) |
| 28.1.6 | Reflect.getOwnPropertyDescriptor ( target , propertyKey ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.getownpropertydescriptor) |
| 28.1.7 | Reflect.getPrototypeOf ( target ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.getprototypeof) |
| 28.1.8 | Reflect.has ( target , propertyKey ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.has) |
| 28.1.9 | Reflect.isExtensible ( target ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.isextensible) |
| 28.1.10 | Reflect.ownKeys ( target ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.ownkeys) |
| 28.1.11 | Reflect.preventExtensions ( target ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.preventextensions) |
| 28.1.12 | Reflect.set ( target , propertyKey , V [ , receiver ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.set) |
| 28.1.13 | Reflect.setPrototypeOf ( target , proto ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.setprototypeof) |
| 28.1.14 | Reflect [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect-%symbol.tostringtag%) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 28.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.construct))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.construct | Supported with Limitations |  |  | The runtime exposes Reflect.construct and validates constructor targets/newTarget values, but this surface is documented as limited until broader Reflect and constructor-edge test262 coverage is imported. |

### 28.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.defineproperty))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.defineProperty | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Reflect/defineProperty/ExecutionTests.cs` |  | Reflect.defineProperty delegates to Object.defineProperty and returns true for successful descriptor definition. Current test262 coverage includes descriptor definition, symbol-keyed properties, length, and name. |

### 28.1.14 ([tc39.es](https://tc39.es/ecma262/#sec-reflect-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect[@@toStringTag] descriptor | Supported | `tests/Jroc.Test262.Tests/built-ins/Reflect/ExecutionTests.cs` | `test/built-ins/Reflect/Symbol.toStringTag.js` | Checked-in coverage now includes Reflect @@toStringTag value and descriptor attributes (`value: "Reflect"`, `writable: false`, `enumerable: false`, `configurable: true`). |

