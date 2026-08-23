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
public static <generated exports contract> Import();
```

No `Import` method is generated for side-effect-only modules. The assembly-root
`Import` method is generated only when the manifest entry module exports values,
and it returns the same generated contract type as
`<Assembly>.Scripts.<entry>.Import()`.

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
