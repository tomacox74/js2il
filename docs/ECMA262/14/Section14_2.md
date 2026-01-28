<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.2: Block

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

Block statements are supported, including lexical scoping for `let`/`const` declarations. Some early-error edge cases and full spec fidelity around declaration instantiation are not exhaustively validated.

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

Feature-level support tracking with test script references.

### 14.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-block-static-semantics-early-errors))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| block early-error edge cases | Supported with Limitations |  | Relies on Acornima parsing plus JS2IL validation/symbol table; not all early-error combinations are explicitly covered by tests. |

### 14.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-block-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| block statement (basic execution) | Supported | [`ArrowFunction_BlockBody_Return.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_BlockBody_Return.js) | Statement lists within blocks execute normally; common block bodies are exercised widely (e.g., function bodies). |

### 14.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-blockdeclarationinstantiation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| lexical block scope (let shadowing) | Supported | [`Variable_LetBlockScope.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetBlockScope.js) |  |
| lexical declarations inside loop blocks | Supported | [`ControlFlow_DoWhile_NestedLet.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_NestedLet.js) |  |
| temporal dead zone (TDZ) in blocks | Not Yet Supported | [`Variable_TemporalDeadZoneAccess.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_TemporalDeadZoneAccess.js) | A TDZ test exists but is currently skipped in the test suite (try/catch + TDZ runtime behavior not implemented yet). |

