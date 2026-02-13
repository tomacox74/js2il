# Node.js Support Documentation

This directory contains modular documentation for JS2IL's Node.js API support.

## Structure

- **Index.md**: Auto-generated index of all modules and globals with their support status
- **Individual module/global files**: One JSON + MD pair per module/global
  - Example: `path.json` (source) + `path.md` (generated)
- **ModuleDoc.schema.json**: JSON schema defining the structure for module/global documentation
- **NodeLimitations.json**: Known limitations shared across all Node.js features

## Workflow

### Adding a New Module or Global

1. Create a new JSON file following the schema:
   ```bash
   # For a module
   cp path.json new_module.json
   # Edit new_module.json to document your module
   
   # For a global
   cp __dirname.json new_global.json
   # Edit new_global.json to document your global
   ```

2. Regenerate documentation:
   ```bash
   npm run generate:node-index          # Update Index.md
   npm run generate:node-module-docs    # Generate markdown for your new file
   # Or both at once:
   npm run generate:node-modules        # Full regeneration
   ```

### Updating Existing Documentation

1. Edit the corresponding JSON file (e.g., `path.json`)
2. Regenerate markdown:
   ```bash
   npm run generate:node-module-docs -- --module path
   # Or regenerate all:
   npm run generate:node-module-docs
   ```

### Regenerating from NodeSupport.json (Legacy)

If you prefer to maintain the monolithic `NodeSupport.json`:

1. Edit `NodeSupport.json`
2. Split into individual files and regenerate:
   ```bash
   npm run generate:node-modules
   ```

This will:
- Split `NodeSupport.json` into individual JSON files
- Generate `Index.md`
- Generate all individual markdown files

## Schema

See `ModuleDoc.schema.json` for the complete schema definition.

### Example Module JSON

```json
{
  "$schema": "./ModuleDoc.schema.json",
  "name": "path",
  "type": "module",
  "status": "partial",
  "docsUrl": "https://nodejs.org/api/path.html",
  "nodeVersionTarget": "22.x LTS",
  "implementation": "JavaScriptRuntime/Node/Path.cs",
  "apis": [
    {
      "name": "join(...parts)",
      "kind": "function",
      "status": "supported",
      "docs": "https://nodejs.org/api/path.html#pathjoinpaths",
      "tests": [
        {
          "name": "Js2IL.Tests.Node.ExecutionTests.Require_Path_Join_Basic",
          "file": "Js2IL.Tests/Node/ExecutionTests.cs#L9"
        }
      ]
    }
  ]
}
```

### Example Global JSON

```json
{
  "$schema": "./ModuleDoc.schema.json",
  "name": "__dirname",
  "type": "global",
  "status": "supported",
  "docsUrl": "https://nodejs.org/api/modules.html#dirname",
  "nodeVersionTarget": "22.x LTS",
  "implementation": "JavaScriptRuntime/Node/GlobalVariables.cs",
  "tests": [
    {
      "name": "Js2IL.Tests.Node.ExecutionTests.Global___dirname_PrintsDirectory",
      "file": "Js2IL.Tests/Node/ExecutionTests.cs#L15"
    }
  ]
}
```

## Status Values

- `supported`: Fully implemented and tested
- `partial`: Partially implemented with known limitations
- `not-supported`: Not yet implemented

## Files

- **Generated Files** (auto-generated, do not edit manually):
  - `Index.md`
  - `*.md` (all markdown files except this README)
  
- **Source Files** (edit these):
  - `*.json` (except schema files)
  - `ModuleDoc.schema.json` (schema definition)
  - `NodeLimitations.json` (shared limitations)

- **Legacy Files** (maintained for compatibility):
  - `NodeSupport.json`: Monolithic source
  - `NodeSupport.md`: Generated monolithic documentation
  - `NodeSupport.schema.json`: Schema for the monolithic format
