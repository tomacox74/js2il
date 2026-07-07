<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 8.2: Scope Analysis

[Back to Section8](Section8.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-07T21:06:59Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 8.2 | Scope Analysis | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-syntax-directed-operations-scope-analysis) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 8.2.1 | Static Semantics: BoundNames | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-boundnames) |
| 8.2.2 | Static Semantics: DeclarationPart | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-declarationpart) |
| 8.2.3 | Static Semantics: IsConstantDeclaration | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isconstantdeclaration) |
| 8.2.4 | Static Semantics: LexicallyDeclaredNames | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-lexicallydeclarednames) |
| 8.2.5 | Static Semantics: LexicallyScopedDeclarations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-lexicallyscopeddeclarations) |
| 8.2.6 | Static Semantics: VarDeclaredNames | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-vardeclarednames) |
| 8.2.7 | Static Semantics: VarScopedDeclarations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-varscopeddeclarations) |
| 8.2.8 | Static Semantics: TopLevelLexicallyDeclaredNames | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-toplevellexicallydeclarednames) |
| 8.2.9 | Static Semantics: TopLevelLexicallyScopedDeclarations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-toplevellexicallyscopeddeclarations) |
| 8.2.10 | Static Semantics: TopLevelVarDeclaredNames | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-toplevelvardeclarednames) |
| 8.2.11 | Static Semantics: TopLevelVarScopedDeclarations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-toplevelvarscopeddeclarations) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 8.2 ([tc39.es](https://tc39.es/ecma262/#sec-syntax-directed-operations-scope-analysis))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Scope analysis across binding discovery, lexical environments, hoisting, and top-level declaration tracking | Supported with Limitations | `tests/Jroc.Tests/SymbolTableBuilderTests.cs`<br>[`Variable_LetBlockScope.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_LetBlockScope.js)<br>[`CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js`](../../../tests/Jroc.Tests/CommonJS/JavaScript/CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js) | suite `language/expressions/arrow-function`<br>suite `language/expressions/class`<br>suite `language/expressions/function` | JROC's symbol-table and scope-building pipeline covers the declaration-collection and environment-boundary analysis needed by the currently supported language surface, including destructuring bindings, lexical block scopes, loop-head scopes, hoisted var/function declarations, and top-level CommonJS-style module scope tracking. Remaining limitations mainly follow unsupported grammar/runtime forms and the fact that ESM-specific top-level semantics are still outside current support. |

### 8.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-boundnames))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| BoundNames collection for declarations and binding patterns | Supported with Limitations | `tests/Jroc.Tests/SymbolTableBuilderTests.cs`<br>[`Variable_ArrayDestructuring_Basic.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_ArrayDestructuring_Basic.js)<br>[`Variable_ObjectDestructuring_Basic.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Basic.js) |  | SymbolTableBuilder records declared identifiers from variable/function/class declarations and supported destructuring patterns; unsupported pattern forms are rejected earlier. |

### 8.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isconstantdeclaration))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| IsConstantDeclaration classification | Supported with Limitations | `tests/Jroc.Tests/SymbolTableBuilderTests.cs`<br>[`Variable_ConstSimple.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_ConstSimple.js) |  | Declaration kinds are mapped to BindingKind (var/let/const) in scope analysis and reused by lowering/codegen. |

### 8.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-lexicallydeclarednames))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| expression-time lexical capture for arrow/function/class scopes | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/arrow-function/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/class/PortExpressionsBatchExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/function/PortExpressionsBatchExecutionTests.cs` |  | Scope analysis now preserves the lexical environment boundaries needed by the covered expression test262 cases, so nested arrows, class elements, and function expressions close over the correct bindings instead of leaking parameter/body scope state. |
| Lexically declared/scoped names | Supported with Limitations | `tests/Jroc.Tests/SymbolTableBuilderTests.cs`<br>[`Variable_LetBlockScope.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_LetBlockScope.js) |  | Block scopes are materialized and lexical bindings are tracked, including dedicated loop-head scopes for let/const in for/for-in/for-of. |

### 8.2.6 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-vardeclarednames))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Var-declared names and var-scoped declarations | Supported with Limitations | `tests/Jroc.Tests/SymbolTableBuilderTests.cs`<br>[`CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js`](../../../tests/Jroc.Tests/CommonJS/JavaScript/CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js) |  | Var/function declarations are hoisted to the nearest function/global scope, with module-parameter shadowing behavior handled explicitly. |

### 8.2.8 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-toplevellexicallydeclarednames))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Top-level lexical/var name tracking | Supported with Limitations | `tests/Jroc.Tests/SymbolTableBuilderTests.cs`<br>[`CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js`](../../../tests/Jroc.Tests/CommonJS/JavaScript/CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js) |  | Top-level declarations are tracked in the module/global scope used by JROC's CommonJS-style compilation model; ESM-specific top-level semantics are outside current support. |

