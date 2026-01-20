# Prototype Support Design (Hybrid Typed + Dynamic)

## Context
JS2IL currently leans heavily on .NET types to represent JavaScript values:

- **Scopes-as-classes**: every JS scope becomes a CLR class with fields for bindings.
- **Intrinsics and runtime objects**: e.g. `JavaScriptRuntime.Array`, `String`, etc. are CLR types.
- **Optimizations** (recent): typed locals and typed intrinsic return values reduce boxing/casts.

This is fast for the subset of JavaScript that behaves like “static object layout + fixed methods”.
However, JavaScript is **prototype-based**:

- Properties can be added/removed at runtime.
- Property lookup walks the **prototype chain**.
- Prototypes can be mutated (e.g. `Foo.prototype.bar = ...`).
- An object’s prototype can be changed (e.g. `Object.setPrototypeOf(obj, p)` / `obj.__proto__ = p`).

Today, many member accesses are effectively mapped to:

- Direct CLR member/field access when inferred safe
- Or runtime helpers like `Object.GetProperty`, `Object.CallMember`, etc.

The gap: CLR types do not naturally model **prototype chain semantics**, and `CallMember`/member lookup semantics need to match Node/V8 expectations.

## Goals
1. **Keep the current typed/CLR representation as the default fast path**.
2. **Correctly support prototype semantics** when code requires it.
3. Prefer a **pay-for-what-you-use** model:
   - If the program never mutates prototypes in observable ways, keep the fast typed strategy.
   - If the program does mutate prototypes (or can), “downgrade” only the affected sites/objects.
4. Make the behavior compatible with Node/V8 where practical (errors, lookup order, etc.).

## Non-goals (initially)
- Full spec completeness for all exotic objects (Proxy, typed arrays as full spec, etc.)
- Full fidelity of property attributes (writable/enumerable/configurable) everywhere
- Perfect deopt granularity on day one (we can start coarse and refine)

## Terminology
- **Shape / hidden class**: a compact description of an object’s own-property layout.
- **Prototype chain**: `obj -> obj.[[Prototype]] -> ... -> null`.
- **IC (Inline Cache)**: a callsite cache keyed by receiver shape/prototype version.
- **Deopt**: switching from a typed fast path to a dynamic runtime path.

## Design Overview: Two-tier object model
### Tier 1: Typed/CLR-backed objects (fast path)
This is what we have today and want to preserve:

- User-class instances: emitted as CLR types with fields
- Runtime intrinsics: CLR types (`JavaScriptRuntime.Array`, `String`, etc.)

Key idea: **typed objects still need a concept of a prototype**.
We can attach this concept in one of two ways:

1) **Side-table prototype metadata** keyed by CLR object identity
- Keep CLR type unchanged.
- Maintain a `ConditionalWeakTable<object, PrototypeInfo>` that stores:
  - `[[Prototype]]` pointer
  - “dynamic extras” dictionary (optional)
  - version stamps

2) **Common base class / interface** for runtime objects
- e.g. `IJsObject` exposing `Prototype`, `GetOwnProperty`, etc.
- More invasive but clearer.

Recommendation: start with **(1) side-table** for minimal disruption; migrate to interfaces later if it pays off.

### Tier 2: Dynamic objects (slow path)
A `DynamicObject` representation that always uses:

- Own property dictionary (string/symbol keys)
- A `[[Prototype]]` pointer
- Shape/version metadata

This tier is used when:

- An object is created via object literal and then mutated in “shape-unstable” ways
- `Object.setPrototypeOf` / `__proto__` is used
- Prototype objects (e.g. `Foo.prototype`) are mutated
- A property access cannot be proven stable

## Prototype representation
Introduce a runtime concept of a prototype object:

- `JsPrototypeObject` (or reuse existing runtime object type) that stores properties and has its own `[[Prototype]]`.
- Support a `null` prototype.

We need to model common built-in prototype chains:

- `Object.prototype`
- `Function.prototype`
- `Array.prototype`
- `String.prototype`
- etc.

For now, we can implement **just enough** for:

- Member lookup through prototypes
- Method calls (`obj.method()`) with correct error behavior when missing/not-callable

## Runtime semantics: property resolution
### Get (read)
Algorithm sketch:

1. If receiver has an own property `p`, return it.
2. Else walk `[[Prototype]]`:
   - If prototype has own property `p`, return it.
   - Continue until `null`.
3. If not found, return `undefined`.

### Set (write)
Start minimal:

- If receiver has own property `p`, set it.
- Else create own property `p` on receiver.

(Full spec involves setters, writable flags, etc.; can be added later.)

### CallMember
`obj.method(args...)` should:

1. Resolve property `method` using Get semantics.
2. If result is `undefined` or not callable:
   - Throw `TypeError` with Node/V8 compatible message shape.
   - Message must include the method name (variable name is a nice-to-have).
3. Invoke callable with correct `this` binding.

This aligns with issue #377.

## Performance strategy: typed fast paths with guarded fallback
### The core tradeoff
- **Typed direct call** is fastest but assumes stable lookup.
- Prototype mutation means lookup can change, so we need guard + fallback.

### Strategy
At compile-time, attempt to keep operations typed:

- If the receiver type is proven and the target member is known (e.g. intrinsic method on CLR type), emit direct `callvirt`.
- Otherwise emit a dynamic helper (`GetProperty` / `CallMember`).

At runtime, make dynamic operations fast via:

- Hidden classes / shapes
- Inline caches with version checks
- Prototype versioning and invalidation

### Prototype versioning
Maintain a `VersionStamp` on each prototype object.

- Any write that could affect lookup (add/replace/delete property) increments the prototype’s version.
- Optional: propagate “dependent” invalidations (e.g. a global counter for all prototypes in a chain).

Callsite cache entries store:

- Receiver shape id
- Prototype chain version tuple (or a combined chain hash)
- Resolved slot/function handle

When versions match → fast dispatch.
When mismatch → full lookup + refresh cache.

### Downgrade / deopt policy
We can implement **coarse deopt first**:

- If any prototype mutation is detected anywhere, force dynamic member lookup for:
  - all objects of that “kind” (e.g. all user objects)
  - or all member calls in the module

Then refine to **targeted deopt**:

- Only downgrade callsites where:
  - receiver type is affected
  - mutated prototype is in its lookup chain

## Detecting prototype mutation (when to downgrade)
### Compile-time signals (static analysis)
In the IR lowering pipeline, detect usage of:

- `Object.setPrototypeOf`
- `Object.create(proto)` with non-default proto
- `__proto__` assignment
- Writes to `.prototype` objects:
  - `Foo.prototype.bar = ...`
  - `Foo.prototype = ...`
- Writes to built-in prototypes:
  - `Array.prototype.foo = ...`
  - `String.prototype.match = ...`

If any are present, mark the module/function as **prototype-dynamic**.

### Runtime signals
Even if not seen statically, runtime libraries or eval-like behavior might mutate.

We can add a runtime switch:

- `RuntimeOptions.EnablePrototypeDeopt`
- Track actual mutations and toggle “dynamic mode” if they occur.

## Compiler impact (where this fits)
### Parsing/validation
No new syntax needed, but validators should avoid rejecting prototype-related constructs unnecessarily.

### Symbol table / type generation
No change required to start.

### Lowering decisions
Member access lowering already selects between:

- Direct call / field access
- Runtime helper calls

We can extend the heuristics:

- If module/function is not prototype-dynamic, allow more typed direct calls.
- If prototype-dynamic, prefer runtime helpers for member reads/calls on affected types.

### IL emission
No special IL needed beyond calling runtime helpers.

## Runtime API changes (proposed)
Introduce/extend runtime helpers:

- `Object.GetPrototype(object receiver) : object?`
- `Object.SetPrototype(object receiver, object? proto) : object`
- `Object.GetProperty(object receiver, string name) : object?`
- `Object.SetProperty(object receiver, string name, object? value) : object`
- `Object.CallMember(object receiver, string methodName, object[] args) : object?`

And dynamic object support:

- `DynamicObject` (own props + prototype + shape)
- `PrototypeInfo` side-table record for CLR-backed objects

## Compatibility notes
### Error types/messages
Node/V8 error behavior matters for developer UX and test compatibility.

- Missing/non-callable member calls should throw `TypeError`.
- Message should include method name; variable name optional.

### `this` binding
Member calls must bind `this` to the receiver, even if the function value came from the prototype.

## Testing plan
Add execution tests that cover:

1. Prototype chain lookup
   - `obj.__proto__.x = 1; console.log(obj.x)`
2. Mutating a prototype after instance creation
   - `function C(){}; let o=new C(); C.prototype.m=()=>1; console.log(o.m())`
3. Overriding prototype method with own property
4. `Object.setPrototypeOf`
5. Node-compatible TypeError for missing member call

## Incremental implementation roadmap
1. **Fix error compatibility for CallMember** (issue #377)
   - Throw `TypeError` with method name included.
2. Add minimal `[[Prototype]]` storage using side-table + dynamic `JsPrototypeObject`.
3. Implement prototype-chain lookup for `GetProperty` / `CallMember`.
4. Add coarse “prototype-dynamic” mode and force runtime helpers when active.
5. Add caching (shape + prototype version) for `GetProperty` and `CallMember`.
6. Refine static analysis to reduce unnecessary deopts.

## Open questions
- Should user-defined `class` instances use a separate JS-level prototype object even when mapped to CLR types?
- How to represent Symbols in property keys (future)?
- How much of property descriptors (get/set, writable, enumerable) is needed short-term?
- What granularity of deopt is worth it for JS2IL’s target workloads (Node scripts vs general JS)?
