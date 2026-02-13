# Global: require(id)

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | global |
| Status | supported |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/modules.html#requireid) |

## Implementation

- `JavaScriptRuntime/CommonJS/Require.cs, JavaScriptRuntime/CommonJS/ModuleContext.cs, JavaScriptRuntime/CommonJS/ModuleName.cs, JavaScriptRuntime/CommonJS/Module.cs`

## Notes

Supports requiring implemented Node core modules (e.g., fs/path) and compiled local modules. Local requires support ./ and ../ resolution relative to the importing module and are cached (module body executes once). Includes full module object support (module.exports, module.id, module.filename, module.path, module.loaded, module.parent, module.children, module.paths, module.require). Does not implement node_modules/package.json resolution (future plan: use package.json main/exports and module-type detection to support npm packages directly).

## Tests

- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Require_Basic` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Require_NestedNameConflict` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Require_RelativeFromModule` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Require_SharedDependency_ExecutedOnce` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Object` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Reassign` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Exports_Function` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Identity` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Loaded` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Require` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_Paths` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
- `Js2IL.Tests.CommonJS.ExecutionTests.CommonJS_Module_ParentChildren` (`Js2IL.Tests/CommonJS/ExecutionTests.cs`)
