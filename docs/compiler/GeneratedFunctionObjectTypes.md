# Generated function-object types

The two-phase compiler plans one canonical, callee-shaped
`JsFunctionObject` subclass for every materializable compiled callable.
Planning is keyed by `CallableId`; call sites do not create wrapper classes or
change the generated object layout.

## Planning

`GeneratedFunctionObjectPlanner` runs with callable discovery and records:

- the existing inferred typed callable signature and canonical MethodDef;
- a deterministic module-qualified generated type name;
- only the typed parent environment references actually required by the
  callable;
- lexical `this`, lexical `new.target`, home-object, and private-brand fields
  only when the callable's own body requires them;
- constructability and whether ambient invocation context is required;
- the JavaScript-visible return family: ordinary value, constructor, Promise,
  generator, or async generator.

`GeneratedFunctionObjectRegistry` is the canonical `CallableId`-keyed store.
It preserves discovery order, rejects inconsistent replanning, supports strict
lookup after phase 1, and keeps each canonical typed entry point and its
generic JavaScript adapter on one generated type and JavaScript identity.

## Closed-world parameter specialization

Callsite inference can specialize simple parameters on module-local function
declarations, arrows, function expressions, class methods, and constructors.
A parameter receives a typed CLR ABI only when every visible direct call or
construction site supplies the same supported stable type and the callable
does not escape. Numeric callsites therefore use `double` parameters directly
instead of boxing arguments and coercing them again in the callee.

Exports, aliases, value uses, computed or unresolved calls, spread arguments,
conflicting argument types, default/rest/`arguments` requirements, and unsafe
parameter writes retain the generic `object` ABI. Inference is positional, so
an uncertain parameter does not prevent independent safe positions from being
specialized. Recursive cycles without sufficient external type evidence also
remain generic.

## Metadata emission

After scope TypeDefs exist, but before callable body compilation, the emitter
declares each sealed subclass with:

- strongly typed readonly environment fields;
- conditional lexical/home-object/private-brand state fields;
- a constructor receiving the shared environment objects;
- `IsConstructor` and `RequiresInvocationContext` overrides;
- the common `CallCore(object, in JsCallArguments)` adapter for callable
  families, while class constructors inherit the specification-required
  throwing implementation from `JsClassConstructorObject`;
- a static
  `__js_call_with_arguments__(JsFunctionObject, object[], object[])` adapter
  only for spread or argument-sensitive direct boundaries that already own an
  array;
- a `ConstructCore(in JsCallArguments, object)` entry point for constructable
  ordinary functions. Generated class constructors inherit the shared
  class-construction implementation from `JsClassConstructorObject`.

MethodDef rows are reserved deterministically during declaration. Bodies are
emitted after deferred scope constructors, preserving ECMA-335 TypeDef
`MethodList` monotonicity across multiple modules.

The call adapter converts generic dynamic arguments at the boundary, invokes
the existing inferred typed `__js_call__` body, and boxes typed value returns
only on return to generic JavaScript flow. Known direct calls continue to use
the canonical typed MethodDef and bypass both object materialization and the
generic adapter.

Dynamic variable, expression-valued, named-member, computed-member, and
construction sites share the `JsCallArguments` common-arity policy. Calls and
`new` with zero through five non-spread arguments use fixed runtime entry
points and do not materialize an argument array. Spread calls and arities above
five use the array ABI so evaluation order and arbitrary argument counts remain
unchanged.

Scope-array callables currently reconstruct the existing scope-array ABI from
their typed environment fields inside the adapter. This is a compatibility
bridge for the staged family migrations, not a return to a universal closure
field: the object stores only required typed environments.

Class declarations and expressions materialize their planned generated
constructor wrapper directly. The wrapper derives from
`JsClassConstructorObject`, retains the class type, captured scopes, and
formal length metadata, and participates in the same `CallableOperations`
call/construct path as every other `JsFunctionObject`. Declarations preserve a
stable cached identity; each evaluation of a class expression receives a fresh
identity. The shared base rejects calls without `new` and centralizes dynamic
construction while the generated canonical class constructor MethodDef
continues to implement the typed constructor body.

## Retired delegate materialization

Every materialized compiled callable family now uses its generated object:
ordinary functions, arrows, methods/accessors, class constructors, async
functions, generators, and async generators. Direct exact-arity calls still
target the canonical typed MethodDef. Spread, `arguments`, and rest-sensitive
direct calls use the generated array adapter rather than constructing a
temporary CLR delegate.
The call site supplies the actual generated function object so the adapter
preserves `arguments.callee` identity and installs/restores `this`,
`new.target`, lexical `super`, arguments, and callee state around the canonical
typed MethodDef call. A proven direct-only rest-parameter callable has no
observable function value; that path passes no object and installs only the
argument state required to form the rest binding.

The only compiler-emitted delegates are allowlisted boundaries:

- `ModuleMainDelegate` and `RequireDelegate` for CommonJS bootstrap;
- runtime-owned built-in/host adapters;
- resumable step delegates immediately enclosed by
  `CompiledContinuation`, which is not a JavaScript callable.

## Contributor guardrails

Do not add AST-dependent semantics to LIR or callable IL emission.
`CallableSemantics` is established during callable discovery/AST-to-HIR
construction and carries function length, async/generator family,
constructability, legacy function properties, named-expression status, and
lexical `this`/`new.target`/`super`/private-name requirements. Class method and
constructor lookup uses the phase-one `ClassSemantics` descriptor. Callable
identity, capture, `new.target`, default/rest/`arguments`, and materialization
decisions must be established before LIR emission. Preserve diagnostics with
`SourceLocation` values, not AST references.

`CallableBoundaryInventoryTests` rejects AST dependencies in LIR/IL emission,
retired binders, new compiler
`ldftn` sites outside the bootstrap/built-in/continuation allowlist,
Delegate-typed generated storage, and scattered runtime Delegate checks.
`GeneratedFunctionObjectEmissionTests` separately verifies typed canonical
signatures, object-backed family materialization, and array-free common-arity
adapters.
