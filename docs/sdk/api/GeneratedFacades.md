# API: generated script facades

Every compiled assembly exposes public static facade types for running its
published scripts directly from C#. The facade is rooted at the sanitized
assembly name:

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

Each script method has this C# shape:

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
