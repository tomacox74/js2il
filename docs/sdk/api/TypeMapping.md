# API: JS ↔ CLR type mapping (hosting)

This is the practical mapping you will see when hosting compiled JS from C#.

## Primitive values

| JavaScript value | CLR value in hosting |
|---|---|
| `number` | `double` (the runtime represents numbers as `System.Double`) |
| `string` | `string` |
| `boolean` | `bool` |
| `undefined` | `null` |
| `null` | `JavaScriptRuntime.JsNull.Null` (runtime sentinel) |

## Objects / functions

Typed hosting:

- JavaScript callable values are returned as `JsCallable`.
- If your contract return type is `IJsHandle`, the hosting layer returns a handle proxy.
- If your contract return type is `IJsConstructor<T>`, the hosting layer returns a constructor proxy.
- If your contract return type is `object`, runtime references remain behind a
  dynamic proxy or `JsCallable`; raw generated/runtime object types are not
  exposed.

Dynamic hosting:

- Primitives are returned as normal CLR values.
- Function values are returned as stable `JsCallable` wrappers.
- Other non-primitive values are wrapped in a dynamic proxy so that:
  - member access (`obj.foo`)
  - member invocation (`obj.foo(1,2)`)
  - invocation of function values (`fn(123)`)

  are all marshalled onto the script thread and use JS calling conventions.

## Async

- A JS `Promise` can be projected as `Task`/`Task<T>`.
- `JsCallable.CallAsync<T>(...)` bridges a callable's Promise result to
  `Task<T>`.
- If the contract expects `Task<T>` and the JS side returns a non-promise value, it is treated as an already-completed task with that value.
- A CLR delegate or `JsHostFunction` callback result of type `Task`/`Task<T>`
  is exposed to JavaScript as a Promise. Faults and cancellation reject it.
- Disposing the owning runtime faults still-pending Promise-to-Task bridges
  with `ObjectDisposedException`.
