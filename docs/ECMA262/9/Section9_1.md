<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.1: Environment Records

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

JS2IL models lexical/function/global environments through compiler-managed scope structures and runtime helpers rather than first-class ECMA-262 Environment Record objects.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.1 | Environment Records | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-environment-records) |

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
| 9.1.1.2 | Object Environment Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records) |
| 9.1.1.2.1 | HasBinding ( N ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-hasbinding-n) |
| 9.1.1.2.2 | CreateMutableBinding ( N , D ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-createmutablebinding-n-d) |
| 9.1.1.2.3 | CreateImmutableBinding ( N , S ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-createimmutablebinding-n-s) |
| 9.1.1.2.4 | InitializeBinding ( N , V ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-initializebinding-n-v) |
| 9.1.1.2.5 | SetMutableBinding ( N , V , S ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-setmutablebinding-n-v-s) |
| 9.1.1.2.6 | GetBindingValue ( N , S ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-getbindingvalue-n-s) |
| 9.1.1.2.7 | DeleteBinding ( N ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-deletebinding-n) |
| 9.1.1.2.8 | HasThisBinding ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-hasthisbinding) |
| 9.1.1.2.9 | HasSuperBinding ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-hassuperbinding) |
| 9.1.1.2.10 | WithBaseObject ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-object-environment-records-withbaseobject) |
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
| 9.1.1.5 | Module Environment Records | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-module-environment-records) |
| 9.1.1.5.1 | GetBindingValue ( N , S ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-module-environment-records-getbindingvalue-n-s) |
| 9.1.1.5.2 | DeleteBinding ( N ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-module-environment-records-deletebinding-n) |
| 9.1.1.5.3 | HasThisBinding ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-module-environment-records-hasthisbinding) |
| 9.1.1.5.4 | GetThisBinding ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-module-environment-records-getthisbinding) |
| 9.1.1.5.5 | CreateImportBinding ( envRec , N , M , N2 ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-createimportbinding) |
| 9.1.2 | Environment Record Operations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-environment-record-operations) |
| 9.1.2.1 | GetIdentifierReference ( env , name , strict ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getidentifierreference) |
| 9.1.2.2 | NewDeclarativeEnvironment ( E ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newdeclarativeenvironment) |
| 9.1.2.3 | NewObjectEnvironment ( O , W , E ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-newobjectenvironment) |
| 9.1.2.4 | NewFunctionEnvironment ( F , newTarget ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newfunctionenvironment) |
| 9.1.2.5 | NewGlobalEnvironment ( G , thisValue ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-newglobalenvironment) |
| 9.1.2.6 | NewModuleEnvironment ( E ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-newmoduleenvironment) |

## Support

Feature-level support tracking with test script references.

### 9.1.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-declarative-environment-records))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Declarative Environment Records for lexical bindings | Supported with Limitations | [`Variable_LetBlockScope.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetBlockScope.js)<br>[`Variable_TemporalDeadZoneAccess.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_TemporalDeadZoneAccess.js) | Lexical bindings are modeled by scope bindings and lowered storage operations; exact spec object shapes/invariants are approximated. |

### 9.1.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-object-environment-records))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Object Environment Records (with-based environments) | Not Yet Supported | `Js2IL.Tests/ValidatorTests.cs` | with statement/object environment semantics are intentionally rejected in validation. |

### 9.1.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-function-environment-records))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Function Environment Records (this/super/new.target plumbing) | Supported with Limitations | [`Function_NewTarget_NewVsCall.js`](../../../Js2IL.Tests/Function/JavaScript/Function_NewTarget_NewVsCall.js)<br>[`Function_NewTarget_Arrow_Inherits.js`](../../../Js2IL.Tests/Function/JavaScript/Function_NewTarget_Arrow_Inherits.js)<br>[`Classes_Inheritance_SuperMethodCall.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_Inheritance_SuperMethodCall.js) | RuntimeServices and lowering carry this/new.target/super behavior for supported forms, with subset coverage of exotic function cases. |

### 9.1.1.4 ([tc39.es](https://tc39.es/ecma262/#sec-global-environment-records))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Global Environment Records (script/CommonJS global bindings) | Supported with Limitations | [`CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js`](../../../Js2IL.Tests/CommonJS/JavaScript/CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js)<br>`Js2IL.Tests/ValidatorTests.cs` | Global bindings are modeled in JS2IL global/module scope and GlobalThis runtime object, but ESM/global declarative record distinctions are incomplete. |

### 9.1.1.5 ([tc39.es](https://tc39.es/ecma262/#sec-module-environment-records))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Module Environment Records | Supported with Limitations | [`Import_StaticImport_FromCjs.js`](../../../Js2IL.Tests/Import/JavaScript/Import_StaticImport_FromCjs.js)<br>[`Import_RequireEsmModule.js`](../../../Js2IL.Tests/Import/JavaScript/Import_RequireEsmModule.js) | Static import/export declarations are supported via lowering to the CommonJS runtime model. Full spec module environment records (including complete live-binding/linking semantics) remain incomplete. |

### 9.1.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-getidentifierreference))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| GetIdentifierReference over nested scope chains | Supported with Limitations | `Js2IL.Tests/SymbolTableBuilderTests.cs`<br>[`Variable_LetBlockScope.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetBlockScope.js) | Identifier resolution is implemented through symbol table + scope chain planning/lowering for supported syntax. |

### 9.1.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-newdeclarativeenvironment))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| NewDeclarativeEnvironment behavior in block/function scopes | Supported with Limitations | `Js2IL.Tests/SymbolTableBuilderTests.cs`<br>[`ControlFlow_ForOf_Let_PerIterationBinding.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Let_PerIterationBinding.js) | Compiler materializes declarative environments as scope instances, including loop-head lexical environments where required. |

