# Tutorial: MSBuild build task

Use this workflow when JavaScript is known at build time. `Jroc.SDK` compiles
it during MSBuild and makes the generated assembly's strongly typed `Import`
and `Run` APIs available to the host project. Examples use C#; the generated
types can also be consumed by compatible .NET languages.

## 1. Create a host project and add the SDK

With the .NET 10 SDK installed:

```shell
dotnet new console -n HostApp
cd HostApp
```

Add this to `HostApp.csproj`, replacing `VERSION` with a released JROC version:

```xml
<ItemGroup>
  <PackageReference Include="Jroc.SDK" Version="VERSION" />
</ItemGroup>
```

`Jroc.SDK` supplies the MSBuild task and the matching runtime implementation
transitively. No global `jroc` installation, direct runtime package reference,
handwritten export interface, or reflection-based assembly loading is needed.

## 2. Add a JavaScript module

Create `JavaScript\math.js`:

```js
function add(left, right) {
  return left + right;
}

exports.version = "1.0.0";
exports.add = add;
```

## 3. Declare the compilation input

Add the file as a `JrocCompile` item:

```xml
<ItemGroup>
  <JrocCompile Include="JavaScript\math.js" AssemblyName="HostedMath" />
</ItemGroup>
```

The SDK compiles the module before assembly references are resolved. Generated
files are written under `obj\<Configuration>\<TargetFramework>\jroc\` by
default. `AssemblyName` makes the public root facade name explicit:
`HostedMath`. The generated DLL is referenced automatically so its contracts
are available to the C# compiler in the same build.

## 4. Call the generated contract

Replace `Program.cs` with:

```csharp
using var exports = HostedMath.Import();

Console.WriteLine(exports.Version);
Console.WriteLine(exports.Add(1d, 2d));
```

The generated `HostedMath.Scripts.math.IExports` interface lives in the compiled
module assembly. Both `HostedMath.Import()` and `HostedMath.Scripts.math.Import()` return
that exact contract.

Build and run the host:

```shell
dotnet build
dotnet run --no-build
```

Expected output is `1.0.0` followed by `3`, on separate lines. The returned
exports contract owns an isolated runtime; `using` shuts it down when the
host is done.

## 5. Run a script without retaining exports

For a side-effect-only script, use the generated `Run` method instead:

```csharp
HostedMath.Run();
HostedMath.Scripts.math.Run("--mode", "test");
```

These are alternative entry points, not calls needed after `Import()`.
Each `Run` starts an isolated runtime, evaluates the selected script, drains
asynchronous work, and disposes the runtime before returning. A module that
only declares exports, such as this example, will not print anything by itself.
Side-effect-only scripts have no `Import` method.

## 6. Customize build output

Use item metadata for one module:

```xml
<JrocCompile Include="JavaScript\math.js"
             AssemblyName="HostedMath"
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

## Compile and import an npm package

Add the package to the host project's `package.json` and lock file, then restore
it before `JrocCompile`. For example, a project using `picocolors` can include:

```xml
<ItemGroup>
  <JrocCompile Include="picocolors"
               CopyToOutputDirectory="true" />
</ItemGroup>

<Target Name="NpmRestore" BeforeTargets="JrocCompile">
  <Exec WorkingDirectory="$(MSBuildProjectDirectory)"
        Command="npm ci" />
</Target>
```

The SDK resolves the package entrypoint from
`$(MSBuildProjectDirectory)\node_modules` using Node-style module resolution,
compiles the package and its dependencies, and references the generated
assembly from the host build. Import it through the package-derived facade:

```csharp
using var colors = global::picocolors.Import();

Console.WriteLine(colors.Red("ERROR"));
Console.WriteLine(colors.Green("OK"));
```

Package names are normalized into valid CLR identifiers. For example,
`picocolors` produces `picocolors`, while `@mixmark-io/domino` produces
`mixmark_io_domino`:

```xml
<JrocCompile Include="@mixmark-io/domino" />
```

```csharp
using var domino = global::mixmark_io_domino.Import();
using var window = domino.CreateWindow("<title>JROC</title>");
Console.WriteLine(window.Document.Title);
```

Set `AssemblyName` on `JrocCompile` when you want to choose the generated
facade name explicitly. If the package publishes supported TypeScript
declarations through `types` or `typings`, JROC uses them to enrich the
generated contract; otherwise it conservatively infers the public surface
from the JavaScript source.

See the runnable [Picocolors](../../../samples/Picocolors) and
[Domino](../../../samples/Domino) samples for complete project and npm files.
