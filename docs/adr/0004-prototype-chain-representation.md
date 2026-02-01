# ADR 0004: Prototype Chain Representation (Side Table vs New Object Types)

- Date: 2026-01-31
- Status: Accepted

## Context

JS2IL currently supports dynamic property access primarily via runtime helpers (e.g. `JavaScriptRuntime.Object`), but does not model ECMAScript `[[Prototype]]`.

We want to add support for prototype chaining and inheritance, using [docs/PrototypeChainSupport.md](../PrototypeChainSupport.md) as the long-term guide.

A near-term goal (Issue #504) is to support:

- legacy `__proto__` get/set
- `Object.getPrototypeOf()`
- `Object.setPrototypeOf()`
- inherited property lookup through a prototype chain

The key design choice for the first increment is how we represent `[[Prototype]]` for objects created/used by generated code.

## Decision

For the initial implementation (Issue #504), **represent `[[Prototype]]` using a runtime side-table**:

- Use a `ConditionalWeakTable<object, PrototypeSlot>` (or equivalent) to associate an optional prototype reference with arbitrary receiver objects.
- Teach property lookup helpers to walk the side-table chain when an own-property miss occurs.
- Special-case the `"__proto__"` key in get/set helpers to behave like a legacy accessor (even before full property descriptor/accessor support exists).

Additionally, **prototype-chain behavior is opt-in**:

- Default behavior remains unchanged unless the compiler detects explicit prototype-related usage.
- When opted-in, the compiler emits a small module-init prologue to enable prototype-chain behavior in the runtime.

We defer introducing a full `IJsObject` / `JsOrdinaryObject` runtime object model until a later stage (descriptors/accessors, function objects, proper `.prototype` semantics for `new`).

## Rationale

### Why a side-table first

- **Minimal disruption**: JS2IL already routes most dynamic member access through runtime helper methods; extending these helpers is a small, localized change.
- **Works with existing CLR-backed objects**: we can attach a prototype to objects represented today as `ExpandoObject`, runtime types, or other host objects without rewriting their representations.
- **Low dependency cost**: `ConditionalWeakTable` is in the BCL; no new runtime package dependencies.
- **Incremental correctness**: enables `__proto__` / `getPrototypeOf` / `setPrototypeOf` and inherited lookup (the core primitive needed for Domino-like prototype rewiring) without implementing the full descriptor model.

### Why not introduce `IJsObject` immediately

A dedicated JS object model is aligned with the long-term design in [docs/PrototypeChainSupport.md](../PrototypeChainSupport.md), but it requires multiple coupled features to be useful:

- property descriptors (enumerable/configurable/writable)
- getters/setters
- function objects with `prototype` properties
- `new` using `constructor.prototype`

Shipping `IJsObject` without these risks creating two incomplete object models and increasing the surface area of changes required for the first milestone.

## Consequences

### Positive

- Enables Issue #504 with minimal compiler changes.
- Keeps performance characteristics close to current behavior for common own-property hits.
- Allows later migration to a richer object model while preserving API behavior.

### Negative

- Prototype chain behavior is not inherently encoded in object instances; it is maintained externally.
- Prototype-aware lookups add overhead on misses.
- Some objects may need policy decisions (e.g., whether prototypes are allowed for certain CLR types).

### Mitigations

- Keep the side-table lookup cheap for the common case (no prototype attached).
- Consider later adding caching/versioning if prototype-sensitive workloads become slow (per the long-term design document).

## Alternatives Considered

### 1) Introduce `IJsObject` / `JsOrdinaryObject` now

Rejected for the first increment due to the larger required surface area (descriptors, accessors, function objects), and because it would force broader compiler/runtime refactoring before we can address Issue #504.

### 2) Store prototype directly in specific runtime types only

Partially viable, but rejected because JS2IL currently uses multiple CLR representations for objects, and this approach would either miss cases or require many wrappers. The side-table approach provides uniform coverage.
