<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.7: Iteration Statements

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

do/while/for loops are supported including break/continue (with labels). for..in and for..of are implemented for common cases but do not provide full spec iterator/enumeration fidelity for all exotic objects/iterables. for await..of is rejected by the validator.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.7 | Iteration Statements | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-iteration-statements) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.7.1 | Semantics | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-iteration-statements-semantics) |
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
| 14.7.4.4 | CreatePerIterationEnvironment ( perIterationBindings ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-createperiterationenvironment) |
| 14.7.5 | The for - in , for - of , and for - await - of Statements | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements) |
| 14.7.5.1 | Static Semantics: Early Errors | Supported | [tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements-static-semantics-early-errors) |
| 14.7.5.2 | Static Semantics: IsDestructuring | Supported | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isdestructuring) |
| 14.7.5.3 | Runtime Semantics: ForDeclarationBindingInitialization | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-fordeclarationbindinginitialization) |
| 14.7.5.4 | Runtime Semantics: ForDeclarationBindingInstantiation | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-fordeclarationbindinginstantiation) |
| 14.7.5.5 | Runtime Semantics: ForInOfLoopEvaluation | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forinofloopevaluation) |
| 14.7.5.6 | ForIn/OfHeadEvaluation ( uninitializedBoundNames , expr , iterationKind ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forinofheadevaluation) |
| 14.7.5.7 | ForIn/OfBodyEvaluation ( lhs , stmt , iteratorRecord , iterationKind , lhsKind , labelSet [ , iteratorKind ] ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forin-div-ofbodyevaluation-lhs-stmt-iterator-lhskind-labelset) |
| 14.7.5.8 | Runtime Semantics: Evaluation | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements-runtime-semantics-evaluation) |
| 14.7.5.9 | EnumerateObjectProperties ( O ) | Partially Supported | [tc39.es](https://tc39.es/ecma262/#sec-enumerate-object-properties) |
| 14.7.5.10 | For-In Iterator Objects | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-for-in-iterator-objects) |
| 14.7.5.10.1 | CreateForInIterator ( object ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-createforiniterator) |
| 14.7.5.10.2 | The %ForInIteratorPrototype% Object | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%foriniteratorprototype%-object) |
| 14.7.5.10.2.1 | %ForInIteratorPrototype%.next ( ) | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-%foriniteratorprototype%.next) |
| 14.7.5.10.3 | Properties of For-In Iterator Instances | Not Yet Supported | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-for-in-iterator-instances) |

## Support

Feature-level support tracking with test script references.

### 14.7.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-dowhileloopevaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| do-while loop (CountDownFromFive) | Supported | [`ControlFlow_DoWhile_CountDownFromFive.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_CountDownFromFive.js) |  |
| do-while loop: break | Supported | [`ControlFlow_DoWhile_Break_AtThree.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_Break_AtThree.js) | break branches to loop end (LoopContext). |
| do-while loop: continue (skip even) | Supported | [`ControlFlow_DoWhile_Continue_SkipEven.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_Continue_SkipEven.js) | continue branches to the post-body test point (LoopContext). |
| do-while loop: labeled break | Supported | [`ControlFlow_DoWhile_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledBreak.js) | Supports break <label> where <label> targets an enclosing loop. |
| do-while loop: labeled continue | Supported | [`ControlFlow_DoWhile_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledContinue.js) | Supports continue <label> where <label> targets an enclosing loop. |

### 14.7.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-whileloopevaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| while loop (CountDownFromFive) | Supported | [`ControlFlow_While_CountDownFromFive.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_CountDownFromFive.js) |  |
| while loop: break | Supported | [`ControlFlow_While_Break_AtThree.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_Break_AtThree.js) | break branches to loop end (LoopContext). |
| while loop: continue (skip even) | Supported | [`ControlFlow_While_Continue_SkipEven.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_Continue_SkipEven.js) | continue branches to loop head (LoopContext). |
| while loop: labeled break | Supported | [`ControlFlow_While_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledBreak.js) | Supports break <label> where <label> targets an enclosing loop. |
| while loop: labeled continue | Supported | [`ControlFlow_While_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledContinue.js) | Supports continue <label> where <label> targets an enclosing loop. |

### 14.7.4.2 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forloopevaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| for loop (CountDownFromFive) | Supported | [`ControlFlow_ForLoop_CountDownFromFive.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_CountDownFromFive.js) |  |
| for loop (CountToFive) | Supported | [`ControlFlow_ForLoop_CountToFive.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_CountToFive.js) |  |
| for loop (GreaterThanOrEqual) | Supported | [`ControlFlow_ForLoop_GreaterThanOrEqual.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_GreaterThanOrEqual.js) |  |
| for loop (LessThanOrEqual) | Supported | [`ControlFlow_ForLoop_LessThanOrEqual.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LessThanOrEqual.js) |  |
| for loop: break | Supported | [`ControlFlow_ForLoop_Break_AtThree.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_Break_AtThree.js) | Implements break by branching to loop end label (LoopContext). |
| for loop: continue | Supported | [`ControlFlow_ForLoop_Continue_SkipEven.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_Continue_SkipEven.js) | Implements continue by branching to the update expression (LoopContext). |
| for loop: labeled break | Supported | [`ControlFlow_ForLoop_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledBreak.js) | Supports break <label> where <label> targets an enclosing loop. |
| for loop: labeled continue | Supported | [`ControlFlow_ForLoop_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledContinue.js) | Supports continue <label> where <label> targets an enclosing loop. |

### 14.7.4.4 ([tc39.es](https://tc39.es/ecma262/#sec-createperiterationenvironment))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| for-loop per-iteration lexical environment (let closure capture) | Supported | [`ControlFlow_ForLoop_LetClosureCapture.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LetClosureCapture.js)<br>[`ControlFlow_ForLoop_LetClosureCapture_Continue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LetClosureCapture_Continue.js) | Implements per-iteration environments for captured loop-head lexical bindings so closures observe 0,1,2 rather than the final value. Currently guarded to only apply when the loop-head captured bindings are the only captured bindings stored in the current leaf scope. |

### 14.7.5 ([tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| for await..of | Not Yet Supported |  | Rejected by Js2IL.Validation.JavaScriptAstValidator (ForOfStatement.Await). |

### 14.7.5.7 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forin-div-ofbodyevaluation-lhs-stmt-iterator-lhskind-labelset))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| for-in over objects (enumerate enumerable keys) | Partially Supported | [`ControlFlow_ForIn_Object_Basic.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Object_Basic.js)<br>[`ControlFlow_ForIn_Continue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Continue.js)<br>[`ControlFlow_ForIn_Break.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Break.js)<br>[`ControlFlow_ForIn_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledContinue.js)<br>[`ControlFlow_ForIn_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledBreak.js) | Lowered to an index loop over JavaScriptRuntime.Object.GetEnumerableKeys(object). Minimal semantics: supports ExpandoObject (object literals), JS Array/Int32Array/string index keys, and IDictionary keys; does not currently model full prototype-chain enumeration rules. |
| for-of over arrays (enumerate values) | Partially Supported | [`ControlFlow_ForOf_Array_Basic.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Array_Basic.js)<br>[`ControlFlow_ForOf_Continue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Continue.js)<br>[`ControlFlow_ForOf_Break.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Break.js)<br>[`ControlFlow_ForOf_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledContinue.js)<br>[`ControlFlow_ForOf_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledBreak.js) | Lowered to an index loop over a normalized iterable (JavaScriptRuntime.Object.NormalizeForOfIterable), then accessed via JavaScriptRuntime.Object.GetLength(object) + GetItem(object, double). Supports arrays, strings, typed arrays, and .NET IEnumerable (via Array.from), but does not implement full JS iterator protocol (Symbol.iterator). |

