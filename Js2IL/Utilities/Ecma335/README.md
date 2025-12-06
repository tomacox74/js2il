# Ecma335 utilities

This folder contains helpers for emitting ECMA-335 metadata and IL constructs used by Js2IL.

## Components

- **AssemblyReferenceRegistry**: Registry for assembly references in a MetadataBuilder. Caches `AssemblyReferenceHandle` instances to avoid duplicate assembly references. Supports System.* BCL assemblies and JavaScriptRuntime assembly.
- **TypeReferenceRegistry**: Registry for type references in a MetadataBuilder. Caches `TypeReferenceHandle` instances for .NET types to avoid duplicate type references. Uses `AssemblyReferenceRegistry` to resolve assembly references for types.
- **TypeBuilder**: Small helpers to define types, nested types, and signatures with safer defaults.

