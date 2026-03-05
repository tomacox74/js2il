# API: Hosting exceptions

Namespace: `Js2IL.Runtime`

The hosting layer translates internal exceptions at the boundary so consuming apps can catch a stable set of exceptions.

## JsRuntimeException

Base type for hosting exceptions.
Typically carries additional context such as module id, member name, contract type, and/or compiled assembly name.

## JsModuleLoadException

Thrown when a module cannot be loaded/evaluated via `JsEngine.LoadModule(...)`.

Common causes:

- module id not found in the compiled assembly
- module initialization throws

## JsContractProjectionException

Thrown when `module.exports` cannot be projected onto the requested contract surface.

Common causes:

- missing export member
- export shape mismatch (expected function but got object)
- contract type missing required metadata (e.g., `[JsModule]` for the no-args overload)

## JsInvocationException

Thrown when a call through a hosting proxy fails.

- `InnerException` carries the underlying cause.
- If the JS code threw, the inner exception is typically `JsErrorException`.

## JsErrorException

Represents a JS `Error` (or other thrown JS value) that was raised during module evaluation or invocation.

When available, it carries:

- JS error name
- JS message
- JS stack
