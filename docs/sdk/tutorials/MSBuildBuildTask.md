# Tutorial: MSBuild build task

This tutorial shows how to compile JavaScript as part of a .NET build with `Jroc.SDK`.

## 1) Add packages

```xml
<ItemGroup>
  <PackageReference Include="Jroc.SDK" Version="VERSION" />
  <PackageReference Include="Jroc.Runtime" Version="VERSION" />
</ItemGroup>
```

`Jroc.SDK` supplies the MSBuild task. `Jroc.Runtime` supplies `Jroc.Runtime.JsEngine` and the runtime support assembly used by compiled modules.

## 2) Add a JavaScript module

Create `JavaScript\math.js`:

```js
function add(left, right) {
  return left + right;
}

exports.version = "1.0.0";
exports.add = add;
```

## 3) Compile during build

Add the file as a `JrocCompile` item:

```xml
<ItemGroup>
  <JrocCompile Include="JavaScript\math.js" />
</ItemGroup>
```

Build the project:

```powershell
dotnet build
```

The SDK compiles the module before assembly references are resolved. By default it writes generated files under `obj\<Configuration>\<TargetFramework>\jroc\math\` and references the generated module assembly so generated exports contracts are available to the C# compiler.

## 4) Call the generated contract

```csharp
using Jroc.Runtime;
using Jroc.math;

using var exports = JsEngine.LoadModule<IMathExports>();

Console.WriteLine(exports.Version);
Console.WriteLine(exports.Add(1, 2));
```

The generated contract interface lives in the compiled module assembly and is annotated with `[JsModule("math")]`, so the no-argument `LoadModule<TExports>()` overload can find the module id automatically.

## 5) Customize build output

Use item metadata for one module:

```xml
<JrocCompile Include="JavaScript\math.js"
             OutputDirectory="generated\math"
             RootModuleId="calculator/math"
             EmitPdb="true"
             CopyToOutputDirectory="true" />
```

Use project properties for defaults shared by all `JrocCompile` items:

```xml
<PropertyGroup>
  <JrocOutputRoot>$(IntermediateOutputPath)jroc</JrocOutputRoot>
  <JrocEmitPdb>true</JrocEmitPdb>
  <JrocCopyToOutputDirectory>true</JrocCopyToOutputDirectory>
</PropertyGroup>
```

Per-item metadata overrides the matching property.

## Package entrypoints

If the host project restores an npm package in the project directory, compile it by module id:

```xml
<JrocCompile Include="@mixmark-io/domino"
             CopyToOutputDirectory="true" />
```

The SDK resolves the package from `$(MSBuildProjectDirectory)` by default and uses `@mixmark-io/domino` as the root module id unless you override `RootModuleId`.
