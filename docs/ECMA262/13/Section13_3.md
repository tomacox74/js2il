<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.3: Left-Hand-Side Expressions

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

JS2IL supports the common Left-Hand-Side Expression forms used throughout the test suite (property access, function calls, `new`, `super`, and core meta-property behavior for `new.target` / `import.meta` in CommonJS-hosted scripts).

Notes on scope: the statuses here describe JS2IL's *compiler/runtime behavior*, not a full mechanistic implementation of every spec abstract operation.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.3 | Left-Hand-Side Expressions | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-left-hand-side-expressions) |

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

Feature-level support tracking with test script references.

### 13.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-property-accessors))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Property access and assignment: obj.prop / obj[prop] (including computed keys) | Supported | [`Variable_AssignmentTargets_MemberAndIndex.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_AssignmentTargets_MemberAndIndex.js) | MemberExpression lowering supports both identifier and computed forms, and supports both read and write targets. `super.prop` as a *value* (non-call) is not supported; only `super.m(...)` is supported for base-method calls. |

### 13.3.5 ([tc39.es](https://tc39.es/ecma262/#sec-new-operator))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| NewExpression for built-in Array constructor (new Array()/new Array(n)/new Array(a,b,...)) | Supported | [`Array_New_Length.js`](../../../Js2IL.Tests/Array/JavaScript/Array_New_Length.js)<br>[`Array_New_MultipleArgs.js`](../../../Js2IL.Tests/Array/JavaScript/Array_New_MultipleArgs.js) | Supported in the new IR pipeline for global (non-shadowed) Array. Lowered via JavaScriptRuntime.Array::Construct(object[] args) to preserve JS Array constructor semantics. |
| NewExpression for built-in Boolean/Number constructors as primitive sugar (new Boolean(x), new Number(x)) | Supported | [`NewExpression_Boolean_Sugar.js`](../../../Js2IL.Tests/Literals/JavaScript/NewExpression_Boolean_Sugar.js)<br>[`NewExpression_Number_Sugar.js`](../../../Js2IL.Tests/Literals/JavaScript/NewExpression_Number_Sugar.js) | Supported in the new IR pipeline for global (non-shadowed) Boolean/Number. Lowered to primitive coercions (TypeUtilities.ToBoolean/ToNumber). |
| NewExpression for built-in Error types (Error/TypeError/RangeError/ReferenceError/SyntaxError/URIError/EvalError/AggregateError) | Supported | [`TryCatch_NewExpression_BuiltInErrors.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_NewExpression_BuiltInErrors.js) | Supported in the new IR pipeline for global (non-shadowed) built-in Error identifiers with 0 or 1 argument. When provided, the message is coerced to string. Spread arguments in `new` are not supported. |
| NewExpression for built-in String constructor as primitive sugar (new String()/new String(x)) | Supported | [`String_New_Sugar.js`](../../../Js2IL.Tests/String/JavaScript/String_New_Sugar.js) | Supported in the new IR pipeline for global (non-shadowed) String. Lowered to primitive string conversion (DotNet2JSConversions.ToString). |
| NewExpression for constructible runtime intrinsics (e.g., Date, Int32Array) | Supported | [`Date_Construct_FromMs_GetTime_ToISOString.js`](../../../Js2IL.Tests/Date/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js)<br>[`Int32Array_Construct_Length.js`](../../../Js2IL.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js) | Supported in the new IR pipeline for global (non-shadowed) intrinsic identifiers registered in JavaScriptRuntime.IntrinsicObjectRegistry that are constructible classes. Currently limited to 0–2 constructor arguments. |
| NewExpression for user-defined classes (new Ctor(...)) | Supported | [`Classes_DeclareEmptyClass.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js)<br>[`Classes_ClassConstructor_WithMultipleParameters.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_WithMultipleParameters.js) | User-defined classes are lowered to direct CLR construction of the generated type, including scope-array wiring for closures in constructors/methods. Spread arguments in `new` are not supported. |
| NewExpression where the constructor is a runtime value (new valueCtor(...)) | Supported |  | When the constructor is not statically known, JS2IL lowers to a runtime dispatch (`JavaScriptRuntime.Object.ConstructValue`) by packing arguments into an object[]. Spread arguments in `new` are not supported. |

### 13.3.6 ([tc39.es](https://tc39.es/ecma262/#sec-function-calls))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| CallExpression on property access: receiver.method(...) (typed intrinsics and generic member dispatch) | Supported | [`Classes_ClassMethod_CallsAnotherMethod.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_CallsAnotherMethod.js)<br>[`TryCatch_CallMember_MissingMethod_IsTypeError.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_CallMember_MissingMethod_IsTypeError.js)<br>[`Function_Call_Spread_MemberCall_ConsoleLog.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_MemberCall_ConsoleLog.js) | Supports intrinsic static calls (e.g., Math.abs) and dynamic member calls via runtime dispatcher when receiver type is not statically known. Spread in call arguments is supported; typed/early-bound member-call optimizations may fall back to the generic member-dispatch path when spread is present. |
| CallExpression: user-defined function calls and indirect calls via function values | Supported | [`Function_GlobalFunctionCallsGlobalFunction.js`](../../../Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionCallsGlobalFunction.js)<br>[`Function_CallViaVariable_Reassignment.js`](../../../Js2IL.Tests/Function/JavaScript/Function_CallViaVariable_Reassignment.js)<br>[`Function_Call_Spread_Basic.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_Basic.js)<br>[`Function_Call_Spread_Middle.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_Middle.js)<br>[`Function_Call_Spread_Multiple.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_Multiple.js)<br>[`Function_Call_Spread_EvaluationOrder.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_EvaluationOrder.js)<br>[`Function_Call_Spread_StringIterable.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_StringIterable.js) | Covers direct function bindings and runtime dispatch for non-binding callee values (closures stored in variables). Spread in call arguments is supported by evaluating arguments left-to-right, expanding spread values using iterator semantics, and invoking via an args-array dispatch path. Spread in `new` argument lists is not supported. |
| Primitive conversion callables: String(x), Number(x), Boolean(x) | Supported | [`PrimitiveConversion_String_Callable.js`](../../../Js2IL.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_String_Callable.js)<br>[`PrimitiveConversion_Number_Callable.js`](../../../Js2IL.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Number_Callable.js)<br>[`PrimitiveConversion_Boolean_Callable.js`](../../../Js2IL.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Boolean_Callable.js) | Supports the callable form (no new) for global (non-shadowed) String/Number/Boolean. Semantics match JS: no-arg returns default ("" / +0 / false); extra arguments are evaluated for side effects but ignored. Conversions are implemented via JavaScriptRuntime.TypeUtilities (ToString/ToNumber/ToBoolean); Number(undefined) yields NaN (undefined represented as CLR null). |

### 13.3.7 ([tc39.es](https://tc39.es/ecma262/#sec-super-keyword))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| super.m(...) base method calls | Supported | [`Classes_Inheritance_SuperMethodCall.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_Inheritance_SuperMethodCall.js)<br>[`Async_Inheritance_SuperAsyncMethod.js`](../../../Js2IL.Tests/Async/JavaScript/Async_Inheritance_SuperAsyncMethod.js)<br>[`Generator_Inheritance_SuperIteratorMethod.js`](../../../Js2IL.Tests/Generator/JavaScript/Generator_Inheritance_SuperIteratorMethod.js) | Supported only for the base-method call form. Property reads/writes like `super.x` / `super[x]` are not yet supported. |
| super(...) calls base constructor in derived constructors | Supported | [`Classes_Inheritance_SuperConstructor_Args.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_Inheritance_SuperConstructor_Args.js) | Supported for derived class constructors. |
| Using this before super throws ReferenceError in derived constructors | Supported | [`Classes_Inheritance_ThisBeforeSuper_Throws.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_Inheritance_ThisBeforeSuper_Throws.js) |  |

### 13.3.8 ([tc39.es](https://tc39.es/ecma262/#sec-argument-lists))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Argument lists (including spread) for CallExpression | Supported with Limitations | [`Function_Call_Spread_Basic.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_Basic.js)<br>[`Function_Call_Spread_Middle.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_Middle.js)<br>[`Function_Call_Spread_Multiple.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_Multiple.js)<br>[`Function_Call_Spread_EvaluationOrder.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_EvaluationOrder.js)<br>[`Function_Call_Spread_StringIterable.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_StringIterable.js)<br>[`Function_Call_Spread_MemberCall_ConsoleLog.js`](../../../Js2IL.Tests/Function/JavaScript/Function_Call_Spread_MemberCall_ConsoleLog.js) | Argument list evaluation supports spread elements in CallExpression argument lists by expanding iterables via the iterator protocol. Spread in `new` argument lists is not supported. |

### 13.3.9 ([tc39.es](https://tc39.es/ecma262/#sec-optional-chains))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Optional chaining (?.) for property/index access and calls | Supported with Limitations | [`BinaryOperator_OptionalChaining_PropertyAccess.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_OptionalChaining_PropertyAccess.js)<br>[`BinaryOperator_OptionalChaining_ComputedKey_ShortCircuit.js`](../../../Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_OptionalChaining_ComputedKey_ShortCircuit.js) | Implements nullish short-circuiting for optional member access (identifier + computed) and optional calls, including skipping evaluation of computed keys / call arguments when the base/callee is nullish. Optional chaining on private fields and other less common forms may be incomplete. |

### 13.3.12 ([tc39.es](https://tc39.es/ecma262/#sec-meta-properties))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| import.meta host object (CommonJS limitation) | Supported with Limitations | [`CommonJS_ImportMeta_Basic.js`](../../../Js2IL.Tests/CommonJS/JavaScript/CommonJS_ImportMeta_Basic.js) | CommonJS-hosted scripts expose a host-defined import.meta object with stable identity per module URL/path key and a `url` property when available. This is not full ESM module-record semantics. |
| new.target in function/constructor call paths | Supported with Limitations | [`Function_NewTarget_NewVsCall.js`](../../../Js2IL.Tests/Function/JavaScript/Function_NewTarget_NewVsCall.js)<br>[`Function_NewTarget_Arrow_Inherits.js`](../../../Js2IL.Tests/Function/JavaScript/Function_NewTarget_Arrow_Inherits.js) | `new.target` is propagated through function call/new invocation ABI. Normal calls observe undefined; constructor calls observe a defined newTarget value. Arrow functions capture lexical newTarget. |

