# Tutorial: Typed hosting

Typed hosting is the recommended UX when you control the compilation step (or you distribute a compiled module assembly that already contains contracts).

## Key idea

- JS2IL emits a generated exports contract interface into the compiled module assembly.
- The interface is annotated with `[JsModule("<moduleId>")]`.
- `JsEngine.LoadModule<TExports>()` uses that attribute to load the module with **no module id argument**.

## Generated contract naming

Contracts follow these conventions (see the generator for the authoritative rules):

- Root namespace: `Js2IL.<AssemblyName>`
- Entry module exports interface: `I<AssemblyName>Exports`
  - Example: assembly `HostedCounterModule.dll` → `Js2IL.HostedCounterModule.IHostedCounterModuleExports`
- Non-entry modules:
  - Namespace includes path segments (PascalCase)
  - Interface name is `I<DisplayName>Exports` (`index` maps to the parent folder name)

## Example module

```js
class Counter {
  constructor(start) { this._value = start; }
  add(delta) { this._value += delta; return this._value; }
  getValue() { return this._value; }
}

async function addAsync(x, y) {
  return x + y;
}

module.exports = {
  version: "1.2.3",
  add: (x, y) => x + y,
  addAsync,
  Counter,
  createCounter: (start) => new Counter(start),
};
```

## Calling it from C#

```csharp
using Js2IL.Runtime;
using Js2IL.HostedCounterModule;

using var exports = JsEngine.LoadModule<IHostedCounterModuleExports>();

Console.WriteLine(exports.Version);
Console.WriteLine(exports.Add(1, 2));

// Exported class → IJsConstructor<T>
using var counter = exports.Counter.Construct(10);
Console.WriteLine(counter.Add(5));

// Async export returns a Promise at runtime → projected as Task in C#
var sum = await exports.AddAsync(1d, 2d);
Console.WriteLine(sum);
```

## How name matching works

Generated contracts are idiomatic C# (PascalCase), but JavaScript exports are usually camelCase.
At runtime, the hosting layer resolves members using these candidates:

- exact member name (`Version`)
- first-letter-lowercased (`version`)

This is why `exports.Version` maps to `module.exports.version`.

## Mutable exports

Typed hosting supports both export reads and export writes:

- `get_Foo` reads `module.exports.foo`
- `set_Foo(value)` writes back through to `module.exports.foo`

Both paths are marshalled onto the owning script thread, so host-side mutation stays thread-affine with the rest of the runtime.

Name matching for setters follows the same contract-to-JavaScript rules as getters:

- exact member name (`MutableValue`)
- first-letter-lowercased (`mutableValue`)
