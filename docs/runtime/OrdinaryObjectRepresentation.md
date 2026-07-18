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

`Object.GetProperty` delegates `JsObject` own reads to `TryGetBoxedValue`.
`JsObject` keeps descriptor lookup, accessor invocation, delete tombstones, and
lazy class-method materialization inside that contract, while preserving the
original receiver for inherited accessors. Exotic subclasses participate
through the same virtual/internal operations. Proxy traps, primitive behavior,
and prototype traversal remain the responsibility of the outer object runtime.

`Array : JsObject` is the first exotic subclass. It inherits identity, ordinary
named and symbol properties, prototype state, and descriptor integration, while
overriding only behavior that is exotic under ECMA-262:

- canonical array-index properties use dense/sparse element storage
- holes remain distinct from present `undefined`
- `length` uses `ArraySetLength` semantics
- indexed definitions and deletions enforce descriptor and integrity invariants
- own keys merge indices, other strings, and symbols in specification order

Array literals and compiler-proven numeric index operations still target direct
array intrinsics. The shared object contract does not replace the specialized
dense path or stringify numeric keys on that path.

Array intrinsic prototypes, per-thread prototype overlays, and
`Array.prototype[Symbol.unscopables]` are ordinary `JsObject` instances. This
matches all other runtime-owned intrinsic prototypes and leaves no
`ExpandoObject` representation in the runtime.

## Host boundary

C# dynamic interoperability is a hosting concern. `JsObject` still carries
transitional DLR support while [#1461](https://github.com/tomacox74/js2il/issues/1461)
moves it to `JsDynamicValueProxy` and `JsDynamicExports`. Internal JavaScript
execution does not dispatch through the DLR.

External CLR dictionaries and POCOs remain host objects. They are supported
through their normal host-object paths and are not treated as runtime-owned
ordinary JavaScript objects.
