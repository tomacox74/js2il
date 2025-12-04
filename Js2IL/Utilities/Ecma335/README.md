# Ecma335 utilities

This folder contains helpers for emitting ECMA-335 metadata and IL constructs used by Js2IL.

## Components

- **AssemblyReferenceRegistry**: Registry for assembly references in a MetadataBuilder. Caches `AssemblyReferenceHandle` instances to avoid duplicate assembly references. Supports System.* BCL assemblies and JavaScriptRuntime assembly.
- **TypeBuilder**: Small helpers to define types, nested types, and signatures with safer defaults.

