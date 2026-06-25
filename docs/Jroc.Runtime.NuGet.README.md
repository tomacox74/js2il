# Jroc.Runtime

`Jroc.Runtime` is the runtime support package for executing JROC-compiled assemblies and hosting compiled modules from .NET.

It ships the `JavaScriptRuntime.dll` assembly plus the `Jroc.Runtime` hosting APIs, including `JsEngine`.

## Which package should I use?

- [`Jroc.Runtime`](https://www.nuget.org/packages/Jroc.Runtime)
  - Use this when your application needs the runtime support library or the public hosting APIs used to load compiled modules.
- [`Jroc.SDK`](https://www.nuget.org/packages/Jroc.SDK)
  - Use this when your project should compile JavaScript during `dotnet build`.
- [`Jroc.Core`](https://www.nuget.org/packages/Jroc.Core)
  - Use this when you need the compiler as a reusable .NET library.
- [`jroc`](https://www.nuget.org/packages/jroc)
  - Use this when you want the standalone CLI/global tool for manual compilation.

Official releases publish `Jroc.Runtime`, `jroc`, `Jroc.Core`, and `Jroc.SDK` together at the same version. Keep the versions aligned when you mix them in one workflow.

## Install

```xml
<ItemGroup>
  <PackageReference Include="Jroc.Runtime" Version="VERSION" />
</ItemGroup>
```

## Package surface

- NuGet package: `Jroc.Runtime`
- Public namespace: `Jroc.Runtime`
- Runtime assembly copied next to compiled outputs: `JavaScriptRuntime.dll`

## Hosting compiled JavaScript from C#

If you want to load a compiled module from C#, start with the hosting docs:

- https://github.com/tomacox74/jroc/blob/master/docs/sdk/Index.md

The main entry point is `Jroc.Runtime.JsEngine`, which can discover module ids and load typed or dynamic exports from compiled assemblies.

## Links

- SDK docs: https://github.com/tomacox74/jroc/blob/master/docs/sdk/Index.md
- Source, issues, docs: https://github.com/tomacox74/jroc
- License: Apache-2.0
