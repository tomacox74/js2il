# Prototype Chain Support Strategy

This document describes how JS2IL should support JavaScript’s prototype-based inheritance while preserving **early-bound** (fast) field/method access for the common case.

## Motivation

Today, JS2IL can compile many calls as **early-bound direct calls** (e.g., a known method on a known runtime type) without guards.

Example: `tests/performance/PrimeJavaScript.js` contains calls that are currently compiled into fast direct calls (e.g., an early-bound call to `setBitsTrue` around the loop-heavy hot path). This is the performance posture we want to preserve by default.

However, JavaScript allows mutation of behavior via:

- Prototype mutation (e.g., `Array.prototype.indexOf = ...`)
- Per-instance method replacement (e.g., `a.indexOf = ...`)
- Prototype reassignment (e.g., `Object.setPrototypeOf(a, ...)` or `a.__proto__ = ...`)

When these appear, JS2IL should switch to a semantics-preserving strategy that is compliant with JavaScript’s property lookup rules.

## High-level Goal

- **Default**: Stay **early-bound** to object fields and methods.
- **Escalate only when needed**: If the program contains **explicit references to prototype chain features** (or other constructs that can observably affect property lookup), compile affected accesses as **late-bound** with the correct lookup behavior.
- **Future goal**: Once we have correct prototype-chain semantics in the “slow path”, add optimizations (inline caches, shape checks, specialization) to make prototype-aware code fast too.

## Terminology

- **Early-bound access**: The compiler emits direct IL to read a field/call a method on a known CLR type, with no prototype lookup and no deopt checks.
- **Late-bound access**: The compiler emits calls into runtime helpers that implement JavaScript property lookup (`Get`/`Set`/`Call`) including prototype-chain traversal.
- **Prototype-sensitive program**: A compilation unit that performs operations that may change or depend on prototype-based dispatch in ways that are observable.

## Relevant ECMA-262 Coverage Docs (in this repo)

These are the ECMA-262 areas that most directly define (or interact with) prototype-chain semantics. They’re useful as a “spec map” when implementing the runtime helpers and deciding when early-bound calls are safe.

- Ordinary object internal methods and `[[Prototype]]` mechanics: [docs/ECMA262/10/Section10_1.md](ECMA262/10/Section10_1.md)
  - Especially: 10.1.1 `[[GetPrototypeOf]]`, 10.1.2 `[[SetPrototypeOf]]`, 10.1.8 `[[Get]]`, 10.1.9 `[[Set]]`, 10.1.12 `OrdinaryObjectCreate`, 10.1.14 `GetPrototypeFromConstructor`.
- Property access, calls, and `new` (where prototype lookup becomes observable at call-sites): [docs/ECMA262/13/Section13_3.md](ECMA262/13/Section13_3.md)
  - Especially: 13.3.2 Property Accessors, 13.3.6 Function Calls, 13.3.5 `new`.
- Object constructor APIs and prototype-related hooks: [docs/ECMA262/20/Section20_1.md](ECMA262/20/Section20_1.md)
  - Especially: `Object.getPrototypeOf`, `Object.setPrototypeOf`, `Object.create`, and `Object.prototype.__proto__`.
- Function objects and the `.prototype` property used by `new`: [docs/ECMA262/20/Section20_2.md](ECMA262/20/Section20_2.md)
  - Especially: 20.2.4.3 `prototype`.
- Classes/methods (class instances use `C.prototype` for method lookup; accessors are also relevant):
  - [docs/ECMA262/15/Section15_7.md](ECMA262/15/Section15_7.md)
  - [docs/ECMA262/15/Section15_4.md](ECMA262/15/Section15_4.md)

## Proposed .NET Representation of the Prototype Chain

This section proposes runtime data structures for implementing prototype-chain semantics *when we opt into the prototype-aware path*, while keeping the current “fast path” representation viable for early-bound code.

### Design Goals

- **Don’t slow down the mainstream case**: keep current early-bound representations (e.g., `JavaScriptRuntime.Array` as a `List<object?>`, object literals as `ExpandoObject`, method calls via intrinsic dispatch) for programs that don’t exercise prototype features.
- **Have a clear, spec-shaped model available** when the compiler must produce prototype-aware code.
- **Enable future optimization** (inline caches / versioning) without requiring a full JS engine.

### Current Runtime Reality (Baseline)

Today, JS2IL uses pragmatic CLR representations:

- Object literals are typically `System.Dynamic.ExpandoObject` accessed via `JavaScriptRuntime.Object.GetProperty/SetProperty`.
- Arrays are `JavaScriptRuntime.Array` (a `List<object?>`) with index access and a few intrinsic methods.
- Functions are typically CLR delegates (plus binding helpers in `JavaScriptRuntime.Closure`); they are not full “Function objects” with a prototype chain.

This is why we can compile early-bound “fast calls” in hot paths like `tests/performance/PrimeJavaScript.js`.

### Two-Layer Model: Fast Values + Prototype-Aware Objects

We can preserve the existing model by introducing *additional* runtime types that are only required when prototype-aware lowering is selected.

#### Layer 1: Fast Path Values (unchanged)

- `JavaScriptRuntime.Array`, `Int32Array`, `string`, `double`, `bool`, `ExpandoObject`, delegates, and host objects.
- Member calls continue to use the existing dispatch (`JavaScriptRuntime.Object.CallMember`, intrinsic direct calls, etc.).

#### Layer 2: Prototype-Aware Values (new)

Introduce a small set of types/interfaces:

- `IJsObject`
  - `IJsObject? Prototype { get; set; }` (models `[[Prototype]]`)
  - `bool TryGetOwnProperty(JsPropertyKey key, out JsPropertyDescriptor desc)`
  - `bool DefineOwnProperty(JsPropertyKey key, JsPropertyDescriptor desc)`
  - Optional: `int ShapeVersion { get; }` (see versioning below)

- `IJsCallable : IJsObject`
  - `object? Call(object? thisArg, object?[] args)` (models `[[Call]]`)
  - Optional: `object? Construct(object?[] args, object? newTarget)` (models `[[Construct]]`)

Concrete types:

- `JsOrdinaryObject : IJsObject`
  - Own properties stored in a dictionary keyed by `JsPropertyKey`.
  - Has a prototype pointer.

- `JsArrayObject : IJsObject`
  - Wraps a fast `JavaScriptRuntime.Array` internally (or inherits from it) but also has an own-property bag + prototype pointer.
  - Numeric index reads/writes still go to the underlying list for speed.

- `JsFunctionObject : IJsCallable`
  - Wraps a compiled delegate + captured scopes (what we already have today).
  - Has its own properties + prototype pointer.

This keeps the “fast values” model intact while giving us a spec-shaped object graph for programs that need it.

### Property Keys and Descriptors

For prototype-correct lookup we eventually need property descriptors.

Proposed types:

- `JsPropertyKey`
  - Either `string` or `JavaScriptRuntime.Symbol` (the repo already has a `Symbol` type).

- `JsPropertyDescriptor`
  - Data: `{ Value, Writable, Enumerable, Configurable }`
  - Accessor: `{ Get, Set, Enumerable, Configurable }` where `Get/Set` are `IJsCallable` or delegate-backed wrappers.

Initial simplification (Phase 0):

- Support only *data properties* in the prototype-aware object model.
- Treat accessors as “future” but keep the representation designed for them.

### Prototype Chain Traversal

Prototype-aware `Get` can follow the spec-shaped approach:

1. Check own property (including numeric-index logic for arrays).
2. If missing, walk `Prototype` repeatedly.
3. If found:
   - For data properties: return value.
   - For accessors: call getter with `thisArg`.
4. If missing: return `undefined` (represented as CLR `null` in JS2IL).

Prototype-aware `Set`:

- If an own writable data property exists, update it.
- Otherwise, if a prototype accessor setter exists, call it.
- Otherwise, define a new own data property (subject to extensibility rules; can be simplified initially).

### Versioning for Fast, Correct Dispatch (Future-Ready)

To keep calls fast in prototype-sensitive programs, add *monotonic version counters*:

- Each `IJsObject` can have an `int ShapeVersion` incremented when:
  - an own property is added/removed/changed
  - `Prototype` changes

- Each intrinsic prototype (e.g., `Array.prototype`) can maintain its own version.

Inline cache shape:

- Cache `(receiverTypeOrShapeId, receiverShapeVersion, prototypeChainVersion, resolvedCallable)`.
- If versions match, call the cached function directly.
- If not, redo prototype-aware lookup and refresh the cache.

This aligns with the document’s earlier “guarded early-binding” direction.

## Functions and Prototypes (Runtime Model)

JavaScript has two related but distinct concepts:

- A function’s `[[Prototype]]` (its *prototype chain* as an object; typically `Function.prototype`).
- A function’s `.prototype` property (an *ordinary object* used as the `[[Prototype]]` of instances created via `new f()`).

### `JsFunctionObject` Basics

Proposed fields/properties:

- `IJsObject Prototype` (this is the function object’s `[[Prototype]]`; usually `Function.prototype`).
- `JsPropertyBag OwnProperties` (for `length`, `name`, user-assigned properties, etc.).
- `object Target` (the compiled delegate we have today).
- `object[] BoundScopes` (existing closure model).

And the callable operations:

- `Call(thisArg, args)` invokes the delegate while setting `RuntimeServices.SetCurrentThis(thisArg)`.
- `Construct(args, newTarget)` creates an instance object and then calls the function as a constructor.

### Constructor Semantics and `.prototype`

For `new F(...)` semantics, the important steps are:

1. Determine the instance’s `[[Prototype]]`:
   - If `F.prototype` is an object, use it.
   - Otherwise use the intrinsic default (e.g., `Object.prototype`).
2. Create the instance object with that prototype.
3. Invoke the constructor body with `this` bound to the instance.
4. If the constructor returns an object/function, that value overrides the instance (we already have this logic in `JavaScriptRuntime.TypeUtilities.IsConstructorReturnOverride`).

In the runtime model this implies:

- Every `JsFunctionObject` that is constructible has an own data property named `"prototype"` whose value is an `IJsObject`.
- That prototype object should have an own property `"constructor"` pointing back to the function (important for typical JS expectations).

### Classes

For `class C { m() {} }`:

- `C` is a constructible function object.
- `C.prototype` is an object containing methods like `m`.
- Instances’ `[[Prototype]]` points at `C.prototype`.

JS2IL can keep its current early-bound method calls for classes when prototype sensitivity is absent, but when prototype semantics are enabled the runtime model above gives a place for:

- adding/replacing `C.prototype.m`
- per-instance overrides `obj.m = ...`
- `Object.setPrototypeOf(obj, ...)`

### Interop With Existing CLR Delegate Model

We do **not** have to abandon delegates. Instead:

- Wrap delegates in `JsFunctionObject` only in prototype-aware mode.
- Keep using `JavaScriptRuntime.Closure.Bind/BindArrow` for scope/this mechanics.
- Teach prototype-aware `Get` to return a `JsFunctionObject` (or delegate) and prototype-aware `Call` to invoke it with the right `this`.

This provides a compatibility bridge between today’s execution model and the spec-shaped future model.

## Scope of Prototype Semantics We Care About

JavaScript property access can be affected by:

1. **Own properties** (data properties)
2. **Prototype properties** (including functions used as methods)
3. **Accessors** (`get`/`set`) on the object or any prototype
4. **Property attributes** and lookup behavior (`[[Get]]`, `[[Set]]`, `[[HasProperty]]`)
5. **Prototype mutation** after object creation

JS2IL does not need to implement every edge case on day one, but the selection logic must be conservative: if we can’t prove early-binding is safe, we must fall back to late-bound.

## Design: Tiered Compilation Modes

We will support at least two modes per compilation unit, and (eventually) per-object:

### Mode A: Fast / Early-bound (default)

Used when we can infer that prototype semantics are not being exercised in a way that could affect the compiled accesses.

Characteristics:

- Member calls on known runtime intrinsic types (e.g., `Array`) are compiled to direct calls.
- No guards/checks for prototype mutation.
- Maximizes performance for mainstream code.

### Mode B: Prototype-aware / Late-bound (opt-in by detection)

Enabled when compile-time analysis detects prototype-related mutation or reflection-like behavior.

Characteristics:

- Property read/write/call go through runtime helpers that implement the spec lookup algorithm.
- More allocations and indirections; slower but correct.
- Can be narrowed to only affected call-sites or only affected objects in later phases.

## Compile-time Detection: When to Escalate

The compiler should conservatively escalate to prototype-aware lowering when any of the following are present.

### 1) Prototype mutation

Examples:

```js
Array.prototype.indexOf = function (value) { return -1; };
Object.prototype.toString = () => "oops";
```

Detection patterns (AST-level):

- Assignment to `X.prototype.<name>`
- `Object.defineProperty(X.prototype, ...)`
- `Reflect.defineProperty(X.prototype, ...)`

### 2) Prototype reassignment / prototype chain rewiring

Examples:

```js
Object.setPrototypeOf(obj, other);
obj.__proto__ = other;
```

Detection patterns:

- Calls to `Object.setPrototypeOf`
- Member assignment to `__proto__`

### 3) Per-instance method replacement / dynamic override

Examples:

```js
let a = [1,2,3];
a.indexOf = function (value) { return value * 2; };
```

This is not “prototype mutation”, but it *does* affect property lookup and method dispatch for `a.indexOf(...)`.

Detection patterns:

- Assignment to a property on an object that is later called as a method.
- `Object.defineProperty(a, "indexOf", ...)`

Conservative strategy (Phase 1): if we see *any* assignment to a property name that is also used as a method call (e.g., `x.indexOf(...)` anywhere), consider that call-site prototype-sensitive unless we can prove it’s a different object.

### 4) Reflection-like dynamic property access

Examples:

```js
x[name]();
x["indexOf"](2);
"indexOf" in x;
for (const k in x) { ... }
```

If code relies on dynamic member names, we should assume prototype semantics matter.

Detection patterns:

- Computed member access `obj[expr]`
- `in` operator
- `for...in`
- `Object.keys/values/entries`, `Reflect.ownKeys`

### 5) `eval` / `with` (if supported)

These features make static reasoning about binding and lookup extremely hard. If they are supported in any form, they should force prototype-aware lowering for the relevant scope.

## How “Escalation” Applies: Granularity Options

We have a few choices for how broadly to apply prototype-aware lowering.

### Option 1: Global switch per compilation unit (simplest)

- If any prototype-sensitive construct exists anywhere, compile all property reads/writes/calls as late-bound.
- Pros: easiest to implement, safest.
- Cons: pessimizes unrelated hot paths.

### Option 2: Per-scope / per-function switch (better)

- Track a flag on the symbol table scope: `PrototypeSensitive`.
- If a function mutates prototypes or uses reflection-like access, mark that function (and possibly callers) as sensitive.
- Compile only accesses inside sensitive scopes as late-bound.

### Option 3: Per-call-site decision (best long-term)

- Each `MemberExpression`/call-site decides early vs late based on local proof.
- Requires alias analysis / points-to approximations.

Roadmap below recommends starting with Option 2 and growing into Option 3.

## Lowering Strategy

### Early-bound lowering (fast path)

For common intrinsics (Array/Object/String/Number/etc.), where we can emit direct IL calls:

- `a.indexOf(2)` → direct `callvirt` to the runtime intrinsic method implementation
- `a.length` → direct field/property access

This should stay as-is **when we can prove** the lookup cannot be affected.

### Late-bound lowering (prototype-aware path)

When prototype sensitivity is detected:

- `a.indexOf(2)` becomes:
  1. `GetProperty(a, "indexOf")` (prototype-aware lookup)
  2. `Call(func, thisArg: a, args: [2])`

Runtime requirements:

- A uniform “JS value” model capable of representing:
  - ordinary objects
  - arrays
  - functions
  - accessors
- Prototype traversal and property descriptor semantics.

## Compile-time Inference Heuristics (Initial)

We want inference that is:

- conservative (never early-bind when semantics could differ)
- cheap (doesn’t require complex whole-program analysis initially)

### Minimal inference (Phase 0)

- If the program contains any of the detection patterns above, mark compilation unit as prototype-sensitive.
- Otherwise, keep current early-bound behavior.

### Scoped inference (Phase 1)

- Track prototype-sensitive constructs per scope.
- Late-bind only inside those scopes.
- Additionally, late-bind call-sites for names that appear in prototype mutations:
  - if we see `Array.prototype.indexOf = ...`, then any `x.indexOf(...)` where `x` could be an Array becomes late-bound (or at least guarded).

### Toward per-call-site inference (Phase 2)

- Add lightweight points-to classification:
  - “definitely intrinsic array”
  - “definitely plain object”
  - “unknown”
- Early-bind only when receiver is “definitely intrinsic array” *and* we have not observed mutation of the relevant property name.

## Guarded Early-binding (Future Optimization)

Even in prototype-sensitive code, we can keep many calls fast by adding guards.

### Inline caches / shape checks

- Cache the resolved function and a “shape/prototype version” token.
- On call:
  - if the token matches, call the cached function (fast)
  - else, re-resolve via prototype-aware lookup and update cache

This brings performance close to early-bound for stable shapes while remaining correct.

### Prototype versioning

Maintain a version counter for:

- each intrinsic prototype (e.g., `Array.prototype`) and/or
- global prototype mutation state

Any mutation increments a version; caches become invalid.

## Interaction with JS2IL’s Architecture

Relevant compilation phases:

- **Validate**: identify unsupported prototype-related constructs early.
- **Symbol Table**: attach flags like `PrototypeSensitive` to scopes.
- **Type Generation**: remains mostly unchanged; impacts are mainly in how expressions are lowered.
- **IL Emission**: change member access/call emission based on sensitivity.

Suggested implementation points:

- Extend `JavaScriptAstValidator` to record “prototype sensitivity triggers” (not just reject/allow).
- Extend IR (or directly IL emission) to carry a `BindingMode` for member access/calls:
  - `EarlyBound`
  - `LateBound`
  - `GuardedEarlyBound` (future)

## Examples (Desired Behavior)

### Prototype mutation triggers late-bound

```js
Array.prototype.indexOf = function (value) { return -1; };
let a = [1,2,3];
console.log(a.indexOf(2));
```

- Detected: assignment to `Array.prototype.indexOf`
- Compile: `a.indexOf(2)` via prototype-aware lookup

### Per-instance override triggers late-bound for that receiver

```js
let a = [1,2,3];
a.indexOf = function (value) { return value * 2; };
console.log(a.indexOf(2));
```

- Detected: assignment to `a.indexOf`
- Compile: `a.indexOf(2)` via own-property lookup then call

### Mainstream case stays early-bound

```js
let a = [1,2,3];
a.indexOf(2);
```

- Detected: no prototype-sensitive constructs
- Compile: early-bound direct call (no lookup overhead)

## Roadmap

### Phase 0 (document + unit-level switch)

- Add prototype sensitivity detection.
- If present anywhere, lower member access/calls late-bound.

### Phase 1 (per-scope sensitivity)

- Track sensitivity per scope/function.
- Late-bind only inside sensitive scopes.

### Phase 2 (property-name targeted)

- Track mutated property names per prototype/receiver category.
- Late-bind only for those property names.

### Phase 3 (guarded fast path)

- Add inline caches / shape checks.
- Add prototype versioning invalidation.

## Open Questions

- What is our initial runtime surface for prototype-aware lookup (new helpers vs expanding existing `Operators`)?
- How do we represent accessors and property descriptors in the runtime?
- What’s the right granularity for versioning (per-prototype vs global)?
- How do we ensure deterministic behavior for tests when mixing early/late bound paths?
