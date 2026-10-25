<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.1: Environment Records

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-25T02:53:21Z

JROC implements environment semantics through compiler-managed scope instances, realm-owned global lexical bindings, object-backed with environments and native ESM binding cells rather than literal ECMA-262 Environment Record objects. General eval, host module loading hooks and full general asynchronous cyclic-module semantics remain limited or unsupported.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.1 | Environment Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-environment-records) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 9.1.1 | The Environment Record Type Hierarchy | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-the-environment-record-type-hierarchy) |
| 9.1.1.1 | Declarative Environment Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records) |
| 9.1.1.1.1 | HasBinding ( N ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records-hasbinding-n) |
| 9.1.1.1.2 | CreateMutableBinding ( N , D ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records-createmutablebinding-n-d) |
| 9.1.1.1.3 | CreateImmutableBinding ( N , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records-createimmutablebinding-n-s) |
| 9.1.1.1.4 | InitializeBinding ( N , V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records-initializebinding-n-v) |
| 9.1.1.1.5 | SetMutableBinding ( N , V , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records-setmutablebinding-n-v-s) |
| 9.1.1.1.6 | GetBindingValue ( N , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records-getbindingvalue-n-s) |
| 9.1.1.1.7 | DeleteBinding ( N ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records-deletebinding-n) |
| 9.1.1.1.8 | HasThisBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records-hasthisbinding) |
| 9.1.1.1.9 | HasSuperBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records-hassuperbinding) |
| 9.1.1.1.10 | WithBaseObject ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records-withbaseobject) |
| 9.1.1.2 | Object Environment Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records) |
| 9.1.1.2.1 | HasBinding ( N ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-hasbinding-n) |
| 9.1.1.2.2 | CreateMutableBinding ( N , D ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-createmutablebinding-n-d) |
| 9.1.1.2.3 | CreateImmutableBinding ( N , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-createimmutablebinding-n-s) |
| 9.1.1.2.4 | InitializeBinding ( N , V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-initializebinding-n-v) |
| 9.1.1.2.5 | SetMutableBinding ( N , V , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-setmutablebinding-n-v-s) |
| 9.1.1.2.6 | GetBindingValue ( N , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-getbindingvalue-n-s) |
| 9.1.1.2.7 | DeleteBinding ( N ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-deletebinding-n) |
| 9.1.1.2.8 | HasThisBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-hasthisbinding) |
| 9.1.1.2.9 | HasSuperBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-hassuperbinding) |
| 9.1.1.2.10 | WithBaseObject ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-withbaseobject) |
| 9.1.1.3 | Function Environment Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-environment-records) |
| 9.1.1.3.1 | BindThisValue ( envRec , V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-bindthisvalue) |
| 9.1.1.3.2 | HasThisBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-environment-records-hasthisbinding) |
| 9.1.1.3.3 | HasSuperBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-environment-records-hassuperbinding) |
| 9.1.1.3.4 | GetThisBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-environment-records-getthisbinding) |
| 9.1.1.3.5 | GetSuperBase ( envRec ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getsuperbase) |
| 9.1.1.4 | Global Environment Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records) |
| 9.1.1.4.1 | HasBinding ( N ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records-hasbinding-n) |
| 9.1.1.4.2 | CreateMutableBinding ( N , D ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records-createmutablebinding-n-d) |
| 9.1.1.4.3 | CreateImmutableBinding ( N , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records-createimmutablebinding-n-s) |
| 9.1.1.4.4 | InitializeBinding ( N , V ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records-initializebinding-n-v) |
| 9.1.1.4.5 | SetMutableBinding ( N , V , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records-setmutablebinding-n-v-s) |
| 9.1.1.4.6 | GetBindingValue ( N , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records-getbindingvalue-n-s) |
| 9.1.1.4.7 | DeleteBinding ( N ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records-deletebinding-n) |
| 9.1.1.4.8 | HasThisBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records-hasthisbinding) |
| 9.1.1.4.9 | HasSuperBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records-hassuperbinding) |
| 9.1.1.4.10 | WithBaseObject ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records-withbaseobject) |
| 9.1.1.4.11 | GetThisBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-global-environment-records-getthisbinding) |
| 9.1.1.4.12 | HasLexicalDeclaration ( envRec , N ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-haslexicaldeclaration) |
| 9.1.1.4.13 | HasRestrictedGlobalProperty ( envRec , N ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-hasrestrictedglobalproperty) |
| 9.1.1.4.14 | CanDeclareGlobalVar ( envRec , N ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-candeclareglobalvar) |
| 9.1.1.4.15 | CanDeclareGlobalFunction ( envRec , N ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-candeclareglobalfunction) |
| 9.1.1.4.16 | CreateGlobalVarBinding ( envRec , N , D ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createglobalvarbinding) |
| 9.1.1.4.17 | CreateGlobalFunctionBinding ( envRec , N , V , D ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createglobalfunctionbinding) |
| 9.1.1.5 | Module Environment Records | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-environment-records) |
| 9.1.1.5.1 | GetBindingValue ( N , S ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-environment-records-getbindingvalue-n-s) |
| 9.1.1.5.2 | DeleteBinding ( N ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-environment-records-deletebinding-n) |
| 9.1.1.5.3 | HasThisBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-environment-records-hasthisbinding) |
| 9.1.1.5.4 | GetThisBinding ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-module-environment-records-getthisbinding) |
| 9.1.1.5.5 | CreateImportBinding ( envRec , N , M , N2 ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createimportbinding) |
| 9.1.2 | Environment Record Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-environment-record-operations) |
| 9.1.2.1 | GetIdentifierReference ( env , name , strict ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getidentifierreference) |
| 9.1.2.2 | NewDeclarativeEnvironment ( E ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newdeclarativeenvironment) |
| 9.1.2.3 | NewObjectEnvironment ( O , W , E ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newobjectenvironment) |
| 9.1.2.4 | NewFunctionEnvironment ( F , newTarget ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newfunctionenvironment) |
| 9.1.2.5 | NewGlobalEnvironment ( G , thisValue ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newglobalenvironment) |
| 9.1.2.6 | NewModuleEnvironment ( E ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newmoduleenvironment) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 9.1.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Declarative Environment Records for lexical bindings | Supported with Limitations | [`Variable_LetBlockScope.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_LetBlockScope.js)<br>[`Variable_TemporalDeadZoneAccess.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_TemporalDeadZoneAccess.js) |  | Scope bindings and lowered storage operations implement lexical initialization, temporal-dead-zone checks, immutable bindings and closure capture. Script-mode global lexical bindings belong to the realm's RuntimeExecutionContext and remain separate from global-object properties. General eval is unsupported outside the compiler's narrow recognized direct-eval literal forms. |

### 9.1.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-object-environment-records))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Object Environment Records (with-based environments) | Supported with Limitations | `tests/Jroc.Tests/ValidatorTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/with/VariableWithCompletionExecutionTests.cs` | `test/language/statements/with/get-binding-value-idref-with-proxy-env.js`<br>`test/language/statements/with/get-mutable-binding-binding-deleted-in-get-unscopables-strict-mode.js`<br>`test/language/statements/with/set-mutable-binding-idref-with-proxy-env.js`<br>`test/language/statements/with/set-mutable-binding-idref-compound-assign-with-proxy-env.js` | Non-strict with environments implement dynamic HasBinding with Symbol.unscopables, separate GetBindingValue and SetMutableBinding existence checks, strict-mode errors after deletion, observable Proxy trap/receiver ordering and property deletion. Identifier calls use the with object as their receiver without replacing lexical this or super bindings. Primitive values are boxed once and the object environment is retained by closures. Declarations keep their declarative/global var-hoisting targets; immutable lexical bindings are not created on with objects. Strict-mode with syntax is rejected, and general eval-dependent environment behavior remains unsupported. The complete pinned with-statement area has 164 branch-locally verified cases and 17 explicit eval exclusions; these are not central trusted acceptance results. |

### 9.1.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-function-environment-records))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Function Environment Records (this/super/new.target plumbing) | Supported with Limitations | [`Function_NewTarget_NewVsCall.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NewTarget_NewVsCall.js)<br>[`Function_NewTarget_Arrow_Inherits.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NewTarget_Arrow_Inherits.js)<br>[`Classes_Inheritance_SuperMethodCall.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_Inheritance_SuperMethodCall.js) |  | RuntimeServices and lowering carry this/new.target/super behavior for supported forms, with subset coverage of exotic function cases. |

### 9.1.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-global-environment-records))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Global Environment Records (script/CommonJS global bindings) | Supported with Limitations | [`CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js`](../../../tests/Jroc.Tests/CommonJS/JavaScript/CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js)<br>`tests/Jroc.Tests/RuntimeExecutionContextTests.cs`<br>`tests/Jroc.Tests/ValidatorTests.cs` |  | Script-mode declarations use realm-owned lexical bindings and declared-name sets, separate from GlobalThis properties. Global declaration instantiation checks lexical conflicts, restricted properties and global-object extensibility. CommonJS wrappers and ESM modules have separate module-local bindings. General eval-driven declaration instantiation remains unsupported. |

### 9.1.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-module-environment-records))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Module Environment Records | Supported with Limitations | [`Import_LiveBindings_Named.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_LiveBindings_Named.js)<br>[`Import_LiveBindings_Cycle.js`](../../../tests/Jroc.Tests/Import/JavaScript/Import_LiveBindings_Cycle.js)<br>`tests/Jroc.Tests/Import/ImportRewriteErrorTests.cs`<br>`tests/Jroc.Tests/RuntimeModuleStateTests.cs` |  | Native static ESM lowering creates live export binding cells, forwarding imports and re-exports, temporal-dead-zone checks and immutable imported bindings. The linker rejects missing or ambiguous exports and invalid import writes/deletes. Binding cells and module namespaces belong to the realm's RuntimeModuleState. Common cyclic graphs are supported; host loading hooks and full general asynchronous cyclic-module evaluation remain limited, as documented in section 16.2. |

### 9.1.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-getidentifierreference))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| GetIdentifierReference over nested scope chains | Supported with Limitations | `tests/Jroc.Tests/SymbolTableBuilderTests.cs`<br>[`Variable_LetBlockScope.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_LetBlockScope.js) |  | Identifier resolution is implemented through symbol table + scope chain planning/lowering for supported syntax. |

### 9.1.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-newdeclarativeenvironment))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| NewDeclarativeEnvironment behavior in block/function scopes | Supported with Limitations | `tests/Jroc.Tests/SymbolTableBuilderTests.cs`<br>[`ControlFlow_ForOf_Let_PerIterationBinding.js`](../../../tests/Jroc.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Let_PerIterationBinding.js) |  | Compiler materializes declarative environments as scope instances, including loop-head lexical environments where required. |

### 9.1.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-newobjectenvironment))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| NewObjectEnvironment for non-strict with statements | Supported with Limitations | `tests/Jroc.Test262.Tests/language/statements/with/VariableWithCompletionExecutionTests.cs` |  | Lowering evaluates and boxes the with object once, probes its dynamic bindings and retains the object for closures created inside the statement. Runtime helpers preserve unscopables, property receivers and strict assignment checks. General eval-dependent object environments remain unsupported. |

### 9.1.2.6 ([tc39.es](https://tc39.es/ecma262/#sec-newmoduleenvironment))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| NewModuleEnvironment for compiled ESM graphs | Supported with Limitations | `tests/Jroc.Tests/Import/NativeEsmOverheadTests.cs`<br>`tests/Jroc.Tests/RuntimeModuleStateTests.cs` |  | Compiler module records and symbol-table import/export plans initialize realm-owned live binding cells and namespace objects. The supported bundled graphs do not provide the complete general host loading and asynchronous cyclic-module algorithms. |
