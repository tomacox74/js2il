# Tutorial: Typed hosting

Typed hosting is the recommended UX when you control the compilation step (or you distribute a compiled module assembly that already contains contracts).

## Key idea

- JROC emits a generated exports contract interface into the compiled module assembly.
- The facade's `Import()` method returns that contract directly.
- Runtime metadata is generated into the compiled assembly; consumer source does
  not need `Jroc.Runtime` types.

## Generated contract naming

Contracts follow these conventions (see the generator for the authoritative rules):

- Each module's static facade contains a nested `IExports` interface.
- Example: assembly `HostedCounterModule.dll`, entry module `counter.js` →
  `HostedCounterModule.Scripts.counter.IExports`.
- `HostedCounterModule.Import()` and
  `HostedCounterModule.Scripts.counter.Import()` return that same interface.

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
using var exports = HostedCounterModule.Import();

Console.WriteLine(exports.Version);
Console.WriteLine(exports.Add(1, 2));

using var counter = exports.Counter.Construct(10);
Console.WriteLine(counter.Add(5));

// Async export returns a Promise at runtime → projected as Task/Task<T> in C#
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

## Classes, objects, arrays, and async calls

Exported classes are surfaced as generated constructor contracts. Constructed
instances and nested object/array handles are also generated contracts that
implement `IDisposable`; they do not inherit from `IJsHandle`, and constructor
properties do not use `IJsConstructor<T>`.

Object literal contracts expose properties, methods, and accessors with the
correct JavaScript receiver. Array contracts expose `Length`, `Get`, `Set`,
`HasIndex`, and `Push`, including sparse-array hole checks. Async exports are
ordinary awaitable `Task`/`Task<T>` methods, and rejection faults the task with
host-facing JavaScript context.
