<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.2: Block

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-08T17:35:30Z

Block statements are supported, including lexical scoping for `let`/`const` declarations and temporal dead zone checks for lexical bindings. Some early-error edge cases and full spec fidelity around declaration instantiation are not exhaustively validated.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.2 | Block | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-block) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.2.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-block-static-semantics-early-errors) |
| 14.2.2 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-block-runtime-semantics-evaluation) |
| 14.2.3 | BlockDeclarationInstantiation ( code , env ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-blockdeclarationinstantiation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-block-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| block early-error edge cases | Supported with Limitations |  |  | Relies on Acornima parsing plus JS2IL validation/symbol table; not all early-error combinations are explicitly covered by tests. |

### 14.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-block-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| block statement (basic execution) | Supported | [`ArrowFunction_BlockBody_Return.js`](../../../tests/Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_BlockBody_Return.js) |  | Statement lists within blocks execute normally; common block bodies are exercised widely (e.g., function bodies). |

### 14.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-blockdeclarationinstantiation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| lexical block scope (let shadowing) | Supported | [`Variable_LetBlockScope.js`](../../../tests/Js2IL.Tests/Variable/JavaScript/Variable_LetBlockScope.js) | `test/language/block-scope/leave/verify-context-in-finally-block.js`<br>`test/language/block-scope/leave/verify-context-in-for-loop-block.js`<br>`test/language/block-scope/leave/verify-context-in-labelled-block.js`<br>`test/language/block-scope/shadowing/lookup-in-and-through-block-contexts.js`<br>`test/language/block-scope/shadowing/dynamic-lookup-from-closure.js`<br>`test/language/block-scope/shadowing/catch-parameter-shadowing-let-declaration.js` | Lexical block environments preserve shadowing through nested blocks, labelled/finally exits, and closure lookups. Current bounded test262 coverage also exercises catch-parameter shadowing and block-scope restoration after control flow leaves the block. |
| lexical declarations inside loop blocks | Supported | [`ControlFlow_DoWhile_NestedLet.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_NestedLet.js) |  |  |
| temporal dead zone (TDZ) in blocks | Supported | [`Variable_TemporalDeadZoneAccess.js`](../../../tests/Js2IL.Tests/Variable/JavaScript/Variable_TemporalDeadZoneAccess.js)<br>[`Variable_TemporalDeadZoneShadowing.js`](../../../tests/Js2IL.Tests/Variable/JavaScript/Variable_TemporalDeadZoneShadowing.js) |  | Reads of block-scoped lexical bindings throw a ReferenceError until the declaration initializes the binding, and shadowing applies the TDZ only to the inner lexical binding. |

