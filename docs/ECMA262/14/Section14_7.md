<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.7: Iteration Statements

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-05-20T16:42:36Z

do/while/for loops are supported including break/continue (with labels). for..of uses the iterator protocol; for..in uses a dedicated For-In Iterator (mutation-aware key enumeration) but does not yet provide full spec fidelity for all exotic objects. for await..of is supported in async functions.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.7 | Iteration Statements | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteration-statements) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.7.1 | Semantics | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-iteration-statements-semantics) |
| 14.7.1.1 | LoopContinues ( completion , labelSet ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-loopcontinues) |
| 14.7.1.2 | Runtime Semantics: LoopEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-loopevaluation) |
| 14.7.2 | The do - while Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-do-while-statement) |
| 14.7.2.1 | Static Semantics: Early Errors | Supported | [tc39.es](https://tc39.es/ecma262/#sec-do-while-statement-static-semantics-early-errors) |
| 14.7.2.2 | Runtime Semantics: DoWhileLoopEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-dowhileloopevaluation) |
| 14.7.3 | The while Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-while-statement) |
| 14.7.3.1 | Static Semantics: Early Errors | Supported | [tc39.es](https://tc39.es/ecma262/#sec-while-statement-static-semantics-early-errors) |
| 14.7.3.2 | Runtime Semantics: WhileLoopEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-whileloopevaluation) |
| 14.7.4 | The for Statement | Supported | [tc39.es](https://tc39.es/ecma262/#sec-for-statement) |
| 14.7.4.1 | Static Semantics: Early Errors | Supported | [tc39.es](https://tc39.es/ecma262/#sec-for-statement-static-semantics-early-errors) |
| 14.7.4.2 | Runtime Semantics: ForLoopEvaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forloopevaluation) |
| 14.7.4.3 | ForBodyEvaluation ( test , increment , stmt , perIterationBindings , labelSet ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-forbodyevaluation) |
| 14.7.4.4 | CreatePerIterationEnvironment ( perIterationBindings ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-createperiterationenvironment) |
| 14.7.5 | The for - in , for - of , and for - await - of Statements | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements) |
| 14.7.5.1 | Static Semantics: Early Errors | Supported | [tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements-static-semantics-early-errors) |
| 14.7.5.2 | Static Semantics: IsDestructuring | Supported | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isdestructuring) |
| 14.7.5.3 | Runtime Semantics: ForDeclarationBindingInitialization | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-fordeclarationbindinginitialization) |
| 14.7.5.4 | Runtime Semantics: ForDeclarationBindingInstantiation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-fordeclarationbindinginstantiation) |
| 14.7.5.5 | Runtime Semantics: ForInOfLoopEvaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forinofloopevaluation) |
| 14.7.5.6 | ForIn/OfHeadEvaluation ( uninitializedBoundNames , expr , iterationKind ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forinofheadevaluation) |
| 14.7.5.7 | ForIn/OfBodyEvaluation ( lhs , stmt , iteratorRecord , iterationKind , lhsKind , labelSet [ , iteratorKind ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forin-div-ofbodyevaluation-lhs-stmt-iterator-lhskind-labelset) |
| 14.7.5.8 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements-runtime-semantics-evaluation) |
| 14.7.5.9 | EnumerateObjectProperties ( O ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-enumerate-object-properties) |
| 14.7.5.10 | For-In Iterator Objects | Supported | [tc39.es](https://tc39.es/ecma262/#sec-for-in-iterator-objects) |
| 14.7.5.10.1 | CreateForInIterator ( object ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-createforiniterator) |
| 14.7.5.10.2 | The %ForInIteratorPrototype% Object | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-%foriniteratorprototype%-object) |
| 14.7.5.10.2.1 | %ForInIteratorPrototype%.next ( ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-%foriniteratorprototype%.next) |
| 14.7.5.10.3 | Properties of For-In Iterator Instances | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-for-in-iterator-instances) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 14.7.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-dowhileloopevaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| do-while loop (CountDownFromFive) | Supported | [`ControlFlow_DoWhile_CountDownFromFive.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_CountDownFromFive.js) |  |  |
| do-while loop: break | Supported | [`ControlFlow_DoWhile_Break_AtThree.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_Break_AtThree.js) |  | break branches to loop end (LoopContext). |
| do-while loop: continue (skip even) | Supported | [`ControlFlow_DoWhile_Continue_SkipEven.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_Continue_SkipEven.js) |  | continue branches to the post-body test point (LoopContext). |
| do-while loop: labeled break | Supported | [`ControlFlow_DoWhile_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledBreak.js) |  | Supports break <label> where <label> targets an enclosing loop. |
| do-while loop: labeled continue | Supported | [`ControlFlow_DoWhile_LabeledContinue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledContinue.js) |  | Supports continue <label> where <label> targets an enclosing loop. |

### 14.7.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-whileloopevaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| while loop (CountDownFromFive) | Supported | [`ControlFlow_While_CountDownFromFive.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_CountDownFromFive.js) |  |  |
| while loop: break | Supported | [`ControlFlow_While_Break_AtThree.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_Break_AtThree.js) |  | break branches to loop end (LoopContext). |
| while loop: continue (skip even) | Supported | [`ControlFlow_While_Continue_SkipEven.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_Continue_SkipEven.js) |  | continue branches to loop head (LoopContext). |
| while loop: labeled break | Supported | [`ControlFlow_While_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledBreak.js) |  | Supports break <label> where <label> targets an enclosing loop. |
| while loop: labeled continue | Supported | [`ControlFlow_While_LabeledContinue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledContinue.js) |  | Supports continue <label> where <label> targets an enclosing loop. |

### 14.7.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forloopevaluation))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| for loop (CountDownFromFive) | Supported | [`ControlFlow_ForLoop_CountDownFromFive.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_CountDownFromFive.js) |  |  |
| for loop (CountToFive) | Supported | [`ControlFlow_ForLoop_CountToFive.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_CountToFive.js) |  |  |
| for loop (GreaterThanOrEqual) | Supported | [`ControlFlow_ForLoop_GreaterThanOrEqual.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_GreaterThanOrEqual.js) |  |  |
| for loop (LessThanOrEqual) | Supported | [`ControlFlow_ForLoop_LessThanOrEqual.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LessThanOrEqual.js) |  |  |
| for loop test expression uses JavaScript truthiness | Supported |  | `test/language/statements/for/12.6.3_2-3-a-ii-18.js` | Loop test expressions lower through JavaScript truthiness coercion, so falsy strings such as "" terminate the loop instead of behaving like non-null CLR references. |
| for loop: break | Supported | [`ControlFlow_ForLoop_Break_AtThree.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_Break_AtThree.js) |  | Implements break by branching to loop end label (LoopContext). |
| for loop: continue | Supported | [`ControlFlow_ForLoop_Continue_SkipEven.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_Continue_SkipEven.js) |  | Implements continue by branching to the update expression (LoopContext). |
| for loop: labeled break | Supported | [`ControlFlow_ForLoop_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledBreak.js) |  | Supports break <label> where <label> targets an enclosing loop. |
| for loop: labeled continue | Supported | [`ControlFlow_ForLoop_LabeledContinue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledContinue.js) |  | Supports continue <label> where <label> targets an enclosing loop. |

### 14.7.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-createperiterationenvironment))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| for-loop per-iteration lexical environment (let closure capture) | Supported | [`ControlFlow_ForLoop_LetClosureCapture.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LetClosureCapture.js)<br>[`ControlFlow_ForLoop_LetClosureCapture_Continue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LetClosureCapture_Continue.js) | `test/language/statements/for/12.6.3_2-3-a-ii-1.js`<br>`test/language/statements/for/12.6.3_2-3-a-ii-2.js`<br>`test/language/statements/for/12.6.3_2-3-a-ii-3.js`<br>`test/language/statements/for/12.6.3_2-3-a-ii-20.js`<br>`test/language/statements/for/12.6.3_2-3-a-ii-21.js` | Implements CreatePerIterationEnvironment by materializing a dedicated loop-head scope instance per iteration and capturing that scope in closures, so closures observe 0,1,2 rather than the final value. Current bounded test262 coverage also exercises fresh lexical bindings across multiple iterations and completion paths. |

### 14.7.5 ([tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| for await..of | Supported | [`Async_ForAwaitOf_Array.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_Array.js)<br>[`Async_ForAwaitOf_AsyncIterator_BreakCloses.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_AsyncIterator_BreakCloses.js)<br>[`Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js`](../../../tests/Js2IL.Tests/Async/JavaScript/Async_ForAwaitOf_SyncIteratorFallback_BreakCloses.js) |  | Lowered using the async iterator protocol (GetAsyncIterator/AsyncIteratorNext/AsyncIteratorClose), including awaiting AsyncIteratorClose on abrupt completion (break/throw/return). |

### 14.7.5.7 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forin-div-ofbodyevaluation-lhs-stmt-iterator-lhskind-labelset))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| for-in over objects (enumerate enumerable keys) | Supported | [`ControlFlow_ForIn_Object_Basic.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Object_Basic.js)<br>[`ControlFlow_ForIn_Continue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Continue.js)<br>[`ControlFlow_ForIn_Break.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Break.js)<br>[`ControlFlow_ForIn_LabeledContinue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledContinue.js)<br>[`ControlFlow_ForIn_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledBreak.js)<br>[`ControlFlow_ForIn_Mutation_DeleteAndAdd.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Mutation_DeleteAndAdd.js)<br>[`ControlFlow_ForIn_ClassFields_BaseAndDerived.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_ClassFields_BaseAndDerived.js)<br>[`ControlFlow_ForIn_Shadowing_NoDuplicateKeys.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Shadowing_NoDuplicateKeys.js)<br>[`cptn-decl-itr.js`](../../../tests/Js2IL.Test262.Tests/language/statements/for-in/JavaScript/cptn-decl-itr.js)<br>[`cptn-decl-skip-itr.js`](../../../tests/Js2IL.Test262.Tests/language/statements/for-in/JavaScript/cptn-decl-skip-itr.js)<br>[`cptn-decl-abrupt-empty.js`](../../../tests/Js2IL.Test262.Tests/language/statements/for-in/JavaScript/cptn-decl-abrupt-empty.js) | `test/language/statements/for-in/cptn-decl-itr.js`<br>`test/language/statements/for-in/cptn-decl-skip-itr.js`<br>`test/language/statements/for-in/cptn-decl-abrupt-empty.js`<br>`test/language/statements/for-in/12.6.4-1.js`<br>`test/language/statements/for-in/12.6.4-2.js`<br>`test/language/statements/for-in/head-const-fresh-binding-per-iteration.js`<br>`test/language/statements/for-in/S12.6.4_A1.js`<br>`test/language/statements/for-in/S12.6.4_A2.js`<br>`test/language/block-scope/syntax/for-in/acquire-properties-from-array.js` | Lowered via a native For-In Iterator (EnumerateObjectProperties/CreateForInIterator) and consumed through JavaScriptRuntime.Object.IteratorNext + IteratorResultDone/Value. Each next() step re-checks key presence so deletions during enumeration are respected, and current bounded test262 coverage also exercises fresh per-iteration lexical bindings for for-in heads plus block-scoped array property acquisition. Prototype chain semantics are approximated for CLR objects by walking the CLR type hierarchy (declared public instance members per type); ExpandoObject/IDictionary do not currently expose a JS-observable [[Prototype]] chain. |
| for-of over arrays (enumerate values) | Supported | [`ControlFlow_ForOf_Array_Basic.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Array_Basic.js)<br>[`ControlFlow_ForOf_Array_SymbolIterator_SparseLiteral.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Array_SymbolIterator_SparseLiteral.js)<br>[`ControlFlow_ForOf_Continue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Continue.js)<br>[`ControlFlow_ForOf_Break.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Break.js)<br>[`ControlFlow_ForOf_LabeledContinue.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledContinue.js)<br>[`ControlFlow_ForOf_LabeledBreak.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledBreak.js)<br>[`ControlFlow_ForOf_CustomIterable_IteratorProtocol.js`](../../../tests/Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_CustomIterable_IteratorProtocol.js)<br>`tests/Js2IL.Tests/Function/JavaScript/Function_Arguments_Mapped_ForOf.js` | suite `pr`<br>suite `nightly`<br>`test/language/statements/for-of/Array.prototype.Symbol.iterator.js`<br>`test/language/statements/for-of/array-contract.js`<br>`test/language/statements/for-of/array-contract-expand.js`<br>`test/language/statements/for-of/array-expand-contract.js`<br>`test/language/statements/for-of/arguments-unmapped-aliasing.js`<br>`test/language/statements/for-of/Array.prototype.entries.js`<br>`test/language/statements/for-of/Array.prototype.keys.js`<br>`test/language/statements/for-of/array-expand.js`<br>`test/language/statements/for-of/array-key-get-error.js`<br>`test/language/statements/for-of/body-dstr-assign-error.js`<br>`test/language/statements/for-of/arguments-mapped.js`<br>`test/language/statements/for-of/arguments-mapped-aliasing.js`<br>`test/language/statements/for-of/arguments-mapped-mutation.js`<br>`test/language/statements/for-of/arguments-unmapped.js`<br>`test/language/statements/for-of/arguments-unmapped-mutation.js`<br>`test/language/statements/for-of/array.js` | Lowered using the iterator protocol via JavaScriptRuntime.Object.GetIterator + iterator.next() + result.value/result.done, and invokes JavaScriptRuntime.Object.IteratorClose on abrupt completion (break/throw/return and iterator throws). Supports built-ins (Array/string/Int32Array), user-defined iterables via [Symbol.iterator], mapped and unmapped arguments objects, and a best-effort fallback for .NET IEnumerable. Sparse array literals now flow through the IR path as arrays whose elisions iterate as undefined values, so explicit array[Symbol.iterator]() and arguments-object for-of cases in the bounded test262 coverage compile successfully. |

