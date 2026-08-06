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
lookup after phase 1, and keeps canonical plus future #737 specialized entry
points on one generated type and JavaScript identity.

## Metadata emission

After scope TypeDefs exist, but before callable body compilation, the emitter
declares each sealed subclass with:

- strongly typed readonly environment fields;
- conditional lexical/home-object/private-brand state fields;
- a constructor receiving the shared environment objects;
- `IsConstructor` and `RequiresInvocationContext` overrides;
- the common `CallCore(object, in JsCallArguments)` adapter;
- a reserved `ConstructCore(in JsCallArguments, object)` entry point only for
  constructable callables.

MethodDef rows are reserved deterministically during declaration. Bodies are
emitted after deferred scope constructors, preserving ECMA-335 TypeDef
`MethodList` monotonicity across multiple modules.

The call adapter converts generic dynamic arguments at the boundary, invokes
the existing inferred typed `__js_call__` body, and boxes typed value returns
only on return to generic JavaScript flow. Known direct calls continue to use
the canonical typed MethodDef and bypass both object materialization and the
generic adapter.

Scope-array callables currently reconstruct the existing scope-array ABI from
their typed environment fields inside the adapter. This is a compatibility
bridge for the staged family migrations, not a return to a universal closure
field: the object stores only required typed environments.

## Staged activation

This phase emits metadata and callable adapters but does not replace an
existing callable family's materialization path. In particular:

- arrows materialize generated function objects directly; see
  [Generated arrow function objects](GeneratedArrowFunctionObjects.md);
- ordinary function declarations/expressions migrate under #1712;
- ordinary and class construction semantics activate under #1713;
- methods/accessors and their home-object behavior migrate under #1714.

The reserved construction adapter therefore throws until the corresponding
family migration implements ECMAScript receiver/prototype/new-target return
processing. Its presence and JavaScript-visible return classification let
those later phases fill the predeclared boundary without changing object
shape or identity.
