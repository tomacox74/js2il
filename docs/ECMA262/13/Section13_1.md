<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.1: Identifiers

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-06-24T17:00:14Z

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.1 | Identifiers | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-identifiers) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.1.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-identifiers-static-semantics-early-errors) |
| 13.1.2 | Static Semantics: StringValue | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-stringvalue) |
| 13.1.3 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-identifiers-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-identifiers-static-semantics-early-errors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Identifier early errors and reserved-word rejection | Supported with Limitations | `tests/Jroc.Test262.Tests/language/identifiers/ExecutionTests.cs` |  | Identifier syntax and reserved-word forms are parser-backed by Acornima and covered by imported language/identifiers cases. This remains limited because the documentation has not been audited against the full Unicode identifier matrix. |

### 13.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-stringvalue))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Identifier StringValue | Supported with Limitations | `tests/Jroc.Test262.Tests/language/identifiers/ExecutionTests.cs` |  | Identifier names are carried through Acornima AST nodes into symbol-table and variable-resolution names. The status remains limited until escaped/Unicode identifier-name coverage is exhaustively mapped. |

### 13.1.3 ([tc39.es](https://tc39.es/ecma262/#sec-identifiers-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Identifier reference evaluation | Supported with Limitations | `tests/Jroc.Tests/Variable/JavaScript/Variable_VarDeclaration.js`<br>[`Variable_LetBlockScope.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_LetBlockScope.js)<br>[`Function_GlobalFunctionLogsGlobalVariable.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_GlobalFunctionLogsGlobalVariable.js) |  | Variable and function identifier references are resolved through symbol-table bindings and generated scope storage. The status remains limited because TDZ and some binding-edge cases are documented elsewhere as incomplete. |

