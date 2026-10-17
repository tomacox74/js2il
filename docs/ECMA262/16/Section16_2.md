<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 16.2: Modules

[Back to Section16](Section16.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-17T05:24:22Z

JROC compiles top-level import/export declarations natively, builds explicit module records, links re-export graphs, plans evaluation, and lowers top-level await through the async state-machine pipeline. Static and dynamic imports support bundled targets, JSON/text/immutable-byte attributes, namespace exotic objects, cached module failures, and the pinned deferred-import extension with asynchronous dependency ordering and readiness checks. All 127 pinned language/import cases have verified native results. Host loading hooks and full general async cyclic-module algorithms remain limited.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 16.2 | Modules | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-modules) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 16.2.1 | Module Semantics | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-module-semantics) |
| 16.2.1.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-semantics-static-semantics-early-errors) |
| 16.2.1.2 | Static Semantics: ImportedLocalNames ( importEntries ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-importedlocalnames) |
| 16.2.1.3 | ModuleRequest Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-modulerequest-record) |
| 16.2.1.3.1 | ModuleRequestsEqual ( left , right ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ModuleRequestsEqual) |
| 16.2.1.4 | Static Semantics: ModuleRequests | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-modulerequests) |
| 16.2.1.5 | Abstract Module Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-abstract-module-records) |
| 16.2.1.5.1 | EvaluateModuleSync ( module ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-EvaluateModuleSync) |
| 16.2.1.6 | Cyclic Module Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-cyclic-module-records) |
| 16.2.1.6.1 | Implementation of Module Record Abstract Methods | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-cyclic-module-record-module-record-methods) |
| 16.2.1.6.1.1 | LoadRequestedModules ( [ hostDefined ] ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-LoadRequestedModules) |
| 16.2.1.6.1.1.1 | InnerModuleLoading ( state , module ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-InnerModuleLoading) |
| 16.2.1.6.1.1.2 | ContinueModuleLoading ( state , moduleCompletion ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-ContinueModuleLoading) |
| 16.2.1.6.1.2 | Link ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-moduledeclarationlinking) |
| 16.2.1.6.1.2.1 | InnerModuleLinking ( module , stack , index ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-InnerModuleLinking) |
| 16.2.1.6.1.3 | Evaluate ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-moduleevaluation) |
| 16.2.1.6.1.3.1 | InnerModuleEvaluation ( module , stack , index ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-innermoduleevaluation) |
| 16.2.1.6.1.3.2 | ExecuteAsyncModule ( module ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-execute-async-module) |
| 16.2.1.6.1.3.3 | GatherAvailableAncestors ( module , execList ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-gather-available-ancestors) |
| 16.2.1.6.1.3.4 | AsyncModuleExecutionFulfilled ( module ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-module-execution-fulfilled) |
| 16.2.1.6.1.3.5 | AsyncModuleExecutionRejected ( module , error ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-async-module-execution-rejected) |
| 16.2.1.6.2 | Example Cyclic Module Record Graphs | N/A (informational) | [tc39.es](https://tc39.es/ecma262/#sec-example-cyclic-module-record-graphs) |
| 16.2.1.7 | Source Text Module Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-records) |
| 16.2.1.7.1 | ParseModule ( sourceText , realm , hostDefined ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-parsemodule) |
| 16.2.1.7.2 | Implementation of Module Record Abstract Methods | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-module-record-methods) |
| 16.2.1.7.2.1 | GetExportedNames ( [ exportStarSet ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getexportednames) |
| 16.2.1.7.2.2 | ResolveExport ( exportName [ , resolveSet ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-resolveexport) |
| 16.2.1.7.3 | Implementation of Cyclic Module Record Abstract Methods | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-cyclic-module-record-methods) |
| 16.2.1.7.3.1 | InitializeEnvironment ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-initialize-environment) |
| 16.2.1.7.3.2 | ExecuteModule ( [ capability ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-source-text-module-record-execute-module) |
| 16.2.1.8 | Synthetic Module Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-synthetic-module-records) |
| 16.2.1.8.1 | CreateDefaultExportSyntheticModule ( defaultExport ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-create-default-export-synthetic-module) |
| 16.2.1.8.2 | ParseJSONModule ( source ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-parse-json-module) |
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
| 16.2.1.12 | AllImportAttributesSupported ( attributes ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-AllImportAttributesSupported) |
| 16.2.1.12.1 | HostGetSupportedImportAttributes ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-hostgetsupportedimportattributes) |
| 16.2.1.13 | GetModuleNamespace ( module ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getmodulenamespace) |
| 16.2.1.14 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-semantics-runtime-semantics-evaluation) |
| 16.2.2 | Imports | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-imports) |
| 16.2.2.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-imports-static-semantics-early-errors) |
| 16.2.2.2 | Static Semantics: ImportEntries | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-importentries) |
| 16.2.2.3 | Static Semantics: ImportEntriesForModule | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-importentriesformodule) |
| 16.2.2.4 | Static Semantics: WithClauseToAttributes | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-withclausetoattributes) |
| 16.2.3 | Exports | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-exports) |
| 16.2.3.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-exports-static-semantics-early-errors) |
| 16.2.3.2 | Static Semantics: ExportedBindings | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportedbindings) |
| 16.2.3.3 | Static Semantics: ExportedNames | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportednames) |
| 16.2.3.4 | Static Semantics: ExportEntries | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportentries) |
| 16.2.3.5 | Static Semantics: ExportEntriesForModule | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-exportentriesformodule) |
| 16.2.3.6 | Static Semantics: ReferencedBindings | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-referencedbindings) |
| 16.2.3.7 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-exports-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 16.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-module-semantics))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Full module record/linking/evaluation model | Supported with Limitations | [`Import_LiveBindings_Cycle.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_LiveBindings_Cycle.js)<br>[`Import_ExportStarFrom.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_ExportStarFrom.js)<br>[`Import_ExportStarFromMultiHop.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_ExportStarFromMultiHop.js) |  | JROC now materializes explicit module-record metadata, performs a separate graph-based linking pass, and plans evaluation order/components before IL generation. This covers module-request records, graph-driven `GetExportedNames` / `ResolveExport`, common cyclic graphs, and link-time ambiguity/missing-export diagnostics for the supported static ESM subset. The runtime still executes through the CommonJS-oriented host model, so host loading hooks, true `ParseModule`, and full async module dependency-graph algorithms remain unsupported. |

### 16.2.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-abstract-module-records))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Top-level await in module bodies | Supported with Limitations | [`Async_TopLevelAwait_ResolvedPromise.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_TopLevelAwait_ResolvedPromise.js)<br>[`Async_TopLevelAwait_PendingPromise.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_TopLevelAwait_PendingPromise.js)<br>[`Async_TopLevelAwait_RejectionCaught.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_TopLevelAwait_RejectionCaught.js)<br>`tests/Jroc.Test262.Tests/language/import/import-defer/evaluation-top-level-await/flattening-order/IdentifiersImportCompletionExecutionTests.cs` |  | Top-level await lowers through an async module-body state machine. Graphs bundled for phase/type-aware imports schedule asynchronous native dependencies without blocking sibling starts, resume dependent bodies after completion, and cache rejection values. Deferred edges eagerly evaluate only their transitive asynchronous frontier; synchronous evaluation is rejected while a required asynchronous dependency is incomplete. Other graphs retain existing wrapper initialization and await behavior. The pinned deferred-import ordering, transitive-async, and readiness cases pass; this is not a claim of complete general asynchronous cyclic-module conformance. |

### 16.2.1.12 ([tc39.es](https://tc39.es/ecma262/#sec-AllImportAttributesSupported))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Import attributes / with clauses | Supported with Limitations | `tests/Jroc.Test262.Tests/language/import/import-attributes/IdentifiersImportCompletionExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/import/import-bytes/IdentifiersImportCompletionExecutionTests.cs`<br>`tests/Jroc.Tests/Import/NativeEsmOverheadTests.cs` |  | Static import/export with clauses and dynamic import options support bundled json, text, and bytes types with separate cached namespace identities. Text preserves the original source; bytes exposes a Uint8Array over immutable backing storage. Relative resources may be binary, have non-JavaScript extensions, or be extensionless. The same physical file can also be compiled as JavaScript without confusing its typed projection. Typed modules expose default only. Unknown attributes/types are explicitly rejected; a general host attribute hook remains unsupported. |

### 16.2.1.13 ([tc39.es](https://tc39.es/ecma262/#sec-getmodulenamespace))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Module namespace objects for static imports | Supported with Limitations | [`Import_Namespace_Esm_Basic.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_Namespace_Esm_Basic.js)<br>[`Import_Namespace_FromCjs_Stable.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_Namespace_FromCjs_Stable.js) |  | Native static imports return cached ESM namespace exotic objects with live binding reads, sorted string export keys, null prototypes, nonextensibility, non-configurable data descriptors, and rejected mutation. CommonJS namespaces retain cached getter-based forwarding. Deferred namespaces have separate identities and a Deferred Module tag. Symbol-like operations and writes do not trigger evaluation; other string-key operations, excluding then, do. Readiness checks include deferred transitive dependencies and preserve cached thrown values. Class field definitions respect namespace exotic operations, and private additions are rejected. |

### 16.2.1.14 ([tc39.es](https://tc39.es/ecma262/#sec-module-semantics-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Module body evaluation via CommonJS require and export getters | Supported with Limitations | [`Import_RequireEsmModule.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_RequireEsmModule.js)<br>[`Import_LiveBindings_Cycle.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_LiveBindings_Cycle.js)<br>[`Import_ExportStarFrom.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_ExportStarFrom.js) |  | Module bodies use generated CommonJS-compatible wrappers and live export accessors. Synchronous graphs retain generated source-order initialization. Graphs bundled for phase/type-aware imports additionally carry native metadata for promise-backed asynchronous dependency scheduling, completion tracking, and cached failures. Common cyclic/live-binding cases, multi-hop re-export chains, and the pinned deferred-import asynchronous graphs are covered. Full general asynchronous cyclic-module algorithms and host loading hooks remain limited. |

### 16.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-imports))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Dynamic import() namespace/default interop | Supported with Limitations | [`Import_DynamicImport_Esm_Namespace.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_DynamicImport_Esm_Namespace.js)<br>[`Import_DynamicImport_Cjs_Namespace.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_DynamicImport_Cjs_Namespace.js) |  | Literal and explicitly bundled computed dynamic imports load asynchronously and return cached ESM namespace exotic objects with live values, sorted UTF-16 export names, null prototypes, nonextensibility, writable/enumerable/non-configurable export descriptors, and rejected writes. Dynamic-only parse/link failures reject at import time, and ESM evaluation failures preserve their thrown value across later imports. CommonJS modules keep the cached default/module.exports projection. General host hooks and complete async graph algorithms remain unsupported. |
| Top-level static import declarations | Supported with Limitations | [`Import_StaticImport_FromCjs.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_StaticImport_FromCjs.js)<br>[`Import_LiveBindings_Named.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_LiveBindings_Named.js)<br>[`Import_LiveBindings_Cycle.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_LiveBindings_Cycle.js)<br>[`Import_Namespace_Esm_Basic.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_Namespace_Esm_Basic.js)<br>[`Import_Namespace_FromCjs_Stable.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_Namespace_FromCjs_Stable.js)<br>`tests/Jroc.Test262.Tests/language/import/import-attributes/IdentifiersImportCompletionExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/import/import-defer/syntax/IdentifiersImportCompletionExecutionTests.cs` |  | Supported forms include side-effect, default, named, namespace, and deferred namespace imports, with top-level declarations accepted in source order. Static import/export modules are lowered natively without injected JavaScript. Imported identifiers read live bindings and are immutable; the linker rejects missing/ambiguous exports. Static with clauses support type json, text, and bytes; unsupported attributes are rejected explicitly. All 127 pinned language/import cases have verified native results, including 117 newly verified cases and their required variants. General host loading hooks remain unsupported. |

### 16.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-exports))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Top-level export declarations | Supported with Limitations | `tests/Jroc.Tests/Import/JavaScript/Import_RequireEsmModule_Lib.mjs`<br>`tests/Jroc.Tests/Import/JavaScript/Import_LiveBindings_Named_Lib.mjs`<br>`tests/Jroc.Tests/Import/JavaScript/Import_LiveBindings_Cycle_A.mjs`<br>`tests/Jroc.Tests/Import/JavaScript/Import_LiveBindings_Cycle_B.mjs`<br>[`Import_ExportNamedFrom.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_ExportNamedFrom.js)<br>[`Import_ExportRenamedFrom.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_ExportRenamedFrom.js)<br>[`Import_ExportStarFrom.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_ExportStarFrom.js)<br>[`Import_ExportStarFromMultiHop.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_ExportStarFromMultiHop.js)<br>[`Import_ExportStarAsNamespace.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_ExportStarAsNamespace.js)<br>`tests/Jroc.Tests/Import/JavaScript/Import_DefaultAnonFunction_Lib.mjs`<br>`tests/Jroc.Tests/Import/JavaScript/Import_DefaultAnonClass_Lib.mjs`<br>`tests/Jroc.Tests/Import/JavaScript/Import_DefaultNamedClass_Lib.mjs`<br>`tests/Jroc.Tests/Import/JavaScript/Import_Namespace_Esm_Basic_Lib.mjs`<br>`tests/Jroc.Tests/Import/JavaScript/Import_Namespace_FromCjs_Stable_Lib.cjs` |  | Supported forms include named/default exports, indirect exports, star exports, and namespace re-exports. Native lowering uses binding cells and live forwarding accessors, preserving temporal-dead-zone errors and CommonJS interop. Typed named/default/namespace re-exports retain their request identity; typed synthetic modules expose only default, which star exports omit. Asynchronous star re-exports replace predeclared placeholders without overriding explicit local exports. Host hooks and full SourceTextModuleRecord environment semantics remain limited. |

