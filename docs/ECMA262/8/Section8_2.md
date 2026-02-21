<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 8.2: Scope Analysis

[Back to Section8](Section8.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 8.2 | Scope Analysis | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-syntax-directed-operations-scope-analysis) |

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

Feature-level support tracking with test script references.

### 8.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-boundnames))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| BoundNames collection for declarations and binding patterns | Supported with Limitations | `Js2IL.Tests/SymbolTableBuilderTests.cs`<br>[`Variable_ArrayDestructuring_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ArrayDestructuring_Basic.js)<br>[`Variable_ObjectDestructuring_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Basic.js) | SymbolTableBuilder records declared identifiers from variable/function/class declarations and supported destructuring patterns; unsupported pattern forms are rejected earlier. |

### 8.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isconstantdeclaration))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| IsConstantDeclaration classification | Supported with Limitations | `Js2IL.Tests/SymbolTableBuilderTests.cs`<br>[`Variable_ConstSimple.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ConstSimple.js) | Declaration kinds are mapped to BindingKind (var/let/const) in scope analysis and reused by lowering/codegen. |

### 8.2.4 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-lexicallydeclarednames))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Lexically declared/scoped names | Supported with Limitations | `Js2IL.Tests/SymbolTableBuilderTests.cs`<br>[`Variable_LetBlockScope.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetBlockScope.js) | Block scopes are materialized and lexical bindings are tracked, including dedicated loop-head scopes for let/const in for/for-in/for-of. |

### 8.2.6 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-vardeclarednames))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Var-declared names and var-scoped declarations | Supported with Limitations | `Js2IL.Tests/SymbolTableBuilderTests.cs`<br>[`CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js`](../../../Js2IL.Tests/CommonJS/JavaScript/CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js) | Var/function declarations are hoisted to the nearest function/global scope, with module-parameter shadowing behavior handled explicitly. |

### 8.2.8 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-toplevellexicallydeclarednames))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Top-level lexical/var name tracking | Supported with Limitations | `Js2IL.Tests/SymbolTableBuilderTests.cs`<br>[`CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js`](../../../Js2IL.Tests/CommonJS/JavaScript/CommonJS_FunctionDeclaration_Hoisting_BeforeUse.js) | Top-level declarations are tracked in the module/global scope used by JS2IL's CommonJS-style compilation model; ESM-specific top-level semantics are outside current support. |

