<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.4: Execution Contexts

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.4 | Execution Contexts | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-execution-contexts) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 9.4.1 | GetActiveScriptOrModule ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-getactivescriptormodule) |
| 9.4.2 | ResolveBinding ( name [ , env ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-resolvebinding) |
| 9.4.3 | GetThisEnvironment ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getthisenvironment) |
| 9.4.4 | ResolveThisBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-resolvethisbinding) |
| 9.4.5 | GetNewTarget ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getnewtarget) |
| 9.4.6 | GetGlobalObject ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getglobalobject) |

## Support

Feature-level support tracking with test script references.

### 9.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-getactivescriptormodule))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| GetActiveScriptOrModule | Not Yet Supported |  | Full script/module record tracking for active execution contexts is not implemented. |

### 9.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-resolvebinding))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ResolveBinding over lexical/function/global scope chains | Supported with Limitations | `Js2IL.Tests/SymbolTableBuilderTests.cs`<br>[`Variable_LetBlockScope.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetBlockScope.js) | Binding resolution is handled by symbol table analysis and lowered scope access paths for supported constructs. |

### 9.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-resolvethisbinding))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| ResolveThisBinding for function/method/arrow contexts | Supported with Limitations | [`Function_ObjectLiteralMethod_ThisBinding.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ObjectLiteralMethod_ThisBinding.js)<br>[`ArrowFunction_LexicalThis_CreatedInMethod.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_LexicalThis_CreatedInMethod.js) | this binding is modeled via RuntimeServices with lexical-this behavior for arrows in supported call paths. |

### 9.4.5 ([tc39.es](https://tc39.es/ecma262/#sec-getnewtarget))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| GetNewTarget semantics in constructor/call flows | Supported with Limitations | [`Function_NewTarget_NewVsCall.js`](../../../Js2IL.Tests/Function/JavaScript/Function_NewTarget_NewVsCall.js)<br>[`Function_NewTarget_Arrow_Inherits.js`](../../../Js2IL.Tests/Function/JavaScript/Function_NewTarget_Arrow_Inherits.js) | new.target is propagated through runtime call helpers for supported function/arrow/class behaviors. |

### 9.4.6 ([tc39.es](https://tc39.es/ecma262/#sec-getglobalobject))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| GetGlobalObject access via GlobalThis | Supported with Limitations | `Js2IL.Tests/ValidatorTests.cs`<br>[`Process_Platform_Versions_And_Env_Basics.js`](../../../Js2IL.Tests/Node/Process/JavaScript/Process_Platform_Versions_And_Env_Basics.js) | Global object access is provided through the runtime GlobalThis object and host bootstrap wiring. |

