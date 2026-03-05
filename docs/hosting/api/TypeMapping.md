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

- If your contract return type is `IJsHandle`, the hosting layer returns a handle proxy.
- If your contract return type is `IJsConstructor<T>`, the hosting layer returns a constructor proxy.
- If your contract return type is `object`, you will typically get the raw runtime object.

Dynamic hosting:

- Primitives are returned as normal CLR values.
- Most non-primitive values (including delegates/functions and objects) are wrapped in a dynamic proxy so that:
  - member access (`obj.foo`)
  - member invocation (`obj.foo(1,2)`)
  - invocation of function values (`fn(123)`)

  are all marshalled onto the script thread and use JS calling conventions.

## Async

- A JS `Promise` can be projected as `Task`/`Task<T>`.
- If the contract expects `Task<T>` and the JS side returns a non-promise value, it is treated as an already-completed task with that value.
