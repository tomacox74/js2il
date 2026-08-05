# Runtime object representation

`JsObject` is JROC's common runtime substrate for JavaScript objects. It provides
identity, shape/slot storage, deterministic own-key ordering, unboxed
number/boolean slots, descriptor integration, and prototype support. Compiler
object literals, function-constructed instances, built-in result records, module
records, Node-created ordinary records, intrinsic prototypes, and `Array` all use
this representation.

Runtime property operations use `ObjectRuntime` and `Object` so descriptors,
prototypes, accessors, integrity levels, proxies, and enumeration retain
ECMAScript behavior. Runtime-created descriptor records are also `JsObject`
instances; callers must not depend on a CLR dynamic-object implementation.

## Ordinary and exotic operations

The virtual internal-operation hooks on `JsObject` cover:

- own descriptor lookup
- own property-value resolution, including data/accessor descriptors and lazy methods
- specialized backing-value lookup and presence
- property definition and assignment
- deletion
- complete own-key enumeration

Ordinary objects implement these operations with shape/slot and descriptor
storage. Generic runtime code dispatches through this shared contract instead of
maintaining a parallel representation switch.

Each `JsObject` has one nullable descriptor-state reference in addition to its
shape and `JsValue[]`. A missing state means every shape slot is an implicit
writable, enumerable, configurable data property. This common representation
does not allocate descriptor dictionaries or duplicate values: descriptor reads
are synthesized from the shape slot and its canonical `JsValue`.

The first non-default data property or accessor lazily allocates compact
shape-indexed flags. Getter/setter payloads are allocated only after the first
accessor. Shape growth, deletion compaction, and clearing keep values, flags, and
accessors at identical slot indices. The extra nullable reference increases an
empty 64-bit `JsObject` allocation from 48 to 56 bytes; objects containing only
default properties pay no additional descriptor-state allocation.

Shared intrinsic `JsObject` templates initialize their baseline descriptors in
this inline representation. Once initialization ends, per-runtime
`ConditionalWeakTable` snapshots remain authoritative for realm-local
overrides, additions, and deletion tombstones, leaving the shared baseline
immutable and lock-free to read. Non-`JsObject` targets such as delegates, CLR
types, exceptions, and host dictionaries continue to use weak-table descriptor
storage.

`JsPropertyDescriptor` reads return independent value copies. Accessor getter
and setter references retain their JavaScript identity across copies, and
ordinary data values always come from `JsValue[]`.

`Object.GetProperty` delegates `JsObject` own reads to `TryGetBoxedValue`.
`JsObject` checks stored descriptor overrides, accessors, and delete tombstones
inside that contract, while preserving the original receiver for inherited
accessors. When no stored descriptor affects the read, it asks the object's
backing-value hook directly instead of materializing a descriptor. Full
descriptor synthesis remains confined to descriptor APIs. Exotic subclasses
participate through the same virtual/internal operations. Proxy traps,
primitive behavior, and prototype traversal remain the responsibility of the
outer object runtime.

`Array : JsObject` is the first exotic subclass. It inherits identity, ordinary
named and symbol properties, prototype state, and descriptor integration, while
overriding only behavior that is exotic under ECMA-262:

- canonical array-index properties use dense/sparse element storage
- holes remain distinct from present `undefined`
- `length` uses `ArraySetLength` semantics
- indexed definitions and deletions enforce descriptor and integrity invariants
- own keys merge indices, other strings, and symbols in specification order

Ordinary reads of default `length`, dense indices, and named Array properties
use their backing storage directly. Stored accessor/data overrides and
tombstones remain authoritative, while APIs such as
`Object.getOwnPropertyDescriptor` continue through Array's descriptor hook.
Named properties use ordinary shape-aligned descriptor state. Canonical indices
and `length` never acquire shape slots; their non-default descriptors use the
same lazily allocated object-owned state through a sparse exotic override map.

Array literals and compiler-proven numeric index operations still target direct
array intrinsics. The shared object contract does not replace the specialized
dense path or stringify numeric keys on that path.

Array intrinsic prototypes, per-thread prototype overlays, and
`Array.prototype[Symbol.unscopables]` are ordinary `JsObject` instances. This
matches all other runtime-owned intrinsic prototypes and leaves no
`ExpandoObject` representation in the runtime.

## Function object foundation

`JsFunctionObject : JsObject` is the common base for object-backed JavaScript
callables. It inherits ordinary property, symbol, descriptor, prototype,
integrity, deletion, and identity behavior from `JsObject`, and its
`[[Prototype]]` is initialized to `Function.prototype`.

`CallableOperations` is the centralized runtime boundary for `IsCallable`,
`[[Call]]`, `IsConstructor`, and `[[Construct]]`. Receiver, arguments, callee,
and `newTarget` are passed per invocation and installed in the runtime's
`AsyncLocal` execution context only for the duration of the call. They are
never stored as mutable fields on the function object, so recursion,
reentrancy, and concurrent calls remain isolated.

`LegacyDelegateFunctionAdapter` keeps existing delegate-backed compiled
functions available during the staged migration. New object-backed callables
derive from `JsFunctionObject` and use the
[fixed-arity/arbitrary invocation ABI](JsFunctionObjectInvocationAbi.md).
Compiler-generated subclasses are tracked under
[#1711](https://github.com/tomacox74/js2il/issues/1711).

## Host boundary

C# dynamic interoperability is a hosting concern. `JsObject` still carries
transitional DLR support while [#1461](https://github.com/tomacox74/js2il/issues/1461)
moves it to `JsDynamicValueProxy` and `JsDynamicExports`. Internal JavaScript
execution does not dispatch through the DLR.

External CLR dictionaries and POCOs remain host objects. They are supported
through their normal host-object paths and are not treated as runtime-owned
ordinary JavaScript objects.
