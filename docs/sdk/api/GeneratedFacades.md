# API: generated script facades

Every compiled assembly exposes public static facade types for running or
importing its published scripts directly from C#. The facade is rooted at the
sanitized assembly name:

```csharp
HelloAssembly.Run();
HelloAssembly.Scripts.hello.Run();
DeepAssembly.Scripts.api.Run();
DeepAssembly.Scripts.api.css.Run("--mode", "test");
```

The root `Run` method executes the manifest entry module. Types nested below
`Scripts` represent the published module path with the file extension removed.
A module can also contain nested module types, so `api.js` and `api/css.js`
produce one `Scripts.api` type with both `Run` and nested `css`.
Compilation reports a facade-name collision when identifier normalization,
case-insensitive paths, or reserved members such as `Run` and `Import` would
make the C# surface ambiguous.

Each script exposes this C# execution method:

```csharp
public static void Run(params string[] args);
```

`Run` always returns `void`, including for modules that export values. It
creates an isolated runtime, executes the selected module, drains its
microtasks, immediates, timers, and top-level asynchronous work, then disposes
the runtime. Repeated and concurrent calls do not share globals, module cache,
`process.argv`, timers, or realm state.

The arguments are exposed with Node-compatible positioning:

```text
process.argv[0] = "jroc"
process.argv[1] = selected module id
process.argv[2..] = exact Run arguments
```

Ordering, duplicates, empty strings, whitespace, Unicode, and flag-looking
values are preserved. A null argument array or null element is rejected before
JavaScript executes.

`process.exit(0)` stops the run successfully without terminating the .NET
process. A nonzero `process.exit(...)` or `process.exitCode` throws
`JsScriptRunException`. Top-level throws, dependency failures, rejected
top-level asynchronous work, unhandled promise rejections, and drained callback
failures also throw `JsScriptRunException`; its `InnerException` carries the
translated JavaScript failure.

Generated facade signatures use only BCL types. Calling `Run` therefore does
not require C# source code to reference JROC runtime APIs. The runtime assembly
is still an implementation dependency and must be deployed beside the compiled
assembly. With `JrocCompile`, set `CopyToOutputDirectory="true"` when the host
does not otherwise reference `Jroc.Runtime`.

`Program.Main(string[] args)` uses the same root `Run` execution path. The
internal module initializer ABI and generated `Modules`, `Packages`, scope, and
callable types are not supported hosting APIs.

## Importing module exports

When compiler analysis determines that a module exports values, its facade also
exposes:

```csharp
public interface IExports : IDisposable { }
public static IExports Import();
```

No `Import` method is generated for side-effect-only modules. The assembly-root
`Import` method is generated only when the manifest entry module exports values,
and it returns the same generated contract type as
`<Assembly>.Scripts.<entry>.Import()`.

Each `IExports` interface is nested under the static facade for its module. For
example, `CommonJS_Export_Class.Scripts.CommonJS_Export_Class_Lib.Import()`
returns
`CommonJS_Export_Class.Scripts.CommonJS_Export_Class_Lib.IExports`.

The export-shape analyzer classifies every module as:

- **NoExports**: no public export surface was detected, so only `Run` is
  emitted.
- **Known**: ESM named/default exports, aliases, namespace exports, re-exports,
  or CommonJS `exports.name`, `module.exports.name`, and object-literal
  `module.exports = { ... }` assignments produce named contract members.
- **Unknown**: conditional or computed CommonJS exports, star re-exports from an
  unknown module, or incomplete inference still produce `Import` with a safe
  generated fallback contract instead of silently hiding the API.

Fallback contracts expose BCL-only members such as `Value` and, for directly
exported callables/classes, `Call(params object[] args)` and
`Construct(params object[] args)`. The returned root contract implements
`IDisposable`; disposing it shuts down the private runtime for that import.
Calls through the root contract or derived dynamic handles after disposal throw
`ObjectDisposedException`. Each `Import` call creates an isolated runtime and
module cache.

## Rich export contracts

Generated public signatures use only BCL types and types emitted into the
compiled assembly. Runtime implementation types such as `IJsHandle`,
`IJsConstructor<T>`, `JsCallable`, `Promise`, `JavaScriptRuntime`, and
`Jroc.Runtime` attributes are not part of the facade contract. Runtime metadata
needed by the proxy layer is emitted as generated implementation metadata.

Class exports produce generated constructor and instance contracts:

```csharp
using var exports = HostedCounterModule.Import();
using var counter = exports.Counter.Construct(10);

Console.WriteLine(counter.Add(5));
Console.WriteLine(counter.GetValue());
```

The constructor contract implements `IDisposable` and exposes `Construct(...)`
plus supported static members. Instance contracts implement `IDisposable` and
expose supported fields, methods, and accessors. Disposing the root exports
object shuts down the owning runtime; derived constructor/object/instance
handles then fail with `ObjectDisposedException`.

Function declarations, function expressions, object methods, and arrow
functions are emitted as C# methods where the shape is known. Simple required
parameters are emitted as ordinary `object` parameters; default/rest/destructured
parameters use a conservative `params object[]` signature so callers can supply
the same argument list JavaScript would see. Regular exported object methods are
called with the exporting object as their receiver, while arrows preserve their
lexical `this` and `arguments` semantics.

Callable return values use the generated nested `ICallable` contract, whose
`Invoke(params object[] args)` method remains bound to the owning import.
Classes returned from functions use either their generated class-specific
constructor contract or the generated `IConstructor` fallback for anonymous
class expressions.

Async functions and async arrows are projected as `Task` or `Task<T>` when the
result type can be safely inferred. Promise rejection becomes a faulted task
with module/member context; disposing the owning import while a task is pending
faults the task with `ObjectDisposedException`.

Plain exported object literals produce generated object contracts. Nested object
literals and array literals receive generated handle contracts, preserving
identity, mutation visibility, and the owning import lifetime. Generated array
contracts expose `Length`, `Get(index)`, `Set(index, value)`, `HasIndex(index)`
to distinguish sparse holes from present `undefined` values, and `Push(...)`.
Object contracts also expose `GetDynamicProperty`, `SetDynamicProperty`, and
`HasDynamicProperty` as a BCL-only fallback for computed or otherwise unknown
properties. These lookups are lazy, so aliases and cycles preserve identity
without recursively materializing an object graph.

## Iterables and generators

Synchronous generator results and values with `Symbol.iterator` project as
`IEnumerable<object>`. Async generators and `Symbol.asyncIterator` values
project as `IAsyncEnumerable<object>`. Generated object contracts implement the
same BCL interfaces when the exported object literal declares the corresponding
well-known-symbol method. Exported class instances and constructed handles also
implement those interfaces when the method is declared or inherited from a
statically known base class.

```csharp
foreach (var value in exports.Values())
{
    Console.WriteLine(value);
}

await foreach (var value in exports.AsyncValues())
{
    Console.WriteLine(value);
}
```

Each enumeration obtains a JavaScript iterator on the owning script thread.
Reusable iterables can produce independent enumerators; generator objects retain
their JavaScript single-use behavior. Disposing an enumerator before completion
invokes the iterator's `return()` method, including generator `finally`
cleanup. Async cancellation performs the same close operation before reporting
`OperationCanceledException`. Disposing the root import closes active
enumerators before shutting down the runtime. Iteration failures retain the
module, member, and JavaScript error/stack context.

Async iterator `next()` and `return()` results use JavaScript
`Promise.resolve` assimilation, so Promise values and custom thenables have the
same behavior. Root disposal requested from a callback already running on the
script thread starts asynchronous iterator cleanup without blocking that
thread, then completes shutdown after cleanup settles. Concurrent enumerator
and root disposal share one cleanup operation, so an in-flight `return()` is
never abandoned. A fulfilled `next()` value must still be an iterator-result
object; primitive results fail through the contextual exception path.

The hosting projection intentionally does not add a bidirectional
`next(value)` API. Use JavaScript code to drive that protocol when a generator
depends on values sent by its consumer.

## Built-in value contracts

Generated contracts preserve identity, mutation, and import lifetime for these
exported built-ins:

- `IDate`: time value mutation and string/ISO operations, including invalid
  dates.
- `IRegExp`: `Source`, `Flags`, mutable `LastIndex`, `Exec`, and `Test`.
  `Exec` returns the generated array contract.
- `IError`: mutable `Name`, `Message`, and `Cause`, plus `Stack`; the same
  contract represents supported Error subclasses.
- `ISymbol`: opaque `Description`, registry key, well-known name, and display
  string. Symbol values cannot be forged or converted to ordinary strings.
- `IDateConstructor`, `IRegExpConstructor`, `IErrorConstructor`, and
  `ISymbolConstructor` when those intrinsic constructor values are exported.

Constructor helper methods are looked up on the exported JavaScript constructor
at call time. Replacing properties such as `Date.now`, `Date.parse`, `Date.UTC`,
or `RegExp.escape` is therefore visible to generated-facade callers.

The same projection is used for direct/default exports, named properties,
nested objects, aliases, and function return values. Invalid date ISO
conversion, invalid regular-expression construction, and unsupported
operations fail explicitly through the contextual hosting exception path.

## Collections

`IMap` and `ISet` expose size, lookup/mutation, and ordered BCL enumeration.
Map enumeration yields generated `IMapEntry` handles rather than converting to
`Dictionary`, so JavaScript key equality and object identity remain intact.
`IWeakMap` and `IWeakSet` expose only `get`/`has`/`set`/`add`/`delete` operations;
they deliberately do not implement enumeration or expose storage.

Object keys and values remain runtime-owned handles. Passing a handle back into
the same import unwraps the original JavaScript object; crossing import
instances is rejected. Object values reached through `Map.Get`, `WeakMap.Get`,
entry/key/value iteration, or a direct generated object export share the
runtime's canonical generated handle whenever their projection contracts are
compatible. Callable keys and values use the same rule rather than switching to
a separate dynamic callable wrapper.

## Binary values

`IArrayBuffer` represents both `ArrayBuffer` and the currently supported
`SharedArrayBuffer`; `IsShared` distinguishes them. It exposes byte length,
maximum length/resizability, copying `Slice`, and supported resize behavior.
`IDataView` preserves its buffer, byte offset/length, endianness, and all
currently implemented numeric accessors. `ITypedArray` preserves its concrete
`Kind`, buffer, offset, byte/element lengths, bytes per element, indexed reads,
ordered iteration, and mutation.

The supported typed-array kinds are `Int8Array`, `Uint8Array`,
`Uint8ClampedArray`, `Int16Array`, `Uint16Array`, `Int32Array`, `Uint32Array`,
`Float32Array`, and `Float64Array`. Multiple projected views continue to share
the JavaScript backing store, so mutations are immediately visible across
views. Host indexes outside the current typed-array view fail with
`ArgumentOutOfRangeException`; invalid `DataView` access uses the contextual
JavaScript exception path.

Slicing a `SharedArrayBuffer` produces another shared buffer with copied,
independent contents; it does not silently downgrade the result to an ordinary
`ArrayBuffer`.

JROC does not currently implement ArrayBuffer detachment, BigInt typed arrays,
typed-array `slice`/`subarray`, or growable `SharedArrayBuffer`. Generated
contracts do not claim those operations. Fixed buffers reject `Resize`
explicitly, and `SharedArrayBuffer` is exposed only for the runtime behavior
JROC currently executes.
