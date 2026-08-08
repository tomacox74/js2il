# Instruction chaining

Instruction chaining lets JROC pass a value directly from one generated IL
instruction to the next instead of storing that value in a temporary local and
loading it again. It is an IL-backend optimization: it changes neither the
JavaScript operation order nor the values the program can observe.

## Part 1: a simple illustration

Consider installing a generated class method:

```js
class Counter {
    increment() {}
}
```

The compiler creates a concrete function-object wrapper, gives that wrapper
its JavaScript function metadata, marks its `prototype` as `undefined`, and
installs it under `"increment"`.

Without chaining, each intermediate result goes through a CLR local:

```text
create wrapper
  -> local method
initialize method
  -> local initializedMethod
mark initializedMethod as non-constructible
  -> local callable
install "increment" with callable
```

The equivalent IL shape is:

```il
newobj FunctionObject_Counter_increment::.ctor()
call !!T Function::InitializeFunctionInstance<T>(!!T, ...)
stloc methodObject

ldloc methodObject
call !!T Function::MarkUndefinedPrototype<T>(!!T)
stloc markedMethodObject

ldloc prototype
ldstr "increment"
ldloc markedMethodObject
call ObjectRuntime::DefineClassElementDataProperty
pop
```

With instruction chaining, the wrapper result stays on the CLR evaluation
stack until property installation consumes it:

```text
load target and key
  -> create wrapper
  -> initialize wrapper
  -> mark wrapper as non-constructible
  -> install property
```

```il
ldloc prototype
ldstr "increment"
newobj FunctionObject_Counter_increment::.ctor()
call !!T Function::InitializeFunctionInstance<T>(!!T, ...)
call !!T Function::MarkUndefinedPrototype<T>(!!T)
call ObjectRuntime::DefineClassElementDataProperty
pop
```

Both versions create exactly one function object and install the same object.
The chained form removes the short-lived typed local and its `stloc`/`ldloc`
traffic while retaining `FunctionObject_Counter_increment` as `T` through the
generic runtime calls.

## Part 2: scheduler and emission details

### Where chaining happens

`LIRStackScheduler` runs after LIR normalization and before local allocation
and IL emission. It is the owner of non-identity instruction ordering and
residency decisions:

```text
normalized MethodBodyIR
  -> LIRStackScheduler
  -> TempMaterializationPlan
  -> TempLocalAllocator
  -> LIRToILCompiler
```

For generated callable installation, the `GeneralRegions` mode recognizes
these consumer roots:

- `ObjectRuntime.DefineClassElementDataProperty`
- `ObjectRuntime.DefineClassElementAccessorProperty`
- `ObjectRuntime.DefineObjectLiteralDataProperty`
- `ObjectRuntime.DefineObjectLiteralAccessorProperty`

Starting at a callable operand of one of those roots, it walks backwards
through a contiguous producer chain:

```text
LIRCreateBoundArrowFunction
  or LIRCreateBoundFunctionExpression
  -> Function.MarkUndefinedPrototype<T>
  -> optional Function.SetAccessorNameIfAnonymous
  -> descriptor installation
```

The scheduler moves the descriptor-installation operation before that
contiguous producer suffix. Its ordinary argument emission then loads the
target and key first, followed by the callable chain as the final value
argument. This is the CLR stack order required by an installation call such
as `DefineClassElementDataProperty(target, key, value)`.

### Eligibility rules

The optimization is deliberately narrow. Every callable-chain result must:

1. have exactly one LIR definition and one use;
2. be defined immediately before its consumer in the chain;
3. stay within one scheduler region;
4. have no assigned variable-slot storage; and
5. terminate at one of the supported descriptor-installation roots.

The scheduler rejects the chain when any condition fails. It then leaves the
original source-order operations and materialized locals unchanged. This
covers callable values that are reused, cross sequence points or control-flow
boundaries, or need storage for a different backend feature.

Only the callable-result chain is moved. Captured scope arrays, home objects,
private brands, object-literal targets, computed keys, and the missing
getter/setter value remain normal materialized inputs. Therefore:

- computed keys still run before their method/accessor values;
- capture reads and allocation happen once at their original evaluation point;
- abrupt completion from a target, key, capture, constructor, initialization,
  naming, or descriptor operation retains its original relative order; and
- descriptor installation still receives the exact generated function object.

For object-literal accessors, the lowerer creates the missing getter or setter
before lowering the generated callable chain. That places the non-callable
argument on the evaluation stack before the callable argument and avoids
moving an observable operation across callable construction.

### Ownership and validation

The schedule records every selected producer as
`TempResidency.ScheduledInline` and marks it scheduler-owned.
`TempMaterializationPlan` respects that ownership, so it cannot independently
rematerialize or allocate the same temporary. `LIRStackScheduleValidator`
checks def/use counts, supported producer/root shapes, region ownership, and
the resulting stack behavior before IL emission.

`LIRToILCompiler` skips a scheduled-inline definition at its original LIR
position. When the reordered root emits its arguments, loading the callable
temp recursively emits the concrete wrapper construction and generic runtime
calls inline. No wrapper value is widened to `object` merely to avoid a local.

This is distinct from rematerialization. Rematerialization suppresses a cheap,
stable producer and reproduces it at a use; instruction chaining emits an
allocation or call exactly once and carries its result to the consumer on the
evaluation stack.

## Related code and tests

- `src/Compiler/IL/LIRStackScheduler.cs` selects and reorders eligible chains.
- `src/Compiler/IL/LIRStackScheduleValidator.cs` validates scheduler ownership.
- `src/Compiler/IL/LIRToILCompiler.GeneratedFunctionObjects.cs` emits the
  concrete generated wrapper and generic initialization calls.
- `src/Compiler/IR/LIR/HIRToLIRLowerer.Lowering.Expressions.ClassDefinitions.cs`
  and `...ArrayObject.cs` preserve accessor argument order during lowering.
- `tests/Jroc.Tests/Classes/JavaScript/Classes_GeneratedMethodFunctionObjects.js`
  and `tests/Jroc.Tests/Object/JavaScript/ObjectLiteral_GeneratedMethodFunctionObjects.js`
  cover class and object-literal method/object identity behavior.

For the scheduler's broader model, modes, and validation boundaries, see
[LIR stack scheduler](LIRStackScheduler.md).
