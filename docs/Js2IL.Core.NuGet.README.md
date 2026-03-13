# Js2IL.Core

`Js2IL.Core` is the referenceable NuGet package for the reusable js2il compiler library.

It ships the `Js2IL.Compiler.dll` assembly so build tasks, hosts, and other .NET tooling can compile JavaScript to .NET assemblies without shelling out to the `js2il` CLI tool.

The public entry points are the existing `Js2IL.Compiler`, `Js2IL.CompilerOptions`, and `Js2IL.CompilerServices` types in the `Js2IL` namespace.
