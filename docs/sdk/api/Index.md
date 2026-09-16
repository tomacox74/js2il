# JROC API reference

The supported consumer entry points are the CLI, MSBuild-generated assembly
APIs, and the in-memory compiler. Start with the
[workflow overview](../Index.md) to choose the appropriate path.

## Command line

- [CLI usage and options](../../../README.md#command-line-usage)

## MSBuild and generated assemblies

- [`JrocCompile` items, metadata, and targets](JrocCompile.md)
- [Generated `Run`, `Import`, and export contracts](GeneratedFacades.md)
- [Generated handles and constructors](Handles.md)

## In-memory compilation

- [Requests, compiled artifacts, and loaded modules](InMemoryCompiler.md)

## Shared behavior

- [Exceptions](Exceptions.md)
- [JavaScript to .NET type mapping](TypeMapping.md) — primitive values,
  generated contracts, dynamic objects, functions, and async projections
