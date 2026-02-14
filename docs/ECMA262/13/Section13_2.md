<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 13.2: Primary Expression

[Back to Section13](Section13.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 13.2 | Primary Expression | Supported | [tc39.es](https://tc39.es/ecma262/#sec-primary-expression) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 13.2.1 | The this Keyword | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-this-keyword) |
| 13.2.1.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-this-keyword-runtime-semantics-evaluation) |
| 13.2.2 | Identifier Reference | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-identifier-reference) |
| 13.2.3 | Literals | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-primary-expression-literals) |
| 13.2.3.1 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-literals-runtime-semantics-evaluation) |
| 13.2.4 | Array Initializer | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array-initializer) |
| 13.2.4.1 | Runtime Semantics: ArrayAccumulation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-arrayaccumulation) |
| 13.2.4.2 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-array-initializer-runtime-semantics-evaluation) |
| 13.2.5 | Object Initializer | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-object-initializer) |
| 13.2.5.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-object-initializer-static-semantics-early-errors) |
| 13.2.5.2 | Static Semantics: IsComputedPropertyKey | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-iscomputedpropertykey) |
| 13.2.5.3 | Static Semantics: PropertyNameList | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-propertynamelist) |
| 13.2.5.4 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-object-initializer-runtime-semantics-evaluation) |
| 13.2.5.5 | Runtime Semantics: PropertyDefinitionEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-propertydefinitionevaluation) |
| 13.2.6 | Function Defining Expressions | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-function-defining-expressions) |
| 13.2.7 | Regular Expression Literals | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-primary-expression-regular-expression-literals) |
| 13.2.7.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-primary-expression-regular-expression-literals-static-semantics-early-errors) |
| 13.2.7.2 | Static Semantics: IsValidRegularExpressionLiteral ( literal ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-isvalidregularexpressionliteral) |
| 13.2.7.3 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-regular-expression-literals-runtime-semantics-evaluation) |
| 13.2.8 | Template Literals | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-template-literals) |
| 13.2.8.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-template-early-errors) |
| 13.2.8.2 | Static Semantics: TemplateStrings | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-static-semantics-templatestrings) |
| 13.2.8.3 | Static Semantics: TemplateString ( templateToken , raw ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-templatestring) |
| 13.2.8.4 | GetTemplateObject ( templateLiteral ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-gettemplateobject) |
| 13.2.8.5 | Runtime Semantics: SubstitutionEvaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-substitutionevaluation) |
| 13.2.8.6 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-template-literals-runtime-semantics-evaluation) |
| 13.2.9 | The Grouping Operator | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-grouping-operator) |
| 13.2.9.1 | Static Semantics: Early Errors | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-grouping-operator-static-semantics-early-errors) |
| 13.2.9.2 | Runtime Semantics: Evaluation | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-grouping-operator-runtime-semantics-evaluation) |

## Support

Feature-level support tracking with test script references.

### 13.2.1 ([tc39.es](https://tc39.es/ecma262/#sec-this-keyword))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Const reassignment throws TypeError | Supported | [`Variable_ConstReassignmentError.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ConstReassignmentError.js) | Assignment to a const Identifier and ++/-- on const emit a runtime TypeError; error is catchable via try/catch. |
| let/const | Supported with Limitations | [`Variable_LetBlockScope.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetBlockScope.js)<br>[`Variable_LetShadowing.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetShadowing.js)<br>[`Variable_LetNestedShadowingChain.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetNestedShadowingChain.js)<br>[`Variable_LetFunctionNestedShadowing.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_LetFunctionNestedShadowing.js)<br>[`Variable_ConstSimple.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ConstSimple.js) | Block scoping, shadowing chain, nested function capture, and simple const initialization implemented. Temporal dead zone access error (Variable_TemporalDeadZoneAccess.js) and reads before initialization are still pending. |

### 13.2.1.1 ([tc39.es](https://tc39.es/ecma262/#sec-this-keyword-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| var | Supported | [`Function_GlobalFunctionChangesGlobalVariableValue.js`](../../../Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionChangesGlobalVariableValue.js)<br>[`Function_GlobalFunctionDeclaresAndCallsNestedFunction.js`](../../../Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionDeclaresAndCallsNestedFunction.js)<br>[`Function_GlobalFunctionLogsGlobalVariable.js`](../../../Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionLogsGlobalVariable.js) |  |

### 13.2.2 ([tc39.es](https://tc39.es/ecma262/#sec-identifier-reference))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Destructuring in variable declarations (object/array patterns, defaults, rest, nested) | Supported | [`PerfHooks_PerformanceNow_Basic.js`](../../../Js2IL.Tests/Node/JavaScript/PerfHooks_PerformanceNow_Basic.js)<br>[`Variable_ObjectDestructuring_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Basic.js)<br>[`Variable_ObjectDestructuring_WithDefaults.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_WithDefaults.js)<br>[`Variable_ObjectDestructuring_Captured.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Captured.js)<br>[`Variable_ObjectDestructuring_Rest.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Rest.js)<br>[`Variable_ArrayDestructuring_Basic.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ArrayDestructuring_Basic.js)<br>[`Variable_ArrayDestructuring_DefaultsAndRest.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_ArrayDestructuring_DefaultsAndRest.js)<br>[`Variable_NestedDestructuring_Defaults.js`](../../../Js2IL.Tests/Variable/JavaScript/Variable_NestedDestructuring_Defaults.js) | Supports object and array binding patterns in variable declarators (const/let/var), including nested patterns, default values, and rest elements/properties. The initializer is evaluated once, then bindings are populated via JavaScriptRuntime.Object.GetItem (string keys for objects, numeric indices for arrays). Object rest ({ a, ...rest }) uses JavaScriptRuntime.Object.Rest to create a new object excluding the bound keys; array rest ([a, ...rest]) builds a new JavaScriptRuntime.Array from the remaining indices. Handles both uncaptured variables (stored in IL locals) and captured variables (stored in scope fields), including cases where destructured bindings are captured by nested functions. Limitations: computed keys in patterns are not yet supported. |
| Object destructuring in class constructor/method parameters | Supported | [`Classes_ClassConstructor_ParameterDestructuring.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_ParameterDestructuring.js)<br>[`Classes_ClassMethod_ParameterDestructuring.js`](../../../Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_ParameterDestructuring.js) | Class constructors and instance methods support object destructuring parameters with full default value support (e.g., constructor({host="localhost", port=8080})). Uses the same conditional IL generation as functions/arrows. Shared implementation in MethodBuilder.EmitObjectPatternParameterDestructuring handles all function types uniformly. Supports shorthand, aliasing, and default values. Limitations: no nested patterns, no rest properties, no array patterns. |
| Object destructuring in function/arrow parameters — basic | Supported | [`Function_ParameterDestructuring_Object.js`](../../../Js2IL.Tests/Function/JavaScript/Function_ParameterDestructuring_Object.js)<br>[`ArrowFunction_ParameterDestructuring_Object.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_ParameterDestructuring_Object.js) | Supports object binding patterns in formal parameters for function declarations and arrow functions, including shorthand (e.g., {a,b}), aliasing (e.g., { a: x }), and default values (e.g., {host="localhost", port=8080}). Default values use conditional IL generation with brfalse branching: property is retrieved, if null the default expression is evaluated, otherwise the retrieved value is used. Limitations: no nested patterns, no rest properties, and no array patterns in parameters. Properties are retrieved via JavaScriptRuntime.Object.GetProperty and bound into the function scope before body execution. |

### 13.2.3 ([tc39.es](https://tc39.es/ecma262/#sec-primary-expression-literals))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Arrow functions | Supported with Limitations | [`ArrowFunction_SimpleExpression.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_SimpleExpression.js)<br>[`ArrowFunction_BlockBody_Return.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_BlockBody_Return.js)<br>[`ArrowFunction_GlobalFunctionCallsGlobalFunction.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionCallsGlobalFunction.js)<br>[`ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal.js)<br>[`ArrowFunction_GlobalFunctionWithMultipleParameters.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionWithMultipleParameters.js)<br>[`ArrowFunction_NestedFunctionAccessesMultipleScopes.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_NestedFunctionAccessesMultipleScopes.js)<br>[`ArrowFunction_CapturesOuterVariable.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_CapturesOuterVariable.js)<br>[`ArrowFunction_ParameterDestructuring_Object.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_ParameterDestructuring_Object.js)<br>[`ArrowFunction_DefaultParameterValue.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_DefaultParameterValue.js) | Covers expression- and block-bodied arrows, multiple parameters, nested functions, closure capture across scopes (including returning functions that capture globals/locals), default parameter values, lexical this, and lexical arguments capture. Not yet supported: rest parameters and spread at call sites. |
| Function declarations | Supported | [`Function_HelloWorld.js`](../../../Js2IL.Tests/Function/JavaScript/Function_HelloWorld.js) | Hoisted and initialized before top-level statement evaluation so functions can call each other by name prior to IIFE invocation. |

### 13.2.3 (Function expressions) ([tc39.es](https://tc39.es/ecma262/#sec-function-definitions))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Function expressions (anonymous) | Supported | [`Function_IIFE_Classic.js`](../../../Js2IL.Tests/Function/JavaScript/Function_IIFE_Classic.js) | Emitted as static methods with delegate creation and closure binding as needed; supports immediate invocation patterns (IIFE). |
| Named function expressions (internal self-binding for recursion) | Supported | [`Function_IIFE_Recursive.js`](../../../Js2IL.Tests/Function/JavaScript/Function_IIFE_Recursive.js) | On first entry, the internal name is eagerly bound to the function delegate instance (constructed via ldftn + newobj) enabling recursion from within the function body. |

### 13.2.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-literals-runtime-semantics-evaluation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Default parameters | Supported | [`Function_DefaultParameterValue.js`](../../../Js2IL.Tests/Function/JavaScript/Function_DefaultParameterValue.js)<br>[`ArrowFunction_DefaultParameterValue.js`](../../../Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_DefaultParameterValue.js) | Default parameter values are supported for function declarations, function expressions, and arrow functions. Supports literal defaults (numbers, strings, booleans) and expression defaults that reference previous parameters (e.g., function f(a, b = a * 2)). Implemented using starg IL pattern when arguments are null. Call sites validate argument count ranges and pad missing optional parameters with ldnull. |
| Rest parameters | Not Yet Supported |  |  |

### 13.2.4.1 ([tc39.es](https://tc39.es/ecma262/#sec-runtime-semantics-arrayaccumulation))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Array literal basic construction | Supported | [`ArrayLiteral.js`](../../../Js2IL.Tests/Literals/JavaScript/ArrayLiteral.js) | Covers creation, element access, and length property. See also verified output in Literals/GeneratorTests.ArrayLiteral.verified.txt. |
| Array literal spread (copy elements) | Supported | [`Array_Spread_Copy.js`](../../../Js2IL.Tests/Literals/JavaScript/Array_Spread_Copy.js)<br>[`Array_LiteralSpread_Basic.js`](../../../Js2IL.Tests/Array/JavaScript/Array_LiteralSpread_Basic.js)<br>[`Array_LiteralSpread_Multiple.js`](../../../Js2IL.Tests/Array/JavaScript/Array_LiteralSpread_Multiple.js)<br>[`Array_LiteralSpread_Mixed.js`](../../../Js2IL.Tests/Array/JavaScript/Array_LiteralSpread_Mixed.js)<br>[`Array_LiteralSpread_Empty.js`](../../../Js2IL.Tests/Array/JavaScript/Array_LiteralSpread_Empty.js)<br>[`Array_LiteralSpread_Nested.js`](../../../Js2IL.Tests/Array/JavaScript/Array_LiteralSpread_Nested.js) | Spread elements in array literals are lowered by seeding any leading non-spread elements, then appending subsequent elements via Add or JavaScriptRuntime.Array.PushRange. PushRange consumes the iterator protocol (arrays, strings, typed arrays, user-defined iterables via Symbol.iterator; .NET IEnumerable fallback), enabling multiple spreads and mixed literal/spread patterns. |

### 13.2.5 ([tc39.es](https://tc39.es/ecma262/#sec-object-initializer))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Spread syntax | Not Yet Supported |  |  |

### 13.2.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-object-initializer-static-semantics-early-errors))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Object literal basic construction | Supported | [`ObjectLiteral.js`](../../../Js2IL.Tests/Literals/JavaScript/ObjectLiteral.js) | Covers creation and property access. See also verified output in Literals/GeneratorTests.ObjectLiteral.verified.txt. |
| Object literal computed property keys ({ [expr]: value }) | Supported | [`ObjectLiteral_ComputedKey_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_Basic.js)<br>[`ObjectLiteral_ComputedKey_EvaluationOrder.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ComputedKey_EvaluationOrder.js) | Lowered as sequential JavaScriptRuntime.Object.SetItem(target, key, value) calls to preserve left-to-right evaluation order and key coercion to string. |
| Object literal method definitions ({ m() { ... } }) | Supported | [`ObjectLiteral_ShorthandAndMethod.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ShorthandAndMethod.js) | Method definitions are lowered as property assignments whose values are compiled function delegates; calls via member dispatch bind 'this' using RuntimeServices.SetCurrentThis. |
| Object literal shorthand properties ({ a }) | Supported | [`ObjectLiteral_ShorthandAndMethod.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_ShorthandAndMethod.js) | Shorthand properties are parsed and lowered the same as explicit properties ({ a: a }). |
| Object literal spread properties ({ ...x }) | Supported | [`ObjectLiteral_Spread_Basic.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_Spread_Basic.js)<br>[`ObjectLiteral_Spread_Multiple.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_Spread_Multiple.js)<br>[`ObjectLiteral_Spread_Clone.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_Spread_Clone.js)<br>[`ObjectLiteral_Spread_Empty.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_Spread_Empty.js)<br>[`ObjectLiteral_Spread_SymbolProperties.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_Spread_SymbolProperties.js)<br>[`ObjectLiteral_Spread_NestedObjects.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_Spread_NestedObjects.js)<br>[`ObjectLiteral_Spread_SkipsNonEnumerable.js`](../../../Js2IL.Tests/Object/JavaScript/ObjectLiteral_Spread_SkipsNonEnumerable.js) | Lowered as: create empty ExpandoObject, then apply members in order. Spread members call JavaScriptRuntime.Object.SpreadInto(target, source). Null/undefined sources are ignored. Copies enumerable own properties (including symbol-keyed properties, modeled via a stable internal key string). |

