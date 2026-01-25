<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.3: Declarations and the Variable Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

JS2IL supports common declaration forms (`let`, `const`, `var`) and destructuring binding patterns. Some spec-required early errors are not exhaustively covered, and TDZ behavior currently has a skipped test (so not fully implemented/validated yet).

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.3 | Declarations and the Variable Statement | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-declarations-and-the-variable-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.3.1 | Let and Const Declarations | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-let-and-const-declarations) |
| 14.3.1.1 | Static Semantics: Early Errors | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-let-and-const-declarations-static-semantics-early-errors) |
| 14.3.1.2 | Runtime Semantics: Evaluation | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-let-and-const-declarations-runtime-semantics-evaluation) |
| 14.3.2 | Variable Statement | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-variable-statement) |
| 14.3.2.1 | Runtime Semantics: Evaluation | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-variable-statement-runtime-semantics-evaluation) |
| 14.3.3 | Destructuring Binding Patterns | Supported | [tc39.es](https://tc39.es/ecma262/#sec-destructuring-binding-patterns) |
| 14.3.3.1 | Runtime Semantics: PropertyBindingInitialization | Supported | [tc39.es](https://tc39.es/ecma262/#sec-destructuring-binding-patterns-runtime-semantics-propertybindinginitialization) |
| 14.3.3.2 | Runtime Semantics: RestBindingInitialization | Supported | [tc39.es](https://tc39.es/ecma262/#sec-destructuring-binding-patterns-runtime-semantics-restbindinginitialization) |
| 14.3.3.3 | Runtime Semantics: KeyedBindingInitialization | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-keyedbindinginitialization) |

## Support

Feature-level support tracking with test script references.

### 14.3.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-let-and-const-declarations-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| const assignment rejection | Supported | [`Variable_ConstReassignmentError.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ConstReassignmentError.js) | Reassignment throws and can be caught. |
| const declaration (basic) | Supported | [`Variable_ConstSimple.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ConstSimple.js) |  |
| let declarations + shadowing | Supported | [`Variable_LetShadowing.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetShadowing.js)<br>[`Variable_LetNestedShadowingChain.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetNestedShadowingChain.js)<br>[`Variable_LetFunctionNestedShadowing.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetFunctionNestedShadowing.js)<br>[`Variable_LetBlockScope.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetBlockScope.js) |  |
| temporal dead zone (TDZ) | Not Yet Supported | [`Variable_TemporalDeadZoneAccess.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_TemporalDeadZoneAccess.js) | A TDZ test exists but is currently skipped in the test suite (try/catch + TDZ runtime behavior not implemented/validated yet). |

### 14.3.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-variable-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| var hoisting edge cases | Partially Supported |  | Basic `var` behavior is exercised, but hoisting/redeclaration edge cases are not explicitly covered by dedicated tests. |
| var statement (basic in functions and global) | Supported | [`Function_GlobalFunctionWithArrayIteration.js`](../../../Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionWithArrayIteration.js)<br>[`Function_NestedFunctionAccessesMultipleScopes.js`](../../../Js2IL.Tests/Function/JavaScript/Function_NestedFunctionAccessesMultipleScopes.js) | Covers local `var` bindings, `var` in for-loops, and nested function scopes. |

### 14.3.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-destructuring-binding-patterns-runtime-semantics-propertybindinginitialization))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| array destructuring binding (basic, defaults, rest) | Supported | [`Variable_ArrayDestructuring_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ArrayDestructuring_Basic.js)<br>[`Variable_ArrayDestructuring_DefaultsAndRest.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ArrayDestructuring_DefaultsAndRest.js) |  |
| destructuring assignment (basic) | Supported | [`Variable_DestructuringAssignment_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_DestructuringAssignment_Basic.js) |  |
| destructuring errors on null/undefined | Supported | [`Variable_Destructuring_NullOrUndefined_ThrowsNodeMessage.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_Destructuring_NullOrUndefined_ThrowsNodeMessage.js) |  |
| object destructuring binding (basic) | Supported | [`Variable_ObjectDestructuring_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Basic.js)<br>[`Variable_ObjectDestructuring_Captured.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Captured.js) |  |
| object destructuring defaults | Supported | [`Variable_ObjectDestructuring_WithDefaults.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_WithDefaults.js)<br>[`Variable_NestedDestructuring_Defaults.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_NestedDestructuring_Defaults.js) |  |

### 14.3.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-destructuring-binding-patterns-runtime-semantics-restbindinginitialization))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| object destructuring rest | Supported | [`Variable_ObjectDestructuring_Rest.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Rest.js) |  |

