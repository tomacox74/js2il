# Tutorial: Getting started

This tutorial walks through compiling a JavaScript module and calling it from a .NET app.

## Prerequisites

- .NET 10 SDK
- `js2il` installed:

```powershell
dotnet tool install --global js2il
```

## 1) Create a JavaScript module

Create `math.js`:

```js
function add(x, y) {
  return x + y;
}

const version = "1.0.0";

module.exports = {
  version,
  add,
};
```

## 2) Compile it with js2il

```powershell
# Default (no debug symbols)
js2il .\math.js .\out

# Optional: emit Portable PDB debug symbols (.pdb) for stepping and better stack traces
js2il .\math.js .\out --pdb
```

This produces (at minimum):

- `out\math.dll` (compiled module assembly)
- `out\math.runtimeconfig.json`
- `out\JavaScriptRuntime.dll`

If you pass `--pdb`, it also produces:

- `out\math.pdb`

Portable PDBs map back to the original `.js` / `.mjs` source path, including rewritten `import` / `export` module code, so managed debuggers and source-mapped stack traces resolve the original file and line numbers. Uncaptured locals appear as normal debugger locals; captured closure variables still surface through generated scope objects rather than ordinary local slots.

## 3) Create a host console app

```powershell
dotnet new console -n HostApp
cd .\HostApp

dotnet add package JavaScriptRuntime
```

Reference the compiled module assembly (`out\math.dll`):

```xml
<!-- HostApp.csproj -->
<ItemGroup>
  <Reference Include="math">
    <HintPath>..\out\math.dll</HintPath>
  </Reference>
</ItemGroup>
```

## 4) Call exports (typed)

If contract generation is enabled (it is **enabled by default**), `math.dll` contains a generated exports interface annotated with `[JsModule("math")]`.

In your host app:

```csharp
using Js2IL.Runtime;

// Namespace and type name are generated from the compiled assembly name.
// For an assembly named "math", the entry exports contract is:
//   Js2IL.math.IMathExports
using Js2IL.math;

using var exports = JsEngine.LoadModule<IMathExports>();

Console.WriteLine(exports.Version);
Console.WriteLine(exports.Add(1, 2));
```

## 5) Call exports (dynamic)

If you want to avoid compile-time references to generated contracts:

```csharp
using Js2IL.Runtime;
using System.Reflection;

var asm = Assembly.LoadFrom("..\\out\\math.dll");
using dynamic exports = JsEngine.LoadModule(asm, moduleId: "math");

Console.WriteLine((string)exports.version);
Console.WriteLine((double)exports.add(1, 2));
```

## Notes

- **Threading**: calls from any host thread are marshalled to the module’s dedicated script thread.
- **Disposal**: always dispose the object returned by `LoadModule(...)` to shut down the script thread.
