<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 28.1: The Reflect Object

[Back to Section28](Section28.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-29T21:23:30Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 28.1 | The Reflect Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect-object) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 28.1.1 | Reflect.apply ( target , thisArgument , argumentsList ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.apply) |
| 28.1.2 | Reflect.construct ( target , argumentsList [ , newTarget ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.construct) |
| 28.1.3 | Reflect.defineProperty ( target , propertyKey , attributes ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.defineproperty) |
| 28.1.4 | Reflect.deleteProperty ( target , propertyKey ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.deleteproperty) |
| 28.1.5 | Reflect.get ( target , propertyKey [ , receiver ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.get) |
| 28.1.6 | Reflect.getOwnPropertyDescriptor ( target , propertyKey ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.getownpropertydescriptor) |
| 28.1.7 | Reflect.getPrototypeOf ( target ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.getprototypeof) |
| 28.1.8 | Reflect.has ( target , propertyKey ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.has) |
| 28.1.9 | Reflect.isExtensible ( target ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.isextensible) |
| 28.1.10 | Reflect.ownKeys ( target ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect.ownkeys) |
| 28.1.11 | Reflect.preventExtensions ( target ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.preventextensions) |
| 28.1.12 | Reflect.set ( target , propertyKey , V [ , receiver ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.set) |
| 28.1.13 | Reflect.setPrototypeOf ( target , proto ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-reflect.setprototypeof) |
| 28.1.14 | Reflect [ %Symbol.toStringTag% ] | Supported | [tc39.es](https://tc39.es/ecma262/#sec-reflect-%symbol.tostringtag%) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 28.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.apply))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.apply | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Reflect/apply/ExecutionTests.cs` | `test/built-ins/Reflect/apply/call-target.js` | Calls the target with the supplied this value and an argument list built with CreateListFromArrayLike, so array-like sources and holes are read by index and surface as undefined. Non-callable targets throw a TypeError. Argument-list coercion edge cases and abrupt completions from exotic array-likes are not yet covered. |

### 28.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.construct))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.construct | Supported with Limitations |  |  | The runtime exposes Reflect.construct and validates constructor targets/newTarget values, but this surface is documented as limited until broader Reflect and constructor-edge test262 coverage is imported. |

### 28.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.defineproperty))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.defineProperty | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Reflect/defineProperty/ExecutionTests.cs` |  | Reflect.defineProperty delegates to Object.defineProperty and returns true for successful descriptor definition. Current test262 coverage includes descriptor definition, symbol-keyed properties, length, and name. |

### 28.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.deleteproperty))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.deleteProperty | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Reflect/deleteProperty/ExecutionTests.cs` | `test/built-ins/Reflect/deleteProperty/delete-properties.js` | Performs the target [[Delete]] and reports the Boolean result rather than throwing for non-configurable properties, which is the observable difference from a strict-mode delete. Proxy deleteProperty traps are honored. Abrupt completions from property-key coercion are not yet covered. |

### 28.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.get))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.get | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Reflect/get/ExecutionTests.cs` | `test/built-ins/Reflect/get/return-value.js`<br>`test/built-ins/Reflect/get/return-value-from-receiver.js` | Implements OrdinaryGet including prototype-chain lookup and the optional receiver argument, so accessor getters are invoked with the receiver as their this value. Getters that are undefined return undefined. Abrupt completions from property-key coercion are not yet covered. |

### 28.1.6 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.getownpropertydescriptor))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.getOwnPropertyDescriptor | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Reflect/getOwnPropertyDescriptor/ExecutionTests.cs` | `test/built-ins/Reflect/getOwnPropertyDescriptor/return-from-data-descriptor.js` | Returns a FromPropertyDescriptor result for own properties with the spec property order (value, writable, enumerable, configurable) and undefined for missing own properties. Accessor-descriptor and symbol-key shapes are not yet covered by checked-in tests. |

### 28.1.7 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.getprototypeof))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.getPrototypeOf | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Reflect/getPrototypeOf/ExecutionTests.cs` | `test/built-ins/Reflect/getPrototypeOf/return-prototype.js` | Returns the target [[Prototype]] and throws a TypeError for non-object targets. Proxy getPrototypeOf traps are honored. Null-prototype and abrupt-completion cases are not yet covered by checked-in tests. |

### 28.1.8 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.has))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.has | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Reflect/has/ExecutionTests.cs` | `test/built-ins/Reflect/has/return-boolean.js` | Performs the target [[HasProperty]] across own and inherited properties, matching the in operator, and throws a TypeError for non-object targets. Abrupt completions from property-key coercion are not yet covered. |

### 28.1.9 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.isextensible))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.isExtensible | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Reflect/isExtensible/ExecutionTests.cs` | `test/built-ins/Reflect/isExtensible/return-boolean.js` | Reports the target [[IsExtensible]] state and throws a TypeError for non-object targets, unlike Object.isExtensible which coerces primitives to false. Proxy isExtensible trap invariants are not yet covered by checked-in tests. |

### 28.1.11 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.preventextensions))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.preventExtensions | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Reflect/preventExtensions/ExecutionTests.cs` | `test/built-ins/Reflect/preventExtensions/prevent-extensions.js` | Marks the target non-extensible and returns a Boolean. Subsequent property definition and prototype mutation on the target throw a TypeError through Object.defineProperty and Object.setPrototypeOf. Proxy preventExtensions trap results are not yet covered by checked-in tests. |

### 28.1.12 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.set))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.set | Supported with Limitations | [`callbackfn-set-value-during-iteration.js`](../../../tests/Jroc.Test262.Tests/built-ins/TypedArray/prototype/reduceRight/JavaScript/callbackfn-set-value-during-iteration.js) | `test/built-ins/TypedArray/prototype/reduceRight/callbackfn-set-value-during-iteration.js` | Supports three-argument assignment with Boolean success results across ordinary objects, arrays, TypedArrays, inherited descriptors, and proxy set traps. Explicit receiver semantics remain unsupported. |

### 28.1.13 ([tc39.es](https://tc39.es/ecma262/#sec-reflect.setprototypeof))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect.setPrototypeOf | Supported with Limitations | `tests/Jroc.Test262.Tests/built-ins/Reflect/setPrototypeOf/ExecutionTests.cs` | `test/built-ins/Reflect/setPrototypeOf/return-true-if-new-prototype-is-set.js` | Implements OrdinarySetPrototypeOf and reports failure with a Boolean instead of throwing, which is the observable difference from Object.setPrototypeOf. Returns true when the prototype is unchanged, and false for a non-extensible target or a cyclic prototype chain. Non-object, non-null proto values throw a TypeError. |

### 28.1.14 ([tc39.es](https://tc39.es/ecma262/#sec-reflect-%symbol.tostringtag%))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Reflect[@@toStringTag] descriptor | Supported | `tests/Jroc.Test262.Tests/built-ins/Reflect/ExecutionTests.cs` | `test/built-ins/Reflect/Symbol.toStringTag.js` | Checked-in coverage now includes Reflect @@toStringTag value and descriptor attributes (`value: "Reflect"`, `writable: false`, `enumerable: false`, `configurable: true`). |

