# Tutorial: Module ids + discovery

When hosting, you select a module using a **module id** (CommonJS module specifier).

## How module ids are used

- Typed hosting:
  - `JsEngine.LoadModule<TExports>()` uses `[JsModule("<moduleId>")]` on the contract interface.
  - `JsEngine.LoadModule<TExports>(moduleId)` lets you override/select a module id explicitly.
- Dynamic hosting:
  - `JsEngine.LoadModule(Assembly compiledAssembly, string moduleId)` loads that module from that assembly.

## Discover module ids in a compiled assembly

If you are loading an assembly dynamically and need to know what it contains:

```csharp
using Js2IL.Runtime;
using System.Reflection;

var asm = Assembly.LoadFrom("compiled.dll");
var ids = JsEngine.GetModuleIds(asm);

foreach (var id in ids)
{
    Console.WriteLine(id);
}
```

`GetModuleIds` uses the assembly-level manifest (`[JsCompiledModule]` attributes) when present, and falls back to scanning well-known namespaces for older assemblies.

## Bare vs path-like ids

Some module ids are bare specifiers like `math`. Others are path-like like `calculator/index`.

When loading, the runtime treats bare specifiers as local modules by default (`"math"` behaves like `"./math"`).

## Package ids

JS2IL also supports compiling and hosting modules with package-like ids (e.g., `@mixmark-io/domino`).
In those cases, you typically pass the full id to `LoadModule(...)`.
