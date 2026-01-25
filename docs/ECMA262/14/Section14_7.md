<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 14.7: Iteration Statements

[Back to Section14](Section14.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 14.7 | Iteration Statements | Supported | [tc39.es](https://tc39.es/ecma262/#sec-iteration-statements) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 14.7.1 | Semantics | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-iteration-statements-semantics) |
| 14.7.1.1 | LoopContinues ( completion , labelSet ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-loopcontinues) |
| 14.7.1.2 | Runtime Semantics: LoopEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-loopevaluation) |
| 14.7.2 | The do - while Statement | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-do-while-statement) |
| 14.7.2.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-do-while-statement-static-semantics-early-errors) |
| 14.7.2.2 | Runtime Semantics: DoWhileLoopEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-dowhileloopevaluation) |
| 14.7.3 | The while Statement | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-while-statement) |
| 14.7.3.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-while-statement-static-semantics-early-errors) |
| 14.7.3.2 | Runtime Semantics: WhileLoopEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-whileloopevaluation) |
| 14.7.4 | The for Statement | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-for-statement) |
| 14.7.4.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-for-statement-static-semantics-early-errors) |
| 14.7.4.2 | Runtime Semantics: ForLoopEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forloopevaluation) |
| 14.7.4.3 | ForBodyEvaluation ( test , increment , stmt , perIterationBindings , labelSet ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-forbodyevaluation) |
| 14.7.4.4 | CreatePerIterationEnvironment ( perIterationBindings ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-createperiterationenvironment) |
| 14.7.5 | The for - in , for - of , and for - await - of Statements | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements) |
| 14.7.5.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements-static-semantics-early-errors) |
| 14.7.5.2 | Static Semantics: IsDestructuring | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isdestructuring) |
| 14.7.5.3 | Runtime Semantics: ForDeclarationBindingInitialization | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-fordeclarationbindinginitialization) |
| 14.7.5.4 | Runtime Semantics: ForDeclarationBindingInstantiation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-fordeclarationbindinginstantiation) |
| 14.7.5.5 | Runtime Semantics: ForInOfLoopEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forinofloopevaluation) |
| 14.7.5.6 | ForIn/OfHeadEvaluation ( uninitializedBoundNames , expr , iterationKind ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forinofheadevaluation) |
| 14.7.5.7 | ForIn/OfBodyEvaluation ( lhs , stmt , iteratorRecord , iterationKind , lhsKind , labelSet [ , iteratorKind ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-forin-div-ofbodyevaluation-lhs-stmt-iterator-lhskind-labelset) |
| 14.7.5.8 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements-runtime-semantics-evaluation) |
| 14.7.5.9 | EnumerateObjectProperties ( O ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-enumerate-object-properties) |
| 14.7.5.10 | For-In Iterator Objects | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-for-in-iterator-objects) |
| 14.7.5.10.1 | CreateForInIterator ( object ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-createforiniterator) |
| 14.7.5.10.2 | The %ForInIteratorPrototype% Object | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%foriniteratorprototype%-object) |
| 14.7.5.10.2.1 | %ForInIteratorPrototype%.next ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-%foriniteratorprototype%.next) |
| 14.7.5.10.3 | Properties of For-In Iterator Instances | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-properties-of-for-in-iterator-instances) |

## Support

Feature-level support tracking with test script references.

### 14.7.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-loopcontinues))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| do-while loop (CountDownFromFive) | Supported | [`ControlFlow_DoWhile_CountDownFromFive.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_CountDownFromFive.js) |  |
| do-while loop: break | Supported | [`ControlFlow_DoWhile_Break_AtThree.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_Break_AtThree.js) | break branches to loop end (LoopContext). |
| do-while loop: continue (skip even) | Supported | [`ControlFlow_DoWhile_Continue_SkipEven.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_Continue_SkipEven.js) | continue branches to the post-body test point (LoopContext). |
| do-while loop: labeled break | Supported | [`ControlFlow_DoWhile_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledBreak.js) | Supports break <label> where <label> targets an enclosing loop. |
| do-while loop: labeled continue | Supported | [`ControlFlow_DoWhile_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledContinue.js) | Supports continue <label> where <label> targets an enclosing loop. |

### 14.7.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-do-while-statement-static-semantics-early-errors))

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

### 14.7.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements-static-semantics-early-errors))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| for-of over arrays (enumerate values) | Partially Supported | [`ControlFlow_ForOf_Array_Basic.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Array_Basic.js)<br>[`ControlFlow_ForOf_Continue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Continue.js)<br>[`ControlFlow_ForOf_Break.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Break.js)<br>[`ControlFlow_ForOf_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledContinue.js)<br>[`ControlFlow_ForOf_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledBreak.js) | Lowered to an index loop over a normalized iterable (JavaScriptRuntime.Object.NormalizeForOfIterable), then accessed via JavaScriptRuntime.Object.GetLength(object) + GetItem(object, double). Supports arrays, strings, typed arrays, and .NET IEnumerable (via Array.from), but does not implement full JS iterator protocol (Symbol.iterator). |

### 14.7.5.2 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics-isdestructuring))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| for-in over objects (enumerate enumerable keys) | Partially Supported | [`ControlFlow_ForIn_Object_Basic.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Object_Basic.js)<br>[`ControlFlow_ForIn_Continue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Continue.js)<br>[`ControlFlow_ForIn_Break.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Break.js)<br>[`ControlFlow_ForIn_LabeledContinue.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledContinue.js)<br>[`ControlFlow_ForIn_LabeledBreak.js`](../../../Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledBreak.js) | Lowered to an index loop over JavaScriptRuntime.Object.GetEnumerableKeys(object). Minimal semantics: supports ExpandoObject (object literals), JS Array/Int32Array/string index keys, and IDictionary keys; does not currently model full prototype-chain enumeration rules. |

