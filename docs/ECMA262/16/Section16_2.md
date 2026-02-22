<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 16.2: Modules

[Back to Section16](Section16.md) | [Back to Index](../Index.md)

JS2IL supports a practical subset of static module syntax by lowering top-level `import` / `export` declarations to the existing CommonJS runtime model. This enables mixed CJS/ESM-style graphs for common patterns, but does not implement full ECMA-262 module-record/linking/evaluation semantics.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 16.2 | Modules | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-modules) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 16.2.1 | Module Semantics | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-semantics) |
| 16.2.1.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-semantics-static-semantics-early-errors) |
| 16.2.1.2 | Static Semantics: ImportedLocalNames ( importEntries ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-importedlocalnames) |
| 16.2.1.3 | ModuleRequest Records | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-modulerequest-record) |
| 16.2.1.3.1 | ModuleRequestsEqual ( left , right ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-ModuleRequestsEqual) |
| 16.2.1.4 | Static Semantics: ModuleRequests | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-modulerequests) |
| 16.2.1.5 | Abstract Module Records | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-abstract-module-records) |
| 16.2.1.5.1 | EvaluateModuleSync ( module ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-EvaluateModuleSync) |
| 16.2.1.6 | Cyclic Module Records | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-cyclic-module-records) |
| 16.2.1.6.1 | Implementation of Module Record Abstract Methods | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-cyclic-module-record-module-record-methods) |
| 16.2.1.6.1.1 | LoadRequestedModules ( [ hostDefined ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-LoadRequestedModules) |
| 16.2.1.6.1.1.1 | InnerModuleLoading ( state , module ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-InnerModuleLoading) |
| 16.2.1.6.1.1.2 | ContinueModuleLoading ( state , moduleCompletion ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-ContinueModuleLoading) |
| 16.2.1.6.1.2 | Link ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-moduledeclarationlinking) |
| 16.2.1.6.1.2.1 | InnerModuleLinking ( module , stack , index ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-InnerModuleLinking) |
| 16.2.1.6.1.3 | Evaluate ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-moduleevaluation) |
| 16.2.1.6.1.3.1 | InnerModuleEvaluation ( module , stack , index ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-innermoduleevaluation) |
| 16.2.1.6.1.3.2 | ExecuteAsyncModule ( module ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-execute-async-module) |
| 16.2.1.6.1.3.3 | GatherAvailableAncestors ( module , execList ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-gather-available-ancestors) |
| 16.2.1.6.1.3.4 | AsyncModuleExecutionFulfilled ( module ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-async-module-execution-fulfilled) |
| 16.2.1.6.1.3.5 | AsyncModuleExecutionRejected ( module , error ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-async-module-execution-rejected) |
| 16.2.1.6.2 | Example Cyclic Module Record Graphs | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-example-cyclic-module-record-graphs) |
| 16.2.1.7 | Source Text Module Records | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-records) |
| 16.2.1.7.1 | ParseModule ( sourceText , realm , hostDefined ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-parsemodule) |
| 16.2.1.7.2 | Implementation of Module Record Abstract Methods | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-module-record-methods) |
| 16.2.1.7.2.1 | GetExportedNames ( [ exportStarSet ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getexportednames) |
| 16.2.1.7.2.2 | ResolveExport ( exportName [ , resolveSet ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-resolveexport) |
| 16.2.1.7.3 | Implementation of Cyclic Module Record Abstract Methods | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-cyclic-module-record-methods) |
| 16.2.1.7.3.1 | InitializeEnvironment ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-initialize-environment) |
| 16.2.1.7.3.2 | ExecuteModule ( [ capability ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-execute-module) |
| 16.2.1.8 | Synthetic Module Records | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-synthetic-module-records) |
| 16.2.1.8.1 | CreateDefaultExportSyntheticModule ( defaultExport ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-create-default-export-synthetic-module) |
| 16.2.1.8.2 | ParseJSONModule ( source ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-parse-json-module) |
| 16.2.1.8.3 | SetSyntheticModuleExport ( module , exportName , exportValue ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-setsyntheticmoduleexport) |
| 16.2.1.8.4 | Implementation of Module Record Abstract Methods | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-smr-module-record-methods) |
| 16.2.1.8.4.1 | LoadRequestedModules ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-smr-LoadRequestedModules) |
| 16.2.1.8.4.2 | GetExportedNames ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-smr-getexportednames) |
| 16.2.1.8.4.3 | ResolveExport ( exportName ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-smr-resolveexport) |
| 16.2.1.8.4.4 | Link ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-smr-Link) |
| 16.2.1.8.4.5 | Evaluate ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-smr-Evaluate) |
| 16.2.1.9 | GetImportedModule ( referrer , request ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-GetImportedModule) |
| 16.2.1.10 | HostLoadImportedModule ( referrer , moduleRequest , hostDefined , payload ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-HostLoadImportedModule) |
| 16.2.1.11 | FinishLoadingImportedModule ( referrer , moduleRequest , payload , result ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-FinishLoadingImportedModule) |
| 16.2.1.12 | AllImportAttributesSupported ( attributes ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-AllImportAttributesSupported) |
| 16.2.1.12.1 | HostGetSupportedImportAttributes ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-hostgetsupportedimportattributes) |
| 16.2.1.13 | GetModuleNamespace ( module ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getmodulenamespace) |
| 16.2.1.14 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-module-semantics-runtime-semantics-evaluation) |
| 16.2.2 | Imports | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-imports) |
| 16.2.2.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-imports-static-semantics-early-errors) |
| 16.2.2.2 | Static Semantics: ImportEntries | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-importentries) |
| 16.2.2.3 | Static Semantics: ImportEntriesForModule | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-importentriesformodule) |
| 16.2.2.4 | Static Semantics: WithClauseToAttributes | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-withclausetoattributes) |
| 16.2.3 | Exports | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-exports) |
| 16.2.3.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-exports-static-semantics-early-errors) |
| 16.2.3.2 | Static Semantics: ExportedBindings | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportedbindings) |
| 16.2.3.3 | Static Semantics: ExportedNames | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportednames) |
| 16.2.3.4 | Static Semantics: ExportEntries | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportentries) |
| 16.2.3.5 | Static Semantics: ExportEntriesForModule | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportentriesformodule) |
| 16.2.3.6 | Static Semantics: ReferencedBindings | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-referencedbindings) |
| 16.2.3.7 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-exports-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 16.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-module-semantics))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Full module record/linking/evaluation model | Not Yet Supported |  | Current implementation does not model full ECMA-262 module records, host hooks, or spec-accurate cyclic linking/evaluation semantics. |

### 16.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-imports))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Top-level static import declarations | Supported with Limitations | [`Import_StaticImport_FromCjs.js`](../../../Js2IL.Tests/Import/JavaScript/Import_StaticImport_FromCjs.js) | Supported forms include side-effect imports, default imports, named imports, and namespace imports. Imports are lowered to CommonJS `require(...)` calls. |

### 16.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-exports))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Top-level export declarations | Supported with Limitations | `Js2IL.Tests/Import/JavaScript/Import_RequireEsmModule_Lib.mjs` | Supported forms include named/default exports, `export ... from`, and `export *` re-exports. Exports are projected through CommonJS `exports` with getter-based wiring. |

