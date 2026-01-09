# ECMAScript 2025 Feature Coverage

[ECMAScript® 2025 Language Specification](https://tc39.es/ecma262/)

This file is auto-generated from ECMAScript2025_FeatureCoverage.json.

## [ECMAScript Language: Expressions](https://tc39.es/ecma262/#sec-ecmascript-language-expressions)

### [Primary Expressions](https://tc39.es/ecma262/#sec-primary-expression)

#### [NumericLiteral](https://tc39.es/ecma262/#sec-numeric-literals)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Numeric literals (integer and decimal) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_AddNumberNumber.js`<br>`Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_MulNumberNumber.js` | Numbers are represented as double and used pervasively across arithmetic, comparison, and control-flow tests. | 13.1.1 |


#### [StringLiteral](https://tc39.es/ecma262/#sec-string-literals)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| String literals (single/double quotes; escapes) | Supported | `Js2IL.Tests/String/JavaScript/String_StartsWith_Basic.js`<br>`Js2IL.Tests/String/JavaScript/String_Replace_Regex_Global.js` | Backed by .NET System.String; values are boxed/unboxed where needed in member calls and concatenation. | 13.1.2 |


#### [BooleanLiteral](https://tc39.es/ecma262/#sec-boolean-literals)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Boolean literals (true/false) | Supported | `Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_Typeof.js`<br>`Js2IL.Tests/JSON/JavaScript/JSON_Parse_SimpleObject.js` | Emits proper IL for true/false and boxes when needed in arrays/log calls. See generator snapshot: Js2IL.Tests/Literals/GeneratorTests.BooleanLiteral.verified.txt. | 13.1.3 |


#### [Template Literals](https://tc39.es/ecma262/#sec-template-literals)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Template literals (basic interpolation) | Supported | `Js2IL.Tests/String/JavaScript/String_TemplateLiteral_Basic.js` | Concatenates quasis and expressions via runtime Operators.Add with JS string/number coercion. Tagged templates are not yet supported. | 13.1.4 |


#### [NullLiteral](https://tc39.es/ecma262/#sec-null-literals)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| null literal | Supported | `Js2IL.Tests/Literals/JavaScript/Literals_NullAndUndefined.js`<br>`Js2IL.Tests/JSON/JavaScript/JSON_Parse_SimpleObject.js` | null emission validated in literals and variable tests; see execution snapshot Js2IL.Tests/Literals/ExecutionTests.Literals_NullAndUndefined.verified.txt. | 13.1.5 |


#### [undefined (Identifier)](https://tc39.es/ecma262/#sec-undefined)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| undefined identifier usage | Supported | `Js2IL.Tests/Literals/JavaScript/Literals_NullAndUndefined.js` | Handled as the ECMAScript undefined value and participates in JS truthiness; see execution snapshot Js2IL.Tests/Literals/ExecutionTests.Literals_NullAndUndefined.verified.txt. | 13.1.6 |


### [Declarations](https://tc39.es/ecma262/#sec-declarations)

#### [let/const](https://tc39.es/ecma262/#sec-let-and-const-declarations)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| let/const | Partially Supported | `Js2IL.Tests/Variable/JavaScript/Variable_LetBlockScope.js`<br>`Js2IL.Tests/Variable/JavaScript/Variable_LetShadowing.js`<br>`Js2IL.Tests/Variable/JavaScript/Variable_LetNestedShadowingChain.js`<br>`Js2IL.Tests/Variable/JavaScript/Variable_LetFunctionNestedShadowing.js`<br>`Js2IL.Tests/Variable/JavaScript/Variable_ConstSimple.js` | Block scoping, shadowing chain, nested function capture, and simple const initialization implemented. Temporal dead zone access error (Variable_TemporalDeadZoneAccess.js) and reads before initialization are still pending. | 13.2.1 |
| Const reassignment throws TypeError | Supported | `Js2IL.Tests/Variable/JavaScript/Variable_ConstReassignmentError.js` | Assignment to a const Identifier and ++/-- on const emit a runtime TypeError; error is catchable via try/catch. | 13.2.1 |


#### [var](https://tc39.es/ecma262/#sec-variable-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| var | Supported | `Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionChangesGlobalVariableValue.js`<br>`Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionDeclaresAndCallsNestedFunction.js`<br>`Js2IL.Tests/Function/JavaScript/Function_GlobalFunctionLogsGlobalVariable.js` |  | 13.2.1 |


#### [Binding patterns (destructuring)](https://tc39.es/ecma262/#sec-destructuring-binding-patterns)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Object destructuring in variable declarations (const/let) — basic | Supported | `Js2IL.Tests/Node/JavaScript/PerfHooks_PerformanceNow_Basic.js`<br>`Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Basic.js`<br>`Js2IL.Tests/Variable/JavaScript/Variable_ObjectDestructuring_Captured.js` | Supports object patterns in variable declarations (const/let), e.g., const { x, y } = obj; The initializer is evaluated once and properties are bound via JavaScriptRuntime.Object.GetProperty. Handles both uncaptured variables (stored in IL locals) and captured variables (stored in scope fields). Works correctly when destructured variables are captured by nested functions. Not yet supported: default values, renaming (alias), rest properties, nested patterns, array destructuring, and assignment destructuring. | 13.2.2 |
| Object destructuring in function/arrow parameters — basic | Supported | `Js2IL.Tests/Function/JavaScript/Function_ParameterDestructuring_Object.js`<br>`Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_ParameterDestructuring_Object.js` | Supports object binding patterns in formal parameters for function declarations and arrow functions, including shorthand (e.g., {a,b}), aliasing (e.g., { a: x }), and default values (e.g., {host="localhost", port=8080}). Default values use conditional IL generation with brfalse branching: property is retrieved, if null the default expression is evaluated, otherwise the retrieved value is used. Limitations: no nested patterns, no rest properties, and no array patterns in parameters. Properties are retrieved via JavaScriptRuntime.Object.GetProperty and bound into the function scope before body execution. | 13.2.2 |
| Object destructuring in class constructor/method parameters | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_ParameterDestructuring.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_ParameterDestructuring.js` | Class constructors and instance methods support object destructuring parameters with full default value support (e.g., constructor({host="localhost", port=8080})). Uses the same conditional IL generation as functions/arrows. Shared implementation in MethodBuilder.EmitObjectPatternParameterDestructuring handles all function types uniformly. Supports shorthand, aliasing, and default values. Limitations: no nested patterns, no rest properties, no array patterns. | 13.2.2 |


#### [Function declarations](https://tc39.es/ecma262/#sec-function-definitions)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Function declarations | Supported | `Js2IL.Tests/Function/JavaScript/Function_HelloWorld.js` | Hoisted and initialized before top-level statement evaluation so functions can call each other by name prior to IIFE invocation. | 13.2.3 |
| Arrow functions | Partially Supported | `Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_SimpleExpression.js`<br>`Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_BlockBody_Return.js`<br>`Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionCallsGlobalFunction.js`<br>`Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionReturnsNestedFunction_LogsParamAndGlobal.js`<br>`Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_GlobalFunctionWithMultipleParameters.js`<br>`Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_NestedFunctionAccessesMultipleScopes.js`<br>`Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_CapturesOuterVariable.js`<br>`Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_ParameterDestructuring_Object.js`<br>`Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_DefaultParameterValue.js` | Covers expression- and block-bodied arrows, multiple parameters, nested functions, closure capture across scopes (including returning functions that capture globals/locals), and default parameter values. Not yet supported: rest parameters, lexical this/arguments semantics, and spread at call sites. | 13.2.3 |


#### [Function expressions (anonymous and named)](https://tc39.es/ecma262/#sec-function-definitions)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Function expressions (anonymous) | Supported | `Js2IL.Tests/Function/JavaScript/Function_IIFE_Classic.js` | Emitted as static methods with delegate creation and closure binding as needed; supports immediate invocation patterns (IIFE). | 13.2.3 (Function expressions) |
| Named function expressions (internal self-binding for recursion) | Supported | `Js2IL.Tests/Function/JavaScript/Function_IIFE_Recursive.js` | On first entry, the internal name is eagerly bound to a self-delegate via JavaScriptRuntime.Closure.CreateSelfDelegate(MethodBase, int), enabling recursion from within the function body. | 13.2.3 (Function expressions) |


#### [Default parameters, Rest parameters](https://tc39.es/ecma262/#sec-function-definitions-runtime-semantics-evaluation)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Default parameters | Supported | `Js2IL.Tests/Function/JavaScript/Function_DefaultParameterValue.js`<br>`Js2IL.Tests/ArrowFunction/JavaScript/ArrowFunction_DefaultParameterValue.js` | Default parameter values are supported for function declarations, function expressions, and arrow functions. Supports literal defaults (numbers, strings, booleans) and expression defaults that reference previous parameters (e.g., function f(a, b = a * 2)). Implemented using starg IL pattern when arguments are null. Call sites validate argument count ranges and pad missing optional parameters with ldnull. | 13.2.3.1 |
| Rest parameters | Not Supported |  |  | 13.2.3.1 |


#### [Spread syntax](https://tc39.es/ecma262/#sec-argument-lists-runtime-semantics-argumentlistevaluation)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Spread syntax | Not Supported |  |  | 13.2.5 |


### [Array Initializer (ArrayLiteral)](https://tc39.es/ecma262/#sec-array-initializer)

#### [ArrayLiteral](https://tc39.es/ecma262/#sec-array-initializer)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array literal basic construction | Supported | `Js2IL.Tests/Literals/JavaScript/ArrayLiteral.js` | Covers creation, element access, and length property. See also verified output in Literals/GeneratorTests.ArrayLiteral.verified.txt. | 13.2.4.1 |
| Array literal spread (copy elements) | Supported | `Js2IL.Tests/Literals/JavaScript/Array_Spread_Copy.js` | Spread elements in array literals are emitted via JavaScriptRuntime.Array.PushRange; supports copying from another array. | 13.2.4.1 |


### [Object Initializer (ObjectLiteral)](https://tc39.es/ecma262/#sec-object-initializer)

#### [ObjectLiteral](https://tc39.es/ecma262/#sec-object-initializer)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Object literal basic construction | Supported | `Js2IL.Tests/Literals/JavaScript/ObjectLiteral.js` | Covers creation and property access. See also verified output in Literals/GeneratorTests.ObjectLiteral.verified.txt. | 13.2.5.1 |


### [Unary Operators](https://tc39.es/ecma262/#sec-ecmascript-language-expressions)

#### [typeof operator](https://tc39.es/ecma262/#sec-typeof-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| typeof | Supported | `Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_Typeof.js` | Implemented via JavaScriptRuntime.TypeUtilities::Typeof and IL emission for UnaryExpression(typeof). typeof null returns 'object'; functions report 'function'; objects report 'object'. | 13.4.3 |


#### [Prefix increment operator (++)](https://tc39.es/ecma262/#sec-prefix-increment-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Unary ++ (Prefix increment) | Supported | `Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_PlusPlusPrefix.js` | Increments the value first, then returns the new value. | 13.4.4 |


#### [Prefix decrement operator (--)](https://tc39.es/ecma262/#sec-prefix-decrement-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Unary -- (Prefix decrement) | Supported | `Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_MinusMinusPrefix.js` | Decrements the value first, then returns the new value. | 13.4.5 |


#### [Bitwise NOT operator (~)](https://tc39.es/ecma262/#sec-bitwise-not-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Unary ~ (Bitwise NOT) | Supported | `Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_BitwiseNot.js` | Converts operand to int32, applies bitwise NOT, converts back to double. Used in bit manipulation patterns. | 13.4.6 |


#### [Logical not operator (!)](https://tc39.es/ecma262/#sec-logical-not-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Unary ! (Logical not) | Supported | `Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_LogicalNot.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_NotFlag.js` | Supported end-to-end in IR pipeline (HIR unary + LIRLogicalNot) using JavaScriptRuntime.TypeUtilities.ToBoolean for JS truthiness, then invert. Covered both in a dedicated unary-operator fixture and in control-flow conditionals (if (!x) ...). | 13.4.7 |


#### [Postfix increment operator (++)](https://tc39.es/ecma262/#sec-postfix-increment-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Unary ++ (Postfix increment) | Supported | `Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_PlusPlusPostfix.js` |  | 13.4.9 |


#### [Postfix decrement operator (--)](https://tc39.es/ecma262/#sec-postfix-decrement-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Unary -- (Postfix decrement) | Supported | `Js2IL.Tests/UnaryOperator/JavaScript/UnaryOperator_MinusMinusPostfix.js` |  | 13.4.10 |


### [Binary Operators](https://tc39.es/ecma262/#sec-ecmascript-language-expressions)

#### [Addition operator (+)](https://tc39.es/ecma262/#sec-additive-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary + (Addition) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_AddNumberNumber.js`<br>`Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_AddStringNumber.js`<br>`Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_AddStringString.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_TwoParams_AddMethod.js` | Fast-path string concat; general '+' follows JS coercion via runtime helper. | 13.5.1 |


#### [Subtraction operator (-)](https://tc39.es/ecma262/#sec-subtraction-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary - (Subtraction) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_SubNumberNumber.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_TwoParams_SubtractMethod.js` | Numeric subtraction; matches JS semantics for non-numeric via coercion helpers where applicable. | 13.5.2 |


#### [Multiplicative operators (*, /, %)](https://tc39.es/ecma262/#sec-multiplicative-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary * (Multiplication) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_MulNumberNumber.js` |  | 13.5.3 |
| Binary / (Division) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_DivNumberNumber.js` |  | 13.5.3 |
| Binary % (Remainder) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_ModNumberNumber.js` |  | 13.5.3 |


#### [Exponentiation operator (**) ](https://tc39.es/ecma262/#sec-exp-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary ** (Exponentiation) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_ExpNumberNumber.js` |  | 13.5.4 |


#### [Bitwise operators (&, |, ^)](https://tc39.es/ecma262/#sec-binary-bitwise-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary & (Bitwise AND) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_BitwiseAndNumberNumber.js` |  | 13.5.6 |
| Binary \| (Bitwise OR) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_BitwiseOrNumberNumber.js` |  | 13.5.6 |
| Binary ^ (Bitwise XOR) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_BitwiseXorNumberNumber.js` |  | 13.5.6 |


#### [Shift operators (<<, >>, >>>)](https://tc39.es/ecma262/#sec-left-shift-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary << (Left shift) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LeftShiftNumberNumber.js` |  | 13.5.7 |
| Binary >> (Signed right shift) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_RightShiftNumberNumber.js` |  | 13.5.7 |
| Binary >>> (Unsigned right shift) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_UnsignedRightShiftNumberNumber.js` |  | 13.5.7 |


#### [Relational operators (<, <=, >, >=)](https://tc39.es/ecma262/#sec-relational-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary < (Less than) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LessThan.js` |  | 13.5.8 |
| Binary <= (Less than or equal) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LessThanOrEqual.js` |  | 13.5.8 |
| Binary > (Greater than) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_GreaterThan.js` |  | 13.5.8 |
| Binary >= (Greater than or equal) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_GreaterThanOrEqual.js` |  | 13.5.8 |


#### [Equality operators (==)](https://tc39.es/ecma262/#sec-equality-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary == (Equality) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_Equal.js`<br>`Js2IL.Tests/Function/JavaScript/Function_IsEven_CompareResultToTrue.js` | Covers numeric and boolean equality, including comparisons against literals and function-returned booleans with selective boxing/unboxing. See also generator snapshot: Js2IL.Tests/BinaryOperator/GeneratorTests.BinaryOperator_EqualBoolean.verified.txt. | 13.5.9 |
| Binary != (Inequality) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_NotEqual.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_NotEqual.js` | Value result emitted via Ceq inversion; conditional form branches with bne.un. Unboxing/coercion rules mirror equality operator handling for numbers/booleans. | 13.5.9 |
| Binary !== (Strict inequality) | Supported |  | Implemented alongside != in the IL generator (value + branching). Dedicated tests to be added; semantics match JavaScript strict inequality (no type coercion). | 13.5.9 |


#### [Logical operators (||, &&)](https://tc39.es/ecma262/#sec-binary-logical-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Binary \|\| (Logical OR) with short-circuit | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LogicalOr_Value.js`<br>`Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LogicalOr_ShortCircuit.js` | Value form returns left if truthy, otherwise right; branching form uses JS ToBoolean for conditions. Right-hand side is not evaluated when short-circuited. Recent fixes ensure strict-equality patterns (e.g. `id === 1024 \|\| id === 2047`) correctly handle captured/boxed variables by performing ToNumber conversion when the variable type is unknown, preventing incorrect direct object-to-number `ceq` comparisons. | 13.5.10 |
| Binary && (Logical AND) with short-circuit | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LogicalAnd_Value.js`<br>`Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_LogicalAnd_ShortCircuit.js` | Value form returns left if falsy, otherwise right; branching form uses JS ToBoolean for conditions. Right-hand side is not evaluated when short-circuited. | 13.5.10 |
| Binary in (property existence on object) | Supported | `Js2IL.Tests/BinaryOperator/JavaScript/BinaryOperator_In_Object_OwnAndMissing.js` | Implements key in obj via JavaScriptRuntime.Object.HasPropertyIn. Supports own properties on ExpandoObject/object literals and numeric/string keys; does not yet traverse prototype chain or throw TypeError for non-object RHS beyond null/undefined. | 13.5.10 |


#### [Conditional (ternary) operator (?:)](https://tc39.es/ecma262/#sec-conditional-operator)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Conditional operator (?:) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Conditional_Ternary.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Conditional_Ternary_ShortCircuit.js` | Expression-level branching with both arms coerced to object where needed; only the selected arm is evaluated. Verified via generator and execution tests in ControlFlow subgroup. | 13.5.16 |


### [Assignment Operators](https://tc39.es/ecma262/#sec-assignment-operators)

#### [Simple Assignment (=)](https://tc39.es/ecma262/#sec-assignment-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Property assignment on objects (obj.prop = value) | Supported | `Js2IL.Tests/Literals/JavaScript/ObjectLiteral_PropertyAssign.js` | Emitted as a dynamic SetProperty call for non-computed MemberExpression targets. Supports ExpandoObject (object literal) and reflection-backed host objects; arrays/typed arrays ignore arbitrary dot properties. | 13.15.1 |


### [Assignment Operators](https://tc39.es/ecma262/#sec-assignment-operators)

#### [Assignment Operators (+=, -=, ...)](https://tc39.es/ecma262/#sec-assignment-operators)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Compound assignment += with strings | Supported | `Js2IL.Tests/String/JavaScript/String_PlusEquals_Append.js` | += on identifiers uses JavaScriptRuntime.Operators.Add for JS coercion and stores back to the same binding; validated by generator snapshot. | 13.15.2 |
| Bitwise compound assignments (\|=, &=, ^=, <<=, >>=, >>>=) | Supported | `Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_BitwiseOrAssignment.js`<br>`Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_BitwiseAndAssignment.js`<br>`Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_BitwiseXorAssignment.js`<br>`Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_LeftShiftAssignment.js`<br>`Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_RightShiftAssignment.js`<br>`Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_UnsignedRightShiftAssignment.js` | Emits load-convert-operate-convert-store pattern with int32 operations. Operands are converted to int32, operation applied, result converted back to double for storage. | 13.15.2 |
| Arithmetic compound assignments (-=, *=, /=, %=, **=) | Supported | `Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_SubtractionAssignment.js`<br>`Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_MultiplicationAssignment.js`<br>`Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_DivisionAssignment.js`<br>`Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_RemainderAssignment.js`<br>`Js2IL.Tests/CompoundAssignment/JavaScript/CompoundAssignment_ExponentiationAssignment.js` | Arithmetic operations work on double values. Exponentiation (**=) uses System.Math.Pow. All operators properly preserve JavaScript numeric semantics. | 13.15.2 |


## [ECMAScript Language: Statements and Declarations](https://tc39.es/ecma262/#sec-ecmascript-language-statements-and-declarations)

### [Return Statement](https://tc39.es/ecma262/#sec-return-statement)

#### [Runtime Semantics: ReturnStatement Evaluation](https://tc39.es/ecma262/#sec-return-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| return statement (basic) | Supported | `Js2IL.Tests/Function/JavaScript/Function_ReturnsStaticValueAndLogs.js` | Function returns propagate values (boxed) to callers; validated by execution snapshot showing returned value. | 14.1.1 |


### [The if Statement](https://tc39.es/ecma262/#sec-if-statement)

#### [Runtime Semantics: Evaluation](https://tc39.es/ecma262/#sec-if-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| if statement (LessThan) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_LessThan.js` |  | 14.6.2 |
| if statement (!flag) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_NotFlag.js` | Logical not in conditional test supported. | 14.6.2 |
| if statement (result == true) | Supported | `Js2IL.Tests/Function/JavaScript/Function_IsEven_CompareResultToTrue.js` | Compares function-returned boolean to true and branches accordingly. | 14.6.2 |
| if condition truthiness (non-boolean) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_If_Truthiness.js` | Conditions like if (url) are coerced via JS ToBoolean semantics (empty string/0/NaN/undefined/null => false; others => true). | 14.6.2 |


### [The do-while Statement](https://tc39.es/ecma262/#sec-do-while-statement)

#### [Runtime Semantics: DoWhileStatement Evaluation](https://tc39.es/ecma262/#sec-do-while-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| do-while loop (CountDownFromFive) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_CountDownFromFive.js` |  | 14.7.1.1 |
| do-while loop: continue (skip even) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_Continue_SkipEven.js` | continue branches to the post-body test point (LoopContext). | 14.7.1.1 |
| do-while loop: break | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_Break_AtThree.js` | break branches to loop end (LoopContext). | 14.7.1.1 |
| do-while loop: labeled continue | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledContinue.js` | Supports continue <label> where <label> targets an enclosing loop. | 14.7.1.1 |
| do-while loop: labeled break | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_DoWhile_LabeledBreak.js` | Supports break <label> where <label> targets an enclosing loop. | 14.7.1.1 |


### [The while Statement](https://tc39.es/ecma262/#sec-while-statement)

#### [Runtime Semantics: WhileStatement Evaluation](https://tc39.es/ecma262/#sec-while-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| while loop (CountDownFromFive) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_CountDownFromFive.js` |  | 14.7.2.1 |
| while loop: continue (skip even) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_Continue_SkipEven.js` | continue branches to loop head (LoopContext). | 14.7.2.1 |
| while loop: break | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_Break_AtThree.js` | break branches to loop end (LoopContext). | 14.7.2.1 |
| while loop: labeled continue | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledContinue.js` | Supports continue <label> where <label> targets an enclosing loop. | 14.7.2.1 |
| while loop: labeled break | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_While_LabeledBreak.js` | Supports break <label> where <label> targets an enclosing loop. | 14.7.2.1 |


### [The for Statement](https://tc39.es/ecma262/#sec-for-statement)

#### [Runtime Semantics: ForLoopEvaluation](https://tc39.es/ecma262/#sec-runtime-semantics-forloopevaluation)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| for loop (CountToFive) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_CountToFive.js` |  | 14.7.4.2 |
| for loop (CountDownFromFive) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_CountDownFromFive.js` |  | 14.7.4.2 |
| for loop (LessThanOrEqual) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LessThanOrEqual.js` |  | 14.7.4.2 |
| for loop (GreaterThanOrEqual) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_GreaterThanOrEqual.js` |  | 14.7.4.2 |
| for loop: continue | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_Continue_SkipEven.js` | Implements continue by branching to the update expression (LoopContext). | 14.7.4.2 |
| for loop: break | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_Break_AtThree.js` | Implements break by branching to loop end label (LoopContext). | 14.7.4.2 |
| for loop: labeled continue | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledContinue.js` | Supports continue <label> where <label> targets an enclosing loop. | 14.7.4.2 |
| for loop: labeled break | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForLoop_LabeledBreak.js` | Supports break <label> where <label> targets an enclosing loop. | 14.7.4.2 |


### [The for-of Statement](https://tc39.es/ecma262/#sec-for-in-and-for-of-statements)

#### [Runtime Semantics: ForInOfBodyEvaluation (for-of)](https://tc39.es/ecma262/#sec-runtime-semantics-forinofbodyevaluation)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| for-of over arrays (enumerate values) | Partially Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Array_Basic.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Continue.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_Break.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledContinue.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForOf_LabeledBreak.js` | Lowered to an index loop over a normalized iterable (JavaScriptRuntime.Object.NormalizeForOfIterable), then accessed via JavaScriptRuntime.Object.GetLength(object) + GetItem(object, double). Supports arrays, strings, typed arrays, and .NET IEnumerable (via Array.from), but does not implement full JS iterator protocol (Symbol.iterator). | 14.7.5.1 |


#### [Runtime Semantics: ForInOfBodyEvaluation (for-in)](https://tc39.es/ecma262/#sec-runtime-semantics-forinofbodyevaluation)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| for-in over objects (enumerate enumerable keys) | Partially Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Object_Basic.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Continue.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_Break.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledContinue.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_ForIn_LabeledBreak.js` | Lowered to an index loop over JavaScriptRuntime.Object.GetEnumerableKeys(object). Minimal semantics: supports ExpandoObject (object literals), JS Array/Int32Array/string index keys, and IDictionary keys; does not currently model full prototype-chain enumeration rules. | 14.7.5.2 |


### [The switch Statement](https://tc39.es/ecma262/#sec-switch-statement)

#### [Runtime Semantics: SwitchStatement Evaluation](https://tc39.es/ecma262/#sec-switch-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| switch statement (cases, fallthrough, default, break) | Supported | `Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_Fallthrough.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_DefaultInMiddle_Fallthrough.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_MultiCaseSharedBody.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_NestedBreak.js`<br>`Js2IL.Tests/ControlFlow/JavaScript/ControlFlow_Switch_LabeledBreak.js` | Supports fallthrough semantics, default placement, multiple case labels sharing a body, nested switch break behavior, and labeled break out of a switch. | 14.12.1 |


### [The try Statement](https://tc39.es/ecma262/#sec-try-statement)

#### [Runtime Semantics: TryStatement Evaluation](https://tc39.es/ecma262/#sec-try-statement)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| throw statement | Supported | `Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding.js`<br>`Js2IL.Tests/TryCatch/JavaScript/TryCatchFinally_ThrowValue.js` | Supports throwing any JS value. Non-Exception values are wrapped in JavaScriptRuntime.JsThrownValueException; catch unwrapping binds the original value. JavaScriptRuntime.Error is thrown directly. | 14.16.1 |
| try/catch (no binding) | Supported | `Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding.js`<br>`Js2IL.Tests/TryCatch/JavaScript/TryCatch_NoBinding_NoThrow.js` | Catch blocks handle values thrown within the try region (including non-Exception JS values via JsThrownValueException) and bind the caught value only when a binding is present. | 14.16.1 |
| try/catch (with binding; block-scoped catch parameter) | Supported | `Js2IL.Tests/TryCatch/JavaScript/TryCatch_ScopedParam.js` | Catch parameter binding is block-scoped to the catch clause and does not leak outside the catch block. | 14.16.1 |
| try/catch/finally | Supported | `Js2IL.Tests/TryCatch/JavaScript/TryCatchFinally_ThrowValue.js` | Supports catch + finally with correct finally execution and catch binding when throwing arbitrary JS values. | 14.16.1 |
| try/finally (no catch) | Partially Supported | `Js2IL.Tests/TryCatch/JavaScript/TryFinally_NoCatch.js`<br>`Js2IL.Tests/TryCatch/JavaScript/TryFinally_NoCatch_Throw.js`<br>`Js2IL.Tests/TryCatch/JavaScript/TryFinally_Return.js` | Finally emission is in place and executes on normal and return exits. Execution test for unhandled throw is skipped pending top-level unhandled Error semantics; generator snapshot verifies structure. | 14.16.1 |


## [ECMAScript Language: Classes](https://tc39.es/ecma262/#sec-ecmascript-language-classes)

### [Class Definitions](https://tc39.es/ecma262/#sec-class-definitions)

#### [Basic class features](https://tc39.es/ecma262/#sec-class-definitions)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Class declaration (empty) | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_DeclareEmptyClass.js` |  | 15.1.1 |
| Instance method (declare and call) | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_ClassWithMethod_HelloWorld.js` |  | 15.1.1 |
| Static method (declare and call) | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_ClassWithStaticMethod_HelloWorld.js` |  | 15.1.1 |
| Instance field initializer (public property default) | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_ClassProperty_DefaultAndLog.js` | Emitted by assigning defaults in the generated .ctor. | 15.1.1 |
| Static field initializer (static property default) | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_ClassWithStaticProperty_DefaultAndLog.js` | Emitted as a static field initialized in a synthesized .cctor; accessed via ldsfld. | 15.1.1 |
| Constructor with parameter and this.field assignment; method reads field | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_Param_Field_Log.js` |  | 15.1.1 |
| Constructor with multiple parameters; method uses fields | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_WithMultipleParameters.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_TwoParams_AddMethod.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_TwoParams_SubtractMethod.js` | Covers multi-parameter constructors and arithmetic in instance methods. | 15.1.1 |
| Private instance field (#) with helper method access | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_ClassPrivateField_HelperMethod_Log.js` | Generated as a private .NET field with a mangled name; accessible only within the class. | 15.1.1 |
| Class methods and constructors accessing ancestor scope variables (global, function, block) | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_AccessGlobalVariable_Log.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_AccessFunctionVariable_Log.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_AccessFunctionVariableAndGlobalVariable_Log.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_AccessArrowFunctionVariable_Log.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_AccessArrowFunctionVariableAndGlobalVariable_Log.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_AccessGlobalVariable_Log.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_AccessFunctionVariable_Log.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_AccessFunctionVariableAndGlobalVariable_Log.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_AccessGlobalVariableAndParameterValue_Log.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_AccessFunctionVariableAndParameterValue_Log.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_AccessFunctionVariableAndGlobalVariableAndParameterValue_Log.js` | Class methods and constructors can now access variables from all ancestor scopes (global, function, block), not just the global scope. Implementation walks the scope tree from the class's parent scope to root, building a complete ancestor chain. The scope array is passed as an argument to methods/constructors, enabling proper multi-level scope access. Supports regular functions, arrow functions, and constructor parameters. Fixed parameter indexing for constructors: when scopes are present, arg0=this, arg1=scopes[], parameters start at arg2. | 15.1.1 |
| Constructor default parameters | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_DefaultParameterValue_Constructor.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassConstructor_ParameterDestructuring.js` | Class constructors support default parameter values and destructuring with defaults. Call sites validate argument count against min/max parameter bounds and pad missing optional arguments with ldnull. Default values are applied using starg IL pattern when arguments are null. Object destructuring parameters support default values (e.g., constructor({host="localhost", port=8080})) with conditional IL generation. | 15.1.1 |
| Instance method default parameters | Supported | `Js2IL.Tests/Classes/JavaScript/Classes_DefaultParameterValue_Method.js`<br>`Js2IL.Tests/Classes/JavaScript/Classes_ClassMethod_ParameterDestructuring.js` | Class instance methods support default parameter values and destructuring with defaults. Methods are registered in ClassRegistry with min/max parameter counts. Call sites validate argument count ranges and pad missing optional parameters with ldnull before callvirt. Object destructuring parameters support default values with conditional IL generation. | 15.1.1 |


## [The Math Object](https://tc39.es/ecma262/#sec-math-object)

### [Value Properties of the Math Object](https://tc39.es/ecma262/#sec-value-properties-of-the-math-object)

#### [Math constants](https://tc39.es/ecma262/#sec-value-properties-of-the-math-object)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.E | Supported |  | Euler’s number e. | 20.2.1 |
| Math.LN10 | Supported |  | Natural logarithm of 10. | 20.2.1 |
| Math.LN2 | Supported |  | Natural logarithm of 2. | 20.2.1 |
| Math.LOG10E | Supported |  | Base-10 logarithm of e. | 20.2.1 |
| Math.LOG2E | Supported |  | Base-2 logarithm of e. | 20.2.1 |
| Math.PI | Supported |  | Ratio of a circle’s circumference to its diameter. | 20.2.1 |
| Math.SQRT1_2 | Supported |  | Square root of 1/2. | 20.2.1 |
| Math.SQRT2 | Supported |  | Square root of 2. | 20.2.1 |


### [Function Properties of the Math Object](https://tc39.es/ecma262/#sec-function-properties-of-the-math-object)

#### [Math.abs](https://tc39.es/ecma262/#sec-math.abs)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.abs(x) | Supported |  | Returns the absolute value; NaN propagates; ±Infinity preserved. | 20.2.2.1 |


#### [Math.acos](https://tc39.es/ecma262/#sec-math.acos)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.acos(x) | Supported |  | Returns arc cosine in radians; out-of-domain yields NaN. | 20.2.2.2 |


#### [Math.acosh](https://tc39.es/ecma262/#sec-math.acosh)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.acosh(x) | Supported |  | Inverse hyperbolic cosine; x < 1 yields NaN; Infinity preserved. | 20.2.2.3 |


#### [Math.asin](https://tc39.es/ecma262/#sec-math.asin)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.asin(x) | Supported |  | Returns arc sine in radians; out-of-domain yields NaN. | 20.2.2.4 |


#### [Math.asinh](https://tc39.es/ecma262/#sec-math.asinh)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.asinh(x) | Supported |  | Inverse hyperbolic sine; handles ±0, NaN, ±Infinity per spec. | 20.2.2.5 |


#### [Math.atan](https://tc39.es/ecma262/#sec-math.atan)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.atan(x) | Supported |  | Returns arc tangent in radians; NaN propagates; ±Infinity maps to ±π/2. | 20.2.2.6 |


#### [Math.atan2](https://tc39.es/ecma262/#sec-math.atan2)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.atan2(y, x) | Supported |  | Quadrant-aware arc tangent; handles zeros, NaN, and infinities per spec. | 20.2.2.7 |


#### [Math.ceil](https://tc39.es/ecma262/#sec-math.ceil)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.ceil(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Ceil_Sqrt_Basic.js` | Implements ceiling for numbers represented as double; arguments coerced via minimal ToNumber semantics. Returns NaN for NaN/undefined or negative zero preserved via .NET semantics. | 20.2.2.9 |


#### [Math.clz32](https://tc39.es/ecma262/#sec-math.clz32)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.clz32(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Imul_Clz32_Basics.js` | Counts leading zero bits in the 32-bit unsigned integer representation. | 20.2.2.10 |


#### [Math.cos](https://tc39.es/ecma262/#sec-math.cos)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.cos(x) | Supported |  | Cosine of x (radians); NaN propagates; Infinity yields NaN. | 20.2.2.11 |


#### [Math.cosh](https://tc39.es/ecma262/#sec-math.cosh)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.cosh(x) | Supported |  | Hyperbolic cosine; handles ±0, NaN, ±Infinity per spec. | 20.2.2.12 |


#### [Math.exp](https://tc39.es/ecma262/#sec-math.exp)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.exp(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Log_Exp_Identity.js` | e^x; consistent with JS semantics for NaN and infinities. | 20.2.2.13 |


#### [Math.expm1](https://tc39.es/ecma262/#sec-math.expm1)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.expm1(x) | Supported |  | Returns e^x - 1 with improved precision for small x. | 20.2.2.14 |


#### [Math.floor](https://tc39.es/ecma262/#sec-math.floor)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.floor(x) | Supported |  | Largest integer less than or equal to x; preserves -0 when appropriate. | 20.2.2.15 |


#### [Math.fround](https://tc39.es/ecma262/#sec-math.fround)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.fround(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Fround_SignedZero.js` | Rounds to nearest 32-bit float; preserves signed zero. | 20.2.2.16 |


#### [Math.hypot](https://tc39.es/ecma262/#sec-math.hypot)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.hypot(...values) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Hypot_Infinity_NaN.js` | Computes sqrt(sum(x_i^2)); returns Infinity if any arg is ±Infinity; NaN if any arg is NaN and none are Infinity. | 20.2.2.17 |


#### [Math.imul](https://tc39.es/ecma262/#sec-math.imul)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.imul(a, b) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Imul_Clz32_Basics.js` | C-style 32-bit integer multiplication with wrapping. | 20.2.2.18 |


#### [Math.log](https://tc39.es/ecma262/#sec-math.log)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.log(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Log_Exp_Identity.js` | Natural logarithm; log(1) = 0; negative x yields NaN; log(0) = -Infinity. | 20.2.2.19 |


#### [Math.log10](https://tc39.es/ecma262/#sec-math.log10)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.log10(x) | Supported |  | Base-10 logarithm; JS semantics for 0, negatives, NaN, and infinities. | 20.2.2.20 |


#### [Math.log1p](https://tc39.es/ecma262/#sec-math.log1p)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.log1p(x) | Supported |  | log(1 + x) with improved precision for small x. | 20.2.2.21 |


#### [Math.log2](https://tc39.es/ecma262/#sec-math.log2)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.log2(x) | Supported |  | Base-2 logarithm; JS semantics for 0, negatives, NaN, and infinities. | 20.2.2.22 |


#### [Math.max](https://tc39.es/ecma262/#sec-math.max)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.max(...values) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Min_Max_NaN_EmptyArgs.js` | Returns the largest of the given numbers; with no arguments returns -Infinity; if any argument is NaN returns NaN. | 20.2.2.23 |


#### [Math.sqrt](https://tc39.es/ecma262/#sec-math.sqrt)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.sqrt(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Ceil_Sqrt_Basic.js` | Returns the square root for non-negative inputs; negative or NaN yields NaN; Infinity maps to Infinity. | 20.2.2.24 |


#### [Math.pow](https://tc39.es/ecma262/#sec-math.pow)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.pow(x, y) | Supported |  | Exponentiation; consistent with JS semantics including NaN and Infinity cases. | 20.2.2.25 |


#### [Math.random](https://tc39.es/ecma262/#sec-math.random)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.random() | Supported |  | Returns a pseudo-random number in the range [0, 1). | 20.2.2.26 |


#### [Math.round](https://tc39.es/ecma262/#sec-math.round)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.round(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Round_Trunc_NegativeHalves.js` | Rounds to the nearest integer; ties at .5 round up toward +∞; exact -0.5 returns -0. | 20.2.2.27 |


#### [Math.sign](https://tc39.es/ecma262/#sec-math.sign)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.sign(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Sign_ZeroVariants.js` | Returns 1, -1, 0, -0, or NaN depending on the sign of x; ±Infinity map to ±1. | 20.2.2.28 |


#### [Math.sin](https://tc39.es/ecma262/#sec-math.sin)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.sin(x) | Supported |  | Sine of x (radians); NaN propagates; Infinity yields NaN. | 20.2.2.29 |


#### [Math.sinh](https://tc39.es/ecma262/#sec-math.sinh)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.sinh(x) | Supported |  | Hyperbolic sine; handles ±0, NaN, ±Infinity per spec. | 20.2.2.30 |


#### [Math.tan](https://tc39.es/ecma262/#sec-math.tan)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.tan(x) | Supported |  | Tangent of x (radians); NaN propagates; Infinity yields NaN. | 20.2.2.31 |


#### [Math.tanh](https://tc39.es/ecma262/#sec-math.tanh)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.tanh(x) | Supported |  | Hyperbolic tangent; handles ±0, NaN, ±Infinity per spec. | 20.2.2.32 |


#### [Math.trunc](https://tc39.es/ecma262/#sec-math.trunc)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.trunc(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Round_Trunc_NegativeHalves.js` | Removes fractional part; preserves sign for zero (can return -0). | 20.2.2.33 |


#### [Math.min](https://tc39.es/ecma262/#sec-math.min)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.min(...values) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Min_Max_NaN_EmptyArgs.js` | Returns the smallest of the given numbers; with no arguments returns Infinity; if any argument is NaN returns NaN. | 20.2.2.34 |


#### [Math.cbrt](https://tc39.es/ecma262/#sec-math.cbrt)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Math.cbrt(x) | Supported | `Js2IL.Tests/Math/JavaScript/Math_Cbrt_Negative.js` | Cube root; handles negative values returning negative result; NaN propagates; Infinity preserved. | 20.2.2.35 |


## [Date Objects](https://tc39.es/ecma262/#sec-date-objects)

### [The Date Constructor](https://tc39.es/ecma262/#sec-date-constructor)

#### [Date constructor](https://tc39.es/ecma262/#sec-date-constructor)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| new Date() (current time) | Supported | `Js2IL.Tests/Date/ExecutionTests.cs` | Constructs a Date representing now (UTC). Stores milliseconds since Unix epoch internally. | 21.4.1 |
| new Date(milliseconds) | Supported | `Js2IL.Tests/Date/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js` | Constructs from milliseconds since Unix epoch; numeric argument is coerced per JS ToNumber minimal behavior. | 21.4.1 |


### [Properties of the Date Constructor](https://tc39.es/ecma262/#sec-properties-of-the-date-constructor)

#### [Date.now](https://tc39.es/ecma262/#sec-date.now)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Date.now() | Supported | `Js2IL.Tests/Date/ExecutionTests.cs` | Returns current time in milliseconds since Unix epoch as a number (boxed double). | 21.4.2.1 |


#### [Date.parse](https://tc39.es/ecma262/#sec-date.parse)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Date.parse(string) | Supported | `Js2IL.Tests/Date/ExecutionTests.cs`<br>`Js2IL.Tests/Date/JavaScript/Date_Parse_IsoString.js` | Parses an ISO-like string to milliseconds since Unix epoch, or NaN on failure; returns a number (boxed double). | 21.4.2.2 |


### [Properties of the Date Prototype Object](https://tc39.es/ecma262/#sec-properties-of-the-date-prototype-object)

#### [Date.prototype.getTime](https://tc39.es/ecma262/#sec-date.prototype.gettime)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Date.prototype.getTime | Supported | `Js2IL.Tests/Date/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js` | Returns milliseconds since Unix epoch as a number (boxed double). | 21.4.3.5 |


#### [Date.prototype.toISOString](https://tc39.es/ecma262/#sec-date.prototype.toisostring)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Date.prototype.toISOString | Supported | `Js2IL.Tests/Date/JavaScript/Date_Construct_FromMs_GetTime_ToISOString.js` | Returns a UTC ISO 8601 string with millisecond precision and trailing 'Z'. | 21.4.3.27 |


## [Array Objects](https://tc39.es/ecma262/#sec-array-objects)

### [Properties of Array Instances](https://tc39.es/ecma262/#sec-properties-of-array-instances)

#### [length](https://tc39.es/ecma262/#sec-properties-of-array-instances)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.length property (read) | Supported | `Js2IL.Tests/Array/JavaScript/Array_LengthProperty_ReturnsCount.js`<br>`Js2IL.Tests/Array/JavaScript/Array_EmptyLength_IsZero.js` | length getter returns number of elements; emitted via JavaScriptRuntime.Object.GetLength(object). Used by for-of implementation. | 23.1.2.1 |


#### [Array.prototype.join](https://tc39.es/ecma262/#sec-array.prototype.join)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.join | Supported | `Js2IL.Tests/Array/JavaScript/Array_Join_Basic.js` | Elements are stringified via DotNet2JSConversions.ToString and joined with a separator (default ','). Codegen dispatches to JavaScriptRuntime.Array.join(object[]). | 23.1.3.13 |


#### [Array.prototype.pop](https://tc39.es/ecma262/#sec-array.prototype.pop)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.pop | Supported | `Js2IL.Tests/Array/JavaScript/Array_Pop_Basic.js` | Removes and returns the last element; when empty returns undefined (represented as null in this runtime). | 23.1.3.20 |


#### [Array.prototype.push](https://tc39.es/ecma262/#sec-array.prototype.push)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.push | Supported | `Js2IL.Tests/Array/JavaScript/Array_Push_Basic.js` | Appends items to the end of the array and returns the new length (as a JS number). | 23.1.3.22 |


#### [Array.prototype.map](https://tc39.es/ecma262/#sec-array.prototype.map)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.map | Partially Supported | `Js2IL.Tests/Array/JavaScript/Array_Map_Basic.js` | Supports value-callback mapping including nested callback closures. thisArg and standard callback parameter injections (index, array) are not yet supported; callback currently receives only the element value. Returns a new array. | 23.1.3.25 |


#### [Array.prototype.sort](https://tc39.es/ecma262/#sec-array.prototype.sort)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.sort (default comparator) | Partially Supported | `Js2IL.Tests/Array/JavaScript/Array_Sort_Basic.js` | Default lexicographic sort implemented in JavaScriptRuntime.Array.sort(); comparator function parameter is not yet supported. Returns the array instance. | 23.1.3.27 |


#### [Array.prototype.slice](https://tc39.es/ecma262/#sec-array.prototype.slice)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.slice | Supported | `Js2IL.Tests/Array/JavaScript/Array_Slice_Basic.js` | Returns a shallow copy; handles negative indices and undefined end per spec. | 23.1.3.28 |


#### [Array.prototype.splice](https://tc39.es/ecma262/#sec-array.prototype.splice)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.prototype.splice | Supported | `Js2IL.Tests/Array/JavaScript/Array_Splice_Basic.js`<br>`Js2IL.Tests/Array/JavaScript/Array_Splice_InsertAndDelete.js` | Mutates the array by removing and/or inserting elements; returns an array of removed elements. | 23.1.3.31 |


### [Properties of the Array Constructor](https://tc39.es/ecma262/#sec-properties-of-the-array-constructor)

#### [Array.isArray](https://tc39.es/ecma262/#sec-array.isarray)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Array.isArray | Supported | `Js2IL.Tests/Array/JavaScript/Array_IsArray_Basic.js` | Returns true for JavaScriptRuntime.Array instances; false otherwise. | 23.1.2 |


### [TypedArray Objects](https://tc39.es/ecma262/#sec-typedarray-objects)

#### [Constructor and basic semantics](https://tc39.es/ecma262/#sec-typedarray-objects)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Int32Array: new Int32Array(length), length, index get/set (basic) | Supported | `Js2IL.Tests/TypedArray/JavaScript/Int32Array_Construct_Length.js`<br>`Js2IL.Tests/TypedArray/Snapshots/ExecutionTests.Int32Array_Construct_Length.verified.txt` | Minimal typed array: exposes numeric length, supports indexing; out-of-bounds reads return 0 and out-of-bounds writes are ignored. | 23.2.1.1 |
| Int32Array from array-like (copy and ToInt32 coercion) | Supported | `Js2IL.Tests/TypedArray/JavaScript/Int32Array_FromArray_CopyAndCoerce.js`<br>`Js2IL.Tests/TypedArray/Snapshots/ExecutionTests.Int32Array_FromArray_CopyAndCoerce.verified.txt` | Copies from array-like and coerces values using ToInt32-style truncation; NaN/Infinity/±0 become 0. | 23.2.1.1 |
| Int32Array.prototype.set(source[, offset]) | Supported | `Js2IL.Tests/TypedArray/JavaScript/Int32Array_Set_FromArray_WithOffset.js`<br>`Js2IL.Tests/TypedArray/Snapshots/ExecutionTests.Int32Array_Set_FromArray_WithOffset.verified.txt` | Copies elements from array-like or another Int32Array with optional offset; elements are coerced via ToInt32; offset < 0 treated as 0; copies stop at destination length. | 23.2.1.1 |


## [Text Processing](https://tc39.es/ecma262/#sec-text-processing)

### [String Objects](https://tc39.es/ecma262/#sec-string-objects)

#### [String.prototype.startsWith](https://tc39.es/ecma262/#sec-string.prototype.startswith)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| String.prototype.startsWith | Supported | `Js2IL.Tests/String/JavaScript/String_StartsWith_Basic.js` | Reflection-based string dispatch routes CLR string receivers to JavaScriptRuntime.String.StartsWith with optional position argument. Returns a boolean value (boxed). | 24.1.3 |


#### [String.prototype.includes](https://tc39.es/ecma262/#sec-string.prototype.includes)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| String.prototype.includes | Supported |  | Reflection-based dispatch recognizes definite string receivers and routes to JavaScriptRuntime.String.Includes; supports optional position argument. Returns a boolean value. (No dedicated JS fixture currently referenced in this doc.) | 24.1.3 |


#### [String.prototype.endsWith](https://tc39.es/ecma262/#sec-string.prototype.endswith)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| String.prototype.endsWith | Supported |  | Implemented in JavaScriptRuntime.String and wired via IL generator for definite string receivers. Supports optional end position. Returns a boolean value. (No dedicated JS fixture currently referenced in this doc.) | 24.1.3 |


#### [String.prototype.split](https://tc39.es/ecma262/#sec-string.prototype.split)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| String.prototype.split | Supported | `Js2IL.Tests/String/JavaScript/String_Split_Basic.js` | Supports string and regular-expression separators and optional limit. Implemented via JavaScriptRuntime.String.Split and returned as JavaScriptRuntime.Array. Separator omitted or undefined returns [input]. Empty string separator splits into individual UTF-16 code units. | 24.1.3 |


#### [String.prototype.replace](https://tc39.es/ecma262/#sec-string.prototype.replace)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| String.prototype.replace (regex literal, string replacement) | Partially Supported | `Js2IL.Tests/String/JavaScript/String_Replace_Regex_Global.js` | Supported when the receiver is String(x), the pattern is a regular expression literal, and the replacement is a string. Global (g) and ignoreCase (i) flags are honored. Function replacement, non-regex patterns, and other flags are not yet implemented. Implemented via host intrinsic JavaScriptRuntime.String.Replace and dynamic resolution in IL generator. | 24.1.3 |


#### [String.prototype.localeCompare](https://tc39.es/ecma262/#sec-string.prototype.localecompare)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| String.prototype.localeCompare (numeric compare) | Supported | `Js2IL.Tests/String/JavaScript/String_LocaleCompare_Numeric.js` | Returns a number (boxed double) consistent with ECMAScript compare semantics; numeric option supported. | 24.1.4 |


### [JSON Object](https://tc39.es/ecma262/#sec-json-object)

#### [JSON.parse](https://tc39.es/ecma262/#sec-json.parse)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| JSON.parse | Partially Supported | `Js2IL.Tests/JSONRuntimeTests.cs` | Implemented via host intrinsic JavaScriptRuntime.JSON.Parse(string). Maps invalid input to SyntaxError and non-string input to TypeError. Reviver parameter is not supported. Objects become ExpandoObject, arrays use JavaScriptRuntime.Array, numbers use double. | 24.5.1 |


## [Keyed Collections](https://tc39.es/ecma262/#sec-keyed-collections)

### [Set Objects](https://tc39.es/ecma262/#sec-set-objects)

#### [Properties of Set Instances](https://tc39.es/ecma262/#sec-properties-of-set-instances)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Set.prototype.add | Supported |  | Backed by JavaScriptRuntime.Set.add; returns the Set instance to allow chaining. (No dedicated JS fixture currently referenced in this doc.) | 25.4.5 |
| Set.prototype.has | Supported |  | Backed by JavaScriptRuntime.Set.has; strict equality for keys based on .NET object identity and string/double value semantics. (No dedicated JS fixture currently referenced in this doc.) | 25.4.5 |
| Set.prototype.size (getter) | Supported |  | Exposed via a 'size' property on JavaScriptRuntime.Set returning a JS number (double). (No dedicated JS fixture currently referenced in this doc.) | 25.4.5 |


## [Promise Object (Host/runtime)](https://tc39.es/ecma262/#sec-promise-objects)

### [Promise constructor and instance methods](https://tc39.es/ecma262/#sec-promise-constructor)

#### [Promise constructor / resolve / reject](https://tc39.es/ecma262/#sec-promise-constructor)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Promise constructor (executor), Promise.resolve, Promise.reject | Supported | `Js2IL.Tests/Promise/JavaScript/Promise_Executor_Resolved.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Executor_Rejected.js` | Constructor accepts an executor delegate and supports the basic resolve/reject fast-paths and dynamic delegate invocation used in tests. Promise.resolve/reject create already-settled Promise instances. | 27.1.1 |


#### [Promise.prototype.then / catch / finally](https://tc39.es/ecma262/#sec-promise.prototype.then)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Promise.prototype.then / catch / finally | Supported | `Js2IL.Tests/Promise/JavaScript/Promise_Resolve_Then.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Reject_Then.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Resolve_ThenFinally.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Reject_FinallyCatch.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Resolve_FinallyThen.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Resolve_FinallyThrows.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Then_ReturnsResolvedPromise.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Then_ReturnsRejectedPromise.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Catch_ReturnsResolvedPromise.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Catch_ReturnsRejectedPromise.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Finally_ReturnsResolvedPromise.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Finally_ReturnsRejectedPromise.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Scheduling_StarvationTest.js` | Implements `then`, `catch`, and `finally` with microtask scheduling support. `finally` handlers are treated as observers: non-Promise return values do not alter the settled result, while returned Promises are awaited and propagated (fixed earlier bug where Promise returns from finally were masked). Tests include chaining and then/catch/finally interactions. | 27.1.2 |


#### [Promise.all / Promise.allSettled / Promise.any / Promise.race](https://tc39.es/ecma262/#sec-promise.all)

| Feature | Status | Test Scripts | Notes | Section |
|---|---|---|---|---|
| Promise.all | Supported | `Js2IL.Tests/Promise/JavaScript/Promise_All_AllResolved.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_All_OneRejected.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_All_EmptyArray.js` | Returns a Promise that resolves when all input promises resolve (with an array of results), or rejects when any input promise rejects (with the first rejection reason). | 27.1.3 |
| Promise.allSettled | Supported | `Js2IL.Tests/Promise/JavaScript/Promise_AllSettled_MixedResults.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_AllSettled_AllResolved.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_AllSettled_AllRejected.js` | Returns a Promise that resolves when all input promises have settled (fulfilled or rejected), with an array of outcome objects containing status and value/reason. | 27.1.3 |
| Promise.any | Supported | `Js2IL.Tests/Promise/JavaScript/Promise_Any_FirstResolved.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Any_AllRejected.js` | Returns a Promise that resolves as soon as any input promise resolves, or rejects with an AggregateError if all input promises reject. | 27.1.3 |
| Promise.race | Supported | `Js2IL.Tests/Promise/JavaScript/Promise_Race_FirstResolved.js`<br>`Js2IL.Tests/Promise/JavaScript/Promise_Race_FirstRejected.js` | Returns a Promise that settles as soon as any input promise settles (resolves or rejects), with the same value or reason. | 27.1.3 |

