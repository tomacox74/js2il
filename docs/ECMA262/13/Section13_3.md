<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.3: Left-Hand-Side Expressions

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.3 | Left-Hand-Side Expressions | Supported | [tc39.es](https://tc39.es/ecma262/#sec-left-hand-side-expressions) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.3.1 | Static Semantics | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics) |
| 13.3.1.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-left-hand-side-expressions-static-semantics-early-errors) |
| 13.3.2 | Property Accessors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-property-accessors) |
| 13.3.2.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-property-accessors-runtime-semantics-evaluation) |
| 13.3.3 | EvaluatePropertyAccessWithExpressionKey ( baseValue , expression , strict ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-evaluate-property-access-with-expression-key) |
| 13.3.4 | EvaluatePropertyAccessWithIdentifierKey ( baseValue , identifierName , strict ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-evaluate-property-access-with-identifier-key) |
| 13.3.5 | The new Operator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-new-operator) |
| 13.3.5.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-new-operator-runtime-semantics-evaluation) |
| 13.3.5.1.1 | EvaluateNew ( constructExpr , arguments ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-evaluatenew) |
| 13.3.6 | Function Calls | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function-calls) |
| 13.3.6.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function-calls-runtime-semantics-evaluation) |
| 13.3.6.2 | EvaluateCall ( func , ref , arguments , tailPosition ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-evaluatecall) |
| 13.3.7 | The super Keyword | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-super-keyword) |
| 13.3.7.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-super-keyword-runtime-semantics-evaluation) |
| 13.3.7.2 | GetSuperConstructor ( ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-getsuperconstructor) |
| 13.3.7.3 | MakeSuperPropertyReference ( actualThis , propertyKey , strict ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-makesuperpropertyreference) |
| 13.3.8 | Argument Lists | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-argument-lists) |
| 13.3.8.1 | Runtime Semantics: ArgumentListEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-argumentlistevaluation) |
| 13.3.9 | Optional Chains | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-optional-chains) |
| 13.3.9.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-optional-chaining-evaluation) |
| 13.3.9.2 | Runtime Semantics: ChainEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-optional-chaining-chain-evaluation) |
| 13.3.10 | Import Calls | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-import-calls) |
| 13.3.10.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-import-call-runtime-semantics-evaluation) |
| 13.3.10.2 | EvaluateImportCall ( specifierExpression [ , optionsExpression ] ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-evaluate-import-call) |
| 13.3.10.3 | ContinueDynamicImport ( promiseCapability , moduleCompletion ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-ContinueDynamicImport) |
| 13.3.11 | Tagged Templates | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-tagged-templates) |
| 13.3.11.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-tagged-templates-runtime-semantics-evaluation) |
| 13.3.12 | Meta Properties | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-meta-properties) |
| 13.3.12.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-meta-properties-runtime-semantics-evaluation) |
| 13.3.12.1.1 | HostGetImportMetaProperties ( moduleRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-hostgetimportmetaproperties) |
| 13.3.12.1.2 | HostFinalizeImportMeta ( importMeta , moduleRecord ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-hostfinalizeimportmeta) |

## Support

Feature-level support tracking with test script references.

### 13.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-static-semantics))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| NewExpression for built-in Array constructor (new Array()/new Array(n)/new Array(a,b,...)) | Supported | [`Array_New_Length.js`](../../../Js2IL.Tests/Array/JavaScript/Array_New_Length.js)<br>[`Array_New_MultipleArgs.js`](../../../Js2IL.Tests/Array/JavaScript/Array_New_MultipleArgs.js) | Supported in the new IR pipeline for global (non-shadowed) Array. Lowered via JavaScriptRuntime.Array::Construct(object[] args) to preserve JS Array constructor semantics. |
| NewExpression for built-in Boolean/Number constructors as primitive sugar (new Boolean(x), new Number(x)) | Supported | [`NewExpression_Boolean_Sugar.js`](../../../Js2IL.Tests/Literals/JavaScript/NewExpression_Boolean_Sugar.js)<br>[`NewExpression_Number_Sugar.js`](../../../Js2IL.Tests/Literals/JavaScript/NewExpression_Number_Sugar.js) | Supported in the new IR pipeline for global (non-shadowed) Boolean/Number. Lowered to primitive coercions (TypeUtilities.ToBoolean/ToNumber). |
| NewExpression for built-in Error types (Error/TypeError/RangeError/ReferenceError/SyntaxError/URIError/EvalError/AggregateError) | Supported | [`TryCatch_NewExpression_BuiltInErrors.js`](../../../Js2IL.Tests/TryCatch/JavaScript/TryCatch_NewExpression_BuiltInErrors.js) | Supported in the new IR pipeline for global (non-shadowed) built-in Error identifiers with 0 or 1 argument. When provided, the message is coerced to string. User-defined constructors / classes and member-expression callees (e.g., new obj.Ctor()) are not yet supported in the IR pipeline. |
| NewExpression for built-in String constructor as primitive sugar (new String()/new String(x)) | Supported | [`String_New_Sugar.js`](../../../Js2IL.Tests/String/JavaScript/String_New_Sugar.js) | Supported in the new IR pipeline for global (non-shadowed) String. Lowered to primitive string conversion (DotNet2JSConversions.ToString). |
| NewExpression for constructible runtime intrinsics (e.g., Date, Int32Array) | Supported | [`Date_Construct_FromMs_GetTime_ToISOString.js`](../../../Js2IL.Tests/Date/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js)<br>[`Int32Array_Construct_Length.js`](../../../Js2IL.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js) | Supported in the new IR pipeline for global (non-shadowed) intrinsic identifiers registered in JavaScriptRuntime.IntrinsicObjectRegistry that are constructible classes. Currently limited to 0–2 constructor arguments. |

### 13.3.6 ([tc39.es](https://tc39.es/ecma262/#sec-function-calls))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Primitive conversion callables: String(x), Number(x), Boolean(x) | Supported | [`PrimitiveConversion_String_Callable.js`](../../../Js2IL.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_String_Callable.js)<br>[`PrimitiveConversion_Number_Callable.js`](../../../Js2IL.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Number_Callable.js)<br>[`PrimitiveConversion_Boolean_Callable.js`](../../../Js2IL.Tests/PrimitiveConversion/JavaScript/PrimitiveConversion_Boolean_Callable.js) | Supports the callable form (no new) for global (non-shadowed) String/Number/Boolean. Semantics match JS: no-arg returns default ("" / +0 / false); extra arguments are evaluated for side effects but ignored. Conversions are implemented via JavaScriptRuntime.TypeUtilities (ToString/ToNumber/ToBoolean); Number(undefined) yields NaN (undefined represented as CLR null). |

### 13.3.7 ([tc39.es](https://tc39.es/ecma262/#sec-super-keyword))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| super.m(...) base method calls | Supported | [`Classes_Inheritance_SuperMethodCall.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_Inheritance_SuperMethodCall.js) |  |
| super(...) calls base constructor in derived constructors | Supported | [`Classes_Inheritance_SuperConstructor_Args.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_Inheritance_SuperConstructor_Args.js) |  |
| Using this before super throws ReferenceError in derived constructors | Supported | [`Classes_Inheritance_ThisBeforeSuper_Throws.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_Inheritance_ThisBeforeSuper_Throws.js) |  |

