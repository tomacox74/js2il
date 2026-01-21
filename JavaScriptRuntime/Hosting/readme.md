# Hosting

This directory contains the .NET hosting surface for running JS2IL-compiled JavaScript assemblies as libraries.

## Contents

- `JsEngine`: public entry point for loading a compiled module and projecting its exports to a .NET contract.
- `JsRuntimeInstance`: hosts a single runtime instance on a dedicated thread and serializes all JS execution onto that thread.
- `JsExportsProxy`: `DispatchProxy` implementation that marshals interface calls to the owning runtime thread and maps them to CommonJS exports.

## Design

See the design doc: [DotNetLibraryHosting.md](../../docs/DotNetLibraryHosting.md)
