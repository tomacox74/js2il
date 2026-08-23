# Tutorial: Module ids + discovery

When hosting, you select a module using a **module id** (CommonJS module specifier).

## How module ids are used

- Typed hosting:
  - Generated facades use `<Assembly>.Import()` for the entry module and
    `<Assembly>.Scripts.<path>.Import()` for explicit modules.
  - `JsEngine.LoadModule<TExports>()` uses generated module metadata (or the
    legacy `[JsModule("<moduleId>")]` on hand-authored contracts).
  - `JsEngine.LoadModule<TExports>(moduleId)` lets you override/select a module id explicitly.
- Dynamic hosting:
  - `JsEngine.LoadDynamicModule(Assembly compiledAssembly, string moduleId)`
    loads that module from that assembly.

## Discover module ids in a compiled assembly

If you are loading an assembly dynamically and need to know what it contains:

```csharp
using Jroc.Runtime;
using System.Reflection;

var asm = Assembly.LoadFrom("compiled.dll");
var ids = JsEngine.GetModuleIds(asm);
var entryId = JsEngine.GetEntryModuleId(asm);

foreach (var id in ids)
{
    Console.WriteLine(id);
}
```

`GetModuleIds` uses the assembly-level manifest (`[JsCompiledModule]` attributes) when present, and falls back to scanning well-known namespaces for older assemblies.

`GetEntryModuleId` reads the separate entry-module declaration. It returns the
canonical id even when the entry also has package or root aliases.

## Bare vs path-like ids

Some module ids are bare specifiers like `math`. Others are path-like like `calculator/index`.

When loading, the runtime treats bare specifiers as local modules by default (`"math"` behaves like `"./math"`).

## Package ids

JROC also supports compiling and hosting modules with package-like ids (e.g., `@mixmark-io/domino`).
In those cases, you typically pass the full id to `LoadModule(...)`.
