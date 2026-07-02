<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.3: Declarations and the Variable Statement

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-07-02T16:37:58Z

JROC supports common declaration forms (`let`, `const`, `var`) and destructuring binding patterns, including temporal dead zone checks for lexical bindings, computed object binding keys, and Proxy-observable object rest behavior. Some spec-required early errors are not exhaustively covered.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.3 | Declarations and the Variable Statement | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-declarations-and-the-variable-statement) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.3.1 | Let and Const Declarations | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-let-and-const-declarations) |
| 14.3.1.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-let-and-const-declarations-static-semantics-early-errors) |
| 14.3.1.2 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-let-and-const-declarations-runtime-semantics-evaluation) |
| 14.3.2 | Variable Statement | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-variable-statement) |
| 14.3.2.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-variable-statement-runtime-semantics-evaluation) |
| 14.3.3 | Destructuring Binding Patterns | Supported | [tc39.es](https://tc39.es/ecma262/#sec-destructuring-binding-patterns) |
| 14.3.3.1 | Runtime Semantics: PropertyBindingInitialization | Supported | [tc39.es](https://tc39.es/ecma262/#sec-destructuring-binding-patterns-runtime-semantics-propertybindinginitialization) |
| 14.3.3.2 | Runtime Semantics: RestBindingInitialization | Supported | [tc39.es](https://tc39.es/ecma262/#sec-destructuring-binding-patterns-runtime-semantics-restbindinginitialization) |
| 14.3.3.3 | Runtime Semantics: KeyedBindingInitialization | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-keyedbindinginitialization) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.3.1.2 ([tc39.es](https://tc39.es/ecma262/#sec-let-and-const-declarations-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| const assignment rejection | Supported | [`Variable_ConstReassignmentError.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_ConstReassignmentError.js) |  | Reassignment throws and can be caught. |
| const declaration (basic) | Supported | [`Variable_ConstSimple.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_ConstSimple.js) | `test/language/statements/const/syntax/const.js`<br>`test/language/statements/const/syntax/with-initializer-case-expression-statement-list.js`<br>`test/language/statements/const/syntax/with-initializer-default-statement-list.js` | Basic const declarations are supported, including the single-statement contexts exercised by the current bounded test262 coverage. |
| let declarations + shadowing | Supported | [`Variable_LetShadowing.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_LetShadowing.js)<br>[`Variable_LetNestedShadowingChain.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_LetNestedShadowingChain.js)<br>[`Variable_LetFunctionNestedShadowing.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_LetFunctionNestedShadowing.js)<br>[`Variable_LetBlockScope.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_LetBlockScope.js) |  |  |
| temporal dead zone (TDZ) | Supported | [`Variable_TemporalDeadZoneAccess.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_TemporalDeadZoneAccess.js)<br>[`Variable_TemporalDeadZoneCapturedRead.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_TemporalDeadZoneCapturedRead.js)<br>[`Variable_TemporalDeadZoneShadowing.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_TemporalDeadZoneShadowing.js) |  | Reads of lexical bindings throw a ReferenceError before initialization, including captured reads and nested shadowing cases that must still preserve access to the outer initialized binding. |

### 14.3.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-variable-statement-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| var hoisting edge cases | Supported with Limitations |  |  | Basic `var` behavior is exercised, but hoisting/redeclaration edge cases are not explicitly covered by dedicated tests. |
| var statement (basic in functions and global) | Supported | [`Function_GlobalFunctionWithArrayIteration.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_GlobalFunctionWithArrayIteration.js)<br>[`Function_NestedFunctionAccessesMultipleScopes.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NestedFunctionAccessesMultipleScopes.js) |  | Covers local `var` bindings, `var` in for-loops, and nested function scopes. |

### 14.3.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-destructuring-binding-patterns-runtime-semantics-propertybindinginitialization))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| array destructuring binding (basic, defaults, rest) | Supported | [`Variable_ArrayDestructuring_Basic.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_ArrayDestructuring_Basic.js)<br>[`Variable_ArrayDestructuring_DefaultsAndRest.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_ArrayDestructuring_DefaultsAndRest.js)<br>`tests/Jroc.Test262.Tests/language/statements/const/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/for/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/statements/variable/ExecutionTests.cs`<br>[`gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js`](../../../tests/Jroc.Test262.Tests/language/expressions/object/dstr/JavaScript/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js) | `test/language/destructuring/binding/syntax/array-pattern-with-no-elements.js`<br>`test/language/destructuring/binding/syntax/array-pattern-with-elisions.js`<br>`test/language/destructuring/binding/syntax/array-elements-with-initializer.js`<br>`test/language/destructuring/binding/syntax/array-elements-with-object-patterns.js`<br>`test/language/destructuring/binding/syntax/array-rest-elements.js`<br>`test/language/destructuring/binding/syntax/recursive-array-and-object-patterns.js`<br>`test/language/expressions/object/dstr/gen-meth-dflt-ary-ptrn-elem-id-init-fn-name-cover.js` | Array binding patterns cover empty patterns, elisions, default initializers, nested object patterns, rest elements, and recursive combinations backed by the current bounded test262 coverage. Statement-position binding cases in `const`, `var`, and `for` headers now also follow the expected iterator-consumption behavior. Covered default initializer cases also infer anonymous function names from the target binding. |
| destructuring assignment (basic) | Supported | [`Variable_DestructuringAssignment_Basic.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_DestructuringAssignment_Basic.js) |  |  |
| destructuring errors on null/undefined | Supported | [`Variable_Destructuring_NullOrUndefined_ThrowsNodeMessage.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_Destructuring_NullOrUndefined_ThrowsNodeMessage.js) |  |  |
| object destructuring binding (basic) | Supported | [`Variable_ObjectDestructuring_Basic.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Basic.js)<br>[`Variable_ObjectDestructuring_Captured.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Captured.js) | `test/language/destructuring/binding/syntax/object-pattern-with-no-property-list.js`<br>`test/language/destructuring/binding/syntax/property-list-single-name-bindings.js`<br>`test/language/destructuring/binding/syntax/property-list-bindings-elements.js`<br>`test/language/destructuring/binding/syntax/property-list-with-property-list.js` | Object binding patterns are supported for empty, single-name, aliased, and nested property-list forms covered by the current bounded test262 ports. |
| object destructuring defaults | Supported | [`Variable_ObjectDestructuring_WithDefaults.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_ObjectDestructuring_WithDefaults.js)<br>[`Variable_NestedDestructuring_Defaults.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_NestedDestructuring_Defaults.js)<br>[`obj-ptrn-id-init-fn-name-gen.js`](../../../tests/Jroc.Test262.Tests/language/expressions/function/dstr/JavaScript/obj-ptrn-id-init-fn-name-gen.js) | `test/language/expressions/function/dstr/obj-ptrn-id-init-fn-name-gen.js` | Object binding defaults are supported, including anonymous generator/function default initializers that infer the target binding name in covered parameter-binding contexts. |

### 14.3.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-destructuring-binding-patterns-runtime-semantics-restbindinginitialization))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| object destructuring rest | Supported | [`Variable_ObjectDestructuring_Rest.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Rest.js) |  |  |
| object rest binding preserves Proxy ownKeys/descriptor/get ordering | Supported |  | `test/language/expressions/object/dstr/object-rest-proxy-ownkeys-returned-keys-order.js`<br>`test/language/expressions/object/dstr/object-rest-proxy-gopd-not-called-on-excluded-keys.js`<br>`test/language/expressions/object/dstr/object-rest-proxy-get-not-called-on-dontenum-keys.js` | Object rest binding now preserves Proxy `ownKeys` order, skips excluded keys before `getOwnPropertyDescriptor`, and only performs `get` on enumerable included keys. |

### 14.3.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-keyedbindinginitialization))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| computed object binding keys preserve with-environment lookup ordering | Supported |  | `test/language/destructuring/binding/keyed-destructuring-property-reference-target-evaluation-order-with-bindings.js` | Computed object binding names are supported in binding patterns, including the observable lookup ordering exercised by sloppy-mode `with` environments and default initializers. |

