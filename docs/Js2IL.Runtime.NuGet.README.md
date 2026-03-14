# Js2IL.Runtime

`Js2IL.Runtime` is the runtime support package for executing JS2IL-compiled assemblies and hosting compiled modules from .NET.

It ships the `JavaScriptRuntime.dll` assembly plus the `Js2IL.Runtime` hosting APIs, including `JsEngine`.

## Which package should I use?

- [`Js2IL.Runtime`](https://www.nuget.org/packages/Js2IL.Runtime)
  - Use this when your application needs the runtime support library or the public hosting APIs used to load compiled modules.
- [`Js2IL.SDK`](https://www.nuget.org/packages/Js2IL.SDK)
  - Use this when your project should compile JavaScript during `dotnet build`.
- [`Js2IL.Core`](https://www.nuget.org/packages/Js2IL.Core)
  - Use this when you need the compiler as a reusable .NET library.
- [`js2il`](https://www.nuget.org/packages/js2il)
  - Use this when you want the standalone CLI/global tool for manual compilation.

Official releases publish `Js2IL.Runtime`, `js2il`, `Js2IL.Core`, and `Js2IL.SDK` together at the same version. Keep the versions aligned when you mix them in one workflow.

## Install

```xml
<ItemGroup>
  <PackageReference Include="Js2IL.Runtime" Version="VERSION" />
</ItemGroup>
```

## Package surface

- NuGet package: `Js2IL.Runtime`
- Public namespace: `Js2IL.Runtime`
- Runtime assembly copied next to compiled outputs: `JavaScriptRuntime.dll`

## Hosting compiled JavaScript from C#

If you want to load a compiled module from C#, start with the hosting docs:

- https://github.com/tomacox74/js2il/blob/master/docs/hosting/Index.md

The main entry point is `Js2IL.Runtime.JsEngine`, which can discover module ids and load typed or dynamic exports from compiled assemblies.

## Links

- Hosting docs: https://github.com/tomacox74/js2il/blob/master/docs/hosting/Index.md
- Source, issues, docs: https://github.com/tomacox74/js2il
- License: Apache-2.0
