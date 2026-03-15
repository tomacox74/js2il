<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 16.2: Modules

[Back to Section16](Section16.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-03-15T19:08:25Z

JS2IL supports a practical subset of module syntax by lowering top-level `import` / `export` declarations to the existing CommonJS runtime model and aligning literal dynamic `import()` with the same pragmatic namespace/default interop surface. Namespace objects, re-exports, and common cyclic/live-binding cases work via getter-based interop. This enables mixed CJS/ESM-style graphs for common patterns, but JS2IL does not implement full ECMA-262 module records, host hooks, top-level `await` / async-module evaluation, or import attributes.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 16.2 | Modules | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-modules) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 16.2.1 | Module Semantics | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-module-semantics) |
| 16.2.1.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-semantics-static-semantics-early-errors) |
| 16.2.1.2 | Static Semantics: ImportedLocalNames ( importEntries ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-importedlocalnames) |
| 16.2.1.3 | ModuleRequest Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-modulerequest-record) |
| 16.2.1.3.1 | ModuleRequestsEqual ( left , right ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ModuleRequestsEqual) |
| 16.2.1.4 | Static Semantics: ModuleRequests | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-modulerequests) |
| 16.2.1.5 | Abstract Module Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-abstract-module-records) |
| 16.2.1.5.1 | EvaluateModuleSync ( module ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-EvaluateModuleSync) |
| 16.2.1.6 | Cyclic Module Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-cyclic-module-records) |
| 16.2.1.6.1 | Implementation of Module Record Abstract Methods | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-cyclic-module-record-module-record-methods) |
| 16.2.1.6.1.1 | LoadRequestedModules ( [ hostDefined ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-LoadRequestedModules) |
| 16.2.1.6.1.1.1 | InnerModuleLoading ( state , module ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-InnerModuleLoading) |
| 16.2.1.6.1.1.2 | ContinueModuleLoading ( state , moduleCompletion ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ContinueModuleLoading) |
| 16.2.1.6.1.2 | Link ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-moduledeclarationlinking) |
| 16.2.1.6.1.2.1 | InnerModuleLinking ( module , stack , index ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-InnerModuleLinking) |
| 16.2.1.6.1.3 | Evaluate ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-moduleevaluation) |
| 16.2.1.6.1.3.1 | InnerModuleEvaluation ( module , stack , index ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-innermoduleevaluation) |
| 16.2.1.6.1.3.2 | ExecuteAsyncModule ( module ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-execute-async-module) |
| 16.2.1.6.1.3.3 | GatherAvailableAncestors ( module , execList ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-gather-available-ancestors) |
| 16.2.1.6.1.3.4 | AsyncModuleExecutionFulfilled ( module ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-module-execution-fulfilled) |
| 16.2.1.6.1.3.5 | AsyncModuleExecutionRejected ( module , error ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-async-module-execution-rejected) |
| 16.2.1.6.2 | Example Cyclic Module Record Graphs | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-example-cyclic-module-record-graphs) |
| 16.2.1.7 | Source Text Module Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-records) |
| 16.2.1.7.1 | ParseModule ( sourceText , realm , hostDefined ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-parsemodule) |
| 16.2.1.7.2 | Implementation of Module Record Abstract Methods | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-module-record-methods) |
| 16.2.1.7.2.1 | GetExportedNames ( [ exportStarSet ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-getexportednames) |
| 16.2.1.7.2.2 | ResolveExport ( exportName [ , resolveSet ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-resolveexport) |
| 16.2.1.7.3 | Implementation of Cyclic Module Record Abstract Methods | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-cyclic-module-record-methods) |
| 16.2.1.7.3.1 | InitializeEnvironment ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-initialize-environment) |
| 16.2.1.7.3.2 | ExecuteModule ( [ capability ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-execute-module) |
| 16.2.1.8 | Synthetic Module Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-synthetic-module-records) |
| 16.2.1.8.1 | CreateDefaultExportSyntheticModule ( defaultExport ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-create-default-export-synthetic-module) |
| 16.2.1.8.2 | ParseJSONModule ( source ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-parse-json-module) |
| 16.2.1.8.3 | SetSyntheticModuleExport ( module , exportName , exportValue ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-setsyntheticmoduleexport) |
| 16.2.1.8.4 | Implementation of Module Record Abstract Methods | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-smr-module-record-methods) |
| 16.2.1.8.4.1 | LoadRequestedModules ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-smr-LoadRequestedModules) |
| 16.2.1.8.4.2 | GetExportedNames ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-smr-getexportednames) |
| 16.2.1.8.4.3 | ResolveExport ( exportName ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-smr-resolveexport) |
| 16.2.1.8.4.4 | Link ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-smr-Link) |
| 16.2.1.8.4.5 | Evaluate ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-smr-Evaluate) |
| 16.2.1.9 | GetImportedModule ( referrer , request ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-GetImportedModule) |
| 16.2.1.10 | HostLoadImportedModule ( referrer , moduleRequest , hostDefined , payload ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-HostLoadImportedModule) |
| 16.2.1.11 | FinishLoadingImportedModule ( referrer , moduleRequest , payload , result ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-FinishLoadingImportedModule) |
| 16.2.1.12 | AllImportAttributesSupported ( attributes ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-AllImportAttributesSupported) |
| 16.2.1.12.1 | HostGetSupportedImportAttributes ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-hostgetsupportedimportattributes) |
| 16.2.1.13 | GetModuleNamespace ( module ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getmodulenamespace) |
| 16.2.1.14 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-semantics-runtime-semantics-evaluation) |
| 16.2.2 | Imports | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-imports) |
| 16.2.2.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-imports-static-semantics-early-errors) |
| 16.2.2.2 | Static Semantics: ImportEntries | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-importentries) |
| 16.2.2.3 | Static Semantics: ImportEntriesForModule | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-importentriesformodule) |
| 16.2.2.4 | Static Semantics: WithClauseToAttributes | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-withclausetoattributes) |
| 16.2.3 | Exports | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-exports) |
| 16.2.3.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-exports-static-semantics-early-errors) |
| 16.2.3.2 | Static Semantics: ExportedBindings | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportedbindings) |
| 16.2.3.3 | Static Semantics: ExportedNames | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportednames) |
| 16.2.3.4 | Static Semantics: ExportEntries | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportentries) |
| 16.2.3.5 | Static Semantics: ExportEntriesForModule | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportentriesformodule) |
| 16.2.3.6 | Static Semantics: ReferencedBindings | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-referencedbindings) |
| 16.2.3.7 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-exports-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 16.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-module-semantics))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Full module record/linking/evaluation model | Not Yet Supported |  | Current implementation parses module source with `ParseScript` plus a rewrite pass; it does not model full ECMA-262 module records, host hooks, or spec-accurate cyclic link/evaluate algorithms. That deeper follow-on remains tracked separately from the practical static/dynamic interop surface. |

### 16.2.1.12 ([tc39.es](https://tc39.es/ecma262/#sec-AllImportAttributesSupported))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Import attributes / with clauses | Not Yet Supported |  | Static module rewriting rejects import/export attributes, and the host hook for supported import attributes is not implemented. |

### 16.2.1.13 ([tc39.es](https://tc39.es/ecma262/#sec-getmodulenamespace))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Module namespace objects for static imports | Supported with Limitations | [`Import_Namespace_Esm_Basic.js`](../../../Js2IL.Tests/Import/JavaScript/Import_Namespace_Esm_Basic.js)<br>[`Import_Namespace_FromCjs_Stable.js`](../../../Js2IL.Tests/Import/JavaScript/Import_Namespace_FromCjs_Stable.js) | Namespace objects are produced through the `__js2il_esm_namespace` helper. ESM namespace imports expose the export object directly, while CommonJS namespace imports use a cached synthetic namespace object with getter-based forwarding. |

### 16.2.1.14 ([tc39.es](https://tc39.es/ecma262/#sec-module-semantics-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Module body evaluation via CommonJS require and export getters | Supported with Limitations | [`Import_RequireEsmModule.js`](../../../Js2IL.Tests/Import/JavaScript/Import_RequireEsmModule.js)<br>[`Import_LiveBindings_Cycle.js`](../../../Js2IL.Tests/Import/JavaScript/Import_LiveBindings_Cycle.js)<br>[`Import_ExportStarFrom.js`](../../../Js2IL.Tests/Import/JavaScript/Import_ExportStarFrom.js) | Module bodies evaluate through generated CommonJS `require(...)` calls and getter-based export wiring. Common cyclic/live-binding cases are covered, but async module execution, top-level `await`, and spec-accurate linking/evaluation ordering are not implemented. |

### 16.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-imports))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Dynamic import() namespace/default interop | Supported with Limitations | [`Import_DynamicImport_Esm_Namespace.js`](../../../Js2IL.Tests/Import/JavaScript/Import_DynamicImport_Esm_Namespace.js)<br>[`Import_DynamicImport_Cjs_Namespace.js`](../../../Js2IL.Tests/Import/JavaScript/Import_DynamicImport_Cjs_Namespace.js) | Literal dynamic `import()` requests resolve through the existing CommonJS loader but now return the same pragmatic namespace/default projection used by the static ESM lowering. ESM-lowered modules resolve to their export object directly; CommonJS-style modules resolve to a cached synthetic namespace object that exposes `default`, `module.exports`, and getter-backed enumerable export keys. Full ECMA-262 module-record linking/evaluation semantics remain out of scope and are tracked separately. |
| Top-level static import declarations | Supported with Limitations | [`Import_StaticImport_FromCjs.js`](../../../Js2IL.Tests/Import/JavaScript/Import_StaticImport_FromCjs.js)<br>[`Import_LiveBindings_Named.js`](../../../Js2IL.Tests/Import/JavaScript/Import_LiveBindings_Named.js)<br>[`Import_LiveBindings_Cycle.js`](../../../Js2IL.Tests/Import/JavaScript/Import_LiveBindings_Cycle.js)<br>[`Import_Namespace_Esm_Basic.js`](../../../Js2IL.Tests/Import/JavaScript/Import_Namespace_Esm_Basic.js)<br>[`Import_Namespace_FromCjs_Stable.js`](../../../Js2IL.Tests/Import/JavaScript/Import_Namespace_FromCjs_Stable.js) | Supported forms include side-effect imports, default imports, named imports, and namespace imports. Imports are lowered to CommonJS `require(...)` calls, and imported identifier reads are rewritten to live reads to better match ESM semantics. Namespace imports from ESM return the module export object; namespace imports from CommonJS return a cached synthetic namespace object whose properties forward through to the underlying exports (live values for keys present at namespace creation time). Imported bindings are treated as immutable; assignment/update/destructuring writes and `delete` of an imported binding are rejected at compile time. In this MVP, static import/export declarations must appear at top level and before non-directive top-level statements, and `with` clauses / import attributes are rejected. |

### 16.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-exports))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Top-level export declarations | Supported with Limitations | `Js2IL.Tests/Import/JavaScript/Import_RequireEsmModule_Lib.mjs`<br>`Js2IL.Tests/Import/JavaScript/Import_LiveBindings_Named_Lib.mjs`<br>`Js2IL.Tests/Import/JavaScript/Import_LiveBindings_Cycle_A.mjs`<br>`Js2IL.Tests/Import/JavaScript/Import_LiveBindings_Cycle_B.mjs`<br>[`Import_ExportNamedFrom.js`](../../../Js2IL.Tests/Import/JavaScript/Import_ExportNamedFrom.js)<br>[`Import_ExportStarFrom.js`](../../../Js2IL.Tests/Import/JavaScript/Import_ExportStarFrom.js)<br>`Js2IL.Tests/Import/JavaScript/Import_Namespace_Esm_Basic_Lib.mjs`<br>`Js2IL.Tests/Import/JavaScript/Import_Namespace_FromCjs_Stable_Lib.cjs` | Supported forms include named/default exports, `export ... from`, and `export *` re-exports. Exports are projected through CommonJS `exports` with getter-based wiring; export getters are installed before generated import `require(...)` calls to improve behavior for common cyclic graphs, but this is not a spec-accurate SourceTextModuleRecord link/evaluate implementation. |

