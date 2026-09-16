# API: JavaScript and CLR type mapping

These projections apply to generated imports from MSBuild-compiled assemblies
and to values returned by the in-memory compilation workflow.

## Primitive values

| JavaScript value | CLR value in hosting |
|---|---|
| `number` | `double` (the runtime represents numbers as `System.Double`) |
| `string` | `string` |
| `boolean` | `bool` |
| `undefined` | `null` |
| `null` | An opaque dynamic proxy when projected as `object`; `null` when a more specific generated reference contract permits no value |

## Objects / functions

MSBuild-generated contracts:

- Known exported JavaScript functions and arrows are generated as C# methods.
- Exported classes are generated as constructor contracts with `Construct(...)`.
- Exported object, array, constructor, and instance contracts implement
  `IDisposable`; they do not expose runtime marker interfaces.
- When analysis cannot determine a narrower public type, a signature may use
  `object` or a generated fallback contract. A strongly typed facade does not
  imply every JavaScript parameter and return value has a concrete CLR type.
- Generator and custom iterable results use `IEnumerable<object>`; async
  generator and async iterable results use `IAsyncEnumerable<object>`.
- Date, RegExp, Error, Symbol, Map/Set/weak collections, ArrayBuffer,
  SharedArrayBuffer, DataView, and supported typed arrays use generated
  `IDisposable` contracts. Their nested values use the same projection rules.

In-memory module exports:

- Primitives are returned as normal CLR values.
- Function values are returned as stable `JsCallable` wrappers.
- Other non-primitive values are wrapped in a dynamic proxy so that:
  - member access (`obj.foo`)
  - member invocation (`obj.foo(1,2)`)
  - invocation of function values (`fn(123)`)

  are all marshalled onto the script thread and use JS calling conventions.

## Async

- A JS `Promise` can be projected as `Task`/`Task<T>`.
- Generated async functions/arrows use `Task`/`Task<T>` directly.
- `JsCallable.CallAsync<T>(...)` bridges a callable's Promise result to
  `Task<T>`.
- If the contract expects `Task<T>` and the JS side returns a non-promise value, it is treated as an already-completed task with that value.
- A CLR delegate callback result of type `Task`/`Task<T>`
  is exposed to JavaScript as a Promise. Faults and cancellation reject it.
- Disposing the owning runtime faults still-pending Promise-to-Task bridges
  with `ObjectDisposedException`.
