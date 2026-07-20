# Object Literal Type Inference

Status: shipped (phases 1–4), tracked by #1428.

JROC infers a specialized CLR type for object literals whose usage is provably
"stable" and compiles construction and property access against that type instead
of the generic `JsObject` dictionary path. The optimization is fully transparent:
any literal that could observe a behavioral difference is left on the generic
path, and even specialized literals keep their `JsObject` base storage in sync so
every dynamic runtime path still sees correct values.

## Phases

| Phase | Issue | What it does |
| --- | --- | --- |
| 1 | #1429 | `SymbolTableBuilder` shape analysis: records an `ObjectLiteralShapeInfo` on bindings declared with an object-literal initializer and conservatively proves eligibility. |
| 2 | #1430 | `TypeGenerator` emits a CLR type per eligible shape, nested under the module type as `Modules.<Module>/ObjectLiterals/L{line}C{col}_{binding}`, deriving from `JsObject`. |
| 3 | #1431 | Lowering constructs eligible literals via the generated constructor and per-member typed accessors instead of `CreateObjectLiteral()`. |
| 4 | #1432 | Property reads and writes on eligible bindings early-bind to the generated `get_<name>`/`set_<name>` accessors (`castclass` + `callvirt`) instead of `ObjectRuntime.GetItem`/`SetItem`. |
| 5 | #1433 | Validation, benchmarks, documentation (this document). |
| 6 | #1434 | Interprocedural propagation: a literal passed into a closed-world callable lets the receiving parameter reuse the generated specialized type, so member reads on the parameter early-bind through the inferred-member path. Uses a monotone, capless fixed point (cascading invalidation) so multi-hop chains and mutual recursion converge; structurally identical literals canonicalize onto one shared CLR type and can join at a parameter. |

## Generated type shape

For `const eligible = { text: "hello", n: 42 };` the compiler emits:

```
class Modules.<Module>/ObjectLiterals/L1C18_eligible : JsObject
{
    private object  _text;
    private float64 _n;

    object  get_text()          { return _text; }
    void    set_text(object v)  { _text = v; base.SetObject("text", v); }
    float64 get_n()             { return _n; }
    void    set_n(float64 v)    { _n = v; base.SetNumber("n", v); }
}
```

- Members with a provably stable primitive initializer get a typed backing field
  (`float64` for numbers, `object` otherwise); reads of `float64` members stay
  unboxed through arithmetic.
- Every setter **mirrors** the value into the base `JsObject` string-keyed
  storage. This keeps all dynamic paths correct: member calls (`obj.fn()`),
  generic-path reads (e.g. destructuring reads), and any access from bindings
  that do not qualify for early binding. Eliding the mirror for fully
  early-bound shapes is a follow-up (#1439).

## Eligibility (phase 1)

A shape is eligible only if the literal itself is well-formed **and** every use
of the binding is provably safe. Anything else disqualifies the shape with a
recorded reason (`ObjectLiteralShapeInfo.Disqualify`).

Literal-level disqualifiers:

- no members, spread elements, getter/setter members
- computed, non-identifier, duplicate, or `__proto__` keys

Use-level disqualifiers (conservative whitelist — anything unrecognized also
disqualifies):

- binding reassigned, aliased to another binding, exported, or `delete` applied
- object returned from a function, stored into another object/array, or stored
  through any assignment (escape)
- object passed to a call, **unless** every receiving callable is closed-world
  (only ever referenced as a direct call/`new` target) and every use of the
  corresponding parameter is itself safe — phase 6 (#1434) then propagates the
  shape into the parameter instead of disqualifying. A closed-world simple
  object-destructuring parameter may also consume the shape without making the
  object escape; destructured bindings receive stable primitive types when all
  call sites agree on the corresponding literal member type. Spread arguments,
  missing arguments, escaping/aliased callables, conflicting shapes at a shared
  parameter, and any unsafe parameter use still fall back to the generic path.
- object used in a spread, enumerated by `for-in`/`for-of`, or used with `in`
- computed / non-identifier member access, access to an undeclared member,
  `delete` of a member
- member used as a destructuring or iteration assignment target
  (`[obj.p] = arr`, `({ x: obj.p } = src)`, `for (obj.p of arr)`)
- call of a non-function member, or a method call whose body (including nested
  functions) contains `this` (arrow members are exempt: they capture lexical
  `this`)

Notably **not** disqualifying:

- closure capture: nested functions may read and write members of a captured
  eligible binding; those accesses early-bind like any other
  (`ObjectLiteral_Inference_ClosureAndAliasing_Parity` shows `castclass` +
  `callvirt` inside the closures)
- calling arrow-function members

Because reflective operations (`Object.keys`, `JSON.stringify`,
`Object.getOwnPropertyDescriptor`, `defineProperty`, `freeze`, `seal`, …)
pass the binding to an unknown (non-closed-world) call, they inherently
disqualify the shape, so descriptor/enumeration semantics never need to be
replicated by the generated type.

## Interprocedural propagation (phase 6, #1434)

When a literal is passed as a positional argument to a *closed-world* callable
(a function/const-bound function expression whose binding is only ever the
callee of a direct call/`new`), the analysis may propagate the shape into the
receiving parameter instead of disqualifying. This is a coupled, monotone fixed
point over parameter *slots* and literal eligibility, iterated to convergence
with **no** iteration cap (the lattice height is finite, so long multi-hop
chains `a → c → d → …` and mutual recursion terminate deterministically).

Structural canonicalization: the shape's structural signature is normalized by
member name (names + member CLR types + function-ness), so two literals that
declare the same members in a **different source order** share one generated
CLR type and can join at a shared parameter. Each literal still constructs and
enumerates in its own source order — only the shared type's field layout is
canonical.

Parameter-use safety mirrors the per-binding rules and is checked once the
representative shape is known:

- **Member reads** early-bind exactly like reads on the original binding.
- **Same-type member writes/updates** (`o.p = <same-type>`, `o.p++`) are safe
  and lower through the generated setter, keeping the typed backing field in
  sync. A write whose value type does not provably match the member's field
  type conservatively demotes the slot and disqualifies the feeding literal.
- **Method calls through the parameter** (`o.fn()`) are conservatively out of
  scope: they pass the object as `this` into a callee this pass does not model,
  so they are treated as an escaping use.
- **Spread/missing arguments** make positional mapping unstable: any literal or
  forwarded parameter at or after the first spread argument in a call is not
  accepted as stable evidence and falls back to the generic path.
- Conflicting shapes at a shared parameter, escaping/aliased callables, and any
  unrecognized parameter use fall back to the generic path.

The parameter ABI stays `object`; only the stable shape token is propagated so
lowering can early-bind member access on the parameter.

Object-literal analysis participates in the compiler's global type-inference
fixed point. Variable, definite-initialization, class-field, callable-parameter,
callable-return, and object-shape inference repeat until their persisted facts
stop changing. This lets a destructured member type feed downstream callables
through arbitrarily long dependency chains without relying on a hard-coded
number of pass repetitions. Repeated states are detected as a compiler error
rather than allowing a non-converging inference cycle.

For a simple object-destructuring parameter such as
`function run({ size, seconds })`, the incoming ABI also remains `object`.
When every direct call passes an eligible object-literal binding and its
`size`/`seconds` members have the same stable primitive types, the extracted
bindings use those types directly. The literal remains eligible because the
destructuring operation reads its mirrored ordinary-object properties without
letting the object escape. Computed, nested, defaulted, or rest patterns remain
conservative fallbacks.

## Early-bound access (phase 4)

Reads/writes lower to `LIRGetInferredMember`/`LIRSetInferredMember` only when:

- the receiver is a variable whose binding has an eligible shape with a
  generated type,
- the member is declared in the shape, and
- the binding is `const` or `let` (a `var` binding can be observed as
  `undefined` before initialization; the generic path produces the correct
  `TypeError` there, whereas a `castclass` would throw `InvalidCastException`).

All write forms are intercepted — simple, compound (`+=`, `-=`, …), `??=`, and
`++`/`--` — so a specialized binding's writes always flow through the mirroring
setter and the typed backing fields can never go stale. Method calls
(`obj.fn()`) intentionally stay on the dynamic member-call path and are served
by the mirror.

## Remaining conservative exclusions

- `var`-bound literals: specialized construction only; access stays dynamic.
- Member method calls are not early-bound (candidate for a later phase).
- Members without a stable primitive type use `object` fields (boxed).
- The `JsObject` mirror write in every setter and at construction is retained
  even when nothing can observe it (#1439 tracks eliding it).
- Any use context the analyzer does not recognize disqualifies the shape.

## Measured impact (local, Windows dev box, .NET 10)

Object-literal read microbenchmark (1000 literals × 20k iterations of
`n.x + n.y + n.f + n.g`, all-double members; jroc Release):

| Variant | Wall time | Allocations |
| --- | --- | --- |
| Eligible (early-bound) | ~1.9–2.0 s | 2.41 GB |
| Escaped (dynamic path) | ~9.5–9.8 s | 4.33 GB |

≈ **5× faster** and **44% fewer allocations** for literal-heavy read loops.
Identical computed results confirm behavioral parity.

Kraken `ai-astar` attribution note: its central `var astar = { … }` namespace
literal does receive a specialized type, but it is constructed exactly once and
is `var`-bound with method-invoked members, so its accesses stay on the dynamic
path and this feature contributes little there; the constructor-created
`GraphNode` objects that dominate `ai-astar` are covered by the
constructor-shape work (#1426). Direct measured run of the compiled scenario:
~2.9–3.8 s wall, ~70.6 MB allocated.

## Test coverage

- `tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_InferredConstruction_Parity.js` — construction parity, descriptor/enumeration fallback
- `tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_EarlyBoundAccess_Parity.js` — all read/write forms, fallback parity, destructuring-target disqualification
- `tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_Inference_EnumerationAndJson_Parity.js` — enumeration order, `Object.keys/values/entries`, integer-key ordering, `JSON.stringify`
- `tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_Inference_DescriptorAndMutation_Parity.js` — `getOwnPropertyDescriptor`, `defineProperty`, `delete`, `freeze`, `seal`, `in`
- `tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_Inference_ClosureAndAliasing_Parity.js` — aliasing/escape disqualification, closure capture (early-bound)
- `tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_InterproceduralInference_Parity.js` — phase 6 interprocedural propagation: direct/multi-hop/mutual-recursion propagation, reordered same-shape join, safe parameter member write/read, incompatible/spread-shift fallback
- `tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_DestructuredParameterInference_Parity.js` — config-style literal propagation into a simple object-destructuring parameter with unboxed numeric bindings
- `tests/Jroc.Tests/SymbolTable/ObjectLiteralShapeAnalysisTests.cs` — phase 6 slot analysis: propagation, canonicalization (order-insensitive signature), safe/conflicting parameter writes, method-call and spread-shift fallback
- `tests/Jroc.Tests/ObjectLiteralTypeGenerationGroundworkTests.cs` — generated type metadata contract, reordered same-shape literals sharing one generated type
