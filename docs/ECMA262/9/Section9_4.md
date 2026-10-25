<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.4: Execution Contexts

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-25T02:53:21Z

RuntimeExecutionContext frames select the realm, global object, module location and active require state, while RuntimeServices invocation frames carry this, arguments, new.target and callee state. Compiler lowering supplies lexical and module origins for supported call paths. Full general ScriptOrModule record tracking and general eval are not implemented.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.4 | Execution Contexts | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-execution-contexts) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 9.4.1 | GetActiveScriptOrModule ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getactivescriptormodule) |
| 9.4.2 | ResolveBinding ( name [ , env ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-resolvebinding) |
| 9.4.3 | GetThisEnvironment ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getthisenvironment) |
| 9.4.4 | ResolveThisBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-resolvethisbinding) |
| 9.4.5 | GetNewTarget ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getnewtarget) |
| 9.4.6 | GetGlobalObject ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getglobalobject) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 9.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-getactivescriptormodule))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Active module origin in supported execution paths (GetActiveScriptOrModule) | Supported with Limitations | `tests/Jroc.Tests/RuntimeExecutionContextTests.cs`<br>`tests/Jroc.Tests/RuntimeModuleStateTests.cs`<br>[`Import_DynamicImport_Esm_Namespace.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_DynamicImport_Esm_Namespace.js)<br>[`Import_ImportMeta_Url.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_ImportMeta_Url.js) |  | Nested execution frames preserve and restore module locations and active require parents. Compiled dynamic imports receive an explicit defining-module identifier, and import.meta and require delegates are cached per realm and module. This supplies module origin for supported compiled paths without implementing the full general ScriptOrModule record stack or general eval. |

### 9.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-resolvebinding))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ResolveBinding over lexical/function/global scope chains | Supported with Limitations | `tests/Jroc.Tests/SymbolTableBuilderTests.cs`<br>[`Variable_LetBlockScope.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_LetBlockScope.js) |  | Binding resolution is handled by symbol table analysis and lowered scope access paths for supported constructs. |

### 9.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-resolvethisbinding))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| ResolveThisBinding for function/method/arrow contexts | Supported with Limitations | [`Function_ObjectLiteralMethod_ThisBinding.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js)<br>[`ArrowFunction_LexicalThis_CreatedInMethod.js`](../../../tests/Jroc.Tests/ArrowFunction/JavaScript/ArrowFunction_LexicalThis_CreatedInMethod.js) |  | this binding is modeled via RuntimeServices with lexical-this behavior for arrows in supported call paths. |

### 9.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-getnewtarget))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| GetNewTarget semantics in constructor/call flows | Supported with Limitations | [`Function_NewTarget_NewVsCall.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NewTarget_NewVsCall.js)<br>[`Function_NewTarget_Arrow_Inherits.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NewTarget_Arrow_Inherits.js)<br>[`Function_GeneratedConstruction_Semantics.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_GeneratedConstruction_Semantics.js) |  | new.target is propagated through generated function construction adapters and runtime call helpers, including alternate Reflect.construct targets, bound constructors, arrows, and derived classes with function-valued bases. |

### 9.4.6 ([tc39.es](https://tc39.es/ecma262/#sec-getglobalobject))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| GetGlobalObject access via GlobalThis | Supported with Limitations | `tests/Jroc.Tests/ValidatorTests.cs`<br>[`Process_Platform_Versions_And_Env_Basics.js`](../../../tests/Jroc.Tests/Node/Process/JavaScript/Process_Platform_Versions_And_Env_Basics.js) |  | Global object access is provided through the runtime GlobalThis object and host bootstrap wiring. |

