# Global: require(id)

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/modules.html#requireid) |

## Implementation

- `src/JavaScriptRuntime/CommonJS/Require.cs, src/JavaScriptRuntime/CommonJS/ModuleContext.cs, src/JavaScriptRuntime/CommonJS/ModuleName.cs, src/JavaScriptRuntime/CommonJS/Module.cs`

## Notes

Supports requiring implemented Node core modules (e.g., fs/path) and compiled local modules. Local requires support ./ and ../ resolution relative to the importing module and are cached (module body executes once). Also supports compile-time resolution of npm packages via node_modules discovery, .js/.mjs/.cjs files, package.json main, package.json type=module entry graphs, conditional exports/imports with import/require/node/default conditions, single-* subpath patterns, and package.json imports aliases that target either package-local relative paths (./...) or bare package specifiers in the supported deterministic slice. Static import/export declarations and literal import()/require() package requests are resolved at compile time so import and require can target different entries from the same package graph. Runtime require does not probe the file system; packages must be discovered at compile time. Custom loaders/hooks, nested package-imports aliases, and broader runtime probing remain unsupported. Includes full module object support (module.exports, module.id, module.filename, module.path, module.loaded, module.parent, module.children, module.paths, module.require).

## Tests

- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Require_Basic` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Require_NestedNameConflict` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Require_RelativeFromModule` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Require_SharedDependency_ExecutedOnce` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Object` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Reassign` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Function` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Identity` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Loaded` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Require` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_Paths` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.ExecutionTests.CommonJS_Module_ParentChildren` (`tests/Jroc.Tests/CommonJS/ExecutionTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesExecutionTests.CommonJS_Require_NodeModules_PackageJson_Exports_And_NestedDependency` (`tests/Jroc.Tests/CommonJS/NodeModulesExecutionTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesExecutionTests.CommonJS_Require_NodeModules_DualMode_Exports_Imports_TypeModule_And_MjsEntry` (`tests/Jroc.Tests/CommonJS/NodeModulesExecutionTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesExecutionTests.CommonJS_Require_NodeModules_PackageImports_BarePackageAlias` (`tests/Jroc.Tests/CommonJS/NodeModulesExecutionTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesExecutionTests.CommonJS_Require_NodeModules_UnsupportedConditions_ReportDiagnostic` (`tests/Jroc.Tests/CommonJS/NodeModulesExecutionTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesGeneratorTests.CommonJS_NodeModules_DualMode_Exports_Imports_TypeModule_And_MjsEntry_EmitsManifest` (`tests/Jroc.Tests/CommonJS/NodeModulesGeneratorTests.cs`)
- `Jroc.Tests.CommonJS.NodeModulesGeneratorTests.CommonJS_NodeModules_PackageImports_BarePackageAlias_EmitsManifest` (`tests/Jroc.Tests/CommonJS/NodeModulesGeneratorTests.cs`)
