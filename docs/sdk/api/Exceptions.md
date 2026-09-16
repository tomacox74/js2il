# API: Hosting exceptions

Namespace: `Jroc.Runtime`

The hosting layer translates internal exceptions at the boundary so consuming apps can catch a stable set of exceptions.

## JsRuntimeException

Base type for hosting exceptions.
Typically carries additional context such as module id, member name, contract type, and/or compiled assembly name.

## JsModuleLoadException

Thrown when a generated `Import()` or in-memory `CompileAndLoadModule(...)`
cannot load or evaluate the selected module.

Common causes:

- module id not found in the compiled assembly
- module initialization throws

## JsContractProjectionException

Thrown when `module.exports` cannot be projected onto the requested contract surface.

Common causes:

- missing export member
- export shape mismatch (expected function but got object)
- a host-defined in-memory contract does not match the runtime script

## JsInvocationException

Thrown when a call through a hosting proxy fails.

- `InnerException` carries the underlying cause.
- If the JS code threw, the inner exception is typically `JsErrorException`.

## JsScriptRunException

Thrown when a generated facade `Run(...)` call does not complete successfully.

Common causes:

- top-level or dependency evaluation throws
- rejected top-level asynchronous work or an unhandled promise rejection
- a timer, immediate, or microtask callback throws while the event loop drains
- nonzero `process.exit(...)` or `process.exitCode`

`ModuleId` and `CompiledAssemblyName` identify the selected script. `ExitCode`
contains the nonzero process exit code when applicable. For JavaScript
failures, `InnerException` is typically `JsErrorException`.

## JsErrorException

Represents a JS `Error` (or other thrown JS value) that was raised during module evaluation or invocation.

When available, it carries:

- JS error name
- JS message
- JS stack

## Compilation failures

`JrocInMemoryCompiler.Compile(...)` and `CompileAndLoadModule(...)` throw
`InvalidOperationException` when compilation fails, with compiler diagnostics
in the message. This is distinct from a successfully compiled script failing
during evaluation or invocation.

## Disposal failures

Using exports or derived values after their import or in-memory module has
been disposed throws `ObjectDisposedException`. Pending Promise bridge tasks
also fault when the owning runtime is disposed.
