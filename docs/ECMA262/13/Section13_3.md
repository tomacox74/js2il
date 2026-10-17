<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.3: Left-Hand-Side Expressions

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-17T02:29:17Z

JROC supports property access, function calls, `new`, `super`, tagged templates, and core meta-property behavior for `new.target` and `import.meta` in compiled modules. The CommonJS host also retains its import.meta extension.

Notes on scope: the statuses here describe JROC's compiler/runtime behavior, not a full mechanistic implementation of every spec abstract operation.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.3 | Left-Hand-Side Expressions | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-left-hand-side-expressions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.3.1 | Static Semantics | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics) |
| 13.3.1.1 | Static Semantics: Early Errors | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-left-hand-side-expressions-static-semantics-early-errors) |
| 13.3.2 | Property Accessors | Supported | [tc39.es](https://tc39.es/ecma262/#sec-property-accessors) |
| 13.3.2.1 | Runtime Semantics: Evaluation | Supported | [tc39.es](https://tc39.es/ecma262/#sec-property-accessors-runtime-semantics-evaluation) |
| 13.3.3 | EvaluatePropertyAccessWithExpressionKey ( baseValue , expression , strict ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-evaluate-property-access-with-expression-key) |
| 13.3.4 | EvaluatePropertyAccessWithIdentifierKey ( baseValue , identifierName , strict ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-evaluate-property-access-with-identifier-key) |
| 13.3.5 | The new Operator | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-new-operator) |
| 13.3.5.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-new-operator-runtime-semantics-evaluation) |
| 13.3.5.1.1 | EvaluateNew ( constructExpr , arguments ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-evaluatenew) |
| 13.3.6 | Function Calls | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-calls) |
| 13.3.6.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-function-calls-runtime-semantics-evaluation) |
| 13.3.6.2 | EvaluateCall ( func , ref , arguments , tailPosition ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-evaluatecall) |
| 13.3.7 | The super Keyword | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-super-keyword) |
| 13.3.7.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-super-keyword-runtime-semantics-evaluation) |
| 13.3.7.2 | GetSuperConstructor ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-getsuperconstructor) |
| 13.3.7.3 | MakeSuperPropertyReference ( actualThis , propertyKey , strict ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-makesuperpropertyreference) |
| 13.3.8 | Argument Lists | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-argument-lists) |
| 13.3.8.1 | Runtime Semantics: ArgumentListEvaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-argumentlistevaluation) |
| 13.3.9 | Optional Chains | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-optional-chains) |
| 13.3.9.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-optional-chaining-evaluation) |
| 13.3.9.2 | Runtime Semantics: ChainEvaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-optional-chaining-chain-evaluation) |
| 13.3.10 | Import Calls | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-import-calls) |
| 13.3.10.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-import-call-runtime-semantics-evaluation) |
| 13.3.10.2 | EvaluateImportCall ( specifierExpression [ , optionsExpression ] ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-evaluate-import-call) |
| 13.3.10.3 | ContinueDynamicImport ( promiseCapability , moduleCompletion ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-ContinueDynamicImport) |
| 13.3.11 | Tagged Templates | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tagged-templates) |
| 13.3.11.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-tagged-templates-runtime-semantics-evaluation) |
| 13.3.12 | Meta Properties | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-meta-properties) |
| 13.3.12.1 | Runtime Semantics: Evaluation | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-meta-properties-runtime-semantics-evaluation) |
| 13.3.12.1.1 | HostGetImportMetaProperties ( moduleRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-hostgetimportmetaproperties) |
| 13.3.12.1.2 | HostFinalizeImportMeta ( importMeta , moduleRecord ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-hostfinalizeimportmeta) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 13.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-property-accessors))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Property access and assignment: obj.prop / obj[prop] (including computed keys) | Supported | [`Variable_AssignmentTargets_MemberAndIndex.js`](../../../tests/Jroc.Tests/Variable/JavaScript/Variable_AssignmentTargets_MemberAndIndex.js)<br>`tests/Jroc.Test262.Tests/language/expressions/property-accessors/ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/property-accessors/ConstructionCompletionExecutionTests.cs` | `test/language/expressions/property-accessors/S11.2.1_A4_T3.js` | MemberExpression lowering supports identifier and computed read/write targets. Computed keys are converted through ToPropertyKey exactly once so observable coercion and abrupt completion are preserved. All 21 applicable pinned property-accessor cases are accounted for: 19 verified passing and two unsupported eval cases. Super references additionally retain the defining home object, actual receiver, and inherited base. |

### 13.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-new-operator))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| NewExpression for built-in Array constructor (new Array()/new Array(n)/new Array(a,b,...)) | Supported | [`Array_New_Length.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_New_Length.js)<br>[`Array_New_MultipleArgs.js`](../../../tests/Jroc.Tests/Array/JavaScript/Array_New_MultipleArgs.js) |  | Supported in the new IR pipeline for global (non-shadowed) Array. Lowered via JavaScriptRuntime.Array::Construct(object[] args) to preserve JS Array constructor semantics. |
| NewExpression for built-in Boolean/Number constructors as primitive sugar (new Boolean(x), new Number(x)) | Supported | [`NewExpression_Boolean_Sugar.js`](../../../tests/Jroc.Tests/Literals/JavaScript/NewExpression_Boolean_Sugar.js)<br>[`NewExpression_Number_Sugar.js`](../../../tests/Jroc.Tests/Literals/JavaScript/NewExpression_Number_Sugar.js) |  | Supported in the new IR pipeline for global (non-shadowed) Boolean/Number. Lowered to primitive coercions (TypeUtilities.ToBoolean/ToNumber). |
| NewExpression for built-in Error types (Error/TypeError/RangeError/ReferenceError/SyntaxError/URIError/EvalError/AggregateError) | Supported | [`TryCatch_NewExpression_BuiltInErrors.js`](../../../tests/Jroc.Tests/TryCatch/JavaScript/TryCatch_NewExpression_BuiltInErrors.js) |  | Built-in Error identifiers use their existing message/options construction paths. Spread arguments are expanded through the shared iterator-aware argument-list builder and runtime construction dispatch. |
| NewExpression for built-in String constructor as primitive sugar (new String()/new String(x)) | Supported | [`String_New_Sugar.js`](../../../tests/Jroc.Tests/String/JavaScript/String_New_Sugar.js) |  | Supported in the new IR pipeline for global (non-shadowed) String. Lowered to primitive string conversion (DotNet2JSConversions.ToString). |
| NewExpression for constructible runtime intrinsics (e.g., Date, Int32Array) | Supported | [`Date_Construct_FromMs_GetTime_ToISOString.js`](../../../tests/Jroc.Tests/Date/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js)<br>[`Int32Array_Construct_Length.js`](../../../tests/Jroc.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js) |  | Supported in the new IR pipeline for global (non-shadowed) intrinsic identifiers registered in JavaScriptRuntime.IntrinsicObjectRegistry that are constructible classes. Currently limited to 0–2 constructor arguments. |
| NewExpression for user-defined classes (new Ctor(...)) | Supported | [`Classes_DeclareEmptyClass.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js)<br>[`Classes_ClassConstructor_WithMultipleParameters.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassConstructor_WithMultipleParameters.js)<br>[`Function_GeneratedConstruction_Semantics.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_GeneratedConstruction_Semantics.js)<br>`tests/Jroc.Test262.Tests/language/expressions/new/RelatedLanguageConformance12BatchTests.cs` | `test/language/expressions/new/S11.2.2_A2.js`<br>`test/language/expressions/new/ctorExpr-isCtor-after-args-eval.js` | User-defined classes without spread retain direct CLR construction of the generated type, including scope-array wiring for closures in constructors/methods. Derived classes with function-valued bases preserve replacement-object returns and propagate the derived constructor as new.target. Spread construction uses runtime dispatch after evaluating the constructor and expanding arguments. |
| NewExpression where the constructor is a runtime value (new valueCtor(...)) | Supported | [`Function_GeneratedConstruction_Semantics.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_GeneratedConstruction_Semantics.js)<br>`tests/Jroc.Test262.Tests/language/expressions/new/ConstructionCompletionExecutionTests.cs` |  | Runtime-valued constructors use inline arguments for common arities or an object[] for spread/larger argument lists. The evaluated constructor is retained before argument evaluation so reassignment of its binding cannot replace the selected constructor. Generated ordinary function objects honor mutable prototype values, alternate Reflect.construct newTarget values, and constructor return overrides. All 59 applicable pinned NewExpression cases are accounted for: 57 verified passing and two unsupported eval cases. |

### 13.3.6 ([tc39.es](https://tc39.es/ecma262/#sec-function-calls))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| CallExpression on property access: receiver.method(...) (typed intrinsics and generic member dispatch) | Supported | [`Classes_ClassMethod_CallsAnotherMethod.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_ClassMethod_CallsAnotherMethod.js)<br>[`TryCatch_CallMember_MissingMethod_IsTypeError.js`](../../../tests/Jroc.Tests/TryCatch/JavaScript/TryCatch_CallMember_MissingMethod_IsTypeError.js)<br>[`Function_Call_Spread_MemberCall_ConsoleLog.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_MemberCall_ConsoleLog.js) |  | Supports intrinsic static calls (e.g., Math.abs) and dynamic member calls via runtime dispatcher when receiver type is not statically known. Spread in call arguments is supported; typed/early-bound member-call optimizations may fall back to the generic member-dispatch path when spread is present. |
| CallExpression: user-defined function calls and indirect calls via function values | Supported | [`Function_GlobalFunctionCallsGlobalFunction.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_GlobalFunctionCallsGlobalFunction.js)<br>[`Function_CallViaVariable_Reassignment.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_CallViaVariable_Reassignment.js)<br>[`Function_Call_Spread_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_Basic.js)<br>[`Function_Call_Spread_Middle.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_Middle.js)<br>[`Function_Call_Spread_Multiple.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_Multiple.js)<br>[`Function_Call_Spread_EvaluationOrder.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_EvaluationOrder.js)<br>[`Function_Call_Spread_StringIterable.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_StringIterable.js)<br>`tests/Jroc.Test262.Tests/language/expressions/call/RelatedLanguageConformance12BatchTests.cs` | `test/language/expressions/call/S11.2.3_A2.js`<br>`test/language/expressions/call/spread-mult-obj-ident.js`<br>`test/language/expressions/call/trailing-comma.js` | Covers direct function bindings and runtime dispatch for non-binding callee values (closures stored in variables). Spread in call arguments is supported by evaluating arguments left-to-right, expanding spread values using iterator semantics, and invoking via an args-array dispatch path. The native Test262 harness additionally verifies 53 current-provenance CallExpression fixtures. |
| Primitive conversion callables: String(x), Number(x), Boolean(x) | Supported | [`PrimitiveConversion_String_Callable.js`](../../../tests/Jroc.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_String_Callable.js)<br>[`PrimitiveConversion_Number_Callable.js`](../../../tests/Jroc.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Number_Callable.js)<br>[`PrimitiveConversion_Boolean_Callable.js`](../../../tests/Jroc.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Boolean_Callable.js) |  | Supports the callable form (no new) for global (non-shadowed) String/Number/Boolean. Semantics match JS: no-arg returns default ("" / +0 / false); extra arguments are evaluated for side effects but ignored. Conversions are implemented via JavaScriptRuntime.TypeUtilities (ToString/ToNumber/ToBoolean); Number(undefined) yields NaN (undefined represented as CLR null). |

### 13.3.7 ([tc39.es](https://tc39.es/ecma262/#sec-super-keyword))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Computed super method calls and computed class-method home objects | Supported with Limitations |  | `test/built-ins/String/prototype/replaceAll/searchValue-replacer-RegExp-call.js`<br>`test/built-ins/String/prototype/replaceAll/searchValue-replacer-RegExp-call-fn.js` | Computed class methods and accessors capture the defining constructor or prototype as their home object rather than inheriting the surrounding invocation's home. Calls of the form super[key](...args) coerce the property key, resolve the inherited callable before argument evaluation, expand spread arguments, and retain the original this receiver. Preserved RegExp subclass fixtures verify the complete String.replaceAll replacement scenarios. |
| super.m(...) base method calls | Supported | [`Classes_Inheritance_SuperMethodCall.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_Inheritance_SuperMethodCall.js)<br>[`Async_Inheritance_SuperAsyncMethod.js`](../../../tests/Jroc.Tests/Async/JavaScript/Async_Inheritance_SuperAsyncMethod.js)<br>[`Generator_Inheritance_SuperIteratorMethod.js`](../../../tests/Jroc.Tests/Generator/JavaScript/Generator_Inheritance_SuperIteratorMethod.js) |  | Class and object-literal super calls preserve the defining home object and original receiver, including computed keys and nested arrows. Super reads, writes, compound/logical assignments, and updates capture the base and coerce keys in specification order. Uninitialized derived this throws before key evaluation; simple assignment defers null-base rejection and key coercion until after the right-hand side. Strict failed writes throw TypeError; sloppy failed writes return the assigned value. |
| super(...) calls base constructor in derived constructors | Supported | [`Classes_Inheritance_SuperConstructor_Args.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_Inheritance_SuperConstructor_Args.js)<br>`tests/Jroc.Test262.Tests/language/expressions/super/RelatedLanguageConformance12BatchTests.cs` | `test/language/expressions/super/call-construct-invocation.js`<br>`test/language/expressions/super/call-bind-this-value.js`<br>`test/language/expressions/super/prop-dot-obj-val.js` | Derived constructors resolve the defining constructor's current prototype before evaluating arguments and preserve the outer new.target while constructing a class-valued base. CLR-only storage initialization does not initialize JavaScript this or execute superclass bodies. Null heritage, nonconstructible super targets, spread arguments, returned objects, nested construction, and cross-realm prototype fallback have preserved native Test262 coverage. |
| Using this before super throws ReferenceError in derived constructors | Supported | [`Classes_Inheritance_ThisBeforeSuper_Throws.js`](../../../tests/Jroc.Tests/Classes/JavaScript/Classes_Inheritance_ThisBeforeSuper_Throws.js) |  |  |

### 13.3.8 ([tc39.es](https://tc39.es/ecma262/#sec-argument-lists))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Argument lists (including spread) for calls and construction | Supported with Limitations | [`Function_Call_Spread_Basic.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_Basic.js)<br>[`Function_Call_Spread_Middle.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_Middle.js)<br>[`Function_Call_Spread_Multiple.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_Multiple.js)<br>[`Function_Call_Spread_EvaluationOrder.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_EvaluationOrder.js)<br>[`Function_Call_Spread_StringIterable.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_StringIterable.js)<br>[`Function_Call_Spread_MemberCall_ConsoleLog.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_Call_Spread_MemberCall_ConsoleLog.js)<br>`tests/Jroc.Test262.Tests/language/expressions/new/ConstructionCompletionExecutionTests.cs` |  | CallExpression and NewExpression argument lists preserve spread elements in HIR and expand iterables left-to-right through the existing iterator protocol. Constructor expressions are resolved before arguments, and iterator lookup/call/step/value errors propagate without invoking the constructor. |
| Call argument-list early error | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/ExpressionSyntaxConformance7BatchParseTests.cs` | `test/language/expressions/call/S11.2.4_A1.3_T1.js` | The native Test262 harness rejects the pinned malformed argument-list fixture with an empty argument between commas. |

### 13.3.9 ([tc39.es](https://tc39.es/ecma262/#sec-optional-chains))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Optional chaining (?.) for property/index access and calls | Supported with Limitations | [`BinaryOperator_OptionalChaining_PropertyAccess.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_OptionalChaining_PropertyAccess.js)<br>[`BinaryOperator_OptionalChaining_ComputedKey_ShortCircuit.js`](../../../tests/Jroc.Tests/BinaryOperator/JavaScript/BinaryOperator_OptionalChaining_ComputedKey_ShortCircuit.js) |  | Implements nullish short-circuiting for optional member access (identifier + computed) and optional calls, including skipping evaluation of computed keys / call arguments when the base/callee is nullish. Optional chaining on private fields and other less common forms may be incomplete. |

### 13.3.10 ([tc39.es](https://tc39.es/ecma262/#sec-import-calls))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Invalid import-call grammar rejection | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/dynamic-import/syntax/invalid/DynamicImportCompletionExecutionTests.cs` |  | The native Test262 harness verifies all 311 pinned parse-negative dynamic-import fixtures, including invalid calls, assignment/update targets, escaped import, and malformed attributes and phase syntax. The complete area has 940 passing cases and one explicit eval exclusion. |

### 13.3.11 ([tc39.es](https://tc39.es/ecma262/#sec-tagged-templates))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Tagged-template reference receivers, frozen arrays, and call-site identity | Supported with Limitations | `tests/Jroc.Test262.Tests/language/expressions/tagged-template/ExpressionsCompletion200ExecutionTests.cs` |  | Property, computed, optional-chain, super, and with-bound tag references preserve their receivers and are resolved before substitutions. Chained template literals retain separate call sites. Cooked and raw arrays are frozen with the required raw descriptor, and invalid tagged escapes produce undefined cooked values. Template caching remains bounded in long-lived hosts; eval-dependent identity cases remain explicitly unsupported. |

### 13.3.12 ([tc39.es](https://tc39.es/ecma262/#sec-meta-properties))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| import.meta module identity, null prototype, and host URL | Supported with Limitations | [`CommonJS_ImportMeta_Basic.js`](../../../tests/Jroc.Tests/CommonJS/JavaScript/CommonJS_ImportMeta_Basic.js)<br>`tests/Jroc.Test262.Tests/language/expressions/import.meta/ExpressionsCompletion200ExecutionTests.cs`<br>`tests/Jroc.Test262.Tests/language/expressions/import.meta/syntax/ExpressionsCompletion200ExecutionTests.cs` |  | Compiled modules expose an extensible, noncallable object with a null prototype and stable identity per module URL, including access from nested functions. Distinct modules have distinct objects. Host URLs preserve canonical module filenames; CommonJS-hosted scripts retain the existing extension. The parser enforces the Module goal and invalid assignment targets; all dynamic function-constructor families reject import.meta in parameters or bodies under the Script goal. This does not claim unrestricted runtime code generation. |
| new.target in function/constructor call paths | Supported with Limitations | [`Function_NewTarget_NewVsCall.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NewTarget_NewVsCall.js)<br>[`Function_NewTarget_Arrow_Inherits.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_NewTarget_Arrow_Inherits.js)<br>[`Function_GeneratedConstruction_Semantics.js`](../../../tests/Jroc.Tests/Function/JavaScript/Function_GeneratedConstruction_Semantics.js)<br>`tests/Jroc.Test262.Tests/language/expressions/new.target/RelatedLanguageConformance12BatchTests.cs` | `test/language/expressions/new.target/escaped-new.js`<br>`test/language/expressions/new.target/value-via-new.js` | `new.target` is propagated through function call/new invocation ABI. Normal calls and class method calls observe undefined; constructor calls observe the actual newTarget, including alternate Reflect.construct targets and nested class-valued super calls. Arrow functions capture lexical newTarget. Native Test262 cases cover unary expressions, super-call targets, super-property method calls, and escaped-keyword rejection. |

